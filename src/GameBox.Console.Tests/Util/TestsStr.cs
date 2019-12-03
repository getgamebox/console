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

using GameBox.Console.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBox.Console.Tests.Util
{
    [TestClass]
    public class TestsStr
    {
        [TestMethod]
        public void TestWordWrap()
        {
            var n = Environment.NewLine;
            Assert.AreEqual($"hello*world{n}menghanyu", Str.WordWrap("hello world menghanyu", 20)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"hello*world***{n}menghanyu", Str.WordWrap("hello world    menghanyu", 20)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"hello*world***{n}menghanyu***", Str.WordWrap("hello world    menghanyu   ", 20)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"hello*双宽度{n}字符world***{n}menghanyu***", Str.WordWrap("hello 双宽度字符world    menghanyu   ", 12)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"hello*双宽度{n}world字符***{n}menghanyu***", Str.WordWrap("hello 双宽度world字符    menghanyu   ", 12)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"hello*你好{n}world********{n}menghanyu***", Str.WordWrap("hello 你好world         menghanyu   ", 14)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"helloworld{n}helloworld", Str.WordWrap("helloworldhelloworld", 10)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"helloworld{n}helloworld", Str.WordWrap("helloworldhelloworld", 10)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"全部双宽度{n}字符测试", Str.WordWrap("全部双宽度字符测试", 10)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"双宽度字符{n}mixedenglish测{n}试", Str.WordWrap("双宽度字符mixedenglish测试", 15)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"双宽度字符{n}mixedenglish测{n}试", Str.WordWrap("双宽度字符mixedenglish测试", 15)
                .Replace(" ", "*", StringComparison.CurrentCulture));

            Assert.AreEqual($"双宽度字符{n}MixedEnglishCon{n}tentOverflowTes{n}t测试", Str.WordWrap("双宽度字符MixedEnglishContentOverflowTest测试", 15)
                .Replace(" ", "*", StringComparison.CurrentCulture));
        }
    }
}
