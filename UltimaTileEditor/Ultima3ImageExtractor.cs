using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima3ImageExtractor
    {
        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("SHAPES.ULT"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null && file_bytes.Length == 5120)
                    {
                        string fullPath = Path.Combine(strImageDir, "SHAPES.png");
                        MakePngU3(file_bytes, fullPath);
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    MakeU3(out file_bytes, image);

                    if (file_bytes != null && file_bytes.Length == 5120)
                    {
                        string fullPath = Path.Combine(strDataDir, "SHAPES.ULT");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            MessageBox.Show("File written!");
                        }
                    }
                }
            }
        }

        public void LoadImageU3(byte[] file_bytes, Bitmap b)
        {
            pngHelper helper = new pngHelper();
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

                        for (int byte_index = 0; byte_index < 8; byte_index += 2)
                        {
                            int b1 = tempbyte1 >> (6 - byte_index) & 0x03;
                            int b2 = tempbyte2 >> (6 - byte_index) & 0x03;

                            byte b3 = (byte)((b1));
                            Color pixColor1 = helper.GetCGAColor(b3);

                            byte b4 = (byte)((b2));
                            Color pixColor2 = helper.GetCGAColor(b4);

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

        public void WriteImageU3(byte[] file_bytes, Bitmap b)
        {
            pngHelper helper = new pngHelper();
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

                                byte b1 = helper.GetCGAByte(pos11);
                                byte b2 = helper.GetCGAByte(pos12);
                                byte b3 = helper.GetCGAByte(pos13);
                                byte b4 = helper.GetCGAByte(pos14);

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
    }
}
