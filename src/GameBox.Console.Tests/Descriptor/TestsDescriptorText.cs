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

using GameBox.Console.Descriptor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace GameBox.Console.Tests.Descriptor
{
    [TestClass]
    public class TestsDescriptorText : AbstractDescriptorData
    {
        public override string LoadExceptedFromFile(string name)
        {
            return File.ReadAllText($"Fixtures/Output/{name}.txt");
        }

        public override IDescriptor GetDescriptor()
        {
            return new DescriptorText();
        }
    }
}
