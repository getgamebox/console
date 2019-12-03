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

namespace GameBox.Console.Question
{
    /// <summary>
    /// Represents a Question.
    /// </summary>
    public class Question
    {
        /// <summary>
        /// The question title.
        /// </summary>
        private readonly string question;

        /// <summary>
        /// The question default value.
        /// </summary>
        private readonly Mixture defaultValue;

        /// <summary>
        /// An array values for the autocompleter.
        /// </summary>
        private Mixture autocompleterValues;

        /// <summary>
        /// The validator for question.
        /// </summary>
        private Func<Mixture, Mixture> validator;

        /// <summary>
        /// The normalizer for question.
        /// </summary>
        private Func<Mixture, Mixture> normalizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Question"/> class.
        /// </summary>
        /// <param name="question">The question title.</param>
        /// <param name="defaultValue">The question default value.</param>
        public Question(string question, Mixture defaultValue = null)
        {
            this.question = question;
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets a value indicating whether the input is password style.
        /// </summary>
        public bool IsPassword { get; private set; }

        /// <summary>
        /// Gets a value indicating whether supported to fallback on non-password question if the response can not be password.
        /// </summary>
        public bool HasPasswordFallback { get; private set; }

        /// <summary>
        /// Gets the max attempts.
        /// </summary>
        public int MaxAttempts { get; private set; }

        /// <summary>
        /// Gets the question title.
        /// </summary>
        /// <returns>The question title.</returns>
        public string GetQuestion()
        {
            return question;
        }

        /// <summary>
        /// Gets the question default value.
        /// </summary>
        /// <returns>The question default value.</returns>
        public Mixture GetDefault()
        {
            return defaultValue;
        }

        /// <summary>
        /// Sets whether the user response must be password style or not.
        /// </summary>
        /// <param name="password">Whether is password.</param>
        public void SetPassword(bool password)
        {
            IsPassword = password;
        }

        /// <summary>
        /// Sets whether to fallback on non-password question if the response can not be password.
        /// </summary>
        /// <param name="fallback">Whether to fallback on non-password question.</param>
        /// <returns>The <see cref="Question"/> instance.</returns>
        public Question SetPasswordFallback(bool fallback)
        {
            HasPasswordFallback = fallback;
            return this;
        }

        /// <summary>
        /// Gets the autocompleter value.
        /// </summary>
        /// <returns>The autocompleter value.</returns>
        public Mixture GetAutocompleterValues()
        {
            return autocompleterValues;
        }

        /// <summary>
        /// Sets values for the autocompleter.
        /// </summary>
        /// <param name="value">The autocompleter.</param>
        /// <returns>The question instance.</returns>
        public Question SetAutocompleterValues(Mixture value)
        {
            if (IsPassword)
            {
                throw new ConsoleLogicException("A password question cannot use the autocompleter.");
            }

            autocompleterValues = value;
            return this;
        }

        /// <summary>
        /// Sets a validator for the question.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <returns>The question instance.</returns>
        public Question SetValidator(Func<Mixture, Mixture> validator)
        {
            this.validator = validator;
            return this;
        }

        /// <summary>
        /// Gets the validator for the question.
        /// </summary>
        /// <returns>The validator.</returns>
        public Func<Mixture, Mixture> GetValidator()
        {
            return validator;
        }

        /// <summary>
        /// Sets a normalizer for the response.
        /// </summary>
        /// <param name="normalizer">The normalizer.</param>
        /// <returns>The question instance.</returns>
        public Question SetNormalizer(Func<Mixture, Mixture> normalizer)
        {
            this.normalizer = normalizer;
            return this;
        }

        /// <summary>
        /// Gets a normalizer for the question.
        /// </summary>
        /// <returns>The normalizer.</returns>
        public Func<Mixture, Mixture> GetNormalizer()
        {
            return normalizer;
        }

        /// <summary>
        /// Sets the maximum number of attempts.
        /// </summary>
        /// <param name="attempts">0 means an unlimited number of attempts.</param>
        /// <returns>The question instance.</returns>
        public Question SetMaxAttempts(int attempts = 0)
        {
            if (attempts < 0)
            {
                throw new InvalidArgumentException("Maximum number of attempts must be large or equal than 0 value.");
            }

            MaxAttempts = attempts;
            return this;
        }
    }
}
