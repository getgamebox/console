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
using System;

namespace GameBox.Console.Tests.Fixtures
{
    internal class FooEventArgs : EventArgs, IStoppableEvent
    {
        public FooEventArgs(string name)
        {
            Name = name;
        }

        public FooEventArgs()
        {
        }

        public string Name { get; set; }

        public bool IsPropagationStopped { get; private set; } = false;

        public void StopPropagation()
        {
            IsPropagationStopped = true;
        }
    }
}
