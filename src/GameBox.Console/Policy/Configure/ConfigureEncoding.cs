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

using GameBox.Console.Input;
using GameBox.Console.Output;

namespace GameBox.Console.Policy.Configure
{
    /// <summary>
    /// The encoding configure policy.
    /// </summary>
    internal sealed class ConfigureEncoding : IConfigurePolicy
    {
        /// <summary>
        /// The console application instance.
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureEncoding"/> class.
        /// </summary>
        /// <param name="application">The console application.</param>
        public ConfigureEncoding(Application application)
        {
            this.application = application;
        }

        /// <inheritdoc />
        public void Execute(IInput input, IOutput output)
        {
            input.Encoding = application.Encoding;
            output.Encoding = application.Encoding;
        }
    }
}
