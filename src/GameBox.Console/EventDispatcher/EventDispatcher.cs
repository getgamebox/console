/*
 * This file is part of the GameBox package.
 *
 * (c) LiuSiJia <394754029@qq.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://github.com/getgamebox/console
 */

using GameBox.Console.Exception;
using GameBox.Console.Util;
using System;
using System.Collections.Generic;

namespace GameBox.Console.EventDispatcher
{
    /// <summary>
    /// The event dispatcher.
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IDictionary<string, IList<EventHandler>> listeners;
        private readonly Stack<string> eventStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventDispatcher"/> class.
        /// </summary>
        public EventDispatcher()
        {
            listeners = new Dictionary<string, IList<EventHandler>>();
            eventStack = new Stack<string>();
        }

        /// <inheritdoc />
        public virtual bool HasListener(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return false;
            }

            return GetListeners(eventName, EventArgs.Empty).Count > 0;
        }

        /// <inheritdoc />
        public virtual void Dispatch(string eventName, object sender, EventArgs eventArgs = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            eventArgs = eventArgs ?? EventArgs.Empty;
            PushEventStack(eventName);

            try
            {
                foreach (var listener in GetListeners(eventName, eventArgs))
                {
                    listener(sender, eventArgs);

                    if (eventArgs is IStoppableEvent stoppableEvent
                            && stoppableEvent.IsPropagationStopped)
                    {
                        break;
                    }
                }
            }
            finally
            {
                PopEventStack(eventName);
            }
        }

        /// <inheritdoc />
        public virtual void AddListener(string eventName, EventHandler listener)
        {
            if (string.IsNullOrEmpty(eventName) || listener == null)
            {
                return;
            }

            if (!listeners.TryGetValue(eventName, out IList<EventHandler> handlers))
            {
                listeners[eventName] = handlers = new List<EventHandler>();
            }

            handlers.Add(listener);
        }

        /// <inheritdoc />
        public virtual void RemoveListener(string eventName, EventHandler listener = null)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                return;
            }

            if (listener == null)
            {
                listeners.Remove(eventName);
                return;
            }

            if (listeners.TryGetValue(eventName, out IList<EventHandler> handlers))
            {
                handlers.Remove(listener);

                if (handlers.Count <= 0)
                {
                    listeners.Remove(eventName);
                }
            }
        }

        /// <inheritdoc />
        public virtual void AddSubscriber(IEventSubscriber subscriber)
        {
            foreach (var subscribed in subscriber.GetSubscribedEvents())
            {
                AddListener(subscribed.Key, subscribed.Value);
            }
        }

        /// <inheritdoc />
        public virtual void RemoveSubscriber(IEventSubscriber subscriber)
        {
            foreach (var subscribed in subscriber.GetSubscribedEvents())
            {
                RemoveListener(subscribed.Key, subscribed.Value);
            }
        }

        /// <summary>
        /// Retrieves all listeners for a given event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="eventArgs">The event args instance.</param>
        /// <returns>An list of the event handler instance.</returns>
        protected virtual IList<EventHandler> GetListeners(string eventName, EventArgs eventArgs)
        {
            if (listeners.TryGetValue(eventName, out IList<EventHandler> handlers))
            {
                return handlers;
            }

            return Array.Empty<EventHandler>();
        }

        /// <summary>
        /// Push into the event stack to avoid relying on loop calls.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        protected virtual void PushEventStack(string eventName)
        {
            if (eventStack.Contains(eventName))
            {
                throw new ConsoleLogicException($"Circular call to script handler \"{eventName}\" detected. {GetEventStackDebugMessage()}");
            }

            eventStack.Push(eventName);
        }

        /// <summary>
        /// Pops the active event from the stack.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        protected virtual void PopEventStack(string eventName)
        {
            var popEventName = eventStack.Pop();
            Guard.Requires<ConsoleLogicException>(
                popEventName == eventName,
                $"The name of the event that pops up is inconsistent with the name of the event that pops up ({popEventName} != {eventName}), and the event stack is confused.");
        }

        /// <summary>
        /// Gets the debug message of the event stack.
        /// </summary>
        /// <returns>Returns the debug message.</returns>
        protected virtual string GetEventStackDebugMessage()
        {
            var previous = string.Join(", ", eventStack.ToArray());
            return $"Event stack [{previous}].";
        }
    }
}
