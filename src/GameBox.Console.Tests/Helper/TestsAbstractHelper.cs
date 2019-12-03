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
    public class TestsAbstractHelper
    {
        [TestMethod]
        public void TestRemoveDecorationWithNullFormatter()
        {
            Assert.AreEqual("hello", AbstractHelper.RemoveDecoration(null, "hello"));
        }
    }
}
