using System;
using System.Drawing;
using System.Text;

namespace useless
{
    public class ImageFoker : IDisposable
    {
        const int
            indexR = 16,
            indexG = 8,
            indexB = 0,
            indexOffset = 4,
            startIndex = 0,
            lengthBitCount = 15;

        readonly Bitmap bmp;
        readonly int width;
        readonly int height;
        readonly int length;

        public ImageFoker(Image image)
        {
            bmp = new Bitmap(image);
            width = bmp.Width;
            height = bmp.Height;
            length = width * height;
        }
        public ImageFoker(string imageFile)
            : this(Image.FromFile(imageFile)) { }

        public int this[int index]
        {
            get => bmp.GetPixel(index % width, index / width).ToArgb();
            set => bmp.SetPixel(index % width, index / width, Color.FromArgb(value));
        }


        static int SetBit(int value, bool bit, int index)
            => bit ? value | (1 << index) : value & ~(1 << index);

        static bool GetBit(int value, int index)
            => (value & (1 << index)) != 0;


        int ReadInt(ref int index, int count = 32)
        {
            int value = 0;
            for (int i = 0; i < count; ++i)
                value = SetBit(value, GetBit(this[(index + i) / 3], ((index + i) % 3) switch
                {
                    0 => indexR + indexOffset,
                    1 => indexG + indexOffset,
                    2 => indexB + indexOffset,
                    _ => throw new ArgumentException("Somthing is wrong!")
                }), i);
            index += count;
            return value;
        }

        int ReadLength(ref int index)
        {
            /*
            int len = 0;
            for (int i = 0; i < lengthBitsCount / 3; i++)
            {
                len = SetBit(len, GetBit(this[i], indexR + indexOffset), i * 3 + 0);
                len = SetBit(len, GetBit(this[i], indexG + indexOffset), i * 3 + 1);
                len = SetBit(len, GetBit(this[i], indexB + indexOffset), i * 3 + 2);
            }
            return len;
            */
            return ReadInt(ref index, lengthBitCount);
        }


        void WriteInt(ref int index, int value, int count = 32)
        {
            for (int i = 0; i < count; ++i)
                this[(index + i) / 3] =
                    SetBit(this[(index + i) / 3], GetBit(value, i), ((index + i) % 3) switch
                    {
                        0 => indexR + indexOffset,
                        1 => indexG + indexOffset,
                        2 => indexB + indexOffset,
                        _ => throw new ArgumentException("Somthing is wrong!")
                    });
            index += count;
        }

        void WriteLength(ref int index, int length)
        {
            WriteInt(ref index, length, lengthBitCount);
        }


        public void WriteString(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            int index = startIndex;
            WriteLength(ref index, str.Length);
            foreach (var symbol in str)
                WriteInt(ref index, symbol, sizeof(char) * 8);
        }

        public string ReadString()
        {
            int index = startIndex;
            int len = ReadLength(ref index);
            if (0 > len || len > (length * 3) / 16)
                len = (length * 3 - lengthBitCount) / 16;
            var sb = new StringBuilder(len);
            for (int i = 0; i < len; ++i)
                sb.Append((char)ReadInt(ref index, sizeof(char) * 8));
            return sb.ToString();
        }

        public void SaveImage(string fileName = null)
        {
            if (fileName == null)
                fileName = DateTime.UtcNow.Ticks.ToString("X") + ".bmp";
            bmp.Save(fileName);
        }

        public static void WriteString(string value, string inpImage, string outImage = null)
        {
            using var img = new ImageFoker(inpImage);
            img.WriteString(value);
            if (outImage == null)
                outImage = inpImage.Insert(inpImage.LastIndexOf('.'), "_changed");
            img.SaveImage(outImage);
        }

        public static string ReadString(string imageFile)
        {
            using var img = new ImageFoker(imageFile);
            var value = img.ReadString();
            return value;
        }

        public static string ReadStringToEnd(string imageFile)
        {
            using var img = new ImageFoker(imageFile);
            int tmp = 0;
            img.WriteLength(ref tmp, (img.length * 3 - lengthBitCount) / 16);
            var value = img.ReadString();
            return value;
        }

        public void Dispose()
        {
            bmp.Dispose();
        }
    }
}