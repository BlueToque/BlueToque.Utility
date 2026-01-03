using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BlueToque.Utility
{
    /// <summary>
    /// Derived from this code 
    /// http://stackoverflow.com/questions/165808/simple-two-way-encryption-for-c-sharp
    /// We're generating a random IV each encrypt and storing it with the key
    /// </summary>
    public class SimpleAES : Singleton<SimpleAES>
    {
        /// <summary> Initialize the algorithm with the default key </summary>
        SimpleAES() { }

        /// <summary> Initialize the AES algorithm with a key </summary>
        SimpleAES(byte[] key) => Key = key;

        /// <summary>
        /// We are storing the key in code
        /// I generated this using a random process
        /// </summary>
        private readonly byte[] Key = [
            0x30, 0x14, 0x06, 0x72, 0x45, 0x4f, 0xc1, 0x14,
            0xe5, 0xc5, 0x43, 0x44, 0xf3, 0xf4, 0x48, 0xf1,
            0xbf, 0x78, 0xf1, 0x26, 0xb0, 0x00, 0x9a, 0xcf,
            0x88, 0xb8, 0xc3, 0x71, 0xe2, 0xe6, 0x2a, 0x72
        ];

        /// <summary>
        /// You can generate a new encryption key this way
        /// </summary>
        /// <returns></returns>
        static public byte[] GenerateEncryptionKey()
        {
            using RijndaelManaged rm = new();
            rm.GenerateKey();
            return rm.Key;
        }

        /// <summary>
        /// Generate a new Initialization Vector
        /// </summary>
        /// <returns></returns>
        static public byte[] GenerateEncryptionVector()
        {
            using RijndaelManaged rm = new();
            rm.GenerateIV();
            return rm.IV;
        }

        #region Encrypt

        /// <summary>
        /// Encrypt to a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string EncryptToBase64(string text) => string.IsNullOrEmpty(text) ? string.Empty : Convert.ToBase64String(Encrypt(text));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public byte[] Encrypt(string text, byte[]? iv = null)
        {
            // Translates our text value into a byte array.
            Byte[] bytes = new UTF8Encoding().GetBytes(text);
            byte[] encrypted;

            // generate the crypto transform with the provided initialization vector
            using RijndaelManaged rm = new();
            if (iv == null)
            {
                rm.GenerateIV();
                iv = rm.IV;
            }

            using (ICryptoTransform encryptorTransform = rm.CreateEncryptor(this.Key, iv))
            using (MemoryStream memoryStream = new())
            using (CryptoStream cs = new(memoryStream, encryptorTransform, CryptoStreamMode.Write))
            {
                cs.Write(bytes, 0, bytes.Length);
                cs.FlushFinalBlock();

                #region Read encrypted value back out of the stream
                memoryStream.Position = 0;
                encrypted = new byte[memoryStream.Length];
                memoryStream.Read(encrypted, 0, encrypted.Length);
                #endregion

                //Clean up.
                cs.Close();
                memoryStream.Close();
            }

            byte[] encryptedValue = new byte[encrypted.Length + iv.Length];
            Array.Copy(iv, 0, encryptedValue, 0, iv.Length);
            Array.Copy(encrypted, 0, encryptedValue, iv.Length, encrypted.Length);
            return encryptedValue;
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public string DecryptFromBase64(string encryptedString) => (string.IsNullOrEmpty(encryptedString)) ? string.Empty : Decrypt(Convert.FromBase64String(encryptedString));

        /// <summary>
        /// Decrypt a byte array
        /// We're assuming the IV is stored with the byte array
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string Decrypt(byte[] source)
        {
            if (source == null) return string.Empty;

            if (source.Length < 16) throw new ArgumentOutOfRangeException(nameof(source));

            // pull the initialization vector from the start of the byte array
            byte[] iv = new byte[16];
            byte[] encryptedValue = new byte[source.Length - 16];
            Array.Copy(source, iv, 16);
            Array.Copy(source, 16, encryptedValue, 0, source.Length - 16);
            return Decrypt(encryptedValue, iv);
        }

        /// <summary>
        /// Decrypt a byte array with the given initialization vector
        /// </summary>
        /// <param name="encryptedValue"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private string Decrypt(byte[] encryptedValue, byte[] iv)
        {
            using RijndaelManaged rm = new();
            using ICryptoTransform decryptorTransform = rm.CreateDecryptor(this.Key, iv);
            using MemoryStream encryptedStream = new();
            using CryptoStream decryptStream = new(encryptedStream, decryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(encryptedValue, 0, encryptedValue.Length);
            decryptStream.FlushFinalBlock();

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion

            return new UTF8Encoding().GetString(decryptedBytes);
        }

        #endregion

        /*
        /// <summary>
        /// Convert a string to a byte array.  
        /// NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
        ///   System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        ///   return encoding.GetBytes(str);
        /// However, this results in character values that cannot be passed in a URL.  So, instead, I just
        /// lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            byte val;
            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }

        /// <summary>
        /// Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
        /// System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        /// return enc.GetString(byteArr);    
        /// </summary>
        /// <param name="byteArr"></param>
        /// <returns></returns>
        public string ByteArrToString(byte[] byteArr)
        {
            byte val;
            string tempStr = "";
            for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
            {
                val = byteArr[i];
                if (val < (byte)10)
                    tempStr += "00" + val.ToString();
                else if (val < (byte)100)
                    tempStr += "0" + val.ToString();
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }
        */
    }
}