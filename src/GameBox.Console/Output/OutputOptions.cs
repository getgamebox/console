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

namespace GameBox.Console.Output
{
    /// <summary>
    /// The options of the output.
    /// </summary>
    [Flags]
    public enum OutputOptions
    {
        /// <summary>
        /// Default of the options, is considered the same as <see cref="OutputOptions.OutputNormal"/> | <see cref="OutputOptions.VerbosityNormal"/>.
        /// </summary>
        None = 0,

        /// <summary>
        /// Normal of the output.(use output formatter)
        /// </summary>
        OutputNormal = 1,

        /// <summary>
        /// Raw of the output.(do nothing)
        /// </summary>
        OutputRaw = 2,

        /// <summary>
        /// Plain of the output.(use output formatter and strip html label)
        /// </summary>
        OutputPlain = 4,

        /// <summary>
        /// Quiet output the message.
        /// </summary>
        VerbosityQuiet = 16,

        /// <summary>
        /// Normal output the message.
        /// </summary>
        VerbosityNormal = 32,

        /// <summary>
        /// Detail output the message.
        /// </summary>
        VerbosityVerbose = 64,

        /// <summary>
        /// Very Detail output the message.
        /// </summary>
        VerbosityVeryVerbose = 128,

        /// <summary>
        /// Debug output the message.
        /// </summary>
        VerbosityDebug = 256,
    }
}
