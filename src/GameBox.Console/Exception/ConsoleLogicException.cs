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

namespace GameBox.Console.Exception
{
    /// <summary>
    /// Generic logic exception class for console application.
    /// </summary>
    public class ConsoleLogicException : ConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogicException"/> class.
        /// </summary>
        public ConsoleLogicException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogicException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        public ConsoleLogicException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogicException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="exitCode">The exit code.</param>
        public ConsoleLogicException(string message, int exitCode)
            : this(message, exitCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogicException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ConsoleLogicException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogicException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ConsoleLogicException(string message, int exitCode, System.Exception innerException)
            : base(message, exitCode, innerException)
        {
        }
    }
}
