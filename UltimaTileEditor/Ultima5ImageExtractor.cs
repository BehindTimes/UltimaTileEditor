using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace UltimaTileEditor
{
    internal class Ultima5ImageExtractor
    {
        public void ExtractImages(string[] images, string strOutDir)
        {
            lzwDecompressor lzw = new lzwDecompressor();
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("TILES.16"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[]? lzw_out;
                    lzw.extract(file_bytes, out lzw_out);
                    if (lzw_out != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TILES.png");
                        helper.MakePngU5(lzw_out, fullPath);
                    }
                }
                else
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[]? lzw_out;
                    lzw.extract(file_bytes, out lzw_out);
                    if (lzw_out != null)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if(value != null)
                        {
                            if(value.ToLower().Contains("mon") || value.ToLower().Contains("items"))
                            {
                                CreateMaskImages(lzw_out, strOutDir, value);
                            }
                            else
                            {
                                CreateImages(lzw_out, strOutDir, value);
                            }                
                        }
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strOutDir)
        {
            lzwDecompressor lzw = new lzwDecompressor();
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("TILES.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeLZWU5(out file_bytes, image);

                    if(file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TILES.16");
                        lzw.compress(file_bytes, fullPath);
                    }         
                }
            }
        }

        private void CreateImages(byte[] lzw_out, string strOutDir, string name)
        {
            pngHelper helper = new pngHelper();

            int temp_offset = 2;
            int numImages = (lzw_out[1] << 8) + lzw_out[0];
            int[] offsets = new int[numImages];
            for (int index = 0; index < numImages; index++)
            {
                offsets[index] = (lzw_out[temp_offset + 3] << 24) + (lzw_out[temp_offset + 2] << 16) + (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 4;
            }
            for (int index = 0; index < numImages; index++)
            {
                temp_offset = offsets[index];
                int width = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;
                int height = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;

                int bufwid = (8 - (width % 8)) % 8;

                int data_size = (width + bufwid) * height / 2;

                try
                {
                    using (Bitmap b = new(width, height))
                    {
                        int temp_width = 0;
                        int temp_height = 0;
                        for(int temp_index = 0; temp_index < data_size; temp_index++)
                        {
                            byte curByte = lzw_out[temp_offset];
                            byte pixel1 = (byte)((curByte >> 4) & 0x0F);
                            byte pixel2 = (byte)(curByte & 0x0F);
                            Color color1 = helper.GetColor(pixel1);
                            Color color2 = helper.GetColor(pixel2);

                            if (temp_width < width)
                            {
                                b.SetPixel(temp_width, temp_height, color1);
                            }
                            temp_width++;
                            if(temp_width >= width + bufwid)
                            {
                                temp_width = 0;
                                temp_height++;
                            }
                            if (temp_width < width)
                            {
                                b.SetPixel(temp_width, temp_height, color2);
                            }
                            temp_width++;
                            if (temp_width >= width + bufwid)
                            {
                                temp_width = 0;
                                temp_height++;
                            }
                            temp_offset++;
                        }

                        string tempName = name + "_" + index.ToString() + ".png";
                        string fullPath = Path.Combine(strOutDir, tempName);
                        b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("LZW file does not exist!");
                    return;
                }
            }
        }

        private void AddMask(Bitmap b, byte[] lzw_out, int temp_offset, string name, string strOutDir)
        {
            int width = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
            temp_offset += 2;
            int height = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
            temp_offset += 2;
            if(width != b.Width)
            {
                MessageBox.Show("Invalid width!");
                return;
            }
            if (height != b.Height)
            {
                MessageBox.Show("Invalid height!");
                return;
            }

            // For masked images, all of them fall within the boundary, so this is never anything other than 0
            // Leaving it in incase of custom modifications, but I'm unsure how Ultima 5 would treat this.
            int bufwid = (8 - (width % 8)) % 8;

            int data_size = (width + bufwid) * height / 8;

            byte[] b8 = new byte[8];

            try
            {
                int temp_width = 0;
                int temp_height = 0;
                for (int temp_index = 0; temp_index < data_size; temp_index++)
                {
                    byte curByte = lzw_out[temp_offset];
                    for (int index = 0; index < 8; index++)
                    {
                        b8[index] = (byte)((curByte >> (7 - index)) & 0x01);
                    }
                    temp_offset++;

                    for (int index = 0; index < 8; index++)
                    {
                        if (temp_width < width)
                        {
                            if (b8[index] != 0) // Set the alpha to 0
                            {
                                Color color1 = b.GetPixel(temp_width, temp_height);
                                Color newColor = Color.FromArgb(0, color1.R, color1.G, color1.B);
                                b.SetPixel(temp_width, temp_height, newColor);
                            }
                        }
                        temp_width++;
                        if (temp_width >= width + bufwid)
                        {
                            temp_width = 0;
                            temp_height++;
                        }
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        private void CreateMaskImages(byte[] lzw_out, string strOutDir, string name)
        {
            pngHelper helper = new pngHelper();

            int temp_offset = 2;
            int numImages = (lzw_out[1] << 8) + lzw_out[0];
            numImages *= 2;
            int[] offsets = new int[numImages];
            for (int index = 0; index < numImages; index++)
            {
                offsets[index] = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;
            }
            for (int index = 0; index < numImages; index += 2)
            {
                temp_offset = offsets[index];
                int width = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;
                int height = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;

                int bufwid = (8 - (width % 8)) % 8;

                int data_size = (width + bufwid) * height / 2;

                try
                {
                    using (Bitmap b = new(width, height))
                    {
                        int temp_width = 0;
                        int temp_height = 0;
                        for (int temp_index = 0; temp_index < data_size; temp_index++)
                        {
                            byte curByte = lzw_out[temp_offset];
                            byte pixel1 = (byte)((curByte >> 4) & 0x0F);
                            byte pixel2 = (byte)(curByte & 0x0F);
                            Color color1 = helper.GetColor(pixel1);
                            Color color2 = helper.GetColor(pixel2);

                            if (temp_width < width)
                            {
                                b.SetPixel(temp_width, temp_height, color1);
                            }
                            temp_width++;
                            if (temp_width >= width + bufwid)
                            {
                                temp_width = 0;
                                temp_height++;
                            }
                            if (temp_width < width)
                            {
                                b.SetPixel(temp_width, temp_height, color2);
                            }
                            temp_width++;
                            if (temp_width >= width + bufwid)
                            {
                                temp_width = 0;
                                temp_height++;
                            }
                            temp_offset++;
                        }

                        AddMask(b, lzw_out, offsets[index + 1], name, strOutDir);

                        string tempName = name + "_" + (index / 2).ToString() + ".png";
                        string fullPath = Path.Combine(strOutDir, tempName);
                        b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("LZW file does not exist!");
                    return;
                }
            }
        }
    }
}
