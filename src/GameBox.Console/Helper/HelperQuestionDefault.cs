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

using GameBox.Console.Formatter;
using GameBox.Console.Output;
using GameBox.Console.Question;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;

namespace GameBox.Console.Helper
{
    /// <summary>
    /// Default Style Guide compliant question helper.
    /// </summary>
    public class HelperQuestionDefault : HelperQuestion
    {
        /// <inheritdoc />
        protected override void WritePrompt(IOutput output, Question.Question question)
        {
            var message = OutputFormatter.EscapeTrailingBackslash(question.GetQuestion());
            var defaultValue = question.GetDefault();

            if (defaultValue is null)
            {
                message = $" <info>{message}</info>:";
            }
            else if (question is QuestionConfirmation)
            {
                message = $" <info>{message} (yes/no)</info> [<comment>{(defaultValue ? "yes" : "no")}</comment>]:";
            }
            else if (question is QuestionChoice questionChoiceMult && questionChoiceMult.IsMultiselect)
            {
                var choices = questionChoiceMult.GetChoices();
                var defaultElements = defaultValue.ToString().Split(QuestionChoice.Separator);
                for (var index = 0; index < defaultElements.Length; index++)
                {
                    defaultElements[index] = MapToChoiceValue(choices, defaultElements[index]);
                }

                message = defaultElements.Length > 0
                    ? $" <info>{message}</info> [<comment>{OutputFormatter.Escape(string.Join($"{QuestionChoice.Separator} ", defaultElements))}</comment>]:"
                    : $" <info>{message}</info>";
            }
            else if (question is QuestionChoice questionChoiceSingle)
            {
                var choices = questionChoiceSingle.GetChoices();
                var content = MapToChoiceValue(choices, defaultValue) ?? string.Empty;
                content = OutputFormatter.Escape(content);
                message = !string.IsNullOrEmpty(content)
                    ? $" <info>{message}</info> [<comment>{content}</comment>]:"
                    : $" <info>{message}</info>:";
            }
            else
            {
                message = $" <info>{message}</info> [<comment>{OutputFormatter.Escape(defaultValue)}</comment>]:";
            }

            output.WriteLine(message);

            if (question is QuestionChoice questionChoice)
            {
                var choices = questionChoice.GetChoices();
                var width = 0;
                foreach (var item in choices)
                {
                    width = Math.Max(width, item.Key.Length);
                }

                foreach (var item in choices)
                {
                    output.WriteLine($"  [<comment>{Str.Pad(width, item.Key)}</comment>] {item.Value}");
                }
            }

            output.Write(" > ");
        }

        /// <summary>
        /// Map the choices key to a value if it exists.
        /// </summary>
        /// <param name="choices">An array of choices.</param>
        /// <param name="defaultValue">The default key or value.</param>
        /// <returns>The choice value.</returns>
        private string MapToChoiceValue(KeyValuePair<string, string>[] choices, string defaultValue)
        {
            var defaultSelectedIndex = Array.FindIndex(choices, (choice) => choice.Key == defaultValue);
            if (defaultSelectedIndex >= 0)
            {
                defaultValue = choices[defaultSelectedIndex].Value;
            }

            return defaultValue;
        }
    }
}
