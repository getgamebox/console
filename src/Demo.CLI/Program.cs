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

using GameBox.Console;
using System;

namespace Demo.CLI
{
    /// <summary>
    /// Entry program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Entry method.
        /// </summary>
        /// <param name="args">The command line args.</param>
#pragma warning disable CA1801
        public static void Main(string[] args)
#pragma warning restore CA1801
        {
            var application = new Application();
            Environment.Exit(application.Run());
        }
    }
}
