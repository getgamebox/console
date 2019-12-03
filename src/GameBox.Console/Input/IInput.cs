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

using GameBox.Console.Util;
using System.Text;

namespace GameBox.Console.Input
{
    /// <summary>
    /// <see cref="IInput"/> is the interface implemented by all input classes.
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// Gets or sets the input encoding.
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Gets a value indicating whether the input is interactive.
        /// </summary>
        /// <returns>true if the input means interactive, false otherwise.</returns>
        bool IsInteractive { get; }

        /// <summary>
        /// Binds the current Input instance with the given arguments and options definition.
        /// </summary>
        /// <param name="definition"><see cref="InputDefinition"/> represents a set of valid command line arguments and options.</param>
        void Bind(InputDefinition definition);

        /// <summary>
        /// Validates the input meets the definition(<see cref="InputDefinition"/>).
        /// </summary>
        void Validate();

        /// <summary>
        /// Returns true if the raw option (not parsed) contain a name.
        /// <para>You should not use this api in high-level code. because it has not been verified.</para>
        /// <para>This interface is only suitable for use before the framework performs validation operations.</para>
        /// </summary>
        /// <param name="name">The name to look for in the raw option.</param>
        /// <param name="ignoreAdditional">Only check real option(Do not check the option passed to the script), skip those following an end of options (--) signal.</param>
        /// <returns>true if the value is contained in the raw option.</returns>
        bool HasRawOption(string name, bool ignoreAdditional = false);

        /// <summary>
        /// Returns the value of a raw option (not parsed).
        /// <para>You should not use this api in high-level code. because it has not been verified.</para>
        /// <para>This interface is only suitable for use before the framework performs validation operations.</para>
        /// </summary>
        /// <param name="name">The name to look for in the raw option.</param>
        /// <param name="defaultValue">The default value to return if no result is found.</param>
        /// <param name="ignoreAdditional">Only check real option(Do not check the option passed to the script), skip those following an end of options (--) signal.</param>
        /// <returns>The argument or option value.</returns>
        Mixture GetRawOption(string name, Mixture defaultValue = null, bool ignoreAdditional = false);

        /// <summary>
        /// Returns the first command line argument.
        /// <para>You should not use this api in high-level code. because it has not been verified.</para>
        /// <para>This interface is only suitable for use before the framework performs validation operations.</para>
        /// </summary>
        /// <returns>The first argument.</returns>
        Mixture GetFirstArgument();

        /// <summary>
        /// Returns true if an <see cref="InputArgument"/> object exists by name.
        /// </summary>
        /// <param name="name">The Input Argument name.</param>
        /// <returns>true if the <see cref="InputArgument"/> object exists, false otherwise.</returns>
        bool HasArgument(string name);

        /// <summary>
        /// Returns the argument value for a given argument name.
        /// return null if the argument value not exists.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <returns>The argument value.</returns>
        Mixture GetArgument(string name);

        /// <summary>
        /// Sets an argument value by name.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The argument value.</param>
        void SetArgument(string name, Mixture value);

        /// <summary>
        /// Returns true if an <see cref="InputOption"/> object exists by name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>true if the <see cref="InputOption"/> object exists, false otherwise.</returns>
        bool HasOption(string name);

        /// <summary>
        /// Returns the option value for a given option name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <returns>The option value.</returns>
        Mixture GetOption(string name);

        /// <summary>
        /// Sets an option value by name.
        /// </summary>
        /// <param name="name">The option name.</param>
        /// <param name="value">The option value.</param>
        void SetOption(string name, Mixture value);

        /// <summary>
        /// Sets the input interactivity.
        /// </summary>
        /// <param name="interactive">If the input should be interactive.</param>
        void SetInteractive(bool interactive);
    }
}
