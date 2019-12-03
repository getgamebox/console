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
    /// When the command not found.
    /// </summary>
    public class CommandNotFoundException : ConsoleException
    {
        /// <summary>
        /// The command alternatives.
        /// </summary>
        private readonly string[] alternatives;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        public CommandNotFoundException()
        {
            alternatives = Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="alternatives">The command alternatives.</param>
        public CommandNotFoundException(string[] alternatives)
        {
            this.alternatives = alternatives ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="alternatives">The command alternatives.</param>
        public CommandNotFoundException(string message, string[] alternatives = null)
            : base(message)
        {
            this.alternatives = alternatives ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        /// <param name="alternatives">The command alternatives.</param>
        public CommandNotFoundException(string message, System.Exception innerException, string[] alternatives = null)
            : base(message, innerException)
        {
            this.alternatives = alternatives ?? Array.Empty<string>();
        }

        /// <summary>
        /// Gets the command alternatives.
        /// </summary>
        /// <returns>The command alternatives.</returns>
        public string[] GetAlternatives()
        {
            return alternatives;
        }
    }
}
