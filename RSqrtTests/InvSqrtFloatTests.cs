// Copyright 2025 Greg Eakin
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
using Xunit;

namespace RSqrtTests;

public class InvSqrtFloatTests
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

    // Inverse square root:
    // gama = 1 / sqrt(y)
    // log(gama) = log(1 / sqrt(y))
    // log(gama) = log(Pow(y, -0.5))
    // log(gama) = -0.5 * log(y);
    // Pow(2.0, -23) * (Mg + Pow(2.0, 23) * Eg) + mu - 127
    //            = -0.5 * (Pow(2.0, -23) * (My + Pow(2.0, 23) * Ey) + mu - 127)
    // Mg + Pow(2.0, 23) * Eg = 1.5 * Pow(2.0, 23) * (127 - mu) - 0.5 * (My + Pow(2.0, 23) * Ey)
    // gama                   = 1597488759 - (b >> 1)

    [Fact]
    public void VideoFactorFloat()
    {
        var c = 1597488758.784;
        Assert.Equal(1597488759, (int)(c + 0.5));
        Assert.Equal(0x5f37be77, (int)(c + 0.5));
        Assert.Equal(1.59748877E+09f, (float)c);
        var str = Float754.FloatToString((float)c);
        Assert.Equal("1.48777735 * 2^(30)", str);

        var mu = 0.0430;
        var c1 = 3.0 / 2.0;
        var c2 = Math.Pow(2.0, 23);
        var c3 = 127.0 - mu;
        var calc = c1 * c2 * c3;
        Assert.Equal(c, calc);
    }

    [Fact]
    public void OriginalFactor()
    {
        var c = 0x5f3759df;
        Assert.Equal(1597463007, c);
        Assert.Equal(1597463007.0f, c);
        var str = Float754.FloatToString(c);
        Assert.Equal("1.48775339 * 2^(30)", str);

        var c1 = 3.0 / 2.0;
        var c2 = Math.Pow(2.0, 23); // 8,388,608
        var c3 = c / c1 / c2;
        var mu = 127.0 - c3;
        Assert.Equal(0.045046567916870117, mu);
    }

    [Fact]
    public void ImprovedVideo()
    {
        var c = 0x5f375a86;
        Assert.Equal(1597463174, c);
        Assert.Equal(1597463174.0f, c);
        var str = Float754.FloatToString(c);
        Assert.Equal("1.48775351 * 2^(30)", str);

        var c1 = 3.0 / 2.0;
        var c2 = Math.Pow(2.0, 23); // 8,388,608
        var c3 = c / c1 / c2;
        var mu = 127.0 - c3;
        Assert.Equal(0.045033295949295393, mu);
    }

    [Fact]
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


    [Fact]
    public void Test()
    {
        var number = 256.0f;
        Assert.Equal("1.00000000 * 2^(8)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);

        Assert.Equal("1.00000000 * 2^(-4)", Float754.FloatToString(0.0625f));
        var y = BitConverter.Int32BitsToSingle(0x5f3759df);
        Assert.Equal("1.43243015 * 2^(63)", Float754.FloatToString(y));

        Assert.Equal("1.93550003 * 2^(-5)", Float754.FloatToString(gama));
        Assert.Equal(0.0625f, gama, 0.01);
    }

    [Fact]
    public void ZeroSqrt()
    {
        var number = 0.0f;
        var bits = Float754.FloatToString(number);
        Assert.Equal("0.00000000 * 2^(-126)", bits);

        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(1.32401508E+19f, gama);
    }

    [Fact]
    public void SmallestDenormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x00000001);
        Assert.Equal(1.40129846E-45f, number);
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(1.32401508E+19f, gama);
    }

    [Fact]
    public void MiddleDenormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x00400000);
        Assert.Equal(5.87747175E-39f, number);
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(1.09343078E+19f, gama);
    }

    [Fact]
    public void LargestDenormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x007FFFFF);
        Assert.Equal(1.17549421E-38f, number);
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(8.92591896E+18f, gama);
    }

    [Fact]
    public void SmallestNormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x00800000);
        Assert.Equal(1.17549435E-38f, number);
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(8.92591841E+18f, gama);
    }

    [Fact]
    public void SqrtOneSixteenthTest()
    {
        var number = 0.0625f;
        Assert.Equal("1.00000000 * 2^(-4)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(4.0f, gama, 0.13f);
    }

    [Fact]
    public void SqrtQuarterTest()
    {
        var number = 0.25f;
        Assert.Equal("1.00000000 * 2^(-2)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(2.0f, gama, 0.065f);
    }

    [Fact]
    public void SqrtHalfTest()
    {
        var number = 0.5f;
        Assert.Equal("1.00000000 * 2^(-1)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(1.414213562f, gama, 0.022f);
    }

    [Fact]
    public void Sqrt1Test()
    {
        var number = 1.0f;
        Assert.Equal("1.00000000 * 2^(0)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(1.0f, gama, 0.0323f);
    }

    [Fact]
    public void Sqrt2Test()
    {
        var number = 2.0f;
        Assert.Equal("1.00000000 * 2^(1)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(0.7071067811f, gama, 0.011f);
    }

    [Fact]
    public void Sqrt4Test()
    {
        var number = 4.0f;
        Assert.Equal("1.00000000 * 2^(2)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(0.5f, gama, 0.017f);
    }

    [Fact]
    public void Sqrt16Test()
    {
        var number = 16.0f;
        Assert.Equal("1.00000000 * 2^(4)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(0.25f, gama, 0.012);
    }

    [Fact]
    public void Sqrt256Test()
    {
        var number = 256.0f;
        Assert.Equal("1.00000000 * 2^(8)", Float754.FloatToString(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(0.0625f, gama, 0.0025);
    }

    [Fact]
    public void LargestNormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x7F7FFFFF);
        Assert.Equal(3.40282347E+38f, number);
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(5.24618366E-20f, gama);
    }

    [Fact]
    public void PositiveInfinityTest()
    {
        var number = float.PositiveInfinity;
        Assert.True(float.IsInfinity(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(5.24618333E-20f, gama);
    }

    [Fact]
    public void NotANumberTest()
    {
        var number = float.NaN;
        Assert.True(float.IsNaN(number));
        var gama = Float754.FastInvSqrtFloat(number);
        Assert.Equal(1.55459938E+19f, gama);
    }
}