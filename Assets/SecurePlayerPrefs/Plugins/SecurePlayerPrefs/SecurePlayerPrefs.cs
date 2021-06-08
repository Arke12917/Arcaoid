using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#pragma warning disable 0162, 0414
/*
 * IMPORTANT Please replace keyArray with your 
 * own 256-AES Key!
 * You can generate it here: http://randomkeygen.com/
 * Just use the CodeIgniter Encryption Keys
 */
namespace SecPlayerPrefs
{
    public class SecurePlayerPrefs : ScriptableObject
    {

        private static readonly byte[] Salt =
        new byte[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 12, 13, 14, 15, 16, 17, 18, 19 };
        private static byte[] keyArray = UTF8Encoding.UTF8.GetBytes("H2h2xZe83AX90788QNqJXRiWX88xWI2b"); // 256-AES key Replace with your own

        private static string Encrypt(string toEncrypt)
        {

#if UNITY_EDITOR
            return toEncrypt;
#endif
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private static string Decrypt(string toDecrypt)
        {
#if UNITY_EDITOR
            return toDecrypt;
#endif
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public static void SetInt(string Key, int Value)
        {
            PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value.ToString()));
        }
        public static void SetString(string Key, string Value)
        {
            PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value));
        }
        public static void SetFloat(string Key, float Value)
        {
            PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value.ToString()));
        }
        public static void SetBool(string Key, bool Value)
        {
            PlayerPrefs.SetString(Encrypt(Key), Encrypt(Value.ToString()));
        }

        public static string GetString(string Key)
        {
            String t = PlayerPrefs.GetString(Encrypt(Key));
            if (t == "") return "";
            return Decrypt(t);
        }
        public static int GetInt(string Key)
        {
            String t = PlayerPrefs.GetString(Encrypt(Key));
            if (t == "") return 0;
            return int.Parse(Decrypt(t));
        }
        public static float GetFloat(string Key)
        {
            String t = PlayerPrefs.GetString(Encrypt(Key));
            if (t == "") return 0;
            return float.Parse(Decrypt(t));
        }
        public static bool GetBool(string Key)
        {
            String t = PlayerPrefs.GetString(Encrypt(Key));
            if (t == "") return false;
            return Boolean.Parse(Decrypt(t));
        }
        public static void DeleteKey(string Key)
        {
            PlayerPrefs.DeleteKey(Encrypt(Key));
        }
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public static void Save()
        {
            PlayerPrefs.Save();
        }

        public static bool HasKey(string Key)
        {
            return PlayerPrefs.HasKey(Encrypt(Key));
        }
    }
}
#pragma warning restore 0162, 0414