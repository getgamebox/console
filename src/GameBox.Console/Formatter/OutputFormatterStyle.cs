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
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBox.Console.Formatter
{
    /// <summary>
    /// Formatter style class for defining styles.
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/ANSI_escape_code.</remarks>
    public class OutputFormatterStyle : IOutputFormatterStyle
    {
        /// <summary>
        /// Whether is supported href.
        /// </summary>
        private readonly bool hrefSupported;

        /// <summary>
        /// Allowable foreground.
        /// </summary>
        private readonly Dictionary<string, (string set, string unset)> availableForegroundColors
            = new Dictionary<string, (string set, string unset)>
        {
            { "black", ("30", "39") },
            { "red", ("31", "39") },
            { "green", ("32", "39") },
            { "yellow", ("33", "39") },
            { "blue", ("34", "39") },
            { "magenta", ("35", "39") },
            { "cyan", ("36", "39") },
            { "white", ("37", "39") },
            { "default", ("39", "39") },
        };

        /// <summary>
        /// Allowable background.
        /// </summary>
        private readonly Dictionary<string, (string set, string unset)> availableBackgroundColors
            = new Dictionary<string, (string set, string unset)>
        {
            { "black", ("40", "49") },
            { "red", ("41", "49") },
            { "green", ("42", "49") },
            { "yellow", ("43", "49") },
            { "blue", ("44", "49") },
            { "magenta", ("45", "49") },
            { "cyan", ("46", "49") },
            { "white", ("47", "49") },
            { "default", ("49", "49") },
        };

        /// <summary>
        /// Allowable options.
        /// </summary>
        private readonly Dictionary<string, (string set, string unset)> availableOptions
            = new Dictionary<string, (string set, string unset)>
        {
            { "bold", ("1", "22") },
            { "underscore", ("4", "24") },
            { "blink", ("5", "25") },
            { "reverse", ("7", "27") },
            { "conceal", ("8", "28") },
        };

        /// <summary>
        /// The foreground color.
        /// </summary>
        private (string set, string unset)? foreground;

        /// <summary>
        /// The background color.
        /// </summary>
        private (string set, string unset)? background;

        /// <summary>
        /// The set options.
        /// </summary>
        private (string set, string unset)[] options;

        /// <summary>
        /// The href.
        /// </summary>
        private string href;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFormatterStyle"/> class.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="options">The options.</param>
        public OutputFormatterStyle(string foreground = null, string background = null, params string[] options)
        {
            SetForeground(foreground);
            SetBackground(background);
            if (options != null)
            {
                foreach (var option in options)
                {
                    SetOption(option);
                }
            }

            hrefSupported = Terminal.GetEnvironmentVariable("TERMINAL_EMULATOR") != "JetBrains-JediTerm";
        }

        /// <inheritdoc />
        public void SetForeground(string color = null)
        {
            if (string.IsNullOrEmpty(color))
            {
                foreground = null;
                return;
            }

            if (!availableForegroundColors.TryGetValue(color, out (string set, string unset) value))
            {
                throw new InvalidArgumentException(
                    $"Invalid foreground color specified: {color}.Expected one of({string.Join(", ", availableForegroundColors.Keys.ToArray())})");
            }

            foreground = value;
        }

        /// <inheritdoc />
        public void SetBackground(string color = null)
        {
            if (string.IsNullOrEmpty(color))
            {
                background = null;
                return;
            }

            if (!availableBackgroundColors.TryGetValue(color, out (string set, string unset) value))
            {
                throw new InvalidArgumentException(
                    $"Invalid background color specified: {color}.Expected one of({string.Join(", ", availableBackgroundColors.Keys.ToArray())})");
            }

            background = value;
        }

        /// <inheritdoc />
        public void SetOption(string effect)
        {
            if (!availableOptions.TryGetValue(effect, out (string set, string unset) optionTuple))
            {
                throw new InvalidArgumentException(
                    $"Invalid option specified: {effect}.Expected one of({string.Join(", ", availableOptions.Keys.ToArray())})");
            }

            if (FindSettedOptionsIndex(optionTuple) < 0)
            {
                Arr.Push(ref options, optionTuple);
            }
        }

        /// <inheritdoc />
        public void UnsetOption(string effect)
        {
            if (!availableOptions.TryGetValue(effect, out (string set, string unset) optionTuple))
            {
                throw new InvalidArgumentException(
                    $"Invalid option specified: {effect}.Expected one of({string.Join(", ", availableOptions.Keys.ToArray())})");
            }

            var index = FindSettedOptionsIndex(optionTuple);
            if (index >= 0)
            {
                Arr.RemoveAt(ref options, index);
            }
        }

        /// <summary>
        /// Sets the href.
        /// </summary>
        /// <param name="href">The href.</param>
        public void SetHref(string href)
        {
            this.href = href;
        }

        /// <inheritdoc />
        public string Format(string text)
        {
            var setCode = Array.Empty<string>();
            var unsetCode = Array.Empty<string>();

            if (foreground.HasValue)
            {
                Arr.Push(ref setCode, foreground.Value.set);
                Arr.Push(ref unsetCode, foreground.Value.unset);
            }

            if (background.HasValue)
            {
                Arr.Push(ref setCode, background.Value.set);
                Arr.Push(ref unsetCode, background.Value.unset);
            }

            if (options != null)
            {
                foreach (var (set, unset) in options)
                {
                    Arr.Push(ref setCode, set);
                    Arr.Push(ref unsetCode, unset);
                }
            }

            if (!string.IsNullOrEmpty(href) && hrefSupported)
            {
                text = $"\u001b]8;;{href}\u001b\\{text}\u001b]8;;\u001b\\";
            }

            return setCode.Length <= 0
                ? text
                : $"\u001b[{string.Join(";", setCode)}m{text}\u001b[{string.Join(";", unsetCode)}m";
        }

        /// <summary>
        /// Find the options index.
        /// </summary>
        /// <param name="tuple">The tuple struct.</param>
        /// <returns>The index of setted option.</returns>
        private int FindSettedOptionsIndex((string set, string unset) tuple)
        {
            options = options ?? Array.Empty<(string set, string unset)>();
            return Array.FindIndex(options, (item) => tuple.set == item.set && tuple.unset == item.unset);
        }
    }
}
