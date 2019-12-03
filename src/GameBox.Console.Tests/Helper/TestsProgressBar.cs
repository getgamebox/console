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

using GameBox.Console.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBox.Console.Tests.Helper
{
    [TestClass]
    public class TestsProgressBar : AbstractTestsHelper
    {
        [TestInitialize]
        public void Setup()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");
        }

        [TestMethod]
        public void TestMultBegin()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output);

            bar.Begin();
            bar.Advance();
            bar.Begin();

            Assert.AreEqual(
@"    0 [>---------------------------]
    1 [->--------------------------]
    0 [>---------------------------]", output.Fetch());
        }

        [TestMethod]
        public void TestAdvance()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output);

            bar.Begin();
            bar.Advance();

            Assert.AreEqual(
@"    0 [>---------------------------]
    1 [->--------------------------]", output.Fetch());
        }

        [TestMethod]
        public void TestAdvanceWithStep()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output);

            bar.Begin();
            bar.Advance(5);

            Assert.AreEqual(
@"    0 [>---------------------------]
    5 [----->----------------------]", output.Fetch());
        }

        [TestMethod]
        public void TestAdvanceMultipleTimes()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output);

            bar.Begin();
            bar.Advance(5);
            bar.Advance(3);

            Assert.AreEqual(
@"    0 [>---------------------------]
    5 [----->----------------------]
    8 [-------->-------------------]", output.Fetch());
        }

        [TestMethod]
        public void TestAdvanceOverMax()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.SetProgress(9);
            bar.Advance();
            bar.Advance();

            Assert.AreEqual(
@"  9/10 [=========================>--] 90%
 10/10 [============================] 100%
 11/11 [============================] 100%", output.Fetch());
        }

        [TestMethod]
        public void TestRegress()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.Begin();
            bar.Advance();
            bar.Advance();
            bar.Advance(-1);

            Assert.AreEqual(
@"  0/10 [>---------------------------] 0%
  1/10 [==>-------------------------] 10%
  2/10 [=====>----------------------] 20%
  1/10 [==>-------------------------] 10%", output.Fetch());
        }

        [TestMethod]
        public void TestRegressWithStep()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.Begin();
            bar.Advance(5);
            bar.Advance(3);
            bar.Advance(-3);

            Assert.AreEqual(
@"  0/10 [>---------------------------] 0%
  5/10 [==============>-------------] 50%
  8/10 [======================>-----] 80%
  5/10 [==============>-------------] 50%", output.Fetch());
        }

        [TestMethod]
        public void TestRegressMinThanZero()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.Begin();
            bar.Advance(5);
            bar.Advance(3);
            bar.Advance(-30);

            Assert.AreEqual(
@"  0/10 [>---------------------------] 0%
  5/10 [==============>-------------] 50%
  8/10 [======================>-----] 80%
  0/10 [>---------------------------] 0%", output.Fetch());
        }

        [TestMethod]
        public void TestSetFormat()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.SetFormat("{bar}");

            bar.Begin();
            bar.Advance(5);
            bar.Advance(3);
            bar.Advance(-30);

            Assert.AreEqual(
@">---------------------------
==============>-------------
======================>-----
>---------------------------", output.Fetch());
        }

        [TestMethod]
        public void TestCustomizations()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.SetBarWidth(10);
            bar.SetBarChar("_");
            bar.SetEmptyBarChar(" ");
            bar.SetProgressChar("/");

            bar.SetFormat("{current}/{max} [{bar}] {percent}%");
            bar.Begin();
            bar.Advance(5);

            Assert.AreEqual(
@" 0/10 [/         ] 0%
 5/10 [_____/    ] 50%", output.Fetch());
        }

        [TestMethod]
        public void TestDisplayWithoutBegin()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.Display();
            Assert.AreEqual(
@"  0/10 [>---------------------------] 0%", output.Fetch());
        }

        [TestMethod]
        public void TestDisplayWithQuietVerbosity()
        {
            var output = CreateOutputStringBuilder();
            output.SetOptions(Output.OutputOptions.VerbosityQuiet);
            var bar = new ProgressBar(output, 10);

            bar.Display();
            Assert.AreEqual(string.Empty, output.Fetch());
        }

        [TestMethod]
        public void TestEndWidthoutBegin()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);
            bar.End();

            Assert.AreEqual(
@" 10/10 [============================] 100%", output.Fetch());
        }

        [TestMethod]
        public void TestPercent()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 50);
            bar.SetRedrawFrequency(1);
            bar.Begin();
            bar.Display();
            bar.Advance();
            bar.Advance();

            Assert.AreEqual(
@"  0/50 [>---------------------------] 0%
  0/50 [>---------------------------] 0%
  1/50 [>---------------------------] 2%
  2/50 [=>--------------------------] 4%", output.Fetch());
        }

        [TestMethod]
        public void TestMultProgressBar()
        {
            var output = CreateOutputStringBuilder();
            var bar1 = new ProgressBar(output, 50);
            var bar2 = new ProgressBar(output, 50);

            bar1.SetRedrawFrequency(1);
            bar2.SetRedrawFrequency(1);

            bar1.Begin();
            bar1.Advance();

            bar2.Begin();
            bar2.Advance(10);

            bar1.Advance();
            bar2.Advance(10);

            bar2.Advance(10);
            bar1.Advance();

            Assert.AreEqual(
@"  0/50 [>---------------------------] 0%
  1/50 [>---------------------------] 2%  0/50 [>---------------------------] 0%
 10/50 [=====>----------------------] 20%
  2/50 [=>--------------------------] 4%
 20/50 [===========>----------------] 40%
 30/50 [================>-----------] 60%
  3/50 [=>--------------------------] 6%", output.Fetch());
        }

        [TestMethod]
        public void TestBeginSetMaxStep()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);
            bar.Begin(50);

            Assert.AreEqual(
@"  0/50 [>---------------------------] 0%", output.Fetch());
        }

        [TestMethod]
        public void TestMessagePlaceholder()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);

            bar.SetMessage("this is message");
            bar.SetFormat("{bar} {message}");

            bar.Begin();
            Assert.AreEqual(
@">--------------------------- this is message", output.Fetch());
        }

        [TestMethod]
        public void TestPlaceholderFormatter()
        {
            var output = CreateOutputStringBuilder();
            var bar = new ProgressBar(output, 10);
            var count = 0;

            bar.SetPlaceholderFormatterDefinition("test", (_, __) =>
            {
                return (count++).ToString();
            });
            bar.SetFormat("{bar} {test}");

            bar.Begin();
            bar.Advance();
            bar.Advance();
            Assert.AreEqual(
@">--------------------------- 0
==>------------------------- 1
=====>---------------------- 2", output.Fetch());
        }

        [TestMethod]
        public void TestDisplayWithVerbosity1()
        {
            var output = CreateOutputStringBuilder();
            output.SetOptions(Output.OutputOptions.VerbosityVerbose);
            var bar = new ProgressBar(output, 10);

            bar.Display();
            Assert.AreEqual(@"  0/10 [>---------------------------] 0% < 1 sec", output.Fetch());
        }

        [TestMethod]
        public void TestDisplayWithVerbosity2()
        {
            var output = CreateOutputStringBuilder();
            output.SetOptions(Output.OutputOptions.VerbosityVeryVerbose);
            var bar = new ProgressBar(output, 10);

            bar.Display();
            Assert.AreEqual(@"  0/10 [>---------------------------] 0% < 1 sec/< 1 sec", output.Fetch());
        }

        [TestMethod]
        public void TestDisplayWithVerbosity3()
        {
            var output = CreateOutputStringBuilder();
            output.SetOptions(Output.OutputOptions.VerbosityDebug);
            var bar = new ProgressBar(output, 10);

            bar.Display();
            Assert.AreEqual(true, output.Fetch().Contains("MiB"));
        }
    }
}
