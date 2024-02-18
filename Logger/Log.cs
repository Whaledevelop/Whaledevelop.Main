using System;
using System.Collections.Generic;
using UnityEngine;

namespace Whaledevelop
{
    public static class Log
    {
        public static ILogger Logger = new UnityLogger();
        public static LogPolicy LogPolicy = new();

        private static readonly Dictionary<LogTag, Color> _logTagsColors = new()
        {
            { LogTag.Debugging, Color.green },
            { LogTag.Important, Color.yellow }
        };

        public static void Flush()
        {
            Logger?.Flush();
        }

        private static string GetColoredMessage(string message, LogTag tag)
        {
            if (!_logTagsColors.TryGetValue(tag, out var color))
            {
                return message;
            }
            var colorHex = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{colorHex}>{message}</color>";
        }

        public static void Info(object message, LogTag tag = LogTag.Debugging)
        {
            if (LogPolicy.ShouldBeLogged(tag))
            {
                Logger?.Info(GetColoredMessage(message.ToString(), tag));
            }
        }

        public static void Info<T>(IEnumerable<T> enumerable, LogTag tag = LogTag.Debugging)
        {
            if (!LogPolicy.ShouldBeLogged(tag))
            {
                return;
            }
            foreach (var item in enumerable)
            {
                Logger?.Info(GetColoredMessage(item.ToString(), tag));
            }
        }

        // public static void Info(Action method, LogTag tag = LogTag.Debugging)
        // {
        //     if (LogPolicy.ShouldBeLogged(tag))
        //     {
        //         Logger?.Info(GetColoredMessage(nameof(method), tag));
        //     }
        // }

        public static void Info(string message, LogTag tag = LogTag.Debugging)
        {
            if (LogPolicy.ShouldBeLogged(tag))
            {
                Logger?.Info(GetColoredMessage(message, tag));
            }
        }

        public static void Warning(string message, LogTag tag = LogTag.Untagged)
        {
            if (LogPolicy.ShouldBeLogged(tag))
            {
                Logger?.Warning(message);
            }
        }

        public static void Error(string message, LogTag tag = LogTag.Untagged)
        {
            if (LogPolicy.ShouldBeLogged(tag))
            {
                Logger?.Error(message);
            }
        }

        public static void Exception(Exception exception, LogTag tag = LogTag.Untagged)
        {
            if (LogPolicy.ShouldBeLogged(tag))
            {
                Logger?.Exception(exception);
            }
        }
    }
}