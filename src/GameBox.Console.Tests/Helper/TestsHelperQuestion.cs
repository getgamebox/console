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

using GameBox.Console.Exception;
using GameBox.Console.Helper;
using GameBox.Console.Question;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameBox.Console.Tests.Helper
{
    [TestClass]
    public class TestsHelperQuestion : AbstractTestsHelper
    {
        public virtual HelperQuestion GetHelper()
        {
            return new HelperQuestion();
        }

        [TestMethod]
        public void TestAskChoice()
        {
            var helper = GetHelper();
            var heroes = new string[] { "candy", "cat", "miaomiao", "menghan" };

            var question = new QuestionChoice("What is your favorite superhero?", heroes, 1);
            question.SetMaxAttempts(1);
            Assert.AreEqual("miaomiao", helper.Ask(CreateInput("2\n"), CreateOutputStringBuilder(), question).ToString());

            question = new QuestionChoice("What is your favorite superhero?", heroes);
            question.SetMaxAttempts(1);
            Assert.AreEqual("miaomiao", helper.Ask(CreateInput("  2  \n"), CreateOutputStringBuilder(), question).ToString());
            Assert.AreEqual("miaomiao", helper.Ask(CreateInput("miaomiao\n"), CreateOutputStringBuilder(), question).ToString());

            question = new QuestionChoice("What is your favorite superhero?", heroes);
            question.SetMaxAttempts(2);
            question.SetErrorMessage("Input \"{0}\" is not a superhero!");
            var output = CreateOutputStringBuilder();
            Assert.AreEqual("miaomiao", helper.Ask(CreateInput("hello\n2\n"), output, question).ToString());
            Assert.AreEqual(true, output.Fetch().Contains("Input \"hello\" is not a superhero!"));

            question = new QuestionChoice("What is your favorite superhero?", heroes);
            question.SetMaxAttempts(1);
            question.SetErrorMessage("Input \"{0}\" is not a superhero!");

            try
            {
                helper.Ask(CreateInput("hello\n"), CreateOutputStringBuilder(), question);
                Assert.Fail("Need to trigger an exception when the maximum number of attempts is exceeded.");
            }
            catch (InvalidArgumentException e)
            {
                Assert.AreEqual("Input \"hello\" is not a superhero!", e.Message);
            }
        }

        [TestMethod]
        public void TestAskChoiceMult()
        {
            var helper = GetHelper();
            var heroes = new string[] { "candy", "cat", "miaomiao", "menghan" };

            var question = new QuestionChoice("What is your favorite superhero?", heroes, 1);
            question.SetMaxAttempts(1);
            question.SetMultiselect(true);

            var ret = (string[])helper.Ask(CreateInput("2,2\n"), CreateOutputStringBuilder(), question);
            Assert.AreEqual(1, ret.Length);
            Assert.AreEqual("miaomiao", string.Join(",", ret));

            ret = helper.Ask(CreateInput("2,cat\n"), CreateOutputStringBuilder(), question);
            Assert.AreEqual(2, ret.Length);
            Assert.AreEqual("miaomiao,cat", string.Join(",", ret));

            question = new QuestionChoice("What is your favorite superhero?", heroes, "1,0");
            question.SetMaxAttempts(1);
            question.SetMultiselect(true);
            ret = helper.Ask(CreateInput("\n"), CreateOutputStringBuilder(), question);
            Assert.AreEqual(2, ret.Length);
            Assert.AreEqual("cat,candy", string.Join(",", ret));

            question = new QuestionChoice("What is your favorite superhero?", heroes, "  1  ,  0  ");
            question.SetMaxAttempts(1);
            question.SetMultiselect(true);
            ret = helper.Ask(CreateInput("\n"), CreateOutputStringBuilder(), question);
            Assert.AreEqual(2, ret.Length);
            Assert.AreEqual("cat,candy", string.Join(",", ret));
        }

        [TestMethod]
        public void TestAskChoiceNonInteractive()
        {
            var helper = GetHelper();
            var heroes = new string[] { "candy", "cat", "miaomiao", "menghan" };

            var question = new QuestionChoice("What is your favorite superhero?", heroes, 0);
            question.SetMaxAttempts(1);
            var ret = helper.Ask(CreateInput(string.Empty, false), CreateOutputStringBuilder(), question);
            Assert.AreEqual("candy", ret.ToString());

            question = new QuestionChoice("What is your favorite superhero?", heroes, 0);
            question.SetMaxAttempts(1);
            question.SetMultiselect(true);
            ret = helper.Ask(CreateInput(string.Empty, false), CreateOutputStringBuilder(), question);
            Assert.AreEqual("candy", string.Join(",", ret));

            question = new QuestionChoice("What is your favorite superhero?", heroes, null);
            question.SetMaxAttempts(1);
            ret = helper.Ask(CreateInput(string.Empty, false), CreateOutputStringBuilder(), question);
            Assert.AreEqual(null, ret);

            // Content with spaces
            heroes = new[] { "candy cat", "miao miao", "meng han yu" };
            question = new QuestionChoice("What is your favorite superhero?", heroes, 0);
            question.SetMaxAttempts(1);
            ret = helper.Ask(CreateInput(string.Empty, false), CreateOutputStringBuilder(), question);
            Assert.AreEqual("candy cat", ret.ToString());

            heroes = new[] { "candy cat", "miao miao", "meng han yu" };
            question = new QuestionChoice("What is your favorite superhero?", heroes, "miao miao");
            question.SetMaxAttempts(1);
            ret = helper.Ask(CreateInput(string.Empty, false), CreateOutputStringBuilder(), question);
            Assert.AreEqual("miao miao", ret.ToString());
        }

        [TestMethod]
        public void TestAsk()
        {
            var helper = GetHelper();

            var question = new Question.Question("What's your name?", "miaomiao");
            Assert.AreEqual("miaomiao", helper.Ask(CreateInput("\n"), CreateOutputStringBuilder(), question).ToString());

            question = new Question.Question("What's your name?", "miaomiao");
            var output = CreateOutputStringBuilder();
            Assert.AreEqual("menghanyu", helper.Ask(CreateInput("menghanyu\n"), output, question).ToString());

            Assert.AreEqual(true, output.Fetch().Contains("What's your name?"));
        }

        [TestMethod]
        [DataRow("", true, true)]
        [DataRow("", false, false)]
        [DataRow("y", true, true)]
        [DataRow("yes", true, true)]
        [DataRow("no", false, true)]
        [DataRow("n", false, true)]
        public void TestAskConfirmation(string ask, bool expected, bool defaultValue)
        {
            var helper = GetHelper();
            var question = new QuestionConfirmation("Do you like mother?", defaultValue);
            Assert.AreEqual(expected, (bool)helper.Ask(CreateInput(ask + "\n"), CreateOutputStringBuilder(), question));
        }

        [TestMethod]
        public void TestAskConfirmationWithCustomTrueAnswer()
        {
            var helper = GetHelper();
            var question = new QuestionConfirmation("Do you like mother?", false, "^(j|y)");
            Assert.AreEqual(true, (bool)helper.Ask(CreateInput("jabc\n"), CreateOutputStringBuilder(), question));
            Assert.AreEqual(true, (bool)helper.Ask(CreateInput("yabc\n"), CreateOutputStringBuilder(), question));
            Assert.AreEqual(false, (bool)helper.Ask(CreateInput("abc\n"), CreateOutputStringBuilder(), question));
        }

        [TestMethod]
        public void TestAskAndUseValidate()
        {
            var helper = GetHelper();

            var question = new Question.Question("What's color do you like?", "green");
            question.SetMaxAttempts(2);
            question.SetValidator((input) =>
            {
                if (input == "red")
                {
                    throw new System.Exception("Don't choose red");
                }

                return input;
            });

            var ret = helper.Ask(CreateInput("red\npink\n"), CreateOutputStringBuilder(), question);
            Assert.AreEqual("pink", ret.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ConsoleLogicException))]
        public void TestEmptyChoices()
        {
            new QuestionChoice("hello?", System.Array.Empty<string>());
        }
    }
}
