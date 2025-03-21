﻿namespace Code.Common.Logger.Service
{
    public interface ILoggerService
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}