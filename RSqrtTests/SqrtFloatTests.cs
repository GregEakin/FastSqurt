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
// FILE:  FloatSqrt.cs
// AUTHOR:  Greg Eakin

using System;
using NUnit.Framework;

namespace RSqrtTests
{
    public class SqrtFloatTests
    {
        // From the video: https://www.youtube.com/watch?v=p8u_k2LIZyo

        // Bits
        // x = (E << 23) | M
        // x = M + Pow(2.0, 23) * E

        // Number
        // x = (1 + M / Pow(2.0, 23)) * Pow(2.0, E - 127)
        // Log(x) = Log(1 + M / Pow(2.0, 23) * Pow(2.0, E - 127))
        // Log(x) = Log(1 + M / Pow(2.0, 23)) + Log(Pow(2.0, E - 127))
        // Log(x) = Log(1 + M / Pow(2.0, 23)) + E - 127

        // Trick
        // log(1 + d) ~= d + mu, for small d, mu = 0.0430

        // Log(x) = M / Pow(2.0, 23) + mu + E - 127
        // Log(x) = Pow(2.0, -23) * (M + Pow(2.0, 23) * E) + mu - 127

        // gama = 1 / sqrt(y)
        // log(gama) = log(1 / sqrt(y))
        // log(gama) = log(Pow(y, -0.5))
        // log(gama) = -0.5 * log(y);
        // Pow(2.0, -23) * (Mg + Pow(2.0, 23) * Eg) + mu - 127
        //            = -0.5 * (Pow(2.0, -23) * (My + Pow(2.0, 23) * Ey) + mu - 127)
        // Mg + Pow(2.0, 23) * Eg = 1.5 * Pow(2.0, 23) * (127 - mu) - 0.5 * (My + Pow(2.0, 23) * Ey)
        // gama                   = 1597488759 - (b >> 1)

        [Test]
        public void VideoFactorFloat()
        {
            var c = 1597488758.784;
            Assert.AreEqual(1597488759, (int)(c + 0.5));
            Assert.AreEqual(0x5f37be77, (int)(c + 0.5));
            Assert.AreEqual(1.59748877E+09f, (float)c);
            var str = Float754.FloatToString((float)c);
            Assert.AreEqual("1.48777735 * 2^(30)", str);

            var mu = 0.0430;
            var c1 = 3.0 / 2.0;
            var c2 = Math.Pow(2.0, 23);
            var c3 = 127.0 - mu;
            var calc = c1 * c2 * c3;
            Assert.AreEqual(c, calc);
        }

        [Test]
        public void OriginalFactor()
        {
            var c = 0x5f3759df;
            Assert.AreEqual(1597463007, c);
            Assert.AreEqual(1597463007.0f, c);
            var str = Float754.FloatToString(c);
            Assert.AreEqual("1.48775339 * 2^(30)", str);

            var c1 = 3.0 / 2.0;
            var c2 = Math.Pow(2.0, 23); // 8,388,608
            var c3 = c / c1 / c2;
            var mu = 127.0 - c3;
            Assert.AreEqual(0.045046567916870117, mu);
        }

        public static float Q_rsqrt(float y)
        {
            var bits = BitConverter.SingleToInt32Bits(y);
            var sum = 1597488759 - (bits >> 1);
            var gama = BitConverter.Int32BitsToSingle(sum);

            // gama *= 1.5F - y * 0.5f * gama * gama;   // 1st iteration
            // gama *= 1.5F - y * 0.5f * gama * gama;   // 2nd iteration, can be removed
            return gama;
        }

        [Test]
        public void Step1()
        {
            var num = new[] { 0.001f, 0.01f, 0.0625f, 0.1f, 0.25f, 0.5f, 1f, 2f, 4f, 8f, 10f, 16f, 100f, 1000f };
            foreach (var n in num)
            {
                var c1 = 0x5f3759df;
                var c2 = 1597488759;
                var ans = 1.0f / Math.Sqrt(n);

                // Console.WriteLine(FloatToString(n));
                // Console.WriteLine(FloatToString(1.0f / (float)Math.Sqrt(n)));
                var i = BitConverter.SingleToInt32Bits(n);

                var sum1 = c1 - (i >> 1);
                var y1 = BitConverter.Int32BitsToSingle(sum1);

                var sum2 = c2 - (i >> 1);
                var y2 = BitConverter.Int32BitsToSingle(sum2);

                var diff = Math.Abs(ans - y1) < Math.Abs(ans - y2) ? "<" : ">";
                Console.WriteLine("{0}, {1}, {2}", ans - y1, ans - y2, diff);
            }
        }


        [Test]
        public void Test()
        {
            var number = 256.0f;
            Assert.AreEqual("1.00000000 * 2^(8)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);

            Assert.AreEqual("1.00000000 * 2^(-4)", Float754.FloatToString(0.0625f));
            var y = BitConverter.Int32BitsToSingle(0x5f3759df);
            Assert.AreEqual("1.43243015 * 2^(63)", Float754.FloatToString(y));

            // Assert.AreEqual("1.93243015 * 2^(-5)", FloatToString(gama));
            Assert.AreEqual(0.0625f, gama, 0.01);
        }

        [Test]
        public void ZeroSqrt()
        {
            var number = 0.0f;
            var bits = Float754.FloatToString(number);
            Assert.AreEqual("0.00000000 * 2^(-126)", bits);

            var gama = Q_rsqrt(number);
            Assert.AreEqual(1.32401508E+19f, gama);
        }

        [Test]
        public void SqrtOneSixteenthTest()
        {
            var number = 0.0625f;
            Assert.AreEqual("1.00000000 * 2^(-4)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(4.0f, gama, 0.2f);
        }

        [Test]
        public void SqrtQuarterTest()
        {
            var number = 0.25f;
            Assert.AreEqual("1.00000000 * 2^(-2)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(2.0f, gama, 0.1f);
        }

        [Test]
        public void SqrtHalfTest()
        {
            var number = 0.5f;
            Assert.AreEqual("1.00000000 * 2^(-1)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(1.414213562f, gama, 0.1f);
        }

        [Test]
        public void Sqrt2Test()
        {
            var number = 2.0f;
            Assert.AreEqual("1.00000000 * 2^(1)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(0.7071067811f, gama, 0.1f);
        }

        [Test]
        public void Sqrt4Test()
        {
            var number = 4.0f;
            Assert.AreEqual("1.00000000 * 2^(2)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(0.5f, gama, 0.1f);
        }

        [Test]
        public void Sqrt16Test()
        {
            var number = 16.0f;
            Assert.AreEqual("1.00000000 * 2^(4)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(0.25f, gama, 0.01f);
        }

        [Test]
        public void Sqrt256Test()
        {
            var number = 256.0f;
            Assert.AreEqual("1.00000000 * 2^(8)", Float754.FloatToString(number));
            var gama = Q_rsqrt(number);
            Assert.AreEqual(0.0625f, gama, 0.01f);
        }
    }
}