using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima4ImageExtractor
    {
        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType)
        {
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("SHAPES.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "SHAPES.png");
                        MakePngU4(file_bytes, fullPath, 2, 32768);
                    }
                }
                else if (image.EndsWith("CHARSET.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "CHARSET.png");
                        MakePngU4(file_bytes, fullPath, 1, 0x2000);
                    }
                }
                else
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        string[] compressed_files = { "ABACUS", "ANIMATE", "GYPSY", "HONCOM", "INSIDE", "OUTSIDE",
                            "PORTAL", "SACHONOR", "SPIRHUM", "TITLE", "TREE", "VALJUS", "WAGON" };
                        string[] rle_files = { "START", "KEY7", "RUNE_0", "RUNE_1", "RUNE_2", "RUNE_3", "RUNE_4", "RUNE_5",
                            "STONCRCL", "HONESTY", "COMPASSN", "VALOR", "JUSTICE", "SACRIFIC", "HONOR",
                            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE" };
                        if (compressed_files.Contains(value))
                        {
                            LzwDecompressor lzw = new LzwDecompressor();

                            byte[] file_bytes = File.ReadAllBytes(image);
                            byte[]? lzw_out;
                            lzw.ExtractU4(file_bytes, out lzw_out);
                            if (lzw_out != null && lzw_out.Length == 32000)
                            {
                                string fullPath = Path.Combine(strImageDir, value + ".png");

                                using (Bitmap b = new Bitmap(320, 200))
                                {
                                    pngHelper helper = new pngHelper();
                                    helper.LoadImage320x200(lzw_out, b, 0);
                                    b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                    Console.WriteLine("Image Created");
                                }
                            }
                        }
                        else if (rle_files.Contains(value))
                        {
                            byte[] file_bytes = File.ReadAllBytes(image);
                            byte[]? rle_out;

                            readRLEFile(file_bytes, out rle_out);

                            if (rle_out != null && rle_out.Length == 32000)
                            {
                                string fullPath = Path.Combine(strImageDir, value + ".png");

                                using (Bitmap b = new Bitmap(320, 200))
                                {
                                    pngHelper helper = new pngHelper();
                                    helper.LoadImage320x200(rle_out, b, 0);
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

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType)
        {
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);

                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    MakeU4(out file_bytes, image, 2);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "SHAPES.EGA");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
                else if (image.EndsWith("CHARSET.png"))
                {
                    byte[]? file_bytes;
                    MakeU4(out file_bytes, image, 1);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "CHARSET.EGA");
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
                            string[] rle_files = { "START", "KEY7", "RUNE_0", "RUNE_1", "RUNE_2", "RUNE_3", "RUNE_4", "RUNE_5",
                            "STONCRCL", "HONESTY", "COMPASSN", "VALOR", "JUSTICE", "SACRIFIC", "HONOR",
                            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE" };
                            if (compressed_files.Contains(value))
                            {
                                byte[]? file_bytes;
                                MakeU4Lzw(out file_bytes, image);
                                if ((null != file_bytes))
                                {
                                    LzwDecompressor lzw = new LzwDecompressor();

                                    string fullPath = Path.Combine(strDataDir, value + ".EGA");
                                    lzw.CompressU4Lzw(file_bytes, fullPath);
                                }
                            }
                            else if (rle_files.Contains(value))
                            {
                                byte[]? file_bytes;
                                MakeU4Lzw(out file_bytes, image);
                                if ((null != file_bytes))
                                {
                                    string fullPath = Path.Combine(strDataDir, value + ".EGA");
                                    writeRLEFile(file_bytes, fullPath);
                                }
                            }
                        }
                    }
                }
            }
            MessageBox.Show("File written!");
        }

        private void writeRLEFile(byte[] file_bytes, string outFile)
        {
            int in_pos = 0;

            using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(outFile, FileMode.Create)))
            {
                while (in_pos < file_bytes.Length)
                {
                    byte curval = file_bytes[in_pos];
                    int length = 1;
                    for(int index = 1; index < 255; index++)
                    {
                        if(in_pos + index >= file_bytes.Length)
                        {
                            break;
                        }
                        byte tempval = file_bytes[in_pos + index];
                        if(tempval != curval)
                        {
                            break;
                        }
                        length++;
                    }
                    
                    if(curval != 2 && length < 5)
                    {
                        binWriter.Write((byte)curval);
                        if(length == 2)
                        {
                            binWriter.Write((byte)curval);
                            in_pos++;
                        }
                        else if (length == 3)
                        {
                            binWriter.Write((byte)curval);
                            binWriter.Write((byte)curval);
                            in_pos +=2 ;
                        }
                        else if (length == 4)
                        {
                            binWriter.Write((byte)curval);
                            binWriter.Write((byte)curval);
                            binWriter.Write((byte)curval);
                            in_pos += 3;
                        }
                        in_pos++;
                    }
                    else
                    {
                        binWriter.Write((byte)2);
                        binWriter.Write((byte)length);
                        binWriter.Write((byte)curval);
                        in_pos += length;
                    }
                }
            }
        }

        private void readRLEFile(byte[] file_bytes, out byte[]? rle_bytes)
        {
            rle_bytes = null;
            byte[] destination = new byte[200 * 160];

            int in_pos = 0;
            int out_pos = 0;
            while(in_pos < file_bytes.Length)
            {
                byte curval = file_bytes[in_pos];
                
                if(curval != 0x2)
                {
                    destination[out_pos] = curval;
                    out_pos++;
                }
                else
                {
                    if(in_pos + 2 > file_bytes.Length)
                    {
                        return; // Invalid file
                    }
                    int count = file_bytes[in_pos + 1];
                    byte value = file_bytes[in_pos + 2];
                    in_pos += 2;
                    for (int index = 0; index < count; index++)
                    {
                        destination[out_pos] = value;
                        out_pos++;
                    }
                }
                in_pos++;
            }
            rle_bytes = destination;
        }

        private void MakeU4Lzw(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                pngHelper helper = new pngHelper();
                byte[] destination = new byte[200 * 160];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Console.WriteLine("Image must be 320x200 pixels!");
                    return;
                }
                helper.CreateImage(destination, image, 320, 200);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private void LoadImageU4(byte[] file_bytes, Bitmap b, int mult)
        {
            pngHelper helper = new pngHelper();

            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 16; ++x_index)
                {
                    long cur_tile = (y_index * 16 + x_index) * (32 * mult * mult);
                    for (int pix_y_index = 0; pix_y_index < (8 * mult); ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < (4 * mult); ++pix_x_index)
                        {
                            byte cur_byte = file_bytes[cur_tile + ((pix_y_index * (4 * mult)) + pix_x_index)];
                            byte b1 = (byte)((cur_byte >> 4) & 0xF);
                            byte b2 = (byte)(cur_byte & 0xF);

                            Color pixColor1 = helper.GetColor(b1);
                            Color pixColor2 = helper.GetColor(b2);

                            b.SetPixel((x_index * (8 * mult)) + pix_x_index * 2, (y_index * (8 * mult)) + pix_y_index, pixColor1);
                            b.SetPixel((x_index * (8 * mult)) + pix_x_index * 2 + 1, (y_index * (8 * mult)) + pix_y_index, pixColor2);
                        }
                    }
                }
            }
        }

        private void MakePngU4(byte[] lzw, string strPng, int mult, int file_size)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != file_size)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(128 * mult, 128 * mult))
                {
                    LoadImageU4(file_bytes, b, mult);
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

        private void WriteTilesU4(byte[] file_bytes, Bitmap b, int mult)
        {
            pngHelper helper = new pngHelper();

            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 16; ++x_index)
                {
                    long cur_tile = (y_index * 16 + x_index) * (32 * mult * mult);
                    for (int pix_y_index = 0; pix_y_index < (8 * mult); ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < (8 * mult); pix_x_index += 2)
                        {
                            Color color1 = b.GetPixel((x_index * (8 * mult)) + pix_x_index, (y_index * (8 * mult)) + pix_y_index);
                            Color color2 = b.GetPixel((x_index * (8 * mult)) + pix_x_index + 1, (y_index * (8 * mult)) + pix_y_index);

                            byte b1 = (byte)((helper.GetByte(color1) << 4) & 0xF0);
                            byte b2 = (byte)(helper.GetByte(color2) & 0x0F);

                            byte outbyte = (byte)(b1 + b2);
                            file_bytes[cur_tile + ((pix_y_index * (4 * mult)) + (pix_x_index / 2))] = outbyte;
                        }
                    }
                }
            }
        }

        private void MakeU4(out byte[]? file_bytes, string strPng, int mult)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[(64 * mult) * (128 * mult)];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 128 * mult && image.Width != 128 * mult)
                {
                    Console.WriteLine("Image must be {0}x{1} pixels!", 128 * mult, 128 * mult);
                    return;
                }
                WriteTilesU4(destination, image, mult);
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
