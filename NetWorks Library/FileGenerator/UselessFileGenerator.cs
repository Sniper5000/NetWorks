﻿using MathThreader;
using NetWorks.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetWorks_Library.FileGenerator
{
    /// <summary>
    /// Useless.. except for tests
    /// </summary>
    public class UselessFileGenerator
    {
        private static readonly long MaxSize = 1073741824; //1GB

        /// <summary>
        /// Creates a random mess <see cref="byte"/>[]
        /// </summary>
        /// <param name="size"> Size of the mess</param>
        /// <returns> messy <see cref="byte"/>[] </returns>
        public static byte[] RandomBytes(long size)
        {
            if (size > MaxSize)
                return new byte[1];

            //this file is made up of numbers
            var UFile = new byte[size];
            var Div = size / 4;
            var Rem = size % 4;

            for (var i = 0; i < Div; i++)
            {
                var R = BitConverter.GetBytes(MathHX.SRandom(-999999, 999999));
                R.CopyTo(UFile, i * 4);
            }

            for (int i = 0; i < Rem; i++)
            {
                var R = MathHX.SRandom();
                UFile[UFile.Length - i] = R;
            }

            return UFile; //VERY.. useless
        }

        /// <summary>
        /// Creates a useless file at path
        /// </summary>
        /// <param name="path"> path that will hold the useless file</param>
        /// <param name="size"> amount of uselessness you require</param>
        public static void CreateUselessFile(string path, long size)
        {
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            var Div = size / (MaxSize / 8);
            var Rem = size % (MaxSize / 8);
            Console.WriteLine($"Generating File.. | Parts ({NumberFormatting.FormatDataMagnitude(MaxSize / 8)}) {Div} / Remaining ({NumberFormatting.FormatDataMagnitude(Rem)})");
            using FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            for (int i = 0; i < Div; i++)
            {
                Console.WriteLine($"Generating File.. | Part #{i + 1} [{NumberFormatting.FormatDataMagnitude(MaxSize / 8)}] of {Div}");
                fileStream.Write(RandomBytes(MaxSize / 8));
            }
            //remaining part
            if (Rem > 0)
            {
                Console.WriteLine($"Generating File.. | Remaining bytes. [{NumberFormatting.FormatDataMagnitude(Rem)}]");
                fileStream.Write(RandomBytes(Rem));
            }
            fileStream.Flush();
            fileStream.Close();
        }
    }
}
