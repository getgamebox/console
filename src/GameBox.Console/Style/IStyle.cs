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
    /// Style helpers interface for console output.
    /// </summary>
    public interface IStyle
    {
        /// <summary>
        /// Formats a command title.
        /// </summary>
        /// <param name="title">The title message.</param>
        void Title(string title);

        /// <summary>
        /// Formats a section title.
        /// </summary>
        /// <param name="section">The section message.</param>
        void Section(string section);

        /// <summary>
        /// Formats a comment message.
        /// </summary>
        /// <param name="comment">The comment message.</param>
        void Comment(string comment);

        /// <summary>
        /// Formats a unordered list.
        /// </summary>
        /// <param name="elements">An array of the message list.</param>
        void Listing(string[] elements);

        /// <summary>
        /// Formats a plain text content.
        /// </summary>
        /// <param name="text">The text content.</param>
        void Text(string text);

        /// <summary>
        /// Formats a success text content.
        /// </summary>
        /// <param name="text">The success text content.</param>
        void Success(string text);

        /// <summary>
        /// Formats a error text content.
        /// </summary>
        /// <param name="text">The error text content.</param>
        void Error(string text);

        /// <summary>
        /// Formats a warning text content.
        /// </summary>
        /// <param name="text">The warning text content.</param>
        void Warning(string text);

        /// <summary>
        /// Formats a note text content.
        /// </summary>
        /// <param name="text">The note text content.</param>
        void Note(string text);

        /// <summary>
        /// Formats a caution text content.
        /// </summary>
        /// <param name="text">The caution text content.</param>
        void Caution(string text);

        /// <summary>
        /// Asks a question.
        /// </summary>
        /// <param name="question">The question title.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="validator">The value validator.</param>
        /// <param name="maxAttempts">The max attempts. 0 means an unlimited number of attempts.</param>
        /// <returns>The user input value.</returns>
        Mixture Ask(string question, Mixture defaultValue = null, Func<Mixture, Mixture> validator = null, int maxAttempts = 0);

        /// <summary>
        /// Asks a question with password field.
        /// </summary>
        /// <param name="question">The question title.</param>
        /// <param name="validator">The value validator.</param>
        /// <param name="maxAttempts">The max attempts. 0 means an unlimited number of attempts.</param>
        /// <returns>The user input value.</returns>
        Mixture AskPassword(string question, Func<Mixture, Mixture> validator = null, int maxAttempts = 0);

        /// <summary>
        /// Asks for confirmation.
        /// </summary>
        /// <param name="question">The question title.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>True if the user agree, otherwise false.</returns>
        bool AskConfirm(string question, bool defaultValue = true);

        /// <summary>
        /// Asks a choice question.
        /// </summary>
        /// <param name="question">The question title.</param>
        /// <param name="choices">An array of chocies.</param>
        /// <param name="defaultValue">The default selected key.</param>
        /// <param name="maxAttempts">The max attempts. 0 means an unlimited number of attempts.</param>
        /// <returns>null if the user not input,otherwise is user select choice value.</returns>
        string AskChoice(string question, KeyValuePair<string, string>[] choices, Mixture defaultValue = null, int maxAttempts = 0);

        /// <summary>
        /// Add newline(s).
        /// </summary>
        /// <param name="count">The number of newlines.</param>
        void NewLine(int count = 1);

        /// <summary>
        /// Starts the progress output.
        /// </summary>
        /// <param name="maxStep">Maximum steps (0 if unknown).</param>
        void ProgressBegin(int maxStep = 0);

        /// <summary>
        /// Advances the progress output <paramref name="steps"/> steps.
        /// </summary>
        /// <param name="steps">Number of steps to advance.</param>
        void ProcessAdvance(int steps = 1);

        /// <summary>
        /// Ended the progress output.
        /// </summary>
        void ProgressEnd();
    }
}
