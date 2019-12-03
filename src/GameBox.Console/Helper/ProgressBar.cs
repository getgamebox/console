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
using GameBox.Console.Output;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GameBox.Console.Helper
{
    /// <summary>
    /// The <see cref="ProgressBar"/> helpers to display progress output.
    /// </summary>
    public class ProgressBar
    {
        /// <summary>
        /// The format normal.
        /// </summary>
        public const string FormatNormal = "normal";

        /// <summary>
        /// The format verbose.
        /// </summary>
        public const string FormatVerbose = "verbose";

        /// <summary>
        /// The format very verbose.
        /// </summary>
        public const string FormatVeryVerbose = "very_verbose";

        /// <summary>
        /// The format debug.
        /// </summary>
        public const string FormatDebug = "debug";

        /// <summary>
        /// The format normal noxmax variants.
        /// </summary>
        public const string FormatNormalNomax = FormatNormal + FormatNomaxVariants;

        /// <summary>
        /// The format verbose noxmax variants.
        /// </summary>
        public const string FormatVerboseNomax = FormatVerbose + FormatNomaxVariants;

        /// <summary>
        /// The format very verbose noxmax variants.
        /// </summary>
        public const string FormatVeryVerboseNomax = FormatVeryVerbose + FormatNomaxVariants;

        /// <summary>
        /// The format debug noxmax variants.
        /// </summary>
        public const string FormatDebugNomax = FormatDebug + FormatNomaxVariants;

        /// <summary>
        /// The format noxmax variants.
        /// </summary>
        public const string FormatNomaxVariants = "_nomax";

        /// <summary>
        /// Whether override the output on the console.
        /// </summary>
        private readonly bool overwrite = true;

        /// <summary>
        /// An <see cref="IOutput"/> instance.
        /// </summary>
        private readonly IOutput output;

        /// <summary>
        /// The message mapping.
        /// </summary>
        private readonly Dictionary<string, string> messages;

        /// <summary>
        /// The format string.
        /// </summary>
        private string format;

        /// <summary>
        /// The key for the format definition.
        /// </summary>
        private string internalFormat;

        /// <summary>
        /// The format definition mapping.
        /// </summary>
        private Dictionary<string, string> formats;

        /// <summary>
        /// The placeholder formatter.
        /// </summary>
        private Dictionary<string, Func<ProgressBar, IOutput, string>> formatters;

        /// <summary>
        /// The redraw frequency.
        /// </summary>
        private int redrawFreq = 1;

        /// <summary>
        /// Whether the first run.
        /// </summary>
        private bool firstRun = true;

        /// <summary>
        /// Format string number of lines.
        /// </summary>
        private int formatLineCount;

        /// <summary>
        /// Returns the progress char.
        /// </summary>
        private string barChar = "\0";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="output">An <see cref="IOutput"/> instance.</param>
        /// <param name="maxSteps">Maximum steps (0 if unknown).</param>
        public ProgressBar(IOutput output, int maxSteps = 0)
        {
            if (output is OutputConsole outputConsole)
            {
                output = outputConsole.GetErrorOutput();
            }

            this.output = output;
            SetMaxSteps(maxSteps);

            if (!output.IsDecorated)
            {
                // disable overwrite when output does not support ANSI codes.
                overwrite = false;

                // set a reasonable redraw frequency so output isn't flooded
                SetRedrawFrequency((int)(maxSteps * 0.1));
            }

            StartTime = DateTime.Now;

            messages = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the current process step.
        /// </summary>
        public int Step { get; private set; }

        /// <summary>
        /// Gets the step width.
        /// </summary>
        public int StepWidth { get; private set; }

        /// <summary>
        /// Gets the maxinum step.
        /// </summary>
        public int MaxSteps { get; private set; }

        /// <summary>
        /// Gets the process step percent.
        /// </summary>
        public float ProgressPercent { get; private set; }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the bar width.
        /// </summary>
        public int BarWidth { get; private set; } = 28;

        /// <summary>
        /// Gets the empty bar char.
        /// </summary>
        public string EmptyBarChar { get; private set; } = "-";

        /// <summary>
        /// Gets the progress char.
        /// </summary>
        public string ProgressChar { get; private set; } = ">";

        /// <inheritdoc cref="barChar"/>
        private string BarChar
        {
            get
            {
                if (barChar == "\0")
                {
                    return MaxSteps > 0 ? "=" : EmptyBarChar;
                }

                return barChar;
            }
        }

        /// <summary>
        /// Sets the max steps.
        /// </summary>
        /// <param name="maxSteps">Maximum steps (0 if unknown).</param>
        public void SetMaxSteps(int maxSteps)
        {
            format = null;
            MaxSteps = Math.Max(0, maxSteps);
            StepWidth = maxSteps > 0 ? Str.Width(maxSteps.ToString()) : 4;
        }

        /// <summary>
        /// Sets the redraw frequency.
        /// </summary>
        /// <param name="freq">The frequency.</param>
        public void SetRedrawFrequency(int freq)
        {
            redrawFreq = Math.Max(freq, 1);
        }

        /// <summary>
        /// Sets the specified format.
        /// </summary>
        /// <param name="format">The key for the format definition.</param>
        public void SetFormat(string format)
        {
            this.format = null;
            internalFormat = format;
        }

        /// <summary>
        /// Sets the progress bar width.
        /// </summary>
        /// <param name="width">The progress bar width.</param>
        public void SetBarWidth(int width)
        {
            BarWidth = Math.Max(1, width);
        }

        /// <summary>
        /// Sets the bar char.
        /// </summary>
        /// <param name="character">The bar char.</param>
        public void SetBarChar(string character)
        {
            barChar = character;
        }

        /// <summary>
        /// Sets the empty bar char.
        /// </summary>
        /// <param name="character">The empty bar char.</param>
        public void SetEmptyBarChar(string character)
        {
            EmptyBarChar = character;
        }

        /// <summary>
        /// Sets the progress char.
        /// </summary>
        /// <param name="character">The progress char.</param>
        public void SetProgressChar(string character)
        {
            ProgressChar = character;
        }

        /// <summary>
        /// Begin the progress output.
        /// </summary>
        public void Begin()
        {
            Begin(MaxSteps);
        }

        /// <summary>
        /// Begin the progress output.
        /// </summary>
        /// <param name="maxStep">The maxinum steps.</param>
        public void Begin(int maxStep)
        {
            StartTime = DateTime.Now;
            Step = 0;
            ProgressPercent = 0;

            SetMaxSteps(maxStep);
            Display();
        }

        /// <summary>
        /// Advances the progress output <see cref="Step" /> steps.
        /// </summary>
        /// <param name="step">The steps.</param>
        public void Advance(int step = 1)
        {
            SetProgress(Step + step);
        }

        /// <summary>
        /// Ended the progress output.
        /// </summary>
        public void End()
        {
            if (MaxSteps <= 0)
            {
                MaxSteps = Step;
            }

            if (Step == MaxSteps && !overwrite)
            {
                // prevent double 100% output
                return;
            }

            SetProgress(MaxSteps);
        }

        /// <summary>
        /// Sets the progress.
        /// </summary>
        /// <param name="step">The current step.</param>
        public void SetProgress(int step)
        {
            if (MaxSteps > 0 && step > MaxSteps)
            {
                MaxSteps = step;
            }
            else if (step < 0)
            {
                step = 0;
            }

            var prevPeriod = Step / redrawFreq;
            var currPeriod = step / redrawFreq;
            Step = step;
            ProgressPercent = MaxSteps > 0 ? (float)Step / MaxSteps : 0;
            if (prevPeriod != currPeriod || MaxSteps == step)
            {
                Display();
            }
        }

        /// <summary>
        /// Outputs the current progress string.
        /// </summary>
        public void Display()
        {
            if (output.IsQuiet)
            {
                return;
            }

            if (string.IsNullOrEmpty(format))
            {
                SetRealFormat(internalFormat ?? GetBestFormat());
            }

            Overwrite(BuildLine());
        }

        /// <summary>
        /// Removes the progress bar from the current line.
        /// </summary>
        /// <remarks>
        /// This is useful if you wish to write some output
        /// while a progress bar is running.
        /// Call Display() to show the progress bar again.
        /// </remarks>
        public void Clear()
        {
            if (!overwrite)
            {
                return;
            }

            if (string.IsNullOrEmpty(format))
            {
                SetRealFormat(internalFormat ?? GetBestFormat());
            }

            Overwrite(string.Empty);
        }

        /// <summary>
        /// Sets a format for a given name.
        /// This method also allow you to override an existing format.
        /// </summary>
        /// <param name="name">The format name.</param>
        /// <param name="format">The format string.</param>
        public void SetFormatDefinition(string name, string format)
        {
            if (formats == null)
            {
                InitFormats();
            }

            formats[name] = format;
        }

        /// <summary>
        /// Sets the placeholder formatter for a given name.
        /// This method also allow you to override an existing formatter.
        /// </summary>
        /// <param name="name">The placeholder name.</param>
        /// <param name="closure">The closure.</param>
        public void SetPlaceholderFormatterDefinition(string name, Func<ProgressBar, IOutput, string> closure)
        {
            if (formatters == null)
            {
                InitPlaceholderFormatters();
            }

            formatters[name] = closure;
        }

        /// <summary>
        /// Gets the placeholder formatter for a given name.
        /// </summary>
        /// <param name="name">The placeholder name.</param>
        /// <returns>The closure.</returns>
        public Func<ProgressBar, IOutput, string> GetPlaceholderFormatterDefinition(string name)
        {
            if (formatters == null)
            {
                InitPlaceholderFormatters();
            }

            return formatters.TryGetValue(name, out Func<ProgressBar, IOutput, string> closure)
                ? closure
                : null;
        }

        /// <summary>
        /// Set the specified placeholder text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="placeholder">The placeholder.</param>
        public void SetMessage(string text, string placeholder = "message")
        {
            messages[placeholder] = text;
        }

        /// <summary>
        /// Gets the specified placeholder text.
        /// </summary>
        /// <param name="placeholder">The placeholder.</param>
        /// <returns>The text.</returns>
        public string GetMessage(string placeholder)
        {
            return messages.TryGetValue(placeholder, out string message)
                ? message
                : null;
        }

        /// <summary>
        /// Overwrites a previous message to the output.
        /// </summary>
        /// <param name="message">The new message.</param>
        private void Overwrite(string message)
        {
            if (overwrite)
            {
                if (!firstRun)
                {
                    if (output is OutputConsoleSection outputConsoleSection)
                    {
                        var lines = (int)Math.Floor((float)Str.Width(message) / Terminal.Width) + formatLineCount + 1;
                        outputConsoleSection.Clear(lines);
                    }
                    else
                    {
                        // Erase previous lines
                        if (formatLineCount > 0)
                        {
                            message = Str.Repeat("\x1B[1A\x1B[2K", formatLineCount) + message;
                        }

                        // Move the cursor to the beginning of the line and erase the line
                        message = $"\x0D\x1B[2K{message}";
                    }
                }
            }
            else if (!firstRun)
            {
                message = Environment.NewLine + message;
            }

            firstRun = false;
            output.Write(message);
        }

        /// <summary>
        /// Gets the best format name.
        /// </summary>
        /// <returns>The best format name.</returns>
        private string GetBestFormat()
        {
            string ret;
            if (output.IsVerbose)
            {
                ret = FormatVerbose;
            }
            else if (output.IsVeryVerbose)
            {
                ret = FormatVeryVerbose;
            }
            else if (output.IsDebug)
            {
                ret = FormatDebug;
            }
            else
            {
                ret = FormatNormal;
            }

            if (MaxSteps <= 0)
            {
                ret += FormatNomaxVariants;
            }

            return ret;
        }

        /// <summary>
        /// Sets the real format string with format definition key.
        /// </summary>
        /// <param name="format">The format definition key.</param>
        private void SetRealFormat(string format)
        {
            this.format = GetFormatDefinition(format) ?? format;
            formatLineCount = Str.SubstringCount(format, "\n");
        }

        /// <summary>
        /// Gets the format definition.
        /// </summary>
        /// <param name="name">The format definition name.</param>
        /// <returns>The format definition string.</returns>
        private string GetFormatDefinition(string name)
        {
            if (formats == null)
            {
                InitFormats();
            }

            return formats.TryGetValue(name, out string format) ? format : null;
        }

        /// <summary>
        /// Inits the format definition.
        /// </summary>
        private void InitFormats()
        {
            formats = new Dictionary<string, string>
            {
                { FormatNormal, " {current}/{max} [{bar}] {percent}%" },
                { FormatNormalNomax, " {current} [{bar}]" },
                { FormatVerbose, " {current}/{max} [{bar}] {percent}% {elapsed}" },
                { FormatVerboseNomax, " {current} [{bar}] {elapsed}" },
                { FormatVeryVerbose, " {current}/{max} [{bar}] {percent}% {elapsed}/{estimated}" },
                { FormatVeryVerboseNomax, " {current} [{bar}] {elapsed}" },
                { FormatDebug, " {current}/{max} [{bar}] {percent}% {elapsed}/{estimated} {memory}" },
                { FormatDebugNomax, " {current} [{bar}] {elapsed} {memory}" },
            };
        }

        /// <summary>
        /// Inits the placeholder formatters.
        /// </summary>
        private void InitPlaceholderFormatters()
        {
            formatters = new Dictionary<string, Func<ProgressBar, IOutput, string>>
            {
                { "elapsed", (bar, _) => AbstractHelper.FormatTime(DateTime.Now.Ticks - StartTime.Ticks) },
                { "memory", (bar, _) => AbstractHelper.FormatMemory(Hardware.GetMemoryUsage()) },
                { "current", (bar, _) => Str.Pad(bar.StepWidth, bar.Step.ToString(), null, Str.PadType.Left) },
                { "max", (bar, _) => bar.MaxSteps.ToString() },
                { "percent", (bar, _) => Math.Floor(bar.ProgressPercent * 100).ToString(CultureInfo.InvariantCulture) },
                {
                    "bar", (bar, _) =>
                    {
                        var completeBars = (int)Math.Floor(bar.MaxSteps > 0
                            ? bar.ProgressPercent * bar.BarWidth
                            : bar.Step % bar.BarWidth);
                        var display = Str.Repeat(BarChar.ToString(), completeBars);

                        if (completeBars >= bar.BarWidth)
                        {
                            return display;
                        }

                        var emptyBars = bar.BarWidth - completeBars - AbstractHelper.StrlenWithoutDecoration(output.Formatter, bar.ProgressChar.ToString());
                        display += bar.ProgressChar + Str.Repeat(bar.EmptyBarChar.ToString(), emptyBars);
                        return display;
                   }
                },
                {
                    "remaining", (bar, _) =>
                    {
                        if (bar.MaxSteps <= 0)
                        {
                            throw new ConsoleLogicException(
                                "Unable to display the remaining time if the maximum number of steps is not set.");
                        }

                        long remaining;
                        if (bar.Step <= 0)
                        {
                            remaining = 0;
                        }
                        else
                        {
                            remaining = (long)Math.Round((double)(DateTime.Now.Ticks - bar.StartTime.Ticks) /
                                                          bar.Step * (bar.MaxSteps - bar.Step));
                        }

                        return AbstractHelper.FormatTime(remaining);
                    }
                },
                {
                    "estimated", (bar, _) =>
                    {
                        if (bar.MaxSteps <= 0)
                        {
                            throw new ConsoleLogicException(
                                "Unable to display the estimated time if the maximum number of steps is not set.");
                        }

                        long estimated;
                        if (bar.Step <= 0)
                        {
                            estimated = 0;
                        }
                        else
                        {
                            estimated = (long)Math.Round((double)(DateTime.Now.Ticks - bar.StartTime.Ticks) / bar.Step * bar.MaxSteps);
                        }

                        return AbstractHelper.FormatTime(estimated);
                    }
                },
            };
        }

        /// <summary>
        /// Build the progress bar info.
        /// </summary>
        /// <returns>The progress bar info.</returns>
        private string BuildLine()
        {
            var regex = "\\{([a-z\\-_]+)(?:\\:([^}]+))?\\}";
            var matches = Regex.Matches(format, regex, RegexOptions.IgnoreCase);
            string line;

            string ReplacementPlaceholder(Match match)
            {
                string text;
                var placeholderFormatter = GetPlaceholderFormatterDefinition(match.Groups[1].Value);
                if (placeholderFormatter != null)
                {
                    text = placeholderFormatter(this, output);
                }
                else if ((text = GetMessage(match.Groups[1].Value)) != null)
                {
                    // No code needs to be executed.
                }
                else
                {
                    return match.Groups[0].Value;
                }

                if (match.Groups.Count >= 3 && !string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    // todo: format the string.
                }

                return text;
            }

            string GetLine()
            {
                line = format;
                foreach (Match match in matches)
                {
                    line = line.Replace(match.Groups[0].Value, ReplacementPlaceholder(match));
                }

                return line;
            }

            line = GetLine();

            // Windows line breaks are \r\n , linux are \n.
            // So we can use \n as a newline. Then trim \r end.
            var linesWidth = Arr.Map(
                line.Split('\n'),
                (subLine) => AbstractHelper.StrlenWithoutDecoration(output.Formatter, subLine.TrimEnd('\r'))).Max();

            if (linesWidth <= Terminal.Width)
            {
                return line;
            }

            SetBarWidth(BarWidth - linesWidth + Terminal.Width);

            return GetLine();
        }
    }
}
