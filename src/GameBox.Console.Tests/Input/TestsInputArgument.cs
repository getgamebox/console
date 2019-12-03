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
using System;

namespace GameBox.Console.Tests.Input
{
    [TestClass]
    public class TestsInputArgument
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyName()
        {
            new InputArgument(null);
        }

        [TestMethod]
        public void TestCreateWithOneName()
        {
            var inputArgument = new InputArgument("command");
            Assert.AreEqual("command", inputArgument.Name);
            Assert.AreEqual(null, inputArgument.GetDefault());
        }

        [TestMethod]
        public void TestRequiredModeWithoutDefaultValue()
        {
            Assert.IsNotNull(new InputArgument("command", InputArgumentModes.Required,
                "The command description."));
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestRequiredModeWithDefaultValue()
        {
            new InputArgument("command", InputArgumentModes.Required,
                "The command description.", "command");
        }

        [TestMethod]
        public void TestCommandArgument()
        {
            var inputArgument = new InputArgument("command_name", InputArgumentModes.Optional,
                "The command description.", "test");
            Assert.AreEqual("The command description.", inputArgument.Description);
            Assert.AreEqual("command_name", inputArgument.ToString());
            string str = inputArgument.GetDefault();
            Assert.AreEqual("test", str);
            string argument = inputArgument;
            Assert.AreEqual("command_name", argument);
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestOptionalArrayModeWithBasicTypeDefaultValue()
        {
            new InputArgument(
                "command_name",
                InputArgumentModes.Optional | InputArgumentModes.IsArray,
                "The command description.", "test");
        }

        [TestMethod]
        public void TestOptionalArrayModeWithArrayDefaultValue()
        {
            var inputArgument = new InputArgument(
                "command_name",
                InputArgumentModes.Optional | InputArgumentModes.IsArray,
                "The command description.", new[] { "param1", "param2" });
            var mixture = inputArgument.GetDefault();
            Assert.AreEqual(true, mixture.IsArray);
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestRequiredArrayModeWithArrayDefaultValue()
        {
            new InputArgument(
                "command_name",
                InputArgumentModes.Required | InputArgumentModes.IsArray,
                "The command description.", new[] { "param1", "param2" });
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestOptionalModeWithArrayDefaultValue()
        {
            new InputArgument("command_name", InputArgumentModes.Optional,
                "The command description.", new[] { "param1", "param2" });
        }
    }
}
