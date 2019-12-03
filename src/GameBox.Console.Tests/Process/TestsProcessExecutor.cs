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

using GameBox.Console.Process;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GameBox.Console.Tests.Process
{
    [TestClass]
    public class TestsProcessExecutor
    {
        [TestMethod]
        public void TestProcessExecutor()
        {
            var shell = new ProcessExecutor();
            Assert.AreEqual(0, shell.Execute("echo hello world", out string[] output, out _));
            Assert.AreEqual(2, output.Length);
            Assert.AreEqual(@"hello world", output[0]);
        }

        [TestMethod]
        public void TestProcessExecutorCwd()
        {
            var shell = new ProcessExecutor();
            shell.Execute("dir", out string[] output1, out _);
            shell.Execute("dir", out string[] output2, out _, Path.Combine(Environment.CurrentDirectory, ".."));

            CollectionAssert.AreNotEqual(output1, output2);
        }

        [TestMethod]
        public void TestErrorOutput()
        {
            var shell = new ProcessExecutor();
            shell.Execute("unknow", out _, out string[] stderr);

            StringAssert.Contains(string.Join(',', stderr), "unknow");
        }

        [TestMethod]
        [ExpectedException(typeof(TimeoutException))]
        public void TestProcessTimeout()
        {
            var command = "ping example.com";

            var shell = new ProcessExecutor()
            {
                Timeout = 1000,
            };

            shell.Execute(command, out _, out _);
        }

        [TestMethod]
        public void TestEscape()
        {
            // "fo\"o"
            Assert.AreEqual("\"fo\\\"o\"", ProcessExecutor.Escape("fo\"o"));

            // "fo\"o bar"
            Assert.AreEqual("\"fo\\\"o bar\"", ProcessExecutor.Escape("fo\"o bar"));

            // "fo\"o %bar% boo"
            Assert.AreEqual("\"fo\\\"o %bar% boo\"", ProcessExecutor.Escape("fo\"o %bar% boo"));

            // "foo \"^%"bar"^%\""
            Assert.AreEqual("\"foo \\\"^%\"bar\"^%\\\"\"", ProcessExecutor.Escape("foo \"%bar%\""));
            Assert.AreEqual("\"foo \\\"^%\"b\"^%\\\"\"", ProcessExecutor.Escape("foo \"%b%\""));
        }
    }
}
