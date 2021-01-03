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
// FILE:  Float754.cs
// AUTHOR:  Greg Eakin

using System;

namespace RSqrtTests
{
    public static class Float754
    {
        public static string FloatToString(float value)
        {
            var bits = BitConverter.SingleToInt32Bits(value);
            var sign = (bits & 0x8000_0000) == 0 ? "" : "-";
            var exponent = (bits & 0x7F80_0000) >> 23;
            var mantissa = bits & 0x007F_FFFF;

            if (exponent == 0)
            {
                var m1 = mantissa / Math.Pow(2, 23);
                var denormalized = $"{sign}{m1:F8} * 2^({exponent - 126})";
                return denormalized;
            }

            var m = 1.0 + mantissa / Math.Pow(2, 23);
            var normalized = $"{sign}{m:F8} * 2^({exponent - 127})";
            return normalized;
        }

        public static float FastInvSqrtFloat(float y)
        {
            var bits = BitConverter.SingleToInt32Bits(y);
            var sum = 1597488759 - (bits >> 1);
            var gama = BitConverter.Int32BitsToSingle(sum);

            // gama *= 1.5F - y * 0.5f * gama * gama;   // 1st iteration
            // gama *= 1.5F - y * 0.5f * gama * gama;   // 2nd iteration, can be removed
            return gama;
        }

        public static float FastSqrtFloat(float y)
        {
            var bits = BitConverter.SingleToInt32Bits(y);
            var sum = 532496253 + (bits >> 1);
            var gama = BitConverter.Int32BitsToSingle(sum);

            // gama = 0.5f * gama + y * 0.5f / gama;   // 1st iteration
            // gama = 0.5f * gama + y * 0.5f / gama;   // 2nd iteration, can be removed
            return gama;
        }
    }
}