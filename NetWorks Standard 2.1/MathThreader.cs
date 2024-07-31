using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using System.Security.Cryptography;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using NetWorks.Serializables;
using System.Net.Http.Headers;

//A very cut down version of MathThreader Lib. supports vector2 & vector3 math.
namespace MathThreader
{
    public class MathHX
    {

        /// <summary>
        /// Safe Number Randomizer | Cryptography based
        /// </summary>
        /// <param name="min"> Randomizer min number </param>
        /// <param name="max"> Randomizer max number </param>
        /// <returns> random integer </returns>
        public static int SRandom(int min, int max) //Safe random number generator
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                RandomNumberGenerator.Fill(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }

        /// <summary>
        /// Safe Number Randomizer | Cryptography based
        /// </summary>
        /// <param name="min"> Randomizer min number </param>
        /// <param name="max"> Randomizer max number </param>
        /// <returns> random long </returns>
        public static long SRandom(long min, long max) //Safe random number generator
        {
            ulong scale = ulong.MaxValue;
            while (scale == ulong.MaxValue)
            {
                // Get eight random bytes.
                byte[] eight_bytes = new byte[8];
                RandomNumberGenerator.Fill(eight_bytes);

                // Convert that into an ulong.
                scale = BitConverter.ToUInt32(eight_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (long)(min + (max - min) *
                (scale / (double)ulong.MaxValue));
        }

        /// <summary>
        /// Safe Number Randomizer | Cryptography based
        /// </summary>
        /// <returns> random byte </returns>
        public static byte SRandom() //Safe random number generator
        {
            byte[] single_byte = new byte[1];
            RandomNumberGenerator.Fill(single_byte);

            return single_byte[0];
        }

        /// <summary>
        /// Linearly Interpolates between A and B by T
        /// </summary>
        /// <param name="start_value"></param>
        /// <param name="end_value"></param>
        /// <param name="pct"></param>
        /// <returns> returns a float with the result </returns>
        public static float Lerp(float start_value, float end_value, float pct)
        {
            return (start_value + (end_value - start_value) * pct);
        }

        /// <summary>
        ///  Linearly Interpolates between Vector3 A and Vector3 B by T
        /// </summary>
        /// <param name="start_value"></param>
        /// <param name="end_value"></param>
        /// <param name="t"></param>
        /// <returns> returns a Vector3 with the result </returns>
        public static Vector3Ser Lerp(Vector3Ser start_value, Vector3Ser end_value, float t)
        {
            return start_value + (end_value - start_value) * t;
        }


        public static float EaseIn(float start_value, float target_value, float time_elapsed, float duration)
        {
            time_elapsed /= duration;
            return target_value * time_elapsed * time_elapsed + start_value;
        }

        //Interpolates only the given percentage, which is included between [0,1]
        public static float EaseIn(float t)
        {
            return t * t;
        }

        /// <summary>
        /// Get Angle between point A and B
        /// </summary>
        /// <param name="A"> Caster </param>
        /// <param name="B"> Receiver </param>
        /// <returns></returns>
        public float LookTowards(Vector2Ser A, Vector2Ser B)
        {
            var Result = 0f;

            var PartA = B.y - A.y;
            var PartB = B.x - A.x;
            Result = (float)Math.Atan2(PartA, PartB);

            return Result;
        }

        public class KeyGenerator
        {
            internal static readonly char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

            public static string GetUniqueKey(int size)
            {
                byte[] data = new byte[4 * size];
                using (var crypto = RandomNumberGenerator.Create())
                {
                    crypto.GetBytes(data);
                }
                StringBuilder result = new StringBuilder(size);
                for (int i = 0; i < size; i++)
                {
                    var rnd = BitConverter.ToUInt32(data, i * 4);
                    var idx = rnd % chars.Length;

                    result.Append(chars[idx]);
                }

                return result.ToString();
            }

            public static string GetUniqueKeyOriginal_BIASED(int size)
            {
                char[] chars =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
                var data = new byte[size];
                RandomNumberGenerator.Fill(data);
                StringBuilder result = new StringBuilder(size);
                foreach (byte b in data)
                {
                    result.Append(chars[b % (chars.Length)]);
                }
                return result.ToString();
            }
        }




    } //MathHX Library / Structs / Basic Functions

    [Serializable]
    public struct Vector3Ser
    {
        public float x, y, z;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        /// <param name="rZ"></param>
        public Vector3Ser(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        public Vector3Ser(Vector2Ser rValue)
        {
            x = rValue.x;
            y = rValue.y;
            z = 0;
        }

        public Vector3Ser(float rX, float rY)
        {
            x = rX;
            y = rY;
            z = 0;
        }
        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        public Vector3Ser normalize()
        {
            float distance = sqrMagnitude();//(float)Math.Sqrt(x * x + y * y + z * z);
            return new Vector3Ser(x / distance, y / distance, z / distance);
        }

        public float sqrMagnitude()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public float Magnitude()
        {
            return (float)(x * x + y * y + z * z);
        }

        public float Distance(Vector3Ser a, Vector3Ser b)
        {
            var dist = MathF.Abs(a.sqrMagnitude() - b.sqrMagnitude());
            return dist;
        }


        //UNITY ONLY
        /// <summary>
        /// Automatic conversion from SerializableVector3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        //public static implicit operator Vector3(Vector3Ser rValue)
        //{
        //    return new Vector3(rValue.x, rValue.y, rValue.z);
        //}
        /// <summary>
        /// Automatic conversion from Vector3 to SerializableVector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        //public static implicit operator Vector3Ser(Vector3 rValue)
        //{
        //    return new Vector3Ser(rValue.x, rValue.y, rValue.z);
        //}
        //END

        /// <summary>
        /// Automatic conversion from SerializableVector2 to SerializableVector3
        /// </summary>
        /// <param name="rValue"></param>
        public static implicit operator Vector3Ser(Vector2Ser rValue)
        {
            return new Vector3Ser(rValue.x, rValue.y, 0);
        }

        public static Vector3Ser operator +(Vector3Ser a, Vector3Ser b) => new Vector3Ser(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3Ser operator +(Vector3Ser a, float b) => new Vector3Ser(a.x + b, a.y + b, a.z + b);
        public static Vector3Ser operator +(Vector3Ser a, double b) => new Vector3Ser(a.x + (float)b, a.y + (float)b, a.z + (float)b);
        public static Vector3Ser operator +(Vector3Ser a, int b) => new Vector3Ser(a.x + b, a.y + b, a.z + b);
        public static Vector3Ser operator -(Vector3Ser a, Vector3Ser b) => new Vector3Ser(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3Ser operator -(Vector3Ser a, float b) => new Vector3Ser(a.x - b, a.y - b, a.z - b);
        public static Vector3Ser operator -(Vector3Ser a, double b) => new Vector3Ser(a.x - (float)b, a.y - (float)b, a.z - (float)b);
        public static Vector3Ser operator -(Vector3Ser a, int b) => new Vector3Ser(a.x - b, a.y - b, a.z - b);
        public static Vector3Ser operator /(Vector3Ser a, Vector3Ser b) => new Vector3Ser(a.x / b.x, a.y / b.y, a.z / b.z);
        public static Vector3Ser operator /(Vector3Ser a, float b) => new Vector3Ser(a.x / b, a.y / b, a.z / b);
        public static Vector3Ser operator *(Vector3Ser a, Vector3Ser b) => new Vector3Ser(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vector3Ser operator *(Vector3Ser a, float b) => new Vector3Ser(a.x * b, a.y * b, a.z * b);
    }

    [Serializable]
    public struct Vector2Ser
    {
        public float x, y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public Vector2Ser(float rX, float rY)
        {
            x = rX;
            y = rY;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}]", x, y);
        }

        public Vector2Ser normalize()
        {
            float distance = sqrMagnitude();
            return new Vector2Ser(x / distance, y / distance);
        }

        public float sqrMagnitude()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public float Magnitude()
        {
            return (float)(x * x + y * y);
        }

        public float Distance(Vector2Ser a, Vector2Ser b)
        {
            var dist = MathF.Abs(a.sqrMagnitude() - b.sqrMagnitude());
            return dist;
        }
        
        //UNITY ONLY
        /// <summary>
        /// Automatic conversion from Vector2Ser to Vector2
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        //public static implicit operator Vector2(Vector2Ser rValue)
        //{
        //    return new Vector2(rValue.x, rValue.y);
        //}
        /// <summary>
        /// Automatic conversion from Vector2 to Vector2Ser
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        //public static implicit operator Vector2Ser(Vector3 rValue)
        //{
        //    return new Vector2Ser(rValue.x, rValue.y);
        //}
        //END

        public static implicit operator Vector2Ser(Vector3Ser rValue)
        {
            return new Vector2Ser(rValue.x, rValue.y);
        }

        public static Vector2Ser operator +(Vector2Ser a, Vector2Ser b) => new Vector2Ser(a.x + b.x, a.y + b.y);
        public static Vector2Ser operator +(Vector2Ser a, float b) => new Vector2Ser(a.x + b, a.y + b);
        public static Vector2Ser operator +(Vector2Ser a, double b) => new Vector2Ser(a.x + (float)b, a.y + (float)b);
        public static Vector2Ser operator +(Vector2Ser a, int b) => new Vector2Ser(a.x + b, a.y + b);
        public static Vector2Ser operator -(Vector2Ser a, Vector2Ser b) => new Vector2Ser(a.x - b.x, a.y - b.y);
        public static Vector2Ser operator -(Vector2Ser a, float b) => new Vector2Ser(a.x - b, a.y - b);
        public static Vector2Ser operator -(Vector2Ser a, double b) => new Vector2Ser(a.x - (float)b, a.y - (float)b);
        public static Vector2Ser operator -(Vector2Ser a, int b) => new Vector2Ser(a.x - b, a.y - b);
        public static Vector2Ser operator /(Vector2Ser a, Vector2Ser b) => new Vector2Ser(a.x / b.x, a.y / b.y);
        public static Vector2Ser operator /(Vector2Ser a, float b) => new Vector2Ser(a.x / b, a.y / b);
        public static Vector2Ser operator *(Vector2Ser a, Vector2Ser b) => new Vector2Ser(a.x * b.x, a.y * b.y);
        public static Vector2Ser operator *(Vector2Ser a, float b) => new Vector2Ser(a.x * b, a.y * b);

    }

    [Serializable]
    public class SerializeTexture
    {
        public int x;
        public int y;
        public byte[] bytes;

        public SerializeTexture(int x, int y, byte[] bytes)
        {
            this.x = x;
            this.y = y;
            this.bytes = bytes;
        }
    }

    [Serializable]
    public class SerializeAudio
    {
        public string Name;
        public int Samples;
        public int Channels;
        public int Frequency;
        public float[] Buffer;

        public SerializeAudio(string name, int samples, int channels, int frequency, float[] buffer)
        {
            Name = name;
            Samples = samples;
            Channels = channels;
            Frequency = frequency;
            Buffer = buffer;
        }
    }


    public static class ArrayExt
    {
        public static T[] GetRow<T>(this T[,] array, int row)
        {
            if (!typeof(T).IsPrimitive)
                throw new InvalidOperationException("Not supported for managed types.");

            if (array == null)
                throw new ArgumentNullException("array");

            int cols = array.GetUpperBound(1) + 1;
            T[] result = new T[cols];

            int size;

            if (typeof(T) == typeof(bool))
                size = 1;
            else if (typeof(T) == typeof(char))
                size = 2;
            else
                size = Marshal.SizeOf<T>();

            Buffer.BlockCopy(array, row * cols * size, result, 0, cols * size);

            return result;
        }
    }

}
