using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTables
{
    class Storage
    {

        public struct Item
        {
            private short seq;
            private short id;
            private Constants.VAR_TYPE type;
            private byte[] value;

            public short Seq { get { return seq; } set { seq = value; } }
            public short Id { get { return id; } set { id = value; } }
            public Constants.VAR_TYPE Type { get { return type; } set { type = value; } }
            public byte[] Value { get { return (byte[])value.Clone(); } set { this.value = (byte[])value.Clone(); } }
            public Item(short seq, short id, Constants.VAR_TYPE type, byte[] value) { this.seq = seq; this.id = id; this.type = type; this.value = (byte[])value.Clone(); }
        }

        private Dictionary<string, Item> store;
        private Dictionary<int, string> pathLookup;

        public Storage()
        {
            store = new Dictionary<string, Item> { };
            pathLookup = new Dictionary<int, string> { };
        }

        public string GetPathFromId(int id)
        {
            if (!pathLookup.ContainsKey(id))
            {
                return "";
            }

            return pathLookup[id];
        }

        public bool IsSeqHigher(string path, short seq)
        {
            if (!store.ContainsKey(path))
            {
                return true;
            }

            return store[path].Seq < seq;
        }

        public void AddValue(string path, short id, Constants.VAR_TYPE type, byte[] value)
        {
            store[path] = new Item(0, id, type, value);
            pathLookup[id] = path;
        }

        public void UpdateValue(string path, short seq, short id, Constants.VAR_TYPE type, byte[] value)
        {
            if (!store.ContainsKey(path))
            {
                AddValue(path, id, type, value);
                return;
            }

            Item i = store[path];

            if (seq <= i.Seq)
            {
                return;
            }

            i.Seq = seq;
            i.Value = value;
            store[path] = i;
        }

        public bool ValueExists(string path)
        {
            return store.ContainsKey(path);
        }

        public bool HasValue(string path)
        {
            return store.ContainsKey(path);
        }

        public Item GetValue(string path)
        {
            return store[path];
        }
    }
}
