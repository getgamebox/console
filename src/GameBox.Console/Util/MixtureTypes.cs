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

namespace GameBox.Console.Util
{
    /// <summary>
    /// The type of the original object with <see cref="Mixture"/>.
    /// </summary>
    [Flags]
    public enum MixtureTypes
    {
        /// <summary>
        /// The original object is null.
        /// </summary>
        None = 0,

        /// <summary>
        /// The original object is array.
        /// </summary>
        Array = 1,

        /// <summary>
        /// The original object is string.
        /// </summary>
        StringValue = 16,

        /// <summary>
        /// The original object is char.
        /// </summary>
        CharValue = 32,

        /// <summary>
        /// The original object is boolean.
        /// </summary>
        BoolValue = 64,

        /// <summary>
        /// The original object is int.
        /// </summary>
        IntValue = 128,

        /// <summary>
        /// The original object is float.
        /// </summary>
        FloatValue = 256,

        /// <summary>
        /// The original object is double.
        /// </summary>
        DoubleValue = 512,
    }
}
