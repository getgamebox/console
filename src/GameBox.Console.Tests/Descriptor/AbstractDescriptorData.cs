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
using GameBox.Console.Tests.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBox.Console.Tests.Descriptor
{
    public abstract class AbstractDescriptorData : AbstractTestsDescriptor
    {
        public abstract string LoadExceptedFromFile(string name);

        [TestMethod]
        public void TestDescriptorCommand1()
        {
            AssertDescriptorCommand(new DescriptorCommand1(), LoadExceptedFromFile("TestDescriptorCommand1"));
        }

        [TestMethod]
        public void TestDescriptorCommand2()
        {
            AssertDescriptorCommand(new DescriptorCommand2(), LoadExceptedFromFile("TestDescriptorCommand2"));
        }

        [TestMethod]
        public void TestDescriptorApplication()
        {
            AssertDescriptorApplication(new DescriptorApplication1(), LoadExceptedFromFile("TestDescriptorApplication1"));
        }

        [TestMethod]
        public void TestDescriptorApplication2()
        {
            AssertDescriptorApplication(new DescriptorApplication2(), LoadExceptedFromFile("TestDescriptorApplication2"));
        }

        [TestMethod]
        public void TestDescriptorOption1()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.ValueNone),
                LoadExceptedFromFile("TestDescriptorOption1"));
        }

        [TestMethod]
        public void TestDescriptorOption2()
        {
            // todo: Added support for array default output in the future.
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.IsArray | InputOptionModes.ValueOptional),
                LoadExceptedFromFile("TestDescriptorOption2"));
        }

        [TestMethod]
        public void TestDescriptorOption3()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.ValueOptional),
                LoadExceptedFromFile("TestDescriptorOption3"));
        }

        [TestMethod]
        public void TestDescriptorOption4()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.ValueNone),
                LoadExceptedFromFile("TestDescriptorOption4"));
        }

        [TestMethod]
        public void TestDescriptorOption5()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.ValueRequired, "multiline\noption description"),
                LoadExceptedFromFile("TestDescriptorOption5"));
        }

        [TestMethod]
        public void TestDescriptorOption6()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o|O", InputOptionModes.ValueRequired, "option with multiple shortcuts"),
                LoadExceptedFromFile("TestDescriptorOption6"));
        }

        [TestMethod]
        public void TestDescriptorOption7()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.ValueRequired, "option with <comment>style</> shortcuts"),
                LoadExceptedFromFile("TestDescriptorOption7"));
        }

        [TestMethod]
        public void TestDescriptorOption8()
        {
            AssertDescriptorOption(
                new InputOption("option_name", "o", InputOptionModes.ValueRequired, "option with default value", "100"),
                LoadExceptedFromFile("TestDescriptorOption8"));
        }

        [TestMethod]
        public void TestDescriptorArgument1()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.Required),
                LoadExceptedFromFile("TestDescriptorArgument1"));
        }

        [TestMethod]
        public void TestDescriptorArgument2()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.IsArray, "argument description"),
                LoadExceptedFromFile("TestDescriptorArgument2"));
        }

        [TestMethod]
        public void TestDescriptorArgument3()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.Optional, "argument description", "default_value"),
                LoadExceptedFromFile("TestDescriptorArgument3"));
        }

        [TestMethod]
        public void TestDescriptorArgument4()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.Required, "multiline\nargument description"),
                LoadExceptedFromFile("TestDescriptorArgument4"));
        }

        [TestMethod]
        public void TestDescriptorArgument5()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.Optional, "argument description", "<comment>style</>"),
                LoadExceptedFromFile("TestDescriptorArgument5"));
        }

        [TestMethod]
        public void TestDescriptorArgument6()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.IsArray | InputArgumentModes.Required),
                LoadExceptedFromFile("TestDescriptorArgument6"));
        }

        [TestMethod]
        public void TestDescriptorArgument7()
        {
            AssertDescriptorArgument(
                new InputArgument("argument_name", InputArgumentModes.IsArray | InputArgumentModes.Optional),
                LoadExceptedFromFile("TestDescriptorArgument7"));
        }

        [TestMethod]
        public void TestDescriptorDefinition1()
        {
            AssertDescriptorDefinition(
                new InputDefinition(),
                LoadExceptedFromFile("TestDescriptorDefinition1"));
        }

        [TestMethod]
        public void TestDescriptorDefinition2()
        {
            AssertDescriptorDefinition(
                new InputDefinition(new InputArgument("argument_name", InputArgumentModes.Required)),
                LoadExceptedFromFile("TestDescriptorDefinition2"));
        }

        [TestMethod]
        public void TestDescriptorDefinition3()
        {
            AssertDescriptorDefinition(
                new InputDefinition(new InputOption("option_name", "o", InputOptionModes.ValueRequired)),
                LoadExceptedFromFile("TestDescriptorDefinition3"));
        }

        [TestMethod]
        public void TestDescriptorDefinition4()
        {
            AssertDescriptorDefinition(
                new InputDefinition(
                    new InputArgument("argument_name", InputArgumentModes.Required),
                    new InputOption("option_name", "o", InputOptionModes.ValueRequired)),
                LoadExceptedFromFile("TestDescriptorDefinition4"));
        }
    }
}
