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

namespace GameBox.Console.Output
{
    /// <summary>
    /// <see cref="IOutputConsole"/> is the interface implemented by all console output class.
    /// This adds information about stderr output stream.
    /// </summary>
    public interface IOutputConsole : IOutput
    {
        /// <summary>
        /// Gets the stderr output interface.
        /// </summary>
        /// <returns>The stderr output value.</returns>
        IOutput GetErrorOutput();

        /// <summary>
        /// Sets the stderr output in current console output.
        /// </summary>
        /// <param name="stderr">The stderr output value.</param>
        void SetErrorOutput(IOutput stderr);
    }
}
