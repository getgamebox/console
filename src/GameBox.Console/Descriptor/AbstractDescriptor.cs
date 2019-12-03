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

namespace GameBox.Console.Descriptor
{
    /// <summary>
    /// <see cref="AbstractDescriptor"/> is the base class for all descriptor classes.
    /// </summary>
    public abstract class AbstractDescriptor : IDescriptor
    {
        /// <summary>
        /// The raw text option key.
        /// </summary>
        internal const string RawText = "raw_text";

        /// <summary>
        /// The raw ouput option key.
        /// </summary>
        internal const string RawOutput = "raw_ouput";

        /// <summary>
        /// The namespace option key.
        /// </summary>
        internal const string Namespace = "namespace";

        /// <summary>
        /// Gets the std output instance.
        /// </summary>
        protected IOutput Output { get; private set; }

        /// <summary>
        /// Describes an object if supported.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="content">The described object.</param>
        /// <param name="options">The described options.</param>
        public void Describe(IOutput output, object content, params Mixture[] options)
        {
            Output = output;
#pragma warning disable S3247
            if (content is Command.Command)
            {
                DescribeCommand((Command.Command)content, options);
            }
            else if (content is Application)
            {
                DescribeApplication((Application)content, options);
            }
            else if (content is InputArgument)
            {
                DescribeInputArgument((InputArgument)content, options);
            }
            else if (content is InputOption)
            {
                DescribeInputOption((InputOption)content, options);
            }
            else if (content is InputDefinition)
            {
                DescribeInputDefinition((InputDefinition)content, options);
            }
            else
            {
                throw new InvalidArgumentException($"Object of type {content.GetType()} is not describable.");
            }
#pragma warning restore S3247
        }

        /// <summary>
        /// Writes message to output.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="decorated">True if the <see cref="OutputOptions.OutputNormal"/>, otherwise <see cref="OutputOptions.OutputRaw"/>.</param>
        protected void Write(string message, bool decorated = false)
        {
            Output.Write(message, false, decorated ? OutputOptions.OutputNormal : OutputOptions.OutputRaw);
        }

        /// <summary>
        /// Describes a <see cref="InputDefinition"/> instance.
        /// </summary>
        /// <param name="definition">The definition instance.</param>
        /// <param name="options">The described options.</param>
        protected abstract void DescribeInputDefinition(InputDefinition definition, params Mixture[] options);

        /// <summary>
        /// Describes a <see cref="InputOption"/> instance.
        /// </summary>
        /// <param name="inputOption">The option instance.</param>
        /// <param name="options">The described options.</param>
        protected abstract void DescribeInputOption(InputOption inputOption, params Mixture[] options);

        /// <summary>
        /// Describes a <see cref="InputArgument"/> instance.
        /// </summary>
        /// <param name="argument">The argument instance.</param>
        /// <param name="options">The described options.</param>
        protected abstract void DescribeInputArgument(InputArgument argument, params Mixture[] options);

        /// <summary>
        /// Describes a <see cref="Command.Command"/> instance.
        /// </summary>
        /// <param name="command">The command instance.</param>
        /// <param name="options">The described options.</param>
        protected abstract void DescribeCommand(Command.Command command, params Mixture[] options);

        /// <summary>
        /// Describes a <see cref="Application"/> instance.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="options">The described options.</param>
        protected abstract void DescribeApplication(Application application, params Mixture[] options);
    }
}
