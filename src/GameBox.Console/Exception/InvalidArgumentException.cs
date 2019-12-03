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
    /// The input parameter is invalid.
    /// </summary>
    public class InvalidArgumentException : ArgumentException, IException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArgumentException"/> class.
        /// </summary>
        public InvalidArgumentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArgumentException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        public InvalidArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArgumentException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public InvalidArgumentException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArgumentException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="paramName">Which parameter throws an exception.</param>
        public InvalidArgumentException(string message, string paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidArgumentException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="paramName">Which parameter throws an exception.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        public InvalidArgumentException(string message, string paramName, System.Exception innerException)
            : base(message, paramName, innerException)
        {
        }

        /// <summary>
        /// Gets the exit code.
        /// </summary>
        public virtual int ExitCode => ExitCodes.GeneralException;
    }
}
