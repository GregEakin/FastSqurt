# Fast Inverse Square Root — A Quake III Algorithm
From the video: [https://www.youtube.com/watch?v=p8u_k2LIZyo](https://www.youtube.com/watch?v=p8u_k2LIZyo)

## Bits
```
x = (E << 23) | M
x = M + Pow(2.0, 23) * E
```

## Number
```
x = (1 + M / Pow(2.0, 23)) * Pow(2.0, E - 127)
```

## Calculation
```
Log(x) = Log(1 + M / Pow(2.0, 23) * Pow(2.0, E - 127))
Log(x) = Log(1 + M / Pow(2.0, 23)) + Log(Pow(2.0, E - 127))
Log(x) = Log(1 + M / Pow(2.0, 23)) + E - 127

// Trick
log(1 + d) ~= d + mu, for small d, mu = 0.0430

Log(x) = M / Pow(2.0, 23) + mu + E - 127
Log(x) = Pow(2.0, -23) * (M + Pow(2.0, 23) * E) + mu - 127

gama = 1 / sqrt(y)
log(gama) = log(1 / sqrt(y))
log(gama) = log(Pow(y, -0.5))
log(gama) = -0.5 * log(y);
Pow(2.0, -23) * (Mg + Pow(2.0, 23) * Eg) + mu - 127
          = -0.5 * (Pow(2.0, -23) * (My + Pow(2.0, 23) * Ey) + mu - 127)
Mg + Pow(2.0, 23) * Eg = 1.5 * Pow(2.0, 23) * (127 - mu) - 0.5 * (My + Pow(2.0, 23) * Ey)
gama = 1597488759 - (y >> 1)
```

```csharp
public static float FastInvSqrt(float y)
{
    var bits = BitConverter.SingleToInt32Bits(y);
    var sum = 1597488759 - (bits >> 1);
    var gama = BitConverter.Int32BitsToSingle(sum);

    gama *= 1.5F - 0.5f * y * gama * gama;   // 1st iteration
    gama *= 1.5F - 0.5f * y * gama * gama;   // 2nd iteration, can be removed
    return gama;
}
```

```csharp
public static float FastSqrtFloat(float y)
{
    var bits = BitConverter.SingleToInt32Bits(y);
    var sum = 532496253 + (bits >> 1);
    var gama = BitConverter.Int32BitsToSingle(sum);

    gama = 0.5f * gama + y * 0.5f / gama;   // 1st iteration
    gama = 0.5f * gama + y * 0.5f / gama;   // 2nd iteration, can be removed
    return gama;
}
```

```csharp
public static double FastInvSqrtDouble(double y)
{
    var bits = BitConverter.DoubleToInt64Bits(y);
    var sum = 6910483146024060928L - (bits >> 1);
    var gama = BitConverter.Int64BitsToDouble(sum);

    gama *= 1.5 - y * 0.5 * gama * gama;   // 1st iteration
    gama *= 1.5 - y * 0.5 * gama * gama;   // 2nd iteration, can be removed
    return gama;
}
```

```csharp
public static double FastSqrtDouble(double y)
{
    var bits = BitConverter.DoubleToInt64Bits(y);
    var sum = 2303494382008020224L + (bits >> 1);
    var gama = BitConverter.Int64BitsToDouble(sum);

    gama = 0.5 * gama + y * 0.5 / gama;   // 1st iteration
    gama = 0.5 * gama + y * 0.5 / gama;   // 2nd iteration, can be removed
    return gama;
}
```
