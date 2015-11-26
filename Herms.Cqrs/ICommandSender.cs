﻿using System;

namespace Herms.Cqrs
{
    public interface ICommandSender
    {
        void Send<T>(T command) where T : Command;
    }
}