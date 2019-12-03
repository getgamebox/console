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
using GameBox.Console.Helper;
using GameBox.Console.Input;
using GameBox.Console.Output;
using GameBox.Console.Question;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GameBox.Console.Style
{
    /// <summary>
    /// Output helper for the Default style guide.
    /// </summary>
    public class StyleDefault : AbstractStyleOutput
    {
        /// <summary>
        /// The console line max length.
        /// </summary>
        private const int MaxLineLength = 120;

        /// <summary>
        /// The console line length(width).
        /// </summary>
        private readonly int lineLength;

        /// <summary>
        /// The std input instance.
        /// </summary>
        private readonly IInput input;

        /// <summary>
        /// The output buffered.
        /// </summary>
        private readonly OutputStringBuilder outputBuffered;

        /// <summary>
        /// The question helper.
        /// </summary>
        private HelperQuestion helperQuestion;

        /// <summary>
        /// The progress bar.
        /// </summary>
        private ProgressBar progressBar;

        /// <summary>
        /// Initializes a new instance of the <see cref="StyleDefault"/> class.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        public StyleDefault(IInput input, IOutput output)
            : base(output)
        {
            this.input = input;
            outputBuffered = new OutputStringBuilder(output.Options, (IOutputFormatter)output.Formatter.Clone());
            outputBuffered.SetDecorated(false);

            // Windows cmd wraps lines as soon as the terminal width is reached,
            // whether there are following chars or not.
            var width = Terminal.Width > 0 ? Terminal.Width : MaxLineLength;
            lineLength = Math.Min(width - (Path.DirectorySeparatorChar == '\\' ? 1 : 0), MaxLineLength);
        }

        /// <inheritdoc />
        public override void Title(string title)
        {
            AutoPrependBlock();
            WriteLine($"<comment>{OutputFormatter.EscapeTrailingBackslash(title)}</comment>");
            WriteLine(
                $"<comment>{Str.Repeat("=", AbstractHelper.StrlenWithoutDecoration(Formatter, title))}</comment>");
            NewLine();
        }

        /// <inheritdoc />
        public override void Section(string section)
        {
            AutoPrependBlock();
            WriteLine($"<comment>{OutputFormatter.EscapeTrailingBackslash(section)}</comment>");
            WriteLine(
                $"<comment>{Str.Repeat("-", AbstractHelper.StrlenWithoutDecoration(Formatter, section))}</comment>");
            NewLine();
        }

        /// <inheritdoc />
        public override void Comment(string comment)
        {
            Block(comment, null, null, "<fg=default;bg=default> // </>", false, false);
        }

        /// <inheritdoc />
        public override void Listing(string[] elements)
        {
            AutoPrependText();
            elements = Arr.Map(elements, (element) => $" * {element}");
            WriteLine(elements);
            NewLine();
        }

        /// <inheritdoc />
        public override void Text(string text)
        {
            AutoPrependText();
            WriteLine($" {text}");
        }

        /// <inheritdoc />
        public override void Success(string text)
        {
            Block(text, "OK", "fg=black;bg=green", Str.Space, true);
        }

        /// <inheritdoc />
        public override void Error(string text)
        {
            Block(text, "ERROR", "fg=white;bg=red", Str.Space, true);
        }

        /// <inheritdoc />
        public override void Warning(string text)
        {
            Block(text, "WARNING", "fg=black;bg=yellow", Str.Space, true);
        }

        /// <inheritdoc />
        public override void Note(string text)
        {
            Block(text, "NOTE", "fg=yellow", " ! ");
        }

        /// <inheritdoc />
        public override void Caution(string text)
        {
            Block(text, "CAUTION", "fg=white;bg=red", " ! ", true);
        }

        /// <inheritdoc />
        public override Mixture Ask(string question, Mixture defaultValue = null, Func<Mixture, Mixture> validator = null, int maxAttempts = 0)
        {
            var questionInstance = new Question.Question(question, defaultValue);
            questionInstance.SetValidator(validator);
            questionInstance.SetMaxAttempts(maxAttempts);

            return AskQuestion(questionInstance);
        }

        /// <inheritdoc />
        public override Mixture AskPassword(string question, Func<Mixture, Mixture> validator = null, int maxAttempts = 0)
        {
            var questionInstance = new Question.Question(question);
            questionInstance.SetPassword(true);
            questionInstance.SetValidator(validator);
            questionInstance.SetMaxAttempts(maxAttempts);

            return AskQuestion(questionInstance);
        }

        /// <inheritdoc />
        public override bool AskConfirm(string question, bool defaultValue = true)
        {
            return AskQuestion(new QuestionConfirmation(question, defaultValue));
        }

        /// <inheritdoc />
        public override string AskChoice(string question, KeyValuePair<string, string>[] choices,
            Mixture defaultValue = null, int maxAttempts = 0)
        {
            return AskQuestion(new QuestionChoice(question, choices, defaultValue)
                .SetMaxAttempts(maxAttempts));
        }

        /// <inheritdoc />
        public override void ProgressBegin(int maxStep = 0)
        {
            progressBar = CreateProgressBar(maxStep);
            progressBar.Begin(maxStep);
        }

        /// <inheritdoc />
        public override void ProcessAdvance(int steps = 1)
        {
            GetProgressBar().Advance(steps);
        }

        /// <inheritdoc />
        public override void ProgressEnd()
        {
            GetProgressBar().End();
            NewLine(2);
            progressBar = null;
        }

        /// <inheritdoc cref="Block(string[],string,string,string,bool,bool)"/>
        /// <param name="message">The text content.</param>
        public void Block(string message, string type = null, string style = null, string prefix = null, bool padding = false,
            bool escape = false)
        {
            Block(message.Split(new[] { Environment.NewLine }, StringSplitOptions.None), type, style, prefix, padding, escape);
        }

        /// <inheritdoc cref="CreateBlock"/>
        public void Block(string[] messages, string type = null, string style = null, string prefix = null, bool padding = false,
            bool escape = false)
        {
            AutoPrependBlock();
            WriteLine(CreateBlock(messages, type, style, prefix, padding, escape));
            NewLine();
        }

        /// <inheritdoc />
        public override void Write(string message, bool newLine = false, OutputOptions options = OutputOptions.None)
        {
            base.Write(message, newLine, options);
            WriteBuffered(message, newLine, options);
        }

        /// <summary>
        /// Writes a message to the output.
        /// </summary>
        /// <param name="messages">An array of the messages.</param>
        /// <param name="newLine">Whether to add a newline.</param>
        /// <param name="options">A bitmask of options. <see cref="OutputOptions.None"/> is considered the same as <see cref="OutputOptions.OutputNormal"/> | <see cref="OutputOptions.VerbosityNormal"/>.</param>
        public void Write(string[] messages, bool newLine = false, OutputOptions options = OutputOptions.None)
        {
            foreach (var message in messages)
            {
                Write(message, newLine, options);
            }
        }

        /// <inheritdoc />
        public override void WriteLine(string message, OutputOptions options = OutputOptions.None)
        {
            base.WriteLine(message, options);
            WriteBuffered(message, true, options);
        }

        /// <summary>
        /// Writes a message to the output and adds a newline at the end.
        /// </summary>
        /// <param name="messages">An array of the messages.</param>
        /// <param name="options">A bitmask of options. <see cref="OutputOptions.None"/> is considered the same as <see cref="OutputOptions.OutputNormal"/> | <see cref="OutputOptions.VerbosityNormal"/>.</param>
        public void WriteLine(string[] messages, OutputOptions options = OutputOptions.None)
        {
            foreach (var message in messages)
            {
                WriteLine(message, options);
            }
        }

        /// <inheritdoc />
        public override ProgressBar CreateProgressBar(int maxSteps = 0)
        {
            var bar = base.CreateProgressBar(maxSteps);

            if (Path.DirectorySeparatorChar == '\\'
                && Terminal.GetEnvironmentVariable("TERM_PROGRAM") != "Hyper"
                && Terminal.GetEnvironmentVariable("CMDER_SHELL").Length <= 0)
            {
                return bar;
            }

            bar.SetEmptyBarChar("░"); // light shade character \u2591
            bar.SetProgressChar("▓");
            bar.SetBarChar("▓"); // dark shade character \u2593
            return bar;
        }

        /// <summary>
        /// Ask a question for user.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <returns>The user answer.</returns>
        public Mixture AskQuestion(Question.Question question)
        {
            if (input.IsInteractive)
            {
                AutoPrependBlock();
            }

            helperQuestion = helperQuestion ?? new HelperQuestionDefault();

            var answer = helperQuestion.Ask(input, this, question);

            if (!input.IsInteractive)
            {
                return answer;
            }

            NewLine();
            return answer;
        }

        /// <summary>
        /// Sets an help question instance.
        /// </summary>
        /// <param name="helper">The helper instance.</param>
        public void SetHelperQuestion(HelperQuestion helper)
        {
            helperQuestion = helper;
        }

        /// <summary>
        /// Returns the progress bar instance.
        /// </summary>
        /// <returns>The progress bar instance.</returns>
        private ProgressBar GetProgressBar()
        {
            if (progressBar == null)
            {
                throw new ConsoleRuntimeException("The ProgressBar is not begin.");
            }

            return progressBar;
        }

        /// <inheritdoc cref="Write(string, bool, OutputOptions)"/>
        private void WriteBuffered(string message, bool newLine = false,
            OutputOptions options = OutputOptions.None)
        {
            // We need to know if the two last chars are Environment.NewLine
            // Preserve the last 4 chars inserted (Environment.NewLine on windows
            // is two chars) in the history buffer
            outputBuffered.Write(message.Length - 4 >= 0 ? message.Substring(message.Length - 4) : message, newLine,
                options);
        }

        /// <summary>
        /// Prepend new block if line isn't has NewLine.
        /// </summary>
        private void AutoPrependBlock()
        {
            var fetched = outputBuffered.Fetch().Replace(Environment.NewLine, "\n");
            if (fetched.Length <= 1)
            {
                NewLine();
                return;
            }

            fetched = fetched.Substring(fetched.Length - 2);
            NewLine(2 - Str.SubstringCount(fetched, "\n"));
        }

        /// <summary>
        /// Prepend new line if last char isn't NewLine.
        /// </summary>
        private void AutoPrependText()
        {
            var fetched = outputBuffered.Fetch();

            // Prepend new line if last char isn't EOL:
            if (fetched.Length <= 0 || fetched[fetched.Length - 1] != '\n')
            {
                NewLine();
            }
        }

        /// <summary>
        /// Formats a text as a block of text.
        /// </summary>
        /// <param name="messages">An array of message content.</param>
        /// <param name="type">The block type (added in [] on first line).</param>
        /// <param name="style">The style to apply to the whole block.</param>
        /// <param name="prefix">The prefix for the block.</param>
        /// <param name="padding">Whether to add vertical padding.</param>
        /// <param name="escape">Whether to escape the message.</param>
        /// <returns>The block content.</returns>
        private string[] CreateBlock(string[] messages, string type = null, string style = null, string prefix = null,
            bool padding = false,
            bool escape = false)
        {
            var indentLength = 0;
            var prefixLength = AbstractHelper.StrlenWithoutDecoration(Formatter, prefix);
            var lineIndentation = string.Empty;

            if (type != null)
            {
                type = $"[{type}] ";
                indentLength = type.Length;
                lineIndentation = Str.Pad(indentLength);
            }

            var lines = new LinkedList<string>();

            // wrap and add newlines for each element
            for (var index = 0; index < messages.Length; index++)
            {
                var text = messages[index];
                if (escape)
                {
                    text = OutputFormatter.Escape(text);
                }

                var merges = Str.WordWrap(text, lineLength - prefixLength - indentLength, Environment.NewLine)
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (var merge in merges)
                {
                    lines.AddLast(merge);
                }

                if (messages.Length > 1 && index < messages.Length - 1)
                {
                    lines.AddLast(string.Empty);
                }
            }

            var firstLineIndex = 0;
            if (padding && IsDecorated)
            {
                firstLineIndex = 1;
                lines.AddFirst(string.Empty);
                lines.AddLast(string.Empty);
            }

            var ret = lines.ToArray();
            var line = new StringBuilder();
            for (var index = 0; index < ret.Length; index++)
            {
                line.Append(prefix);

                if (type != null)
                {
                    line.Append(firstLineIndex == index ? type : lineIndentation);
                }

                line.Append(ret[index]);

                line.Append(Str.Pad(
                    lineLength - AbstractHelper.StrlenWithoutDecoration(Formatter, line.ToString())));

                if (!string.IsNullOrEmpty(style))
                {
                    ret[index] = $"<{style}>{line}</>";
                }
                else
                {
                    ret[index] = line.ToString();
                }

                line.Clear();
            }

            return ret;
        }
    }
}
