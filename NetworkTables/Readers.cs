using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTables
{
    class Readers
    {
        public static byte[] ReadBoolAsBytes(Func<int,byte[]> readFunction)
        {
            return readFunction(1);
        }

        public static bool ReadBool(Func<int,byte[]> readFunction)
        {
            return Struct.UnpackBool(ReadBoolAsBytes(readFunction));
        }

        public static byte[] ReadNumAsBytes(Func<int,byte[]> readFunction)
        {
            return readFunction(8);
        }

        public static double ReadNum(Func<int,byte[]> readFunction)
        {
            return Struct.UnpackDouble(Struct.ByteOrder.BigEndian, ReadNumAsBytes(readFunction));
        }

        public static byte[] ReadStrAsBytes(Func<int,byte[]> readFunction)
        {
            int len = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, readFunction(2));
            return readFunction(len);
        }

        public static string ReadStr(Func<int,byte[]> readFunction)
        {
            return Encoding.UTF8.GetString(ReadStrAsBytes(readFunction));
        }

        public static bool[] BoolArrayFromBytes(byte[] data)
        {
            int len = Struct.UnpackInt16(Struct.ByteOrder.BigEndian, Subset(data, 0, 2));
            bool[] bools = new bool[len];
            for (int i=0;i<len;i++)
            {
                bools[i] = Struct.UnpackBool(Subset(data, 2 + i, 1));
            }
            return bools;
        }

        public static byte[] BytesFromBoolArray(bool[] bools)
        {
            int len = bools.Length;
            byte[] buf = new byte[len + 2];
            Struct.PackInt16(Struct.ByteOrder.BigEndian, (short)len).CopyTo(buf, 0);
            for (int i=0;i<len;i++)
            {
                Struct.PackBool(bools[i]).CopyTo(buf, 2 + i);
            }
            return buf;
        }

        public static T[] Subset<T>(T[] array, int start, int length)
        {
            T[] subset = new T[length];
            Array.Copy(array, start, subset, 0, length);
            return subset;
        }
    }
}
