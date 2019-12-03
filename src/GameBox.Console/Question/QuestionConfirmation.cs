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
using System.Text.RegularExpressions;

namespace GameBox.Console.Question
{
    /// <summary>
    /// Represents a yes/no question.
    /// </summary>
    public class QuestionConfirmation : Question
    {
        /// <summary>
        /// A regex to match the "yes" answer.
        /// </summary>
        private readonly string trueAnswerRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionConfirmation"/> class.
        /// </summary>
        /// <param name="question">The question title.</param>
        /// <param name="defaultValue">The question default value.</param>
        /// <param name="trueAnswerRegex">A regex to match the "yes" answer.</param>
        public QuestionConfirmation(string question, bool defaultValue = true,
            string trueAnswerRegex = "^y")
            : base(question, defaultValue)
        {
            this.trueAnswerRegex = trueAnswerRegex;
            SetNormalizer(DefaultNormalizer);
        }

        /// <summary>
        /// The default answer normalizer.
        /// </summary>
        /// <param name="value">The answer value.</param>
        /// <returns>The normalize value.</returns>
        private Mixture DefaultNormalizer(Mixture value)
        {
            string answer = value;
            answer = answer.Trim();
            var answerIsTrue = Regex.IsMatch(answer, trueAnswerRegex, RegexOptions.IgnoreCase);
            var defaultValue = GetDefault();

            if (defaultValue is null || !defaultValue)
            {
                return answerIsTrue;
            }

            return string.IsNullOrEmpty(answer) || answerIsTrue ||
                   string.Equals(answer, bool.TrueString, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
