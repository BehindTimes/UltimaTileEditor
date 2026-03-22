using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima3ImageExtractor
    {
        public void ExtractUpgradeImagesEGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            PngHelper helper = new PngHelper();
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                if (image.EndsWith("SHAPES.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null && file_bytes.Length == 10240)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + "_EGA.png");
                            MakePngU3EGA(file_bytes, fullPath, 10, 8, 16, 16);
                        }
                    }
                }
                else if (image.EndsWith("CHARSET.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length == 4096)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + "_EGA.png");
                            MakePngU3EGA(file_bytes, fullPath, 16, 8, 8, 8);
                        }
                    }
                }
                else if (imageType == 2)
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 32000;
                        byte[] file_bytes = File.ReadAllBytes(image);

                        if(file_bytes.Length == fileSize)
                        {
                            string fullPath = Path.Combine(strImageDir, value + "_EGA.png");
                            MakePngU3EGA(file_bytes, fullPath, 1, 1, 320, 200);
                        }
                    }
                }
                else if (imageType == 3) // The animation
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 11776;
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes.Length != fileSize)
                        {
                            return;
                        }
                        string fullPath = Path.Combine(strImageDir, value + "_EGA.png");
                        MakePngU3EGA(file_bytes, fullPath, 1, 16, 92, 16);
                    }
                }
                else if (imageType == 5) // The moons
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 256;
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes.Length != fileSize)
                        {
                            return;
                        }
                        string fullPath = Path.Combine(strImageDir, value + "_EGA.png");
                        MakePngU3EGA(file_bytes, fullPath, 4, 2, 8, 8);
                    }
                }
            }
        }

        public void ExtractUpgradeImagesVGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {

        }

        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            if (palette == 1)
            {
                ExtractUpgradeImagesEGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }
            else if (palette == 2)
            {
                ExtractUpgradeImagesVGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }
            PngHelper helper = new PngHelper();
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
                else if (image.EndsWith("MOONS.ULT"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes.Length == 0x80)
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                        if (value != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            MakeMoonsPngU3(file_bytes, fullPath);
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
                else if (imageType == 3) // The animation
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 5888;
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes.Length != fileSize)
                        {
                            return;
                        }
                        string fullPath = Path.Combine(strImageDir, value + ".png");
                        using (Bitmap b = new Bitmap(92, 256))
                        {
                            helper.CreateCGAImage(file_bytes, b, 23, 256);
                            b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                            Console.WriteLine("Image Created");
                        }
                        
                    }
                }
                else if (imageType == 4) // Lord British Signature
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        int fileSize = 640;
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes.Length != fileSize)
                        {
                            return;
                        }
                        string fullPath = Path.Combine(strImageDir, value + ".png");
                        using (Bitmap b = new Bitmap(256, 256))
                        {
                            CreateSignature(file_bytes, b);
                            b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                        }

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
                if (image.EndsWith("SHAPES_EGA.png"))
                {
                    byte[]? file_bytes;
                    MakeU3UpgradeEGATiles(out file_bytes, image, 10, 8, 16, 16);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "SHAPES.EGA");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("ANIMATE_EGA.png"))
                {
                    byte[]? file_bytes;
                    MakeU3UpgradeEGATiles(out file_bytes, image, 1, 16, 92, 16);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "ANIMATE.EGA");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("BLANK_EGA.png"))
                {
                    byte[]? file_bytes;
                    MakeU3UpgradeEGATiles(out file_bytes, image, 1, 1, 320, 200);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "BLANK.EGA");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("CHARSET_EGA.png"))
                {
                    byte[]? file_bytes;
                    MakeU3UpgradeEGATiles(out file_bytes, image, 16, 8, 8, 8);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "CHARSET.EGA");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("EXOD_EGA.png"))
                {
                    byte[]? file_bytes;
                    MakeU3UpgradeEGATiles(out file_bytes, image, 1, 1, 320, 200);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "EXOD.EGA");

                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                }
                else if (image.EndsWith("MOONS_EGA.png"))
                {
                    byte[]? file_bytes;
                    MakeU3UpgradeEGATiles(out file_bytes, image, 4, 2, 8, 8);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strDataDir, "MOONS.EGA");

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

        public void CompressImagesUpgradeVGA(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
        }

        private void MakeU3UpgradeCGATiles(out byte[]? file_bytes, string strPng, int columns, int rows, int width, int height)
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

        private void MakeU3UpgradeEGATiles(out byte[]? file_bytes, string strPng, int columns, int rows, int width, int height)
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

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            if (palette == 1) // EGA Upgrade
            {
                CompressImagesUpgradeEGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }
            else if (palette == 2) // VGA Upgrade
            {
                CompressImagesUpgradeVGA(images, strDataDir, strImageDir, imageType, palette);
                return;
            }
            PngHelper helper = new PngHelper();
            bool written = false;

            foreach (string tempimage in images)
            {
                string image = Path.Combine(strImageDir, tempimage);
                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    MakeU3(out file_bytes, image, 16, 16, 10, 8);

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
                else if (image.EndsWith("MOONS.png"))
                {
                    byte[]? file_bytes;
                    MakeU3Moons(out file_bytes, image, 4, 2, 8, 8);

                    if (file_bytes != null && file_bytes.Length == 128)
                    {
                        string fullPath = Path.Combine(strDataDir, "MOONS_Test.ULT");
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
                else if (imageType == 3)
                {
                    string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                    if (value != null)
                    {
                        byte[]? file_bytes;
                        helper.MakeCGAImage(out file_bytes, image, 23, 256);

                        if (file_bytes != null && file_bytes.Length == 5888)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".DAT");
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

        private void CreateSignature(byte[] file_bytes, Bitmap b)
        {
            Color gray = Color.FromArgb(0xAA, 0xAA, 0xAA);

            using (Graphics g = Graphics.FromImage(b))
            {
                // Fill the entire drawing surface with a specific color (e.g., Green)
                g.Clear(Color.Black);
            }

            for (int index = 0; index < file_bytes.Length; index += 2)
            {
                int x = file_bytes[index];
                int y = file_bytes[index + 1];
                if (x > 0 && y > 0 && x < 256 && y < 256)
                {
                    b.SetPixel(x, 256 - y, gray);
                    b.SetPixel(x + 1, 256 - y, gray);
                }
            }
        }

        private void MakePngU3(byte[] lzw, string strPng)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != 0x1400)
                {
                    return;
                }
                using (Bitmap b = new Bitmap(160, 128))
                {
                    LoadImageU3(file_bytes, b, 16, 16, 10, 8);
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

        public void MakePngU3EGA(byte[] file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
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

        public void MakePngU3CGA(byte[] file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            using Bitmap b = new(NUM_X * width, NUM_Y * height);

            PngHelper helper = new PngHelper();

            int byte_counter = 0;

            for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
            {
                for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                {
                    for (int scanline_index = 0; scanline_index < 2; scanline_index++)
                    {
                        for (int pix_indexY = 0; pix_indexY < height; pix_indexY += 2)
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

                                b.SetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY + scanline_index, c1);
                                b.SetPixel(tile_indexX * width + pix_indexX + 1, tile_indexY * height + pix_indexY + scanline_index, c2);
                                b.SetPixel(tile_indexX * width + pix_indexX + 2, tile_indexY * height + pix_indexY + scanline_index, c3);
                                b.SetPixel(tile_indexX * width + pix_indexX + 3, tile_indexY * height + pix_indexY + scanline_index, c4);
                                byte_counter++;
                            }
                        }
                    }
                }
            }

            b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
            System.Diagnostics.Debug.WriteLine("Image Created");
        }

        public void MakeMoonsPngU3(byte[] lzw, string strPng)
        {
            try
            {
                byte[] file_bytes = lzw;
                if (file_bytes.Length != 128)
                {
                    return;
                }
                MakePngU3CGA(file_bytes, strPng, 4, 2, 8, 8);

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

            PngHelper helper = new PngHelper();
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
            PngHelper helper = new PngHelper();
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

        public void MakeU3Moons(out byte[]? file_bytes, string strPng, int NUM_X, int NUM_Y, int width, int height)
        {
            file_bytes = null;
            try
            {
                byte[] destination = new byte[NUM_X * (width / 4) * NUM_Y * height];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != NUM_Y * height && image.Width != NUM_X * width)
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", NUM_X * width, NUM_Y * height);
                    return;
                }

                PngHelper helper = new PngHelper();

                int byte_counter = 0;

                for (int tile_indexY = 0; tile_indexY < NUM_Y; tile_indexY++)
                {
                    for (int tile_indexX = 0; tile_indexX < NUM_X; tile_indexX++)
                    {
                        for (int scanline_index = 0; scanline_index < 2; scanline_index++)
                        {
                            for (int pix_indexY = 0; pix_indexY < height; pix_indexY += 2)
                            {
                                for (int pix_indexX = 0; pix_indexX < width; pix_indexX += 4)
                                {

                                    Color c1 = image.GetPixel(tile_indexX * width + pix_indexX, tile_indexY * height + pix_indexY + scanline_index);
                                    Color c2 = image.GetPixel(tile_indexX * width + pix_indexX + 1, tile_indexY * height + pix_indexY + scanline_index);
                                    Color c3 = image.GetPixel(tile_indexX * width + pix_indexX + 2, tile_indexY * height + pix_indexY + scanline_index);
                                    Color c4 = image.GetPixel(tile_indexX * width + pix_indexX + 3, tile_indexY * height + pix_indexY + scanline_index);

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
