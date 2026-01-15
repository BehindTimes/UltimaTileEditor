using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima1ImageExtractor
    {
        public static void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            foreach (string tempimage in images)
            {
                string image = Path.Combine(strDataDir, tempimage);
                string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                if (value != null)
                {
                    if (image.EndsWith("TILES.BIN"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            switch (palette)
                            {
                                case 1: // CGA - Not supported
                                    break;
                                case 2: // Tandy - Not supported
                                    MakePngU1Tandy(file_bytes, fullPath, 13, 4, 16, 16);
                                    break;
                                default: // EGA
                                    MakePngU1EGA(file_bytes, fullPath, 13, 4, true);
                                    break;
                            }
                        }
                    }
                    else if (image.EndsWith("MOND.BIN"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            switch (palette)
                            {
                                case 1: // CGA - Not supported
                                    break;
                                case 2: // Tandy - Not supported
                                    MakePngU1Tandy(file_bytes, fullPath, 19, 1, 16, 16);
                                    break;
                                default: // EGA
                                    MakePngU1EGA(file_bytes, fullPath, 19, 1, true);
                                    break;
                            }
                        }
                    }
                    else if (image.EndsWith("TOWN.BIN"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            switch (palette)
                            {
                                case 1: // CGA - Not supported
                                    break;
                                case 2: // Tandy - Not supported
                                    MakePngU1Tandy(file_bytes, fullPath, 17, 3, 8, 8);
                                    break;
                                default: // EGA
                                    MakePngU1EGA(file_bytes, fullPath, 17, 3, false);
                                    break;
                            }
                        }
                    }
                    else if (image.EndsWith("FIGHT.BIN"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            switch (palette)
                            {
                                case 1: // CGA - Not supported
                                    break;
                                case 2: // Tandy - Not supported
                                    MakePngU1Tandy(file_bytes, fullPath, 7, 2, 24, 19);
                                    break;
                                default: // EGA - Not supported
                                    break;
                            }
                        }
                    }
                    else if (image.EndsWith("SPACE.BIN"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            switch (palette)
                            {
                                case 1: // CGA - Not supported
                                    break;
                                case 2: // Tandy - Not supported
                                    MakePngU1Tandy(file_bytes, fullPath, 6, 4, 32, 19);
                                    break;
                                default: // EGA - Not supported
                                    break;
                            }
                        }
                    }
                    else if (image.EndsWith("CASTLE.16"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            if (file_bytes.Length == 32000)
                            {
                                using Bitmap b = new(320, 200);
                                PngHelper helper = new();
                                helper.LoadImage320x200(file_bytes, b, 1);
                                b.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
                                Console.WriteLine("Image Created");
                            }
                        }
                    }
                    else if (image.EndsWith("NIF.BIN"))
                    {
                        byte[] file_bytes = File.ReadAllBytes(image);
                        if (file_bytes.Length == 6720)
                        {
                            string fullPath = Path.Combine(strImageDir, value + ".png");
                            CreateSimpleBitmap(file_bytes, 320, 168, fullPath);
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
                string? value = System.IO.Path.GetFileNameWithoutExtension(image);
                if (value != null)
                {
                    if (image.EndsWith("TILES.png"))
                    {
                        byte[]? file_bytes = null;
                        switch (palette)
                        {
                            case 1: // CGA - Not supported
                                break;
                            case 2: // Tandy - Not supported
                                MakeU1Tandy(out file_bytes, image, 13, 4, 16, 16);
                                break;
                            default: // EGA
                                MakeU1(out file_bytes, image, 13, 4, true);
                                break;
                        }

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".BIN");
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                        }
                    }
                    else if (image.EndsWith("MOND.png"))
                    {
                        byte[]? file_bytes = null;
                        switch (palette)
                        {
                            case 1: // CGA - Not supported
                                break;
                            case 2: // Tandy - Not supported
                                MakeU1Tandy(out file_bytes, image, 19, 1, 16, 16);
                                break;
                            default: // EGA
                                MakeU1(out file_bytes, image, 19, 1, true);
                                break;
                        }

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".BIN");
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                        }
                    }
                    else if (image.EndsWith("TOWN.png"))
                    {
                        byte[]? file_bytes = null;
                        switch (palette)
                        {
                            case 1: // CGA - Not supported
                                break;
                            case 2: // Tandy - Not supported
                                MakeU1Tandy(out file_bytes, image, 17, 3, 8, 8);
                                break;
                            default: // EGA
                                MakeU1(out file_bytes, image, 17, 3, false);
                                break;
                        } 

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".BIN");
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                    else if (image.EndsWith("FIGHT.png"))
                    {
                        byte[]? file_bytes = null;
                        switch (palette)
                        {
                            case 1: // CGA - Not supported
                                break;
                            case 2: // Tandy - Not supported
                                MakeU1Tandy(out file_bytes, image, 7, 2, 24, 19);
                                break;
                            default: // EGA - Not supported
                                break;
                        }

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".BIN");
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                    else if (image.EndsWith("SPACE.png"))
                    {
                        byte[]? file_bytes = null;
                        switch (palette)
                        {
                            case 1: // CGA - Not supported
                                break;
                            case 2: // Tandy - Not supported
                                MakeU1Tandy(out file_bytes, image, 6, 4, 32, 19);
                                break;
                            default: // EGA - Not supported
                                break;
                        }

                        if (file_bytes != null)
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".BIN");
                            using BinaryWriter binWriter = new(File.Open(fullPath, FileMode.Create));
                            binWriter.Write(file_bytes);
                            written = true;
                        }
                    }
                    else if (image.EndsWith("CASTLE.png"))
                    {
                        if (palette == 0) // 16 color
                        {
                            string fullPath = Path.Combine(strDataDir, value + "_test.16");
                            MakeU1Image(out byte[]? file_bytes, image);
                            if (null != file_bytes)
                            {
                                WriteU1Image(file_bytes, fullPath);
                                written = true;
                            }
                        }
                        else if (palette == 1) // 4 color - Not Supported
                        {
                            string fullPath = Path.Combine(strDataDir, value + "_test.4");
                        }
                        else // Tandy - Not Supported
                        {

                        }
                    }
                    else if (image.EndsWith("NIF.png"))
                    {
                        if (palette == 0) // 16 color
                        {
                            string fullPath = Path.Combine(strDataDir, value + ".BIN");
                            MakeSimpleImage(out byte[]? file_bytes, image, 320, 168);
                            if (null != file_bytes)
                            {
                                WriteU1Image(file_bytes, fullPath);
                                written = true;
                            }
                        }
                    }
                    else
                    {
                        
                    }
                }
            }
            if(written)
            {
                MessageBox.Show("Files written!");
            }
        }

        private static void MakeSimpleImage(out byte[]? file_bytes, string strPng, int width, int height)
        {
            file_bytes = null;
            try
            {
                PngHelper helper = new();
                byte[] destination = new byte[40 * 168];
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 168 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be 320x168 pixels!");
                    return;
                }
                int curPos = 0;
                for (int indexY = 0; indexY < height; indexY++)
                {
                    for (int indexX = 0; indexX < width / 8; indexX++)
                    {
                        byte curByte = 0;

                        for (int tempIndexX = 0; tempIndexX < 8; tempIndexX++)
                        {
                            int curX = indexX * 8 + tempIndexX;
                            int curY = indexY;

                            Color tempColor = image.GetPixel(curX, curY);
                            if (tempColor.R == 255 && tempColor.G == 255 && tempColor.B == 255)
                            {
                                curByte |= (byte)(1 << (7 - tempIndexX));
                            }
                        }
                        destination[curPos] = curByte;
                        curPos++;
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

        private static void CreateSimpleBitmap(byte[] file_data, int width, int height, string strBitmap)
        {
            using Bitmap b = new(width, height);
            int curPos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width / 8; indexX++)
                {
                    byte curByte = file_data[curPos];

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

        private static void WriteU1Image(byte[] file_bytes, string strOut)
        {
            using BinaryWriter binWriter = new(File.Open(strOut, FileMode.Create));
            binWriter.Write(file_bytes);
        }

        private static void MakeU1Image(out byte[]? file_bytes, string strPng)
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
                // Because of the custom palette which shares the same colors,
                // a perfect replication of the original file cannot be accomplished
                helper.CreateImageWithPalette(destination, image, 320, 200, 1);
                file_bytes = destination;
            }
            catch (IOException)
            {
                Console.WriteLine("PNG file does not exist!");
                return;
            }
        }

        public static void LoadImageU1_8(byte[] file_bytes, Bitmap b, int width, int height)
        {
            PngHelper helper = new();
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

        public static void LoadImageU1(byte[] file_bytes, Bitmap b, int width, int height)
        {
            PngHelper helper = new();
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

        public static void MakePngU1EGA(byte[] lzw, string strPng, int width, int height, bool is16)
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
                using Bitmap b = new(width * tilesize, height * tilesize);
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
            catch (IOException)
            {
                Console.WriteLine("LZW file does not exist!");
                return;
            }
        }

        public static void WriteImageU1_8(byte[] file_bytes, Bitmap b, int width, int height)
        {
            PngHelper helper = new();
            int pos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    for (int row = 0; row < 8; row++)
                    {
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

        public static void WriteImageU1(byte[] file_bytes, Bitmap b, int width, int height)
        {
            PngHelper helper = new();
            int pos = 0;
            for (int indexY = 0; indexY < height; indexY++)
            {
                for (int indexX = 0; indexX < width; indexX++)
                {
                    for (int row = 0; row < 16; row++)
                    {
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

        public static void MakeU1Tandy(out byte[]? file_bytes, string strPng, int numXtiles, int numYtiles, int tile_width, int tile_height)
        {
            PngHelper helper = new();
            const int numPixelsPerTile = 2;
            file_bytes = null;
            try
            {
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if(image.Width != numXtiles * tile_width && image.Height != numYtiles * tile_height)
                {
                    Debug.WriteLine("Image must be {0}x{1} pixels!", numXtiles * tile_width, numYtiles * tile_height);
                    return;
                }
                byte[] destination = new byte[((numXtiles * tile_width) / numPixelsPerTile) * (numYtiles * tile_height)];
                int curTile = 0;
                int tileSize = (tile_width / numPixelsPerTile) * tile_height;
                for (int tileYindex = 0; tileYindex < numYtiles; tileYindex++)
                {
                    for (int tileXindex = 0; tileXindex < numXtiles; tileXindex++)
                    {
                        for (int indexY = 0; indexY < tile_height; ++indexY)
                        {
                            for (int indexX = 0; indexX < (tile_width / numPixelsPerTile); indexX++)
                            {
                                Color c1 = image.GetPixel((tileXindex * tile_width) + (indexX * numPixelsPerTile), tileYindex * tile_height + indexY);
                                Color c2 = image.GetPixel((tileXindex * tile_width) + (indexX * numPixelsPerTile) + 1, tileYindex * tile_height + indexY);
                                byte color1 = helper.GetByte(c1);
                                byte color2 = helper.GetByte(c2);
                                byte color = (byte)((color1 << 4) + color2);
                                destination[(tileSize * curTile) + indexY * (tile_width / numPixelsPerTile) + indexX] = color;

                            }
                        }
                        curTile++;
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

        public static void MakeU1(out byte[]? file_bytes, string strPng, int width, int height, bool is16)
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
                    Debug.WriteLine("Image must be {0}x{1} pixels!", width * tilesize, height * tilesize);
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
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        static void MakePngU1Tandy(byte[] file_data, string strPng, int numXtiles, int numYtiles, int tilewidth, int tileheight)
        {
            PngHelper helper = new();
            using Bitmap b = new(numXtiles * tilewidth, numYtiles * tileheight);
            const int numPixelsPerTile = 2;
            int curTile = 0;
            int tileSize = (tilewidth / numPixelsPerTile) * tileheight;
            for (int tileYindex = 0; tileYindex < numYtiles; tileYindex++)
            {
                for (int tileXindex = 0; tileXindex < numXtiles; tileXindex++)
                {
                    for (int indexY = 0; indexY < tileheight; ++indexY)
                    {
                        for (int indexX = 0; indexX < (tilewidth / numPixelsPerTile); indexX++)
                        {
                            byte curByte = file_data[(tileSize * curTile) + indexY * (tilewidth / numPixelsPerTile) + indexX];
                            byte color1 = (byte)((curByte >> 4) & 0xF);
                            byte color2 = (byte)(curByte & 0xF);

                            Color c1 = helper.GetColor(color1);
                            Color c2 = helper.GetColor(color2);

                            b.SetPixel((tileXindex * tilewidth) + (indexX * numPixelsPerTile), tileYindex * tileheight + indexY, c1);
                            b.SetPixel((tileXindex * tilewidth) + (indexX * numPixelsPerTile) + 1, tileYindex * tileheight + indexY, c2);

                        }
                    }
                    curTile++;
                }
            }
            b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
            Console.WriteLine("Image Created");
        }
    }
}
