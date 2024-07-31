using System;
using System.Collections.Generic;
using System.Linq;

namespace NetWorks.Utils
{

    public static class NumberFormatting
    {
        // Hundreds? nah, Ks? nah, Ms? how about Bs? , Ts? not enough?, then Qs? 
        // Sadly not enough but long won't let us go any higher D:
        private static readonly List<NumberSuffixEntry> numberMagnitudeSuffixes = new List<NumberSuffixEntry>()
    {
        { new NumberSuffixEntry((long) Math.Pow(1000, 1), "K") },
        { new NumberSuffixEntry((long) Math.Pow(1000, 2), "M") },
        { new NumberSuffixEntry((long) Math.Pow(1000, 3), "B") },
        { new NumberSuffixEntry((long) Math.Pow(1000, 4), "T") },
        { new NumberSuffixEntry((long) Math.Pow(1000, 5), "Q") },
        { new NumberSuffixEntry((long) Math.Pow(1000, 6), "Qn") },
    };

        private static readonly List<NumberSuffixEntry> dataMagnitudeSuffixes = new List<NumberSuffixEntry>()
    {
        { new NumberSuffixEntry((long)Math.Pow(1024, 0), "B") },
        { new NumberSuffixEntry((long) Math.Pow(1024, 1), "KB") },
        { new NumberSuffixEntry((long) Math.Pow(1024, 2), "MB") },
        { new NumberSuffixEntry((long) Math.Pow(1024, 3), "GB") },
        { new NumberSuffixEntry((long) Math.Pow(1024, 4), "TB") },
        { new NumberSuffixEntry((long) Math.Pow(1024, 5), "PB") },
        { new NumberSuffixEntry((long) Math.Pow(1024, 6), "EB") },
    };

        private class NumberSuffixEntry
        {
            public long UnitValue;
            public string Suffix;
            public NumberSuffixEntry(long UValue, string Sfx)
            {
                UnitValue = UValue;
                Suffix = Sfx;
            }
        };


        public static string FormatNumberMagnitudeInteger(float number)
        {
            return FormatNumberSuffix(numberMagnitudeSuffixes, number, 0);
        }

        public static string FormatNumberMagnitude(float number, int decimalPlaces = 1)
        {
            return FormatNumberSuffix(numberMagnitudeSuffixes, number, decimalPlaces);
        }

        public static string FormatDataMagnitude(float size, int decimalPlaces = 1)
        {
            if (size == 0) return "0B";
            return FormatNumberSuffix(dataMagnitudeSuffixes, size, decimalPlaces);
        }

        private static string FormatNumberSuffix(List<NumberSuffixEntry> entries, float number, int decimalPlaces)
        {
            if (decimalPlaces < 0)
                throw new ArgumentException("Tried formatting a number with negative decimal places", nameof(decimalPlaces));

            foreach (var entry in Enumerable.Reverse(entries))
            {
                if (Math.Abs(number) >= entry.UnitValue)
                {
                    float units = number / entry.UnitValue;
                    return units.ToString("0." + new string('#', decimalPlaces)) + entry.Suffix;
                }
            }

            return number.ToString();
        }
    }
}