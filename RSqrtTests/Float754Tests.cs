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
// FILE:  Float754Tests.cs
// AUTHOR:  Greg Eakin

using System;
using Xunit;

namespace RSqrtTests;

public class Float754Tests
{

    [Fact]
    public void FloatToStringTest()
    {
        var number = 12.375f;
        var bits = Float754.FloatToString(number);
        Assert.Equal("1.54687500 * 2^(3)", bits);
    }

    [Fact]
    public void ZeroTest()
    {
        var number = 0.0f;
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("0.00000000 * 2^(-126)", floatToString);
    }

    [Fact]
    public void NegativeZeroTest()
    {
        // 0x8000_0000
        var number = BitConverter.Int32BitsToSingle(-2_147_483_648);
        Assert.Equal(0.0f, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("-0.00000000 * 2^(-126)", floatToString);
    }

    [Fact]
    public void OneTest()
    {
        var number = 1.0f;
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("1.00000000 * 2^(0)", floatToString);
    }

    [Fact]
    public void MinusOneTest()
    {
        var number = -1.0f;
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("-1.00000000 * 2^(0)", floatToString);
    }

    [Fact]
    public void SmallestDenormalizedNumber()
    {
        var number = BitConverter.Int32BitsToSingle(0x00000001);
        Assert.False(float.IsNormal(number));
        Assert.Equal(1.4e-45f, number);
        Assert.Equal(float.Epsilon, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("0.00000012 * 2^(-126)", floatToString);
    }

    [Fact]
    public void MiddleDenormalizedNumber()
    {
        var number = BitConverter.Int32BitsToSingle(0x00400000);
        Assert.False(float.IsNormal(number));
        Assert.Equal(5.87747175E-39f, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("0.50000000 * 2^(-126)", floatToString);
    }

    [Fact]
    public void LargestDenormalizedNumber()
    {
        var number = BitConverter.Int32BitsToSingle(0x007FFFFF);
        Assert.False(float.IsNormal(number));
        Assert.Equal(1.17549421E-38f, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("0.99999988 * 2^(-126)", floatToString);
    }

    [Fact]
    public void SmallestNormalizedNumber()
    {
        var number = BitConverter.Int32BitsToSingle(0x00800000);
        Assert.True(float.IsNormal(number));
        Assert.Equal(1.17549435E-38f, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("1.00000000 * 2^(-126)", floatToString);
    }

    [Fact]
    public void LargestNormalizedNumber()
    {
        var number = BitConverter.Int32BitsToSingle(0x7F7FFFFF);
        Assert.True(float.IsNormal(number));
        Assert.Equal(3.40282347E+38f, number);
        Assert.Equal(float.MaxValue, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("1.99999988 * 2^(127)", floatToString);
    }

    [Fact]
    public void PositiveInfinityTest()
    {
        var number = float.PositiveInfinity;
        Assert.True(float.IsInfinity(number));
        Assert.True(float.IsPositiveInfinity(number));
        Assert.Equal(float.PositiveInfinity, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("1.00000000 * 2^(128)", floatToString);
    }

    [Fact]
    public void NegativeInfinityTest()
    {
        var number = float.NegativeInfinity;
        Assert.True(float.IsInfinity(number));
        Assert.True(float.IsNegativeInfinity(number));
        Assert.Equal(float.NegativeInfinity, number);
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("-1.00000000 * 2^(128)", floatToString);
    }

    [Fact]
    public void NotANumberTest()
    {
        var number = float.NaN;
        Assert.True(float.IsNaN(number));
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("-1.50000000 * 2^(128)", floatToString);
    }

    [Fact]
    public void SmallOneTest()
    {
        var number = 1.0000001f;
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("1.00000012 * 2^(0)", floatToString);
    }

    [Fact]
    public void SmallTwoTest()
    {
        var number = 1.6000001f;
        var floatToString = Float754.FloatToString(number);
        Assert.Equal("1.60000014 * 2^(0)", floatToString);
    }
}