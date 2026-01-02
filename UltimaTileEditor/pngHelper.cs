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

        private Color GetCGAColor(byte curByte)
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

        private byte GetByte(Color curColor)
        {
            int retval = Array.IndexOf(color_array, curColor);
            if (retval < 0)
            {
                retval = 0;
            }
            return (byte)retval;
        }

        private byte GetCGAByte(Color curColor)
        {
            int retval = Array.IndexOf(cga_color_array, curColor);
            if (retval < 0)
            {
                retval = 0;
            }
            return (byte)retval;
        }

        

        public void LoadImageU1_8(byte[] file_bytes, Bitmap b, int width, int height)
        {
            int pos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    for (int row = 0; row < 8; row++)
                    {
                        byte[] rowval = new byte[8];
                        int blue = (file_bytes[pos + 0]);
                        int green = (file_bytes[pos + 1]);
                        int red = (file_bytes[pos + 2]);
                        int intensity = (file_bytes[pos + 3]);
                        pos += 4;

                        for (int i = 0; i < 8; i++)
                        {
                            rowval[i] += (byte)(((blue >> (7 - i)) & 0x01) << 0);
                            rowval[i] += (byte)(((green >> (7 - i)) & 0x01) << 1);
                            rowval[i] += (byte)(((red >> (7 - i)) & 0x01) << 2);
                            rowval[i] += (byte)(((intensity >> (7 - i)) & 0x01) << 3);
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            Color pixColor1 = GetColor(rowval[i]);
                            b.SetPixel(indexX * 8 + i, indexY * 8 + row, pixColor1);
                        }
                    }
                }
            }
        }

        public void LoadImageU1(byte[] file_bytes, Bitmap b, int width, int height)
        {
            int pos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    for (int row = 0; row < 16; row++)
                    {
                        byte[] rowval = new byte[16];
                        int blue = (file_bytes[pos + 0] << 8) + (file_bytes[pos + 1]);
                        int green = (file_bytes[pos + 2] << 8) + (file_bytes[pos + 3]);
                        int red = (file_bytes[pos + 4] << 8) + (file_bytes[pos + 5]);
                        int intensity = (file_bytes[pos + 6] << 8) + (file_bytes[pos + 7]);
                        pos += 8;

                        for (int i = 0; i < 16; i++)
                        {
                            rowval[i] += (byte)(((blue >> (15 - i)) & 0x01) << 0);
                            rowval[i] += (byte)(((green >> (15 - i)) & 0x01) << 1);
                            rowval[i] += (byte)(((red >> (15 - i)) & 0x01) << 2);
                            rowval[i] += (byte)(((intensity >> (15 - i)) & 0x01) << 3);
                        }
                        for (int i = 0; i < 16; i++)
                        {
                            Color pixColor1 = GetColor(rowval[i]);
                            b.SetPixel(indexX * 16 + i, indexY * 16 + row, pixColor1);
                        }
                    }
                }
            }
        }

        public void LoadImageU2(byte[] file_bytes, Bitmap b)
        {
            int offset = 0;
            for (int tile_indexY = 0; tile_indexY < 8; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < 8; tile_indexX++)
                {
                    int curTile = tile_indexY * 8 + tile_indexX;
                    offset += 2;
                    for (int y_index = 0; y_index < 16; y_index++)
                    {
                        for (int x_index = 0; x_index < 4; ++x_index)
                        {
                            byte curData = file_bytes[(y_index * 4) + x_index + (curTile * 64) + offset];
                            int tempX = 0;
                            for (int shift_index = 6; shift_index >= 0; shift_index -= 2)
                            {
                                byte tempbytes = (byte)(curData >> shift_index & 0b11);
                                Color curColor = GetCGAColor(tempbytes);
                                b.SetPixel(((x_index * 4) + tempX) + (tile_indexX * 16), y_index + (tile_indexY * 16), curColor);
                                tempX++;
                            }
                        }
                    }
                }
            }
        }

        public void LoadImageU3(byte[] file_bytes, Bitmap b)
        {
            int x_offset = 0;
            int y_offset = 0;

            for (int index = 0; index < 0x1400; index += 0x40)
            {
                if (index >= 0xA00)
                {
                    y_offset = 16;
                }

                x_offset %= 40;

                for (int y_index = 0; y_index < 8; ++y_index)
                {
                    for (int x_index = 0; x_index < 4; ++x_index)
                    {
                        byte tempbyte1 = file_bytes[index + 4 * y_index + x_index];
                        byte tempbyte2 = file_bytes[index + 4 * y_index + x_index + 0x20];

                        for(int byte_index = 0; byte_index < 8; byte_index += 2)
                        {
                            int b1 = tempbyte1 >> (6 - byte_index) & 0x03;
                            int b2 = tempbyte2 >> (6 - byte_index) & 0x03;

                            byte b3 = (byte)((b1));
                            Color pixColor1 = GetCGAColor(b3);

                            byte b4 = (byte)((b2));
                            Color pixColor2 = GetCGAColor(b4);

                            int xxx = (x_index * 4) + (byte_index / 2) + (x_offset * 16);
                            int yyy = (y_index * 2) + y_offset;

                            b.SetPixel((x_index * 4) + (byte_index / 2) + (x_offset * 16), (y_index * 2) + y_offset, pixColor1);
                            b.SetPixel((x_index * 4) + (byte_index / 2) + (x_offset * 16), (y_index * 2) + y_offset + 1, pixColor2);
                        }
                    }
                }
                x_offset++;
            }
        }

        public void LoadImageU4(byte[] file_bytes, Bitmap b)
        {
            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 16; ++x_index)
                {
                    long cur_tile = (y_index * 16 + x_index) * 128;
                    for (int pix_y_index = 0; pix_y_index < 16; ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < 8; ++pix_x_index)
                        {
                            byte cur_byte = file_bytes[cur_tile + ((pix_y_index * 8) + pix_x_index)];
                            byte b1 = (byte)((cur_byte >> 4) & 0xF);
                            byte b2 = (byte)(cur_byte & 0xF);

                            Color pixColor1 = GetColor(b1);
                            Color pixColor2 = GetColor(b2);

                            b.SetPixel((x_index * 16) + pix_x_index * 2, (y_index * 16) + pix_y_index, pixColor1);
                            b.SetPixel((x_index * 16) + pix_x_index * 2 + 1, (y_index * 16) + pix_y_index, pixColor2);
                        }
                    }
                }
            }
        }

        public void LoadImageU5(byte[] file_bytes, Bitmap b)
        {
            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 32; ++x_index)
                {
                    long cur_tile = (y_index * 32 + x_index) * 128;
                    for (int pix_y_index = 0; pix_y_index < 16; ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < 8; ++pix_x_index)
                        {
                            byte cur_byte = file_bytes[cur_tile + ((pix_y_index * 8) + pix_x_index)];
                            byte b1 = (byte)((cur_byte >> 4) & 0xF);
                            byte b2 = (byte)(cur_byte & 0xF);

                            Color pixColor1 = GetColor(b1);
                            Color pixColor2 = GetColor(b2);

                            b.SetPixel((x_index * 16) + pix_x_index * 2, (y_index * 16) + pix_y_index, pixColor1);
                            b.SetPixel((x_index * 16) + pix_x_index * 2 + 1, (y_index * 16) + pix_y_index, pixColor2);
                        }
                    }
                }
            }
        }

        public void MakePngU1(byte[] lzw, string strPng, int width, int height, bool is16)
        {
            try
            {
                byte[] file_bytes = lzw;
                int tilesize = 16;
                int file_mult = 128;
                if(!is16)
                {
                    tilesize = 8;
                    file_mult = 32;
                }
                if (file_bytes.Length != width * height * file_mult)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(width * tilesize, height * tilesize))
                {
                    if(is16)
                    {
                        LoadImageU1(file_bytes, b, width, height);
                    }
                    else
                    {
                        LoadImageU1_8(file_bytes, b, width, height);
                    }

                    b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public void MakePngU2(byte[] lzw, string strPng, int numTiles, int tileSize)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != tileSize * numTiles)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(16 * 8, 16 * 8))
                {
                    LoadImageU2(file_bytes, b);
                    b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public void MakePngU3(byte[] lzw, string strPng)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != 0x1400)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(640, 32))
                {
                    LoadImageU3(file_bytes, b);
                    b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public void MakePngU4(byte[] lzw, string strPng)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != 32768)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(256, 256))
                {
                    LoadImageU4(file_bytes, b);
                    b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public void MakePngU5(byte[] lzw, string strPng)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != 65536)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(512, 256))
                {
                    LoadImageU5(file_bytes, b);
                    b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public void WriteImageU1_8(byte[] file_bytes, Bitmap b, int width, int height)
        {
            int pos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    for (int row = 0; row < 8; row++)
                    {
                        byte[] rowval = new byte[8];
                        byte[] colorvals = new byte[8];

                        for (int i = 0; i < 8; i++)
                        {
                            Color pixColor1 = b.GetPixel(indexX * 8 + i, indexY * 8 + row);
                            colorvals[i] = (byte)GetByte(pixColor1);
                        }

                        int blue = 0;
                        int green = 0;
                        int red = 0;
                        int intensity = 0;

                        for (int i = 0; i < 8; i++)
                        {
                            blue += (byte)(((colorvals[i] >> 0) & 0x01));
                            green += (byte)(((colorvals[i] >> 1) & 0x01));
                            red += (byte)(((colorvals[i] >> 2) & 0x01));
                            intensity += (byte)(((colorvals[i] >> 3) & 0x01));

                            if (i < 7)
                            {
                                blue <<= 1;
                                green <<= 1;
                                red <<= 1;
                                intensity <<= 1;
                            }
                        }

                        file_bytes[pos + 0] = (byte)(blue & 0xFF);
                        file_bytes[pos + 1] = (byte)(green & 0xFF);
                        file_bytes[pos + 2] = (byte)(red & 0xFF);
                        file_bytes[pos + 3] = (byte)(intensity & 0xFF);
                        pos += 4;
                    }
                }
            }
        }

        public void WriteImageU1(byte[] file_bytes, Bitmap b, int width, int height)
        {
            int pos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    for (int row = 0; row < 16; row++)
                    {
                        byte[] rowval = new byte[16];
                        byte[] colorvals = new byte[16];

                        for (int i = 0; i < 16; i++)
                        {
                            Color pixColor1 = b.GetPixel(indexX * 16 + i, indexY * 16 + row);
                            colorvals[i] = (byte)GetByte(pixColor1);
                        }

                        int blue = 0;
                        int green = 0;
                        int red = 0;
                        int intensity = 0;

                        for (int i = 0; i < 16; i++)
                        {
                            blue += (byte)(((colorvals[i] >> 0) & 0x01));
                            green += (byte)(((colorvals[i] >> 1) & 0x01));
                            red += (byte)(((colorvals[i] >> 2) & 0x01));
                            intensity += (byte)(((colorvals[i] >> 3) & 0x01));

                            if(i < 15)
                            {
                                blue <<= 1;
                                green <<= 1;
                                red <<= 1;
                                intensity <<= 1;
                            }
                        }

                        file_bytes[pos + 0] = (byte)((blue >> 8) & 0xFF);
                        file_bytes[pos + 1] = (byte)(blue & 0xFF);
                        file_bytes[pos + 2] = (byte)((green >> 8) & 0xFF);
                        file_bytes[pos + 3] = (byte)(green & 0xFF);
                        file_bytes[pos + 4] = (byte)((red >> 8) & 0xFF);
                        file_bytes[pos + 5] = (byte)(red & 0xFF);
                        file_bytes[pos + 6] = (byte)((intensity >> 8) & 0xFF);
                        file_bytes[pos + 7] = (byte)(intensity & 0xFF);
                        pos += 8;
                    }
                }
            }
        }

        public void WriteImageU2(byte[] file_bytes, Bitmap b)
        {
            int tile_size = 66;
            byte NUM_COL = 0x04;
            byte NUM_ROW = 0x10;

            for (int tile_indexY = 0; tile_indexY < 8; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < 8; tile_indexX++)
                {
                    int curTile = tile_indexY * 8 + tile_indexX;
                    file_bytes[curTile * tile_size] = NUM_COL;     // Write the width size
                    file_bytes[curTile * tile_size + 1] = NUM_ROW; // Write the height size

                    for (int row = 0; row < NUM_ROW; row++)
                    {
                        for (int col = 0; col < NUM_COL; col++)
                        {
                            Color color1 = b.GetPixel(col * 4 + 0 + (tile_indexX * 16), row + (tile_indexY * 16));
                            Color color2 = b.GetPixel(col * 4 + 1 + (tile_indexX * 16), row + (tile_indexY * 16));
                            Color color3 = b.GetPixel(col * 4 + 2 + (tile_indexX * 16), row + (tile_indexY * 16));
                            Color color4 = b.GetPixel(col * 4 + 3 + (tile_indexX * 16), row + (tile_indexY * 16));

                            byte b1 = GetCGAByte(color1);
                            byte b2 = GetCGAByte(color2);
                            byte b3 = GetCGAByte(color3);
                            byte b4 = GetCGAByte(color4);

                            byte finalbyte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + (b4 << 0));

                            file_bytes[curTile * tile_size + 2 + (row * NUM_COL) + col] = finalbyte;
                        }
                    }
                }
            }
        }

        public void WriteImageU3(byte[] file_bytes, Bitmap b)
        {
            int filepos = 0;
            for (int curTileIndexY = 0; curTileIndexY < 2; curTileIndexY++)
            {
                for (int curTileIndexX = 0; curTileIndexX < 40; curTileIndexX++)
                {
                    for (int tileHeight = 0; tileHeight < 8; tileHeight++)
                    {
                        for (int plane = 0; plane < 2; plane++)
                        {
                            for (int tileWidth = 0; tileWidth < 4; tileWidth++)
                            {
                                int posX1 = (tileWidth * 4) + 0 + (curTileIndexX * 16);
                                int posX2 = (tileWidth * 4) + 1 + (curTileIndexX * 16);
                                int posX3 = (tileWidth * 4) + 2 + (curTileIndexX * 16);
                                int posX4 = (tileWidth * 4) + 3 + (curTileIndexX * 16);

                                int posY = (curTileIndexY * 16) + (tileHeight * 2) + plane;

                                Color pos11 = b.GetPixel(posX1, posY);
                                Color pos12 = b.GetPixel(posX2, posY);
                                Color pos13 = b.GetPixel(posX3, posY);
                                Color pos14 = b.GetPixel(posX4, posY);

                                byte b1 = GetCGAByte(pos11);
                                byte b2 = GetCGAByte(pos12);
                                byte b3 = GetCGAByte(pos13);
                                byte b4 = GetCGAByte(pos14);

                                byte finalbyte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + (b4 << 0));
                                int temppos = (tileHeight * 4) + tileWidth + filepos + (plane * 0x20);
                                file_bytes[temppos] = finalbyte;
                            }
                        }
                    }
                    filepos += 0x40;
                }
            }
        }

        public void WriteImageU4(byte[] file_bytes, Bitmap b)
        {
            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 16; ++x_index)
                {
                    long cur_tile = (y_index * 16 + x_index) * 128;
                    for (int pix_y_index = 0; pix_y_index < 16; ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < 16; pix_x_index += 2)
                        {
                            Color color1 = b.GetPixel((x_index * 16) + pix_x_index, (y_index * 16) + pix_y_index);
                            Color color2 = b.GetPixel((x_index * 16) + pix_x_index + 1, (y_index * 16) + pix_y_index);

                            byte b1 = (byte)((GetByte(color1) << 4) & 0xF0);
                            byte b2 = (byte)(GetByte(color2) & 0x0F);

                            byte outbyte = (byte)(b1 + b2);
                            file_bytes[cur_tile + ((pix_y_index * 8) + (pix_x_index / 2))] = outbyte;
                        }
                    }
                }
            }
        }

        public void WriteImageU5(byte[] file_bytes, Bitmap b)
        {
            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 32; ++x_index)
                {
                    long cur_tile = (y_index * 32 + x_index) * 128;
                    for (int pix_y_index = 0; pix_y_index < 16; ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < 16; pix_x_index += 2)
                        {
                            Color color1 = b.GetPixel((x_index * 16) + pix_x_index, (y_index * 16) + pix_y_index);
                            Color color2 = b.GetPixel((x_index * 16) + pix_x_index + 1, (y_index * 16) + pix_y_index);

                            byte b1 = (byte)((GetByte(color1) << 4) & 0xF0);
                            byte b2 = (byte)(GetByte(color2) & 0x0F);

                            byte outbyte = (byte)(b1 + b2);
                            file_bytes[cur_tile + ((pix_y_index * 8) + (pix_x_index / 2))] = outbyte;
                        }
                    }
                }
            }
        }

        public void MakeU1(out byte[]? file_bytes, string strPng, int width, int height, bool is16)
        {
            int tilesize = 16;
            int file_mult = 128;
            if (!is16)
            {
                tilesize = 8;
                file_mult = 32;
            }

            file_bytes = null;
            try
            {
                byte[] destination = new byte[width * height * file_mult];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != height * tilesize && image.Width != width * tilesize)
                {
                    Console.WriteLine("Image must be {0}x{1} pixels!", width * tilesize, height * tilesize);
                    return;
                }
                if(is16)
                {
                    WriteImageU1(destination, image, width, height);
                }
                else
                {
                    WriteImageU1_8(destination, image, width, height);
                }
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public void MakeU2(out byte[]? file_bytes, string strPng)
        {
            int tileSize = 66;
            int numTiles = 64;

            file_bytes = null;
            try
            {
                byte[] destination = new byte[tileSize * numTiles];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 128 && image.Width != 128)
                {
                    Console.WriteLine("Image must be 128x128 pixels!");
                    return;
                }
                WriteImageU2(destination, image);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public void MakeU3(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[160 * 32];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 32 && image.Width != 640)
                {
                    Console.WriteLine("Image must be 640x32 pixels!");
                    return;
                }
                WriteImageU3(destination, image);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public void MakeU4(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[128 * 256];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 256 && image.Width != 256)
                {
                    Console.WriteLine("Image must be 256x256 pixels!");
                    return;
                }
                WriteImageU4(destination, image);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public void MakeLZWU5(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[256 * 256];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 256 && image.Width != 512)
                {
                    Console.WriteLine("Image must be 512x256 pixels!");
                    return;
                }
                WriteImageU5(destination, image);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }
    }
}
