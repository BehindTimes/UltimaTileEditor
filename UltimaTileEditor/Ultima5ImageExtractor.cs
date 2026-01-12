using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Formats.Tar;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UltimaTileEditor
{
    internal class Ultima5ImageExtractor
    {
        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            LzwDecompressor lzw = new LzwDecompressor();

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("TILES.16"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[]? lzw_out;
                    lzw.Extract(file_bytes, out lzw_out);
                    if (lzw_out != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "TILES.png");
                        MakePngU5(lzw_out, fullPath, 2);
                    }
                }
                else if (image.EndsWith("TILES.4"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[]? lzw_out;
                    lzw.Extract(file_bytes, out lzw_out);
                    if (lzw_out != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "TILES.png");
                        MakePngU5(lzw_out, fullPath, 4);
                    }
                }
                else if (image.EndsWith("IBM.CH") || image.EndsWith("RUNES.CH"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length == 1024)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            CreateSimpleBitmap(file_bytes, 8, 8, 16, 8, fullPath);
                        }
                    }
                }
                else if (image.EndsWith("IBM.HCS") || image.EndsWith("RUNES.HCS"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length == 3072)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            CreateSimpleBitmap(file_bytes, 16, 12, 16, 8, fullPath);
                        }
                    }
                }
                else if (image.EndsWith("BRITISH.BIT") || image.EndsWith("TITLE.BIT"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[]? lzw_out;
                    lzw.Extract(file_bytes, out lzw_out);
                    if (lzw_out != null)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            CreateBitImages(lzw_out, value, strImageDir);
                        }
                    }
                }
                else if (image.EndsWith("WD.BIT")) // This file is not compressed
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        CreateBitImages(file_bytes, value, strImageDir);
                    }
                }
                else if (image.EndsWith("BRITISH.PTH")) // This file is not compressed
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        CreatePathImage(file_bytes, value, strImageDir);
                    }
                }
                else
                {
                    if (palette == 0) // EGA
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        byte[]? lzw_out;
                        lzw.Extract(file_bytes, out lzw_out);
                        if (lzw_out != null)
                        {
                            string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                            if (value != null)
                            {
                                if (value.ToLower().Contains("mon") || value.ToLower().Contains("items"))
                                {
                                    CreateMaskImages(lzw_out, strImageDir, value, 2);
                                }
                                else
                                {
                                    CreateImages(lzw_out, strImageDir, value, 2);
                                }
                            }
                        }
                    }
                    else if (palette == 1) // CGA
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        byte[]? lzw_out;
                        lzw.Extract(file_bytes, out lzw_out);
                        if (lzw_out != null)
                        {
                            string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                            if (value != null)
                            {
                                if (value.ToLower().Contains("mon") || value.ToLower().Contains("items"))
                                {
                                    CreateMaskImages(lzw_out, strImageDir, value, 4);
                                }
                                else
                                {
                                    CreateImages(lzw_out, strImageDir, value, 4);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            LzwDecompressor lzw = new LzwDecompressor();

            Dictionary<string, Dictionary<int, Bitmap>> image_data = new Dictionary<string, Dictionary<int, Bitmap>>();
            bool written = false;

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);

                if (palette == 0) // EGA
                {
                    if (tempimage.EndsWith("TILES.png"))
                    {
                        byte[]? file_bytes;
                        MakeLZWU5(out file_bytes, image, 2);

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, "TILES.16");
                            lzw.Compress(file_bytes, fullPath);
                            written = true;
                        }
                    }
                    else if ((image.EndsWith("IBM.png") || image.EndsWith("RUNES.png")) && imageType == 4)
                    {
                        try
                        {
                            Bitmap b = (Bitmap)System.Drawing.Image.FromFile(image);
                            if (b.Width == 128 && b.Height == 64)
                            {
                                string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                                if (value != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, value + ".CH");
                                    ReadSimpleBitmap(b, 8, 8, 16, 8, fullPath);
                                    written = true;
                                }
                            }
                        }
                        catch (IOException)
                        {
                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                            return;
                        }
                    }
                    else if ((image.EndsWith("IBM.png") || image.EndsWith("RUNES.png")) && imageType == 5)
                    {
                        try
                        {
                            Bitmap b = (Bitmap)System.Drawing.Image.FromFile(image);
                            if (b.Width == 256 && b.Height == 96)
                            {
                                string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                                if (value != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, value + ".HCS");
                                    ReadSimpleBitmap(b, 16, 12, 16, 8, fullPath);
                                    written = true;
                                }
                            }
                        }
                        catch (IOException)
                        {
                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                            return;
                        }
                    }
                    else if(imageType ==7) // Path files, do nothing
                    {
                        return;
                    }
                    else
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);

                        if (value != null)
                        {
                            string strName;
                            int picNum = 0;
                            string[] nameParams = value.Split('_');
                            if (nameParams.Length == 2)
                            {
                                strName = nameParams[0];
                                if (int.TryParse(nameParams[1], out picNum))
                                {
                                    if (DataFiles.Ultima5Pict.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                    else if (DataFiles.Ultima5Masked.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                    else if (DataFiles.Ultima5Dng.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                    else if (DataFiles.Ultima5BitFiles.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else // CGA
                {
                    if (tempimage.EndsWith("TILES.png"))
                    {
                        byte[]? file_bytes;
                        MakeLZWU5(out file_bytes, image, 4);

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, "TILES.4");
                            lzw.Compress(file_bytes, fullPath);
                            written = true;
                        }
                    }
                    else if (imageType == 1 || imageType == 2 || imageType == 3) // Image and Dungeon files
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);

                        if (value != null)
                        {
                            string strName;
                            int picNum = 0;
                            string[] nameParams = value.Split('_');
                            if (nameParams.Length == 2)
                            {
                                strName = nameParams[0];
                                if (int.TryParse(nameParams[1], out picNum))
                                {
                                    if (DataFiles.Ultima5Pict.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                    else if (DataFiles.Ultima5Masked.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                    else if (DataFiles.Ultima5Dng.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                    else if (DataFiles.Ultima5BitFiles.Any(x => strName.StartsWith(x)))
                                    {
                                        if (!image_data.ContainsKey(strName))
                                        {
                                            image_data[strName] = new Dictionary<int, Bitmap>();
                                        }
                                        try
                                        {
                                            image_data[strName][picNum] = (Bitmap)System.Drawing.Image.FromFile(image);
                                        }
                                        catch (IOException)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PNG file does not exist!");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Now that the files are loaded, combine them
            if (palette == 0) // EGA
            {
                switch (imageType)
                {
                    case 1: // Masked Images
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes = null;
                                BuildMaskImage(out file_bytes, key, image_data[key], image_data[key].Count, 2);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".16");
                                    lzw.Compress(file_bytes, fullPath);
                                }
                            }
                        }
                        written = true;
                        break;
                    case 2: // Dungeon Images
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes;
                                BuildImage(out file_bytes, key, image_data[key], 28, true, 2);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".16");
                                    lzw.Compress(file_bytes, fullPath);
                                }
                            }
                        }
                        written = true;
                        break;
                    case 3: // Regular Images
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes;
                                BuildImage(out file_bytes, key, image_data[key], image_data[key].Count, false, 2);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".16");
                                    lzw.Compress(file_bytes, fullPath);
                                    written = true;
                                }
                            }
                        }
                        break;
                    case 4: // CH files
                        break;
                    case 5: // HCS files
                        break;
                    case 6: // BIT files
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes;
                                BuildImageSimple(out file_bytes, key, image_data[key], image_data[key].Count);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".BIT");
                                    if (key.EndsWith("BRITISH") || key.EndsWith("TITLE"))
                                    {
                                        lzw.Compress(file_bytes, fullPath);
                                        written = true;
                                    }
                                    else
                                    {
                                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                                        {
                                            binWriter.Write(file_bytes);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            else if(palette == 1) // CGA
            {
                switch (imageType)
                {
                    case 1: // Masked Images
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes = null;
                                BuildMaskImage(out file_bytes, key, image_data[key], image_data[key].Count, 4);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".4");

                                    lzw.Compress(file_bytes, fullPath);
                                }
                            }
                        }
                        written = true;
                        break;
                    case 2: // Dungeon Images
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes;
                                BuildImage(out file_bytes, key, image_data[key], 28, true, 4);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".4");
                                    lzw.Compress(file_bytes, fullPath);
                                }
                            }
                        }
                        written = true;
                        break;
                    case 3: // Regular Images
                        foreach (string key in image_data.Keys)
                        {
                            if (ValidateImageArray(key, image_data[key]))
                            {
                                byte[]? file_bytes;
                                BuildImage(out file_bytes, key, image_data[key], image_data[key].Count, false, 4);

                                if (file_bytes != null)
                                {
                                    string fullPath = Path.Combine(strDataDir, key + ".4");
                                    lzw.Compress(file_bytes, fullPath);
                                    written = true;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            if (written)
            {
                MessageBox.Show("Files written!");
            }
        }

        private void LoadPathImage(byte[] file_data, Bitmap b)
        {
            using (Graphics gfx = Graphics.FromImage(b))
            {
                // Clear the entire bitmap and fill it with a solid color (e.g., Green)
                gfx.Clear(Color.Black); //
            }

            const int offsetX = 44;
            const int offsetY = 68;
            const int imageWidth = 320;
            const int imageHeight = 200;

            int xpos = offsetX;
            int ypos = offsetY;

            int zerocount = 0;

            b.SetPixel(xpos, ypos, Color.White);
            for (int index = 0; index < file_data.Length; index++)
            {
                byte curByte = file_data[index];
                if (curByte != 0)
                {
                    byte leftright = (byte)((curByte >> 4) & 0xF);
                    byte updown = (byte)(curByte & 0xF);
                    if (leftright >= 8)
                    {
                        int subval = leftright - 8;
                        xpos -= subval;
                    }
                    else
                    {
                        xpos += leftright;
                    }
                    if (updown >= 8)
                    {
                        int subval = updown - 8;
                        ypos -= subval;
                    }
                    else
                    {
                        ypos += updown;
                    }

                    if ((updown >= 3 && updown < 8) || (updown >= 0x0a) || (leftright >= 3 && leftright < 8) || (leftright >= 0x0a)) // d5
                    {
                        continue;
                    }
                    else
                    {
                        if (xpos >= 0 && xpos < imageWidth && ypos >= 0 && ypos < imageHeight)
                        {
                            b.SetPixel(xpos, ypos, Color.White);
                        }
                    }

                }
                else
                {
                    zerocount++;
                    switch (zerocount)
                    {
                        case 1:
                            xpos = offsetX + 20;
                            ypos = offsetY + 26;
                            break;
                        case 2:
                            xpos = offsetX + 99;
                            ypos = offsetY + 10;
                            break;
                        case 3:
                            xpos = offsetX + 123;
                            ypos = offsetY + 37;
                            break;
                        default:
                            return;
                    }

                    if (xpos >= 0 && xpos < imageWidth && ypos >= 0 && ypos < imageHeight)
                    {
                        b.SetPixel(xpos, ypos, Color.White);
                    }
                }
            }
        }

        private void CreatePathImage(byte[] file_data, string name, string strImageDir)
        {
            try
            {
                using (Bitmap b = new Bitmap(320, 200))
                {
                    string fullPath = Path.Combine(strImageDir, name + "_PATH.png");

                    LoadPathImage(file_data, b);
                    b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                    Console.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("LZW file does not exist!");
                return;
            }
        }

        private void CreateBitImages(byte[] file_data, string name, string strImageDir)
        {
            int temp_offset = 2;
            int numImages = (file_data[1] << 8) + file_data[0];
            int[] offsets = new int[numImages];
            for (int index = 0; index < numImages; index++)
            {
                offsets[index] = (file_data[temp_offset + 1] << 8) + (file_data[temp_offset + 0] << 0);
                temp_offset += 2;
            }
            for (int index = 0; index < numImages; index++)
            {
                string fullPath = Path.Combine(strImageDir, name + "_" + index.ToString() + ".png");

                int curPos = offsets[index];
                int width = (file_data[curPos + 1] << 8) + (file_data[curPos + 0] << 0);
                curPos += 2;
                int height = (file_data[curPos + 1] << 8) + (file_data[curPos + 0] << 0);
                curPos += 2;
                CreateSimpleBitmapWithOffset(file_data, width, height, fullPath, curPos);
            }
        }

        private void CreateSimpleBitmapWithOffset(byte[] file_data, int width, int height, string strBitmap, int offset)
        {
            using (Bitmap b = new Bitmap(width, height))
            {
                int curPos = 0;
                for (int indexY = 0; indexY < height; indexY++)
                {
                    for (int indexX = 0; indexX < width / 8; indexX++)
                    {
                        byte curByte = file_data[curPos + offset];

                        for (int tempIndexX = 0; tempIndexX < 8; tempIndexX++)
                        {
                            int curX = indexX * 8 + tempIndexX;
                            int curY = indexY;
                            int curVal = (curByte >> (7 - tempIndexX)) & 0x1;
                            if (curVal == 0)
                            {
                                b.SetPixel(curX, curY, Color.Black);
                            }
                            else
                            {
                                b.SetPixel(curX, curY, Color.White);
                            }
                        }

                        curPos++;
                    }
                }
                b.Save(strBitmap, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void CreateSimpleBitmap(byte[] file_data, int tile_width, int tile_height, int numX, int numY, string strBitmap)
        {
            using (Bitmap b = new Bitmap(numX * tile_width, numY * tile_height))
            {
                int curTile = 0;
                int tempWidth = 0;
                int tempHeight = 0;
                for (int index = 0; index < file_data.Length; index++)
                {
                    byte curData = file_data[index];
                    for (int tempIndexX = 0; tempIndexX < 8; tempIndexX++)
                    {
                        int curX = (curTile % numX) * tile_width + tempIndexX + tempWidth;
                        int curY = (curTile / numX) * tile_height + tempHeight;
                        int curVal = (curData >> (7 - tempIndexX)) & 0x1;
                        if (curVal == 0)
                        {
                            b.SetPixel(curX, curY, Color.Black);
                        }
                        else
                        {
                            b.SetPixel(curX, curY, Color.White);
                        }
                    }
                    tempWidth += 8;
                    if(tempWidth >= tile_width)
                    {
                        tempWidth = 0;
                        tempHeight++;
                    }
                    if(tempHeight >= tile_height)
                    {
                        tempHeight = 0;
                        curTile++;
                    }
                }
                b.Save(strBitmap, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        public void MakePngU5(byte[] lzw, string strPng, int numPixelsPerByte)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != (512 * 256 / numPixelsPerByte))
                {
                    return;
                }
                using (Bitmap b = new Bitmap(512, 256))
                {
                    LoadImageU5(file_bytes, b, numPixelsPerByte);
                    b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                    Debug.WriteLine("Image Created");
                }
            }
            catch (IOException)
            {
                Debug.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public void ReadSimpleBitmap(Bitmap b, int tile_width, int tile_height, int numX, int numY, string strOutFile)
        {
            byte[] file_data = new byte[(tile_width / 8) * tile_height * numY * numX];
            int curPos = 0;
            for (int indexY = 0; indexY < numY; indexY++)
            {
                for (int indexX = 0; indexX < numX; indexX++)
                {
                    for(int indexHeight = 0; indexHeight < tile_height; indexHeight++)
                    {
                        for (int indexWidth = 0; indexWidth < tile_width / 8; indexWidth++)
                        {
                            byte curByte = 0;
                            for(int x = 0; x < 8; x++)
                            {
                                int curX = (indexX * tile_width) + (indexWidth * 8) + x;
                                int curY = (indexY * tile_height) + indexHeight;
                                Color tempColor = b.GetPixel(curX, curY);
                                if(tempColor.R == 255 && tempColor.G == 255 && tempColor.B == 255)
                                {
                                    curByte |= (byte)(1 << (7 - x));
                                }
                            }
                            file_data[curPos] = curByte;
                            curPos++;
                        }
                    }
                }
            }

            using (BinaryWriter binWriter =
                    new BinaryWriter(File.Open(strOutFile, FileMode.Create)))
            {
                binWriter.Write(file_data);
            }
        }

        private void BuildMaskImage(out byte[]? file_bytes, string name, Dictionary<int, Bitmap> value, int numKeys, int numPixelsPerByte)
        {
            file_bytes = null;
            List<byte> outImage = new List<byte>();
            byte num1 = (byte)((numKeys >> 8) & 0xFF);
            byte num2 = (byte)(numKeys & 0xFF);
            outImage.Add(num2);
            outImage.Add(num1);
            int curPos = 2 + numKeys * 4;

            if (numPixelsPerByte != 8 && numPixelsPerByte != 4 && numPixelsPerByte != 2)
            {
                return; // Error
            }

            for (int index = 0; index < numKeys; index++)
            {
                byte b3 = (byte)((curPos >> 8) & 0xFF);
                byte b4 = (byte)((curPos >> 0) & 0xFF);
                outImage.Add(b4);
                outImage.Add(b3);

                if (value.ContainsKey(index))
                {
                    int bufWidth = value[index].Width;
                    int bufNum = (8 / numPixelsPerByte) * 2;
                    int tempBuf = (bufNum - (value[index].Width % bufNum)) % bufNum;

                    bufWidth += tempBuf;

                    curPos += ((bufWidth / numPixelsPerByte) * value[index].Height);
                    curPos += 4; // Remember to include width & height
                    byte b1 = (byte)((curPos >> 8) & 0xFF);
                    byte b2 = (byte)((curPos >> 0) & 0xFF);
                    outImage.Add(b2);
                    outImage.Add(b1);
                    curPos += ((bufWidth / 8) * value[index].Height);
                    curPos += 4; // Remember to include width & height
                }
                else
                {
                    return; // Shouldn't happen, so error
                }
            }
            for (int index = 0; index < numKeys; index++)
            {
                int bufWidth = value[index].Width;
                int bufNum = (8 / numPixelsPerByte) * 2;
                int tempBuf = (bufNum - (value[index].Width % bufNum)) % bufNum;

                bufWidth += tempBuf;

                // Write the width & height for the image
                byte b3 = (byte)((value[index].Width >> 8) & 0xFF);
                byte b4 = (byte)((value[index].Width >> 0) & 0xFF);
                outImage.Add(b4);
                outImage.Add(b3);
                b3 = (byte)((value[index].Height >> 8) & 0xFF);
                b4 = (byte)((value[index].Height >> 0) & 0xFF);
                outImage.Add(b4);
                outImage.Add(b3);
                byte[] imageBuf;
                byte[] maskBuf;
                GenerateImageMaskBytes(out imageBuf, out maskBuf, value[index], bufWidth / numPixelsPerByte, numPixelsPerByte);
                outImage.AddRange(imageBuf);
                // Write the width & height for the image mask
                b3 = (byte)((value[index].Width >> 8) & 0xFF);
                b4 = (byte)((value[index].Width >> 0) & 0xFF);
                outImage.Add(b4);
                outImage.Add(b3);
                b3 = (byte)((value[index].Height >> 8) & 0xFF);
                b4 = (byte)((value[index].Height >> 0) & 0xFF);
                outImage.Add(b4);
                outImage.Add(b3);
                outImage.AddRange(maskBuf);
            }

            file_bytes = outImage.ToArray();
        }

        private void BuildImageSimple(out byte[]? file_bytes, string name, Dictionary<int, Bitmap> value, int numKeys)
        {
            file_bytes = null;
            List<byte> outImage = new List<byte>();
            byte num1 = (byte)((numKeys >> 8) & 0xFF);
            byte num2 = (byte)(numKeys & 0xFF);
            outImage.Add(num2);
            outImage.Add(num1);
            int curPos = 2 + numKeys * 2;
            for (int index = 0; index < numKeys; index++)
            {
                byte b1 = (byte)((curPos >> 8) & 0xFF);
                byte b2 = (byte)((curPos >> 0) & 0xFF);
                outImage.Add(b2);
                outImage.Add(b1);

                if (value.ContainsKey(index))
                {
                    int bufWidth = value[index].Width;
                    curPos += ((bufWidth / 8) * value[index].Height);
                    curPos += 4; // Remember to include width & height
                }
            }
            for (int index = 0; index < numKeys; index++)
            {
                if (value.ContainsKey(index))
                {
                    byte b3 = (byte)((value[index].Width >> 8) & 0xFF);
                    byte b4 = (byte)((value[index].Width >> 0) & 0xFF);
                    outImage.Add(b4);
                    outImage.Add(b3);
                    b3 = (byte)((value[index].Height >> 8) & 0xFF);
                    b4 = (byte)((value[index].Height >> 0) & 0xFF);
                    outImage.Add(b4);
                    outImage.Add(b3);
                    byte[] imageBuf;
                    if (value[index].Width % 8 != 0)
                    {
                        return; // Invalid bitmap
                    }

                    GenerateSimpleImageBytes(out imageBuf, value[index], value[index].Width / 8);
                    outImage.AddRange(imageBuf);
                }
            }
            file_bytes = outImage.ToArray();
        }

        private void BuildImage(out byte[]? file_bytes, string name, Dictionary<int, Bitmap> value, int numKeys, bool excludeDungeonKeys, int numPixelsPerByte)
        {
            file_bytes = null;
            List<byte> outImage = new List<byte>();
            byte num1 = (byte)((numKeys >> 8) & 0xFF);
            byte num2 = (byte)(numKeys & 0xFF);
            outImage.Add(num2);
            outImage.Add(num1);
            int curPos = 2 + numKeys * 4;

            if (numPixelsPerByte != 8 && numPixelsPerByte != 4 && numPixelsPerByte != 2)
            {
                return; // Error
            }

            for (int index = 0; index < numKeys; index++)
            {
                byte b1 = (byte)((curPos >> 24) & 0xFF);
                byte b2 = (byte)((curPos >> 16) & 0xFF);
                byte b3 = (byte)((curPos >> 8) & 0xFF);
                byte b4 = (byte)((curPos >> 0) & 0xFF);
                if (excludeDungeonKeys && (index == 8 || index == 24))
                {
                    outImage.Add(0);
                    outImage.Add(0);
                    outImage.Add(0);
                    outImage.Add(0);
                }
                else
                {
                    outImage.Add(b4);
                    outImage.Add(b3);
                    outImage.Add(b2);
                    outImage.Add(b1);
                } 

                if (value.ContainsKey(index))
                {
                    int bufNum = (8 / numPixelsPerByte) * 2;
                    int bufWidth = (bufNum - (value[index].Width % bufNum)) % bufNum;
                    bufWidth += value[index].Width;

                    curPos += ((bufWidth / numPixelsPerByte) * value[index].Height);
                    curPos += 4; // Remember to include width & height
                }
            }
            for (int index = 0; index < numKeys; index++)
            {
                if (value.ContainsKey(index))
                {
                    int bufWidth = value[index].Width;
                    int bufNum = (8 / numPixelsPerByte) * 2;
                    int tempBuf = (bufNum - (value[index].Width % bufNum)) % bufNum;

                    bufWidth += tempBuf;

                    byte b3 = (byte)((value[index].Width >> 8) & 0xFF);
                    byte b4 = (byte)((value[index].Width >> 0) & 0xFF);
                    outImage.Add(b4);
                    outImage.Add(b3);
                    b3 = (byte)((value[index].Height >> 8) & 0xFF);
                    b4 = (byte)((value[index].Height >> 0) & 0xFF);
                    outImage.Add(b4);
                    outImage.Add(b3);
                    byte[] imageBuf;

                    GenerateImageBytes(out imageBuf, value[index], bufWidth / numPixelsPerByte, numPixelsPerByte);
                    outImage.AddRange(imageBuf);
                }
            }
            file_bytes = outImage.ToArray();
        }

        private void GenerateImageMaskBytes(out byte[] file_bytes, out byte[] mask_bytes, Bitmap b, int bufWidth, int numPixelsPerByte)
        {
            pngHelper helper = new pngHelper();

            file_bytes = new byte[bufWidth * b.Height];
            mask_bytes = new byte[bufWidth / (8 / numPixelsPerByte) * b.Height];
            int temp_index = 0;
            byte outbyte = 0;

            for (int indexY = 0; indexY < b.Height; indexY++)
            {
                temp_index = (indexY * bufWidth) / (8 / numPixelsPerByte);

                for (int indexX = 0; indexX < b.Width; indexX += numPixelsPerByte)
                {
                    int offsetByte = indexX / 8;

                    if(numPixelsPerByte == 4)
                    {
                        Color color1 = b.GetPixel(indexX, indexY);
                        byte b1 = (byte)(((helper.GetCGAByteIgnoreAlpha(color1) & 0b11) << 6));
                        byte b2 = 0;
                        byte b3 = 0;
                        byte b4 = 0;
                        if (color1.A < 128)
                        {
                            int offsetval = (1 << (7 - ((indexX % 8) % 8)));
                            mask_bytes[temp_index + offsetByte] |= (byte)offsetval;
                        }

                        if (indexX + 1 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 1, indexY);
                            b2 = (byte)((helper.GetCGAByteIgnoreAlpha(color2) & 0b11) << 4);
                            if (color2.A < 128)
                            {
                                int offsetval = (1 << (7 - (((indexX + 1) % 8) % 8)));
                                mask_bytes[temp_index + offsetByte] |= (byte)offsetval;
                            }
                        }
                        if (indexX + 2 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 2, indexY);
                            b3 = (byte)((helper.GetCGAByteIgnoreAlpha(color2) & 0b11) << 2);
                            if (color2.A < 128)
                            {
                                int offsetval = (1 << (7 - (((indexX + 2) % 8) % 8)));
                                mask_bytes[temp_index + offsetByte] |= (byte)offsetval;
                            }
                        }
                        if (indexX + 3 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 3, indexY);
                            b4 = (byte)(helper.GetCGAByteIgnoreAlpha(color2) & 0b11);
                            if (color2.A < 128)
                            {
                                int offsetval = (1 << (7 - (((indexX + 3) % 8) % 8)));
                                mask_bytes[temp_index + offsetByte] |= (byte)offsetval;
                            }
                        }
                        outbyte = (byte)(b1 + b2 + b3 + b4);
                    }
                    else
                    {
                        Color color1 = b.GetPixel(indexX, indexY);
                        byte b1 = (byte)((helper.GetByteIgnoreAlpha(color1) << 4) & 0xF0);
                        byte b2 = 0;
                        if (color1.A < 128)
                        {
                            int offsetval = (1 << (7 - ((indexX % 8) % 8)));
                            mask_bytes[temp_index + offsetByte] |= (byte)offsetval;
                        }

                        if (indexX + 1 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 1, indexY);
                            b2 = (byte)(helper.GetByteIgnoreAlpha(color2) & 0x0F);
                            if (color2.A < 128)
                            {
                                int offsetval = (1 << (7 - (((indexX + 1) % 8) % 8)));
                                mask_bytes[temp_index + offsetByte] |= (byte)offsetval;
                            }
                        }
                        outbyte = (byte)(b1 + b2);
                    }
  
                    file_bytes[indexY * bufWidth + (indexX / numPixelsPerByte)] = outbyte;
                }
            }
        }

        private void GenerateSimpleImageBytes(out byte[] file_bytes, Bitmap b, int bufWidth)
        {
            pngHelper helper = new pngHelper();

            file_bytes = new byte[bufWidth * b.Height];
            for (int indexY = 0; indexY < b.Height; indexY++)
            {
                for (int indexX = 0; indexX < bufWidth; indexX++)
                {
                    byte curByte = 0;
                    for (int tempIndexX = 0; tempIndexX < 8; tempIndexX++)
                    {
                        int curX = indexX * 8 + tempIndexX;
                        int curY = indexY;

                        Color tempColor = b.GetPixel(curX, curY);
                        if (tempColor.R == 255 && tempColor.G == 255 && tempColor.B == 255)
                        {
                            curByte |= (byte)(1 << (7 - tempIndexX));
                        }
                    }
                    file_bytes[indexY * bufWidth + indexX] = curByte;
                }
            }
        }

        private void GenerateImageBytes(out byte[] file_bytes, Bitmap b, int bufWidth, int numPixelsPerByte)
        {
            pngHelper helper = new pngHelper();

            file_bytes = new byte[bufWidth * b.Height];
            for (int indexY = 0; indexY < b.Height; indexY++)
            {
                for (int indexX = 0; indexX < b.Width; indexX += numPixelsPerByte)
                {
                    byte outbyte = 0;
                    if (numPixelsPerByte == 4)
                    {
                        Color color1 = b.GetPixel(indexX, indexY);
                        byte b1 = (byte)((helper.GetCGAByte(color1) & 0b11) << 6);
                        byte b2 = 0;
                        byte b3 = 0;
                        byte b4 = 0;

                        if (indexX + 1 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 1, indexY);
                            b2 = (byte)((helper.GetCGAByte(color2) & 0b11) << 4);
                        }
                        if (indexX + 2 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 2, indexY);
                            b3 = (byte)((helper.GetCGAByte(color2) & 0b11) << 2);
                        }
                        if (indexX + 3 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 3, indexY);
                            b4 = (byte)(helper.GetCGAByte(color2) & 0b11);
                        }
                        outbyte = (byte)(b1 + b2 + b3 + b4);
                    }
                    else
                    {
                        Color color1 = b.GetPixel(indexX, indexY);
                        byte b1 = (byte)((helper.GetByte(color1) << 4) & 0xF0);
                        byte b2 = 0;

                        if (indexX + 1 < b.Width)
                        {
                            Color color2 = b.GetPixel(indexX + 1, indexY);
                            b2 = (byte)(helper.GetByte(color2) & 0x0F);
                        }
                        outbyte = (byte)(b1 + b2);
                    }
                        
                    file_bytes[indexY * bufWidth + (indexX / numPixelsPerByte)] = outbyte;
                }
            }
        }

        private bool Checkkeys(Dictionary<int, Bitmap> value, int numKeys, bool excludeDungeonKeys)
        {
            bool valid = true;
            for (int index = 0; index < numKeys; index++)
            {
                if (!value.ContainsKey(index))
                {
                    if (excludeDungeonKeys)
                    {
                        if (index == 8 || index == 24)
                        {
                            continue;
                        }
                    }
                    valid = false;
                    break;
                }
            }
            return valid;
        }

        private bool ValidateImageArray(string key, Dictionary<int, Bitmap> value)
        {
            switch(key)
            {
                case "CREATE":
                    if(value.Count == 11)
                    {
                        return Checkkeys(value, 11, false);
                    }
                    break;
                case "END1":
                    if (value.Count == 3)
                    {
                        return Checkkeys(value, 3, false);
                    }
                    break;
                case "END2":
                    if (value.Count == 3)
                    {
                        return Checkkeys(value, 3, false);
                    }
                    break;
                case "ENDSC":
                    if (value.Count == 1)
                    {
                        return Checkkeys(value, 1, false);
                    }
                    break;
                case "STARTSC":
                    if (value.Count == 3)
                    {
                        return Checkkeys(value, 3, false);
                    }
                    break;
                case "STORY1":
                    if (value.Count == 3)
                    {
                        return Checkkeys(value, 3, false);
                    }
                    break;
                case "STORY2":
                    if (value.Count == 4)
                    {
                        return Checkkeys(value, 4, false);
                    }
                    break;
                case "STORY3":
                    if (value.Count == 2)
                    {
                        return Checkkeys(value, 2, false);
                    }
                    break;
                case "STORY4":
                    if (value.Count == 2)
                    {
                        return Checkkeys(value, 2, false);
                    }
                    break;
                case "STORY5":
                    if (value.Count == 2)
                    {
                        return Checkkeys(value, 2, false);
                    }
                    break;
                case "STORY6":
                    if (value.Count == 8)
                    {
                        return Checkkeys(value, 8, false);
                    }
                    break;
                case "TEXT":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "ULTIMA":
                    if (value.Count == 5)
                    {
                        return Checkkeys(value, 5, false);
                    }
                    break;
                case "DNG1":
                    if (value.Count == 26)
                    {
                        return Checkkeys(value, 28, true);
                    }
                    break;
                case "DNG2":
                    if (value.Count == 26)
                    {
                        return Checkkeys(value, 28, true);
                    }
                    break;
                case "DNG3":
                    if (value.Count == 26)
                    {
                        return Checkkeys(value, 28, true);
                    }
                    break;
                case "ITEMS":
                    if (value.Count == 20)
                    {
                        return Checkkeys(value, 20, false);
                    }
                    break;
                case "MON0":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON1":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON2":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON3":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON4":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON5":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON6":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "MON7":
                    if (value.Count == 6)
                    {
                        return Checkkeys(value, 6, false);
                    }
                    break;
                case "WD":
                    if (value.Count == 1)
                    {
                        return Checkkeys(value, 1, false);
                    }
                    break;
                case "BRITISH":
                    if (value.Count == 1)
                    {
                        return Checkkeys(value, 1, false);
                    }
                    break;
                case "TITLE":
                    if (value.Count == 10)
                    {
                        return Checkkeys(value, 10, false);
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        public void LoadImageU5(byte[] file_bytes, Bitmap b, int numPixelsPerByte)
        {
            if(numPixelsPerByte != 2 && numPixelsPerByte != 4)
            {
                return; // Invalid pixel depth
            }
            pngHelper helper = new pngHelper();

            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 32; ++x_index)
                {
                    long cur_tile = (y_index * 32 + x_index) * 16 * (16 / numPixelsPerByte);
                    for (int pix_y_index = 0; pix_y_index < 16; ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < 16 / numPixelsPerByte; ++pix_x_index)
                        {
                            if(numPixelsPerByte == 4)
                            {
                                byte cur_byte = file_bytes[cur_tile + ((pix_y_index * (16 / numPixelsPerByte)) + pix_x_index)];
                                byte b1 = (byte)((cur_byte >> 6) & 0b11);
                                byte b2 = (byte)((cur_byte >> 4) & 0b11);
                                byte b3 = (byte)((cur_byte >> 2) & 0b11);
                                byte b4 = (byte)(cur_byte & 0b11);

                                Color pixColor1 = helper.GetCGAColor(b1);
                                Color pixColor2 = helper.GetCGAColor(b2);
                                Color pixColor3 = helper.GetCGAColor(b3);
                                Color pixColor4 = helper.GetCGAColor(b4);

                                b.SetPixel((x_index * 16) + pix_x_index * numPixelsPerByte, (y_index * 16) + pix_y_index, pixColor1);
                                b.SetPixel((x_index * 16) + pix_x_index * numPixelsPerByte + 1, (y_index * 16) + pix_y_index, pixColor2);
                                b.SetPixel((x_index * 16) + pix_x_index * numPixelsPerByte + 2, (y_index * 16) + pix_y_index, pixColor3);
                                b.SetPixel((x_index * 16) + pix_x_index * numPixelsPerByte + 3, (y_index * 16) + pix_y_index, pixColor4);
                            }
                            else
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
        }

        public void MakeLZWU5(out byte[]? file_bytes, string strPng, int numPixelsPerByte)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[256 * (512 / numPixelsPerByte)];
                Bitmap image = (Bitmap)System.Drawing.Image.FromFile(strPng);
                if (image.Height != 256 && image.Width != 512)
                {
                    Console.WriteLine("Image must be 512x256 pixels!");
                    return;
                }
                WriteImageU5(destination, image, numPixelsPerByte);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public void WriteImageU5(byte[] file_bytes, Bitmap b, int numPixelsPerByte)
        {
            if(numPixelsPerByte != 2 && numPixelsPerByte != 4)
            {
                return; // Invalid
            }
            pngHelper helper = new pngHelper();
            byte outbyte;

            for (int y_index = 0; y_index < 16; ++y_index)
            {
                for (int x_index = 0; x_index < 32; ++x_index)
                {
                    long cur_tile = (y_index * 32 + x_index) * (16 * (16 / numPixelsPerByte));
                    for (int pix_y_index = 0; pix_y_index < 16; ++pix_y_index)
                    {
                        for (int pix_x_index = 0; pix_x_index < 16; pix_x_index += numPixelsPerByte)
                        {
                            if(numPixelsPerByte == 4)
                            {
                                Color color1 = b.GetPixel((x_index * 16) + pix_x_index, (y_index * 16) + pix_y_index);
                                Color color2 = b.GetPixel((x_index * 16) + pix_x_index + 1, (y_index * 16) + pix_y_index);
                                Color color3 = b.GetPixel((x_index * 16) + pix_x_index + 2, (y_index * 16) + pix_y_index);
                                Color color4 = b.GetPixel((x_index * 16) + pix_x_index + 3, (y_index * 16) + pix_y_index);

                                byte b1 = (byte)(((helper.GetCGAByte(color1) & 0b11) << 6));
                                byte b2 = (byte)(((helper.GetCGAByte(color2) & 0b11) << 4));
                                byte b3 = (byte)(((helper.GetCGAByte(color3) & 0b11) << 2));
                                byte b4 = (byte)(((helper.GetCGAByte(color4) & 0b11) << 0));

                                outbyte = (byte)(b1 + b2 + b3 + b4);       
                            }
                            else
                            {
                                Color color1 = b.GetPixel((x_index * 16) + pix_x_index, (y_index * 16) + pix_y_index);
                                Color color2 = b.GetPixel((x_index * 16) + pix_x_index + 1, (y_index * 16) + pix_y_index);

                                byte b1 = (byte)((helper.GetByte(color1) << 4) & 0xF0);
                                byte b2 = (byte)(helper.GetByte(color2) & 0x0F);

                                outbyte = (byte)(b1 + b2);
                            }
                            file_bytes[cur_tile + ((pix_y_index * (16 / numPixelsPerByte)) + (pix_x_index / numPixelsPerByte))] = outbyte;
                        }
                    }
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

        private void CreateMaskImages(byte[] lzw_out, string strOutDir, string name, int numPixelsPerByte)
        {
            pngHelper helper = new pngHelper();

            int modnum = 0;
            int byteInc = 0;
            if (numPixelsPerByte == 8)
            {
                modnum = 0b1;
                byteInc = 1;
            }
            else if (numPixelsPerByte == 4)
            {
                modnum = 0b11;
                byteInc = 2;
            }
            else if (numPixelsPerByte == 2)
            {
                modnum = 0b1111;
                byteInc = 4;
            }
            else
            {
                return; // Invalid pixels per byte
            }

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

                int bufNum = (8 / numPixelsPerByte) * 2;
                int bufWidth = (bufNum - (width % bufNum)) % bufNum;

                int data_size = (width + bufWidth) * height / numPixelsPerByte;

                if (lzw_out.Length < temp_offset + data_size)
                {
                    return; // Invalid file
                }

                try
                {
                    using (Bitmap b = new(width, height))
                    {
                        int curPos = temp_offset;
                        for (int indexY = 0; indexY < height; indexY++)
                        {
                            for (int indexX = 0; indexX < data_size / height; indexX++)
                            {
                                byte curByte = lzw_out[curPos];
                                for (int byteIndex = 0; byteIndex < 8; byteIndex += byteInc)
                                {
                                    byte curColor = (byte)(((int)(curByte) >> ((8 - byteInc) - byteIndex)) & modnum);
                                    if (indexX * numPixelsPerByte + (byteIndex / byteInc) < width)
                                    {
                                        Color color1;
                                        if (numPixelsPerByte == 4)
                                        {
                                            color1 = helper.GetCGAColor(curColor);
                                        }
                                        else
                                        {
                                            color1 = helper.GetColor(curColor);
                                        }

                                        b.SetPixel(indexX * numPixelsPerByte + (byteIndex / byteInc), indexY, color1);
                                    }
                                }
                                curPos++;
                            }
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

        private void CreateImages(byte[] lzw_out, string strOutDir, string name, int numPixelsPerByte)
        {
            pngHelper helper = new pngHelper();

            int modnum = 0;
            int byteInc = 0;
            if (numPixelsPerByte == 8)
            {
                modnum = 0b1;
                byteInc = 1;
            }
            else if (numPixelsPerByte == 4)
            {
                modnum = 0b11;
                byteInc = 2;
            }
            else if (numPixelsPerByte == 2)
            {
                modnum = 0b1111;
                byteInc = 4;
            }
            else
            {
                return; // Invalid pixels per byte
            }

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
                // For dungeons, offsets 8 & 24 are always 0.
                if (temp_offset == 0)
                {
                    continue;
                }
                int width = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;
                int height = (lzw_out[temp_offset + 1] << 8) + (lzw_out[temp_offset + 0] << 0);
                temp_offset += 2;

                int bufNum = (8 / numPixelsPerByte) * 2;
                int bufWidth = (bufNum - (width % bufNum)) % bufNum;

                int data_size = (width + bufWidth) * height / numPixelsPerByte;

                if (lzw_out.Length < temp_offset + data_size)
                {
                    return; // Invalid file
                }

                try
                {
                    using (Bitmap b = new(width, height))
                    {
                        int curPos = temp_offset;
                        for (int indexY = 0; indexY < height; indexY++)
                        {
                            for (int indexX = 0; indexX < data_size / height; indexX++)
                            {
                                byte curByte = lzw_out[curPos];
                                for (int byteIndex = 0; byteIndex < 8; byteIndex += byteInc)
                                {
                                    byte curColor = (byte)(((int)(curByte) >> ((8 - byteInc) - byteIndex)) & modnum);
                                    if (indexX * numPixelsPerByte + (byteIndex / byteInc) < width)
                                    {
                                        Color color1;
                                        if (numPixelsPerByte == 4)
                                        {
                                            color1 = helper.GetCGAColor(curColor);
                                        }
                                        else
                                        {
                                            color1 = helper.GetColor(curColor);
                                        }

                                        b.SetPixel(indexX * numPixelsPerByte + (byteIndex / byteInc), indexY, color1);
                                    }
                                }
                                curPos++;
                            }
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
    }
}
