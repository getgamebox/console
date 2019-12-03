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

namespace GameBox.Console.Tests.Helper
{
    [TestClass]
    public class TestsHelperFormatter
    {
        [TestMethod]
        public void TestFormatSection()
        {
            var formatter = new HelperFormatter();
            var formatted = formatter.FormatSection("exe", "Some text to display");
            Assert.AreEqual("<info>[exe]</info> Some text to display", formatted);
        }

        [TestMethod]
        public void TestFormatText()
        {
            var formatter = new HelperFormatter();
            var formatted = formatter.FormatText("hello world", "red");
            Assert.AreEqual("<red>hello world</red>", formatted);
        }

        [TestMethod]
        public void TestFormatBlock()
        {
            var formatter = new HelperFormatter();
            var formatted = formatter.FormatBlock("Some text to display", "error");
            Assert.AreEqual("<error> Some text to display </error>", formatted);

            formatted = formatter.FormatBlock(new[] { "Some text to display", "foo bar" }, "error");
            Assert.AreEqual(
@"<error> Some text to display </error>
<error> foo bar              </error>", formatted);

            formatted = formatter.FormatBlock(new[] { "Some text to display" }, "error", true);
            Assert.AreEqual(
@"<error>                        </error>
<error>  Some text to display  </error>
<error>                        </error>", formatted);
        }

        [TestMethod]
        public void TestFormatWithDoubleWidthChar()
        {
            var formatter = new HelperFormatter();
            var formatted = formatter.FormatBlock("喵喵大人啦啦啦", "error", true);
            Assert.AreEqual(
@"<error>                  </error>
<error>  喵喵大人啦啦啦  </error>
<error>                  </error>", formatted);
        }

        [TestMethod]
        public void TestFormatBlockEscaping()
        {
            var formatter = new HelperFormatter();
            var formatted = formatter.FormatBlock("<info>some info</info>", "error", true);
            Assert.AreEqual(
@"<error>                            </error>
<error>  \<info>some info\</info>  </error>
<error>                            </error>", formatted);
        }
    }
}
