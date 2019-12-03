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

using GameBox.Console.Command;
using GameBox.Console.EventDispatcher;
using GameBox.Console.Events;
using GameBox.Console.Exception;
using GameBox.Console.Input;
using GameBox.Console.Output;
using GameBox.Console.Tester;
using GameBox.Console.Tests.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dispatcher = GameBox.Console.EventDispatcher.EventDispatcher;

namespace GameBox.Console.Tests
{
    [TestClass]
    public class TestsApplication
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            // Guaranteed to have output space.
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");
        }

        [TestMethod]
        public void TestConstructor()
        {
            var application = new Application("foo", "bar");
            Assert.AreEqual("foo", application.Name,
                "Construct Application() takes the application name as its first argument.");
            Assert.AreEqual("bar", application.Version,
                "Construct Application() takes the version as its second argument.");

            var commands = application.All();
            Assert.AreEqual(CommandList.Name, commands[0].NameOrAlias,
                "Construct Application() registered the list commands by default.");
            Assert.AreEqual(CommandHelp.Name, commands[1].NameOrAlias,
                "Construct Application() registered the help commands by default.");
        }

        [TestMethod]
        public void TestSetGetName()
        {
            var application = new Application();
            application.SetName("foo");

            Assert.AreEqual("foo", application.Name,
                $"{nameof(application.SetName)}() sets the name of the application");
        }

        [TestMethod]
        public void TestSetGetVersion()
        {
            var application = new Application();
            application.SetVersion("bar");

            Assert.AreEqual("bar", application.Version,
                $"{nameof(application.SetVersion)}() sets the version of the application");
        }

        [TestMethod]
        public void TestGetLongVersion()
        {
            var application = new Application("foo", "bar");
            Assert.AreEqual("foo <info>bar</info>", application.GetLongVersion(),
                $"{nameof(application.GetLongVersion)}() returns the long version of the application");
        }

        [TestMethod]
        public void TestGetHelp()
        {
            var application = new Application();
            Assert.AreEqual("Console Tool", application.GetHelp(),
                $"{nameof(application.GetHelp)}) returns a help message");
        }

        [TestMethod]
        public void TestAll()
        {
            var application = new Application();
            var commands = application.All();

            Assert.AreEqual(typeof(CommandList), commands[0].Command.GetType(),
                $"{nameof(application.All)}() returns the registered commands");

            application.Add(new CommandFoo());
            commands = application.All(CommandFoo.Namespace);
            Assert.AreEqual(1, commands.Length,
                $"{nameof(application.All)}() takes a namespace as its first argument");
            Assert.AreEqual(typeof(CommandFoo), commands[0].Command.GetType(),
                $"{nameof(application.All)}() takes a namespace as its first argument");
        }

        [TestMethod]
        public void TestAllWithCommandLoader()
        {
            var application = new Application();

            application.Add(new CommandFoo());
            application.SetCommandLoader(new FactoryCommandLoader(new Dictionary<string, Func<Command.Command>>
            {
                { CommandFoo1.Name, () => new CommandFoo1() },
            }));

            var commands = application.All(CommandFoo.Namespace);

            Assert.AreEqual(2, commands.Length,
                $"{nameof(Application.All)}() takes a namespace as its first argument");
            Assert.AreEqual(CommandFoo.Name, commands[0].NameOrAlias,
                $"{nameof(application.All)}() returns the registered commands");
            Assert.AreEqual(CommandFoo1.Name, commands[1].NameOrAlias,
                $"{nameof(application.All)}() returns the registered commands");
        }

        [TestMethod]
        public void TestAdd()
        {
            var application = new Application();

            var foo = new CommandFoo();
            application.Add(foo);

            application.All().First((item) => item.Command == foo);

            Assert.AreSame(foo, application.All().First((item) => item.Command == foo).Command,
                $"{nameof(Application.Add)}() registers a command.");

            application = new Application();
            foo = new CommandFoo();
            application.Add(foo);
            var foo1 = new CommandFoo1();
            application.Add(foo1);

            Assert.AreSame(foo, application.All().First((item) => item.Command == foo).Command,
                $"{nameof(Application.Add)}() registers a command.");
            Assert.AreSame(foo1, application.All().First((item) => item.Command == foo1).Command,
                $"{nameof(Application.Add)}() registers a command.");
        }

        [TestMethod]
        public void TestHas()
        {
            var application = new Application();

            Assert.AreEqual(true, application.Has(CommandList.Name),
                $"{nameof(application.Has)}() returns true if a named command is registered.");
            Assert.AreEqual(true, application.Has(CommandHelp.Name),
                $"{nameof(application.Has)}() returns true if a named command is registered.");
        }

        [TestMethod]
        public void TestGet()
        {
            var application = new Application();
            var foo = new CommandFoo();
            application.Add(foo);

            Assert.AreSame(foo, application.Get(CommandFoo.Name),
                $"{nameof(Application.Get)}() returns a command by name");
            Assert.AreSame(foo, application.Get(CommandFoo.Alias),
                $"{nameof(Application.Get)}() returns a command by alias1");
        }

        [TestMethod]
        public void TestGetWantHelp()
        {
            var application = new Application();
            application.Add(new CommandFoo());

            // simulate --help
            var field = typeof(Application).GetField(
                "wantHelps",
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(application, true);

            var helpCommand = application.Get(CommandFoo.Name);
            Assert.AreEqual(typeof(CommandHelp), helpCommand.GetType(),
                $"{nameof(Application.Get)}() returns the help command if --help is provided as the input.");
        }

        [TestMethod]
        public void TestGetWithCommandLoader()
        {
            var application = new Application();
            application.Add(new CommandFoo());

            Command.Command foo1 = null;
            application.SetCommandLoader(new FactoryCommandLoader(new Dictionary<string, Func<Command.Command>>
            {
                { CommandFoo1.Name, () => foo1 = new CommandFoo1() },
            }));

            // lazy-loading need active first. so couldn't use aliases name.
            Assert.AreEqual(true, application.Has(CommandFoo1.Name),
                $"{nameof(Application.Get)}() returns a command by name");
            Assert.AreSame(foo1, application.Get(CommandFoo1.Alias),
                $"{nameof(Application.Get)}() returns a command by alias");
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(CommandNotFoundException),
            "The command foo-undefine does not exist.")]
        public void TestGetInvalidCommand()
        {
            var application = new Application();
            application.Get("foo-undefine");
        }

        [TestMethod]
        public void TestSilentHelp()
        {
            var application = new Application
            {
                CatchExceptions = false,
            };

            var tester = new TesterApplication(application);
            tester.Run("-h -q", AbstractTester.OptionDecorated(false));

            Assert.AreEqual(string.Empty, tester.GetDisplay(), "-q will silent the help message.");
        }

        [TestMethod]
        public void TestGetNamespace()
        {
            var application = new Application();

            application.Add(new CommandFoo());
            application.Add(new CommandFoo1());

            var namespaces = application.GetNamespaces();

            Assert.AreEqual(1, namespaces.Length,
                $"{nameof(Application.GetNamespaces)} returns an array of unique used namespaces");
            Assert.AreEqual(CommandFoo.Namespace, namespaces[0]);
        }

        [TestMethod]
        public void TestFindNamespace()
        {
            var application = new Application();

            application.Add(new CommandFoo());

            Assert.AreEqual(
                CommandFoo.Namespace,
                application.FindNamespace(CommandFoo.Namespace),
                $"{nameof(Application.FindNamespace)} returns the given namespace if it exists");
            Assert.AreEqual(
                CommandFoo.Namespace,
                application.FindNamespace(CommandFoo.Namespace.Substring(0, 1)),
                $"{nameof(Application.FindNamespace)} returns the given abbreviation if it exists");

            application.Add(new CommandBar());
            Assert.AreEqual(
                CommandBar.Namespace,
                application.FindNamespace(CommandBar.Namespace),
                $"{nameof(Application.FindNamespace)} returns the given namespace if it exists");
            Assert.AreEqual(
                CommandBar.Namespace,
                application.FindNamespace(CommandBar.Namespace.Substring(0, 1)),
                $"{nameof(Application.FindNamespace)} returns the given abbreviation if it exists");
        }

        [TestMethod]
        public void TestFindNamespaceWithSubNamespaces()
        {
            var application = new Application();
            application.Add(new CommandFooSubNamespaced1());
            application.Add(new CommandFooSubNamespaced2());

            Assert.AreEqual(CommandFooSubNamespaced1.Namespace, application.FindNamespace(CommandFooSubNamespaced1.Namespace),
                $"{nameof(Application.FindNamespace)} returns commands even if the commands are only contained in subnamespaces.");
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(NamespaceNotFoundException),
            "The namespace \"f\" is ambiguous.\nDid you mean one of these?\n    foo\n    fooa")]
        public void TestFindAmbiguousNamespace()
        {
            var application = new Application();
            application.Add(new CommandFoo());
            application.Add(new CommandFooA());

            application.FindNamespace(CommandFoo.Namespace.Substring(0, 1));
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(NamespaceNotFoundException),
            "There are no commands defined in the \"undefined\" namespace.")]
        public void TestFindInvalidNamespace()
        {
            var application = new Application();
            application.FindNamespace("undefined");
        }

        [TestMethod]
        public void TestFindNonAmbiguous()
        {
            var application = new Application();
            application.Add(new CommandAnonymous("test-titi"));
            application.Add(new CommandAnonymous("test-toto", "test"));

            Assert.AreEqual("test-toto", application.Find("test").Name);
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(CommandNotFoundException),
            "Command \"fooa\" is not defined")]
        public void TestFindUniqueNameButNamespaceName()
        {
            var application = new Application();
            application.Add(new CommandFoo());
            application.Add(new CommandFoo1());
            application.Add(new CommandFooA());

            application.Find(CommandFooA.Namespace);
        }

        [TestMethod]
        public void TestFind()
        {
            var application = new Application();
            application.Add(new CommandFoo());

            Assert.AreEqual(typeof(CommandFoo), application.Find(CommandFoo.Name).GetType(),
                $"{nameof(Application.Find)}() returns a command if its name exists");

            Assert.AreEqual(typeof(CommandHelp), application.Find("h").GetType(),
                $"{nameof(Application.Find)}() returns a command if its name exists");

            Assert.AreEqual(typeof(CommandFoo), application.Find("f/baz").GetType(),
                $"{nameof(Application.Find)}() returns a command if the abbreviation for the namespace exists");

            Assert.AreEqual(typeof(CommandFoo), application.Find("f/b").GetType(),
                $"{nameof(Application.Find)}() returns a command if the abbreviation for the namespace and the command name exist");

            application = new Application();
            var command = new CommandFoo();
            command.SetAlias("afooa");
            application.Add(command);
            Assert.AreEqual(typeof(CommandFoo), application.Find("a").GetType(),
                $"{nameof(Application.Find)}() returns a command if the abbreviation exists for an alias");
        }

        [TestMethod]
        public void TestFindCaseSensitiveFirst()
        {
            var application = new Application();

            application.Add(new CommandFooSameCaseUppercase());
            application.Add(new CommandFooSameCaseLowercase());

            Assert.AreEqual("foo/BAR", application.Find("f/B").Name,
                $"{nameof(Application.Find)}() returns a command if the abbreviation is the correct case");
            Assert.AreEqual("foo/BAR", application.Find("f/BAR").Name,
                $"{nameof(Application.Find)}() returns a command if the abbreviation is the correct case");
            Assert.AreEqual("foo/bar", application.Find("f/b").Name,
                $"{nameof(Application.Find)}() returns a command if the abbreviation is the correct case");
            Assert.AreEqual("foo/bar", application.Find("f/bar").Name,
                $"{nameof(Application.Find)}() returns a command if the abbreviation is the correct case");
        }

        [TestMethod]
        public void TestFindCaseInsensitiveAsFallback()
        {
            var application = new Application();
            application.Add(new CommandFooSameCaseLowercase());

            Assert.AreEqual("foo/bar", application.Find("f/b").Name,
                $"{nameof(Application.Find)}() returns a command if the abbreviation is the correct case");
            Assert.AreEqual("foo/bar", application.Find("f/B").Name,
                $"{nameof(Application.Find)}() will fallback to case insensitivity");
            Assert.AreEqual("foo/bar", application.Find("f/BaR").Name,
                $"{nameof(Application.Find)}() will fallback to case insensitivity");
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(CommandNotFoundException),
            "Command \"FoO/BaR\" is ambiguous.\nDid you mean one of these?\n    foo/BAR foo/BAR command\n    foo/bar foo/bar command")]
        public void TestFindCaseInsensitiveSuggestions()
        {
            var application = new Application();

            application.Add(new CommandFooSameCaseUppercase());
            application.Add(new CommandFooSameCaseLowercase());

            application.Find("FoO/BaR");
        }

        [TestMethod]
        public void TestFindWithCommandLoader()
        {
            var application = new Application();

            application.SetCommandLoader(new FactoryCommandLoader(new Dictionary<string, Func<Command.Command>>
            {
                {
                    "foo/bar", () => new CommandAnonymous("foo/bar", "afoo")
                },
            }));

            Assert.AreEqual(typeof(CommandAnonymous), application.Find("foo/bar").GetType(),
                $"{nameof(Application.Find)} returns a command if its name exists");

            Assert.AreEqual(typeof(CommandHelp), application.Find("h").GetType(),
                $"{nameof(Application.Find)} returns a command if its name exists");

            Assert.AreEqual(typeof(CommandAnonymous), application.Find("f/bar").GetType(),
                $"{nameof(Application.Find)} returns a command if the abbreviation for the namespace exists");

            Assert.AreEqual(typeof(CommandAnonymous), application.Find("f/b").GetType(),
                $"{nameof(Application.Find)} returns a command if the abbreviation for the namespace and the command name exist");

            Assert.AreEqual(typeof(CommandAnonymous), application.Find("a").GetType(),
                $"{nameof(Application.Find)} returns a command if the abbreviation exists for an alias");
        }

        [DataRow("f", "Command \"f\" is not defined.")]
        [DataRow("a", "Command \"a\" is ambiguous.\nDid you mean one of these?\n" +
                        "    afoobar  The foo/bar command\n" +
                        "    afoobar1 The foo/bar1 command\n" +
                        "    afoobar2 The foo1/bar command")]
        [DataRow("foo/b", "Command \"foo/b\" is ambiguous.\nDid you mean one of these?\n" +
                        "    foo/bar  The foo/bar command\n" +
                        "    foo/bar1 The foo/bar1 command\n" +
                        "    foo1/bar The foo1/bar command")]
        [TestMethod]
        public void TestFindWithAmbiguousAbbreviations(string abbreviation, string expectedExceptionMessage)
        {
            var application = new Application();

            var command = new CommandAnonymous("foo/bar", "afoobar");
            command.SetDescription("The foo/bar command");
            application.Add(command);

            command = new CommandAnonymous("foo/bar1", "afoobar1");
            command.SetDescription("The foo/bar1 command");
            application.Add(command);

            command = new CommandAnonymous("foo1/bar", "afoobar2");
            command.SetDescription("The foo1/bar command");
            application.Add(command);

            try
            {
                application.Find(abbreviation);
                Assert.Fail("Need throw CommandNotFoundException exception.");
            }
            catch (CommandNotFoundException ex)
            {
                expectedExceptionMessage = expectedExceptionMessage.Replace("\n", Environment.NewLine, StringComparison.Ordinal);
                if (ex.Message.IndexOf(
                    expectedExceptionMessage,
                    StringComparison.InvariantCultureIgnoreCase) < 0)
                {
                    Assert.AreEqual(expectedExceptionMessage, ex.Message);
                }
            }
        }

        [TestMethod]
        public void TestFindCommandEqualNamespace()
        {
            var application = new Application();

            application.Add(new CommandAnonymous("foo3/bar"));
            application.Add(new CommandAnonymous("foo3/bar/xyz"));

            Assert.AreEqual("foo3/bar", application.Find("foo3/bar").Name,
                $"{nameof(Application.Find)}() returns the good command even if a namespace has same name");
            Assert.AreEqual("foo3/bar/xyz", application.Find("foo3/bar/xyz").Name,
                $"{nameof(Application.Find)}() returns a command even if its namespace equals another command name");
        }

        [TestMethod]
        public void TestFindCommandWithAmbiguousNamespacesButUniqueName()
        {
            var application = new Application();
            application.Add(new CommandAnonymous("foo/bar"));
            application.Add(new CommandAnonymous("foobar/foo"));

            Assert.AreEqual("foobar/foo", application.Find("f/f").Name,
                $"{nameof(Application.Find)}() returns the good command even if a ambiguous namespace but unique name.");
        }

        [TestMethod]
        public void TestFindCommandWithMissingNamespace()
        {
            var application = new Application();
            application.Add(new CommandAnonymous("foo4/bar/xyz"));

            Assert.AreEqual("foo4/bar/xyz", application.Find("foo4//xyz").Name,
                $"{nameof(Application.Find)}() returns the good command even if missing namespace.");
        }

        [DataRow("foo3/barr")]
        [DataRow("fooo3/barr")]
        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(CommandNotFoundException),
            "Did you mean this")]
        public void TestFindAlternativeExceptionMessageSingle(string name)
        {
            var application = new Application();
            application.Add(new CommandAnonymous("foo3/bar"));

            application.Find(name);
        }

        [TestMethod]
        public void TestDontRunAlternativeNamespaceName()
        {
            var application = new Application();
            application.Add(new CommandFoo1());
            var tester = new TesterApplication(application);
            tester.Run("foos/baz1", AbstractTester.OptionDecorated(false));

            var display = tester.GetDisplay();
            Assert.AreEqual(true, display.Contains("There are no commands defined in the \"foos\" namespace."));
            Assert.AreEqual(true, display.Contains("Did you mean this?"));
        }

        [TestMethod]
        public void TestCanRunAlternativeCommandName()
        {
            var application = new Application
            {
                CatchExceptions = false,
            };
            application.Add(new CommandFooWithoutAlias());

            var tester = new TesterApplication(application);
            tester.SetInputs(new[] { "y" });

            tester.Run("foos", AbstractTester.OptionDecorated(false));
            var output = tester.GetDisplay();

            Assert.AreEqual(true, output.IndexOf(
                "Command \"foos\" is not defined",
                StringComparison.Ordinal) >= 0);
            Assert.AreEqual(true, output.IndexOf(
                "Do you want to run \"foo\" instead?  (yes/no) [no]:",
                StringComparison.Ordinal) >= 0);
            Assert.AreEqual(true, output.IndexOf(
                "called",
                StringComparison.Ordinal) >= 0);
        }

        [TestMethod]
        public void TestDontRunAlternativeCommandName()
        {
            var application = new Application();
            application.Add(new CommandFooWithoutAlias());

            var tester = new TesterApplication(application);
            tester.SetInputs(new[] { "n" });

            var exitCode = tester.Run("foos", AbstractTester.OptionDecorated(false));
            var display = tester.GetDisplay();

            Assert.AreEqual(1, exitCode);
            Assert.AreEqual(true, display.Contains("Command \"foos\" is not defined"));
            Assert.AreEqual(true, display.Contains("Do you want to run \"foo\" instead?  (yes/no) [no]:"));
        }

        [TestMethod]
        public void TestFindAlternativeExceptionMessageMultiple()
        {
            // Guaranteed to have output space.
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");
            var application = new Application();
            application.Add(new CommandAnonymous("foo/bar"));
            application.Add(new CommandAnonymous("foo/bar1"));
            application.Add(new CommandAnonymous("foo1/bar"));

            // test command
            try
            {
                application.Find("foo/baR");
                throw new System.Exception();
            }
            catch (CommandNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(true, message.Contains("Did you mean one of these"));
                Assert.AreEqual(true, message.Contains("foo/bar"));
                Assert.AreEqual(true, message.Contains("foo/bar1"));
                Assert.AreEqual(true, message.Contains("foo1/bar"));
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }

            // test namespace
            try
            {
                application.Find("foo2/bar");
                throw new System.Exception();
            }
            catch (NamespaceNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(true, message.Contains("Did you mean one of these"));
                Assert.AreEqual(true, message.Contains("foo1"));
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a NamespaceNotFoundException if namespace does not exist, with alternatives");
                throw;
            }

            application.Add(new CommandAnonymous("foo3/bar"));
            application.Add(new CommandAnonymous("foo3/bar/toh"));

            // test subnamespace
            try
            {
                application.Find("foo3/");
                throw new System.Exception();
            }
            catch (CommandNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(true, message.Contains("Did you mean one of these"));
                Assert.AreEqual(true, message.Contains("foo3/bar"));
                Assert.AreEqual(true, message.Contains("foo3/bar/toh"));
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }
        }

        [TestMethod]
        public void TestFindAlternativeCommands()
        {
            var application = new Application();
            application.Add(new CommandAnonymous("foo/bar"));
            application.Add(new CommandAnonymous("foo/bar1", "afoobar1"));
            application.Add(new CommandAnonymous("foo1/bar"));

            string commandName = null;
            try
            {
                application.Find(commandName = "Unknown command");
            }
            catch (CommandNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(0, exception.GetAlternatives().Length);
                Assert.AreEqual(true, message.Contains($"Command \"{commandName}\" is not defined."));
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }

            try
            {
#pragma warning disable S1854
                application.Find(commandName = "bar1");
#pragma warning restore S1854
                throw new System.Exception();
            }
            catch (CommandNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(2, exception.GetAlternatives().Length);
                Assert.AreEqual("afoobar1", exception.GetAlternatives()[0]);
                Assert.AreEqual("foo/bar1", exception.GetAlternatives()[1]);
                Assert.AreEqual(true, message.Contains($"Command \"{commandName}\" is not defined."));
                Assert.AreEqual(true, message.Contains($"afoobar1"));
                Assert.AreEqual(true, message.Contains($"foo/bar1"));
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }
        }

        [TestMethod]
        public void TestFindAlternativeCommandsWithAnAlias()
        {
            var application = new Application();
            var command = new CommandAnonymous("foo/bar", "foo2");
            application.Add(command);
            Assert.AreSame(command, application.Find("foo"));
        }

        [TestMethod]
        public void TestFindAlternativeNamespace()
        {
            var application = new Application();
            application.Add(new CommandAnonymous("foo/bar", "afoobar"));
            application.Add(new CommandAnonymous("foo/bar1", "afoobar1"));
            application.Add(new CommandAnonymous("foo1/bar", "afoobar2"));
            application.Add(new CommandAnonymous("foo3/bar"));

            try
            {
                application.Find("Unknown-namespace/Unknown-command");
                throw new System.Exception();
            }
            catch (CommandNotFoundException exception)
            {
                Assert.AreEqual(0, exception.GetAlternatives().Length);
                Assert.AreEqual("There are no commands defined in the \"Unknown-namespace\" namespace.", exception.Message);
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }

            try
            {
                application.Find("foo2/command");
                throw new System.Exception();
            }
            catch (NamespaceNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(3, exception.GetAlternatives().Length);
                Assert.AreEqual("foo", exception.GetAlternatives()[0]);
                Assert.AreEqual("foo1", exception.GetAlternatives()[1]);
                Assert.AreEqual("foo3", exception.GetAlternatives()[2]);
                Assert.AreEqual(true, message.Contains($"There are no commands defined in the \"foo2\" namespace."));
                Assert.AreEqual(true, message.Contains($"foo"));
                Assert.AreEqual(true, message.Contains($"foo1"));
                Assert.AreEqual(true, message.Contains($"foo3"));
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }
        }

        [TestMethod]
        public void TestFindAlternativesOutput()
        {
            var application = new Application();

            application.Add(new CommandAnonymous("foo/bar", "afoobar"));
            application.Add(new CommandAnonymous("foo/bar1", "afoobar1"));
            application.Add(new CommandAnonymous("foo1/bar", "afoobar2"));
            application.Add(new CommandAnonymous("foo3/bar"));

            try
            {
                application.Find("foo");
                throw new System.Exception();
            }
            catch (CommandNotFoundException exception)
            {
                var message = exception.Message;
                Assert.AreEqual(7, exception.GetAlternatives().Length);
                Assert.AreEqual("afoobar", exception.GetAlternatives()[0]);
                Assert.AreEqual("afoobar1", exception.GetAlternatives()[1]);
                Assert.AreEqual("afoobar2", exception.GetAlternatives()[2]);
                Assert.AreEqual("foo/bar", exception.GetAlternatives()[3]);
                Assert.AreEqual("foo/bar1", exception.GetAlternatives()[4]);
                Assert.AreEqual("foo1/bar", exception.GetAlternatives()[5]);
                Assert.AreEqual("foo3/bar", exception.GetAlternatives()[6]);

                Assert.AreEqual(true, message.Contains($"Command \"foo\" is not defined"));
                Assert.AreEqual(true, message.Contains($"Did you mean one of these"));
            }
            catch (System.Exception)
            {
                Assert.Fail("Find() throws a CommandNotFoundException if command does not exist, with alternatives");
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CommandNotFoundException))]
        public void TestFindWithDoubleColonInNameThrowsException()
        {
            var application = new Application();
            application.Add(new CommandAnonymous("foo/bar", "afoobar"));
            application.Add(new CommandAnonymous("foo3/bar/toh"));

            application.Find("foo//bar");
        }

        [TestMethod]
        public void TestSetCatchExceptions()
        {
            var application = new Application()
            {
                CatchExceptions = true,
            };
            var tester = new TesterApplication(application);

            Assert.AreEqual(true, application.CatchExceptions);
            tester.Run("foo", AbstractTester.OptionDecorated(false));

            Assert.AreEqual(true, tester.GetDisplay().Contains("Command \"foo\" is not defined."));

            tester.Run("foo", AbstractTester.OptionDecorated(false), AbstractTester.OptionStdErrorSeparately(true));

            Assert.AreEqual(true, tester.GetDisplayError().Contains("Command \"foo\" is not defined."));
            Assert.AreEqual(string.Empty, tester.GetDisplay());

            application.CatchExceptions = false;

            try
            {
                tester.Run("foo", AbstractTester.OptionDecorated(false));
                Assert.Fail("CatchExceptions sets the catch exception flag.");
            }
            catch (System.Exception e) when (!(e is AssertFailedException))
            {
                Assert.AreEqual(
                    true,
                    e.Message.Contains("Command \"foo\" is not defined."),
                    "CatchExceptions sets the catch exception flag.");
            }
        }

        [TestMethod]
        public void TestRenderException()
        {
            // Guaranteed to have output space.
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");

            var application = new Application()
            {
                CatchExceptions = true,
            };

            var tester = new TesterApplication(application);
            tester.Run("foo", AbstractTester.OptionDecorated(false), AbstractTester.OptionStdErrorSeparately(true));
            var displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("Command \"foo\" is not defined."));
            Assert.AreEqual(false, displayError.Contains("Exception trace"));

            tester.Run(
                "foo",
                AbstractTester.OptionDecorated(false),
                AbstractTester.OptionStdErrorSeparately(true),
                AbstractTester.OptionVerbosity(OutputOptions.VerbosityVerbose));
            displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("Exception trace"));

            tester.Run(
                "list --foo=true",
                AbstractTester.OptionDecorated(false),
                AbstractTester.OptionStdErrorSeparately(true));
            displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("The \"--foo\" option does not exist."));
            Assert.AreEqual(true, displayError.Contains("list [--raw] [-f--format FORMAT] [--] [<namespace>]"));

            application.Add(new CommandThrowException());
            tester.Run(
                "foo/exception",
                AbstractTester.OptionDecorated(true),
                AbstractTester.OptionVerbosity(OutputOptions.VerbosityVeryVerbose));

            var display = tester.GetDisplay();
            Assert.AreEqual(true, display.Contains("First exception"));
            Assert.AreEqual(true, display.Contains("Second exception"));
            Assert.AreEqual(true, display.Contains("Third exception"));

            tester.Run(
               "foo/exception",
               AbstractTester.OptionDecorated(true),
               AbstractTester.OptionStdErrorSeparately(true),
               AbstractTester.OptionVerbosity(OutputOptions.VerbosityVerbose));
            displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("First exception"));
            Assert.AreEqual(true, displayError.Contains("Second exception"));
            Assert.AreEqual(true, displayError.Contains("Third exception"));

            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "32");
            tester = new TesterApplication(application);

            tester.Run(
               "zxcasd",
               AbstractTester.OptionDecorated(false),
               AbstractTester.OptionStdErrorSeparately(true));

            displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("ined."));
        }

        [TestMethod]
        public void TestRenderExceptionWithDoubleWidthCharacters()
        {
            var application = new Application();

            // Guaranteed to have output space.
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");

            // use double width characters. ex: chinese, japanese
            application.Register("foo").SetCode(() =>
            {
                throw new System.Exception("异常消息");
            });

            var tester = new TesterApplication(application);
            tester.Run(
                "foo",
                AbstractTester.OptionDecorated(false),
                AbstractTester.OptionStdErrorSeparately(true));

            var displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("异常消息"));

            tester.Run(
                "foo",
                AbstractTester.OptionDecorated(true),
                AbstractTester.OptionStdErrorSeparately(true));

            displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("[37;41m  异常消息  [39;49m"));

            application.Register("foo").SetCode(() =>
            {
                throw new System.Exception("让我们测试需要换行的异常消息");
            });

            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "15");
            tester = new TesterApplication(application);
            tester.Run(
                "foo",
                AbstractTester.OptionDecorated(false),
                AbstractTester.OptionStdErrorSeparately(true));
            displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("  让我们测试  "));
            Assert.AreEqual(true, displayError.Contains("  需要换行的  "));
            Assert.AreEqual(true, displayError.Contains("  异常消息    "));
        }

        [TestMethod]
        public void TestRenderExceptionLineBreaks()
        {
            // Guaranteed to have output space.
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");

            var application = new Application();

            application.Register("foo").SetCode(() =>
            {
                throw new System.Exception("hello\r\nworld\nmenghanyu");
            });

            var tester = new TesterApplication(application);
            tester.Run(
                "foo",
                AbstractTester.OptionDecorated(false),
                AbstractTester.OptionStdErrorSeparately(true));

            var displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("  hello  "));
            Assert.AreEqual(true, displayError.Contains("  world  "));
            Assert.AreEqual(true, displayError.Contains("  menghanyu  "));
        }

        [TestMethod]
        public void TestRenderExceptionWithProperty()
        {
            // Guaranteed to have output space.
            Environment.SetEnvironmentVariable(EnvironmentVariables.ConsoleBufferWidth, "120");

            var application = new Application();
            application.Add(new CommandThrowExceptionProperty());

            var tester = new TesterApplication(application);
            tester.Run(
                "foo/exception-property",
                AbstractTester.OptionDecorated(false),
                AbstractTester.OptionStdErrorSeparately(true),
                AbstractTester.OptionVerbosity(OutputOptions.VerbosityDebug));

            var displayError = tester.GetDisplayError();
            Assert.AreEqual(true, displayError.Contains("first property exception"));
        }

        [TestMethod]
        public void TestRunBuiltInCommand()
        {
            var application = new Application()
            {
                CatchExceptions = false,
            };
            var tester = new TesterApplication(application);
            tester.Run(string.Empty, AbstractTester.OptionDecorated(false));
            Assert.AreEqual(
@"Console Tool

Usage:
  command [options] [arguments]

Options:
  -h, --help            Display this help message.
  -q, --quiet           Do not output any message.
  -V, --version         Display this application version.
      --ansi            Force ANSI output.
      --no-ansi         Disable ANSI output.
  -n, --no-interaction  Do not ask any interactive question.
  -v|vv|vvv, --verbose  Increase the verbosity of messages: -v for normal output, -vv for more verbose output and -vvv for debug.

Available commands:
  help  Displays help for a command
  list  Lists commands
", tester.GetDisplay());

            var helptxt = @"Description:
  Lists commands

Usage:
  list [options] [--] [<namespace>]

Arguments:
  command               The command to execute.
  namespace             The namespace name.

Options:
      --raw             To output raw command list.
  -f, --format=FORMAT   The output format (txt, json) [default: txt]
  -h, --help            Display this help message.
  -q, --quiet           Do not output any message.
  -V, --version         Display this application version.
      --ansi            Force ANSI output.
      --no-ansi         Disable ANSI output.
  -n, --no-interaction  Do not ask any interactive question.
  -v|vv|vvv, --verbose  Increase the verbosity of messages: -v for normal output, -vv for more verbose output and -vvv for debug.

Help:
  The list command lists all commands:
    dotnet testhost.dll list
  You can also display the commands for a specific namespace:
    dotnet testhost.dll list namespace
  You can also output the information in other formats by using the --format option:
    dotnet testhost.dll list --format=json
  It's also possible to get raw list of commands (useful for embedding command runner):
    dotnet testhost.dll list --raw
";

            tester.Run("--help", AbstractTester.OptionDecorated(false));
            Assert.AreEqual(helptxt, tester.GetDisplay());

            tester.Run("-h", AbstractTester.OptionDecorated(false));
            Assert.AreEqual(helptxt, tester.GetDisplay());

            tester.Run("list --help", AbstractTester.OptionDecorated(false));
            Assert.AreEqual(helptxt, tester.GetDisplay());

            tester.Run("list -h", AbstractTester.OptionDecorated(false));
            Assert.AreEqual(helptxt, tester.GetDisplay());

            tester.Run("--ansi");
            Assert.AreEqual(true, tester.Output.IsDecorated, "forces color output if --ansi is passed");

            tester.Run("--no-ansi");
            Assert.AreEqual(false, tester.Output.IsDecorated, "forces color output to be disabled if --ansi is passed");

            var version = @"Console Tool
";
            tester.Run("--version");
            Assert.AreEqual(version, tester.GetDisplay());

            tester.Run("-V");
            Assert.AreEqual(version, tester.GetDisplay());

            tester.Run("list --quiet");
            Assert.AreEqual(string.Empty, tester.GetDisplay(), "--quiet hiden all output.");

            tester.Run("list -q");
            Assert.AreEqual(string.Empty, tester.GetDisplay(), "-q hiden all output.");

            tester.Run("list -v");
            Assert.AreEqual(true, tester.Output.IsVerbose, "sets the output to verbose if -v is passed");

            tester.Run("list -vv");
            Assert.AreEqual(true, tester.Output.IsVeryVerbose, "sets the output to verbose if -vv is passed");

            tester.Run("list -vvv");
            Assert.AreEqual(true, tester.Output.IsDebug, "sets the output to verbose if -vvv is passed");

            application.Add(new CommandFoo());
            tester.Run("foo/baz --no-interaction", AbstractTester.OptionDecorated(false));
            Assert.AreEqual(
@"Execute called
", tester.GetDisplay(), "does not call Command.Interact() if --no-interaction is passed");

            tester.Run("foo/baz -n", AbstractTester.OptionDecorated(false));
            Assert.AreEqual(
@"Execute called
", tester.GetDisplay(), "does not call Command.Interact() if -n is passed");
        }

        [TestMethod]
        public void TestRunWithGlobalOptionAndNoCommand()
        {
            var application = new Application()
            {
                CatchExceptions = false,
            };
            application.GetDefinition()
                .AddOptions(
                    new InputOption("foo", null, InputOptionModes.ValueOptional));

            var input = new InputArgs(new string[] { "--foo=bar" });
            Assert.AreEqual(0, application.Run(input));
        }

        [TestMethod]
        public void TestRunReturnsIntegerExitCode()
        {
            var application = new Application();

            application.Register("foo").SetCode(() =>
            {
                throw new ConsoleException("code exception", 4);
            });

            var tester = new TesterApplication(application);
            Assert.AreEqual(4, tester.Run("foo"));
        }

        [TestMethod]
        public void TestRunDispatchesIntegerExitCode()
        {
            var application = new Application();
            IEventDispatcher dispatcher = new Dispatcher();
            application.SetDispatcher(dispatcher);

            var actualExitCode = -1;
            dispatcher.AddListener(ApplicationEvents.ConsoleTerminate, (sender, eventArgs) =>
            {
                actualExitCode = ((ConsoleTerminateEventArgs)eventArgs).ExitCode;
            });

            application.Register("test").SetCode(() =>
            {
                throw new ConsoleException("test exception", 5);
            });

            var tester = new TesterApplication(application);
            tester.Run("test");

            Assert.AreEqual(5, actualExitCode);
        }

        [TestMethod]
        public void TestRunDispatchesGeneralExceptionReturnsExitCode()
        {
            var application = new Application();
            IEventDispatcher dispatcher = new Dispatcher();
            application.SetDispatcher(dispatcher);

            var actualExitCode = -1;
            dispatcher.AddListener(ApplicationEvents.ConsoleTerminate, (sender, eventArgs) =>
            {
                actualExitCode = ((ConsoleTerminateEventArgs)eventArgs).ExitCode;
            });

            application.Register("test").SetCode(() =>
            {
                throw new System.Exception("test exception");
            });

            var tester = new TesterApplication(application);
            tester.Run("test");

            Assert.AreEqual(1, actualExitCode);
        }

        [TestMethod]
        public void TestRunAllowsErrorListenersToSilenceTheException()
        {
            var application = new Application();
            IEventDispatcher dispatcher = new Dispatcher();
            application.SetDispatcher(dispatcher);

            dispatcher.AddListener(ApplicationEvents.ConsoleError, (sender, eventArgs) =>
            {
                ((ConsoleErrorEventArgs)eventArgs).SetExitCode(0);
            });

            application.Register("test").SetCode(() =>
            {
                throw new ConsoleException("test exception", 5);
            });

            var tester = new TesterApplication(application);
            Assert.AreEqual(0, tester.Run("test"));
        }

        [TestMethod]
        public void TestRunEventDispatcherKeepStackTrack()
        {
            var application = new Application();
            IEventDispatcher dispatcher = new Dispatcher();
            application.SetDispatcher(dispatcher);

            application.Register("bar").SetCode(() =>
            {
                throw new ConsoleException("test exception", 1);
            });

            var tester = new TesterApplication(application);
            Assert.AreEqual(1, tester.Run("bar", AbstractTester.OptionVerbosity(OutputOptions.VerbosityDebug)));
            StringAssert.Contains(
                tester.GetDisplay(),
                "at GameBox.Console.Tests.TestsApplication.<>c.<TestRunEventDispatcherKeepStackTrack>");
        }

        [TestMethod]
        public void TestRunEventDispatcherKeepStackTrackWithCommandNotFound()
        {
            var application = new Application();
            IEventDispatcher dispatcher = new Dispatcher();
            application.SetDispatcher(dispatcher);

            var tester = new TesterApplication(application);
            Assert.AreEqual(1, tester.Run("commnad-not-found", AbstractTester.OptionVerbosity(OutputOptions.VerbosityDebug)));
            StringAssert.Contains(
                tester.GetDisplay(),
                "in GameBox.Console.Application.Find(String)");
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(ConsoleLogicException),
            "An option with shortcut \"e\" already exists.")]
        public void TestAddingOptionWithDuplicateShortcut()
        {
            var application = new Application()
            {
                CatchExceptions = false,
            };
            application
                .GetDefinition()
                .AddOptions(
                new InputOption("env", "-e", InputOptionModes.ValueRequired, "Environment"));

            application
                .Register("foo")
                .AddOption("foo", "-e", InputOptionModes.ValueRequired, "My option with a shortcut.")
                .SetCode(() => { });

            var tester = new TesterApplication(application);
            tester.Run("foo");
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(ConsoleLogicException),
            "An argument with name \"command\" already exists.")]
        public void TestAddingAlreadySetDefinition()
        {
            var application = new Application()
            {
                CatchExceptions = false,
            };

            application.Register("foo")
                .SetDefinition(new InputArgument(Command.Command.ArgumentCommand, InputArgumentModes.Required,
                    "The command to execute."))
                .SetCode(() => { });

            var tester = new TesterApplication(application);
            tester.Run("foo");
        }

        [TestMethod]
        public void TestGetDefaultInputDefinitionReturnsDefaultValues()
        {
            var application = new Application();
            var definition = application.GetDefinition();

            Assert.AreEqual(true, definition.HasArgument(Command.Command.ArgumentCommand));

            Assert.AreEqual(true, definition.HasOption("help"));
            Assert.AreEqual(true, definition.HasOption("quiet"));
            Assert.AreEqual(true, definition.HasOption("verbose"));
            Assert.AreEqual(true, definition.HasOption("version"));
            Assert.AreEqual(true, definition.HasOption("ansi"));
            Assert.AreEqual(true, definition.HasOption("no-ansi"));
            Assert.AreEqual(true, definition.HasOption("no-interaction"));
        }

        [TestMethod]
        public void TestOverwritingDefaultInputDefinitionOverwritesDefaultValues()
        {
            var application = new CustomApplication();
            var definition = application.GetDefinition();

            Assert.AreNotEqual(true, definition.HasArgument(Command.Command.ArgumentCommand));

            Assert.AreNotEqual(true, definition.HasOption("help"));
            Assert.AreNotEqual(true, definition.HasOption("quiet"));
            Assert.AreNotEqual(true, definition.HasOption("verbose"));
            Assert.AreNotEqual(true, definition.HasOption("version"));
            Assert.AreNotEqual(true, definition.HasOption("ansi"));
            Assert.AreNotEqual(true, definition.HasOption("no-ansi"));
            Assert.AreNotEqual(true, definition.HasOption("no-interaction"));

            Assert.AreEqual(true, definition.HasOption("custom"));
        }

        [TestMethod]
        public void TestSettingCustomInputDefinitionOverwritesDefaultValues()
        {
            var application = new CustomApplication();
            application.SetDefinition(new InputDefinition(
                    new InputOption("custom", "-c", InputOptionModes.ValueNone, "custom option")));

            var definition = application.GetDefinition();

            Assert.AreNotEqual(true, definition.HasArgument(Command.Command.ArgumentCommand));

            Assert.AreNotEqual(true, definition.HasOption("help"));
            Assert.AreNotEqual(true, definition.HasOption("quiet"));
            Assert.AreNotEqual(true, definition.HasOption("verbose"));
            Assert.AreNotEqual(true, definition.HasOption("version"));
            Assert.AreNotEqual(true, definition.HasOption("ansi"));
            Assert.AreNotEqual(true, definition.HasOption("no-ansi"));
            Assert.AreNotEqual(true, definition.HasOption("no-interaction"));

            Assert.AreEqual(true, definition.HasOption("custom"));
        }

        [TestMethod]
        public void TestRunWithDispatcher()
        {
            var application = new Application();
            application.SetDispatcher(CreateTestDispatcher());

            application.Register("foo").SetCode((output) => { output.Write("foo."); });

            var tester = new TesterApplication(application);
            tester.Run("foo");

            Assert.AreEqual("command.foo.terminate.", tester.GetDisplay());
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(typeof(ConsoleLogicException), "error.")]
        public void TestRunWithExceptionAndDispatcher()
        {
            var application = new Application()
            {
                CatchExceptions = false,
            };
            application.SetDispatcher(CreateTestDispatcher());

            application.Register("foo").SetCode((output) =>
            {
                throw new ConsoleRuntimeException("exception.");
            });

            var tester = new TesterApplication(application);
            tester.Run("foo");
        }

        [TestMethod]
        public void TestRunDispatchesAllEventsWithException()
        {
            var application = new Application();
            application.SetDispatcher(CreateTestDispatcher());

            application.Register("foo").SetCode((output) =>
            {
                output.Write("foo.");
                throw new ConsoleRuntimeException("exception.");
            });

            var tester = new TesterApplication(application);
            tester.Run("foo");

            Assert.AreEqual(true, tester.GetDisplay().Contains("command.foo.error.terminate."));
        }

        [TestMethod]
        public void TestRunDispatchesAllEventsWithExceptionInListener()
        {
            var application = new Application();
            var dispatcher = CreateTestDispatcher();

            dispatcher.AddListener(ApplicationEvents.ConsoleCommand, (sender, eventArgs) =>
            {
                throw new ConsoleRuntimeException("foo");
            });

            application.SetDispatcher(dispatcher);

            application.Register("foo").SetCode((output) =>
            {
                output.Write("foo.");
            });

            var tester = new TesterApplication(application);
            tester.Run("foo");
            Assert.AreEqual(true, tester.GetDisplay().Contains("command.error.terminate."));
        }

        [TestMethod]
        public void TestRunTerminateEventChangeTheExitCode()
        {
            var application = new Application();
            var dispatcher = CreateTestDispatcher();

            dispatcher.AddListener(ApplicationEvents.ConsoleTerminate, (sender, eventArgs) =>
            {
                ((ConsoleTerminateEventArgs)eventArgs).SetExitCode(10);
            });

            application.SetDispatcher(dispatcher);

            application.Register("foo").SetCode((output) =>
            {
                output.Write("foo.");
            });

            var tester = new TesterApplication(application);
            Assert.AreEqual(10, tester.Run("foo"));
        }

        [TestMethod]
        public void TestConsoleErrorEventIsTriggeredOnCommandNotFound()
        {
            var application = new Application();
            var dispatcher = new Dispatcher();

            dispatcher.AddListener(ApplicationEvents.ConsoleError, (sender, eventArgs) =>
            {
                var errorEventArgs = (ConsoleErrorEventArgs)eventArgs;
                Assert.AreEqual(null, errorEventArgs.Command);
                Assert.AreEqual(true, errorEventArgs.Exception is CommandNotFoundException);
                errorEventArgs.Output.Write("silenced command not found.");
            });

            application.SetDispatcher(dispatcher);

            var tester = new TesterApplication(application);
            Assert.AreEqual(1, tester.Run("unknow command"));
            System.Console.WriteLine(tester.GetDisplay());
            Assert.AreEqual(true, tester.GetDisplay().Contains("silenced command not found."));
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(typeof(ConsoleRuntimeException), "not handled.")]
        public void TestErrorIsRethrownIfNotHandledByConsoleErrorEvent()
        {
            var application = new Application()
            {
                CatchExceptions = false,
            };
            application.SetDispatcher(new Dispatcher());

            application.Register("foo").SetCode((output) =>
            {
                throw new ConsoleRuntimeException("not handled.");
            });

            var tester = new TesterApplication(application);
            tester.Run("foo");
        }

        [TestMethod]
        public void TestRunDispatchesAllEventsWithError()
        {
            var application = new Application();
            application.SetDispatcher(CreateTestDispatcher());

            application.Register("foo").SetCode((output) =>
            {
                output.Write("foo.");
                throw new ConsoleRuntimeException("fooerror.");
            });

            var tester = new TesterApplication(application);
            tester.Run("foo");

            Assert.AreEqual(true, tester.GetDisplay().Contains("command.foo.error.terminate."));
        }

        private IEventDispatcher CreateTestDispatcher(bool skipCommand = false)
        {
            var dispatcher = new Dispatcher();

            dispatcher.AddListener(ApplicationEvents.ConsoleCommand, (sender, eventArgs) =>
            {
                var commandEventArgs = (ConsoleCommandEventArgs)eventArgs;
                commandEventArgs.Output.Write("command.");
                commandEventArgs.SetSkipCommand(skipCommand);
            });

            dispatcher.AddListener(ApplicationEvents.ConsoleTerminate, (sender, eventArgs) =>
            {
                var terminateEventArgs = (ConsoleTerminateEventArgs)eventArgs;
                terminateEventArgs.Output.Write("terminate.");
                if (!skipCommand)
                {
                    terminateEventArgs.SetExitCode(ExitCodes.SkipCommnad);
                }
            });

            dispatcher.AddListener(ApplicationEvents.ConsoleError, (sender, eventArgs) =>
            {
                var errorEventArgs = (ConsoleErrorEventArgs)eventArgs;
                errorEventArgs.Output.Write("error.");
                errorEventArgs.SetException(new ConsoleLogicException("error.", errorEventArgs.ExitCode, errorEventArgs.Exception));
            });

            return dispatcher;
        }

        private class CustomApplication : Application
        {
            protected override InputDefinition CreateDefaultInputDefinition()
            {
                return new InputDefinition(
                    new InputOption("custom", "-c", InputOptionModes.ValueNone, "custom option"));
            }
        }
    }
}
