using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTables
{
    class Struct
    {
        public enum ByteOrder
        {
            BigEndian,
            LittleEndian,
            Native
        }

        /*
        Format  C# Type     Standard size
        B       byte        1
        ?       bool        1

        h       int16       2
        H       uint16      2
        i       int32       4
        I       uint32      4
        l       int64       4
        L       uint64      4

        e       half        2
        f       single      4
        d       double      8
        */

        private static byte[] FixByteOrder(ByteOrder order, byte[] buf)
        {
            if ((BitConverter.IsLittleEndian && order == ByteOrder.BigEndian) ||
                (!BitConverter.IsLittleEndian && order == ByteOrder.LittleEndian))
            {
                Array.Reverse(buf);
            }

            return buf;
        }

        public static byte[] PackByte(byte val)
        {
            return new byte[] { val };
        }

        public static byte UnpackByte(byte[] buf)
        {
            return buf[0];
        }

        public static byte[] PackBool(bool val)
        {
            return BitConverter.GetBytes(val);
        }

        public static bool UnpackBool(byte[] buf)
        {
            return BitConverter.ToBoolean(buf, 0);
        }

        public static byte[] PackInt16(ByteOrder order, Int16 val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static Int16 UnpackInt16(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToInt16(buf, 0);
        }

        public static byte[] PackUInt16(ByteOrder order, UInt16 val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static UInt16 UnpackUInt16(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToUInt16(buf, 0);
        }

        public static byte[] PackInt32(ByteOrder order, Int32 val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static Int32 UnpackInt32(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToUInt16(buf, 0);
        }

        public static byte[] PackUInt32(ByteOrder order, UInt32 val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static UInt32 UnpackUInt32(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToUInt32(buf, 0);
        }

        public static byte[] PackInt64(ByteOrder order, Int64 val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static Int64 UnpackInt64(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToInt64(buf, 0);
        }

        public static byte[] PackUInt64(ByteOrder order, UInt64 val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static UInt64 UnpackUInt64(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToUInt64(buf, 0);
        }

        public static byte[] PackFloat(ByteOrder order, float val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static float UnpackFloat(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToSingle(buf, 0);
        }

        public static byte[] PackDouble(ByteOrder order, double val)
        {
            byte[] buf = BitConverter.GetBytes(val);

            return FixByteOrder(order, buf);
        }

        public static double UnpackDouble(ByteOrder order, byte[] buf)
        {
            buf = FixByteOrder(order, buf);

            return BitConverter.ToDouble(buf, 0);
        }
    }
}
