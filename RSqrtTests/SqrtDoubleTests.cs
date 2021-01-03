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
            var c = 6.9104831460240609E+18d;
            Assert.AreEqual(6910483146024060928L, (long)(c + 0.5));
            Assert.AreEqual(0x5FE6F7CED9168800L, (long)(c + 0.5));
            Assert.AreEqual(6.9104831460240609E+18d, c);
            // var str = Float754.DoubleToString(c);
            // Assert.AreEqual("1.48777735 * 2^(30)", str);

            var mu = 0.0430;
            var c1 = 3.0 / 2.0;
            var c2 = Math.Pow(2.0, 52);
            var c3 = 1023.0 - mu;
            var calc = c1 * c2 * c3;
            Assert.AreEqual(c, calc);
        }
        public static double Q_dsqrt(double y)
        {
            var bits = BitConverter.DoubleToInt64Bits(y);
            var sum = 6910483146024060928L - (bits >> 1);
            var gama = BitConverter.Int64BitsToDouble(sum);

            // gama *= 1.5F - y * 0.5 * gama * gama;   // 1st iteration
            // gama *= 1.5F - y * 0.5 * gama * gama;   // 2nd iteration, can be removed
            return gama;
        }

        [Test]
        public void Test()
        {
            var number = 256.0;
            Assert.AreEqual("1.0000000000000000 * 2^(8)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);

            Assert.AreEqual("1.0000000000000000 * 2^(-4)", Double754.DoubleToString(0.0625));
            var y = BitConverter.Int64BitsToDouble(0x5FE6F7CED9168800L);
            Assert.AreEqual("1.4355000000000473 * 2^(511)", Double754.DoubleToString(y));

            // Assert.AreEqual("1.93243015 * 2^(-5)", DoubleToString(gama));
            Assert.AreEqual(0.0625, gama, 0.01);
        }

        [Test]
        public void ZeroSqrt()
        {
            var number = 0.0;
            var bits = Double754.DoubleToString(number);
            Assert.AreEqual("0.0000000000000000 * 2^(-1022)", bits);

            var gama = Q_dsqrt(number);
            Assert.AreEqual(9.6234541417166161E+153d, gama);
        }

        [Test]
        public void SqrtOneSixteenthTest()
        {
            var number = 0.0625;
            Assert.AreEqual("1.0000000000000000 * 2^(-4)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(4.0, gama, 0.2);
        }

        [Test]
        public void SqrtQuarterTest()
        {
            var number = 0.25;
            Assert.AreEqual("1.0000000000000000 * 2^(-2)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(2.0, gama, 0.1);
        }

        [Test]
        public void SqrtHalfTest()
        {
            var number = 0.5;
            Assert.AreEqual("1.0000000000000000 * 2^(-1)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(1.414213562, gama, 0.1);
        }

        [Test]
        public void Sqrt2Test()
        {
            var number = 2.0;
            Assert.AreEqual("1.0000000000000000 * 2^(1)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(0.7071067811, gama, 0.1);
        }

        [Test]
        public void Sqrt4Test()
        {
            var number = 4.0;
            Assert.AreEqual("1.0000000000000000 * 2^(2)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(0.5, gama, 0.1);
        }

        [Test]
        public void Sqrt16Test()
        {
            var number = 16.0;
            Assert.AreEqual("1.0000000000000000 * 2^(4)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(0.25, gama, 0.01);
        }

        [Test]
        public void Sqrt256Test()
        {
            var number = 256.0;
            Assert.AreEqual("1.0000000000000000 * 2^(8)", Double754.DoubleToString(number));
            var gama = Q_dsqrt(number);
            Assert.AreEqual(0.0625, gama, 0.01);
        }
    }
}