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
// FILE:  DoubleSqrt.cs
// AUTHOR:  Greg Eakin

using System;
using NUnit.Framework;

namespace RSqrtTests
{
    public class InvSqrtDoubleTests
    {
        // From the video: https://www.youtube.com/watch?v=p8u_k2LIZyo

        // Bits
        // x = (E << 52) | M
        // x = M + Pow(2.0, 52) * E

        // Number
        // x = (1 + M / Pow(2.0, 52)) * Pow(2.0, E - 1023)
        // Log(x) = Log(1 + M / Pow(2.0, 52) * Pow(2.0, E - 1023))
        // Log(x) = Log(1 + M / Pow(2.0, 52)) + Log(Pow(2.0, E - 1023))
        // Log(x) = Log(1 + M / Pow(2.0, 52)) + E - 1023

        // Trick
        // log(1 + d) ~= d + mu, for small d, mu = 0.0430

        // Log(x) = M / Pow(2.0, 52) + mu + E - 1023
        // Log(x) = Pow(2.0, -52) * (M + Pow(2.0, 52) * E) + mu - 1023

        // Inverse square root:
        // gama = 1 / sqrt(y)
        // log(gama) = log(1 / sqrt(y))
        // log(gama) = log(Pow(y, -0.5))
        // log(gama) = -0.5 * log(y);
        // Pow(2.0, -52) * (Mg + Pow(2.0, 52) * Eg) + mu - 1023
        //            = -0.5 * (Pow(2.0, -52) * (My + Pow(2.0, 52) * Ey) + mu - 1023)
        // Mg + Pow(2.0, 52) * Eg = 1.5 * Pow(2.0, 52) * (1023 - mu) - 0.5 * (My + Pow(2.0, 52) * Ey)
        // gama                   = 6910483146024060928L - (b >> 1)

        [Test]
        public void VideoFactorDouble()
        {
            var c = 6.9104831460240609E+18;
            Assert.AreEqual(6910483146024060928L, (long)(c + 0.5));
            Assert.AreEqual(0x5FE6F7CED9168800L, (long)(c + 0.5));
            Assert.AreEqual(6.9104831460240609E+18, c);
            var str = Double754.DoubleToString(c);
            Assert.AreEqual("1.4984721679687500 * 2^(62)", str);

            var mu = 0.0430;
            var c1 = 3.0 / 2.0;
            var c2 = Math.Pow(2.0, 52);
            var c3 = 1023.0 - mu;
            var calc = c1 * c2 * c3;
            Assert.AreEqual(c, calc);
        }

        [Test]
        public void Test()
        {
            var number = 256.0;
            Assert.AreEqual("1.0000000000000000 * 2^(8)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);

            Assert.AreEqual("1.0000000000000000 * 2^(-4)", Double754.DoubleToString(0.0625));
            var y = BitConverter.Int64BitsToDouble(0x5FE6F7CED9168800L);
            Assert.AreEqual("1.4355000000000473 * 2^(511)", Double754.DoubleToString(y));

            Assert.AreEqual("1.9355000000000473 * 2^(-5)", Double754.DoubleToString(gama));
            Assert.AreEqual(0.0625, gama, 0.01);
        }

        [Test]
        public void ZeroSqrt()
        {
            var number = 0.0;
            var bits = Double754.DoubleToString(number);
            Assert.AreEqual("0.0000000000000000 * 2^(-1022)", bits);

            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(9.6234541417166161E+153, gama);
        }

        [Test]
        public void SmallestDenormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x0000_0000_0000_0001L);
            Assert.AreEqual(4.9406564584124654E-324, number);
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(9.6234541417166161E+153, gama);
        }

        [Test]
        public void MiddleDenormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x0008_0000_0000_0000L);
            Assert.AreEqual(1.1125369292536007E-308, number);
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(7.9474781504737915E+153, gama);
        }

        [Test]
        public void LargestDenormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x000F_FFFF_FFFF_FFFFL);
            Assert.AreEqual(2.2250738585072009E-308, number);
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(6.4877030621011334E+153, gama);
        }

        [Test]
        public void SmallestNormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x0010_0000_0000_0000L);
            Assert.AreEqual(2.2250738585072014E-308, number);
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(6.4877030621011327E+153, gama);
        }

        [Test]
        public void SqrtOneSixteenthTest()
        {
            var number = 0.0625;
            Assert.AreEqual("1.0000000000000000 * 2^(-4)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(4.0, gama, 0.13);
        }

        [Test]
        public void SqrtQuarterTest()
        {
            var number = 0.25;
            Assert.AreEqual("1.0000000000000000 * 2^(-2)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(2.0, gama, 0.065);
        }

        [Test]
        public void SqrtHalfTest()
        {
            var number = 0.5;
            Assert.AreEqual("1.0000000000000000 * 2^(-1)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(1.414213562, gama, 0.022);
        }

        [Test]
        public void Sqrt2Test()
        {
            var number = 2.0;
            Assert.AreEqual("1.0000000000000000 * 2^(1)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(0.7071067811, gama, 0.011);
        }

        [Test]
        public void Sqrt4Test()
        {
            var number = 4.0;
            Assert.AreEqual("1.0000000000000000 * 2^(2)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(0.5, gama, 0.017);
        }

        [Test]
        public void Sqrt16Test()
        {
            var number = 16.0;
            Assert.AreEqual("1.0000000000000000 * 2^(4)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(0.25, gama, 0.012);
        }

        [Test]
        public void Sqrt256Test()
        {
            var number = 256.0;
            Assert.AreEqual("1.0000000000000000 * 2^(8)", Double754.DoubleToString(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(0.0625, gama, 0.0025);
        }

        [Test]
        public void LargestNormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x7FEF_FFFF_FFFF_FFFFL);
            Assert.AreEqual(1.7976931348623157E+308, number);
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(7.2178092426191773E-155, gama);
        }

        [Test]
        public void PositiveInfinityTest()
        {
            var number = double.PositiveInfinity;
            Assert.IsTrue(double.IsInfinity(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(7.2178092426191764E-155, gama);
        }

        [Test]
        public void NotANumberTest()
        {
            var number = double.NaN;
            Assert.IsTrue(double.IsNaN(number));
            var gama = Double754.FastInvSqrtDouble(number);
            Assert.AreEqual(1.1299430132959441E+154, gama);
        }
    }
}