using System;
using System.Buffers.Binary;
using System.Text;

namespace Legion.Archive
{
    public class AmigaBinaryReader : IBinaryReader
    {
        private readonly byte[] _buffer;
        private int _pos = 0;

        public AmigaBinaryReader(byte[] data)
        {
            _buffer = data;
        }

        public void Skip(int num)
        {
            _pos += num;
        }

        public byte ReadInt8()
        {
            return ReadBytes(1)[0];
        }

        public byte[] ReadInt8Array(int num)
        {
            return ReadBytes(num).ToArray();
        }

        public short ReadInt16()
        {
            return BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));
        }

        public short[] ReadInt16Array(int num)
        {
            var arr = new short[num];
            for (int i = 0; i < num; i++)
            {
                arr[i] = ReadInt16();
            }
            return arr;
        }

        public int ReadInt32()
        {
            return BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
        }

        public int[] ReadInt32Array(int num)
        {
            var arr = new int[num];
            for (int i = 0; i < num; i++)
            {
                arr[i] = ReadInt32();
            }
            return arr;
        }

        public string ReadCString(int length)
        {
            return Encoding.Default.GetString(ReadBytes(length));
        }

        public string ReadText()
        {
            var length = ReadInt8();
            return ReadCString(length);
        }

        private ReadOnlySpan<byte> ReadBytes(int num)
        {
            var oldPos = _pos;
            _pos += num;
            return new ReadOnlySpan<byte>(_buffer, oldPos, num);
        }
    }
}