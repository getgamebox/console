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

#pragma warning disable S3218

using GameBox.Console.Process;
using System;
using System.IO;
using System.Text;

namespace GameBox.Console.Util
{
    /// <summary>
    /// The terminal helper.
    /// </summary>
    public static class Terminal
    {
        /// <summary>
        /// The process instance.
        /// </summary>
        private static IProcessExecutor process;

        /// <summary>
        /// Internal console implement.
        /// </summary>
        private static IConsole console;

        /// <summary>
        /// Initializes static members of the <see cref="Terminal"/> class.
        /// </summary>
#pragma warning disable S3963
        static Terminal()
        {
            try
            {
                console = new TerminalSystemConsole();
            }
            catch (IOException)
            {
                console = new TerminalNoneConsole();
            }

            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, Width.ToString());
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferHeight, Height.ToString());
        }
#pragma warning restore S3963

        /// <summary>
        /// The console interface.
        /// </summary>
        private interface IConsole
        {
            /// <summary>
            /// Gets the width of buffer area.
            /// </summary>
            int Width { get; }

            /// <summary>
            /// Gets the height of buffer area.
            /// </summary>
            int Height { get; }

            /// <summary>
            /// Gets a value indicating whether input has been redirected from the standard input stream.
            /// </summary>
            bool IsInputRedirected { get; }

            /// <summary>
            /// Gets or sets the encoding the console uses to write output.
            /// </summary>
            Encoding OutputEncoding { get; set; }

            /// <summary>
            /// Gets or sets the encoding the console uses to read input.
            /// </summary>
            Encoding InputEncoding { get; set; }

            /// <summary>
            /// Acquires the standard output stream.
            /// </summary>
            /// <returns>The standard output strean.</returns>
            Stream GetStandardOutput();

            /// <summary>
            /// Acquires the standard error stream.
            /// </summary>
            /// <returns>The standard error stream.</returns>
            Stream GetStandardError();

            /// <summary>
            /// Acquires the standard input stream.
            /// </summary>
            /// <returns>The standard error stream.</returns>
            Stream GetStandardInput();

            /// <summary>
            /// Obtains the next character or function key pressed by the user.
            /// </summary>
            ConsoleKeyInfo ReadKey(bool intercept);
        }

        /// <summary>
        /// Gets the width of buffer area.
        /// </summary>
        public static int Width
        {
            get
            {
                var width = GetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth);

                if (!(width is null))
                {
                    return width;
                }

                return console.Width;
            }
        }

        /// <summary>
        /// Gets the height of buffer area.
        /// </summary>
        public static int Height
        {
            get
            {
                var height = GetEnvironmentVariable(EnvironmentVariables.ConsoleBufferHeight);

                if (!(height is null))
                {
                    return height;
                }

                return console.Height;
            }
        }

        /// <summary>
        /// Gets a value indicating whether input has been redirected from the standard input stream.
        /// </summary>
        public static bool IsInputRedirected => console.IsInputRedirected;

        /// <summary>
        /// Gets or sets the encoding the console uses to write output.
        /// </summary>
        public static Encoding OutputEncoding
        {
            get => console.OutputEncoding;
            set => console.OutputEncoding = value;
        }

        /// <summary>
        /// Gets or sets the encoding the console uses to read input.
        /// </summary>
        public static Encoding InputEncoding
        {
            get => console.InputEncoding;
            set => console.InputEncoding = value;
        }

        /// <summary>
        /// Acquires the standard output stream.
        /// </summary>
        /// <returns>The standard output strean.</returns>
        public static Stream GetStandardOutput()
        {
            return console.GetStandardOutput();
        }

        /// <summary>
        /// Acquires the standard error stream.
        /// </summary>
        /// <returns>The standard error stream.</returns>
        public static Stream GetStandardError()
        {
            return console.GetStandardError();
        }

        /// <summary>
        /// Acquires the standard input stream.
        /// </summary>
        /// <returns>The standard error stream.</returns>
        public static Stream GetStandardInput()
        {
            return console.GetStandardInput();
        }

        /// <summary>
        /// Returns a string array containing the command-line arguments for the current process.
        /// </summary>
        /// <returns>An array of the command line arguments.</returns>
        public static string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }

        /// <summary>
        /// Creates, modifies, or deletes an environment variable stored in the current process
        /// or in the Windows operating system registry key reserved for the current user
        /// or local machine.
        /// </summary>
        /// <param name="variable">The name of the environment variable.</param>
        /// <param name="value">The environment variable value.</param>
        /// <param name="target">Where the variable is saved.</param>
        public static void SetEnvironmentVariable(string variable, Mixture value,
            EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            Environment.SetEnvironmentVariable(variable, value, target);
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process or from
        ///  the Windows operating system registry key for the current user or local machine.
        /// </summary>
        /// <param name="variable">The name of the environment variable.</param>
        /// <param name="target">Where the variable is readed.</param>
        /// <returns>The environment variable value.</returns>
        public static Mixture GetEnvironmentVariable(
            string variable,
            EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            return Environment.GetEnvironmentVariable(variable, target);
        }

        /// <summary>
        /// Remove the exists variable.
        /// </summary>
        /// <param name="variable">The name of the environment variable.</param>
        public static void RemoveEnvironmentVariable(string variable)
        {
            Environment.SetEnvironmentVariable(variable, null);
        }

        /// <inheritdoc cref="IProcessExecutor.Execute"/>
        public static int Execute(string command)
        {
            var ret = Execute(command, out _);
            return ret;
        }

        /// <inheritdoc cref="IProcessExecutor.Execute"/>
        public static int Execute(string command, out string[] stdout, string cwd = null)
        {
            return Execute(command, out stdout, out _, cwd);
        }

        /// <inheritdoc cref="IProcessExecutor.Execute"/>
        public static int Execute(string command, out string[] stdout, out string[] stderr, string cwd = null)
        {
            if (process == null)
            {
                process = new ProcessExecutor();
            }

            return process.Execute(command, out stdout, out stderr, cwd);
        }

        /// <summary>
        /// Sets the process executor.
        /// </summary>
        /// <param name="process">The process executor instance.</param>
        public static void SetProcessExecutor(IProcessExecutor process)
        {
            Terminal.process = process;
        }

        /// <summary>
        /// System console not supported implement.
        /// </summary>
        private sealed class TerminalNoneConsole : IConsole, IDisposable
        {
            /// <summary>
            /// The memory output.
            /// </summary>
            private Stream output;

            /// <summary>
            /// The memory input stream.
            /// </summary>
            private Stream input;

            /// <summary>
            /// The memory error stream.
            /// </summary>
            private Stream error;

            /// <inheritdoc/>
            public int Width => 80;

            /// <inheritdoc/>
            public int Height => 80;

            /// <inheritdoc/>
            public Encoding OutputEncoding
            {
                get => Encoding.UTF8;
                set
                {
                    // ignore.
                }
            }

            /// <inheritdoc/>
            public Encoding InputEncoding
            {
                get => OutputEncoding;
                set
                {
                    // ignore.
                }
            }

            /// <inheritdoc/>
            public bool IsInputRedirected => true;

            /// <inheritdoc/>
            public Stream GetStandardOutput()
            {
                return output ?? (output = new MemoryStream());
            }

            /// <inheritdoc/>
            public Stream GetStandardError()
            {
                return error ?? (error = new MemoryStream());
            }

            /// <inheritdoc/>
            public Stream GetStandardInput()
            {
                return input ?? (input = new MemoryStream());
            }

            /// <inheritdoc/>
            public ConsoleKeyInfo ReadKey(bool intercept)
            {
                throw new NotSupportedException("The analog console does not support this feature.");
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                output?.Dispose();
                input?.Dispose();
                error?.Dispose();
            }
        }

        /// <summary>
        /// System console implement.
        /// </summary>
#pragma warning disable CA1812
        private sealed class TerminalSystemConsole : IConsole, IDisposable
#pragma warning restore CA1812
        {
            /// <summary>
            /// Standard output stream.
            /// </summary>
            private Stream standardOutputStream;

            /// <summary>
            /// Standard input stream.
            /// </summary>
            private Stream standardInputStream;

            /// <summary>
            /// Standard error stream.
            /// </summary>
            private Stream standardErrorStream;

            /// <summary>
            /// Initializes a new instance of the <see cref="TerminalSystemConsole"/> class.
            /// </summary>
            public TerminalSystemConsole()
            {
                // Just test system console whether supported.
#pragma warning disable S1481
                var width = System.Console.BufferWidth;
#pragma warning restore S1481
            }

            /// <inheritdoc/>
            public int Width => System.Console.BufferWidth;

            /// <inheritdoc/>
            public int Height => System.Console.BufferHeight;

            /// <inheritdoc/>
            public bool IsInputRedirected => System.Console.IsInputRedirected;

            /// <inheritdoc/>
            public Encoding OutputEncoding
            {
                get => System.Console.OutputEncoding;
                set => System.Console.OutputEncoding = value;
            }

            /// <inheritdoc/>
            public Encoding InputEncoding
            {
                get => System.Console.InputEncoding;
                set => System.Console.InputEncoding = value;
            }

            /// <inheritdoc/>
            public Stream GetStandardOutput()
            {
                if (standardOutputStream != null && standardOutputStream.CanWrite)
                {
                    return standardOutputStream;
                }

                return standardOutputStream = System.Console.OpenStandardOutput();
            }

            /// <inheritdoc/>
            public Stream GetStandardError()
            {
                if (standardErrorStream != null && standardErrorStream.CanWrite)
                {
                    return standardErrorStream;
                }

                return standardErrorStream = System.Console.OpenStandardError();
            }

            /// <inheritdoc/>
            public Stream GetStandardInput()
            {
                if (standardInputStream != null && standardInputStream.CanRead)
                {
                    return standardInputStream;
                }

                return standardInputStream = new WrappedConsoleStandardInputStream();
            }

            /// <inheritdoc/>
            public ConsoleKeyInfo ReadKey(bool intercept)
            {
                return System.Console.ReadKey(intercept);
            }

            /// <inheritdoc cref="IDisposable.Dispose"/>
            public void Dispose()
            {
                standardOutputStream?.Dispose();
                standardInputStream?.Dispose();
                standardErrorStream?.Dispose();
            }

            /// <summary>
            /// Wrapped console standard input stream.
            /// </summary>
            private sealed class WrappedConsoleStandardInputStream : Stream
            {
                /// <summary>
                /// The base standard input stream.
                /// </summary>
                private readonly Stream baseStream;

                /// <summary>
                /// Initializes a new instance of the <see cref="WrappedConsoleStandardInputStream"/> class.
                /// </summary>
                public WrappedConsoleStandardInputStream()
                {
                    baseStream = System.Console.OpenStandardInput();
                }

                /// <inheritdoc />
                public override bool CanRead => baseStream.CanRead;

                /// <inheritdoc />
                public override bool CanSeek => baseStream.CanSeek;

                /// <inheritdoc />
                public override bool CanWrite => baseStream.CanWrite;

                /// <inheritdoc />
                public override long Length => baseStream.Length;

                /// <inheritdoc />
                public override long Position
                {
                    get => baseStream.Position;
                    set => baseStream.Position = value;
                }

                /// <inheritdoc />
                public override void Flush()
                {
                    baseStream.Flush();
                }

                /// <inheritdoc />
                public override int Read(byte[] buffer, int offset, int count)
                {
                    return baseStream.Read(buffer, offset, count);
                }

                /// <inheritdoc />
                public override long Seek(long offset, SeekOrigin origin)
                {
                    return baseStream.Seek(offset, origin);
                }

                /// <inheritdoc />
                public override void SetLength(long value)
                {
                    baseStream.SetLength(value);
                }

                /// <inheritdoc />
                public override void Write(byte[] buffer, int offset, int count)
                {
                    baseStream.Write(buffer, offset, count);
                }

                /// <inheritdoc cref="IDisposable.Dispose"/>
                protected override void Dispose(bool disposing)
                {
                    baseStream?.Dispose();
                }
            }
        }
    }
}
