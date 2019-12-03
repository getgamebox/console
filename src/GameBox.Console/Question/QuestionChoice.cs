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
using System.Text.RegularExpressions;

namespace GameBox.Console.Question
{
    /// <summary>
    /// Represents a choice question.
    /// </summary>
    public class QuestionChoice : Question
    {
        /// <summary>
        /// The multiselect separator.
        /// </summary>
        public const char Separator = ',';

        /// <summary>
        /// An array of the choices.
        /// </summary>
        private readonly KeyValuePair<string, string>[] choices;

        /// <summary>
        /// The prompt for choices.
        /// </summary>
        private string prompt = " > ";

        /// <summary>
        /// The error message.
        /// </summary>
        private string errorMessage = "Value \"{0}\" is invalid";

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionChoice"/> class.
        /// </summary>
        /// <param name="question">The question message.</param>
        /// <param name="choices">The choices list.</param>
        /// <param name="defaultValue">The default value.</param>
        public QuestionChoice(string question, KeyValuePair<string, string>[] choices, Mixture defaultValue = null)
            : base(question, defaultValue)
        {
            this.choices = choices;
            SetValidator(GetDefaultValidator());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionChoice"/> class.
        /// </summary>
        /// <param name="question">The question message.</param>
        /// <param name="choices">The choices list.</param>
        /// <param name="defaultValue">The default value.</param>
        public QuestionChoice(string question, string[] choices, Mixture defaultValue = null)
            : base(question, defaultValue)
        {
            if (choices == null || choices.Length <= 0)
            {
                throw new ConsoleLogicException("Choice question must have at least 1 choice available.");
            }

            var i = 0;
            this.choices = Arr.Map(
                choices,
                (choice) => new KeyValuePair<string, string>($"{i++}", choice));
            SetValidator(GetDefaultValidator());
        }

        /// <summary>
        /// Gets a value indicating whether a multi choice question.
        /// </summary>
        public bool IsMultiselect { get; private set; }

        /// <summary>
        /// Returns available choices.
        /// </summary>
        /// <returns>The available choices.</returns>
        public KeyValuePair<string, string>[] GetChoices()
        {
            return choices;
        }

        /// <summary>
        /// Gets the prompt for choices.
        /// </summary>
        /// <returns>Returns the prompt for choices.</returns>
        public string GetPrompt()
        {
            return prompt;
        }

        /// <summary>
        /// Sets the prompt for choices.
        /// </summary>
        /// <param name="prompt">The prompt for choices.</param>
        /// <returns>The question instance.</returns>
        public QuestionChoice SetPrompt(string prompt)
        {
            this.prompt = prompt;
            return this;
        }

        /// <summary>
        /// Sets whether a multi choice question.
        /// </summary>
        /// <param name="isMultiselect">Whether a multi choice question.</param>
        /// <returns>The question instance.</returns>
        public QuestionChoice SetMultiselect(bool isMultiselect)
        {
            IsMultiselect = isMultiselect;
            return this;
        }

        /// <summary>
        /// Sets the error message for invalid values.
        /// </summary>
        /// <remarks>The error message has a string placeholder ({0}) for the invalid value.</remarks>
        /// <param name="message">The error message.</param>
        /// <returns>The question instance.</returns>
        public QuestionChoice SetErrorMessage(string message)
        {
            errorMessage = message;
            return this;
        }

        /// <summary>
        /// Gets the default validator.
        /// </summary>
        /// <returns>Returns the default validator.</returns>
        private Func<Mixture, Mixture> GetDefaultValidator()
        {
            return (selected) =>
            {
                if (selected is null)
                {
                    return null;
                }

                selected = selected.ToString().Replace(Str.Space, string.Empty);
                string[] selectedChoices;
                if (IsMultiselect)
                {
                    // Check for a separated comma values.
                    if (!Regex.IsMatch(selected, $"^[^{Separator}]+(?:{Separator}[^{Separator}]+)*$"))
                    {
                        throw new InvalidArgumentException(string.Format(format: errorMessage, args: selected));
                    }

                    selectedChoices = selected.ToString().Split(Separator);
                }
                else
                {
                    selectedChoices = selected;
                }

                var multiselectChoices = new List<string>();
                var results = new List<int>();
                foreach (var value in selectedChoices)
                {
                    for (var i = 0; i < choices.Length; i++)
                    {
                        if (choices[i].Key == value || choices[i].Value.Replace(Str.Space, string.Empty) == value)
                        {
                            results.Add(i);
                        }
                    }

                    if (results.Count > 1)
                    {
                        throw new InvalidArgumentException(
                            $"The provided answer is ambiguous. Value should be one of {string.Join(" or ", Arr.Map(results, i => choices[i].Key))}.");
                    }

                    if (results.Count <= 0)
                    {
                        throw new InvalidArgumentException(string.Format(errorMessage, value));
                    }

                    if (!IsMultiselect)
                    {
                        return choices[results[0]].Value;
                    }

                    multiselectChoices.Add(choices[results[0]].Value);
                    results.Clear();
                }

                return multiselectChoices.Distinct().ToArray();
            };
        }
    }
}
