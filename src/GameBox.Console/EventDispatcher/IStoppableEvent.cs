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

namespace GameBox.Console.EventDispatcher
{
    /// <summary>
    /// An event whose processing may be interrupted when the event has been handled.
    /// </summary>
    public interface IStoppableEvent
    {
        /// <summary>
        /// Gets a value indicating whether propagation stopped.
        /// </summary>
        bool IsPropagationStopped { get; }
    }
}
