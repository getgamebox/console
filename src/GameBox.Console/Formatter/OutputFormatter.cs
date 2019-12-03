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
using System.Text;
using System.Text.RegularExpressions;

namespace GameBox.Console.Formatter
{
    /// <summary>
    /// Formatter class for console output.
    /// </summary>
    public class OutputFormatter : IOutputFormatterWrappable
    {
        /// <summary>
        /// Indicates a legal formatted tag.
        /// </summary>
        protected const string TagRegex = "[a-z-][^<>]*";

        /// <summary>
        /// The styles mapping.
        /// </summary>
        private readonly Dictionary<string, IOutputFormatterStyle> styles;

        /// <summary>
        /// Represents the style stack currently in effect.
        /// </summary>
        private readonly Stack<IOutputFormatterStyle> styleStack;

        /// <summary>
        /// The default style.
        /// </summary>
        private IOutputFormatterStyle defaultStytle;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFormatter"/> class.
        /// </summary>
        /// <param name="enable">Whether this formatter should actually decorate strings.</param>
        /// <param name="styles">The style mapping.</param>
        public OutputFormatter(bool enable = false, IDictionary<string, IOutputFormatterStyle> styles = null)
        {
            Enable = enable;
            styleStack = new Stack<IOutputFormatterStyle>();
            this.styles = new Dictionary<string, IOutputFormatterStyle>
            {
                { "comment", new OutputFormatterStyle("yellow") },
                { "info", new OutputFormatterStyle("green") },
                { "question", new OutputFormatterStyle("black", "cyan") },
                { "error", new OutputFormatterStyle("white", "red") },
            };

            if (styles == null)
            {
                return;
            }

            foreach (var item in styles)
            {
                this.styles[item.Key.ToLower()] = item.Value;
            }
        }

        /// <inheritdoc />
        public bool Enable { get; set; }

        /// <summary>
        /// Escapes special char in given text.
        /// </summary>
        /// <param name="message">Message to escape.</param>
        /// <returns>Escaped message.</returns>
        public static string Escape(string message)
        {
            message = Regex.Replace(message, "([^\\\\]?)<", "$1\\<");
            return EscapeTrailingBackslash(message);
        }

        /// <summary>
        /// Escapes trailing "\" in given text when the end is "\".
        /// </summary>
        /// <param name="message">Message to escape.</param>
        /// <returns>Escaped message.</returns>
        public static string EscapeTrailingBackslash(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return message;
            }

            if (message[message.Length - 1] != '\\')
            {
                return message;
            }

            var length = message.Length;
            message = message.TrimEnd('\\');
            message = message.Replace("\0", string.Empty);
            if (length - message.Length > 0)
            {
                message += Str.Repeat("\0", length - message.Length);
            }

            return message;
        }

        /// <inheritdoc />
        public string Format(string message)
        {
            return FormatAndWrap(message);
        }

        /// <inheritdoc />
        public string FormatAndWrap(string message, int width = 0)
        {
            var offset = 0;
            var ret = new StringBuilder();
            var matches = Regex.Matches(message, $"<(({TagRegex}) | /({TagRegex})?)>",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            var currentLineLength = 0;

            foreach (Match match in matches)
            {
                var position = match.Index;
                var text = match.Value;

                // Handling tags that use "\" comments
                if (position != 0 && message[position - 1] == '\\')
                {
                    continue;
                }

                // Add the text up to the next tag
                // ex: hello<section>world</section>
                //     step 1 : hello
                //     step 2 : world
                ret.Append(ApplyCurrentStyle(message.Substring(offset, position - offset), ret.ToString(), width,
                    ref currentLineLength));
                offset = position + text.Length;

                // opening tag?
                var isOpenTag = text[1] != '/';
                var tag = isOpenTag ? match.Groups[1].Value : match.Groups[3].Value;

                IOutputFormatterStyle style;
                if (!isOpenTag && string.IsNullOrEmpty(tag))
                {
                    // </>
                    if (styleStack.Count > 0)
                    {
                        styleStack.Pop();
                    }
                }
                else if ((style = GetStyleFromText(tag)) == null)
                {
                    ret.Append(ApplyCurrentStyle(text, ret.ToString(), width, ref currentLineLength));
                }
                else if (isOpenTag)
                {
                    styleStack.Push(style);
                }
                else
                {
                    if (styleStack.Count <= 0
                        || styleStack.Peek() != style)
                    {
                        throw new ConsoleLogicException("Incorrectly nested style tag found.");
                    }

                    styleStack.Pop();
                }
            }

            ret.Append(ApplyCurrentStyle(message.Substring(offset), ret.ToString(),
                width, ref currentLineLength));

            var output = ret.ToString();
            if (output.IndexOf('\0') >= 0)
            {
                output = output.Replace('\0', '\\');
            }

            return output.Replace("\\<", "<");
        }

        /// <inheritdoc />
        public IOutputFormatterStyle GetStyle(string name)
        {
            if (!HasStyle(name))
            {
                throw new InvalidArgumentException($"Undefined style: {name}");
            }

            return styles[name.ToLower()];
        }

        /// <inheritdoc />
        public bool HasStyle(string name)
        {
            return !string.IsNullOrEmpty(name) && styles.ContainsKey(name.ToLower());
        }

        /// <inheritdoc />
        public void SetStyle(string name, IOutputFormatterStyle style)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidArgumentException("Style name not allowed to be empty or null.");
            }

            styles[name.ToLower()] = style;
        }

        /// <inheritdoc />
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Tries to create new style instance from text.
        /// </summary>
        /// <param name="text">The tag text.</param>
        /// <returns>The style instance.</returns>
        protected virtual IOutputFormatterStyle CreateStyleFromText(string text)
        {
            // Valid values are:
            //  - fg=red;bg=green
            //  - fg=red;options=blod,blink
            var matches = Regex.Matches(text, "([^=]+)=([^;]+)(;|$)");
            if (matches.Count <= 0)
            {
                return null;
            }

            var style = GetDefaultStyle();
            foreach (Match match in matches)
            {
                var name = match.Groups[1].Value.ToLower();
                var value = match.Groups[2].Value.ToLower();
                if (name == "fg")
                {
                    style.SetForeground(value);
                }
                else if (name == "bg")
                {
                    style.SetBackground(value);
                }
                else if (name == "href" && style is OutputFormatterStyle internalStyle)
                {
                    internalStyle.SetHref(value);
                }
                else if (name == "options")
                {
                    var options = value.Split(',', ';');
                    foreach (var option in options)
                    {
                        style.SetOption(option);
                    }
                }
                else
                {
                    return null;
                }
            }

            return style;
        }

        /// <summary>
        /// Returns the default style.
        /// </summary>
        /// <returns>The default style.</returns>
        protected virtual IOutputFormatterStyle GetDefaultStyle()
        {
            return new OutputFormatterStyle();
        }

        /// <summary>
        /// Tries to create new style instance from text.
        /// </summary>
        /// <param name="text">The tag text.</param>
        /// <returns>The style instance.</returns>
        private IOutputFormatterStyle GetStyleFromText(string text)
        {
            return styles.TryGetValue(text, out IOutputFormatterStyle style)
                ? style
                : CreateStyleFromText(text);
        }

        /// <summary>
        /// Applies current style from stack to text, if must be applied.
        /// </summary>
        /// <param name="text">The style text.</param>
        /// <param name="current">The current recorded styled text.</param>
        /// <param name="width">The wrap width.</param>
        /// <param name="currentLineLength">Current line length.</param>
        /// <returns>The styled text.</returns>
        private string ApplyCurrentStyle(string text, string current, int width, ref int currentLineLength)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            IOutputFormatterStyle style;
            if (styleStack.Count <= 0)
            {
                style = defaultStytle ?? (defaultStytle = GetDefaultStyle());
            }
            else
            {
                style = styleStack.Peek();
            }

            if (width <= 0)
            {
                return Enable ? style.Format(text) : text;
            }

            // Prevent the position when the truncation is just adjacent to the space
            // ex: hello world
            // Not use trim():     Use trim():
            // > hello             > hello
            // >  world            〉world
            if (currentLineLength <= 0 && !string.IsNullOrEmpty(current))
            {
                text = text.TrimStart();
            }

            current = current ?? string.Empty;

            var ret = new StringBuilder(text.Length + (text.Length / width * 3));
            if (currentLineLength <= 0 && current.Length >= Environment.NewLine.Length
                && current.Substring(current.Length - Environment.NewLine.Length) != Environment.NewLine)
            {
                ret.Append(Environment.NewLine);
            }

            // Calculate the remaining width remaining at the last call.
            // ex: width = 8
            //  step 1 : hello     > hello
            //  step 2 : world     > hello wo     // prefix =  wo
            //                     > rld          // text   = rld
            if (currentLineLength > 0)
            {
                var i = Math.Min(width - currentLineLength, text.Length);
                ret.Append(text.Substring(0, i)).Append(Environment.NewLine);
                text = text.Substring(i);
            }

            // The newline character under windows is "\r\n" under linux is "\n"
            // So we can use "\n" to judge the newline
            var isNewLineEnd = text.Length > 0 && text[text.Length - 1] == '\n';

            // Wrap content in text according to width.
            var wrappedText =
                Str.WordWrap(
                    Environment.NewLine != "\n"
                        ? text.Replace(Environment.NewLine, "\n")
                        : text,
                    width);
            ret.Append(wrappedText);

            if (isNewLineEnd)
            {
                ret.Append(Environment.NewLine);
            }

            wrappedText = ret.ToString();
            var lines = wrappedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            var end = lines[lines.Length - 1];
            if (width == (currentLineLength = end.Length))
            {
                currentLineLength = 0;
            }

            // Formatted if enabled, otherwise the text is wrapped.
            if (!Enable)
            {
                return wrappedText;
            }

            for (var index = 0; index < lines.Length; index++)
            {
                lines[index] = style.Format(lines[index]);
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
