using System;

namespace Legion.Utils
{
    public readonly record struct BoundingBox(int X, int Y, int Width, int Height)
    {
        public static BoundingBox Empty { get; } = new BoundingBox(0, 0, 0, 0);

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public bool Contains(int x, int y)
        {
            return x >= Left && x < Right && y >= Top && y < Bottom;
        }
    }
}