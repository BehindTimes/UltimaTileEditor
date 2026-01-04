using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima1ImageExtractor
    {
        public void ExtractImages(string[] images, string strOutDir)
        {
            foreach (string image in images)
            {
                if (image.EndsWith("TILES.BIN"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TILES.png");
                        MakePngU1(file_bytes, fullPath, 13, 4, true);
                    }
                }
                else if (image.EndsWith("MOND.BIN"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "MOND.png");
                        MakePngU1(file_bytes, fullPath, 19, 1, true);
                    }
                }
                else if (image.EndsWith("TOWN.BIN"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TOWN.png");
                        MakePngU1(file_bytes, fullPath, 17, 3, false);
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strOutDir)
        {
            foreach (string image in images)
            {
                if (image.EndsWith("TILES.png"))
                {
                    byte[]? file_bytes;
                    MakeU1(out file_bytes, image, 13, 4, true);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "EGATILES.BIN");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
                else if (image.EndsWith("MOND.png"))
                {
                    byte[]? file_bytes;
                    MakeU1(out file_bytes, image, 19, 1, true);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "EGAMOND.BIN");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
                else if (image.EndsWith("TOWN.png"))
                {
                    byte[]? file_bytes;
                    MakeU1(out file_bytes, image, 17, 3, false);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "EGATOWN.BIN");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
            }
            MessageBox.Show("Files written!");
        }

        public void LoadImageU1_8(byte[] file_bytes, Bitmap b, int width, int height)
        {
            pngHelper helper = new pngHelper();
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
                            Color pixColor1 = helper.GetColor(rowval[i]);
                            b.SetPixel(indexX * 8 + i, indexY * 8 + row, pixColor1);
                        }
                    }
                }
            }
        }

        public void LoadImageU1(byte[] file_bytes, Bitmap b, int width, int height)
        {
            pngHelper helper = new pngHelper();
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
                            Color pixColor1 = helper.GetColor(rowval[i]);
                            b.SetPixel(indexX * 16 + i, indexY * 16 + row, pixColor1);
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
                if (!is16)
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
                    if (is16)
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

        public void WriteImageU1_8(byte[] file_bytes, Bitmap b, int width, int height)
        {
            pngHelper helper = new pngHelper();
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
                            colorvals[i] = (byte)helper.GetByte(pixColor1);
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
            pngHelper helper = new pngHelper();
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
                            colorvals[i] = (byte)helper.GetByte(pixColor1);
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

                            if (i < 15)
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
                if (is16)
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
    }
}
