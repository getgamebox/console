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
using System.Collections.Generic;
using System.Linq;

namespace GameBox.Console.Descriptor
{
    /// <summary>
    /// The application description classes.
    /// </summary>
    public class DescriptionApplication
    {
        /// <summary>
        /// The global namespace id.
        /// </summary>
        public const string GlobalNamespace = "_global";

        /// <summary>
        /// The application instance.
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// The specified namespace.
        /// </summary>
        private readonly string @namespace;

        /// <summary>
        /// Whether the command should be publicly shown or not.
        /// </summary>
        private readonly bool showHidden;

        /// <summary>
        /// The generted namespace.
        /// </summary>
        private readonly List<(string id, string[] commandNames)> namespaces;

        /// <summary>
        /// The generted commands.
        /// </summary>
        private readonly Dictionary<string, Command.Command> commands;

        /// <summary>
        /// The commands aliases.
        /// </summary>
        private readonly Dictionary<string, Command.Command> aliases;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionApplication"/> class.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="namespace">The specified namespace.</param>
        /// <param name="showHidden">Whether the command should be publicly shown or not.</param>
        public DescriptionApplication(Application application, string @namespace = null, bool showHidden = false)
        {
            this.application = application;
            this.@namespace = @namespace;
            this.showHidden = showHidden;
            commands = new Dictionary<string, Command.Command>();
            aliases = new Dictionary<string, Command.Command>();
            namespaces = new List<(string id, string[] commandNames)>();
            InspectApplication();
        }

        /// <summary>
        /// Return the commands, Exclude aliases.
        /// </summary>
        /// <returns>The command array.</returns>
        public Command.Command[] GetCommandsExcludeAliases()
        {
            // Alias and namespace matching will not be included in the display list.
            return commands.Values.ToArray();
        }

        /// <summary>
        /// Return the specified command.
        /// </summary>
        /// <param name="name">The name or alias.</param>
        /// <returns>The specified command.</returns>
        public Command.Command GetCommand(string name)
        {
            if (!commands.TryGetValue(name, out Command.Command command)
                && !aliases.TryGetValue(name, out command))
            {
                throw new CommandNotFoundException($"Command {name} does not exist.");
            }

            return command;
        }

        /// <summary>
        /// Return to the namespace and the list of commands under the namespace.
        /// </summary>
        /// <remarks>The command name list include the command aliases name.</remarks>
        /// <returns>The namespace result array.</returns>
        public (string @namespace, string[] commandNames)[] GetNamespaces()
        {
            return namespaces.ToArray();
        }

        /// <summary>
        /// Sorted the commands. according to namespace and name.
        /// </summary>
        /// <param name="items">The command array.</param>
        /// <returns>The sorted namespace and commands.</returns>
        private static IEnumerable<(string @namespace, (string nameOrAlias, Command.Command command)[] commands)> SortCommands(
            IEnumerable<(string nameOrAlias, Command.Command command)> items)
        {
            var namespacedCommands = new Dictionary<string, List<(string nameOrAlias, Command.Command command)>>();
            var gloablNamespacedCommands = new List<(string nameOrAlias, Command.Command command)>();
            foreach (var item in items)
            {
                var key = Application.ExtractNamespace(item.nameOrAlias, 1);
                if (string.IsNullOrEmpty(key))
                {
                    gloablNamespacedCommands.Add(item);
                    continue;
                }

                if (!namespacedCommands.TryGetValue(key, out List<(string nameOrAlias, Command.Command command)> collect))
                {
                    namespacedCommands[key] = collect = new List<(string nameOrAlias, Command.Command command)>();
                }

                collect.Add(item);
            }

            gloablNamespacedCommands.Sort((left, right)
                => string.CompareOrdinal(left.nameOrAlias, right.nameOrAlias));
            yield return (GlobalNamespace, gloablNamespacedCommands.ToArray());

            var orderedEnumerable = namespacedCommands.OrderBy((item) => item.Key);
            foreach (var namespacedCommand in orderedEnumerable)
            {
                namespacedCommand.Value.Sort((left, right)
                    => string.CompareOrdinal(left.nameOrAlias, right.nameOrAlias));
                yield return (namespacedCommand.Key, namespacedCommand.Value.ToArray());
            }
        }

        /// <summary>
        /// Inspect the command.
        /// </summary>
        private void InspectApplication()
        {
            commands.Clear();
            aliases.Clear();

            var all = application.All(!string.IsNullOrEmpty(@namespace)
                ? application.FindNamespace(@namespace) : null);

            var commandCollect = new List<string>();
            foreach (var namespaceAndCommands in SortCommands(all))
            {
                foreach (var (nameOrALias, command) in namespaceAndCommands.commands)
                {
                    if (string.IsNullOrEmpty(command.Name) || (showHidden && command.IsHidden))
                    {
                        continue;
                    }

                    if (command.Name == nameOrALias)
                    {
                        commands[nameOrALias] = command;
                    }
                    else
                    {
                        aliases[nameOrALias] = command;
                    }

                    commandCollect.Add(nameOrALias);
                }

                namespaces.Add((namespaceAndCommands.@namespace, commandCollect.ToArray()));
                commandCollect.Clear();
            }
        }
    }
}
