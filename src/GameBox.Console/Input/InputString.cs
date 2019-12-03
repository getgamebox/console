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

using System;
using System.Collections.Generic;
using System.Text;

namespace GameBox.Console.Input
{
    /// <summary>
    /// Represents an input provided as a string.
    /// </summary>
    public class InputString : InputArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputString"/> class.
        /// </summary>
        /// <param name="input">A string representing the parameters from the CLI.</param>
        public InputString(string input)
            : base(Array.Empty<string>())
        {
            SetArgs(Tokenize(input));
        }

        /// <summary>
        /// Tokenizes a string.
        /// </summary>
        private IEnumerable<string> Tokenize(string input)
        {
            // todo: need to be fully implement https://ss64.com/nt/syntax-esc.html
            var args = new List<string>();
            var currentArgs = new StringBuilder();
            var escape = false;
            var inQuote = false;
            var hadQuote = false;
            var prevChar = '\0';

            for (var i = 0; i < input.Length; i++)
            {
                var cursorChar = input[i];
                if (cursorChar == '\\' && !escape)
                {
                    // Beginning of a backslash-escape sequence
                    escape = true;
                }
                else if (cursorChar == '\\' && escape)
                {
                    // Double backslash, keep one
                    currentArgs.Append(cursorChar);
                    escape = false;
                }
                else if (cursorChar == '"' && !escape)
                {
                    // Toggle quoted range
                    inQuote = !inQuote;
                    hadQuote = true;
                    if (inQuote && prevChar == '"')
                    {
                        // Doubled quote within a quoted range is like escaping
                        currentArgs.Append(cursorChar);
                    }
                }
                else if (char.IsWhiteSpace(cursorChar) && !inQuote)
                {
                    if (escape)
                    {
                        // Add pending escape char
                        currentArgs.Append('\\');
                        escape = false;
                    }

                    // Accept empty arguments only if they are quoted
#pragma warning disable S2583
                    if (currentArgs.Length > 0 || hadQuote)
#pragma warning restore S2583
                    {
                        args.Add(currentArgs.ToString());
                    }

                    // Reset for next argument
                    currentArgs.Clear();
                    hadQuote = false;
                }
                else
                {
                    if (escape)
                    {
                        // Add pending escape char
                        currentArgs.Append('\\');
                        escape = false;
                    }

                    // Copy character from input, no special meaning
                    currentArgs.Append(cursorChar);
                }

                prevChar = cursorChar;
            }

            // Save last argument
            if (currentArgs.Length > 0 || hadQuote)
            {
                args.Add(currentArgs.ToString());
            }

            return args;
        }
    }
}
