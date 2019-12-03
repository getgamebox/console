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

using GameBox.Console.Util;
using System;
using System.Collections.Generic;

namespace GameBox.Console.Style
{
    /// <summary>
    /// <see cref="IStyle"/> extend method.
    /// </summary>
    public static class ExtendStyle
    {
        /// <inheritdoc cref="IStyle.Text"/>
        public static void Text(this IStyle style, string[] text)
        {
            if (text == null)
            {
                return;
            }

            foreach (var input in text)
            {
                style.Text(input);
            }
        }

        /// <inheritdoc cref="IStyle.Comment"/>
        public static void Comment(this IStyle style, string[] comment)
        {
            if (comment == null)
            {
                return;
            }

            style.Comment(string.Join(Environment.NewLine, comment));
        }

        /// <inheritdoc cref="IStyle.Success"/>
        public static void Success(this IStyle style, string[] text)
        {
            if (text == null)
            {
                return;
            }

            style.Success(string.Join(Environment.NewLine, text));
        }

        /// <inheritdoc cref="IStyle.Error"/>
        public static void Error(this IStyle style, string[] text)
        {
            if (text == null)
            {
                return;
            }

            style.Error(string.Join(Environment.NewLine, text));
        }

        /// <inheritdoc cref="IStyle.Warning"/>
        public static void Warning(this IStyle style, string[] text)
        {
            if (text == null)
            {
                return;
            }

            style.Warning(string.Join(Environment.NewLine, text));
        }

        /// <inheritdoc cref="IStyle.Note"/>
        public static void Note(this IStyle style, string[] text)
        {
            if (text == null)
            {
                return;
            }

            style.Note(string.Join(Environment.NewLine, text));
        }

        /// <inheritdoc cref="IStyle.Caution"/>
        public static void Caution(this IStyle style, string[] text)
        {
            if (text == null)
            {
                return;
            }

            style.Caution(string.Join(Environment.NewLine, text));
        }

        /// <summary>
        /// Asks a choice question.
        /// </summary>
        /// <param name="style"><see cref="IStyle"/> instance.</param>
        /// <param name="question">The question title.</param>
        /// <param name="choices">An array of chocies.</param>
        /// <param name="defaultValue">The default selected key.</param>
        /// <param name="maxAttempts">The max attempts. 0 means an unlimited number of attempts.</param>
        /// <returns>null if the user not input,otherwise is user select choice value.</returns>
        public static string AskChoice(this IStyle style, string question, string[] choices, Mixture defaultValue = null, int maxAttempts = 0)
        {
            var i = 1;
            return style.AskChoice(
                question,
                Arr.Map(choices, (choice) => new KeyValuePair<string, string>($"{i++}", choice)), defaultValue,
                maxAttempts);
        }
    }
}
