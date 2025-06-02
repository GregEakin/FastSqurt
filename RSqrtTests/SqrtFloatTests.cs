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

    // Square root:
    // gama = sqrt(y)
    // log(gama) = log(sqrt(y))
    // log(gama) = log(Pow(y, 0.5))
    // log(gama) = 0.5 * log(y);
    // Pow(2.0, -23) * (Mg + Pow(2.0, 23) * Eg) + mu - 127
    //            = 0.5 * (Pow(2.0, -23) * (My + Pow(2.0, 23) * Ey) + mu - 127)
    // (Mg + Pow(2.0, 23) * Eg) + Pow(2.0, 23) * (mu - 127)
    //            = 0.5 * ((My + Pow(2.0, 23) * Ey) + Pow(2.0, 23) * (mu - 127))
    // (Mg + Pow(2.0, 23) * Eg) 
    //            = 0.5 * ((My + Pow(2.0, 23) * Ey) + Pow(2.0, 23) * (mu - 127)) - Pow(2.0, 23) * (mu - 127)
    //            = 0.5 * (My + Pow(2.0, 23) * Ey) + 0.5 * Pow(2.0, 23) * (mu - 127) - Pow(2.0, 23) * (mu - 127)
    //            = 0.5 * Pow(2.0, 23) * (127 - mu) + 0.5 * (My + Pow(2.0, 23) * Ey)
    // gama       = 532496253 + (b >> 1)

    [Fact]
    public void VideoFactorFloat()
    {
        var c = 532496252.92799997d;
        Assert.Equal(532496253, (int)(c + 0.5));
        Assert.Equal(0x1FBD3F7D, (int)(c + 0.5));
        Assert.Equal(532496256.0f, (float)c);
        var str = Float754.FloatToString((float)c);
        Assert.Equal("1.98370314 * 2^(28)", str);

        var mu = 0.0430;
        var c1 = 1.0 / 2.0;
        var c2 = Math.Pow(2.0, 23);
        var c3 = 127.0 - mu;
        var calc = c1 * c2 * c3;
        Assert.Equal(c, calc);
    }

    [Fact]
    public void Test()
    {
        var number = 256.0f;
        Assert.Equal("1.00000000 * 2^(8)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);

        Assert.Equal("1.00000000 * 2^(4)", Float754.FloatToString(16.0f));
        var y = BitConverter.Int32BitsToSingle(0x1FBD3F7D);
        Assert.Equal("1.47850001 * 2^(-64)", Float754.FloatToString(y));

        Assert.Equal("1.97850001 * 2^(3)", Float754.FloatToString(gama));
        Assert.Equal(16.0f, gama, 0.18);
    }

    [Fact]
    public void ZeroSqrt()
    {
        var number = 0.0f;
        var bits = Float754.FloatToString(number);
        Assert.Equal("0.00000000 * 2^(-126)", bits);

        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(0.0, gama, 8.1e-20);
    }

    [Fact]
    public void SmallestDenormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x00000001);
        Assert.Equal(1.40129846E-45f, number);
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(8.01496461E-20f, gama);
    }

    [Fact]
    public void MiddleDenormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x00400000);
        Assert.Equal(5.87747175E-39f, number);
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(9.37021732E-20f, gama);
    }

    [Fact]
    public void LargestDenormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x007FFFFF);
        Assert.Equal(1.17549421E-38f, number);
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(1.07254694E-19f, gama);
    }

    [Fact]
    public void SmallestNormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x00800000);
        Assert.Equal(1.17549435E-38f, number);
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(1.072547E-19f, gama);
    }

    [Fact]
    public void SqrtOneSixteenthTest()
    {
        var number = 0.0625f;
        Assert.Equal("1.00000000 * 2^(-4)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(0.25f, gama, 0.00269f);
    }

    [Fact]
    public void SqrtQuarterTest()
    {
        var number = 0.25f;
        Assert.Equal("1.00000000 * 2^(-2)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(0.5f, gama, 0.00538f);
    }

    [Fact]
    public void SqrtHalfTest()
    {
        var number = 0.5f;
        Assert.Equal("1.00000000 * 2^(-1)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(0.7071067811f, gama, 0.0322f);
    }

    [Fact]
    public void Sqrt2Test()
    {
        var number = 2.0f;
        Assert.Equal("1.00000000 * 2^(1)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(1.41421356237f, gama, 0.0643f);
    }

    [Fact]
    public void Sqrt1Test()
    {
        var number = 1.0f;
        Assert.Equal("1.00000000 * 2^(0)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(1.0f, gama, 0.0108f);
    }

    [Fact]
    public void Sqrt4Test()
    {
        var number = 4.0f;
        Assert.Equal("1.00000000 * 2^(2)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(2.0f, gama, 0.0216f);
    }

    [Fact]
    public void Sqrt16Test()
    {
        var number = 16.0f;
        Assert.Equal("1.00000000 * 2^(4)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(4.0f, gama, 0.0431f);
    }

    [Fact]
    public void Sqrt256Test()
    {
        var number = 256.0f;
        Assert.Equal("1.00000000 * 2^(8)", Float754.FloatToString(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(16.0f, gama, 0.173f);
    }

    [Fact]
    public void LargestNormalizedNumberSqrt()
    {
        var number = BitConverter.Int32BitsToSingle(0x7F7FFFFF);
        Assert.Equal(3.40282347E+38f, number);
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(1.82484406E+19f, gama);
    }

    [Fact]
    public void PositiveInfinityTest()
    {
        var number = float.PositiveInfinity;
        Assert.True(float.IsInfinity(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(1.82484417E+19f, gama);
    }

    [Fact]
    public void NotANumberTest()
    {
        var number = float.NaN;
        Assert.True(float.IsNaN(number));
        var gama = Float754.FastSqrtFloat(number);
        Assert.Equal(6.65971189E-20f, gama);
    }
}