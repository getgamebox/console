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
    /// When the namespace not found.
    /// </summary>
    public class NamespaceNotFoundException : CommandNotFoundException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceNotFoundException"/> class.
        /// </summary>
        public NamespaceNotFoundException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceNotFoundException"/> class.
        /// </summary>
        /// <param name="alternatives">The namespace alternatives.</param>
        public NamespaceNotFoundException(string[] alternatives)
            : base(alternatives)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="alternatives">The namespace alternatives.</param>
        public NamespaceNotFoundException(string message, string[] alternatives = null)
            : base(message, alternatives)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The exception message as a single string.</param>
        /// <param name="innerException">The exception as a inner exception.</param>
        /// <param name="alternatives">The namespace alternatives.</param>
        public NamespaceNotFoundException(string message, System.Exception innerException, string[] alternatives = null)
            : base(message, innerException, alternatives)
        {
        }
    }
}
