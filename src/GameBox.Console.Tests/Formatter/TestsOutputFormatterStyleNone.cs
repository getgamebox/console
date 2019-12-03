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

using GameBox.Console.Formatter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBox.Console.Tests.Formatter
{
    [TestClass]
    public class TestsOutputFormatterStyleNone
    {
        [TestMethod]
        public void TestOutputFormatterStyleNone()
        {
            var style = new OutputFormatterStyleNone();
            style.SetBackground(null);
            style.SetForeground(null);
            style.SetOption(null);
            style.UnsetOption(null);
        }

        [TestMethod]
        public void TestFormat()
        {
            var style = new OutputFormatterStyleNone();
            Assert.AreEqual("hello world", style.Format("hello world"));
        }
    }
}
