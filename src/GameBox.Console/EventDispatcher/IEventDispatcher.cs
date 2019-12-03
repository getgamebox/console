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

namespace GameBox.Console.EventDispatcher
{
    /// <summary>
    /// The <see cref="IEventDispatcher"/> is event listener system.
    /// Listeners are registered on the manager and events are dispatched through the manager.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Checks whether an event has any registered listeners.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>true if the specified event has any listeners, false otherwise.</returns>
        bool HasListener(string eventName);

        /// <summary>
        /// Dispatches an event to all registered listeners.
        /// </summary>
        /// <remarks>
        /// If the event name is null or empty, then no events will be fired and
        /// no exception will be thrown.
        /// </remarks>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="sender">The sender instance.</param>
        /// <param name="eventArgs">The event to pass to the event handlers/listeners.</param>
        void Dispatch(string eventName, object sender, EventArgs eventArgs = null);

        /// <summary>
        /// Adds an event listener that listens on the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The listener.</param>
        void AddListener(string eventName, EventHandler listener);

        /// <summary>
        /// Removes an event listener from the specified event.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="listener">The specified listener to removed. Removed all listener if given null.</param>
        void RemoveListener(string eventName, EventHandler listener = null);

        /// <summary>
        /// Adds object methods as listeners for the events in GetSubscribedEvents.
        /// </summary>
        /// <param name="subscriber">The subscriber instance.</param>
        void AddSubscriber(IEventSubscriber subscriber);

        /// <summary>
        /// Removes object methods as listeners for the events in GetSubscribedEvents.
        /// </summary>
        /// <param name="subscriber">The subscriber instance.</param>
        void RemoveSubscriber(IEventSubscriber subscriber);
    }
}
