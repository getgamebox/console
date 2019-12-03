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

using System;
using System.Collections.Generic;

namespace GameBox.Console.EventDispatcher
{
    /// <summary>
    /// Represents an event subscriber.
    /// </summary>
    public interface IEventSubscriber
    {
        /// <summary>
        /// Gets an mapping of event names this subscriber wants to listen to.
        /// </summary>
        /// <returns>Returns an mapping of event names this subscriber wants to listen to.</returns>
        IDictionary<string, EventHandler> GetSubscribedEvents();
    }
}
