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
    /// Console runtime exception, throwing this exception program all operations that allow the try class should terminate.
    /// </summary>
    public class ConsoleRuntimeException : ConsoleException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRuntimeException"/> class.
        /// </summary>
        public ConsoleRuntimeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRuntimeException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        public ConsoleRuntimeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRuntimeException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="exitCode">The exit code.</param>
        public ConsoleRuntimeException(string message, int exitCode)
            : this(message, exitCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRuntimeException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ConsoleRuntimeException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleRuntimeException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public ConsoleRuntimeException(string message, int exitCode, System.Exception innerException)
            : base(message, exitCode, innerException)
        {
        }
    }
}
