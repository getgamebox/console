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

using GameBox.Console.EventDispatcher;
using GameBox.Console.Exception;
using GameBox.Console.Tests.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Dispatcher = GameBox.Console.EventDispatcher.EventDispatcher;

namespace GameBox.Console.Tests.EventDispatcher
{
    [TestClass]
    public class TestsEventDispatcher
    {
        private IEventDispatcher dispatcher;

        [TestInitialize]
        public void Initialize()
        {
            dispatcher = new Dispatcher();
        }

        [TestMethod]
        public void TestDispatch()
        {
            var foo = new Mock<EventHandler>();
            dispatcher.AddListener("foo", foo.Object);
            dispatcher.Dispatch("foo", this, new FooEventArgs("foo"));
            dispatcher.Dispatch("foo", this);
            foo.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TestHasListener()
        {
            var foo = new Mock<EventHandler>();
            dispatcher.AddListener("foo", foo.Object);
            Assert.IsTrue(dispatcher.HasListener("foo"));
        }

        [TestMethod]
        public void TestRemoveListener()
        {
            var foo = new Mock<EventHandler>();
            var bar = new Mock<EventHandler>();

            dispatcher.AddListener("foo", foo.Object);
            dispatcher.AddListener("bar", bar.Object);
            dispatcher.RemoveListener("foo", foo.Object);

            var eventArgs = new FooEventArgs("foo");
            dispatcher.Dispatch("foo", this, eventArgs);
            dispatcher.Dispatch("bar", this, eventArgs);

            foo.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Never);
            bar.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
        }

        [TestMethod]
        public void TestRemoveAllListener()
        {
            var fooFirst = new Mock<EventHandler>();
            var fooSecond = new Mock<EventHandler>();
            var bar = new Mock<EventHandler>();
            dispatcher.AddListener("foo", fooFirst.Object);
            dispatcher.AddListener("foo", fooSecond.Object);
            dispatcher.AddListener("bar", bar.Object);

            dispatcher.Dispatch("foo", this);
            dispatcher.Dispatch("bar", this);

            fooFirst.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            fooSecond.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            bar.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);

            dispatcher.RemoveListener("foo");
            dispatcher.Dispatch("foo", this);
            dispatcher.Dispatch("bar", this);

            fooFirst.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            fooSecond.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            bar.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Exactly(2));
        }

        [TestMethod]
        public void TestAddRemoveSubscriber()
        {
            var foo = new Mock<EventHandler>();
            var bar = new Mock<EventHandler>();
            var eventSubscriber = new Mock<IEventSubscriber>();
            eventSubscriber.Setup((o) => o.GetSubscribedEvents())
                .Returns(new Dictionary<string, EventHandler>()
                {
                    { "foo", foo.Object },
                    { "bar", bar.Object },
                });

            dispatcher.AddSubscriber(eventSubscriber.Object);
            dispatcher.Dispatch("foo", this);

            foo.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            bar.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Never);

            dispatcher.RemoveSubscriber(eventSubscriber.Object);
            dispatcher.Dispatch("bar", this);

            foo.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            bar.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Never);
        }

        [TestMethod]
        public void TestPropagationStopped()
        {
            var fooFirst = new Mock<EventHandler>();
            var fooSecond = new Mock<EventHandler>();
            var args = new Mock<EventArgs>();
            args.As<IStoppableEvent>().SetupSequence((o) => o.IsPropagationStopped)
                .Returns(false).Returns(false).Returns(true);

            dispatcher.AddListener("foo", fooFirst.Object);
            dispatcher.AddListener("foo", fooSecond.Object);

            dispatcher.Dispatch("foo", this, args.Object);
            fooFirst.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
            fooSecond.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);

            dispatcher.Dispatch("foo", this, args.Object);
            fooFirst.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Exactly(2));
            fooSecond.Verify((o) => o.Invoke(this, It.IsAny<EventArgs>()), Times.Once);
        }

        [TestMethod]
        [ExpectedExceptionAndMessage(
            typeof(ConsoleLogicException),
            "Circular call to script handler \"foo\" detected. Event stack [foo].")]
        public void TestDispatchCircularCall()
        {
            var listener = new Mock<EventHandler>();
            listener.Setup((o) => o.Invoke(this, It.IsAny<EventArgs>()))
                .Callback(() =>
                {
                    dispatcher.Dispatch("foo", this);
                });

            dispatcher.AddListener("foo", listener.Object);
            dispatcher.Dispatch("foo", this);
        }
    }
}
