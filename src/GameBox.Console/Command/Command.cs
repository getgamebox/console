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
using GameBox.Console.Input;
using GameBox.Console.Output;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GameBox.Console.Command
{
    /// <summary>
    /// Base class for all commands.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// The argument command name.
        /// </summary>
        public const string ArgumentCommand = "command";

        /// <summary>
        /// The synopsis for the command.
        /// </summary>
        private readonly string[] synopsis;

        /// <summary>
        /// The command usage example.
        /// </summary>
        private readonly List<string> usages;

        /// <summary>
        /// The original help for the command.
        /// </summary>
        private string help;

        /// <summary>
        /// Alias for the command.
        /// </summary>
        private string[] aliases = Array.Empty<string>();

        /// <summary>
        /// Is merged definition from application definition.
        /// </summary>
        private bool applicationDefinitionMerged;

        /// <summary>
        /// Is merged definition from application definition with arguments.
        /// </summary>
        private bool applicationDefinitionMergedWithArguments;

        /// <summary>
        /// Whether ignore validation errors for the command.
        /// </summary>
        private bool ignoreValidationErrors;

        /// <summary>
        /// Whether the command environment is ready to be completed.
        /// </summary>
        private bool commandEnvironmentalPreparation;

        /// <summary>
        /// Whether the command is inited.
        /// </summary>
        private bool inited;

        /// <summary>
        /// A code closure that fires when the command is called.
        /// </summary>
        private Func<IInput, IOutput, int> code;

        private InputDefinition definition;
        private string name;
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        public Command()
        {
            definition = new InputDefinition();
            synopsis = new string[2];
            ignoreValidationErrors = false;
            usages = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        public Command(string name)
            : this()
        {
            SetName(name);
        }

        /// <summary>
        /// Detail level of synopsis.
        /// </summary>
        private enum SynopsisDetailLevels
        {
            /// <summary>
            /// Short version of the synopsis (without options).
            /// </summary>
            Short,

            /// <summary>
            /// Full version of the synopsis.
            /// </summary>
            Long,
        }

        /// <summary>
        /// Gets console application.
        /// </summary>
        public Application Application { get; private set; }

        /// <summary>
        /// Gets current command name.
        /// </summary>
        public string Name => GetName();

        /// <summary>
        /// Gets description for the command.
        /// </summary>
        public string Description => GetDescription();

        /// <summary>
        /// Gets help for the command.
        /// </summary>
        public string Help => GetProcessedHelp();

        /// <summary>
        /// Gets a value indicating whether the command should be publicly shown or not.
        /// </summary>
        public virtual bool IsHidden => false;

        /// <summary>
        /// Gets defined command line arguments and options.
        /// </summary>
        public InputDefinition GetDefinition()
        {
            Initialize();
            return definition;
        }

        /// <summary>
        /// Runs the command.
        /// The code to execute defined by overriding the <see cref="Execute"/> method in a sub-class.
        /// </summary>
        /// <param name="input">The std input object.</param>
        /// <param name="output">The std output object.</param>
        /// <returns>The exit code.</returns>
        public virtual int Run(IInput input, IOutput output)
        {
            Initialize();

            BuildEnvironmentalPreparation(input);

            Initialize(input, output);

            if (input.IsInteractive)
            {
                Interact(input, output);
            }

            // The command name argument is often omitted when a command is executed
            // directly with its Run() method. It would fail the validation if we
            // didn't make sure the command argument is present.
            if (input.HasArgument(ArgumentCommand) && input.GetArgument(ArgumentCommand) is null)
            {
                input.SetArgument(ArgumentCommand, Name);
            }

            input.Validate();

            if (code != null)
            {
                return code(input, output);
            }

            return Execute(input, output);
        }

        /// <summary>
        /// Gets aliases for the command.
        /// </summary>
        /// <returns>An array of the aliases.</returns>
        public string[] GetAliases()
        {
            Initialize();
            return aliases;
        }

        /// <summary>
        /// Gets the command usage example.
        /// </summary>
        /// <returns>The command usage example.</returns>
        public string[] GetUsages()
        {
            Initialize();
            return usages.ToArray();
        }

        /// <summary>
        /// Returns the synopsis for the command.
        /// </summary>
        /// <param name="isShort">Whether to show the short version of the synopsis (without options) or not.</param>
        /// <returns>The synopsis for the command.</returns>
        public string GetSynopsis(bool isShort = false)
        {
            var index = (int)(isShort ? SynopsisDetailLevels.Short : SynopsisDetailLevels.Long);

            if (string.IsNullOrEmpty(synopsis[index]))
            {
                synopsis[index] = $"{Name} {GetDefinition().GetSynopsis(isShort)}";
            }

            return synopsis[index];
        }

        /// <summary>
        /// Sets the name of the command.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetName(string name)
        {
            ValidateName(name);
            this.name = name;
            return this;
        }

        /// <summary>
        /// Sets console application of the command.
        /// </summary>
        /// <param name="application">The console application.</param>
        public void SetApplication(Application application)
        {
            Application = application;
        }

        /// <summary>
        /// Sets an array of <see cref="InputArgument"/> and <see cref="InputOption"/> instances or a <see cref="InputDefinition"/> instance.
        /// </summary>
        /// <param name="definitions">An array of argument and option instances or a definition instance.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetDefinition(params IInputDefinition[] definitions)
        {
            if (definitions == null)
            {
                return this;
            }

            if (definitions.Length == 1 && definitions[0] is InputDefinition)
            {
                definition = (InputDefinition)definitions[0];
            }
            else
            {
                definition.SetDefinition(definitions);
            }

            applicationDefinitionMerged = false;
            return this;
        }

        /// <summary>
        /// Sets description for the command.
        /// </summary>
        /// <param name="description">The description for the command.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetDescription(string description)
        {
            this.description = description ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Sets help for the command.
        /// </summary>
        /// <param name="helpMessage">The help for the command.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetHelp(string helpMessage)
        {
            help = helpMessage ?? string.Empty;
            return this;
        }

        /// <summary>
        /// Sets the aliases for the command.
        /// </summary>
        /// <param name="aliasNames">Alias for the command.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetAlias(params string[] aliasNames)
        {
            if (aliasNames == null)
            {
                throw new ArgumentNullException(nameof(aliasNames));
            }

            foreach (var alias in aliasNames)
            {
                ValidateName(alias);
            }

            aliases = aliasNames;
            return this;
        }

        /// <summary>
        /// Adds the command usage.
        /// </summary>
        /// <param name="usage">The command usage.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command AddUsage(string usage)
        {
            if (!string.IsNullOrEmpty(Name)
                && usage.IndexOf(Name, StringComparison.Ordinal) != 0)
            {
                usage = $"{Name} {usage}";
            }

            usages.Add(usage);
            return this;
        }

        /// <summary>
        /// Adds an argument.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="mode">The argument mode.</param>
        /// <param name="description">A description text.</param>
        /// <param name="defaultValue">The default value(<see cref="InputArgumentModes.Optional"/> mode only).</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public virtual Command AddArgument(string name, InputArgumentModes mode = InputArgumentModes.Optional,
            string description = null, Mixture defaultValue = null)
        {
            GetDefinition().AddArguments(new InputArgument(name, mode, description, defaultValue));
            return this;
        }

        /// <summary>
        /// Adds an option.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <param name="shortcut">The shortcuts, can be null, a string of shortcuts delimited by | or an array of shortcuts.</param>
        /// <param name="mode">The option mode.</param>
        /// <param name="description">A description text.</param>
        /// <param name="defaultValue">The default value(must be null for <see cref="InputOptionModes.ValueNone"/>).</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public virtual Command AddOption(string name, string shortcut = null,
            InputOptionModes mode = InputOptionModes.ValueNone,
            string description = null, Mixture defaultValue = null)
        {
            GetDefinition().AddOptions(new InputOption(name, shortcut, mode, description, defaultValue));
            return this;
        }

        /// <summary>
        /// Sets the code closure.
        /// </summary>
        /// <param name="func">The code closure.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetCode(Func<IInput, IOutput, int> func)
        {
            code = func;
            return this;
        }

        /// <summary>
        /// Sets the code closure.
        /// </summary>
        /// <param name="action">The code closure.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetCode(Action action)
        {
            return SetCode((input, output) =>
            {
                action();
                return ExitCodes.Normal;
            });
        }

        /// <summary>
        /// Sets the code closure.
        /// </summary>
        /// <param name="action">The code closure.</param>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command SetCode(Action<IOutput> action)
        {
            return SetCode((input, output) =>
            {
                action(output);
                return ExitCodes.Normal;
            });
        }

        /// <summary>
        /// Ignore validation errors for the command.
        /// </summary>
        /// <returns>Returns the <see cref="Command"/> instance.</returns>
        public Command IgnoreValidationErrors()
        {
            ignoreValidationErrors = true;
            return this;
        }

        /// <summary>
        /// Build a command execution environment.
        /// </summary>
        /// <param name="input">The std input instance.</param>
        internal void BuildEnvironmentalPreparation(IInput input)
        {
            // The commands can be triggered multiple times on
            // different inputs so we have to bind each time.
            if (commandEnvironmentalPreparation)
            {
                goto bind;
            }

            // force the creation of the synopsis before the merge with the gloabl(application) definition
            // We don't want the global options and arguments to be in the synopsis of the command.
            GetSynopsis(true);
            GetSynopsis();

            // add the application arguments and options
            MergeApplicationDefinition();

            commandEnvironmentalPreparation = true;

        // bind the input against the command specific arguments/options
        bind:
            try
            {
                input.Bind(GetDefinition());
            }
            catch (ValidationException) when (ignoreValidationErrors)
            {
                // Ignore the ValidationException
            }
        }

        /// <summary>
        /// Gets the <see cref="InputDefinition"/> to be used to create representations of this Command.
        /// </summary>
        /// <remarks>The <see cref="GetDefinition()"/> changed by merging with the application. So this method return the original command representation.</remarks>
        /// <returns>The original definition.</returns>
        internal virtual InputDefinition GetOriginalDefinition()
        {
            return GetDefinition();
        }

        /// <summary>
        /// Merges the application definition with the command definition.
        /// </summary>
        /// <param name="mergeArguments">Whether to merge or not the Application definition arguments to command definition arguments.</param>
        internal void MergeApplicationDefinition(bool mergeArguments = true)
        {
            if (Application == null)
            {
                return;
            }

            if (!applicationDefinitionMerged)
            {
                GetDefinition().AddOptions(Application.GetDefinition().GetOptions());
                applicationDefinitionMerged = true;
            }

            if (applicationDefinitionMergedWithArguments || !mergeArguments)
            {
                return;
            }

            // We save the current arguments and add them to the global arguments.
            // If the same argument appear, current argument will override global argument.
            var currentArguments = GetDefinition().GetArguments();
            GetDefinition().SetArguments(Application.GetDefinition().GetArguments());
            GetDefinition().AddArguments(currentArguments);

            applicationDefinitionMergedWithArguments = true;
        }

        /// <summary>
        /// Initialize the command.
        /// </summary>
        protected internal void Initialize()
        {
            if (inited)
            {
                return;
            }

            inited = true;
            Configure();
        }

        /// <summary>
        /// Configures the command.
        /// </summary>
        protected virtual void Configure()
        {
        }

        /// <summary>
        /// Returns the processed help for the command.
        /// </summary>
        /// <returns>The processed help.</returns>
        protected virtual string GetProcessedHelp()
        {
            Initialize();

            if (string.IsNullOrEmpty(help))
            {
                return string.Empty;
            }

            var processedHelp = help;
            processedHelp = processedHelp.Replace("{command.name}", Name);
            processedHelp = processedHelp.Replace("{command.class_fullname}", GetType().FullName);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 0)
            {
                var file = Path.GetFileNameWithoutExtension(args[0]);
                var extension = Path.GetExtension(args[0]);

                if (extension == ".dll")
                {
                    file = $"dotnet {file}.dll";
                }

                processedHelp = processedHelp.Replace("{environment.executable_file}", file);
            }
            else
            {
                processedHelp = processedHelp.Replace("{environment.executable_file}", @"\<cli>");
            }

            return processedHelp;
        }

        /// <summary>
        /// <para>Initializes the command after the input has been bound and before the input is validated.</para>
        /// <seealso cref="IInput.Bind"/>
        /// <seealso cref="IInput.Validate"/>
        /// </summary>
        /// <param name="input">The std input object.</param>
        /// <param name="output">The std output object.</param>
        protected virtual void Initialize(IInput input, IOutput output)
        {
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <remarks>
        /// This method is not abstract because you can use this class
        /// as a concrete class. In this case, instead of defining the
        /// <see cref="Execute(IInput, IOutput)"/> method, you set the
        /// code to execute by passing a Closure to the
        /// <see cref="SetCode(Action)"/> method.
        /// </remarks>
        /// <param name="input">The std input object.</param>
        /// <param name="output">The std output object.</param>
        /// <returns>0 if everything went fine, or an error code.</returns>
        protected virtual int Execute(IInput input, IOutput output)
        {
            throw new NotImplementedException(
                $"You must override the {nameof(Execute)}() method in the concrete command class.");
        }

        /// <summary>
        /// Interacts with the user.
        /// This method is executed after the <see cref="Initialize()"/> and before the input is validated.
        /// <para>This means that this is the only place where the command can interactively ask for values of missing required arguments.</para>
        /// <seealso cref="IInput.Validate"/>
        /// </summary>
        /// <param name="input">The std input object.</param>
        /// <param name="output">The std output object.</param>
        protected virtual void Interact(IInput input, IOutput output)
        {
        }

        /// <summary>
        /// Validates a command name.
        /// It must be non-empty and a-z,A-Z,0-9,-.
        /// </summary>
        /// <param name="name">The command name.</param>
        private static void ValidateName(string name)
        {
            var symbol = Regex.Escape(Application.NamespaceSymbol.ToString());
            if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, $"^[a-zA-Z0-9]([a-zA-Z0-9\\-\\{symbol}][a-zA-Z0-9]?)*$"))
            {
                throw new InvalidArgumentException($"Command name {name} is invalid.");
            }
        }

        /// <summary>
        /// Get the command name.
        /// </summary>
        private string GetName()
        {
            Initialize();
            return name;
        }

        /// <summary>
        /// Get the command description.
        /// </summary>
        private string GetDescription()
        {
            Initialize();
            return description;
        }
    }
}
