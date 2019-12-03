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

namespace GameBox.Console.Input
{
    /// <summary>
    /// Definition <see cref="InputOption"/> necessity and mode.
    /// </summary>
    [Flags]
    public enum InputOptionModes
    {
        /// <summary>
        /// The option not accept value
        /// </summary>
        ValueNone = 1,

        /// <summary>
        /// Required value if set the option
        /// </summary>
        ValueRequired = 2,

        /// <summary>
        /// Optional value if set the option
        /// </summary>
        ValueOptional = 4,

        /// <summary>
        /// The option is array
        /// </summary>
        IsArray = 8,
    }
}
