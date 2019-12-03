/*
 * This file is part of the GameBox package.
 *
 * (c) Sijia Liu <394754029@qq.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getgamebox/console
 */

using GameBox.Console.Exception;
using GameBox.Console.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameBox.Console.Tests.Input
{
    [TestClass]
    public class TestsInputDefinitioncs
    {
        [TestMethod]
        public void TestCreateNull()
        {
            var inputDefinition = new InputDefinition();
            var inputArguments = inputDefinition.GetArguments();
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(0, inputArguments.Length);
            Assert.AreEqual(0, inputOptions.Length);
        }

        [TestMethod]
        public void TestCreateWithArgumentAndOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("command", InputArgumentModes.Required,
                    "The command to execute."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                    "Do not output any message."));
            var inputArguments = inputDefinition.GetArguments();
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(1, inputArguments.Length);
            Assert.AreEqual(2, inputOptions.Length);
        }

        [TestMethod]
        public void TestAddArgumentsAndOptions()
        {
            var inputDefinition = new InputDefinition();

            inputDefinition.AddArguments(new InputArgument("command", InputArgumentModes.Required,
                "The command to execute."));
            inputDefinition.AddOptions(new InputOption("help", "-h", InputOptionModes.ValueNone,
                "Display this help message."));
            inputDefinition.AddOptions(new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                "Do not output any message."));

            var inputArguments = inputDefinition.GetArguments();
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(1, inputArguments.Length);
            Assert.AreEqual(2, inputOptions.Length);
            var inputArgument = inputDefinition.GetArgument(0);
            Assert.AreNotEqual(null, inputArgument);
            inputArgument = inputDefinition.GetArgument(1);
            Assert.AreEqual(null, inputArgument);
            inputArgument = inputDefinition.GetArgument("command");
            Assert.AreNotEqual(null, inputArgument);
            Assert.AreEqual(true, inputDefinition.HasArgument("command"));
            Assert.AreEqual(false, inputDefinition.HasArgument(1));

            var inputOption = inputDefinition.GetOption("help");
            Assert.AreNotEqual(null, inputOption);
            inputOption = inputDefinition.GetOption("save");
            Assert.AreEqual(null, inputOption);
            Assert.AreEqual(true, inputDefinition.HasOption("help"));
            inputOption = inputDefinition.GetOptionForShortcut("h");
            Assert.AreNotEqual(null, inputOption);
            inputOption = inputDefinition.GetOptionForShortcut('q');
            Assert.AreNotEqual(null, inputOption);
            Assert.AreEqual(true, inputDefinition.HasShortcut("h"));
            Assert.AreEqual(true, inputDefinition.HasShortcut('h'));
            Assert.AreEqual(1, inputDefinition.GetArgumentRequiredCount());
            Assert.AreNotEqual(null, inputDefinition.GetSynopsis(true));
            Assert.AreNotEqual(null, inputDefinition.GetSynopsis(false));
        }

        [TestMethod]
        public void TestCreateOnlyArguments()
        {
            var inputDefinition = new InputDefinition(new InputArgument(
                "command",
                InputArgumentModes.Optional | InputArgumentModes.IsArray,
                "The command to execute.", new[] { "one", "two" }));

            var inputArguments = inputDefinition.GetArguments();
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(1, inputArguments.Length);
            Assert.AreEqual(0, inputOptions.Length);

            var mixtures = inputDefinition.GetArgumentDefaults();
            Assert.AreEqual(1, mixtures.Length);
            Assert.AreEqual(true, mixtures[0].IsArray);
            mixtures = inputDefinition.GetOptionsDefaults();
            Assert.AreEqual(0, mixtures.Length);
        }

        [TestMethod]
        public void TestSetArgumentsAndAddArguments()
        {
            var inputDefinition = new InputDefinition(new InputArgument(
                "command",
                InputArgumentModes.Optional | InputArgumentModes.IsArray,
                "The command to execute.", new[] { "one", "two" }));

            inputDefinition.SetArguments(
                new InputArgument("test", InputArgumentModes.Optional,
                    "The command to test."),
                new InputArgument("test1", InputArgumentModes.Optional,
                    "The command to test1."));

            var inputArguments = inputDefinition.GetArguments();
            Assert.AreEqual(2, inputArguments.Length);

            inputDefinition.AddArguments(
                new InputArgument("test2", InputArgumentModes.IsArray,
                    "The command to test2."));
            inputArguments = inputDefinition.GetArguments();
            Assert.AreEqual(3, inputArguments.Length);

            inputArguments = inputDefinition.GetArguments();
            Assert.AreEqual(3, inputArguments.Length);
            inputDefinition.AddArguments(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestSetSameArguments()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("test2", InputArgumentModes.Optional,
                    "The command to test2."));
            inputDefinition.AddArguments(
                new InputArgument("test2", InputArgumentModes.Optional,
                    "The command to test2."));
        }

        [TestMethod]
        public void TestCreateOnlyOptions()
        {
            var inputDefinition = new InputDefinition(
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                    "Do not output any message."));

            var inputArguments = inputDefinition.GetArguments();
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(0, inputArguments.Length);
            Assert.AreEqual(2, inputOptions.Length);

            var mixtures = inputDefinition.GetArgumentDefaults();
            Assert.AreEqual(0, mixtures.Length);
            mixtures = inputDefinition.GetOptionsDefaults();
            Assert.AreEqual(2, mixtures.Length);
            Assert.AreEqual(null, mixtures[0]);
        }

        [TestMethod]
        public void TestSetDefinition()
        {
            var inputDefinition = new InputDefinition(
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                    "Do not output any message."));

            inputDefinition.SetDefinition(
                new InputOption("test", "-a", InputOptionModes.ValueNone,
                    "Display this test message."));
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(1, inputOptions.Length);
        }

        [TestMethod]
        public void TestAddOptions()
        {
            var inputDefinition = new InputDefinition(
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                    "Do not output any message."));

            inputDefinition.AddOptions(
                new InputOption("test", "-c", InputOptionModes.ValueNone,
                    "Description here."));
            var inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(3, inputOptions.Length);

            inputDefinition.AddOptions(
                new InputOption("help", "-d", InputOptionModes.ValueNone,
                    "Display this help message."));
            inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(4, inputOptions.Length);

            inputDefinition.AddOptions(null);
            inputOptions = inputDefinition.GetOptions();
            Assert.AreEqual(4, inputOptions.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestAddSameOptions()
        {
            var inputDefinition = new InputDefinition(
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                    "Do not output any message."));

            inputDefinition.AddOptions(
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."));
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void AddArgumentAfterArrayArgument()
        {
            var inputDefinition = new InputDefinition();
            inputDefinition.AddArguments(
                new InputArgument("test", InputArgumentModes.IsArray,
                    "The command to test.", new[] { "one", "two" }),
                new InputArgument("test1", InputArgumentModes.Optional,
                    "The command to test1."));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestGetNotExistShortcut()
        {
            var inputDefinition = new InputDefinition(
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone,
                    "Do not output any message."));
            inputDefinition.GetOptionForShortcut("test");
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleException))]
        public void TestAddIInputDefinition()
        {
            var test = new Mock<IInputDefinition>();
            new InputDefinition(test.Object);
        }
    }
}
