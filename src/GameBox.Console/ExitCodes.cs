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

namespace GameBox.Console
{
    /// <summary>
    /// Exit code.
    /// </summary>
    public static class ExitCodes
    {
        /// <summary>
        /// The normal exit.
        /// </summary>
        public const int Normal = 0;

        /// <summary>
        /// The unknow(general) exception exit.
        /// </summary>
        public const int GeneralException = 1;

        /// <summary>
        /// The return code for skipped commands, this will also be passed into the terminate event.
        /// </summary>
        public const int SkipCommnad = 113;
    }
}
