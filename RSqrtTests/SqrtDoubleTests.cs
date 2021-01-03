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
    public class SqrtDoubleTests
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

        // Square root:
        // gama = sqrt(y)
        // log(gama) = log(sqrt(y))
        // log(gama) = log(Pow(y, 0.5))
        // log(gama) = 0.5 * log(y);
        // Pow(2.0, -52) * (Mg + Pow(2.0, 52) * Eg) + mu - 1023
        //            = 0.5 * (Pow(2.0, -52) * (My + Pow(2.0, 52) * Ey) + mu - 1023)
        // (Mg + Pow(2.0, 52) * Eg) + Pow(2.0, 52) * (mu - 1023)
        //            = 0.5 * ((My + Pow(2.0, 52) * Ey) + Pow(2.0, 52) * (mu - 1023))
        // (Mg + Pow(2.0, 52) * Eg) 
        //            = 0.5 * ((My + Pow(2.0, 52) * Ey) + Pow(2.0, 52) * (mu - 1023)) - Pow(2.0, 52) * (mu - 1023)
        //            = 0.5 * (My + Pow(2.0, 52) * Ey) + 0.5 * Pow(2.0, 52) * (mu - 1023) - Pow(2.0, 52) * (mu - 1023)
        //            = 0.5 * Pow(2.0, 52) * (1023 - mu) + 0.5 * (My + Pow(2.0, 52) * Ey)
        // gama       = 532496253 + (b >> 1)

        [Test]
        public void VideoFactorFloat()
        {
            var c = 2.3034943820080202E+18d;
            Assert.AreEqual(2_303_494_382_008_020_224L, (long)(c + 0.5));
            Assert.AreEqual(0x1FF7_A7EF_9DB2_2D00L, (long)(c + 0.5));
            var str = Double754.DoubleToString(c);
            Assert.AreEqual("1.9979628906250000 * 2^(60)", str);

            var mu = 0.0430;
            var c1 = 1.0 / 2.0;
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
            var gama = Double754.FastSqrtDouble(number);

            Assert.AreEqual("1.0000000000000000 * 2^(4)", Double754.DoubleToString(16));
            Assert.AreEqual("1.9784999999999968 * 2^(3)", Double754.DoubleToString(gama));
            Assert.AreEqual(16.0, gama, 0.173);
        }

        [Test]
        public void ZeroSqrt()
        {
            var number = 0.0;
            var bits = Double754.DoubleToString(number);
            Assert.AreEqual("0.0000000000000000 * 2^(-1022)", bits);

            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.1027156771079482E-154, gama);
        }

        [Test]
        public void SmallestDenormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x0000_0000_0000_0001L);
            Assert.AreEqual(4.9406564584124654E-324, number);
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.1027156771079482E-154, gama);
        }

        [Test]
        public void MiddleDenormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x0008_0000_0000_0000L);
            Assert.AreEqual(1.1125369292536007E-308, number);
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.2891741953879534E-154, gama);
        }

        [Test]
        public void LargestDenormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x000F_FFFF_FFFF_FFFFL);
            Assert.AreEqual(2.2250738585072009E-308, number);
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.4756327136679584E-154, gama);
        }

        [Test]
        public void SmallestNormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x0010_0000_0000_0000L);
            Assert.AreEqual(2.2250738585072014E-308, number);
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.4756327136679585E-154, gama);
        }

        [Test]
        public void SqrtOneSixteenthTest()
        {
            var number = 0.0625;
            Assert.AreEqual("1.0000000000000000 * 2^(-4)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(0.25, gama, 0.00269);
        }

        [Test]
        public void SqrtQuarterTest()
        {
            var number = 0.25;
            Assert.AreEqual("1.0000000000000000 * 2^(-2)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(0.5, gama, 0.00538);
        }

        [Test]
        public void SqrtHalfTest()
        {
            var number = 0.5;
            Assert.AreEqual("1.0000000000000000 * 2^(-1)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(0.70710678118654752440084436210485, gama, 0.0322);
        }

        [Test]
        public void Sqrt2Test()
        {
            var number = 2.0;
            Assert.AreEqual("1.0000000000000000 * 2^(1)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.4142135623730950488016887242097, gama, 0.0643);
        }

        [Test]
        public void Sqrt4Test()
        {
            var number = 4.0;
            Assert.AreEqual("1.0000000000000000 * 2^(2)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(2.0, gama, 0.0216);
        }

        [Test]
        public void Sqrt16Test()
        {
            var number = 16.0;
            Assert.AreEqual("1.0000000000000000 * 2^(4)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(4.0, gama, 0.0431);
        }

        [Test]
        public void Sqrt256Test()
        {
            var number = 256.0;
            Assert.AreEqual("1.0000000000000000 * 2^(8)", Double754.DoubleToString(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(16.0, gama, 0.173);
        }

        [Test]
        public void LargestNormalizedNumberSqrt()
        {
            var number = BitConverter.Int64BitsToDouble(0x7FEF_FFFF_FFFF_FFFFL);
            Assert.AreEqual(1.7976931348623157E+308, number);
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.3263673994695691E+154, gama);
        }

        [Test]
        public void PositiveInfinityTest()
        {
            var number = double.PositiveInfinity;
            Assert.IsTrue(double.IsInfinity(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(1.3263673994695693E+154, gama);
        }

        [Test]
        public void NotANumberTest()
        {
            var number = double.NaN;
            Assert.IsTrue(double.IsNaN(number));
            var gama = Double754.FastSqrtDouble(number);
            Assert.AreEqual(9.1625715882794302E-155, gama);
        }
    }
}