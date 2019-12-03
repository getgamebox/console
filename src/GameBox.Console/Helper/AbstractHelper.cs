/*
 * This file is part of the GameBox package.
 *
 * (c) Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getgamebox/console
 */

using GameBox.Console.Exception;
using GameBox.Console.Formatter;
using System.Text.RegularExpressions;

namespace GameBox.Console.Helper
{
    /// <summary>
    /// <see cref="AbstractHelper"/> is the base class for all helper classes.
    /// </summary>
    public abstract class AbstractHelper : IHelper
    {
        /// <summary>
        /// The time formats.
        /// </summary>
        private static readonly TimeFormat[] TimeFormats =
        {
            new TimeFormat(0, "< 1 sec"),
            new TimeFormat(1, "1 sec"),
            new TimeFormat(2, "{0} secs", 1),
            new TimeFormat(60, "1 min"),
            new TimeFormat(120, "{0} mins", 60),
            new TimeFormat(3600, "1 hr"),
            new TimeFormat(7200, "{0} hrs", 3600),
            new TimeFormat(86400, "1 day"),
            new TimeFormat(172800, "{0} days", 86400),
        };

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <summary>
        /// Calculates the specified string length, excluding the length occupied by rich text.
        /// </summary>
        /// <param name="formatter">The decoration formatter.</param>
        /// <param name="str">The specified string.</param>
        /// <returns>The length without decoration.</returns>
        public static int StrlenWithoutDecoration(IOutputFormatter formatter, string str)
        {
            return RemoveDecoration(formatter, str)?.Length ?? 0;
        }

        /// <summary>
        /// Removed the string decoration.
        /// </summary>
        /// <param name="formatter">The decoration formatter.</param>
        /// <param name="str">The specified string.</param>
        /// <returns>The decorated string was removed.</returns>
        public static string RemoveDecoration(IOutputFormatter formatter, string str)
        {
            if (formatter == null || string.IsNullOrEmpty(str))
            {
                return str;
            }

            var isDecorated = formatter.Enable;
            try
            {
                formatter.Enable = false;
                str = formatter.Format(str);

                // todo: test regex \\033\\[[^m]*m
                str = Regex.Replace(str, "\\033\\[[^m]*m", string.Empty);
                return str;
            }
            finally
            {
                formatter.Enable = isDecorated;
            }
        }

        /// <summary>
        /// Format the memory. Automatically select the most appropriate display size.
        /// </summary>
        /// <param name="memory">The memory size.</param>
        /// <returns>The formatted memory.</returns>
        public static string FormatMemory(long memory)
        {
            if (memory >= 1024 * 1024 * 1024)
            {
                return (memory / 1024 / 1024 / 1024D).ToString("f1") + " GiB";
            }

            if (memory >= 1024 * 1024)
            {
                return (memory / 1024 / 1024D).ToString("f1") + " MiB";
            }

            if (memory >= 1024)
            {
                return (memory / 1024) + " KiB";
            }

            return memory + " B";
        }

        /// <summary>
        /// Format the time.
        /// </summary>
        /// <param name="ticks">The elapsed ticks.</param>
        /// <returns>The formatted time.</returns>
        public static string FormatTime(long ticks)
        {
            var second = ticks / 10000000;
            for (var index = 0; index < TimeFormats.Length; index++)
            {
                var format = TimeFormats[index];
                if (second < format.Threshold)
                {
                    continue;
                }

                if (index == TimeFormats.Length - 1 ||
                    (second < TimeFormats[index + 1].Threshold))
                {
                    return format.Unit <= 0
                        ? format.Format
                        : string.Format(format.Format, second / format.Unit);
                }
            }

            throw new ConsoleException($"{nameof(FormatTime)} assert exception");
        }

        /// <summary>
        /// The time format struct.
        /// </summary>
        private struct TimeFormat
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TimeFormat"/> struct.
            /// </summary>
            public TimeFormat(long threshold, string format, long unit = 0)
            {
                Threshold = threshold;
                Format = format;
                Unit = unit;
            }

            /// <summary>
            /// Gets format validation threshold.
            /// </summary>
            public long Threshold { get; }

            /// <summary>
            /// Gets format string.
            /// </summary>
            public string Format { get; }

            /// <summary>
            /// Gets the time unit.
            /// </summary>
            public long Unit { get; }
        }
    }
}
