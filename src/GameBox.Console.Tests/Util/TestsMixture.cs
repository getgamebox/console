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

#pragma warning disable S3415
#pragma warning disable S1854

using GameBox.Console.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GameBox.Console.Tests.Util
{
    [TestClass]
    public class TestsMixture
    {
        [TestMethod]
        public void TestIntTypeConvertToBasicType()
        {
            Mixture mixture = new Mixture();
            mixture = 100;

            int intValue = mixture;
            Assert.AreEqual(intValue, 100);

            string strValue = mixture;
            Assert.AreEqual(strValue, "100");

            bool boolValue = mixture;
            Assert.AreEqual(boolValue, true);

            float floatValue = mixture;
            Assert.AreEqual(floatValue, 100.0f);

            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 100.0d);

            char charValue = mixture;
            Assert.AreEqual(charValue, 'd');
        }

        [TestMethod]
        public void TestFloatTypeConvertToBasicType()
        {
            Mixture mixture = new Mixture();
            mixture = 100.0f;

            int intValue = mixture;
            Assert.AreEqual(intValue, 100);

            string strValue = mixture;
            Assert.AreEqual(strValue, "100");

            bool boolValue = mixture;
            Assert.AreEqual(boolValue, true);

            float floatValue = mixture;
            Assert.AreEqual(floatValue, 100.0f);

            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 100.0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestFloatTypeConvertToCharType()
        {
            Mixture mixture = new Mixture();
            mixture = 100.0f;
            char charValue = mixture;
            Assert.AreEqual(charValue, 'd');
        }

        [TestMethod]
        public void TestDoubleTypeConvertToBasicType()
        {
            Mixture mixture = new Mixture();
            mixture = 100.0d;

            int intValue = mixture;
            Assert.AreEqual(intValue, 100);

            string strValue = mixture;
            Assert.AreEqual(strValue, "100");

            bool boolValue = mixture;
            Assert.AreEqual(boolValue, true);

            float floatValue = mixture;
            Assert.AreEqual(floatValue, 100.0f);

            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 100.0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestDoubleTypeConvertToCharType()
        {
            Mixture mixture = new Mixture();
            mixture = 100.0d;
            char charValue = mixture;
            Assert.AreEqual(charValue, 'd');
        }

        [TestMethod]
        public void TestBoolTypeConvertToBasicType()
        {
            Mixture mixture = new Mixture();
            mixture = true;

            int intValue = mixture;
            Assert.AreEqual(intValue, 1);

            string strValue = mixture;
            Assert.AreEqual(strValue, "True");

            bool boolValue = mixture;
            Assert.AreEqual(boolValue, true);

            float floatValue = mixture;
            Assert.AreEqual(floatValue, 1.0f);

            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 1.0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestBoolTypeConvertToCharType()
        {
            Mixture mixture = new Mixture();
            mixture = true;
            char charValue = mixture;
            Assert.AreEqual(charValue, 'd');
        }

        [TestMethod]
        public void TestStringTypeConvertToBasicType()
        {
            Mixture mixture = new Mixture();
            mixture = "1";

            int intValue = mixture;
            Assert.AreEqual(intValue, 1);

            string strValue = mixture;
            Assert.AreEqual(strValue, "1");

            float floatValue = mixture;
            Assert.AreEqual(floatValue, 1.0f);

            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 1.0d);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestStringTypeConvertToBoolType()
        {
            Mixture mixture = new Mixture();
            mixture = "1";
            bool boolValue = mixture;
            Assert.AreEqual(boolValue, true);
        }

        [TestMethod]
        public void TestCharTypeConvertToBasicType()
        {
            Mixture mixture = new Mixture();
            mixture = 'a';

            int intValue = mixture;
            Assert.AreEqual(intValue, 97);

            string strValue = mixture;
            Assert.AreEqual(strValue, "a");

            char charValue = mixture;
            Assert.AreEqual(charValue, 'a');
        }

        [TestMethod]
        public void TestIntTypeConvertToArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = 100;

            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "100");

            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 100);

            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 100.0f);

            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 100.0d);

            char[] arrCharValue = mixture;
            Assert.AreEqual(arrCharValue.Length, 1);
            Assert.AreEqual(arrCharValue[0], 'd');

            bool[] arrBoolValue = mixture;
            Assert.AreEqual(arrBoolValue.Length, 1);
            Assert.AreEqual(arrBoolValue[0], true);
        }

        [TestMethod]
        public void TestFloatTypeConvertToArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = 100.0f;

            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "100");

            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 100);

            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 100.0f);

            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 100.0d);

            bool[] arrBoolValue = mixture;
            Assert.AreEqual(arrBoolValue.Length, 1);
            Assert.AreEqual(arrBoolValue[0], true);
        }

        [TestMethod]
        public void TestDoubleTypeConvertToArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = 100.0d;

            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "100");

            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 100);

            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 100.0f);

            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 100.0d);

            bool[] arrBoolValue = mixture;
            Assert.AreEqual(arrBoolValue.Length, 1);
            Assert.AreEqual(arrBoolValue[0], true);
        }

        [TestMethod]
        public void TestBoolTypeConvertToArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = true;

            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "True");

            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 1);

            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 1.0f);

            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 1.0d);

            bool[] arrBoolValue = mixture;
            Assert.AreEqual(arrBoolValue.Length, 1);
            Assert.AreEqual(arrBoolValue[0], true);
        }

        [TestMethod]
        public void TestCharTypeConvertToArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = 'a';
            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 97);

            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "a");
        }

        [TestMethod]
        public void TestStringTypeConvertToArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = "1";
            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 1);

            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 1.0f);

            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 1.0d);

            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "1");

            char[] arrCharValue = mixture;
            Assert.AreEqual(arrCharValue.Length, 1);
            Assert.AreEqual(arrCharValue[0], '1');
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestArrTypeConvertToIntType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { "100" };
            int value = mixture;
            Assert.AreEqual(value, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestArrTypeConvertToFloatType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { "100" };
            float value = mixture;
            Assert.AreEqual(value, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestArrTypeConvertToDoubleType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { "100" };
            double value = mixture;
            Assert.AreEqual(value, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestArrTypeConvertToCharType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { "100" };
            char value = mixture;
            Assert.AreEqual(value, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestArrTypeConvertToBoolType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { "100" };
            bool value = mixture;
            Assert.AreEqual(value, 100);
        }

        [TestMethod]
        public void TestDefaultValue()
        {
            Mixture mixture = new Mixture();
            int intValue = mixture;
            float floatValue = mixture;
            double doubleValue = mixture;
            bool boolValue = mixture;
            char charValue = mixture;
            string[] strValue = mixture;
            bool[] arrBoolValue = mixture;
            Assert.AreEqual(intValue, 0);
            Assert.AreEqual(boolValue, false);
            Assert.AreEqual(floatValue, 0.0f);
            Assert.AreEqual(doubleValue, 0.0d);
            Assert.AreEqual(charValue, 0.0d);
            Assert.AreEqual(strValue, null);
            Assert.AreEqual(arrBoolValue, null);
        }

        [TestMethod]
        public void TestDefaultValueEx()
        {
            Mixture mixture = null;
            int intValue = mixture;
            float floatValue = mixture;
            double doubleValue = mixture;
            bool boolValue = mixture;
            char charValue = mixture;
            string[] strValue = mixture;
            bool[] arrBoolValue = mixture;
            Assert.AreEqual(intValue, 0);
            Assert.AreEqual(boolValue, false);
            Assert.AreEqual(floatValue, 0.0f);
            Assert.AreEqual(doubleValue, 0.0d);
            Assert.AreEqual(charValue, 0.0d);
            Assert.AreEqual(strValue, null);
            Assert.AreEqual(arrBoolValue, null);
        }

        [TestMethod]
        public void TestConstructor()
        {
            Mixture mixture = new Mixture(100);
            int intValue = mixture;
            Assert.AreEqual(intValue, 100);

            mixture = new Mixture(100.0f);
            float floatValue = mixture;
            Assert.AreEqual(floatValue, 100.0f);

            mixture = new Mixture(100.0d);
            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 100.0d);

            mixture = new Mixture('a');
            char charValue = mixture;
            Assert.AreEqual(charValue, 'a');

            mixture = new Mixture(true);
            bool boolValue = mixture;
            Assert.AreEqual(boolValue, true);

            mixture = new Mixture("100");
            string strValue = mixture;
            Assert.AreEqual(strValue, "100");

            mixture = new Mixture(new[] { 1 });
            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 1);
            Assert.AreEqual(arrIntValue[0], 1);

            mixture = new Mixture(new[] { true, false });
            bool[] arrBoolValue = mixture;
            Assert.AreEqual(arrBoolValue.Length, 2);
            Assert.AreEqual(arrBoolValue[0], true);
            Assert.AreEqual(arrBoolValue[1], false);

            mixture = new Mixture(new[] { 1.0f });
            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 1.0f);

            mixture = new Mixture(new[] { 1.0d });
            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 1.0d);

            mixture = new Mixture(new[] { 'a' });
            char[] arrCharValue = mixture;
            Assert.AreEqual(arrCharValue.Length, 1);
            Assert.AreEqual(arrCharValue[0], 'a');

            mixture = new Mixture(new[] { "aaa" });
            string[] arrStringValue = mixture;
            Assert.AreEqual(arrStringValue.Length, 1);
            Assert.AreEqual(arrStringValue[0], "aaa");
        }

        [TestMethod]
        public void TestArrImplicit()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { true, false };
            bool[] arrBoolValue = mixture;
            Assert.AreEqual(arrBoolValue.Length, 2);
            Assert.AreEqual(arrBoolValue[0], true);
            Assert.AreEqual(arrBoolValue[1], false);

            mixture = new[] { 1, 2, 3 };
            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 3);
            Assert.AreEqual(arrIntValue[0], 1);
            Assert.AreEqual(arrIntValue[1], 2);
            Assert.AreEqual(arrIntValue[2], 3);

            mixture = new[] { 1.0f };
            float[] arrFloatValue = mixture;
            Assert.AreEqual(arrFloatValue.Length, 1);
            Assert.AreEqual(arrFloatValue[0], 1.0f);

            mixture = new[] { 1.0d };
            double[] arrDoubleValue = mixture;
            Assert.AreEqual(arrDoubleValue.Length, 1);
            Assert.AreEqual(arrDoubleValue[0], 1.0f);

            mixture = new[] { 'a' };
            char[] arrCharValue = mixture;
            Assert.AreEqual(arrCharValue.Length, 1);
            Assert.AreEqual(arrCharValue[0], 'a');
        }

        [TestMethod]
        public void TestEqual()
        {
            Mixture m1 = new Mixture('a');
            Mixture m2 = new Mixture('a');
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(1);
            m2 = new Mixture(1);
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(1.0f);
            m2 = new Mixture(1.0f);
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(1.0d);
            m2 = new Mixture(1.0d);
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(true);
            m2 = new Mixture(true);
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture("aaa");
            m2 = new Mixture("aaa");
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { 1 });
            m2 = new Mixture(new[] { 1 });
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { 1 });
            m2 = new Mixture(new[] { 2 });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { 1.0f });
            m2 = new Mixture(new[] { 1.0f });
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { 1.0f });
            m2 = new Mixture(new[] { 2.0f });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { 1.0d });
            m2 = new Mixture(new[] { 1.0d });
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { 1.0d });
            m2 = new Mixture(new[] { 2.0d });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { 'a', 'b' });
            m2 = new Mixture(new[] { 'a', 'b' });
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { 'a', 'b' });
            m2 = new Mixture(new[] { 'a', 'c' });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { true, false, true });
            m2 = new Mixture(new[] { true, false, true });
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { true, false, true });
            m2 = new Mixture(new[] { false, false, true });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { "aa", "bb", "cc" });
            m2 = new Mixture(new[] { "aa", "bb", "cc" });
            Assert.AreEqual(m1 == m2, true);

            m1 = new Mixture(new[] { "aa", "bb", "cc" });
            m2 = new Mixture(new[] { "a", "b", "c" });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { 'a', 'b' });
            m2 = new Mixture(new[] { "aa", "bb", "cc" });
            Assert.AreEqual(m1 == m2, false);

            m1 = new Mixture(new[] { 1 });
            m2 = new Mixture(new[] { 1, 2, 3 });
            Assert.AreEqual(m1 == m2, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestToBoolArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { 1, 0 };
            bool[] arrValue = mixture;
            Assert.AreEqual(arrValue.Length, 2);
            Assert.AreEqual(arrValue[0], true);
            Assert.AreEqual(arrValue[0], false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestToStringArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { 1, 0 };
            string[] arrValue = mixture;
            Assert.AreEqual(arrValue.Length, 2);
            Assert.AreEqual(arrValue[0], "1");
            Assert.AreEqual(arrValue[0], "0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestToFloatArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { 1, 0 };
            float[] arrValue = mixture;
            Assert.AreEqual(arrValue.Length, 2);
            Assert.AreEqual(arrValue[0], 1.0f);
            Assert.AreEqual(arrValue[0], 0.0f);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestToDoubleArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { 1, 0 };
            double[] arrValue = mixture;
            Assert.AreEqual(arrValue.Length, 2);
            Assert.AreEqual(arrValue[0], 1.0d);
            Assert.AreEqual(arrValue[0], 0.0d);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestToCharArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { 1, 0 };
            char[] arrValue = mixture;
            Assert.AreEqual(arrValue.Length, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestToIntArrayType()
        {
            Mixture mixture = new Mixture();
            mixture = new[] { 1.0f, 2.0f };
            int[] arrValue = mixture;
            Assert.AreEqual(arrValue.Length, 2);
        }

        [TestMethod]
        public void TestStringTypeWithGC()
        {
            var uselessArr1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var uselessArr2 = new[] { "1", "11", "13", "14", "15", "16", "12", "2" };
            var uselessArr3 = new string[1000000];

            Mixture mixture = new Mixture();
            mixture = "1";

            uselessArr1 = null;
            uselessArr2 = null;
            uselessArr3 = null;
#pragma warning disable S1215
            GC.Collect();
#pragma warning restore S1215

            int intValue = mixture;
            Assert.AreEqual(intValue, 1);

            string strValue = mixture;
            Assert.AreEqual(strValue, "1");

            float floatValue = mixture;
            Assert.AreEqual(floatValue, 1.0f);

            double doubleValue = mixture;
            Assert.AreEqual(doubleValue, 1.0d);

            string[] strArrayValue = mixture;
            Assert.AreEqual(strArrayValue.Length, 1);
            Assert.AreEqual(strArrayValue[0], "1");
        }

        [TestMethod]
        public void TestArrayTypeWithGC()
        {
            var uselessArr1 = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var uselessArr2 = new[] { "1", "11", "13", "14", "15", "16", "12", "2" };
            var uselessArr3 = new[] { 1.0f, 2.0f };
            var uselessArr4 = new char[1000000];

            Mixture mixture = new Mixture();
            mixture = new[] { 1, 2, 3 };

            uselessArr1 = null;
            uselessArr2 = null;
            uselessArr3 = null;
            uselessArr4 = null;
#pragma warning disable S1215
            GC.Collect();
#pragma warning restore S1215

            int[] arrIntValue = mixture;
            Assert.AreEqual(arrIntValue.Length, 3);
            Assert.AreEqual(arrIntValue[0], 1);
            Assert.AreEqual(arrIntValue[1], 2);
            Assert.AreEqual(arrIntValue[2], 3);
        }

        [TestMethod]
        public void TestCompareInt()
        {
            Mixture m = new Mixture();
            m = 100;
            var result = m == 100;
            Assert.AreEqual(true, result);
            result = m == 99;
            Assert.AreEqual(false, result);
            result = m != 100;
            Assert.AreEqual(false, result);
            result = m != 99;
            Assert.AreEqual(true, result);
            result = m == 100.0f;
            Assert.AreEqual(true, result);
            result = m == 100.0d;
            Assert.AreEqual(true, result);
            result = m == 'd';
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestCompareFloat()
        {
            Mixture m = new Mixture();
            m = 100.0f;
            var result = m == 100.0f;
            Assert.AreEqual(true, result);
            result = m == 99.0f;
            Assert.AreEqual(false, result);
            result = m != 100.0f;
            Assert.AreEqual(false, result);
            result = m != 99.0f;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(true, result);
            result = m == 100.0d;
            Assert.AreEqual(true, result);
            result = m == 'd';
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestCompareDouble()
        {
            Mixture m = new Mixture();
            m = 100.0d;
            var result = m == 100.0d;
            Assert.AreEqual(true, result);
            result = m == 99.0d;
            Assert.AreEqual(false, result);
            result = m != 100.0d;
            Assert.AreEqual(false, result);
            result = m != 99.0d;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(true, result);
            result = m == 100.0f;
            Assert.AreEqual(true, result);
            result = m == 'd';
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestCompareChar()
        {
            Mixture m = new Mixture();
            m = 'd';
            var result = m == 'd';
            Assert.AreEqual(true, result);
            result = m == 'a';
            Assert.AreEqual(false, result);
            result = m != 'd';
            Assert.AreEqual(false, result);
            result = m != 'a';
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(true, result);
            result = m == 100.0f;
            Assert.AreEqual(true, result);
            result = m == 100.0d;
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void TestCompareString()
        {
            Mixture m = new Mixture();
            m = "test";
            var result = m == "test";
            Assert.AreEqual(true, result);
            result = m == "a";
            Assert.AreEqual(false, result);
            result = m != "test";
            Assert.AreEqual(false, result);
            result = m != "a";
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
            result = m == 100.0f;
            Assert.AreEqual(false, result);
            result = m == 100.0d;
            Assert.AreEqual(false, result);
            result = m == 'c';
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCompareArrayInt()
        {
            Mixture m = new Mixture();
            m = new[] { 1, 2 };
            var testArray1 = new[] { 1, 2 };
            var testArray2 = new[] { 1, 3 };
            var testArray3 = new[] { 1, 2, 3 };
            var result = testArray1 == m;
            Assert.AreEqual(true, result);
            result = m == testArray2;
            Assert.AreEqual(false, result);
            result = m == testArray3;
            Assert.AreEqual(false, result);
            result = testArray1 != m;
            Assert.AreEqual(false, result);
            result = m != testArray2;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCompareArrayFloat()
        {
            Mixture m = new Mixture();
            m = new[] { 1.0f, 2.0f };
            var testArray1 = new[] { 1.0f, 2.0f };
            var testArray2 = new[] { 1.0f, 3.0f };
            var testArray3 = new[] { 1.0f, 2.0f, 3.0f };
            var result = testArray1 == m;
            Assert.AreEqual(true, result);
            result = m == testArray2;
            Assert.AreEqual(false, result);
            result = m == testArray3;
            Assert.AreEqual(false, result);
            result = testArray1 != m;
            Assert.AreEqual(false, result);
            result = m != testArray2;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCompareArrayDouble()
        {
            Mixture m = new Mixture();
            m = new[] { 1.0d, 2.0d };
            var testArray1 = new[] { 1.0d, 2.0d };
            var testArray2 = new[] { 1.0d, 3.0d };
            var testArray3 = new[] { 1.0d, 2.0d, 3.0d };
            var result = testArray1 == m;
            Assert.AreEqual(true, result);
            result = m == testArray2;
            Assert.AreEqual(false, result);
            result = m == testArray3;
            Assert.AreEqual(false, result);
            result = testArray1 != m;
            Assert.AreEqual(false, result);
            result = m != testArray2;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCompareArrayChar()
        {
            Mixture m = new Mixture();
            m = new[] { 'a', 'b' };
            var testArray1 = new[] { 'a', 'b' };
            var testArray2 = new[] { 'a', 'c' };
            var testArray3 = new[] { 'a', 'b', 'c' };
            var result = testArray1 == m;
            Assert.AreEqual(true, result);
            result = m == testArray2;
            Assert.AreEqual(false, result);
            result = m == testArray3;
            Assert.AreEqual(false, result);
            result = testArray1 != m;
            Assert.AreEqual(false, result);
            result = m != testArray2;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCompareArrayBool()
        {
            Mixture m = new Mixture();
            m = new[] { true, false };
            var testArray1 = new[] { true, false };
            var testArray2 = new[] { true, true };
            var testArray3 = new[] { true, false, true };
            var result = testArray1 == m;
            Assert.AreEqual(true, result);
            result = m == testArray2;
            Assert.AreEqual(false, result);
            result = m == testArray3;
            Assert.AreEqual(false, result);
            result = testArray1 != m;
            Assert.AreEqual(false, result);
            result = m != testArray2;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestCompareArrayString()
        {
            Mixture m = new Mixture();
            m = new[] { "aaa", "bbb" };
            var testArray1 = new[] { "aaa", "bbb" };
            var testArray2 = new[] { "aaa", "ccc" };
            var testArray3 = new[] { "aaa", "bbb", "ccc" };
            var result = testArray1 == m;
            Assert.AreEqual(true, result);
            result = m == testArray2;
            Assert.AreEqual(false, result);
            result = m == testArray3;
            Assert.AreEqual(false, result);
            result = testArray1 != m;
            Assert.AreEqual(false, result);
            result = m != testArray2;
            Assert.AreEqual(true, result);
            result = m == 100;
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestToString()
        {
            Mixture m = new Mixture();
            m = 1;
            Assert.AreEqual(m.ToString(), "1");
        }

        [TestMethod]
        public void TestName()
        {
            Mixture m = new Mixture();
            m = 1;
            m.Name = "1";
            Assert.AreEqual(m.Name, "1");
        }

        [TestMethod]
        public void TestMixtureLength()
        {
            Mixture m = new Mixture();
            m = 100;
            Assert.AreEqual(3, m.Length);
            m = 100.1f;
            Assert.AreEqual(5, m.Length);
            m = 100.0d;
            Assert.AreEqual(3, m.Length);
            m = true;
            Assert.AreEqual(4, m.Length);
            m = false;
            Assert.AreEqual(5, m.Length);
            m = 'a';
            Assert.AreEqual(1, m.Length);
            m = "123";
            Assert.AreEqual(3, m.Length);
            m = new[] { 1, 2 };
            Assert.AreEqual(2, m.Length);
        }
    }
}
