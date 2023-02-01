namespace Legion.Archive
{
    public interface IBinaryReader
    {
        void Skip(int num);
        byte ReadInt8();
        short ReadInt16();
        int ReadInt32();
        byte[] ReadInt8Array(int num);
        int[] ReadInt32Array(int num);
        short[] ReadInt16Array(int num);
        string ReadCString(int length);
        string ReadText();
    }
}
