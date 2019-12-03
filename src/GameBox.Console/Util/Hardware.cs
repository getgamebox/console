/*
 * This file is part of the GameBox package , borrowed from CatLib(catlib.io) and modified.
 *
 * (c) MouGuangYi <muguangyi@hotmail.com> , Yu Meng Han <menghanyu1994@gmail.com>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://catlib.io
 */

namespace GameBox.Console.Util
{
    /// <summary>
    /// Expressed program hardware info.
    /// </summary>
    public static class Hardware
    {
        /// <summary>
        /// Get the program memory usage.
        /// </summary>
        /// <returns>The memory usage.</returns>
        public static long GetMemoryUsage()
        {
            // todo: optiminzation
            var p = System.Diagnostics.Process.GetCurrentProcess();
            return p.WorkingSet64;
        }
    }
}
