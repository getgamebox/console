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
using GameBox.Console.Question;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBox.Console.Tests.Helper
{
    [TestClass]
    public class TestsHelperQuestionDefault : TestsHelperQuestion
    {
        public override HelperQuestion GetHelper()
        {
            return new HelperQuestionDefault();
        }

        [TestMethod]
        public void TestAskChoiceDefault()
        {
            var helper = GetHelper();
            var heroes = new string[] { "candy", "cat", "miaomiao", "menghan" };
            var output = CreateOutputStringBuilder();

            var question = new QuestionChoice("What is your favorite superhero?", heroes, 1);
            question.SetMaxAttempts(1);
            Assert.AreEqual("miaomiao", helper.Ask(CreateInput("2\n"), output, question).ToString());
            Assert.AreEqual(true, output.Fetch().Contains("What is your favorite superhero? [cat]"));
        }

        [TestMethod]
        public void TestAskConfirmationDefault()
        {
            var helper = GetHelper();
            var output = CreateOutputStringBuilder();

            var question = new QuestionConfirmation("Do you like superhero?", true);
            Assert.AreEqual(true, (bool)helper.Ask(CreateInput("yes\n"), output, question));
            Assert.AreEqual(true, output.Fetch().Contains("Do you like superhero? (yes/no) [yes]"));
        }
    }
}
