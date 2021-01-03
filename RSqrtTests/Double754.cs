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
// FILE:  Double754.cs
// AUTHOR:  Greg Eakin

using System;

namespace RSqrtTests
{
    public static class Double754
    {
        public static string DoubleToString(double value)
        {
            var bits = BitConverter.DoubleToInt64Bits(value);
            var sign = value >= 0 ? "" : "-";
            var exponent = (bits & 0x7FF0_0000_0000_0000) >> 52;
            var mantissa = bits & 0x000F_FFFF_FFFF_FFFF;

            if (exponent == 0)
            {
                var m1 = mantissa / Math.Pow(2, 52);
                var denormalized = $"{sign}{m1:F16} * 2^({exponent - 1022})";
                return denormalized;
            }

            var m = 1.0 + mantissa / Math.Pow(2, 52);
            var normalized = $"{sign}{m:F16} * 2^({exponent - 1023})";
            return normalized;
        }

        public static double FastInvSqrtDouble(double y)
        {
            var bits = BitConverter.DoubleToInt64Bits(y);
            var sum = 6910483146024060928L - (bits >> 1);
            var gama = BitConverter.Int64BitsToDouble(sum);

            // gama *= 1.5 - y * 0.5 * gama * gama;   // 1st iteration
            // gama *= 1.5 - y * 0.5 * gama * gama;   // 2nd iteration, can be removed
            return gama;
        }

        public static double FastSqrtDouble(double y)
        {
            var bits = BitConverter.DoubleToInt64Bits(y);
            var sum = 2303494382008020224L + (bits >> 1);
            var gama = BitConverter.Int64BitsToDouble(sum);

            // gama *= 1.5 - y * 0.5 * gama * gama;   // 1st iteration
            // gama *= 1.5 - y * 0.5 * gama * gama;   // 2nd iteration, can be removed
            return gama;
        }
    }
}