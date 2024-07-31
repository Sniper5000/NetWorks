using System;
using System.Collections.Generic;

static class ByteSearch
{
    // Adapted from https://stackoverflow.com/a/283648 :Ja: | Our Code :D
    public static int IndexOfSequence (this byte[] self, byte[] sequence)
    {
        if (IsEmptyLocate(self, sequence))
            return -1;

        for (int i = 0; i < self.Length; i++)
        {
            if (!IsMatch(self, i, sequence))
                continue;

            return i;
        }

        return -1;
    }

    static bool IsMatch (byte[] array, int position, byte[] candidate)
    {
        if (candidate.Length > (array.Length - position))
            return false;

        for (int i = 0; i < candidate.Length; i++)
            if (array[position + i] != candidate[i])
                return false;

        return true;
    }

    static bool IsEmptyLocate (byte[] array, byte[] candidate)
    {
        return array == null
            || candidate == null
            || array.Length == 0
            || candidate.Length == 0
            || candidate.Length > array.Length;
    }
}