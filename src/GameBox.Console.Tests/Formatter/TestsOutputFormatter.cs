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
using GameBox.Console.Formatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBox.Console.Tests.Formatter
{
    [TestClass]
    public class TestsOutputFormatter
    {
        [TestMethod]
        public void TestDefaultStyle()
        {
            var formatter = new OutputFormatter();
            Assert.AreEqual(true, formatter.HasStyle("comment"));
            Assert.AreEqual(true, formatter.HasStyle("info"));
            Assert.AreEqual(true, formatter.HasStyle("question"));
            Assert.AreEqual(true, formatter.HasStyle("error"));
        }

        [TestMethod]
        public void TestGetStyle()
        {
            var formatter = new OutputFormatter();
            Assert.AreNotEqual(null, formatter.GetStyle("comment"));
            Assert.AreNotEqual(null, formatter.GetStyle("info"));
            Assert.AreNotEqual(null, formatter.GetStyle("question"));
            Assert.AreNotEqual(null, formatter.GetStyle("error"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestGetInvalidStyle()
        {
            var formatter = new OutputFormatter();
            formatter.GetStyle("invalid-style");
        }

        [TestMethod]
        public void TestSetStyle()
        {
            var formatter = new OutputFormatter();
            Assert.AreEqual(false, formatter.HasStyle("new-style"));
            formatter.SetStyle("new-style", new OutputFormatterStyleNone());
            Assert.AreEqual(true, formatter.HasStyle("new-style"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void TestSetInvalidStyle()
        {
            var formatter = new OutputFormatter();
            formatter.SetStyle(null, new OutputFormatterStyleNone());
        }

        [TestMethod]
        public void TestHasStyle()
        {
            var formatter = new OutputFormatter();
            Assert.AreEqual(false, formatter.HasStyle("new-style"));
            formatter.SetStyle("new-style", new OutputFormatterStyleNone());
            Assert.AreEqual(true, formatter.HasStyle("new-style"));
            Assert.AreEqual(true, formatter.HasStyle("NEW-STYLE"));
            Assert.AreEqual(true, formatter.HasStyle("New-Style"));
        }

        [TestMethod]
        public void TestFormat()
        {
            var formatter = new OutputFormatter()
            {
                Enable = true,
            };
            var actual = formatter.Format("<comment>hello</> <info>world</info>");
            Assert.AreEqual("\u001b[33mhello\u001b[39m \u001b[32mworld\u001b[39m", actual);

            actual = formatter.Format("<question>hello</> <error>world</>");
            Assert.AreEqual("\u001b[30;46mhello\u001b[39;49m \u001b[37;41mworld\u001b[39;49m", actual);
        }

        [TestMethod]
        public void TestFormatAndWrap()
        {
            var formatter = new OutputFormatter()
            {
                Enable = true,
            };

            var actual = formatter.FormatAndWrap("<comment>hello</> <info>world</info>", 8);
            System.Console.WriteLine(actual);
            Assert.AreEqual(
$"\u001b[33mhello\u001b[39m {Environment.NewLine}\u001b[32mworld\u001b[39m", actual);

            actual = formatter.FormatAndWrap("<comment>HelloMyNameIsMenhanyu</>", 8);
            Assert.AreEqual(
$"\u001b[33mHelloMyN\u001b[39m{Environment.NewLine}\u001b[33mameIsMen\u001b[39m{Environment.NewLine}\u001b[33mhanyu\u001b[39m", actual);
        }
    }
}
