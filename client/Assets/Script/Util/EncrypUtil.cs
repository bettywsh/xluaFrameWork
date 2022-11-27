

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class EncrypUtil
{

    private static string SecretKey = "98fd3baf-e4bb-4fd1-941b-33fe8ea69d66";

    private static string PublicKey =
        //@"<RSAKeyValue><Modulus>sjH+muDvEUjMrd0WM98J1BiG+mqLSj7nsEvUq8VDnsdMuWhRNIzNJyVvdzx2PZBpYo/voZUImuWjAMYnDBVr+F8pksMnvYyX6BRUtTWPxbhbw4Int6+SiSlLqbOKFM3XxiPRyWWdzPHxiOKhFS1XzJe4ws/IO0qDq6aq+55efnM=</Modulus><Exponent>AQAB</Exponent><P>6rwr5kh+mBOgL/NNxLv04XBziOaqY0MDz+w/ZaT+BTWLkXHcyA7RWkU10u5xNfJFta4zyfswFOm5G3V72MwB2Q==</P><Q>wlaWouTYdSxVSe73mwm7DqK9E+XC0yuv7mc6E6z6vrXe/CJgTInQ3gY6DCGt89o68Cr6vC7+yp3pi3ohVu9HKw==</Q><DP>QxtosX0oM/Hoacz0/rl9WLX9UbYgICCYhmjT9wskU/jq3bnflJIn59bBfHwOgY2xwC79P1FAfboT4XXE4kXwgQ==</DP><DQ>LSQd03Ki0PzeRtEqVKn6+FhW4SNSkOip8g+qTt20VM48IIb/pWexy5DYtO6x0F3VqOni2gl0h64MjKhuzBnChw==</DQ><InverseQ>yLJHiriLnQJVw6vPfwvLDgbQa/PgRGIL85TeFz16emlFjM0SevBZHpBzqpV7S5BtLlL1taczdlo+EDKmcNcO/A==</InverseQ><D>rzGKAVmekZ7CZTmVZ6AkOJt4sWDo2zEnduPHDq4eAlAttafDFuhLlu4uPct68KI3iki7L2CoGiolbVBj+XRiNiP3W/d0oSsYG5QPaLaHPY0xFQNcEscxHNVTd471tVNoRs9aaccKTmxByhwAfG8Vl7AQwejy84CHlI1g+MHLwnE=</D></RSAKeyValue>";
        @"<RSAKeyValue><Modulus>iABfbNMQozgGBnJdh5kc2qT3kyLQVGUjEUZfBpleivOygIdHVq1KY4UGSl+cluOMm2iQ7erWOxM897LfDS6ndYdk2FYSp7t8IzfuxeDnCTxBQBDso6yobAbOuaEtbObw6jsfrwTg9lkC4uHHAcrX/N5MTsLfHxptJw121YjSVYU=</Modulus><Exponent>AQAB</Exponent><P>7tsEBqHkcLb0d2y2fLskfNYklsk1YbJfHk+MrRgA84W9BB2UUoVt8pLweNA189Cjv2eUYXcacp9kFvlapBHULQ==</P><Q>kcNoTTIqEQcC7dnZos0LpRWRxeB1NYqsNoCk+78IA7ycFpb+DqW4p8OnZ6wsju708aB1pFQy1wz3aqqLNUuluQ==</Q><DP>rnsrK7UjbPsZBgxSGjeS5eaAf+2noyBrv3aY6vD38OynMNRyvv1dXa/dHkPqxZqJo46Eo9Yfac/pi6bHgcbMWQ==</DP><DQ>gaUdtdtyUua7kjX7PrPFMbW3jBoR5edLOMa+9zJ3vGsbIXR2zyxSytRYpvsaPp8GuYqQV/KTrXZY8URywJph8Q==</DQ><InverseQ>MFv5ibhqbmXq+LJv4Znm63lDYSHHjOeMo/Ix10ZkD4igk22GtIblj/PgFX+nEGVGrfzl8WoyD30vyBcrGWpeuQ==</InverseQ><D>cz/lncHq+nXyXSozakJtOjfL+WrqImqmYfBBfMUhYh0L6nE5GhG11UoYP5RwjUl9kQD2uDdmnh86bimtbW1YyDEpmsP4Kj7NkjNtCki4yL/RNuWET7JAEWwk5zKiewPU6fUP7/AnlUnelZ0dkIc27EWT8bBNCGOfzZ3K/gSwwaE=</D></RSAKeyValue>";
    //private static string SerectKey =
    //    @"<RSAKeyValue><Modulus>iABfbNMQozgGBnJdh5kc2qT3kyLQVGUjEUZfBpleivOygIdHVq1KY4UGSl+cluOMm2iQ7erWOxM897LfDS6ndYdk2FYSp7t8IzfuxeDnCTxBQBDso6yobAbOuaEtbObw6jsfrwTg9lkC4uHHAcrX/N5MTsLfHxptJw121YjSVYU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    private static string SerectKey =
        @"<RSAKeyValue><Modulus>sjH+muDvEUjMrd0WM98J1BiG+mqLSj7nsEvUq8VDnsdMuWhRNIzNJyVvdzx2PZBpYo/voZUImuWjAMYnDBVr+F8pksMnvYyX6BRUtTWPxbhbw4Int6+SiSlLqbOKFM3XxiPRyWWdzPHxiOKhFS1XzJe4ws/IO0qDq6aq+55efnM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    /// <summary>
    /// 使用给定密钥加密
    /// </summary>
    /// <param name="original">明文</param>
    /// <param name="key">密钥</param>
    /// <returns>密文</returns>
    public static byte[] EncryptKey(byte[] original, byte[] key)
    {
        TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        des.Key = MakeMd5(key);
        des.Mode = CipherMode.ECB;
        return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
    }

    /// <summary>
    /// 使用给定密钥解密数据
    /// </summary>
    /// <param name="encrypted">密文</param>
    /// <param name="key">密钥</param>
    /// <returns>明文</returns>
    public static byte[] DecryptKey(byte[] encrypted, byte[] key)
    {
        TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
        des.Key = MakeMd5(key);
        des.Mode = CipherMode.ECB;
        return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
    }

    /// <summary>
    /// 生成MD5摘要
    /// </summary>
    /// <param name="original">数据源</param>
    /// <returns>摘要</returns>
    public static byte[] MakeMd5(byte[] original)
    {
        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        byte[] keyhash = hashmd5.ComputeHash(original);
        hashmd5 = null;
        return keyhash;
    }

    /// <summary>
    /// 生成MD5的二進制簽名
    /// </summary>
    /// <param name="md5URL"></param>
    /// <returns></returns>
    public static string MakeMd5(string md5URL)
    {
        string orign = SortFristChar(md5URL);
        string sortMd5  = orign + "&sign=" + SecretKey;
        return md5URL.TrimEnd('&') + "&sign=" + MD5Encrypt(sortMd5);
    }


    /// <summary>
    /// MD5的重新排序後進行的MD5的驗證
    /// </summary>
    /// <param name="md5URL"></param>
    /// <returns></returns>
    public static string SortFristChar(string md5URL)
    {
        string md5str = md5URL.TrimEnd('&');
        var listStr = md5str.Split('&').OrderBy(x => x.ToUpper());

        return string.Join("&",listStr);
    }


    /// <summary>
    /// RSA Encrypt 
    /// </summary>
    /// <param name="sSource" >Source string</param>
    /// <param name="sPublicKey" >public key</param>
    /// <returns></returns>
    public static string EncryptRSAString(string sSource)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        string plaintext = sSource;
        rsa.FromXmlString(SerectKey);

        Byte[] PlaintextData = Encoding.UTF8.GetBytes(plaintext);
        int MaxBlockSize = rsa.KeySize / 8 - 11;    //加密块最大长度限制

        if (PlaintextData.Length <= MaxBlockSize)
            return Convert.ToBase64String(rsa.Encrypt(PlaintextData, false));

        using (MemoryStream PlaiStream = new MemoryStream(PlaintextData))
        using (MemoryStream CrypStream = new MemoryStream())
        {
            Byte[] Buffer = new Byte[MaxBlockSize];
            int BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);

            while (BlockSize > 0)
            {
                Byte[] ToEncrypt = new Byte[BlockSize];
                Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);

                Byte[] Cryptograph = rsa.Encrypt(ToEncrypt, false);
                CrypStream.Write(Cryptograph, 0, Cryptograph.Length);

                BlockSize = PlaiStream.Read(Buffer, 0, MaxBlockSize);
            }

            return Convert.ToBase64String(CrypStream.ToArray(), Base64FormattingOptions.None);
        }
    }


    public static string DecryptRSAString(string ciphertext)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(PublicKey);
        
        Byte[] CiphertextData = Convert.FromBase64String(ciphertext);
        int MaxBlockSize = rsa.KeySize / 8;    //解密块最大长度限制
 
        if (CiphertextData.Length <= MaxBlockSize)
            return Encoding.UTF8.GetString(rsa.Decrypt(CiphertextData, false));
 
        using (MemoryStream CrypStream = new MemoryStream(CiphertextData))
        using (MemoryStream PlaiStream = new MemoryStream())
        {
            Byte[] Buffer = new Byte[MaxBlockSize];
            int BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);

            while (BlockSize > 0)
            {
                Byte[] ToDecrypt = new Byte[BlockSize];
                Array.Copy(Buffer, 0, ToDecrypt, 0, BlockSize);

                Byte[] Plaintext = rsa.Decrypt(ToDecrypt, false);
                PlaiStream.Write(Plaintext, 0, Plaintext.Length);

                BlockSize = CrypStream.Read(Buffer, 0, MaxBlockSize);
            }

            return Encoding.UTF8.GetString(PlaiStream.ToArray());
        }
        
    }

    /// <summary>
    /// 使用给定密钥字符串解密string
    /// </summary>
    /// <param name="original">密文</param>
    /// <param name="key">密钥</param>
    /// <returns>明文</returns>
    public static string Decrypt(string original)
    {
        return Decrypt(original, SecretKey, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// 使用给定密钥字符串解密string,返回指定编码方式明文
    /// </summary>
    /// <param name="encrypted">密文</param>
    /// <param name="key">密钥</param>
    /// <param name="encoding">字符编码方案</param>
    /// <returns>明文</returns>
    public static string Decrypt(string encrypted, string key, Encoding encoding)
    {
        try
        {
            byte[] buff = Convert.FromBase64String(encrypted);
            byte[] kb = System.Text.Encoding.UTF8.GetBytes(key);
            return encoding.GetString(DecryptKey(buff, kb));
        }
        catch(Exception e)
        {
            return String.Empty;
        }
    }

    /// <summary>
    /// 使用给定密钥字符串加密string
    /// </summary>
    /// <param name="original">原始文字</param>
    /// <param name="key">密钥</param>
    /// <returns>密文</returns>
    public static string Encrypt(string original)
    {
        byte[] buff = System.Text.Encoding.UTF8.GetBytes(original);
        byte[] kb = System.Text.Encoding.UTF8.GetBytes(SecretKey);
        return Convert.ToBase64String(EncryptKey(buff, kb));
    }

    /// <summary>
    /// 用MD5加密字符串
    /// </summary>
    /// <param name="password">待加密的字符串</param>
    /// <returns></returns>
    public static string MD5Encrypt(string password)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] hashedDataBytes;
        hashedDataBytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
        StringBuilder tmp = new StringBuilder();
        foreach (byte i in hashedDataBytes)
        {
            tmp.Append(i.ToString("x2"));
        }
        return tmp.ToString();
    }

}
