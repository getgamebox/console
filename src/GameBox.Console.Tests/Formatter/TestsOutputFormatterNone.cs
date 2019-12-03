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
using System;

namespace GameBox.Console.Tests.Formatter
{
    [TestClass]
    public class TestsOutputFormatterNone
    {
        [TestMethod]
        public void TestDefaultEnable()
        {
            var formatter = new OutputFormatterNone();
            Assert.AreEqual(false, formatter.Enable);
        }

        [TestMethod]
        public void TestFormat()
        {
            var formatter = new OutputFormatterNone();
            Assert.AreEqual("hello <color>world</>", formatter.Format("hello <color>world</>"));
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestGetStyle()
        {
            var formatter = new OutputFormatterNone();
            formatter.GetStyle("style");
        }

        [TestMethod]
        public void TestHasStyle()
        {
            var formatter = new OutputFormatterNone();
            Assert.AreEqual(false, formatter.HasStyle("style"));
        }

        [TestMethod]
        public void TestSetStyle()
        {
            var formatter = new OutputFormatterNone();
            formatter.SetStyle("style", new OutputFormatterStyleNone());
            Assert.AreEqual(false, formatter.HasStyle("style"));
        }

        [TestMethod]
        public void TestClone()
        {
            var formatter = new OutputFormatterNone();
            Assert.AreNotSame(formatter, formatter.Clone());
        }
    }
}
