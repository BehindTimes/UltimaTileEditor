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

        public byte GetCGAByte(Color curColor)
        {
            int retval = Array.IndexOf(cga_color_array, curColor);
            if (retval < 0)
            {
                retval = 0;
            }
            return (byte)retval;
        }
    }
}
