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
// FILE:  Float754Tests.cs
// AUTHOR:  Greg Eakin

using System;
using NUnit.Framework;

namespace RSqrtTests
{
    public class Float754Tests
    {

        [Test]
        public void FloatToStringTest()
        {
            var number = 12.375f;
            var bits = Float754.FloatToString(number);
            Assert.AreEqual("1.54687500 * 2^(3)", bits);
        }

        [Test]
        public void ZeroTest()
        {
            var number = 0.0f;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("0.00000000 * 2^(-126)", floatToString);
        }

        [Test]
        public void NegativeZeroTest()
        {
            var number = BitConverter.Int32BitsToSingle(-2147483648);
            Assert.AreEqual(0.0f, number);
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("-0.00000000 * 2^(-126)", floatToString);
        }

        [Test]
        public void OneTest()
        {
            var number = 1.0f;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("1.00000000 * 2^(0)", floatToString);
        }

        [Test]
        public void MinusOneTest()
        {
            var number = -1.0f;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("-1.00000000 * 2^(0)", floatToString);
        }

        [Test]
        public void SmallestDenormalizedNumber()
        {
            var number = BitConverter.Int32BitsToSingle(0x00000001);
            Assert.AreEqual(1.4e-45f, number);
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("0.00000012 * 2^(-126)", floatToString);
        }

        [Test]
        public void MiddleDenormalizedNumber()
        {
            var number = BitConverter.Int32BitsToSingle(0x00400000);
            Assert.AreEqual(5.87747175E-39f, number);
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("0.50000000 * 2^(-126)", floatToString);
        }

        [Test]
        public void LargestDenormalizedNumber()
        {
            var number = BitConverter.Int32BitsToSingle(0x007FFFFF);
            Assert.AreEqual(1.17549421E-38f, number);
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("0.99999988 * 2^(-126)", floatToString);
        }

        [Test]
        public void SmallestNormalizedNumber()
        {
            var number = BitConverter.Int32BitsToSingle(0x00800000);
            Assert.AreEqual(1.17549435E-38f, number);
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("1.00000000 * 2^(-126)", floatToString);
        }

        [Test]
        public void LargestNormalizedNumber()
        {
            var number = BitConverter.Int32BitsToSingle(0x7F7FFFFF);
            Assert.AreEqual(3.40282347E+38f, number);
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("1.99999988 * 2^(127)", floatToString);
        }

        [Test]
        public void PositiveInfinityTest()
        {
            var number = float.PositiveInfinity;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("1.00000000 * 2^(128)", floatToString);
        }

        [Test]
        public void NegativeInfinityTest()
        {
            var number = float.NegativeInfinity;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("-1.00000000 * 2^(128)", floatToString);
        }

        [Test]
        public void NotANumberTest()
        {
            var number = float.NaN;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("-1.50000000 * 2^(128)", floatToString);
        }

        [Test]
        public void SmallOneTest()
        {
            var number = 1.0000001f;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("1.00000012 * 2^(0)", floatToString);
        }

        [Test]
        public void SmallTwoTest()
        {
            var number = 1.6000001f;
            var floatToString = Float754.FloatToString(number);
            Assert.AreEqual("1.60000014 * 2^(0)", floatToString);
        }
    }
}