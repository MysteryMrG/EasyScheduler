﻿using System.Threading.Tasks;
using EasyScheduler.Models;

namespace EasyScheduler.Processor.States
{
    public static class StateChangerExtensions
    {
        public static async Task ChangeStateAsync(
            this IStateChanger @this, PublishedMessage message, IState state, IStorageConnection connection)
        {
            using (var transaction = connection.CreateTransaction())
            {
                @this.ChangeState(message, state, transaction);
                await transaction.CommitAsync();
            }
        }

        public static async Task ChangeStateAsync(
            this IStateChanger @this, ReceivedMessage message, IState state, IStorageConnection connection)
        {
            using (var transaction = connection.CreateTransaction())
            {
                @this.ChangeState(message, state, transaction);
                await transaction.CommitAsync();
            }
        }
    }
}
