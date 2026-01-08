using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UltimaTileEditor
{
    internal class pngHelper
    {
        Color[] color_array = {
            Color.FromArgb(0, 0, 0),            //black
            Color.FromArgb(0, 0, 0xAA),         //blue
            Color.FromArgb(0, 0xAA, 0),         //green
            Color.FromArgb(0, 0xAA, 0xAA),      //cyan
            Color.FromArgb(0xAA, 0, 0),         //red
            Color.FromArgb(0xAA, 0, 0xAA),      //magenta
            Color.FromArgb(0xAA, 0x55, 0),      //brown
            Color.FromArgb(0xAA, 0xAA, 0xAA),   //light gray
            Color.FromArgb(0x55, 0x55, 0x55),   //dark gray
            Color.FromArgb(0x55, 0x55, 0xFF),   //bright blue
            Color.FromArgb(0x55, 0xFF, 0x55),   //bright green
            Color.FromArgb(0x55, 0xFF, 0xFF),   //bright cyan
            Color.FromArgb(0xFF, 0x55, 0x55),   //bright red
            Color.FromArgb(0xFF, 0x55, 0xFF),   //bright magenta
            Color.FromArgb(0xFF, 0xFF, 0x55),   //bright yellow
            Color.FromArgb(0xFF, 0xFF, 0xFF)    //white
        };

        // https://moddingwiki.shikadi.net/wiki/Ultima_I_Full_Screen_Graphic_Format
        Color[] color_array_u1 = {
            Color.FromArgb(0, 0, 0),            //black
            Color.FromArgb(0x55, 0x55, 0x55),   //dark gray
            Color.FromArgb(0xAA, 0xAA, 0xAA),   //light gray
            Color.FromArgb(0xFF, 0xFF, 0xFF),   //white
            Color.FromArgb(0xFF, 0xFF, 0xFF),   //white
            Color.FromArgb(0, 0xAA, 0),         //green
            Color.FromArgb(0x55, 0xFF, 0x55),   //bright green
            Color.FromArgb(0xAA, 0x55, 0),      //brown
            Color.FromArgb(0xFF, 0x55, 0x55),   //bright red
            Color.FromArgb(0xAA, 0xAA, 0xAA),   //light gray
            Color.FromArgb(0xFF, 0x55, 0xFF),   //bright magenta
            Color.FromArgb(0, 0xAA, 0xAA),      //cyan
            Color.FromArgb(0, 0, 0xAA),         //blue
            Color.FromArgb(0x55, 0x55, 0xFF),   //bright blue
            Color.FromArgb(0xAA, 0xAA, 0xAA),   //light gray
            Color.FromArgb(0xFF, 0xFF, 0xFF)    //white
        };

        Color[] cga_color_array = {
            Color.FromArgb(0, 0, 0),            //black
            Color.FromArgb(0, 0xAA, 0xAA),      //cyan
            Color.FromArgb(0xAA, 0, 0xAA),      //magenta
            Color.FromArgb(0xAA, 0xAA, 0xAA)    //light gray
        };

        public Color GetCGAColor(byte curByte)
        {
            Color pixColor = Color.Black;

            if (curByte < 4)
            {
                pixColor = cga_color_array[curByte];
            }

            return pixColor;
        }

        public Color GetColorWithPalette(byte curByte, int palette)
        {
            Color pixColor = Color.Black;

            if (curByte < 16)
            {
                switch(palette)
                {
                    case 1: // Ultima 1
                        pixColor = color_array_u1[curByte];
                        break;
                    default: // EGA
                        pixColor = color_array[curByte];
                        break;
                }
            }

            return pixColor;
        }

        public Color GetColor(byte curByte)
        {
            Color pixColor = Color.Black;

            if (curByte < 16)
            {
                pixColor = color_array[curByte];
            }

            return pixColor;
        }

        public byte GetByte(Color curColor)
        {
            int retval = Array.IndexOf(color_array, curColor);
            if (retval < 0)
            {
                retval = 0;
            }
            return (byte)retval;
        }

        public byte GetByteIgnoreAlpha(Color curColor)
        {
            Color tempColor = Color.FromArgb(curColor.R, curColor.G, curColor.B);
            int retval = Array.IndexOf(color_array, tempColor);
            if (retval < 0)
            {
                retval = 0;
            }
            return (byte)retval;
        }

        public byte GetByteWithPalette(Color curColor, int palette)
        {
            int retval = 0;
            switch (palette)
            {
                case 1: // Ultima 1
                    retval = Array.IndexOf(color_array_u1, curColor);
                    if (retval < 0)
                    {
                        retval = 0;
                    }
                    break;
                default:
                    retval = Array.IndexOf(color_array, curColor);
                    if (retval < 0)
                    {
                        retval = 0;
                    }
                    break;
            }
            
            return (byte)retval;
        }

        public byte GetCGAByte(Color curColor)
        {
            int retval = Array.IndexOf(cga_color_array, curColor);
            if (retval < 0)
            {
                retval = 0;
            }
            return (byte)retval;
        }

        public void LoadImage320x200(byte[] file_bytes, Bitmap b, int palette)
        {
            for (int y_index = 0; y_index < 200; ++y_index)
            {
                for (int x_index = 0; x_index < 160; ++x_index)
                {
                    byte cur_byte = file_bytes[y_index * 160 + x_index];
                    byte b1 = (byte)((cur_byte >> 4) & 0xF);
                    byte b2 = (byte)(cur_byte & 0xF);

                    Color pixColor1 = GetColorWithPalette(b1, palette);
                    Color pixColor2 = GetColorWithPalette(b2, palette);

                    b.SetPixel(x_index * 2, y_index, pixColor1);
                    b.SetPixel(x_index * 2 + 1, y_index, pixColor2);
                }
            }
        }

        public void CreateImage(byte[] file_bytes, Bitmap b, int width, int height)
        {
            int curByte = 0;

            for (int y_index = 0; y_index < height; ++y_index)
            {
                for (int x_index = 0; x_index < width; x_index += 2)
                {
                    Color color1 = b.GetPixel(x_index, y_index);
                    Color color2 = b.GetPixel(x_index + 1, y_index);

                    byte b1 = (byte)((GetByte(color1) << 4) & 0xF0);
                    byte b2 = (byte)(GetByte(color2) & 0x0F);

                    byte outbyte = (byte)(b1 + b2);
                    file_bytes[curByte] = outbyte;
                    curByte++;
                }
            }
        }

        public void CreateCGAImage(byte[] file_bytes, Bitmap b, int width, int height)
        {
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    byte curByte = file_bytes[indexY * width + indexX];

                    byte b1 = (byte)((curByte >> 6) & 0b11);
                    byte b2 = (byte)((curByte >> 4) & 0b11);
                    byte b3 = (byte)((curByte >> 2) & 0b11);
                    byte b4 = (byte)((curByte >> 0) & 0b11);

                    Color c1 = GetCGAColor(b1);
                    Color c2 = GetCGAColor(b2);
                    Color c3 = GetCGAColor(b3);
                    Color c4 = GetCGAColor(b4);

                    b.SetPixel(indexX * 4 + 0, indexY, c1);
                    b.SetPixel(indexX * 4 + 1, indexY, c2);
                    b.SetPixel(indexX * 4 + 2, indexY, c3);
                    b.SetPixel(indexX * 4 + 3, indexY, c4);
                }
            }
        }

        public void MakeCGAImage(out byte[]? file_bytes, string strPng, int width, int height)
        {
            file_bytes = null;
            try
            {
                int curPos = 0;
                byte[] destination = new byte[width * height];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Width != width * 4 && image.Height != height)
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", width * 4, height);
                    return;
                }
                for (int y_index = 0; y_index < height; y_index++)
                {
                    for (int x_index = 0; x_index < width; x_index++)
                    {
                        Color color1 = image.GetPixel(x_index * 4 + 0, y_index);
                        Color color2 = image.GetPixel(x_index * 4 + 1, y_index);
                        Color color3 = image.GetPixel(x_index * 4 + 2, y_index);
                        Color color4 = image.GetPixel(x_index * 4 + 3, y_index);

                        byte b1 = (byte)(GetCGAByte(color1));
                        byte b2 = (byte)(GetCGAByte(color2));
                        byte b3 = (byte)(GetCGAByte(color3));
                        byte b4 = (byte)(GetCGAByte(color4));

                        byte outByte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + b4);
                        destination[curPos] = outByte;
                        curPos++;
                    }
                }
                file_bytes = destination;
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public void CreateImageWithPalette(byte[] file_bytes, Bitmap b, int width, int height, int palette)
        {
            int curByte = 0;

            for (int y_index = 0; y_index < height; ++y_index)
            {
                for (int x_index = 0; x_index < width; x_index += 2)
                {
                    Color color1 = b.GetPixel(x_index, y_index);
                    Color color2 = b.GetPixel(x_index + 1, y_index);

                    byte b1 = (byte)((GetByteWithPalette(color1, palette) << 4) & 0xF0);
                    byte b2 = (byte)(GetByteWithPalette(color2, palette) & 0x0F);

                    byte outbyte = (byte)(b1 + b2);
                    file_bytes[curByte] = outbyte;
                    curByte++;
                }
            }
        }

        public void MakeU2Pic(byte[] file_bytes, string strPng)
        {
            using (Bitmap b = new Bitmap(320, 200))
            {
                const int planeSize = 0x2000;
                const int rowSize = 80;
                int curPos = 0;
                int numRows = 200;
                for (int planeIndex = 0; planeIndex < 2; planeIndex++)
                {
                    curPos = planeSize * planeIndex;

                    for (int indexY = 0; indexY < numRows; indexY += 2)
                    {
                        for (int index = 0; index < rowSize; index++)
                        {
                            byte curByte = file_bytes[curPos];
                            byte b1 = (byte)((curByte >> 6) & 0b11);
                            byte b2 = (byte)((curByte >> 4) & 0b11);
                            byte b3 = (byte)((curByte >> 2) & 0b11);
                            byte b4 = (byte)((curByte >> 0) & 0b11);

                            Color c1 = GetCGAColor(b1);
                            Color c2 = GetCGAColor(b2);
                            Color c3 = GetCGAColor(b3);
                            Color c4 = GetCGAColor(b4);

                            b.SetPixel(index * 4 + 0, indexY + planeIndex, c1);
                            b.SetPixel(index * 4 + 1, indexY + planeIndex, c2);
                            b.SetPixel(index * 4 + 2, indexY + planeIndex, c3);
                            b.SetPixel(index * 4 + 3, indexY + planeIndex, c4);

                            curPos++;
                        }
                    }
                }

                b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                System.Diagnostics.Debug.WriteLine("Image Created");
            }
        }

        private void WritePicU2(ref byte[] file_bytes, Bitmap b, string strOutFile)
        {
            const int planeSize = 0x2000;
            const int rowSize = 80;
            int curPos = 0;
            int numRows = 200;

            for (int planeIndex = 0; planeIndex < 2; planeIndex++)
            {
                curPos = planeSize * planeIndex;

                for (int indexY = 0; indexY < numRows; indexY += 2)
                {
                    for (int index = 0; index < rowSize; index++)
                    {
                        byte curByte = 0;
                        Color c1 = b.GetPixel(index * 4 + 0, indexY + planeIndex);
                        Color c2 = b.GetPixel(index * 4 + 1, indexY + planeIndex);
                        Color c3 = b.GetPixel(index * 4 + 2, indexY + planeIndex);
                        Color c4 = b.GetPixel(index * 4 + 3, indexY + planeIndex);

                        byte b1 = GetCGAByte(c1);
                        byte b2 = GetCGAByte(c2);
                        byte b3 = GetCGAByte(c3);
                        byte b4 = GetCGAByte(c4);

                        curByte += (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + b4);
                        file_bytes[curPos] = curByte;

                        curPos++;
                    }
                }
            }

            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strOutFile, FileMode.Create)))
            {
                binWriter.Write(file_bytes);
            }
        }

        public void MakeU2PicData(ref byte[] file_bytes, string strPng, string strOutFile)
        {
            try
            {
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be 320x200 pixels!");
                    return;
                }
                WritePicU2(ref file_bytes, image, strOutFile);
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }
    }
}
