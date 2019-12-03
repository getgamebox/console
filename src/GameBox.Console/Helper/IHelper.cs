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

namespace GameBox.Console.Helper
{
    /// <summary>
    /// <see cref="IHelper"/> is the interface all helpers must implement.
    /// </summary>
    public interface IHelper
    {
        /// <summary>
        /// Gets a value indicate the canonical name of this helper.
        /// </summary>
        string Name { get; }
    }
}
