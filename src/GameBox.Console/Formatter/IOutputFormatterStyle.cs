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

namespace GameBox.Console.Formatter
{
    /// <summary>
    /// Formatter style interface for defining styles.
    /// </summary>
    public interface IOutputFormatterStyle
    {
        /// <summary>
        /// Sets style foreground color name.
        /// </summary>
        /// <param name="color">The foreground color name.</param>
        void SetForeground(string color = null);

        /// <summary>
        /// Sets style background color name.
        /// </summary>
        /// <param name="color">The background color name.</param>
        void SetBackground(string color = null);

        /// <summary>
        /// Apply some specific style option.
        /// </summary>
        /// <param name="effect">The specific style option effect.</param>
        void SetOption(string effect);

        /// <summary>
        /// Cancel some specific style option.
        /// </summary>
        /// <param name="effect">The specific style option effect.</param>
        void UnsetOption(string effect);

        /// <summary>
        /// Format the style to a given text.
        /// </summary>
        /// <param name="text">The given text.</param>
        /// <returns>Formatted text.</returns>
        string Format(string text);
    }
}
