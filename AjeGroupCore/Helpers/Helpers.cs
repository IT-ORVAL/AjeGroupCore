﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AjeGroupCore.Helpers
{
    public class Helpers
    {
        public static string PhoneFormatter(string phone)
        {
            var myPhone = phone.Replace("(", string.Empty);
            myPhone = myPhone.Replace(")", string.Empty);
            myPhone = myPhone.Replace("-", string.Empty);
            myPhone = myPhone.Replace(" ", string.Empty);

            return myPhone;
        }

        public static string EncryptString(string text, string keyString)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                    {
                        using (var msEncrypt = new MemoryStream())
                        {
                            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }

                            var iv = aesAlg.IV;

                            var decryptedContent = msEncrypt.ToArray();

                            var result = new byte[iv.Length + decryptedContent.Length];

                            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                            Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                            return Convert.ToBase64String(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return string.Empty;
            }


        }

        public static string DecryptString(string cipherText, string keyString)
        {
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);

                var iv = new byte[16];
                var cipher = new byte[16];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
                var key = Encoding.UTF8.GetBytes(keyString);

                using (var aesAlg = Aes.Create())
                {
                    using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                    {
                        string result;
                        using (var msDecrypt = new MemoryStream(cipher))
                        {
                            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(csDecrypt))
                                {
                                    result = srDecrypt.ReadToEnd();
                                }
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return string.Empty;
            }


        }

        public static string Base64ForUrlEncode(string str)
        {

            var encbuff = Encoding.UTF8.GetBytes(str);
            //var result = WebUtility.UrlDecode(encbuff);

            return encbuff.ToString();
        }

        public static string Base64ForUrlDecode(string str)
        {
            return str != null ? WebUtility.UrlDecode(str) : null;
        }
    }
}
