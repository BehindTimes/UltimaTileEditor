using System;
using System.Collections.Generic;
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
    }
}
