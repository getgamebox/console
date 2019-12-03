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
using GameBox.Console.Output;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameBox.Console.Policy.Render
{
    /// <summary>
    /// Default exception render.
    /// </summary>
    public sealed class RenderException
    {
        /// <summary>
        /// Renders a caught exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="output">The standard error instance.</param>
        public static void Render(System.Exception exception, IOutput output)
        {
            var index = 0;
            do
            {
                if (index++ > 0)
                {
                    output.WriteLine(string.Empty);
                }

                var message = exception.Message;
                var maxLength = 0;
                var frameworkException = exception as IException;
                var verbose = output.IsVerbose || output.IsVeryVerbose || output.IsDebug;
                string title = string.Empty;

                if (string.IsNullOrEmpty(message) || verbose)
                {
                    var exitCode = -1;
                    if (frameworkException != null)
                    {
                        exitCode = frameworkException.ExitCode;
                    }

                    if (exitCode != -1)
                    {
                        title = $"  [{exception.GetType()} ({exitCode})]  ";
                    }
                    else
                    {
                        title = $"  [{exception.GetType()}]  ";
                    }

                    maxLength = Str.Width(title);
                }

                var width = Terminal.Width > 0 ? Terminal.Width - 1 : int.MaxValue;
                var lines = new LinkedList<(string message, int length)>();

                // First we loop through the line breaks and then process the width of the screen.
                if (!string.IsNullOrEmpty(message))
                {
                    // \r\n and \n We all consider valid line breaks to
                    // handle problems on different platforms
                    var segments = Regex.Split(message, "\r?\n");
                    foreach (var segment in segments)
                    {
                        foreach (var line in SplitStringByWidth(segment, width - 4))
                        {
                            // pre-format lines to get the right string length
                            var lineLength = Str.Width(line) + 4;
                            lines.AddLast((line, lineLength));
                            maxLength = Math.Max(lineLength, maxLength);
                        }
                    }
                }

                var messages = new LinkedList<string>();

                if (string.IsNullOrEmpty(message) || verbose)
                {
                    var target = "Source";
                    var line = string.Empty;
                    if (exception.TargetSite != null)
                    {
                        if (exception.TargetSite.ReflectedType.Assembly.GetName().Name == exception.Source)
                        {
                            target = "Assembly";
                        }

                        var targetSite = exception.TargetSite.ReflectedType.Name + "." + exception.TargetSite.Name;
                        line += $"Method \"{targetSite}()\" ";
                    }

                    line += $"In \"{exception.Source}\" {target}:";
                    messages.AddLast($"<comment>{OutputFormatter.Escape(line)}</comment>");
                }

                var emptyLine = $"<error>{Str.Pad(maxLength)}</error>";
                messages.AddLast(emptyLine);

                if (string.IsNullOrEmpty(message) || verbose)
                {
                    var padLength = Math.Max(0, maxLength - Str.Width(title));
                    messages.AddLast($"<error>{title}{Str.Pad(padLength)}</error>");
                }

                foreach (var line in lines)
                {
                    messages.AddLast($"<error>  {OutputFormatter.Escape(line.message)}  {Str.Pad(maxLength - line.length)}</error>");
                }

                messages.AddLast(emptyLine);

                output.WriteLine(messages, OutputOptions.VerbosityQuiet);

                if (!verbose)
                {
                    continue;
                }

                output.WriteLine("  <comment>Exception trace:</comment>", OutputOptions.VerbosityQuiet);

                foreach (var stackTrace in ParseStackTrace(exception))
                {
                    if (!string.IsNullOrEmpty(stackTrace.Fallback))
                    {
                        output.WriteLine(stackTrace.Fallback);
                        continue;
                    }

                    var namespaceName = string.IsNullOrEmpty(stackTrace.Namespace) ? "[namespace]" : stackTrace.Namespace;
                    var className = string.IsNullOrEmpty(stackTrace.ClassName) ? "[class]" : stackTrace.ClassName;
                    var methodName = string.IsNullOrEmpty(stackTrace.MethodName) ? "[method]" : stackTrace.MethodName;
                    var filePath = string.IsNullOrEmpty(stackTrace.FilePath) ? "[file]" : stackTrace.FilePath;
                    var line = stackTrace.Line < 0 ? "n/a" : stackTrace.Line.ToString();
                    var paramters = string.Empty;

                    if (stackTrace.Parameters != null && stackTrace.Parameters.Length > 0)
                    {
                        paramters = string.Join(", ", Arr.Map(stackTrace.Parameters, (item) => item.Type));
                    }

                    if (output.IsDebug)
                    {
                        output.WriteLine($"    in {namespaceName}.{className}.{methodName}({paramters}) at <info>{filePath}:{line}</info>", OutputOptions.VerbosityQuiet);
                    }
                    else if (output.IsVeryVerbose)
                    {
                        output.WriteLine($"    in {className}.{methodName}({paramters}) at <info>{filePath}:{line}</info>", OutputOptions.VerbosityQuiet);
                    }
                    else
                    {
                        output.WriteLine($"    in {className}.{methodName}() at <info>{filePath}:{line}</info>", OutputOptions.VerbosityQuiet);
                    }
                }

                if (!string.IsNullOrEmpty(exception.HelpLink))
                {
                    output.WriteLine($"    Help link:{exception.HelpLink}");
                }

                output.WriteLine(string.Empty);
            }
            while ((exception = exception.InnerException) != null);
        }

        /// <summary>
        /// Split the specified string into multiple lines based on width.
        /// </summary>
        /// <param name="input">The specified string.</param>
        /// <param name="width">The width.</param>
        /// <returns>The multiple lines.</returns>
        internal static IEnumerable<string> SplitStringByWidth(string input, int width)
        {
            var builder = new StringBuilder();
            var lineLength = 0;
            foreach (var c in input)
            {
                // Handling double-width characters.
                var charLength = Str.Width(c);
                if (lineLength + charLength <= width)
                {
                    builder.Append(c);
                    lineLength += charLength;
                    continue;
                }

                if (builder.Length > 0)
                {
                    yield return builder.ToString();
                    builder.Clear();
                }

                builder.Append(c);
                lineLength = charLength;
            }

            if (builder.Length > 0)
            {
                yield return builder.ToString();
            }
        }

        /// <summary>
        /// Parse the stack trace from exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>An array of stack trace.</returns>
        private static StackTrace[] ParseStackTrace(System.Exception exception)
        {
            var frames = exception.StackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var regex = new Regex(@"at ([^\)]*\)) in (.*):line (\d*)$");
            var traces = new LinkedList<StackTrace>();
            foreach (var frame in frames)
            {
                var match = regex.Match(frame);
                if (match.Groups.Count != 4)
                {
                    traces.AddLast(new StackTrace() { Fallback = frame });
                    continue;
                }

                try
                {
                    var methodInfo = new MethodInfo(match.Groups[1].Value);
                    var filePath = match.Groups[2].Value;
                    if (!int.TryParse(match.Groups[3].Value, out int line))
                    {
                        line = -1;
                    }

                    traces.AddLast(new StackTrace()
                    {
                        FilePath = filePath,
                        ClassName = methodInfo.ClassName,
                        MethodName = methodInfo.MethodName,
                        Namespace = methodInfo.Namespace,
                        Parameters = methodInfo.Parameters,
                        Line = line,
                    });
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (System.Exception)
                {
                    traces.AddLast(new StackTrace() { Fallback = frame });
                    continue;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return traces.ToArray();
        }

        /// <summary>
        /// The stack trace.
        /// </summary>
        private sealed class StackTrace
        {
            /// <summary>
            /// Gets or sets the file path.
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// Gets or sets the class name.
            /// </summary>
            public string ClassName { get; set; }

            /// <summary>
            /// Gets or sets the namespace name.
            /// </summary>
            public string Namespace { get; set; }

            /// <summary>
            /// Gets or sets the method name.
            /// </summary>
            public string MethodName { get; set; }

            /// <summary>
            /// Gets or sets the method parameters.
            /// </summary>
            public (string Type, string Name)[] Parameters { get; set; }

            /// <summary>
            /// Gets or sets the line. -1 means unknow.
            /// </summary>
            public int Line { get; set; }

            /// <summary>
            /// Gets or sets content for fallback when parsing fails.
            /// </summary>
            public string Fallback { get; set; }
        }

        /// <summary>
        /// The method info parser.
        /// </summary>
        private sealed class MethodInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MethodInfo"/> class.
            /// </summary>
            /// <param name="method">The method info.</param>
            public MethodInfo(string method)
            {
                Parse(method);
            }

            /// <summary>
            /// Gets the classname.
            /// </summary>
            public string ClassName { get; private set; }

            /// <summary>
            /// Gets the namespace name.
            /// </summary>
            public string Namespace { get; private set; }

            /// <summary>
            /// Gets the method name.
            /// </summary>
            public string MethodName { get; private set; }

            /// <summary>
            /// Gets the parameter list.
            /// </summary>
            public (string Type, string Name)[] Parameters { get; private set; }

            /// <summary>
            /// Create a new unable parse exception.
            /// </summary>
            /// <param name="method">The method string.</param>
            /// <returns>The exception instance.</returns>
            private static System.Exception CreateUnableParseException(string method)
            {
                throw new ConsoleException($"Unable to parse the exception stack:{method}");
            }

            /// <summary>
            /// Parse the method info.
            /// </summary>
            /// <param name="method">The method string.</param>
            private void Parse(string method)
            {
                var match = Regex.Match(method, @"(.*)\((.*)\)");
                if (match.Groups.Count != 3)
                {
                    throw CreateUnableParseException(method);
                }

                var namespaceAndClassAndMethod = match.Groups[1].Value;
                var namespaceAndClassAndMethodSegment = namespaceAndClassAndMethod.Split('.');

                if (namespaceAndClassAndMethodSegment.Length <= 1)
                {
                    throw CreateUnableParseException(method);
                }
                else if (namespaceAndClassAndMethodSegment.Length == 2)
                {
                    ClassName = namespaceAndClassAndMethodSegment[0];
                    MethodName = namespaceAndClassAndMethodSegment[1];
                }
                else
                {
                    MethodName = namespaceAndClassAndMethodSegment[namespaceAndClassAndMethodSegment.Length - 1];
                    ClassName = namespaceAndClassAndMethodSegment[namespaceAndClassAndMethodSegment.Length - 2];
                    Namespace = string.Join(".", namespaceAndClassAndMethodSegment, 0, namespaceAndClassAndMethodSegment.Length - 2);
                }

                var parameterSegments = match.Groups[2].Value.Split(',');
                var parameters = new LinkedList<(string, string)>();
                foreach (var segment in parameterSegments)
                {
                    var typeAndName = segment.Split(new[] { Str.Space }, StringSplitOptions.RemoveEmptyEntries);
                    parameters.AddLast((typeAndName[0], typeAndName[1]));
                }

                Parameters = parameters.ToArray();
            }
        }
    }
}
