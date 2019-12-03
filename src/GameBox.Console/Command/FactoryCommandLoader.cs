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
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBox.Console.Command
{
    /// <summary>
    /// A simple command loader. It will instantiate commands lazily.
    /// </summary>
    public class FactoryCommandLoader : ICommandLoader
    {
        /// <summary>
        /// The command mapping.
        /// </summary>
        private readonly IDictionary<string, Func<Command>> factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryCommandLoader"/> class.
        /// </summary>
        /// <param name="factories">The command factories.</param>
        public FactoryCommandLoader(IDictionary<string, Func<Command>> factories)
        {
            this.factories = factories;
        }

        /// <inheritdoc />
        public Command Load(string name)
        {
            if (!factories.TryGetValue(name, out Func<Command> factory))
            {
                throw new CommandNotFoundException($"Command {name} does not exist.");
            }

            return factory();
        }

        /// <inheritdoc />
        public bool Has(string name)
        {
            return factories.ContainsKey(name);
        }

        /// <inheritdoc />
        public string[] GetNames()
        {
            return factories.Keys.ToArray();
        }
    }
}
