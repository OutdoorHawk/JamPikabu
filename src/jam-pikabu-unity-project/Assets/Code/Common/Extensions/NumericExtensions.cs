using System;

namespace Code.Common.Extensions
{
  public static class NumericExtensions
  {
    public static float ZeroIfNegative(this float value) => value >= 0 ? value : 0;

    public static int ZeroIfNegative(this int value) => value >= 0 ? value : 0;
    
    public static int RoundToNearestFive(int number)
    {
      return (int)(Math.Round(number / 5.0) * 5);
    }
  }
}