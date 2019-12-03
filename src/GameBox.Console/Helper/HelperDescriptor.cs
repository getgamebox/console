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
using GameBox.Console.Exception;
using GameBox.Console.Output;
using GameBox.Console.Util;
using System.Collections.Generic;

namespace GameBox.Console.Helper
{
    /// <summary>
    /// This class adds helper method to describe objects in various formats.
    /// </summary>
    public class HelperDescriptor : AbstractHelper
    {
        /// <summary>
        /// The descriptors mapping.
        /// </summary>
        private readonly Dictionary<string, IDescriptor> descriptors;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelperDescriptor"/> class.
        /// </summary>
        public HelperDescriptor()
        {
            descriptors = new Dictionary<string, IDescriptor>
            {
                { "txt", new DescriptorText() },
                { "json", new DescriptorJson() },
            };
        }

        /// <inheritdoc />
        public override string Name => "descriptor";

        /// <summary>
        /// Sets the format option.
        /// </summary>
        /// <param name="format">The format type.</param>
        /// <returns>The described option.</returns>
        public static Mixture OptionFormat(string format)
        {
            return new Mixture(format ?? string.Empty) { Name = "format" };
        }

        /// <summary>
        /// Sets the raw text option.
        /// </summary>
        /// <param name="isRaw">Whether the raw text.</param>
        /// <returns>The described option.</returns>
        public static Mixture OptionRawText(bool isRaw)
        {
            return new Mixture(isRaw) { Name = AbstractDescriptor.RawText };
        }

        /// <summary>
        /// Sets the raw output option.
        /// </summary>
        /// <param name="isRaw">Whether the raw output.</param>
        /// <returns>The described option.</returns>
        public static Mixture OptionRawOutput(bool isRaw)
        {
            return new Mixture(isRaw) { Name = AbstractDescriptor.RawOutput };
        }

        /// <summary>
        /// Sets the namespace option.
        /// </summary>
        /// <param name="namespace">The namespace option.</param>
        /// <returns>The described option.</returns>
        public static Mixture Namespace(string @namespace)
        {
            return new Mixture(@namespace ?? string.Empty) { Name = AbstractDescriptor.Namespace };
        }

        /// <summary>
        /// Describes an object if supported.
        /// </summary>
        /// <param name="output">The std output instance.</param>
        /// <param name="content">The described object.</param>
        /// <param name="options">The described option.</param>
        public void Describe(IOutput output, object content, params Mixture[] options)
        {
            var format = options.Get("format", "txt");
            if (!descriptors.TryGetValue(format, out IDescriptor descriptor))
            {
                throw new InvalidArgumentException($"Unsupported format {format}");
            }

            descriptor.Describe(output, content, options);
        }

        /// <summary>
        /// Registers a descriptor. this method can override existing decorators.
        /// </summary>
        /// <param name="format">The format type.</param>
        /// <param name="descriptor">The descriptor instance.</param>
        /// <returns>The <see cref="HelperDescriptor"/> instance.</returns>
        public HelperDescriptor Register(string format, IDescriptor descriptor)
        {
            descriptors[format] = descriptor;
            return this;
        }
    }
}
