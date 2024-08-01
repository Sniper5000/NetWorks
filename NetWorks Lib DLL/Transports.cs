using MemoryPack;
using System;
using System.Text;

namespace NetWorks
{
    public class Transports
    {

        #region Serializers
        /// <summary>
        /// Serializes a class into a byte[] array
        /// </summary>
        /// <typeparam name="T"> Class </typeparam>
        /// <param name="obj"> Object </param>
        /// <returns> Serialized string </returns>
        public static byte[] SerializeBClass<T>(T obj)
        {
            if (obj == null)
            {
                Console.WriteLine($"Null Reference");
                throw new NullReferenceException();
            }

            /*
            if (!obj.GetType().IsSerializable)
            {
                Console.WriteLine($"The object is an instance of a class that is not flagged as serializable");
                throw new ArgumentException("The object is an instance of a class that is not flagged as serializable", nameof(obj));
            }
            */

            var bytes = MemoryPackSerializer.Serialize(obj);

            if (bytes.Length < 1)
            {
                //Uhh? wtf?
                Console.WriteLine($"Serializer returned null or 0 bytes..");

                try
                {
                    bytes = MemoryPackSerializer.Serialize(obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


            return bytes;
        }

        /// <summary>
        /// Deserializes a byte[] array into a class
        /// </summary>
        /// <param name="str"> Serialized class string </param>
        /// <returns> Desearilized Class </returns>
        public static T DeserializeBClass<T>(byte[] bytes)
        {
            return MemoryPackSerializer.Deserialize<T>(bytes) ?? throw new NullReferenceException();
        }

        /// <summary>
        /// Serializes a field into a <see cref="byte"/>[]
        /// </summary>
        /// <typeparam name="T"> Type </typeparam>
        /// <param name="obj"> Field </param>
        /// <returns> Serialized <see cref="byte"/>[] </returns>
        public static byte[] SerializeBField<T>(T obj)
        {
            if (obj == null)
            {
                Console.WriteLine($"Null Reference");
                throw new NullReferenceException();
            }
            /*
            if (!obj.GetType().IsSerializable)
            {
                Console.WriteLine($"This hasn't been marked as serializeable {nameof(obj)}");
                throw new ArgumentException("The object is an instance of a class that is not flagged as serializable", nameof(obj));
            }
            */
            var bytes = MemoryPackSerializer.Serialize(obj.GetType(), obj);

            if (bytes.Length < 1)
            {
                //Uhh? wtf?
                Console.WriteLine($"Serializer returned null or 0 bytes..");

                try
                {
                    bytes = MemoryPackSerializer.Serialize(obj.GetType(), obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


            return bytes;
        }

        /// <summary>
        /// Deserializes a byte[] array into an object
        /// </summary>
        /// <param name="bytes"> <see cref="byte"/>[] to deserialize</param>
        /// <param name="type"> <see cref="Type"/> of the object</param>
        /// <returns> Desearilized object </returns>
        public static object DeserializeBField(byte[] bytes, Type type)
        {
            return MemoryPackSerializer.Deserialize(type, bytes) ?? throw new NullReferenceException();
        }

        /// <summary>
        /// Serializes a field into a <see cref="string"/>
        /// </summary>
        /// <typeparam name="T"> Type </typeparam>
        /// <param name="obj"> Field </param>
        /// <returns> Serialized <see cref="string"/></returns>
        public static string SerializeSField<T>(T obj)
        {
            if (obj == null)
            {
                Console.WriteLine($"Null Reference");
                throw new NullReferenceException();
            }
            /*
            if (!obj.GetType().IsSerializable)
            {
                Console.WriteLine($"This hasn't been marked as serializeable {nameof(obj)}");
                throw new ArgumentException("The object is an instance of a class that is not flagged as serializable", nameof(obj));
            }
            */
            var bytes = MemoryPackSerializer.Serialize(obj.GetType(), obj);

            if (bytes.Length < 1)
            {
                //Uhh? wtf?
                Console.WriteLine($"Serializer returned null or 0 bytes..");

                try
                {
                    bytes = MemoryPackSerializer.Serialize(obj.GetType(), obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Deserializes a string into a class
        /// </summary>
        /// <param name="text"> Serialized class string </param>
        /// <returns> Desearilized Class </returns>
        public static object DeserializeSField(string text, Type type)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return MemoryPackSerializer.Deserialize(type, bytes) ?? throw new NullReferenceException();
        }
        #endregion


    }
}