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

using GameBox.Console.Descriptor;
using GameBox.Console.Input;
using GameBox.Console.Output;
using GameBox.Console.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBox.Console.Tests.Descriptor
{
    public abstract class AbstractTestsDescriptor
    {
        public void AssertDescriptorCommand(Command.Command command, string excepted)
        {
            AssertDescription(excepted, command);
        }

        public void AssertDescriptorOption(InputOption option, string excepted)
        {
            AssertDescription(excepted, option);
        }

        public void AssertDescriptorArgument(InputArgument argument, string excepted)
        {
            AssertDescription(excepted, argument);
        }

        public void AssertDescriptorDefinition(InputDefinition definition, string excepted)
        {
            AssertDescription(excepted, definition);
        }

        public void AssertDescriptorApplication(Application application, string excepted)
        {
            AssertDescription(excepted, application);
        }

        public abstract IDescriptor GetDescriptor();

        private void AssertDescription(string excepted, object describedObject, params Mixture[] options)
        {
            var output = new OutputStringBuilder();
            GetDescriptor().Describe(output, describedObject, options);
            Assert.AreEqual(excepted.Trim(), output.Fetch().Trim());
        }
    }
}
