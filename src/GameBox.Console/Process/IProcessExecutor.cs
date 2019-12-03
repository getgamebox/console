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

namespace GameBox.Console.Process
{
    /// <summary>
    /// Represents a command line process executor.
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        /// Executes the given command with command line.
        /// </summary>
        /// <param name="command">The command that will be executed.</param>
        /// <param name="stdout">An array of output from the command.</param>
        /// <param name="stderr">An array of error output from the command.</param>
        /// <param name="cwd">Specify the execution path of the command.</param>
        /// <returns>The return status of the executed command.</returns>
        int Execute(string command, out string[] stdout, out string[] stderr, string cwd = null);
    }
}
