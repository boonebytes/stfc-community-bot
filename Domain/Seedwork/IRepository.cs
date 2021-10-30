﻿using System;
namespace DiscordBot.Domain.Seedwork
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
