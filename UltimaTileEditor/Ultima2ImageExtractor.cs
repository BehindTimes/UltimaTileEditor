using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima2ImageExtractor
    {

        public void MakeU2UpgradePic(byte[] file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            using Bitmap b = new(NUM_X * width, NUM_Y * height);

            PngHelper helper = new PngHelper();

            int byte_counter = 0;

            for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                {
                    for(int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                    {
                        for (int pix_indexX = 0; pix_indexX < width; pix_indexX++)
                        {
                            int curByte = file_bytes[byte_counter];
                            byte b1 = (byte)((curByte >> 0) & 0xf);
                            Color c1 = helper.GetColor(b1);

                            b.SetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY, c1);
                            byte_counter++;
                        }
                    }
                }
            }

            b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
            System.Diagnostics.Debug.WriteLine("Image Created");
        }

        public void MakeU2UpgradeTile(byte[] file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            using Bitmap b = new(NUM_X * width, NUM_Y * height);

            PngHelper helper = new PngHelper();

            int byte_counter = 0;

            for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                {
                    for (int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                    {
                        for (int pix_indexX = 0; pix_indexX < width; pix_indexX += 2)
                        {
                            int curByte = file_bytes[byte_counter];
                            byte b1 = (byte)((curByte >> 4) & 0xf);
                            byte b2 = (byte)((curByte >> 0) & 0xf);
                            Color c1 = helper.GetColor(b1);
                            Color c2 = helper.GetColor(b2);

                            b.SetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY, c1);
                            b.SetPixel(tile_indexX * width + pix_indexX + 1, tile_indexY * height + pix_indexY, c2);
                            byte_counter++;
                        }
                    }
                }
            }

            b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
            System.Diagnostics.Debug.WriteLine("Image Created");
        }

        public void MakeU2UpgradeTileCGA(byte[] file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            using Bitmap b = new(NUM_X * width, NUM_Y * height);

            PngHelper helper = new PngHelper();

            int byte_counter = 0;

            for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                {
                    for (int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                    {
                        for (int pix_indexX = 0; pix_indexX < width; pix_indexX += 4)
                        {
                            int curByte = file_bytes[byte_counter];
                            byte b1 = (byte)((curByte >> 6) & 0x3);
                            byte b2 = (byte)((curByte >> 4) & 0x3);
                            byte b3 = (byte)((curByte >> 2) & 0x3);
                            byte b4 = (byte)((curByte >> 0) & 0x3);
                            Color c1 = helper.GetCGAColor(b1);
                            Color c2 = helper.GetCGAColor(b2);
                            Color c3 = helper.GetCGAColor(b3);
                            Color c4 = helper.GetCGAColor(b4);

                            b.SetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY, c1);
                            b.SetPixel(tile_indexX * width + pix_indexX + 1, tile_indexY * height + pix_indexY, c2);
                            b.SetPixel(tile_indexX * width + pix_indexX + 2, tile_indexY * height + pix_indexY, c3);
                            b.SetPixel(tile_indexX * width + pix_indexX + 3, tile_indexY * height + pix_indexY, c4);
                            byte_counter++;
                        }
                    }
                }
            }

            b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
            System.Diagnostics.Debug.WriteLine("Image Created");
        }

        private void ExtractUpgradeImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            PngHelper helper = new PngHelper();

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("EGATILES"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length != 8320)
                    {
                        return;
                    }
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);

                    if (value != null)
                    {
                        string fullPath = Path.Combine(strImageDir, value + ".png");
                        MakeU2UpgradeTile(file_bytes, fullPath, 13, 5, 16, 16);
                    }
                }
                else if (image.EndsWith("CGATILES"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length != 4160)
                    {
                        return;
                    }
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);

                    if (value != null)
                    {
                        string fullPath = Path.Combine(strImageDir, value + ".png");
                        MakeU2UpgradeTileCGA(file_bytes, fullPath, 13, 5, 16, 16);
                    }
                }
                else if (image.EndsWith(".EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length != 64000)
                    {
                        return;
                    }
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);

                    if (value != null)
                    {
                        string fullPath = Path.Combine(strImageDir, value + ".png");
                        MakeU2UpgradePic(file_bytes, fullPath, 1, 1, 320, 200);
                    }
                }
                else if (imageType == 1)
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 0x4000;
                        if (image.EndsWith("PICDRA"))
                        {
                            fileSize = 0x4080; // ? Is this an error, or is there a reason for this?
                        }
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

        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            if(palette == 1 || palette == 2)
            {
                ExtractUpgradeImages(images, strDataDir, strImageDir, imageType, palette);
                return;
            }
            int dataStartOffset = 0x7c40;
            int tileSize = 66;
            int numTiles = 64;
            PngHelper helper = new PngHelper();

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("ULTIMAII.EXE"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    // Sanity check to make sure it's not a modified Ultima 2
                    if(file_bytes.Length != 37344)
                    {
                        return;
                    }
                    byte[] tile_data = new byte[tileSize * numTiles];
                    Array.Copy(file_bytes, dataStartOffset, tile_data, 0, tileSize * numTiles);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strImageDir, "ULTIMAII.png");
                        MakePngU2(tile_data, fullPath, numTiles, tileSize);
                    }
                }
                else if (imageType == 1)
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 0x4000;
                        if (image.EndsWith("PICDRA"))
                        {
                            fileSize = 0x4080; // ? Is this an error, or is there a reason for this?
                        }
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

        public void CompressImagesUpgradeEGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            PngHelper helper = new PngHelper();
            bool written = false;

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("EGATILES.png"))
                {
                    byte[]? file_bytes;
                    MakeU2UpgradeEGATiles(out file_bytes, image, 13, 5, 16, 16);

                    if(file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "EGATILES");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
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
                        byte[]? file_bytes;
                        MakeU2UpgradeEGAPic(out file_bytes, image);

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".EGA");

                            using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                            {
                                binWriter.Write(file_bytes);
                                written = true;
                            }
                        }
                    }
                }
            }

            if (written)
            {
                MessageBox.Show("File written!");
            }
        }

        public void CompressImagesUpgradeCGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            PngHelper helper = new PngHelper();
            bool written = false;

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("CGATILES.png"))
                {
                    byte[]? file_bytes;
                    MakeU2UpgradeCGATiles(out file_bytes, image, 13, 5, 16, 16);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "CGATILES");

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
            if (palette == 1) // EGA Upgrade
            {
                CompressImagesUpgradeEGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }
            else if (palette == 2) // CGA Upgrade
            {
                if (imageType != 1) // The PIC files haven't changed format
                {
                    CompressImagesUpgradeCGA(images, strDataDir, strImageDir, imageType, palette);
                    return;
                }  
            }

            int dataStartOffset = 0x7c40;
            bool written = false;
            PngHelper helper = new PngHelper();

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("ULTIMAII.png"))
                {
                    byte[]? file_bytes;
                    MakeU2(out file_bytes, image);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "ULTIMAII.EXE");
                        byte[] exe_bytes = File.ReadAllBytes(fullPath);
                        // Sanity check to make sure it's not a modified Ultima 2
                        if (exe_bytes.Length != 37344)
                        {
                            return;
                        }
                        if (exe_bytes.Length > dataStartOffset + file_bytes.Length)
                        {
                            Array.Copy(file_bytes, 0, exe_bytes, dataStartOffset, file_bytes.Length);
                        }
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(exe_bytes);
                            written = true;
                        }
                    }
                }
                else if (imageType == 1)
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
                        if(write0x4000)
                        {
                            outData[0x4000] = 0x1a; // ? Is this a mistake on thier part, or what is the actual reason for the extra 128 bytes?
                        }
                        string fullPath = Path.Combine(strDataDir, value);

                        helper.MakeU2PicData(ref outData, image, fullPath);
                        written = true;
                    }
                }
            }
            if(written)
            {
                MessageBox.Show("File written!");
            }
        }

        private void LoadImageU2(byte[] file_bytes, Bitmap b)
        {
            PngHelper helper = new PngHelper();
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
                                Color curColor = helper.GetCGAColor(tempbytes);
                                b.SetPixel(((x_index * 4) + tempX) + (tile_indexX * 16), y_index + (tile_indexY * 16), curColor);
                                tempX++;
                            }
                        }
                    }
                }
            }
        }

        private void MakePngU2(byte[] lzw, string strPng, int numTiles, int tileSize)
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

        private void WriteImageU2(byte[] file_bytes, Bitmap b)
        {
            PngHelper helper = new PngHelper();
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

                            byte b1 = helper.GetCGAByte(color1);
                            byte b2 = helper.GetCGAByte(color2);
                            byte b3 = helper.GetCGAByte(color3);
                            byte b4 = helper.GetCGAByte(color4);

                            byte finalbyte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + (b4 << 0));

                            file_bytes[curTile * tile_size + 2 + (row * NUM_COL) + col] = finalbyte;
                        }
                    }
                }
            }
        }

        private void MakeU2UpgradeEGAPic(out byte[]? file_bytes, string strPng)
        {
            PngHelper helper = new PngHelper();
            file_bytes = null;

            try
            {
                byte[] destination = new byte[64000];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", 320, 200);
                    return;
                }

                int byte_counter = 0;

                for (int pix_indexY = 0; pix_indexY < 200; pix_indexY++)
                {
                    for (int pix_indexX = 0; pix_indexX < 320; pix_indexX++)
                    {
                        Color c1 = image.GetPixel(pix_indexX, pix_indexY);

                        byte b1 = helper.GetByte(c1);

                        destination[byte_counter] = b1;
                        byte_counter++;
                    }
                }

                file_bytes = destination;
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private void MakeU2UpgradeEGATiles(out byte[]? file_bytes, string strPng, int columns, int rows, int width, int height)
        {
            PngHelper helper = new PngHelper();
            file_bytes = null;

            try
            {
                byte[] destination = new byte[(columns * rows * width * height) / 2];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != (rows * height) && image.Width != (columns * width))
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", (columns * width), (rows * height));
                    return;
                }

                int byte_counter = 0;
                for (int tile_indexY = 0; tile_indexY < rows; tile_indexY++)
                {
                    for (int tile_indexX = 0; tile_indexX < columns; tile_indexX++)
                    {
                        for (int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                        {
                            for (int pix_indexX = 0; pix_indexX < width; pix_indexX += 2)
                            {

                                Color c1 = image.GetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY);
                                Color c2 = image.GetPixel(tile_indexX * width + pix_indexX + 1, tile_indexY * height + pix_indexY);

                                byte b1 = helper.GetByte(c1);
                                byte b2 = helper.GetByte(c2);

                                byte finalbyte = (byte)((b1 << 4) + (b2 << 0));

                                destination[byte_counter] = finalbyte;
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
        }

        private void MakeU2UpgradeCGATiles(out byte[]? file_bytes, string strPng, int columns, int rows, int width, int height)
        {
            PngHelper helper = new PngHelper();
            file_bytes = null;

            try
            {
                byte[] destination = new byte[(columns * rows * width * height) / 4];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != (rows * height) && image.Width != (columns * width))
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", (columns * width), (rows * height));
                    return;
                }

                int byte_counter = 0;
                for (int tile_indexY = 0; tile_indexY < rows; tile_indexY++)
                {
                    for (int tile_indexX = 0; tile_indexX < columns; tile_indexX++)
                    {
                        for (int pix_indexY = 0; pix_indexY < height; pix_indexY++)
                        {
                            for (int pix_indexX = 0; pix_indexX < width; pix_indexX += 4)
                            {

                                Color c1 = image.GetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY);
                                Color c2 = image.GetPixel(tile_indexX * width + pix_indexX + 1, tile_indexY * height + pix_indexY);
                                Color c3 = image.GetPixel(tile_indexX * width + pix_indexX + 2, tile_indexY * height + pix_indexY);
                                Color c4 = image.GetPixel(tile_indexX * width + pix_indexX + 3, tile_indexY * height + pix_indexY);

                                byte b1 = helper.GetCGAByte(c1);
                                byte b2 = helper.GetCGAByte(c2);
                                byte b3 = helper.GetCGAByte(c3);
                                byte b4 = helper.GetCGAByte(c4);

                                byte finalbyte = (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + (b4 << 0));

                                destination[byte_counter] = finalbyte;
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
        }

        private void MakeU2(out byte[]? file_bytes, string strPng)
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
                    Debug.WriteLine("Image must be 128x128 pixels!");
                    return;
                }
                WriteImageU2(destination, image);
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
