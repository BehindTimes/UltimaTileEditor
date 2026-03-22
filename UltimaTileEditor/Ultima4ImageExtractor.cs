using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima4ImageExtractor
    {
        private Color[] color_array_VGA = new Color[256];

        private bool LoadU4VGAPalette(string strDataDir)
        {

            Array.Fill(color_array_VGA, Color.Black);
            string fullPath = Path.Combine(strDataDir, "u4vga.pal");
            try
            {
                byte[] file_bytes = File.ReadAllBytes(fullPath);
                if (file_bytes.Length != 768)
                {
                    return false;
                }
                for (int index = 0; index < 256; index++)
                {
                    byte[] rgb = new byte[3];
                    rgb[0] = (byte)(file_bytes[(index * 3) + 0] * 4);
                    rgb[1] = (byte)(file_bytes[(index * 3) + 1] * 4);
                    rgb[2] = (byte)(file_bytes[(index * 3) + 2] * 4);
                    color_array_VGA[index] = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("u4vga.pal file does not exist!");
                return false;
            }

            return true;
        }

        public void MakePngU4VGA(byte[] file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            using Bitmap b = new(NUM_X * width, NUM_Y * height, PixelFormat.Format8bppIndexed);

            // 2.Define and set the color palette
            ColorPalette palette = b.Palette;
            for (int i = 0; i < color_array_VGA.Length && i < 256; i++)
            {
                palette.Entries[i] = color_array_VGA[i];
            }
            b.Palette = palette; // Set the modified palette back to the bitmap

            // 3. Populate pixel data (example with simple grayscale indices)
            BitmapData data = b.LockBits(new Rectangle(0, 0, NUM_X * width, NUM_Y * height), ImageLockMode.WriteOnly, b.PixelFormat);

            // Create a byte array for the pixel indices (one byte per pixel for 8bpp)
            byte[] pixels = new byte[NUM_X * width * NUM_Y * height];

            PngHelper helper = new PngHelper();

            int byte_counter = 0;

            for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                {
                    for (int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                    {
                        for (int pix_indexX = 0; pix_indexX < width; pix_indexX++)
                        {
                            int curByte = file_bytes[byte_counter];
                            Color c1 = color_array_VGA[curByte];

                            int temppos = (tile_indexY * height + pix_indexY) * (NUM_X * width);
                            temppos += (tile_indexX * width) + pix_indexX;
                            pixels[temppos] = file_bytes[byte_counter];
                            //b.SetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY, c1);
                            byte_counter++;
                        }
                    }
                }
            }

            // Copy the pixel data to the unmanaged memory of the bitmap
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            b.UnlockBits(data);

            b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
            System.Diagnostics.Debug.WriteLine("Image Created");
        }

        private void ExtractUpgradeImagesVGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            // Load the colors in
            if (!LoadU4VGAPalette(strDataDir))
            {
                return;
            }

            PngHelper helper = new PngHelper();
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("shapes.vga"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null && file_bytes.Length == 65536)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + "_vga.png");
                            MakePngU4VGA(file_bytes, fullPath, 16, 16, 16, 16);
                        }
                    }
                }
                else if (image.EndsWith("charset.vga"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length == 8192)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + "_vga.png");
                            MakePngU4VGA(file_bytes, fullPath, 16, 8, 8, 8);
                        }
                    }
                }
            }
        }

        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            if (palette == 2)
            {
                ExtractUpgradeImagesVGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("SHAPES.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "SHAPES.png");
                        LoadPngTile(file_bytes, fullPath, 16, 16, 16, 16, 2);
                    }
                }
                if (image.EndsWith("SHAPES.CGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "SHAPES.png");
                        LoadPngTile(file_bytes, fullPath, 16, 16, 16, 16, 4);
                    }
                }
                else if (image.EndsWith("CHARSET.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "CHARSET.png");
                        LoadPngTile(file_bytes, fullPath, 8, 8, 16, 16, 2);
                    }
                }
                else if (image.EndsWith("CHARSET.CGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "CHARSET.png");
                        LoadPngTile(file_bytes, fullPath, 8, 8, 16, 8, 4);
                    }
                }
                else
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        string[] compressed_files = [ "ABACUS", "ANIMATE", "GYPSY", "HONCOM", "INSIDE", "OUTSIDE",
                            "PORTAL", "SACHONOR", "SPIRHUM", "TITLE", "TREE", "VALJUS", "WAGON" ];
                        string[] rle_files = [ "START", "KEY7", "RUNE_0", "RUNE_1", "RUNE_2", "RUNE_3", "RUNE_4", "RUNE_5",
                            "STONCRCL", "HONESTY", "COMPASSN", "VALOR", "JUSTICE", "SACRIFIC", "HONOR",
                            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE" ];
                        if (compressed_files.Contains(value))
                        {
                            LzwDecompressor lzw = new();

                            byte[] file_bytes = File.ReadAllBytes(image);
                            lzw.ExtractU4(file_bytes, out byte[]? lzw_out);
                            if (palette == 1) // CGA
                            {
                                if (lzw_out != null && lzw_out.Length == 0x4007)
                                {
                                    string fullPath = Path.Combine(strImageDir, value + ".png");

                                    using Bitmap b = new(320, 200);
                                    LoadPIC(lzw_out, b);
                                    b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                    Console.WriteLine("Image Created");
                                }
                            }
                            else // EGA
                            {
                                if (lzw_out != null && lzw_out.Length == 32000)
                                {
                                    string fullPath = Path.Combine(strImageDir, value + ".png");

                                    using Bitmap b = new(320, 200);
                                    PngHelper helper = new();
                                    helper.LoadImage320x200(lzw_out, b, 0);
                                    b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                    Console.WriteLine("Image Created");
                                }
                            }  
                        }
                        else if (rle_files.Contains(value))
                        {
                            byte[] file_bytes = File.ReadAllBytes(image);

                            if (palette == 1) // CGA - Not supported
                            {
                                string fullPath = Path.Combine(strImageDir, value + "_CGA.png");
                                ReadCGARLE(fullPath, file_bytes);
                            }
                            else // EGA
                            {
                                ReadRLEFile(file_bytes, out byte[]? rle_out);

                                if (rle_out != null && rle_out.Length == 32000)
                                {
                                    string fullPath = Path.Combine(strImageDir, value + ".png");

                                    if (palette == 1) // CGA
                                    {
                                        using Bitmap b = new(320, 200);
                                        PngHelper helper = new();
                                        helper.LoadImage320x200(rle_out, b, 0);
                                        b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                        Console.WriteLine("Image Created");
                                    }
                                    else
                                    {
                                        using Bitmap b = new(320, 200);
                                        PngHelper helper = new();
                                        helper.LoadImage320x200(rle_out, b, 0);
                                        b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                        Console.WriteLine("Image Created");
                                    }
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

        private void MakeU4UpgradeVGATiles(out byte[]? file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            PngHelper helper = new PngHelper();
            file_bytes = null;
            Bitmap image;

            try
            {
                image = (Bitmap)Image.FromFile(strPng);
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }

            BitmapData data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                               ImageLockMode.ReadOnly, image.PixelFormat);

            try
            {
                byte[] destination = new byte[(NUM_X * NUM_Y * width * height)];

                if (image.Height != (NUM_Y * height) && image.Width != (NUM_X * width))
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", (NUM_X * width), (NUM_Y * height));
                    return;
                }

                if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                {
                    Debug.WriteLine("Image must be of 8bpp index color in order to properly write!");
                    return;
                }

                // Get the address of the first line
                IntPtr ptr = data.Scan0;
                int stride = data.Stride;

                // Marshal data to a byte array
                byte[] pixels = new byte[stride * image.Height];
                Marshal.Copy(ptr, pixels, 0, pixels.Length);

                int byte_counter = 0;
                for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
                {
                    for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                    {
                        for (int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                        {
                            for (int pix_indexX = 0; pix_indexX < width; pix_indexX++)
                            {
                                int temppos = (tile_indexY * height + pix_indexY) * stride;
                                temppos += (tile_indexX * width) + pix_indexX;
                                destination[byte_counter] = pixels[temppos];

                                byte_counter++;
                            }
                        }
                    }

                    file_bytes = destination;
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
            finally
            {
                image.UnlockBits(data);
            }
        }

        public void CompressImagesUpgradeVGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            // Load the colors in
            if (!LoadU4VGAPalette(strDataDir))
            {
                return;
            }

            PngHelper helper = new PngHelper();
            bool written = false;

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("shapes_vga.png"))
                {
                    byte[]? file_bytes;
                    MakeU4UpgradeVGATiles(out file_bytes, image, 16, 16, 16, 16);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "shapes.vga");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("charset_vga.png"))
                {
                    byte[]? file_bytes;
                    MakeU4UpgradeVGATiles(out file_bytes, image, 16, 8, 8, 8);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "charset.vga");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
            }

            if (written)
            {
                MessageBox.Show("File written!");
            }
        }

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            if (palette == 2) // VGA Upgrade
            {
                CompressImagesUpgradeVGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }

            bool written = false;
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);

                if (image.EndsWith("SHAPES.png"))
                {
                    if(palette == 1) // CGA
                    {
                        string fullPath = Path.Combine(strDataDir, "SHAPES.CGA");
                        MakePngTile(out byte[]? file_bytes, image, 16, 16, 16, 16, 4);
                        if(file_bytes != null)
                        {
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                    else // EGA
                    {
                        string fullPath = Path.Combine(strDataDir, "SHAPES.EGA");
                        MakePngTile(out byte[]? file_bytes, image, 16, 16, 16, 16, 2);
                        if (file_bytes != null)
                        {
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }        
                }
                else if (image.EndsWith("CHARSET.png"))
                {
                    if (palette == 1) // CGA
                    {
                        string fullPath = Path.Combine(strDataDir, "CHARSET.CGA");
                        MakePngTile(out byte[]? file_bytes, image, 8, 8, 16, 8, 4);

                        if (file_bytes != null)
                        {
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                    else // EGA
                    {
                        string fullPath = Path.Combine(strDataDir, "CHARSET.EGA");
                        MakePngTile(out byte[]? file_bytes, image, 8, 8, 16, 16, 2);

                        if (file_bytes != null)
                        {
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
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
                            string[] compressed_files = [ "ABACUS", "ANIMATE", "GYPSY", "HONCOM", "INSIDE", "OUTSIDE",
                            "PORTAL", "SACHONOR", "SPIRHUM", "TITLE", "TREE", "VALJUS", "WAGON" ];
                            string[] rle_files = [ "START", "KEY7", "RUNE_0", "RUNE_1", "RUNE_2", "RUNE_3", "RUNE_4", "RUNE_5",
                            "STONCRCL", "HONESTY", "COMPASSN", "VALOR", "JUSTICE", "SACRIFIC", "HONOR",
                            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE",

                            "START_CGA", "KEY7_CGA", "RUNE_0_CGA", "RUNE_1_CGA", "RUNE_2_CGA", "RUNE_3_CGA", "RUNE_4_CGA", "RUNE_5_CGA",
                            "STONCRCL_CGA", "HONESTY_CGA", "COMPASSN_CGA", "VALOR_CGA", "JUSTICE_CGA", "SACRIFIC_CGA", "HONOR_CGA",
                            "SPIRIT_CGA", "HUMILITY_CGA", "TRUTH_CGA", "LOVE_CGA", "COURAGE_CGA"
                            ];
                            if (compressed_files.Contains(value))
                            {
                                byte[]? file_bytes;
                                if (palette == 1) // CGA
                                {
                                    MakeU4CGA(out file_bytes, image);
                                    if ((null != file_bytes))
                                    {
                                        LzwDecompressor lzw = new();

                                        string fullPath = Path.Combine(strDataDir, value + ".PIC");
                                        lzw.CompressU4Lzw(file_bytes, fullPath);
                                        written = true;
                                    }
                                }
                                else
                                {
                                    MakeU4Lzw(out file_bytes, image);
                                    if ((null != file_bytes))
                                    {
                                        LzwDecompressor lzw = new();

                                        string fullPath = Path.Combine(strDataDir, value + ".EGA");
                                        lzw.CompressU4Lzw(file_bytes, fullPath);
                                        written = true;
                                    }
                                }
                            }
                            else if (rle_files.Contains(value))
                            {
                                if (palette == 1) // CGA
                                {
                                    string tempValue = value;
                                    tempValue = tempValue.Replace("_CGA", "");

                                    string fullPath = Path.Combine(strDataDir, tempValue + ".PIC");
                                    CompressCGARLE(out byte[]? file_bytes, image);
                                    if ((null != file_bytes))
                                    {
                                        using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                                        binWriter.Write(file_bytes);
                                        written = true;
                                    }
                                }
                                else
                                {
                                    MakeU4Lzw(out byte[]? file_bytes, image);
                                    if ((null != file_bytes))
                                    {
                                        string fullPath = Path.Combine(strDataDir, value + ".EGA");
                                        WriteRLEFile(file_bytes, fullPath);
                                        written = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(written)
            {
                MessageBox.Show("File written!");
            }
        }

        private static void WriteRLEFile(byte[] file_bytes, string outFile)
        {
            int in_pos = 0;

            using BinaryWriter binWriter =
                    new(File.Open(outFile, FileMode.Create));
            while (in_pos < file_bytes.Length)
            {
                byte curval = file_bytes[in_pos];
                int length = 1;
                for (int index = 1; index < 255; index++)
                {
                    if (in_pos + index >= file_bytes.Length)
                    {
                        break;
                    }
                    byte tempval = file_bytes[in_pos + index];
                    if (tempval != curval)
                    {
                        break;
                    }
                    length++;
                }

                if (curval != 2 && length < 5)
                {
                    binWriter.Write((byte)curval);
                    if (length == 2)
                    {
                        binWriter.Write((byte)curval);
                        in_pos++;
                    }
                    else if (length == 3)
                    {
                        binWriter.Write((byte)curval);
                        binWriter.Write((byte)curval);
                        in_pos += 2;
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

        private static void DrawCGARLE(string strOutFile, byte[] file_data)
        {
            PngHelper helper = new();

            using (Bitmap b = new Bitmap(320, 200))
            {
                for (int index = 0; index < file_data.Length; index++)
                {
                    byte curbyte = file_data[index];
                    int row = index / 0x50;
                    int col = index % 0x50;
                    if (row >= 200 || col >= 320)
                    {
                        Debug.WriteLine("Exceeds image size");
                        return;
                    }

                    //GetCGAColor
                    byte b1 = (byte)((curbyte >> 6) & 0b11);
                    byte b2 = (byte)((curbyte >> 4) & 0b11);
                    byte b3 = (byte)((curbyte >> 2) & 0b11);
                    byte b4 = (byte)((curbyte >> 0) & 0b11);

                    Color c1 = helper.GetCGAColor(b1);
                    Color c2 = helper.GetCGAColor(b2);
                    Color c3 = helper.GetCGAColor(b3);
                    Color c4 = helper.GetCGAColor(b4);

                    b.SetPixel(col * 4 + 0, 199 - row, c1);
                    b.SetPixel(col * 4 + 1, 199 - row, c2);
                    b.SetPixel(col * 4 + 2, 199 - row, c3);
                    b.SetPixel(col * 4 + 3, 199 - row, c4);
                }

                b.Save(strOutFile, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine("Image Created");
            }
        }

        private static void ReadCGARLE(string strOutFile, byte[] file_bytes)
        {
            /*int b1 = (file_bytes[2]);
            int b2 = (file_bytes[3] << 8);
            int width = b1 + b2;
            b1 = (file_bytes[4]);
            b2 = (file_bytes[5] << 8);
            int height = b1 + b2;*/

            //byte[] destination = new byte[width * height];

            int startoffset = 0x15;
            List<byte> testlist = new List<byte>();
            int segment_length = 5;

            for (int segmentindex = 0; segmentindex < 2; segmentindex++)
            {
                startoffset += segment_length - 5;
                segment_length = file_bytes[startoffset] + (file_bytes[startoffset + 1] << 8);

                startoffset += 5;

                for (int index = startoffset; index < (startoffset - 5) + segment_length; index++)
                {
                    byte curbyte = file_bytes[index];
                    int numbytes = 0;
                    int curoffset = 0;
                    if (curbyte == 0x01) // RLE flag
                    {
                        if (index + 1 >= file_bytes.Length)
                        {
                            Debug.WriteLine("ERROR 1");
                            return;
                        }
                        if (file_bytes[index + 1] == 0) // going to take the next two bytes
                        {
                            numbytes = file_bytes[index + 2] + (file_bytes[index + 3] << 8);
                            curoffset = 4;
                        }
                        else
                        {
                            numbytes = file_bytes[index + 1];
                            curoffset = 2;
                        }
                        if (numbytes > 0)
                        {
                            for (int rleindex = 0; rleindex < numbytes; rleindex++)
                            {
                                testlist.Add(file_bytes[index + curoffset]);
                            }
                        }
                        index += curoffset;
                    }
                    else
                    {
                        testlist.Add(file_bytes[index]);
                    }
                }
            }
            byte[] out_bytes = testlist.ToArray();
            if(out_bytes.Length != 16000)
            {
                return;
            }
            DrawCGARLE(strOutFile, out_bytes);
        }

        private static void ReadRLEFile(byte[] file_bytes, out byte[]? rle_bytes)
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

        private static void ConvertImageToByteArray(ref Bitmap image, out byte[] outbytes)
        {
            PngHelper helper = new();

            // 320x200 CGA, 4 pixels per byte
            outbytes = new byte[16000];
            int curPos = 0;
            for (int indexY = 0; indexY < image.Height; indexY++)
            {
                for (int indexX = 0; indexX < image.Width; indexX += 4)
                {
                    byte curByte = 0;
                    for(int indexZ = 0; indexZ < 4; indexZ++)
                    {
                        Color curColor = image.GetPixel(indexX + indexZ, indexY);
                        byte tempByte = helper.GetCGAByte(curColor);
                        curByte <<= 2;
                        curByte |= (byte)(tempByte & 0b11);
                    }
                    outbytes[curPos] = curByte;
                    curPos++;
                }
            }
        }

        private static void CompressPlane(ref byte[] plane, out byte[]? compressed_bytes)
        {
            compressed_bytes = null;
            List<byte> plane_data = new List<byte>();
            for (int index = 0; index < plane.Length; index++)
            {
                int count = 1;
                byte curByte = plane[index];
                for(int tempIndex = index + 1; tempIndex < plane.Length; tempIndex++)
                {
                    if (plane[tempIndex] == curByte)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if(count == 1 && curByte != 1)
                {
                    plane_data.Add(curByte);
                }
                else if (count == 2 && curByte != 1)
                {
                    plane_data.Add(curByte);
                    plane_data.Add(curByte);

                    index++;
                }
                else if (count == 3 && curByte != 3)
                {
                    plane_data.Add(curByte);
                    plane_data.Add(curByte);
                    plane_data.Add(curByte);

                    index += 2;
                }
                else
                {
                    if (count > 0xFF)
                    {
                        plane_data.Add(0x01);
                        plane_data.Add(0x00);

                        if (count > 0xFFFF)
                        {
                            // This shouldn't happen, especially since the maxamum size of the image is 16000 bytes long
                            return;
                        }
                        plane_data.Add((byte)(count & 0xFF));
                        plane_data.Add((byte)((count >> 8) & 0xFF));
                        plane_data.Add(curByte);
                    }
                    else
                    {
                        plane_data.Add(0x01);
                        plane_data.Add((byte)count);
                        plane_data.Add(curByte);
                    }
                    index += (count - 1);
                }
            }
            compressed_bytes = plane_data.ToArray();
        }

        private static void CompressCGARLE(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                List<byte> list_data = new List<byte>();
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be 320x200 pixels!");
                    return;
                }

                // We display bottom to top, so flip it to make writing easier
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                // PC Paint PIC format
                // Id           - size 2    0x1234 (4660)
                // Width        - size 2    0x0140 (320)
                // Height       - size 2    0x00c8 (200)
                // X Offset     - size 2    0x0000 (0)
                // Y Offset     - size 2    0x0035 (53)
                // Plane Info   - size 1    0x02 (2)
                // Palette Flag - size 1    0xff (255)
                // Video Mode   - size 1    0x41 (65) CGAx320x200x4
                // Palette Type - size 2    0x0001 (1)
                // Palette Size - size 2    0x0002 (2)
                // Palette Data - size 2    0x0300 (3, 0) - stored per byte
                // Block count  - size 2    0x0002 (2)

                list_data.AddRange([0x34, 0x12, 0x40, 0x01, 0xc8, 0x00, 0x00, 0x00, 0x35, 0x00, 0x02, 0xff, 0x41, 0x01, 0x00, 0x02, 0x00, 0x03, 0x00, 0x02, 0x00]);
                byte[] image_bytes;
                ConvertImageToByteArray(ref image, out image_bytes);

                byte[] plane_1 = new byte[8192];
                byte[] plane_2 = new byte[7808];
                Array.Copy(image_bytes, plane_1, 8192);
                Array.Copy(image_bytes, 8192, plane_2, 0, 7808);

                byte[]? compressed_1;
                byte[]? compressed_2;

                CompressPlane(ref plane_1, out compressed_1);
                CompressPlane(ref plane_2, out compressed_2);

                // Sanity check
                if(compressed_1  != null && compressed_2 != null)
                {
                    int tempSize = compressed_1.Length + 5;
                    byte size_1 = (byte)((tempSize >> 8) & 0xff);
                    byte size_2 = (byte)((tempSize) & 0xff);
                    list_data.Add(size_2);
                    list_data.Add(size_1);
                    list_data.AddRange([0, 32, 1]);

                    list_data.AddRange(compressed_1);

                    tempSize = compressed_2.Length + 5;
                    size_1 = (byte)((tempSize >> 8) & 0xff);
                    size_2 = (byte)((tempSize) & 0xff);
                    list_data.Add(size_2);
                    list_data.Add(size_1);
                    list_data.AddRange([128, 30, 1]);

                    list_data.AddRange(compressed_2);

                    file_bytes = list_data.ToArray();
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private static void MakeU4Lzw(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                PngHelper helper = new();
                byte[] destination = new byte[200 * 160];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be 320x200 pixels!");
                    return;
                }
                helper.CreateImage(destination, image, 320, 200);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private static void MakePngTile(out byte[]? file_bytes, string strPingFile, int width, int height, int numXtiles, int numYtiles, int numPixelsPerByte)
        {
            file_bytes = null;
            PngHelper helper = new();
            int interlace = numPixelsPerByte == 4 ? 2 : 1;
            int interlace_offset = (height * (width / numPixelsPerByte)) / interlace;

            try
            {
                Bitmap image = (Bitmap)Image.FromFile(strPingFile);
                if (image.Height != numYtiles * height && image.Width != width * numXtiles)
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", width * numXtiles, numYtiles * height);
                    return;
                }
                byte[] destination = new byte[numXtiles * numYtiles * height * (width / numPixelsPerByte)];

                for (int y_index = 0; y_index < numYtiles; ++y_index)
                {
                    for (int x_index = 0; x_index < numXtiles; ++x_index)
                    {
                        int cur_tile = ((y_index * numXtiles + x_index) * (width / numPixelsPerByte) * height);

                        for (int interlace_index = 0; interlace_index < interlace; interlace_index++)
                        {
                            int temp_interlace = interlace_index * interlace_offset;
                            for (int indexY = 0; indexY < height; indexY += interlace)
                            {
                                for (int indexX = 0; indexX < width; indexX += numPixelsPerByte)
                                {
                                    if (numPixelsPerByte == 4) // CGA
                                    {
                                        Color c1 = image.GetPixel((x_index * width) + indexX + 0, (y_index * height) + indexY + interlace_index);
                                        Color c2 = image.GetPixel((x_index * width) + indexX + 1, (y_index * height) + indexY + interlace_index);
                                        Color c3 = image.GetPixel((x_index * width) + indexX + 2, (y_index * height) + indexY + interlace_index);
                                        Color c4 = image.GetPixel((x_index * width) + indexX + 3, (y_index * height) + indexY + interlace_index);

                                        byte b1 = helper.GetCGAByte(c1);
                                        byte b2 = helper.GetCGAByte(c2);
                                        byte b3 = helper.GetCGAByte(c3);
                                        byte b4 = helper.GetCGAByte(c4);

                                        byte out_byte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + b4);
                                        destination[cur_tile + temp_interlace + ((indexY / interlace) * (width / numPixelsPerByte)) + (indexX / numPixelsPerByte)] = out_byte;
                                    }
                                    else // EGA
                                    {
                                        Color c1 = image.GetPixel((x_index * width) + indexX + 0, (y_index * height) + indexY + interlace_index);
                                        Color c2 = image.GetPixel((x_index * width) + indexX + 1, (y_index * height) + indexY + interlace_index);

                                        byte b1 = helper.GetByte(c1);
                                        byte b2 = helper.GetByte(c2);

                                        byte out_byte = (byte)((b1 << 4) + b2);
                                        destination[cur_tile + temp_interlace + ((indexY / interlace) * (width / numPixelsPerByte)) + (indexX / numPixelsPerByte)] = out_byte;
                                    }
                                }
                            }
                        }
                    }
                }
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private static void LoadTileU4(byte[] file_bytes, Bitmap b, int width, int height, int numXtiles, int numYtiles, int numPixelsPerByte)
        {
            PngHelper helper = new();
            int interlace = numPixelsPerByte == 4 ? 2 : 1;
            int interlace_offset = (height * (width / numPixelsPerByte)) / interlace;

            for (int y_index = 0; y_index < numYtiles; ++y_index)
            {
                for (int x_index = 0; x_index < numXtiles; ++x_index)
                {
                    int cur_tile = ((y_index * numXtiles + x_index) * (width / numPixelsPerByte) * height);

                    for (int interlace_index = 0; interlace_index < interlace; interlace_index++)
                    {
                        int temp_interlace = interlace_index * interlace_offset;
                        for (int indexY = 0;  indexY < height; indexY += interlace)
                        {
                            for (int indexX = 0; indexX < width; indexX += numPixelsPerByte)
                            {
                                if(numPixelsPerByte == 4) // CGA
                                {
                                    byte cur_byte = file_bytes[cur_tile + temp_interlace + ((indexY / interlace) * (width / numPixelsPerByte)) + (indexX / numPixelsPerByte)];

                                    byte b1 = (byte)((cur_byte >> 6) & 0b11);
                                    byte b2 = (byte)((cur_byte >> 4) & 0b11);
                                    byte b3 = (byte)((cur_byte >> 2) & 0b11);
                                    byte b4 = (byte)(cur_byte & 0b11);

                                    Color pixColor1 = helper.GetCGAColor(b1);
                                    Color pixColor2 = helper.GetCGAColor(b2);
                                    Color pixColor3 = helper.GetCGAColor(b3);
                                    Color pixColor4 = helper.GetCGAColor(b4);

                                    b.SetPixel((x_index * width) + indexX + 0, (y_index * height) + indexY + interlace_index, pixColor1);
                                    b.SetPixel((x_index * width) + indexX + 1, (y_index * height) + indexY + interlace_index, pixColor2);
                                    b.SetPixel((x_index * width) + indexX + 2, (y_index * height) + indexY + interlace_index, pixColor3);
                                    b.SetPixel((x_index * width) + indexX + 3, (y_index * height) + indexY + interlace_index, pixColor4);
                                }
                                else // EGA
                                {
                                    byte cur_byte = file_bytes[cur_tile + temp_interlace + ((indexY / interlace) * (width / numPixelsPerByte)) + (indexX / numPixelsPerByte)];

                                    byte b1 = (byte)((cur_byte >> 4) & 0xF);
                                    byte b2 = (byte)((cur_byte >> 0) & 0xF);

                                    Color pixColor1 = helper.GetColor(b1);
                                    Color pixColor2 = helper.GetColor(b2);

                                    b.SetPixel((x_index * width) + indexX + 0, (y_index * height) + indexY + interlace_index, pixColor1);
                                    b.SetPixel((x_index * width) + indexX + 1, (y_index * height) + indexY + interlace_index, pixColor2);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void LoadPngTile(byte[] lzw, string strPng, int tile_width, int tile_height, int numXtiles, int numYtiles, int numPixelsPerByte)
        {
            try
            {
                byte[] file_bytes = lzw;
                int file_size = (tile_width / numPixelsPerByte) * tile_height * numXtiles * numYtiles;
                if (file_bytes.Length != file_size)
                {
                    return;
                }
                using Bitmap b = new(numXtiles * tile_width, numYtiles * tile_height);
                LoadTileU4(file_bytes, b, tile_width, tile_height, numXtiles, numYtiles, numPixelsPerByte);
                b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                Console.WriteLine("Image Created");
            }
            catch (IOException)
            {
                Debug.WriteLine("LZW file does not exist!");
                return;
            }
        }

        private static void MakeU4CGA(out byte[]? file_bytes, string strPng)
        {
            file_bytes = null;
            try
            {
                PngHelper helper = new();
                byte[] destination = new byte[0x4007];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be 320x200 pixels!");
                    return;
                }
                destination[0] = 0xFD;
                destination[2] = 0xB8;
                destination[6] = 0x40;

                const int START_OFFSET = 7; // A few extra bytes I'm unsure about, but they're consistent, and play into 16192 for CGA memory
                const int INTERLACE_SIZE = 8192;
                const int NUM_PIXELS_PER_BYTE = 4;
                const int NUM_BYTES_PER_LINE = 320 / NUM_PIXELS_PER_BYTE;

                for (int interlaceIndex = 0; interlaceIndex < 2; interlaceIndex++)
                {
                    int tempStart = (interlaceIndex * INTERLACE_SIZE) + START_OFFSET;
                    for (int indexY = 0; indexY < 100; indexY++)
                    {
                        for (int indexX = 0; indexX < 320; indexX += NUM_PIXELS_PER_BYTE)
                        {
                            Color c1 = image.GetPixel(indexX + 0, indexY * 2 + interlaceIndex);
                            Color c2 = image.GetPixel(indexX + 1, indexY * 2 + interlaceIndex);
                            Color c3 = image.GetPixel(indexX + 2, indexY * 2 + interlaceIndex);
                            Color c4 = image.GetPixel(indexX + 3, indexY * 2 + interlaceIndex);

                            byte color1 = helper.GetCGAByte(c1);
                            byte color2 = helper.GetCGAByte(c2);
                            byte color3 = helper.GetCGAByte(c3);
                            byte color4 = helper.GetCGAByte(c4);
                            byte color = (byte)((color1 << 6) + (color2 << 4) + (color3 << 2) + color4);

                            destination[tempStart + (indexY * NUM_BYTES_PER_LINE) + (indexX / NUM_PIXELS_PER_BYTE)] = color;
                        }
                    }
                }

                // PCPaint V1.0 - Meta data left over, but we're not writing it.  It will leave some differences in the final file

                file_bytes = destination;
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        // All the PICs are 320x200, so not worrying about making it generic
        private static void LoadPIC(byte[] file_bytes, Bitmap b)
        {
            PngHelper helper = new();
            const int START_OFFSET = 7; // A few extra bytes I'm unsure about, but they're consistent, and play into 16192 for CGA memory
            const int INTERLACE_SIZE = 8192;
            const int NUM_PIXELS_PER_BYTE = 4;
            const int NUM_BYTES_PER_LINE = 320 / NUM_PIXELS_PER_BYTE;

            for (int interlaceIndex = 0; interlaceIndex < 2; interlaceIndex++)
            {
                int tempStart = (interlaceIndex * INTERLACE_SIZE) + START_OFFSET;
                for(int indexY = 0;  indexY < 100; indexY++)
                {
                    for (int indexX = 0; indexX < 320; indexX += NUM_PIXELS_PER_BYTE)
                    {
                        byte curByte = file_bytes[tempStart + (indexY * NUM_BYTES_PER_LINE) + (indexX / NUM_PIXELS_PER_BYTE)];
                        byte b1 = (byte)((curByte >> 6) & 0b11);
                        byte b2 = (byte)((curByte >> 4) & 0b11);
                        byte b3 = (byte)((curByte >> 2) & 0b11);
                        byte b4 = (byte)(curByte & 0b11);

                        Color pixColor1 = helper.GetCGAColor(b1);
                        Color pixColor2 = helper.GetCGAColor(b2);
                        Color pixColor3 = helper.GetCGAColor(b3);
                        Color pixColor4 = helper.GetCGAColor(b4);

                        b.SetPixel(indexX + 0, indexY * 2 + interlaceIndex, pixColor1);
                        b.SetPixel(indexX + 1, indexY * 2 + interlaceIndex, pixColor2);
                        b.SetPixel(indexX + 2, indexY * 2 + interlaceIndex, pixColor3);
                        b.SetPixel(indexX + 3, indexY * 2 + interlaceIndex, pixColor4);
                    }
                }
            }
        }
    }
}
