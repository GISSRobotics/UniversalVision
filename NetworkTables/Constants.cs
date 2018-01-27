using System;
using System.Collections.Generic;
using System.Text;

namespace NetworkTables
{
    class Constants
    {

		public static int DEFAULT_PORT = 1735;

        public static int PROTOCOL_VERSION = 0x0200;

        public enum MSG_TYPE
        {
            NOOP = 0x00,
            HELLO = 0x01,

            HELLO_DONE = 0x03,


            ASSIGN = 0x10,
            UPDATE = 0x11
        }

        public enum VAR_TYPE
        {
            BOOL = 0x00,
            NUM = 0x01,
            STR = 0x02,

            BOOL_ARRAY = 0x10,
            NUM_ARRAY = 0x11,
            STR_ARRAY = 0x12
        }

    }
}
