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

using GameBox.Console.Style;
using GameBox.Console.Tester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBox.Console.Tests.Style
{
    [TestClass]
    public class TestsStyleDefault
    {
        [TestInitialize]
        public void Setup()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");
        }

        [TestMethod]
        public void TestCaution()
        {
            var tester = RunTester((style) =>
            {
                style.Caution("hello world");
            });

            Assert.AreEqual(
@"
 ! [CAUTION] hello world                                                                                               

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestTitleWarning()
        {
            var tester = RunTester((style) =>
            {
                style.Title("Title");
                style.Warning("Warning...");
                style.Title("Title");
            });

            Assert.AreEqual(
@"
Title
=====

 [WARNING] Warning...                                                                                                  

Title
=====

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestSuccessWithArray()
        {
            var tester = RunTester((style) =>
            {
                style.Success(new[] { "Success1", "Success2" });
            });

            Assert.AreEqual(
@"
 [OK] Success1                                                                                                         
                                                                                                                       
      Success2                                                                                                         

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestErrorWithArray()
        {
            var tester = RunTester((style) =>
            {
                style.Error(new[] { "Error1", "Error2" });
            });

            Assert.AreEqual(
@"
 [ERROR] Error1                                                                                                        
                                                                                                                       
         Error2                                                                                                        

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestWarningWithArray()
        {
            var tester = RunTester((style) =>
            {
                style.Warning(new[] { "Warning1", "Warning2" });
            });

            Assert.AreEqual(
@"
 [WARNING] Warning1                                                                                                    
                                                                                                                       
           Warning2                                                                                                    

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestNoteWithArray()
        {
            var tester = RunTester((style) =>
            {
                style.Note(new[] { "Note1", "Note2" });
            });

            Assert.AreEqual(
@"
 ! [NOTE] Note1                                                                                                        
 !                                                                                                                     
 !        Note2                                                                                                        

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestCautionWithArray()
        {
            var tester = RunTester((style) =>
            {
                style.Caution(new[] { "Caution1", "Caution2" });
            });

            Assert.AreEqual(
@"
 ! [CAUTION] Caution1                                                                                                  
 !                                                                                                                     
 !           Caution2                                                                                                  

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestAdmonish()
        {
            var tester = RunTester((style) =>
            {
                style.Warning("Warning...");
                style.Caution("Caution...");
                style.Error("Error...");
                style.Success("Success...");
                style.Note("Note...");
                style.Block("Custom block", "CUSTOM", "fg=white;bg=green", "● ", true);
            });

            Assert.AreEqual(
@"
 [WARNING] Warning...                                                                                                  

 ! [CAUTION] Caution...                                                                                                

 [ERROR] Error...                                                                                                      

 [OK] Success...                                                                                                       

 ! [NOTE] Note...                                                                                                      

● [CUSTOM] Custom block                                                                                                

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestSecondTitle()
        {
            var tester = RunTester((style) =>
            {
                style.Title("First title");
                style.Section("Second title");
            });

            Assert.AreEqual(
@"
First title
===========

Second title
------------

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestEnsureSingleBlank()
        {
            var tester = RunTester((style) =>
            {
                style.Write("Hello , my name is menghanyu");
                style.Title("First title");

                style.WriteLine("Hello , my name is menghanyu");
                style.Title("Second title");

                style.Write("Hello , my name is menghanyu");
                style.Write(string.Empty);
                style.Title("Third title");

                style.Write("Hello , my name is menghanyu");
                style.Write(new[] { string.Empty, string.Empty, string.Empty });
                style.Title("Fourth title");

                style.WriteLine("Hello , my name is menghanyu");
                style.WriteLine(new[] { string.Empty, string.Empty });
                style.Title("Fifth title");

                style.WriteLine("Hello , my name is menghanyu");
                style.NewLine(2);
                style.Title("Six title");
            });

            Assert.AreEqual(
@"Hello , my name is menghanyu

First title
===========

Hello , my name is menghanyu

Second title
============

Hello , my name is menghanyu

Third title
===========

Hello , my name is menghanyu

Fourth title
============

Hello , my name is menghanyu


Fifth title
===========

Hello , my name is menghanyu


Six title
=========

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestEnsureBlockLineEnding()
        {
            var tester = RunTester((style) =>
            {
                style.WriteLine("Hello , my name is menghanyu");
                style.Listing(new[]
                {
                    "hello",
                    "miaomiao",
                });

                style.Write("Hello , my name is menghanyu");
                style.Listing(new[]
                {
                    "hello",
                    "miaomiao",
                });

                style.Write("Hello , my name is menghanyu");
                style.Text(new[]
                {
                    "hello",
                    "miaomiao",
                });

                style.NewLine();

                style.Write("Hello , my name is menghanyu");
                style.Comment(new[]
                {
                    "hello",
                    "miaomiao",
                    "mengmeng",
                });
            });

            Assert.AreEqual(
@"Hello , my name is menghanyu
 * hello
 * miaomiao

Hello , my name is menghanyu
 * hello
 * miaomiao

Hello , my name is menghanyu
 hello
 miaomiao

Hello , my name is menghanyu

 // hello                                                                                                              
 //                                                                                                                    
 // miaomiao                                                                                                           
 //                                                                                                                    
 // mengmeng                                                                                                           

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestEnsureEndingBlockBlankLine()
        {
            var tester = RunTester((style) =>
            {
                style.Listing(new[]
                {
                    "hello",
                    "miaomiao",
                });
                style.Success("success - miaomiao");
            });

            Assert.AreEqual(
@"
 * hello
 * miaomiao

 [OK] success - miaomiao                                                                                               

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestEnsureNoInteractiveInput()
        {
            var tester = RunTester((style) =>
            {
                style.Title("title");
                style.Ask("hello?", "world!");
                style.AskChoice("choice question with default", new[]
                {
                    "choice1",
                    "choice2",
                }, "choice1");
                style.AskConfirm("confirmation with yes default", true);
                style.Text("wowowowo");
            });

            Assert.AreEqual(
@"
title
=====

 wowowowo
", tester.GetDisplay());
        }

        [TestMethod]
        public void TestUseBlockMethod()
        {
            var tester = RunTester((style) =>
            {
                style.Block(new[] { "Custom block", "Second custom block line" }, "CUSTOM", "fg=white;bg=green", "X ", true);
            });

            Assert.AreEqual(
@"
X [CUSTOM] Custom block                                                                                                
X                                                                                                                      
X          Second custom block line                                                                                    

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestUseLongBlock()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "32");
            var tester = RunTester((style) =>
            {
                style.Block("apple cat apple dog dudu hello wolrd you are grood put some time usually go to.", "CUSTOM", "fg=white;bg=green", "X ", true);
            });

            Assert.AreEqual(
@"
X [CUSTOM] apple cat apple dog 
X          dudu hello wolrd    
X          you are grood put   
X          some time usually   
X          go to.              

", tester.GetDisplay());
        }

        [TestMethod]
        public void TestLongComment()
        {
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "32");
            var tester = RunTester((style) =>
            {
                style.Comment("ONCE UPON A TIME a girl named Cinderella lived with her stepmother and two stepsisters.  Poor Cinderella had to work hard all day long so the others could rest. It was she who had to wake up each morning when it was still dark and cold to start the fire.  It was she who cooked the meals. It was she who kept the fire going. The poor girl could not stay clean, from all the ashes and cinders by the fire.");
            });

            Assert.AreEqual(
@"
 // ONCE UPON A TIME a girl    
 // named Cinderella lived     
 // with her stepmother and    
 // two stepsisters.  Poor     
 // Cinderella had to work     
 // hard all day long so the   
 // others could rest. It was  
 // she who had to wake up     
 // each morning when it was   
 // still dark and cold to     
 // start the fire.  It was    
 // she who cooked the meals.  
 // It was she who kept the    
 // fire going. The poor girl  
 // could not stay clean, from 
 // all the ashes and cinders  
 // by the fire.               

", tester.GetDisplay());
        }

        private TesterCommand RunTester(Action<StyleDefault> code)
        {
            var command = new Command.Command();
            command.SetCode((input, output) =>
            {
                code(new StyleDefault(input, output));
                return 0;
            });
            var tester = new TesterCommand(command);
            tester.Execute(string.Empty, AbstractTester.Interactive(false), AbstractTester.OptionDecorated(false));
            return tester;
        }
    }
}
