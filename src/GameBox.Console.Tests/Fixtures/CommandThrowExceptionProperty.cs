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

using GameBox.Console.Input;
using GameBox.Console.Output;

namespace GameBox.Console.Tests.Fixtures
{
    public class CommandThrowExceptionProperty : Command.Command
    {
#pragma warning disable CA1065
        protected virtual bool ThrowException => throw new System.Exception("first property exception");
#pragma warning restore CA1065

        protected override void Configure()
        {
            SetName("foo/exception-property");
            SetDescription("The foo/exception-property command");
        }

        protected override int Execute(IInput input, IOutput output)
        {
            try
            {
                try
                {
                    var value = ThrowException;
                    throw new System.Exception($"First exception <p>{value}</p>");
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (System.Exception e)
                {
                    throw new System.Exception("Second exception <comment>comment</comment>", e);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (System.Exception e)
            {
                throw new System.Exception("Third exception <fg=blue;bg=red>comment</>", e);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
