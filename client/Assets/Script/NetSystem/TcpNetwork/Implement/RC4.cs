// Addby hubolin 20170616
// 网络层消息加解密

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

    public sealed class RC4
    {

        private static RC4 _instance;

        private static readonly object _syncLock = new object();

        public readonly byte[] keyDat = { 65, 106, 119, 101, 114, 80, 106, 98, 112, 56, 51, 53, 72, 109, 118, 38, 64, 103 };

        public byte[] t;
        public byte[] s;

        public List<byte> keyList = new List<byte>();

        //MemoryStream mMSSerialize = new MemoryStream();
        //BinaryFormatter mBF = new BinaryFormatter();

        public static RC4 Instance()
        {
            if(_instance == null)
            {
                lock (_syncLock)
                {
                    if(_instance == null)
                    {
                        _instance = new RC4();
                        _instance.init();
                    }
                }
            }

            return _instance;
        }

        public void init()
        {
            RC4_set_key(keyDat);
        }

        public struct RC4_key
        {
            public RC4_key(byte[] dat, byte a, byte b)
            {
            }
        }

        public RC4_key rc4_key = new RC4_key(new byte[256], 0, 0);

        public static readonly int len_eight = 8;

        private RC4()
        {
        }
        public void RC4EncryptTo(ref byte[] bytes)
        {
            if (bytes != null)
            {
                keyStream(bytes.Length);
                RC4_encrypt(ref bytes);
            }
        }

        public void RC4DecryptTo(ref byte[] bytes)
        {
            if (bytes != null)
            {
                keyStream(bytes.Length);
                RC4_encrypt(ref bytes);
            }
        }

        public void RC4_set_key(byte[] data)
        {

            //init rc4
            int i = 0, j = 0;
            t = new byte[256];
            s = new byte[256];

            for(i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
                t[i] = data[i % data.Length];
            }
            for(i = 0; i < 256; i++)
            {
                j = (j + s[i] + t[i]) % 256;
                swap_byte(ref s, i, ref s, j);
            }
        }


        private void swap_byte(ref byte[] a, int index1, ref byte[] b, int index2)
        {
            byte x;
            x = a[index1];
            a[index1] = b[index2];
            b[index2] = x;
        }

        private void keyStream(int dataLen)
        {
            keyList.Clear();

            int i = 0, j = 0;
            byte t;
            for(int z = 0; z < dataLen; z++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;

                t = (byte)((s[i] + s[j]) % 256);

                keyList.Add(t);
            }
        }

        private void RC4_encrypt(ref byte[] data)
        {

            int k = 0;
            for(k = 0; k < data.Length; k++)
            {
                data[k] ^= keyList[k];
            }
        }



    }