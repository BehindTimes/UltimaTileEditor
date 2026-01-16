using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima4ImageExtractor
    {
        public static void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
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
                                //ReadCGACompressed(file_bytes, out byte[]? rle_out);
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

        public static void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
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
                            "SPIRIT", "HUMILITY", "TRUTH", "LOVE", "COURAGE" ];
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

        private static void ReadCGACompressed(byte[] file_bytes, out byte[]? out_bytes)
        {
            out_bytes = null;

            int b1 = (file_bytes[2]);
            int b2 = (file_bytes[3] << 8);
            int width = b1 + b2;
            b1 = (file_bytes[4]);
            b2 = (file_bytes[5] << 8);
            int height = b1 + b2;

            byte[] destination = new byte[width * height];

            int j = 9;
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
