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
    public class TestsOutputFormatterStyle
    {
        [TestMethod]
        [DataRow("bold")]
        [DataRow("underscore")]
        [DataRow("blink")]
        [DataRow("reverse")]
        [DataRow("conceal")]
        public void TestSetOption(string option)
        {
            var style = new OutputFormatterStyle();
            style.SetOption(option);
        }
    }
}
