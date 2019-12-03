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
using GameBox.Console.Input;
using GameBox.Console.Output;
using GameBox.Console.Question;
using GameBox.Console.Util;
using System;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace GameBox.Console.Helper
{
    /// <summary>
    /// The <see cref="HelperQuestion"/> class provides helpers to interact with the user.
    /// </summary>
    public class HelperQuestion : AbstractHelper
    {
        /// <summary>
        /// Whether enable stty command.
        /// </summary>
        private static bool? stty;

        /// <summary>
        /// A valid unix shell.
        /// </summary>
        private static string shell;

        /// <summary>
        /// The input stream.
        /// </summary>
        private Stream inputStream;

        /// <inheritdoc />
        public override string Name => "question";

        /// <summary>
        /// Gets or sets the text encoding.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Sets whether usage of stty.
        /// </summary>
        /// <param name="enable">Whether the stty enabled.</param>
        public static void SetSttyEnable(bool enable)
        {
            stty = enable;
        }

        /// <summary>
        /// Asks a question to the user.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        /// <param name="question">The question.</param>
        /// <returns>The user answer.</returns>
        public Mixture Ask(IInput input, IOutput output, Question.Question question)
        {
            if (output is OutputConsole outputConsole)
            {
                output = outputConsole.GetErrorOutput();
            }

            if (!input.IsInteractive)
            {
                return DoAskNoInteractive(output, question);
            }

            if (input is IInputStreamable streamable)
            {
                inputStream = streamable.GetInputStream();
            }

            inputStream = inputStream ?? Terminal.GetStandardInput();

            if (question.GetValidator() == null)
            {
                return DoAsk(input, output, question);
            }

            Mixture Interviewer()
            {
                return DoAsk(input, output, question);
            }

            return ValidateAttempts(Interviewer, output, question);
        }

        /// <summary>
        /// Outputs an error message.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="exception">The exception instance.</param>
        protected virtual void WriteError(IOutput output, System.Exception exception)
        {
            // todo: use formatter
            var message = $"<error>{exception.Message}</error>";
            output.WriteLine(message);
        }

        /// <summary>
        /// Outputs the question prompt.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="question">The question.</param>
        protected virtual void WritePrompt(IOutput output, Question.Question question)
        {
            var message = question.GetQuestion();

            if (!(question is QuestionChoice))
            {
                output.Write(message);
                return;
            }

            var questionChoice = (QuestionChoice)question;
            var choices = questionChoice.GetChoices();
            var maxWidth = Arr.Map(choices, (choice) => choice.Key.Length).Max();

            output.WriteLine(message);
            foreach (var item in choices)
            {
                var width = maxWidth - item.Key.Length;
                output.WriteLine($"  [<info>{item.Key}{Str.Pad(width)}</info>] {item.Value}");
            }

            output.Write(questionChoice.GetPrompt());
        }

        /// <summary>
        /// Gets a password input from user.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The user input value.</returns>
        protected virtual Mixture GetPasswordInput(IOutput output, Stream inputStream)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var answer = new StringBuilder();
                do
                {
                    // todo: this code will make it impossible to test.
                    var key = System.Console.ReadKey(true);
                    if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                    {
                        answer.Append(key.KeyChar);
                    }
                    else if (key.Key == ConsoleKey.Backspace && answer.Length > 0)
                    {
                        answer.Remove(answer.Length - 1, 1);
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
                while (true);

                return answer.ToString();
            }

            if (HasSttyAvailable())
            {
                Terminal.Execute("stty -g", out string[] sttyMode);
                Terminal.Execute("stty -echo");

                var value = ReadInput(inputStream, Encoding);

                Terminal.Execute($"stty {string.Join(Environment.NewLine, sttyMode).TrimEnd()}");

                output.WriteLine(string.Empty);
                return value;
            }

            var validShell = GetShell();
            if (string.IsNullOrEmpty(validShell))
            {
                throw new ConsoleRuntimeException($"Unable to hide the response.");
            }

            var readCmd = validShell == "csh" ? "set mypassword = $<" : "read -r mypassword";
            var command = $"/usr/bin/env {validShell} -c 'stty -echo; {readCmd}; stty echo; echo $mypassword'";
            Terminal.Execute(command, out string[] stdout);
            output.WriteLine(string.Empty);

            return string.Join(Environment.NewLine, stdout).TrimEnd();
        }

        /// <summary>
        /// Returns whether Stty command is available or not.
        /// </summary>
        /// <returns>Whether stty command is available.</returns>
        private static bool HasSttyAvailable()
        {
            if (stty != null)
            {
                return stty.Value;
            }

            stty = Terminal.Execute("stty 2>&1") == 0;
            return stty.Value;
        }

        /// <summary>
        /// Returns a valid unix shell.
        /// </summary>
        /// <returns>The valid shell name, null in case no valid shell is found.</returns>
        private static string GetShell()
        {
            if (shell != null || !File.Exists("/usr/bin/env"))
            {
                return shell;
            }

            shell = string.Empty;

            // handle other OSs with bash/zsh/ksh/csh if available to hide the answer
            var test = "/usr/bin/env {0} -c 'echo OK' 2> /dev/null";
            foreach (var sh in new[] { "bash", "zsh", "ksh", "csh" })
            {
                Terminal.Execute(string.Format(test, sh), out string[] stdout);

                if (stdout.Length >= 1 && stdout[0] == "OK")
                {
                    shell = sh;
                    break;
                }
            }

            return shell;
        }

        /// <summary>
        /// Asks the question to the user.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        /// <param name="question">The question.</param>
        /// <returns>The user answer.</returns>
        private Mixture DoAsk(IInput input, IOutput output, Question.Question question)
        {
            WritePrompt(output, question);

            var autocomplete = question.GetAutocompleterValues();

            Mixture ret = null;
            if (autocomplete is null || !HasSttyAvailable())
            {
                if (question.IsPassword)
                {
                    try
                    {
                        ret = GetPasswordInput(output, inputStream);
                    }
#pragma warning disable CA1031
                    catch (System.Exception) when (question.HasPasswordFallback)
#pragma warning restore CA1031
                    {
                        // ignore exception when has password fallback.
                    }
                }

                ret = ret ?? ReadInput(inputStream, Encoding ?? input.Encoding);
            }
            else
            {
                ret = AutoComplete(output, question, inputStream, autocomplete);
            }

            var section = output as OutputConsoleSection;
            section?.AddContent(ret);

            ret = (!(ret is null) && ret.Length > 0) ? ret : question.GetDefault();

            var normalizer = question.GetNormalizer();
            if (normalizer != null)
            {
                ret = normalizer(ret);
            }

            return ret;
        }

        /// <summary>
        /// Get results when no interaction occurs.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="question">The question.</param>
        /// <returns>The default value.</returns>
        private Mixture DoAskNoInteractive(IOutput output, Question.Question question)
        {
            var defaultValue = question.GetDefault();
            if (defaultValue is null || !(question is QuestionChoice))
            {
                goto normalizer;
            }

            var questionChoice = (QuestionChoice)question;
            if (!questionChoice.IsMultiselect)
            {
                goto normalizer;
            }

        normalizer:
            var normalizer = question.GetNormalizer();
            if (normalizer != null)
            {
                defaultValue = normalizer(defaultValue);
            }

            if (question.GetValidator() == null)
            {
                return defaultValue;
            }

            Mixture Interviewer()
            {
                return defaultValue;
            }

            return ValidateAttempts(Interviewer, output, question);
        }

        /// <summary>
        /// Autocompletes a question.
        /// </summary>
        /// <remarks>Auto-completion can help users get problem completion through tabs.</remarks>
        /// <param name="output">The std output instance.</param>
        /// <param name="question">The question.</param>
        /// <param name="inputStream">The std input stream.</param>
        /// <param name="autocomplete">The autocompletes value.</param>
        /// <returns>Returns the autocompletes value.</returns>
        private Mixture AutoComplete(IOutput output, Question.Question question, Stream inputStream,
            Mixture autocomplete)
        {
            // todo: implement the AutoComplete method.
            throw new NotImplementedException($"{nameof(AutoComplete)} is not implemented.");
        }

        /// <summary>
        /// Read the user input from <paramref name="inputStream"/>.
        /// </summary>
        private Mixture ReadInput(Stream inputStream, Encoding encoding = null)
        {
            Mixture ret = null;
            encoding = encoding ?? Encoding ?? Encoding.UTF8;

            // We only correct the position when we allow seek.
            long position = 0;
            if (inputStream.CanSeek)
            {
                position = inputStream.Position;
            }

            using (var reader = new StreamReader(inputStream, encoding,
                true, 128, true))
            {
                ret = reader.ReadLine();

                // We skip this step directly when it is not allowed,
                // because there will be no bugs even if you do not
                // use position correction under the console program.
                // It contains a potential problem:
                // !5
                if (inputStream.CanSeek && !(ret is null))
                {
                    inputStream.Seek(
                        position + encoding.GetByteCount(ret.ToString()) + encoding.GetByteCount(Environment.NewLine),
                        SeekOrigin.Begin);
                }
            }

            return ret;
        }

        /// <summary>
        /// Validates an attempt.
        /// </summary>
        /// <param name="interviewer"> A callable that will ask for a question and return the result.</param>
        /// <param name="output">The std output instance.</param>
        /// <param name="question">The question instance.</param>
        /// <returns>The user input value.</returns>
        private Mixture ValidateAttempts(Func<Mixture> interviewer, IOutput output, Question.Question question)
        {
            var attempts = question.MaxAttempts;
            var permanent = attempts <= 0;
            ExceptionDispatchInfo exception = null;

            while (permanent || --attempts >= 0)
            {
                if (exception != null)
                {
                    WriteError(output, exception.SourceException);
                }

                try
                {
                    return question.GetValidator()(interviewer());
                }
                catch (System.Exception e) when (!(e is ConsoleRuntimeException))
                {
                    exception = ExceptionDispatchInfo.Capture(e);
                }
            }

            exception?.Throw();
            throw new ConsoleException($"{nameof(ValidateAttempts)} assert exception.");
        }
    }
}
