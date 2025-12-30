using System;

namespace BlueToque.Utility
{
    public static class ByteHelper
    {
        /// <summary>
        /// https://stackoverflow.com/questions/16340/how-do-i-generate-a-hashcode-from-a-byte-array-in-c
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ComputeHash(params byte[] data)
        {
            int num = -2128831035;
            for (int i = 0; i < data.Length; i++)
            {
                num = (num ^ data[i]) * 16777619;
            }

            num += num << 13;
            num ^= num >> 7;
            num += num << 3;
            num ^= num >> 17;
            return num + (num << 5);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/16340/how-do-i-generate-a-hashcode-from-a-byte-array-in-c
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetHashCode(byte[] data)
        {
            int num = 0;
            for (int i = 0; i < data.Length; i++)
                num = (num * 31) ^ data[i];
            return num;
        }

        //
        // Summary:
        //     convert a byte array to a hexidecimal string
        //
        // Parameters:
        //   bytes:
        public static string ToHex(this byte[] bytes) => Convert.ToHexString(bytes);

        //
        // Parameters:
        //   b:
        public static string ToHex(this byte b) => b.ToString("X2");

        //
        // Summary:
        //     Convert a hexidecimal string to a byte array
        //
        // Parameters:
        //   hex:
        public static byte[] ToBytes(this string hex) => Convert.FromHexString(hex);

        //if (hex.IsNullOrEmpty())
        //    throw new ArgumentNullException("string is null");

        //if (hex.Length % 2 == 1)
        //    throw new Exception("The binary key cannot have an odd number of digits");

        //byte[] array = new byte[hex.Length / 2];
        //for (int i = 0; i < hex.Length; i += 2)
        //    array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

        //return array;

        //
        // Parameters:
        //   bytes:
        public static byte ToByte(this string bytes) => byte.Parse(bytes);

    }
}
