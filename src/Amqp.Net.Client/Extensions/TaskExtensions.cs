﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Amqp.Net.Client.Extensions
{
    internal static class TaskExtensions
    {
        internal static Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> task,
                                                                   Func<TResult, TNewResult> func)
        {
            if (task.Exception != null)
                throw task.Exception.GetBaseException();

            return task.ContinueWith(_ => func(_.Result));
        }

        internal static Task<TNewResult> Then<TResult, TNewResult>(this Task<TResult> task,
                                                                   Func<TResult, Task<TNewResult>> func)
        {
            if (task.Exception != null)
                throw task.Exception.GetBaseException();

            return task.ContinueWith(_ => func(_.Result))
                       .Unwrap();
        }

        internal static Task<TNewResult> Then<TNewResult>(this Task task,
                                                          Func<TNewResult> func)
        {
            if (task.Exception != null)
                throw task.Exception.GetBaseException();

            return task.ContinueWith(_ => func());
        }

        internal static Task<TResult> LogError<TResult>(this Task<TResult> task)
        {
            task.Exception?.GetBaseException().Log();

            return task;
        }

        internal static Task LogError(this Task task)
        {
            task.Exception?.GetBaseException().Log();

            return task;
        }

        internal static void Log(this Exception exception)
        {
            WriteEntry(LogLevel.Error, $"{exception.Message}\n{exception.StackTrace}");
        }

        internal static Task<TResult> Log<TResult>(this Task<TResult> task,
                                                   Func<TResult, String> func,
                                                   LogLevel level = LogLevel.Debug)
        {
            if (task.Exception == null)
                WriteEntry(level, func(task.Result));

            return task;
        }

        private static void WriteEntry(LogLevel level, String message)
        {
            var entry = $"[{Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(3, '0')}][{level.ToString().ToUpperInvariant().PadRight(7, ' ')}]:{message}";
            Debug.WriteLine(entry);
            WriteToConsole(level, entry);
        }

        private static readonly IDictionary<LogLevel, ConsoleColor> ColorMap = new Dictionary<LogLevel, ConsoleColor>
                                                                                   {
                                                                                       { LogLevel.Debug, ConsoleColor.Cyan },
                                                                                       { LogLevel.Information, ConsoleColor.White },
                                                                                       { LogLevel.Warning, ConsoleColor.Yellow },
                                                                                       { LogLevel.Trace, ConsoleColor.Blue },
                                                                                       { LogLevel.Critical, ConsoleColor.DarkRed }
                                                                                   };

        private static void WriteToConsole(LogLevel level, String message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ColorMap[level];
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }
    }
}