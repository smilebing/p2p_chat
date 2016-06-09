using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Model
{
    //RSA非对称加密

    class RSA
    {
        //生成keys   arr[0] 是 private key 
        //arr[1] 是 public key
        public static string[] GenerateKeys()
        {
            string[] sKeys = new String[2];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            sKeys[0] = rsa.ToXmlString(true);
            sKeys[1] = rsa.ToXmlString(false);
            return sKeys;
        }　

        //rsa 加密
        public byte[] RSAEncrypt(string publicKey,byte [] content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publicKey);
            cipherbytes = rsa.Encrypt(content, false);

            return cipherbytes;      
        }

        public byte[] RSAEncrypt(string publicKey, string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publicKey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(content), false);

            return cipherbytes;
        }

        //rsa 解密
        public byte[] RSADecrypt(string privateKey,byte []  content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privateKey);
            cipherbytes = rsa.Decrypt(content, false);

            return cipherbytes;
        }

        public byte[] RSADecrypt(string privateKey,string content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privateKey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(content), false);

            return cipherbytes;   
        }
    }
}
