using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyScheduler.Abstractions;
using EasyScheduler.Infrastructure;
using EasyScheduler.Models;
using EasyScheduler.Processor;
using EasyScheduler.Processor.States;

namespace EasyScheduler
{
    public abstract class BasePublishMessageSender : IPublishMessageSender, IPublishExecutor
    {
        private readonly IStorageConnection _connection;
        private readonly SchedulerOptions _options;
        private readonly IStateChanger _stateChanger;
        private readonly IContentSerializer _contentSerializer;

        protected BasePublishMessageSender(
            SchedulerOptions options,
            IStorageConnection connection,
            IContentSerializer contentSerializer,
            IStateChanger stateChanger)
        {
            _options = options;
            _connection = connection;
            _stateChanger = stateChanger;
            _contentSerializer = contentSerializer;
        }

        public abstract Task<OperateResult> PublishAsync(string keyName, string content, IDictionary<string, object> headers);

        public async Task<OperateResult> SendAsync(PublishedMessage message)
        {
            bool retry;
            OperateResult result;
            do
            {
                var executedResult = await SendWithoutRetryAsync(message);
                result = executedResult.Item2;
                if (result == OperateResult.Success)
                {
                    return result;
                }
                retry = executedResult.Item1;
            } while (retry);

            return result;
        }

        private async Task<(bool, OperateResult)> SendWithoutRetryAsync(PublishedMessage message)
        {

            var value = _contentSerializer.DeSerialize<RQMessage>(message.Content);

            var sendValues = _contentSerializer.Serialize(value.Value);

            var headers = new Dictionary<string, object>();
            foreach (var item in value.Headers)
            {
                headers.Add(item.Key, item.Value);
            }

            var result = await PublishAsync(message.Name, sendValues, headers);

            if (result.Succeeded)
            {
                await SetSuccessfulState(message);
                return (false, OperateResult.Success);
            }

            var needRetry = await SetFailedState(message, result.Exception);
            return (needRetry, OperateResult.Failed(result.Exception));
        }


        private Task SetSuccessfulState(PublishedMessage message)
        {
            var succeededState = new SucceededState(_options.CompletedJobExpiredAfter);
            return _stateChanger.ChangeStateAsync(message, succeededState, _connection);
        }

        private async Task<bool> SetFailedState(PublishedMessage message, Exception ex)
        {
            AddErrorReasonToContent(message, ex);

            var needRetry = UpdateMessageForRetry(message);

            await _stateChanger.ChangeStateAsync(message, new FailedState(), _connection);

            return needRetry;
        }

        private static void AddErrorReasonToContent(PublishedMessage message, Exception exception)
        {
            message.Content = Helper.AddExceptionProperty(message.Content, exception);
        }

        private bool UpdateMessageForRetry(PublishedMessage message)
        {
            var retryBehavior = RetryBehavior.DefaultRetry;

            var retries = ++message.Retries;
            message.ExpiresAt = message.Added.AddSeconds(retryBehavior.RetryIn(retries));

            var retryCount = Math.Min(_options.FailedRetryCount, retryBehavior.RetryCount);
            if (retries >= retryCount)
            {
                if (retries == _options.FailedRetryCount)
                {
                    try
                    {
                        _options.FailedThresholdCallback?.Invoke(message.Name, message.Content);

                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                return false;
            }


            return true;
        }


    }
}
