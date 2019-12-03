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
using GameBox.Console.Policy.Configure;
using GameBox.Console.Style;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;

namespace GameBox.Console
{
    /// <summary>
    /// An Application is the container for a collection of commands.
    /// It is the main entry point of a Console application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// The namespace symbol.
        /// </summary>
        public const char NamespaceSymbol = '/';

        /// <summary>
        /// The default application name or version.
        /// </summary>
        internal const string Unknow = "UNKNOWN";

        /// <summary>
        /// The policies with configure.
        /// </summary>
        private readonly List<IConfigurePolicy> configurePolicies;

        /// <summary>
        /// The mapping for the command name or alias.
        /// </summary>
        private readonly Dictionary<string, Command.Command> commands;

        /// <summary>
        /// Default command executed when there is no command.
        /// </summary>
        private string defaultCommand = CommandList.Name;

        /// <summary>
        /// Indicates the command current running.
        /// </summary>
        private Command.Command runningCommand;

        /// <summary>
        /// The global event listener system.
        /// </summary>
        private IEventDispatcher dispatcher;

        /// <summary>
        /// The command loader.
        /// </summary>
        private ICommandLoader commandLoader;

        /// <summary>
        /// The <see cref="InputDefinition"/> for this application.
        /// </summary>
        private InputDefinition definition;

        /// <summary>
        /// Whether the application is initialize.
        /// </summary>
        private bool inited;

        /// <summary>
        /// Whether the command need helps.
        /// </summary>
        private bool wantHelps;

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="name">The name of the application.</param>
        /// <param name="version">The version of the application.</param>
        public Application(string name = Unknow, string version = Unknow)
        {
            SetName(name);
            Version = version;
            commands = new Dictionary<string, Command.Command>();
            configurePolicies = new List<IConfigurePolicy>
            {
                new ConfigureEncoding(this),
                new ConfigureAnsi(),
                new ConfigureInteractive(),
                new ConfigureVerbosity(),
            };
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether needs catch the exceptions.
        /// </summary>
        public bool CatchExceptions { get; set; } = true;

        /// <summary>
        /// Gets or sets the input and output encoding.
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Runs the current application.
        /// </summary>
        /// <remarks>see: https://www.tldp.org/LDP/abs/html/exitcodes.html detail.</remarks>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        /// <returns>Returns 0 if everything went fine, or an error code(1-255).</returns>
        public virtual int Run(IInput input = null, IOutput output = null)
        {
            input = input ?? new InputArgs();
            output = output ?? new OutputConsole();

            ExecuteConfigurePolicy(input, output);

            int exitCode;
            try
            {
                exitCode = DoRun(input, output);
            }
            catch (System.Exception exception) when (CatchExceptions)
            {
                RenderException(exception, output);

                if (exception is IException temp)
                {
                    exitCode = temp.ExitCode <= ExitCodes.Normal ? ExitCodes.GeneralException : temp.ExitCode;
                }
                else
                {
                    exitCode = ExitCodes.GeneralException;
                }
            }

            return exitCode;
        }

        /// <summary>
        /// Returns the long version of the application.
        /// </summary>
        /// <returns>The long version of the application.</returns>
        public virtual string GetLongVersion()
        {
            if (Name == Unknow)
            {
                return "Console Tool";
            }

            return Version != Unknow ? $"{Name} <info>{Version}</info>" : Name;
        }

        /// <summary>
        /// Gets the help message.
        /// </summary>
        /// <returns>The help message.</returns>
        public virtual string GetHelp() => GetLongVersion();

        /// <summary>
        /// Sets the application name.
        /// </summary>
        /// <param name="name">The application name.</param>
        public void SetName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Sets the application version.
        /// </summary>
        /// <param name="version">The application version.</param>
        public void SetVersion(string version)
        {
            Version = version;
        }

        /// <summary>
        /// Sets the global event listener system.
        /// </summary>
        /// <param name="eventDispatcher">The event listener system.</param>
        public void SetDispatcher(IEventDispatcher eventDispatcher)
        {
            dispatcher = eventDispatcher;
        }

        /// <summary>
        /// Sets a loader to load the command.
        /// </summary>
        /// <param name="loader">The command loader.</param>
        public void SetCommandLoader(ICommandLoader loader)
        {
            commandLoader = loader;
        }

        /// <summary>
        /// Sets default command executed when there is no command.
        /// </summary>
        /// <param name="command">The default command.</param>
        /// <returns>The <see cref="Application"/> instance.</returns>
        public Application SetDefaultCommand(string command)
        {
            defaultCommand = command;
            return this;
        }

        /// <summary>
        /// Sets the global definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public void SetDefinition(InputDefinition definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Adds the configure policy.
        /// </summary>
        /// <param name="policy">The configure policy.</param>
        public void AddConfigurePolicy(IConfigurePolicy policy)
        {
            if (configurePolicies.Contains(policy))
            {
                throw new ConsoleLogicException($"The policy {policy} already exists.");
            }

            configurePolicies.Add(policy);
        }

        /// <summary>
        /// Registers a new command.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <returns>The command instance.</returns>
        public Command.Command Register(string name)
        {
            return Add(new Command.Command(name));
        }

        /// <summary>
        /// Finds a command by name or alias.
        /// this command tries to find the best match if you give it an abbreviation.
        /// of a name or alias.
        /// </summary>
        /// <param name="name">The command name or alias.</param>
        /// <returns>The command instance.</returns>
        public Command.Command Find(string name)
        {
            Initialize();

            var allCommands = commandLoader != null ? commandLoader.GetNames() : Array.Empty<string>();
            allCommands = Arr.Merge(allCommands, commands.Keys.ToArray()).Distinct().ToArray();

            var nameSegment = name.Split(NamespaceSymbol);
            nameSegment = Arr.Map(nameSegment, (str) => Regex.Escape(str) + $"[^{NamespaceSymbol}]*[^{NamespaceSymbol}]*");

            var expr = string.Join(NamespaceSymbol.ToString(), nameSegment);
            var matchCommands = Arr.Filter(allCommands, (item) => Regex.IsMatch(item, $"^{expr}"));

            if (matchCommands.Length <= 0)
            {
                matchCommands = Arr.Filter(allCommands, (item) => Regex.IsMatch(item, $"^{expr}", RegexOptions.IgnoreCase));
            }

            if (matchCommands.Length <= 0 || !Arr.Test(allCommands, (command) => Regex.IsMatch(command, $"^{expr}$", RegexOptions.IgnoreCase)))
            {
                var pos = name.IndexOf(NamespaceSymbol);
                if (pos != -1)
                {
                    // check if a namespace exists.
                    FindNamespace(name.Substring(0, pos));
                }

                var (message, alternatives) =
                    ForamtDidYouMeanThis(name, allCommands, $"Command \"{name}\" is not defined.");

                throw new CommandNotFoundException(message, alternatives);
            }

            var exact = Array.Exists(matchCommands, (command) => command == name);

            if (!exact && matchCommands.Length > 1)
            {
                // filter out aliases for commands which are already on the list
                var exists = new HashSet<Command.Command>();
                matchCommands = Arr.Filter(matchCommands, (command) =>
                {
                    var commandInstance = Has(command) ? commands[command] : null;
                    if (commandInstance == null)
                    {
                        return false;
                    }

                    return exists.Add(commandInstance);
                });
            }

            if (matchCommands.Length == 1 || exact)
            {
                return Get((exact || matchCommands.Length <= 0) ? name : matchCommands[0]);
            }

            // The edge of the form retains 10 lengths as padding.
            var usableWidth = Terminal.Width - 10;
            var maxCommandNameLength = 0;
            foreach (var command in matchCommands)
            {
                maxCommandNameLength = Math.Max(Str.Width(command), maxCommandNameLength);
            }

            var abbrev = new StringBuilder();
            var abbrevs = Arr.Map(matchCommands, (command) =>
            {
                abbrev.Append(Str.Pad(maxCommandNameLength, command))
                    .Append(Str.Space).Append(Get(command).Description);

                try
                {
                    if (Str.Width(abbrev.ToString()) > usableWidth)
                    {
                        if (usableWidth > 3)
                        {
                            return abbrev.ToString(0, usableWidth - 3) + "...";
                        }
                        else
                        {
                            return "...";
                        }
                    }

                    return abbrev.ToString();
                }
                finally
                {
                    abbrev.Clear();
                }
            });

            var suggestions = GetAbbreviationSuggestions(abbrevs);

            throw new CommandNotFoundException(
                $"Command \"{name}\" is ambiguous.{Environment.NewLine}Did you mean one of these?{Environment.NewLine}{suggestions}");
        }

        /// <summary>
        /// Finds a registered namespace by a name or an abbreviation.
        /// </summary>
        /// <param name="namespace">A namespace or abbreviation to search for.</param>
        /// <returns>A registered namespace.</returns>
        /// <exception cref="NamespaceNotFoundException">When namespace is incorrect or ambiguous.</exception>
        public string FindNamespace(string @namespace)
        {
            var allNamespaces = GetNamespaces();
            var namespaces = Arr.Filter(
                allNamespaces,
                (item) => item.IndexOf(@namespace, StringComparison.OrdinalIgnoreCase) == 0);

            if (namespaces.Length <= 0)
            {
                var (message, alternatives) = ForamtDidYouMeanThis(@namespace, allNamespaces,
                    $"There are no commands defined in the \"{@namespace}\" namespace.");

                throw new NamespaceNotFoundException(message, alternatives);
            }

            var exact = Array.Exists(namespaces, (item) => item == @namespace);

            if (namespaces.Length > 1 && !exact)
            {
                throw new NamespaceNotFoundException(
                    $"The namespace \"{@namespace}\" is ambiguous.{Environment.NewLine}Did you mean one of these?{Environment.NewLine}{GetAbbreviationSuggestions(namespaces)}",
                    namespaces);
            }

            return exact ? @namespace : namespaces[0];
        }

        /// <summary>
        /// Gets an command by name or alias.
        /// </summary>
        /// <param name="name">The name or alias.</param>
        /// <returns>The command instance.</returns>
        public Command.Command Get(string name)
        {
            Initialize();

            if (!Has(name))
            {
                throw new CommandNotFoundException($"The command {name} does not exist.");
            }

            var command = commands[name];

            if (!wantHelps)
            {
                return command;
            }

            wantHelps = false;
            var helpCommand = (CommandHelp)Get(CommandHelp.Name);
            helpCommand.SetCommand(command);
            return helpCommand;
        }

        /// <summary>
        /// Returns an array of all unique namespaces used by currently registered commands.
        /// </summary>
        /// <returns>An array of namespace.</returns>
        public string[] GetNamespaces()
        {
            var namespaces = new List<string>();
            foreach (var command in All())
            {
                namespaces.AddRange(ExtractAllNamespace(command.NameOrAlias));

                foreach (var alias in command.Command.GetAliases())
                {
                    namespaces.AddRange(ExtractAllNamespace(alias));
                }
            }

            return namespaces.Distinct().ToArray();
        }

        /// <summary>
        /// Get the <see cref="InputDefinition"/> related to this Application.
        /// </summary>
        /// <returns>The <see cref="InputDefinition"/> for this Application.</returns>
        public virtual InputDefinition GetDefinition()
        {
            definition = definition ?? CreateDefaultInputDefinition();
            return definition;
        }

        /// <summary>
        /// Check if the command exists.
        /// </summary>
        /// <param name="name">The name or alias.</param>
        /// <returns>True if the command exists, false otherwise.</returns>
        public bool Has(string name)
        {
            Initialize();

            if (commands.ContainsKey(name))
            {
                return true;
            }

            if (commandLoader == null)
            {
                return false;
            }

            return commandLoader.Has(name) && (Add(commandLoader.Load(name)) != null);
        }

        /// <summary>
        /// Adds a command instance.
        /// </summary>
        /// <remarks>
        /// If the command with the same name alreay exists, it will be overriden.
        /// If the command not enabled it wile be not added.
        /// </remarks>
        /// <param name="command">The command instance.</param>
        /// <returns>True if added.</returns>
        public Command.Command Add(Command.Command command)
        {
            Initialize();

            // We needs set application first, because something
            // checked need the application.
            command.SetApplication(this);
            if (command.IsHidden)
            {
                command.SetApplication(null);
                return null;
            }

            command.Initialize();

            if (string.IsNullOrEmpty(command.Name))
            {
                throw new ConsoleLogicException(
                    $"The command defined in {command} cannot have an empty name.");
            }

            commands[command.Name] = command;
            foreach (var alias in command.GetAliases())
            {
                commands[alias] = command;
            }

            return command;
        }

        /// <summary>
        /// Gets the commands in given namespace.
        /// </summary>
        /// <remarks>
        /// The tuple names are the full names or alias of the command.
        /// </remarks>
        /// <param name="namespace">The namespace.</param>
        /// <returns>An array of Command instances.</returns>
        public (string NameOrAlias, Command.Command Command)[] All(string @namespace = null)
        {
            Initialize();

            List<(string, Command.Command)> results;
            if (@namespace == null)
            {
                var mapping = Arr.Map(commands, (item) => (item.Key, item.Value));
                if (commandLoader == null)
                {
                    return mapping;
                }

                // Do not reutrns alias command from ICommandLoader
                // The motivation behind ICommandLoader is lazy-loading,
                // it only maps command names to command factories, which means
                // it can't use Command.Aliases as it does not deal with command instances.
                results = new List<(string, Command.Command)>(mapping);

                foreach (var name in commandLoader.GetNames())
                {
                    if (results.Exists((tuple) => tuple.Item1 == name) || !Has(name))
                    {
                        continue;
                    }

                    var item = Get(name);
                    results.Add((name, item));
                }

                return results.ToArray();
            }

            results = new List<(string, Command.Command)>();
            foreach (var item in commands)
            {
                if (@namespace == ExtractNamespace(item.Key))
                {
                    results.Add((item.Key, item.Value));
                }
            }

            if (commandLoader == null)
            {
                return results.ToArray();
            }

            foreach (var name in commandLoader.GetNames())
            {
                if (results.Exists((tuple) => tuple.Item1 == name) || !Has(name)
                    || @namespace != ExtractNamespace(name))
                {
                    continue;
                }

                var item = Get(name);
                results.Add((item.Name, item));
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns the namespace part of the command name.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="limit">The maximum number of parts of the namespace.</param>
        /// <returns>The namespace part of the command name.</returns>
        internal static string ExtractNamespace(string name, int? limit = null)
        {
            var parts = name.Split(new[] { NamespaceSymbol }, StringSplitOptions.RemoveEmptyEntries);
            Arr.Pop(ref parts);

            return string.Join(NamespaceSymbol.ToString(), Arr.Slice(parts, 0, limit));
        }

        /// <summary>
        /// Runs the current application.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        /// <returns>int 0 if everything went fine, or an error code.</returns>
        protected virtual int DoRun(IInput input, IOutput output)
        {
            if (input.HasRawOption("--version", true) || input.HasRawOption("-V", true))
            {
                output.WriteLine(GetLongVersion());
                return 0;
            }

            var commandName = GetCommandName(input);

            if (input.HasRawOption("--help", true) ||
                input.HasRawOption("-h", true))
            {
                if (string.IsNullOrEmpty(commandName))
                {
                    commandName = CommandHelp.Name;
                    input = new InputArgs(new[] { commandName, "list" });
                }
                else
                {
                    wantHelps = true;
                }
            }

            if (string.IsNullOrEmpty(commandName))
            {
                commandName = defaultCommand;
            }

            Command.Command command;
            try
            {
                runningCommand = null;
                command = Find(commandName);
            }
            catch (System.Exception exception)
            {
                // Not command not found exception or there are multiple alternatives
                // Then terminate the process.
                if (!(exception is CommandNotFoundException commandNotFoundException && !(exception is NamespaceNotFoundException))
                    || (commandNotFoundException.GetAlternatives().Length != 1)
                    || !input.IsInteractive)
                {
                    if (dispatcher == null)
                    {
                        throw;
                    }

                    var eventArgs = new ConsoleErrorEventArgs(input, output, exception);
                    dispatcher.Dispatch(ApplicationEvents.ConsoleError, this, eventArgs);

                    if (eventArgs.ExitCode == 0)
                    {
                        return 0;
                    }

                    // Use the throw guard.
                    ExceptionDispatchInfo.Capture(eventArgs.Exception).Throw();
                    throw eventArgs.Exception;
                }

                var alternative = commandNotFoundException.GetAlternatives()[0];
                var style = new StyleDefault(input, output);
                style.Block(
                    $"{Environment.NewLine}Command \"{commandName}\" is not defined.{Environment.NewLine}",
                    null, "error");

                if (!style.AskConfirm($"Do you want to run \"{alternative}\" instead? ", false))
                {
                    if (dispatcher == null)
                    {
                        return ExitCodes.GeneralException;
                    }

                    var eventArgs = new ConsoleErrorEventArgs(input, output, exception);
                    dispatcher.Dispatch(ApplicationEvents.ConsoleError, this, eventArgs);
                    return Math.Min(eventArgs.ExitCode, 255);
                }

                command = Find(alternative);
            }

            runningCommand = command;
            var exitCode = DoRunCommand(runningCommand, input, output);
            runningCommand = null;

            return Math.Min(exitCode, 255);
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <returns>The command name.</returns>
        protected virtual string GetCommandName(IInput input)
        {
            return input.GetFirstArgument();
        }

        /// <summary>
        /// Gets the default input definition.
        /// </summary>
        /// <returns>An <see cref="InputDefinition"/> instance.</returns>
        protected virtual InputDefinition CreateDefaultInputDefinition()
        {
            return new InputDefinition(
                new InputArgument(Command.Command.ArgumentCommand, InputArgumentModes.Required,
                    "The command to execute."),
                new InputOption("--help", "-h", InputOptionModes.ValueNone, "Display this help message."),
                new InputOption("--quiet", "-q", InputOptionModes.ValueNone, "Do not output any message."),
                new InputOption("--verbose", "-v|vv|vvv", InputOptionModes.ValueNone,
                    "Increase the verbosity of messages: -v for normal output, -vv for more verbose output and -vvv for debug."),
                new InputOption("--version", "-V", InputOptionModes.ValueNone, "Display this application version."),
                new InputOption("--ansi", string.Empty, InputOptionModes.ValueNone, "Force ANSI output."),
                new InputOption("--no-ansi", string.Empty, InputOptionModes.ValueNone, "Disable ANSI output."),
                new InputOption("--no-interaction", "-n", InputOptionModes.ValueNone,
                    "Do not ask any interactive question."));
        }

        /// <summary>
        /// Gets the default commands that should always be available.
        /// </summary>
        /// <returns>The default commands.</returns>
        protected virtual Command.Command[] GetDefaultCommands()
        {
            return new Command.Command[]
            {
                new CommandList(),
                new CommandHelp(),
            };
        }

        /// <summary>
        /// Runs the current command.
        /// </summary>
        /// <param name="command">The current command.</param>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        /// <returns>int 0 if everything went fine, or an error code.</returns>
        protected int DoRunCommand(Command.Command command, IInput input, IOutput output)
        {
            // todo: set helper std input
            if (dispatcher == null)
            {
                return command.Run(input, output);
            }

            // build command environmental before the EventArgsConsoleCommand event,
            // so the listeners have access to input options/arguments
            command.BuildEnvironmentalPreparation(input);

            int exitCode;
            ExceptionDispatchInfo exceptionDispatchInfo = null;
            try
            {
                var eventArgs = new ConsoleCommandEventArgs(command, input, output);
                dispatcher.Dispatch(ApplicationEvents.ConsoleCommand, this, eventArgs);
                exitCode = eventArgs.SkipCommand
                    ? ExitCodes.SkipCommnad
                    : command.Run(input, output);
            }
#pragma warning disable CA1031
            catch (System.Exception exception)
            {
                var eventArgs = new ConsoleErrorEventArgs(input, output, exception, command);
                dispatcher.Dispatch(ApplicationEvents.ConsoleError, this, eventArgs);
                if ((exitCode = eventArgs.ExitCode) != 0)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(eventArgs.Exception);
                }
            }
#pragma warning restore CA1031

            var terminateEvent = new ConsoleTerminateEventArgs(command, input, output, exitCode);
            dispatcher.Dispatch(ApplicationEvents.ConsoleTerminate, this, terminateEvent);

            exceptionDispatchInfo?.Throw();
            return terminateEvent.ExitCode;
        }

        /// <summary>
        /// Renders a caught exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="output">The standard error instance.</param>
        protected void RenderException(System.Exception exception, IOutput output)
        {
            output = (output as IOutputConsole)?.GetErrorOutput() ?? output;

            output.WriteLine(string.Empty, OutputOptions.VerbosityQuiet);

            DoRenderException(exception, output);

            if (runningCommand == null)
            {
                return;
            }

            output.WriteLine($"<info>{string.Format(runningCommand.GetSynopsis(), Name)}</info>", OutputOptions.VerbosityQuiet);
            output.WriteLine(string.Empty, OutputOptions.VerbosityQuiet);
        }

        /// <inheritdoc cref="RenderException"/>
        protected virtual void DoRenderException(System.Exception exception, IOutput output)
        {
            Policy.Render.RenderException.Render(exception, output);
        }

        /// <summary>
        /// Returns all namespaces of the command name.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <returns>All namespace combination of the command name.</returns>
        private static string[] ExtractAllNamespace(string name)
        {
            var parts = name.Split(NamespaceSymbol);
            Arr.Pop(ref parts);

            return Str.JoinList(parts, NamespaceSymbol);
        }

        /// <summary>
        /// Format the alternatives message.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="collection">An array of collection.</param>
        /// <param name="message">The output message.</param>
        /// <returns>The alternatives message.</returns>
        private static (string message, string[] alternatives) ForamtDidYouMeanThis(string name, string[] collection,
            string message = null)
        {
            message = message ?? string.Empty;
            var alternatives = FindAlternatives(name, collection);

            if (alternatives.Length <= 0)
            {
                return (message, alternatives);
            }

            message += alternatives.Length == 1
                ? $"{Environment.NewLine}{Environment.NewLine}Did you mean this?{Environment.NewLine}    "
                : $"{Environment.NewLine}{Environment.NewLine}Did you mean one of these?{Environment.NewLine}    ";

            return (message + string.Join($"{Environment.NewLine}    ", alternatives), alternatives);
        }

        /// <summary>
        /// Returns abbreviated suggestions in string format.
        /// </summary>
        /// <param name="abbrevs">Abbreviated suggestions to convert.</param>
        /// <returns>A formatted string of abbreviated suggestions.</returns>
        private static string GetAbbreviationSuggestions(string[] abbrevs)
        {
            return "    " + string.Join($"{Environment.NewLine}    ", abbrevs);
        }

        /// <summary>
        /// Finds alternative of <paramref name="name"/> among <paramref name="collection"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="collection">An array of collection.</param>
        /// <returns>A sorted array of similar string.</returns>
        private static string[] FindAlternatives(string name, string[] collection)
        {
            const double threshold = 1e3;
            var alternatives = new Dictionary<string, double>();
            var collectionParts = new (string collectionName, string[] parts)[collection.Length];

            for (var index = 0; index < collection.Length; index++)
            {
                collectionParts[index] = (collection[index], collection[index].Split(NamespaceSymbol));
            }

            var subnames = name.Split(NamespaceSymbol);
            for (var i = 0; i < subnames.Length; i++)
            {
                foreach (var (collectionName, parts) in collectionParts)
                {
                    var exists = alternatives.ContainsKey(collectionName);
                    if (exists && (i >= parts.Length))
                    {
                        alternatives[collectionName] += threshold;
                        continue;
                    }

                    if (i >= parts.Length)
                    {
                        continue;
                    }

                    var distance = Str.Levenshtein(subnames[i], parts[i]);

                    if (distance <= (subnames[i].Length / 3) || (!string.IsNullOrEmpty(subnames[i]) &&
                        parts[i].IndexOf(subnames[i], StringComparison.Ordinal) >= 0))
                    {
                        alternatives[collectionName] = exists
                            ? alternatives[collectionName] + distance
                            : distance;
                    }
                    else if (exists)
                    {
                        alternatives[collectionName] += threshold;
                    }
                }
            }

            foreach (var item in collection)
            {
                var distance = Str.Levenshtein(name, item);
                if (distance < (name.Length / 3) || item.IndexOf(name, StringComparison.Ordinal) >= 0)
                {
                    alternatives[item] = alternatives.ContainsKey(item)
                        ? alternatives[item] - distance
                        : distance;
                }
            }

            var result = Arr.Filter(alternatives, (item) => item.Value < (2 * threshold));

            Array.Sort(
                result,
                (left, right) => string.Compare(left.ToString(), right.ToString(), StringComparison.Ordinal));

            return Arr.Map(result, (item) => item.Key);
        }

        /// <summary>
        /// Configures the input and output instances based on the user arguments and options.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        /// <param name="output">The std output instance.</param>
        private void ExecuteConfigurePolicy(IInput input, IOutput output)
        {
            foreach (var policy in configurePolicies)
            {
                policy.Execute(input, output);
            }
        }

        /// <summary>
        /// Initialize the console application.
        /// </summary>
        private void Initialize()
        {
            // This method should not be called in the constructor.
            // because it called the virtual method
            if (inited)
            {
                return;
            }

            inited = true;
            foreach (var command in GetDefaultCommands())
            {
                Add(command);
            }
        }
    }
}
