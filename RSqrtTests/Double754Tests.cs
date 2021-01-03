// Copyright 2021 Greg Eakin
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SUBSYSTEM: RSqrtTests
// FILE:  Double754Tests.cs
// AUTHOR:  Greg Eakin

using System;
using NUnit.Framework;

namespace RSqrtTests
{
    public class Double754Tests
    {

        [Test]
        public void DoubleToStringTest()
        {
            var number = 12.375;
            var bits = Double754.DoubleToString(number);
            Assert.AreEqual("1.5468750000000000 * 2^(3)", bits);
        }

        [Test]
        public void ZeroTest()
        {
            var number = 0.0;
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("0.0000000000000000 * 2^(-1022)", doubleToString);
        }

        [Test]
        public void NegativeZeroTest()
        {
            // 0x8000_0000_0000_0000L
            var number = BitConverter.Int64BitsToDouble(-9_223_372_036_854_775_808L);
            Assert.AreEqual(0.0, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("0.0000000000000000 * 2^(-1022)", doubleToString);
        }

        [Test]
        public void OneTest()
        {
            var number = 1.0;
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("1.0000000000000000 * 2^(0)", doubleToString);
        }

        [Test]
        public void MinusOneTest()
        {
            var number = -1.0;
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("-1.0000000000000000 * 2^(0)", doubleToString);
        }

        [Test]
        public void SmallestDenormalizedNumber()
        {
            var number = BitConverter.Int64BitsToDouble(0x0000_0000_0000_0001L);
            Assert.IsFalse(double.IsNormal(number));
            Assert.AreEqual(4.9406564584124654E-324d, number);
            Assert.AreEqual(double.Epsilon, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("0.0000000000000002 * 2^(-1022)", doubleToString);
        }

        [Test]
        public void MiddleDenormalizedNumber()
        {
            var number = BitConverter.Int64BitsToDouble(0x0008_0000_0000_0000L);
            Assert.IsFalse(double.IsNormal(number));
            Assert.AreEqual(1.1125369292536007E-308d, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("0.5000000000000000 * 2^(-1022)", doubleToString);
        }

        [Test]
        public void LargestDenormalizedNumber()
        {
            var number = BitConverter.Int64BitsToDouble(0x000F_FFFF_FFFF_FFFFL);
            Assert.IsFalse(double.IsNormal(number));
            Assert.AreEqual(2.2250738585072009E-308d, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("0.9999999999999998 * 2^(-1022)", doubleToString);
        }

        [Test]
        public void SmallestNormalizedNumber()
        {
            var number = BitConverter.Int64BitsToDouble(0x0010_0000_0000_0000L);
            Assert.IsTrue(double.IsNormal(number));
            Assert.AreEqual(2.2250738585072014E-308d, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("1.0000000000000000 * 2^(-1022)", doubleToString);
        }

        [Test]
        public void LargestNormalizedNumber()
        {
            var number = BitConverter.Int64BitsToDouble(0x7FEF_FFFF_FFFF_FFFFL);
            Assert.IsTrue(double.IsNormal(number));
            Assert.AreEqual(1.7976931348623157E+308d, number);
            Assert.AreEqual(double.MaxValue, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("1.9999999999999998 * 2^(1023)", doubleToString);
        }

        [Test]
        public void PositiveInfinityTest()
        {
            var number = double.PositiveInfinity;
            Assert.IsTrue(double.IsInfinity(number));
            Assert.IsTrue(double.IsPositiveInfinity(number));
            Assert.AreEqual(double.PositiveInfinity, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("1.0000000000000000 * 2^(1024)", doubleToString);
        }

        [Test]
        public void NegativeInfinityTest()
        {
            var number = double.NegativeInfinity;
            Assert.IsTrue(double.IsInfinity(number));
            Assert.IsTrue(double.IsNegativeInfinity(number));
            Assert.AreEqual(double.NegativeInfinity, number);
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("-1.0000000000000000 * 2^(1024)", doubleToString);
        }

        [Test]
        public void NotANumberTest()
        {
            var number = double.NaN;
            Assert.IsTrue(double.IsNaN(number));
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("-1.5000000000000000 * 2^(1024)", doubleToString);
        }

        [Test]
        public void SmallOneTest()
        {
            var number = 1.000000000000001;
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("1.0000000000000011 * 2^(0)", doubleToString);
        }

        [Test]
        public void SmallTwoTest()
        {
            var number = 1.600000000000001;
            var doubleToString = Double754.DoubleToString(number);
            Assert.AreEqual("1.6000000000000010 * 2^(0)", doubleToString);
        }
    }
}