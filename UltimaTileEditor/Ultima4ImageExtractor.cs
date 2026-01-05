using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima4ImageExtractor
    {
        public void ExtractImages(string[] images, string strOutDir)
        {
            foreach (string image in images)
            {
                if (image.EndsWith("SHAPES.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "SHAPES.png");
                        MakePngU4(file_bytes, fullPath);
                    }
                }
                else
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        string[] compressed_files = { "ABACUS", "ANIMATE", "GYPSY", "HONCOM", "INSIDE", "OUTSIDE",
                            "PORTAL", "SACHONOR", "SPIRHUM", "TITLE", "TREE", "VALJUS", "WAGON" };
                        if (compressed_files.Contains(value))
                        {
                            LzwDecompressor lzw = new LzwDecompressor();

                            byte[] file_bytes = File.ReadAllBytes(image);
                            byte[]? lzw_out;
                            lzw.ExtractU4(file_bytes, out lzw_out);
                            if (lzw_out != null && lzw_out.Length == 32000)
                            {
                                string fullPath = Path.Combine(strOutDir, value + ".png");

                                using (Bitmap b = new Bitmap(320, 200))
                                {
                                    LoadImage320x200(lzw_out, b);
                                    b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                    Console.WriteLine("Image Created");
                                }
                            }
                        }
                        else
                        {
                            
                        }
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strOutDir)
        {
            foreach (string image in images)
            {
                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    MakeU4(out file_bytes, image);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "SHAPES.EGA");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
                else
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        if (value != null)
                        {
                            string[] compressed_files = { "ABACUS", "ANIMATE", "GYPSY", "HONCOM", "INSIDE", "OUTSIDE",
                            "PORTAL", "SACHONOR", "SPIRHUM", "TITLE", "TREE", "VALJUS", "WAGON" };
                            if (compressed_files.Contains(value))
                            {
                                byte[]? file_bytes;
                                MakeU4Lzw(out file_bytes, image);
                                if ((null != file_bytes))
                                {
                                    LzwDecompressor lzw = new LzwDecompressor();

                                    string fullPath = Path.Combine(strOutDir, value + "_test.EGA");
                                    lzw.CompressU4Lzw(file_bytes, fullPath);
                                }
                            }
                        }
                    }
                }
            }
            MessageBox.Show("File written!");
        }

        public void MakeU4Lzw(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[200 * 160];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Console.WriteLine("Image must be 320x200 pixels!");
                    return;
                }
                WriteLzwImageU4(destination, image, 320, 200);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private void LoadImage320x200(byte[] file_bytes, Bitmap b)
        {
            pngHelper helper = new pngHelper();

            for (int y_index = 0; y_index < 200; ++y_index)
            {
                for (int x_index = 0; x_index < 160; ++x_index)
                {
                    byte cur_byte = file_bytes[y_index * 160 + x_index];
                    byte b1 = (byte)((cur_byte >> 4) & 0xF);
                    byte b2 = (byte)(cur_byte & 0xF);

                    Color pixColor1 = helper.GetColor(b1);
                    Color pixColor2 = helper.GetColor(b2);

                    b.SetPixel(x_index * 2, y_index, pixColor1);
                    b.SetPixel(x_index * 2 + 1, y_index, pixColor2);
                }
            }
        }

        public void LoadImageU4(byte[] file_bytes, Bitmap b)
        {
            pngHelper helper = new pngHelper();

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

                            Color pixColor1 = helper.GetColor(b1);
                            Color pixColor2 = helper.GetColor(b2);

                            b.SetPixel((x_index * 16) + pix_x_index * 2, (y_index * 16) + pix_y_index, pixColor1);
                            b.SetPixel((x_index * 16) + pix_x_index * 2 + 1, (y_index * 16) + pix_y_index, pixColor2);
                        }
                    }
                }
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

        private void WriteTilesU4(byte[] file_bytes, Bitmap b)
        {
            pngHelper helper = new pngHelper();

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

                            byte b1 = (byte)((helper.GetByte(color1) << 4) & 0xF0);
                            byte b2 = (byte)(helper.GetByte(color2) & 0x0F);

                            byte outbyte = (byte)(b1 + b2);
                            file_bytes[cur_tile + ((pix_y_index * 8) + (pix_x_index / 2))] = outbyte;
                        }
                    }
                }
            }
        }

        private void WriteLzwImageU4(byte[] file_bytes, Bitmap b, int width, int height)
        {
            pngHelper helper = new pngHelper();
            int curByte = 0;

            for (int y_index = 0; y_index < height; ++y_index)
            {
                for (int x_index = 0; x_index < width; x_index += 2)
                {
                    Color color1 = b.GetPixel(x_index, y_index);
                    Color color2 = b.GetPixel(x_index + 1, y_index);

                    byte b1 = (byte)((helper.GetByte(color1) << 4) & 0xF0);
                    byte b2 = (byte)(helper.GetByte(color2) & 0x0F);

                    byte outbyte = (byte)(b1 + b2);
                    file_bytes[curByte] = outbyte;
                    curByte++;
                }
            }
        }

        private void MakeU4(out byte[]? file_bytes, string strPng)
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
                WriteTilesU4(destination, image);
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
