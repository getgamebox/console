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

using GameBox.Console.Policy.Render;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GameBox.Console.Tests.Policy.Render
{
    [TestClass]
    public class TestsRenderException
    {
        [TestMethod]
        [DataRow(new[] { "foo", "bar", "baz" }, "foobarbaz", 3)]
        [DataRow(new[] { "f", "o", "o", "b", "a", "r", "b", "a", "z" }, "foobarbaz", 0)]
        [DataRow(new[] { "f", "o", "o", "b", "a", "r", "b", "a", "z" }, "foobarbaz", 1)]
        [DataRow(new[] { "foobarba", "z" }, "foobarbaz", 8)]
        [DataRow(new[] { "foobarbaz" }, "foobarbaz", 9)]
        [DataRow(new[] { "foobarbaz" }, "foobarbaz", 10)]
        public void TestSplitStringByWidth(string[] expected, string input, int width)
        {
            var actual = RenderException.SplitStringByWidth(input, width).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
