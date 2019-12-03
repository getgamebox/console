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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using SProcess = System.Diagnostics.Process;
using STimeout = System.Threading.Timeout;

namespace GameBox.Console.Process
{
    /// <inheritdoc />
    public class ProcessExecutor : IProcessExecutor
    {
        private readonly LinkedList<string> output;
        private readonly LinkedList<string> error;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessExecutor"/> class.
        /// </summary>
        public ProcessExecutor()
        {
            output = new LinkedList<string>();
            error = new LinkedList<string>();
        }

        /// <summary>
        /// Gets or sets the milliseconds of timeout.
        /// </summary>
        public virtual int Timeout { get; set; } = STimeout.Infinite;

        /// <summary>
        /// Escapes a string to be used as a shell argument.
        /// </summary>
        /// <param name="argument">The argument that will be escaped.</param>
        /// <returns>The escaped argument.</returns>
        public static string Escape(string argument)
        {
            if (string.IsNullOrEmpty(argument))
            {
                return "\"\"";
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"'{argument.Replace("\'", "'\\''")}'";
            }

            if (argument.Contains("\0"))
            {
                argument = argument.Replace("\0", "?");
            }

            if (!Regex.IsMatch(argument, "[\\/()%!^\"<>&|\\s]"))
            {
                return argument;
            }

            var escapedArgument = new StringBuilder();
            var quote = false;

            bool IsSurroundedBy(string arg, char c)
            {
                return arg.Length > 2 && c == arg[0] && c == arg[arg.Length - 1];
            }

            foreach (var part in Regex.Split(argument, "(\")"))
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                if (part == "\"")
                {
                    escapedArgument.Append(@"\""");
                }
                else if (IsSurroundedBy(part, '%'))
                {
                    // Avoid environment variable expansion
                    escapedArgument.Append($"^%\"{part.Substring(1, part.Length - 2)}\"^%");
                }
                else
                {
                    escapedArgument.Append(part);

                    // Escape trailing backslash
                    if (part[part.Length - 1] == '\\')
                    {
                        escapedArgument.Append("\\");
                    }

                    quote = true;
                }
            }

            if (quote)
            {
                return $"\"{escapedArgument.ToString()}\"";
            }

            return escapedArgument.ToString();
        }

        /// <inheritdoc />
        public virtual int Execute(string command, out string[] stdout, out string[] stderr, string cwd = null)
        {
            using (var process = CreateShellProcess())
            {
                output.Clear();
                error.Clear();

                process.StartInfo.Arguments = GetShellCommand(command);
                process.StartInfo.WorkingDirectory = cwd ?? Environment.CurrentDirectory;
                process.Start();

                process.ErrorDataReceived += (sender, eventArgs) =>
                {
                    error.AddLast(eventArgs.Data);
                };

                process.OutputDataReceived += (sender, eventArgs) =>
                {
                    output.AddLast(eventArgs.Data);
                };

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                try
                {
                    if (!process.WaitForExit(Timeout))
                    {
                        throw new TimeoutException($"Command \"{command}\" execute timeout({Timeout} ms)");
                    }
                }
                finally
                {
                    process.KillTree((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                }

                stdout = output.ToArray();
                stderr = error.ToArray();

                return process.ExitCode;
            }
        }

        /// <summary>
        /// Create a new process instance.
        /// </summary>
        /// <returns>The process instance.</returns>
        protected virtual SProcess CreateShellProcess()
        {
            return new SProcess
            {
                StartInfo =
                {
                    FileName = GetShellPath(),
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                },
            };
        }

        /// <summary>
        /// Returns the name of the shell to the operating system.
        /// </summary>
        /// <returns>The name of the shell.</returns>
        protected virtual string GetShellPath()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "cmd.exe";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "/bin/bash";
            }

            throw new NotSupportedException("The operating system does not support executing the command line.");
        }

        /// <summary>
        /// Returns the shell command argument.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The shell command.</returns>
        protected virtual string GetShellCommand(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"/c \"{command}\"";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"-c \"{command}\"";
            }

            throw new NotSupportedException("The operating system does not support executing the command line.");
        }
    }
}
