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
using System.IO;

namespace GameBox.Console.Tests.Input
{
    [TestClass]
    public class TestsInputArgs
    {
        [TestMethod]
        public void TestCreateNoArgumentsNoDefinition()
        {
            Assert.IsNotNull(new InputArgs(System.Array.Empty<string>()));
        }

        [TestMethod]
        public void TestCreateWithoutDefinition()
        {
            Assert.IsNotNull(new InputArgs(new[] { "list" }));
        }

        [TestMethod]
        public void TestCreateWithoutArguments()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("command", InputArgumentModes.Optional,
                    "The command to execute."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."));
            Assert.IsNotNull(new InputArgs(System.Array.Empty<string>(), inputDefinition));
        }

        [TestMethod]
        public void TestCreateWithArgumentsAndDefinition()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional,
                    "The command to install."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                "The way of global to execute.", "bbb"));
            Assert.IsNotNull(new InputArgs(new[] { "installpath", "--global" }, inputDefinition));
        }

        [TestMethod]
        public void TestGetOptionalModelArgument()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional,
                    "The command to install."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                    "The way of global to execute.", "bbb"));
            var inputArgs = new InputArgs(new[] { "installpath", "--global" }, inputDefinition);

            var mixture = inputArgs.GetArgument("install");
            Assert.AreEqual("install", mixture.Name);
            string str = mixture;
            Assert.AreEqual("installpath", str);
        }

        [TestMethod]
        public void TestGetOptionalArrayModelArgument()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new[] { "1", "2" }),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                    "The way of global to execute.", "bbb"));
            var inputArgs = new InputArgs(new[] { "installpath", "--global" }, inputDefinition);

            var mixture = inputArgs.GetArgument("install");
            Assert.AreEqual("install", mixture.Name);
            Assert.AreEqual(true, mixture.IsArray);
            string[] str = mixture;
            Assert.AreEqual(1, str.Length);
            Assert.AreEqual("installpath", str[0]);
        }

        [TestMethod]
        public void TestGetOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional,
                    "The command to install."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                    "The way of global to execute.", "bbb"));
            var inputArgs = new InputArgs(new[] { "installpath", "--global=bbb" }, inputDefinition);

            var mixture = inputArgs.GetOption("global");
            Assert.AreNotEqual(null, mixture);
            string str = mixture;
            Assert.AreEqual("bbb", str);

            mixture = inputArgs.GetOption("help");
            Assert.AreEqual(null, mixture);
        }

        [TestMethod]
        public void TestRawOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional,
                    "The command to install."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                    "The way of global to execute.", "bbb"));
            var inputArgs = new InputArgs(new[] { "installpath", "--global=bbb" }, inputDefinition);

            var result = inputArgs.HasRawOption("install");
            Assert.AreEqual(true, result);
            result = inputArgs.HasRawOption("test");
            Assert.AreEqual(false, result);

            var mixture = inputArgs.GetRawOption("install");
            string str = mixture;
            Assert.AreEqual("path", str);
            mixture = inputArgs.GetRawOption("--global");
            Assert.AreNotEqual(null, mixture);
            str = mixture;
            Assert.AreEqual("bbb", str);
        }

        [TestMethod]
        public void TestShortOptionExist()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("test", "-i", InputOptionModes.ValueOptional,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { "install", "-ih" }, inputDefinition);
            Assert.AreNotEqual(null, inputArgs);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestShortOptionNotExist()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("test", "-i", InputOptionModes.ValueOptional,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            new InputArgs(new[] { "install", "-a" }, inputDefinition);
        }

        [TestMethod]
        public void TestShortOptionSet()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("test", "-i", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { "install", "-hi" }, inputDefinition);
            var mixture = inputArgs.GetOption("help");
            string str = mixture;
            Assert.AreEqual("True", str);
            mixture = inputArgs.GetOption("test");
            str = mixture;
            Assert.AreEqual(true, mixture.IsString);
            Assert.AreEqual("True", str);
            str = inputArgs.ToString();
            Assert.AreNotEqual(string.Empty, str);
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestShortOptionSetNotExist()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            new InputArgs(new[] { "install", "-hi" }, inputDefinition);
        }

        [TestMethod]
        public void TestMoreShortOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("test", "-t", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { "install", "-h", "-t" }, inputDefinition);
            Assert.AreNotEqual(null, inputArgs);
        }

        [TestMethod]
        public void TestInteractive()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("--help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--test", null, InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                    "The way of global to execute."));
            var inputArgs = new InputArgs(new[] { "install", "--global=123", "t" }, inputDefinition);

            Assert.AreEqual(true, inputArgs.IsInteractive);
            inputArgs.SetInteractive(false);
            Assert.AreEqual(false, inputArgs.IsInteractive);
        }

        [TestMethod]
        public void TestStream()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("--help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--test", null, InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional,
                    "The way of global to execute."));
            var inputArgs = new InputArgs(new[] { "install", "--global=123", "t" }, inputDefinition);

            using (Stream stream = Stream.Null)
            {
                inputArgs.SetInputStream(stream);
                Stream stream2 = inputArgs.GetInputStream();
                Assert.AreEqual(stream, stream2);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ParseException))]
        public void TestLongOptionWithNoValueNoneModes()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("--help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--test", null, InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueNone,
                    "The way of global to execute."));
            new InputArgs(new[] { "install", "--global=123", "-t" }, inputDefinition);
        }

        [TestMethod]
        public void TestLongOptionWithValueEmpty()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("--help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--test", null, InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueRequired,
                    "The way of global to execute.", "123"));
            var inputArgs = new InputArgs(new[] { "install", "--global=" }, inputDefinition);
            var mixture = inputArgs.GetOption("global");
            string str = mixture;
            Assert.AreEqual(string.Empty, str);
        }

        [TestMethod]
        public void TestParseEmptyToken()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("test", "-i", InputOptionModes.ValueOptional,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            Assert.AreNotEqual(null, inputArgs);
            inputArgs = new InputArgs(new[] { "--" }, inputDefinition);
            Assert.AreNotEqual(null, inputArgs);
        }

        [TestMethod]
        public void TestSetExistsOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("test", "-i", InputOptionModes.ValueOptional,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            inputArgs.SetOption("test", "Just for test.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestSetNoExistsOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            inputArgs.SetOption("test", "Just for test.");
        }

        [TestMethod]
        public void TestGetExistsOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("test", "-i", InputOptionModes.ValueOptional,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            var mixture = inputArgs.GetOption("test");
            Assert.AreEqual(null, mixture);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestGetNotExistsOption()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            inputArgs.GetOption("test");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestSetNotExistsArgument()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            inputArgs.SetArgument("test", "Just for test.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestGetNotExistsArgument()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional | InputArgumentModes.IsArray,
                    "The command to install.", new Mixture(new[] { "1", "2" })),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { string.Empty }, inputDefinition);
            inputArgs.GetArgument("test");
        }

        [TestMethod]
        public void TestCreateArrayOptionModel()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional,
                    "The command to install."),
                new InputOption("help", "-h", InputOptionModes.ValueNone,
                    "Display this help message."),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { "installpath", "--global=bbb" }, inputDefinition);

            var mixture = inputArgs.GetOption("global");
            Assert.AreEqual(true, mixture.IsArray);
            string[] str = mixture;
            Assert.AreEqual(1, str.Length);
            Assert.AreEqual("bbb", str[0]);
        }

        [TestMethod]
        public void TestCreateRequiredModelWithDefaultValue()
        {
            var inputDefinition = new InputDefinition(
                new InputArgument("install", InputArgumentModes.Optional,
                    "The command to install."),
                new InputOption("help", "-h", InputOptionModes.ValueRequired,
                    "Display this help message.", "123"),
                new InputOption("--global", "-g", InputOptionModes.ValueOptional | InputOptionModes.IsArray,
                    "The way of global to execute.", new[] { "aaa", "bbb" }));
            var inputArgs = new InputArgs(new[] { "installpath", "-h123" }, inputDefinition);
            Mixture mixture = inputArgs.GetOption("help");
            Assert.AreEqual(true, mixture.IsString);
            string str = mixture;
            Assert.AreEqual("123", str);
        }
    }
}
