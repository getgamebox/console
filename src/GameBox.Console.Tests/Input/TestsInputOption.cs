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
using GameBox.Console.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBox.Console.Tests.Input
{
    [TestClass]
    public class TestsInputOption
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCreateWithEmptyName()
        {
            new InputOption(null);
        }

        [TestMethod]
        public void TestCreateWithOneName()
        {
            var inputOption = new InputOption("help");
            Assert.AreEqual("help", inputOption.Name);
            Mixture m = inputOption.GetDefault();
            Assert.AreEqual(null, m);
            var arrStr = inputOption.GetShortcut();
            Assert.AreEqual(0, arrStr.Length);
        }

        [TestMethod]
        public void TestEquals()
        {
            var inputOption = new InputOption("help");
            var inputOption2 = new InputOption("help");
            var inputOption3 = new InputOption("help3");
            Assert.AreEqual(inputOption, inputOption2);
            Assert.AreNotEqual(inputOption, inputOption3);
        }

        [TestMethod]
        public void TestShortCut()
        {
            var inputOption = new InputOption("help", "-h");
            var arrStr = inputOption.GetShortcut();
            Assert.AreEqual(1, arrStr.Length);
            Assert.AreEqual("h", arrStr[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestNoneModeWithDefaultValue()
        {
            Assert.IsNotNull(new InputOption("help", "-h", InputOptionModes.ValueNone,
                "Display this help message.", "DefaultValue"));
        }

        [TestMethod]
        public void TestOptionalModeWithDefaultValue()
        {
            Assert.IsNotNull(new InputOption("help", "-h", InputOptionModes.ValueOptional,
                "Display this help message.", "DefaultValue"));
        }

        [TestMethod]
        public void TestRequiredModeWithDefaultValue()
        {
            Assert.IsNotNull(new InputOption("help", "-h", InputOptionModes.ValueRequired,
                "Display this help message.", "DefaultValue"));
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestOptionalArrayModeWithBasicTypeDefaultValue()
        {
            new InputOption("help", "-h",
                InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                "Display this help message.", "DefaultValue");
        }

        [TestMethod]
        public void TestOptionalArrayModeWithArrayTypeDefaultValue()
        {
            var inputOption = new InputOption("help", "-h",
                InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                "Display this help message.", new[] { "one", "two" });
            Mixture mixture = inputOption.GetDefault();
            Assert.AreNotEqual(null, mixture);
            Assert.AreEqual(true, mixture.IsArray);
            string[] strs = mixture;
            Assert.AreEqual(2, strs.Length);
            Assert.AreEqual("one", strs[0]);
            Assert.AreEqual("two", strs[1]);
        }

        [TestMethod]
        public void TestRequiredArrayModeWithArrayTypeDefaultValue()
        {
            var inputOption = new InputOption("custom", "-c",
                InputOptionModes.ValueRequired | InputOptionModes.IsArray,
                "Description here", new[] { "test1", "test2", "test3" });
            Mixture mixture = inputOption.GetDefault();
            Assert.AreNotEqual(null, mixture);
            Assert.AreEqual(true, mixture.IsArray);
            string[] strs = mixture;
            Assert.AreEqual(3, strs.Length);
            Assert.AreEqual("test3", strs[2]);
        }

        [TestMethod]
        public void TestHelpInputOption()
        {
            var inputOption = new InputOption("help", "-h",
                InputOptionModes.ValueNone, "Display this help message.");
            Assert.AreEqual("help", inputOption.Name);
            var arrStr = inputOption.GetShortcut();
            Assert.AreEqual(1, arrStr.Length);
            Assert.AreEqual("h", arrStr[0]);
            Assert.AreNotEqual(1, inputOption.GetHashCode());
            Assert.AreEqual("Display this help message.", inputOption.Description);
        }

        [TestMethod]
        public void TestVerboseInputOption()
        {
            var inputOption = new InputOption("--verbose", "-v|vv|vvv",
                InputOptionModes.ValueNone,
                "Increase the verbosity of messages: 1 for normal output, 2 for more verbose output and 3 for debug.");
            Assert.AreEqual("verbose", inputOption.Name);
            var arrStr = inputOption.GetShortcut();
            Assert.AreEqual(3, arrStr.Length);
            Assert.AreEqual("v", arrStr[0]);
            Assert.AreEqual("vv", arrStr[1]);
            Assert.AreEqual("vvv", arrStr[2]);
            Assert.AreEqual("Increase the verbosity of messages: 1 for normal output, 2 for more verbose output and 3 for debug.", inputOption.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestIllegalName()
        {
            new InputOption("---help", "-h");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestIllegalShortcut()
        {
            new InputOption("help", "-h&3");
        }
    }
}
