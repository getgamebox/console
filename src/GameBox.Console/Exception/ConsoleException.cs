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

namespace GameBox.Console.Exception
{
    /// <summary>
    /// Generic exception class for console application.
    /// </summary>
    public class ConsoleException : System.Exception, IException
    {
        /// <summary>
        /// The exit code.
        /// </summary>
        private readonly int exitCode = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleException"/> class.
        /// </summary>
        public ConsoleException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        public ConsoleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="exitCode">The exit code.</param>
        public ConsoleException(string message, int exitCode)
            : this(message, exitCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ConsoleException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ConsoleException(string message, int exitCode, System.Exception innerException)
            : base(message, innerException)
        {
            this.exitCode = exitCode;
        }

        /// <summary>
        /// Gets the exit code.
        /// </summary>
        public virtual int ExitCode => exitCode < 0 ? ExitCodes.GeneralException : Math.Min(exitCode, 255);
    }
}
