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

namespace GameBox.Console.Util
{
    /// <summary>
    /// <see cref="Mixture"/> extension method.
    /// </summary>
    public static class MixtureExtend
    {
        /// <summary>
        /// Find the first <see cref="Mixture"/> object whose name matches in the Mixture array.
        /// </summary>
        /// <param name="collection">The mixture array.</param>
        /// <param name="name">The specified name.</param>
        /// <returns>The mixture.</returns>
        public static Mixture Get(this Mixture[] collection, string name)
        {
            return collection.Get(name, Mixture.Null);
        }

        /// <summary>
        /// Find the first <see cref="Mixture"/> object whose name matches in the Mixture array.
        /// </summary>
        /// <param name="collection">The mixture array.</param>
        /// <param name="name">The specified name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The mixture.</returns>
        public static Mixture Get(this Mixture[] collection, string name, Mixture defaultValue)
        {
            if (collection == null)
            {
                return defaultValue;
            }

            foreach (var item in collection)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Whether the specified <see cref="Mixture"/> object is exists.
        /// </summary>
        /// <param name="collection">The mixture array.</param>
        /// <param name="name">The specified name.</param>
        /// <returns>True if the specified object is exists.</returns>
        public static bool Has(this Mixture[] collection, string name)
        {
            if (collection == null)
            {
                return false;
            }

            foreach (var item in collection)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Test the specified <see cref="Mixture"/> object is exists. and returns it.
        /// </summary>
        /// <param name="collection">The mixture array.</param>
        /// <param name="name">The specified name.</param>
        /// <param name="result">The returns result.</param>
        /// <returns>True if the specified object is exists.</returns>
        public static bool TryGet(this Mixture[] collection, string name, out Mixture result)
        {
            result = Mixture.Null;
            if (collection == null)
            {
                return false;
            }

            foreach (var item in collection)
            {
                if (item.Name != name)
                {
                    continue;
                }

                result = item;
                return true;
            }

            return false;
        }
    }
}
