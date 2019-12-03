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
    /// Definition InputArgument necessity and mode.
    /// </summary>
    [Flags]
    public enum InputArgumentModes
    {
        /// <summary>
        /// The arugment is required
        /// </summary>
        Required = 1,

        /// <summary>
        /// The argument is optional
        /// </summary>
        Optional = 2,

        /// <summary>
        /// The argument is array
        /// </summary>
        IsArray = 4,
    }
}
