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

using GameBox.Console.Exception;

namespace GameBox.Console.Command
{
    /// <summary>
    /// <see cref="ICommandLoader"/> is the interface implemented by all command loader classes.
    /// </summary>
    /// <remarks>
    /// The motivation behind <see cref="ICommandLoader"/> is lazy-loading,
    /// it only maps command names to command factories, which means it
    /// can't use <see cref="Command.GetAliases()"/> as it does not deal with
    /// command instances. Thus, if you try to run a command by using
    /// some:alias where some:alias is defined via <see cref="Command.SetAlias"/>()
    /// in the command, the command won't be resolved and you will get
    /// a <see cref="CommandNotFoundException"/>.
    /// </remarks>
    public interface ICommandLoader
    {
        /// <summary>
        /// Loads a command.
        /// </summary>
        /// <param name="name">The name for command.</param>
        /// <returns>The <see cref="Command"/> instance.</returns>
        Command Load(string name);

        /// <summary>
        /// Checks if a command exists.
        /// </summary>
        /// <param name="name">The name for command.</param>
        /// <returns>true if the <see cref="Command"/> object exists, false otherwise.</returns>
        bool Has(string name);

        /// <summary>
        /// Get all registered command names(not include alias).
        /// </summary>
        /// <returns>All registered command names.</returns>
        string[] GetNames();
    }
}
