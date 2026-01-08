using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima3ImageExtractor
    {
        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            pngHelper helper = new pngHelper();
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("SHAPES.ULT"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null && file_bytes.Length == 5120)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            MakePngU3(file_bytes, fullPath);
                        }
                    }
                }
                else if(image.EndsWith("CHARSET.ULT"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length == 2048)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            MakeCharsetPngU3(file_bytes, fullPath);
                        }
                    }
                }
                else if (imageType == 2)
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 0x4000;
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes.Length != fileSize)
                        {
                            return;
                        }
                        string fullPath = Path.Combine(strImageDir, value + ".png");
                        helper.MakeU2Pic(file_bytes, fullPath);
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            pngHelper helper = new pngHelper();
            bool written = false;

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    MakeU3(out file_bytes, image, 16, 16, 40, 2);

                    if (file_bytes != null && file_bytes.Length == 5120)
                    {
                        string fullPath = Path.Combine(strDataDir, "SHAPES.ULT");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("CHARSET.png"))
                {
                    byte[]? file_bytes;
                    MakeU3(out file_bytes, image, 8, 8, 16, 8);

                    if (file_bytes != null && file_bytes.Length == 2048)
                    {
                        string fullPath = Path.Combine(strDataDir, "CHARSET.ULT");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (imageType == 2)
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        byte[] outData;
                        int fileSize = 0x4000;
                        bool write0x4000 = false; ;
                        if (value.EndsWith("PICDRA"))
                        {
                            fileSize = 0x4080; // ? Is this an error, or is there a reason for this?
                            write0x4000 = true;
                        }
                        outData = new byte[fileSize];
                        if (write0x4000)
                        {
                            outData[0x4000] = 0x1a; // ? Is this a mistake on thier part, or what is the actual reason for the extra 128 bytes?
                        }
                        string fullPath = Path.Combine(strDataDir, value + ".IBM");

                        helper.MakeU2PicData(ref outData, image, fullPath);
                        written = true;
                    }
                }
            }
            if (written)
            {
                MessageBox.Show("File written!");
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
                    LoadImageU3(file_bytes, b, 16, 16, 40, 2);
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

        public void MakeCharsetPngU3(byte[] lzw, string strPng)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != 2048)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(128, 64))
                {
                    LoadImageU3(file_bytes, b, 8, 8, 16, 8);
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

        public void LoadImageU3(byte[] file_bytes, Bitmap b, int tilewidth, int tileheight, int numcolumns, int numrows)
        {
            int tilesize = tilewidth * tileheight / 4;

            pngHelper helper = new pngHelper();
            int x_offset = 0;
            int y_offset = 0;

            int rowcount = 0;
            int rowsize = tilesize * numcolumns;

            for (int index = 0; index < file_bytes.Length; index += tilesize, rowcount += tilesize)
            {
                if (rowcount >= rowsize)
                {
                    y_offset += tileheight;
                    rowcount -= rowsize;
                }

                x_offset %= numcolumns;

                for (int y_index = 0; y_index < tileheight / 2; ++y_index)
                {
                    for (int x_index = 0; x_index < tilewidth / 4; ++x_index)
                    {
                        byte tempbyte1 = file_bytes[index + (tilewidth / 4) * y_index + x_index];
                        byte tempbyte2 = file_bytes[index + (tilewidth / 4) * y_index + x_index + (tilesize / 2)];

                        for (int byte_index = 0; byte_index < 8; byte_index += 2)
                        {
                            int b1 = tempbyte1 >> (6 - byte_index) & 0x03;
                            int b2 = tempbyte2 >> (6 - byte_index) & 0x03;

                            byte b3 = (byte)((b1));
                            Color pixColor1 = helper.GetCGAColor(b3);

                            byte b4 = (byte)((b2));
                            Color pixColor2 = helper.GetCGAColor(b4);

                            b.SetPixel((x_index * 4) + (byte_index / 2) + (x_offset * tilewidth), (y_index * 2) + y_offset, pixColor1);
                            b.SetPixel((x_index * 4) + (byte_index / 2) + (x_offset * tilewidth), (y_index * 2) + y_offset + 1, pixColor2);
                        }
                    }
                }
                x_offset++;
            }
        }

        public void WriteImageU3(byte[] file_bytes, Bitmap b, int twidth, int theight, int numcolumns, int numrows)
        {
            pngHelper helper = new pngHelper();
            int datasize = twidth * theight / 4;
            int planesize = datasize / 2;
            int filepos = 0;
            for (int curTileIndexY = 0; curTileIndexY < numrows; curTileIndexY++)
            {
                for (int curTileIndexX = 0; curTileIndexX < numcolumns; curTileIndexX++)
                {
                    for (int tileHeight = 0; tileHeight < (theight / 2); tileHeight++)
                    {
                        for (int plane = 0; plane < 2; plane++)
                        {
                            for (int tileWidth = 0; tileWidth < (twidth / 4); tileWidth++)
                            {
                                int posX1 = (tileWidth * 4) + 0 + (curTileIndexX * twidth);
                                int posX2 = (tileWidth * 4) + 1 + (curTileIndexX * twidth);
                                int posX3 = (tileWidth * 4) + 2 + (curTileIndexX * twidth);
                                int posX4 = (tileWidth * 4) + 3 + (curTileIndexX * twidth);

                                int posY = (curTileIndexY * theight) + (tileHeight * 2) + plane;

                                Color pos11 = b.GetPixel(posX1, posY);
                                Color pos12 = b.GetPixel(posX2, posY);
                                Color pos13 = b.GetPixel(posX3, posY);
                                Color pos14 = b.GetPixel(posX4, posY);

                                byte b1 = helper.GetCGAByte(pos11);
                                byte b2 = helper.GetCGAByte(pos12);
                                byte b3 = helper.GetCGAByte(pos13);
                                byte b4 = helper.GetCGAByte(pos14);

                                byte finalbyte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + (b4 << 0));
                                int temppos = (tileHeight * (twidth / 4)) + tileWidth + filepos + (plane * planesize);
                                file_bytes[temppos] = finalbyte;
                            }
                        }
                    }
                    filepos += datasize;
                }
            }
        }

        public void MakeU3(out byte[]? file_bytes, string strPng, int tilewidth, int tileheight, int numcolumns, int numrows)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[numcolumns * (tilewidth / 4) * numrows * tileheight];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != numrows * tileheight && image.Width != numcolumns * tilewidth)
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", numcolumns * tilewidth, numrows * tileheight);
                    return;
                }
                WriteImageU3(destination, image, tilewidth, tileheight, numcolumns, numrows);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }
    }
}
