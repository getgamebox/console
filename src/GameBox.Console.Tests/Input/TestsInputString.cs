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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace GameBox.Console.Tests.Input
{
    [TestClass]
    public class TestsInputString
    {
        [TestMethod]
        [DataRow("-f foo", "-f foo")]
        [DataRow("-f --bar=foo \"a b c d\"", "-f --bar=foo \"a b c d\"")]
        public void TestToString(string expected, string input)
        {
            Assert.AreEqual(expected, new InputString(input).ToString());
        }

        [TestMethod]
        [DataRow(new string[0], "", "Method parses an empty string")]
        [DataRow(new[] { "foo" }, "foo", "Method parses arguments")]
        [DataRow(new[] { "foo", "bar" }, "  foo  bar  ", "Method ignores whitespaces between arguments")]
        [DataRow(new[] { "quoted" }, "\"quoted\"", "Method parses quoted arguments")]
        [DataRow(new[] { "a\rb\nc\td" }, "\"a\rb\nc\td\"", "Method parses whitespace chars in strings")]
        [DataRow(new[] { "a", "b", "c", "d" }, "\"a\"\r\"b\"\n\"c\"\t\"d\"", "Method parses whitespace chars between args as spaces")]
        [DataRow(new[] { "-a" }, "-a", "Method parses short options")]
        [DataRow(new[] { "-azc" }, "-azc", "Method parses aggregated short options")]
        [DataRow(new[] { "-awithavalue" }, "-awithavalue", "Method parses short options with a value")]
        [DataRow(new[] { "-afoo bar" }, "-a\"foo bar\"", "Method parses short options with a value")]
        [DataRow(new[] { "-afoo bar\"foo bar" }, "-a\"foo bar\"\"foo bar\"", "Method parses short options with a value")]
        [DataRow(new[] { "--long-option" }, "--long-option", "Method parses long options")]
        [DataRow(new[] { "--long-option=foo" }, "--long-option=foo", "Method parses long options with a value")]
        [DataRow(new[] { "--long-option=foo bar" }, "--long-option=\"foo bar\"", "Method parses long options with a value")]
        [DataRow(new[] { "--long-option=foo\"bar" }, "--long-option=\"foo\"\"bar\"", "Method parses long options with a value")]
        [DataRow(new[] { "foo", "-a", "-ffoo", "--long", "bar" }, "foo -a -ffoo --long bar", "Method parses when several arguments and options")]
        public void TestTokenize(string[] expected, string input, string message)
        {
            var instance = new InputString(input);
            var property = typeof(InputArgs).GetField("args", BindingFlags.NonPublic | BindingFlags.Instance);
            var actual = (string[])property.GetValue(instance);
            CollectionAssert.AreEqual(expected, actual, message);
        }
    }
}
