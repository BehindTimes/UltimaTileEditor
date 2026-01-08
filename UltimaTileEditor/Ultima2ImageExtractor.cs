using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima2ImageExtractor
    {
        public void ExtractImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            int dataStartOffset = 0x7c40;
            int tileSize = 66;
            int numTiles = 64;

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
                        MakeU2Pic(file_bytes, fullPath);
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            int dataStartOffset = 0x7c40;
            bool written = false;

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

                        MakeU2PicData(ref outData, image, fullPath);
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
            pngHelper helper = new pngHelper();
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

        private void WritePicU2(ref byte[] file_bytes, Bitmap b, string strOutFile)
        {
            pngHelper helper = new pngHelper();
            const int planeSize = 0x2000;
            const int rowSize = 80;
            int curPos = 0;
            int numRows = 200;

            for (int planeIndex = 0; planeIndex < 2; planeIndex++)
            {
                curPos = planeSize * planeIndex;

                for (int indexY = 0; indexY < numRows; indexY += 2)
                {
                    for (int index = 0; index < rowSize; index++)
                    {
                        byte curByte = 0;
                        Color c1 = b.GetPixel(index * 4 + 0, indexY + planeIndex);
                        Color c2 = b.GetPixel(index * 4 + 1, indexY + planeIndex);
                        Color c3 = b.GetPixel(index * 4 + 2, indexY + planeIndex);
                        Color c4 = b.GetPixel(index * 4 + 3, indexY + planeIndex);

                        byte b1 = helper.GetCGAByte(c1);
                        byte b2 = helper.GetCGAByte(c2);
                        byte b3 = helper.GetCGAByte(c3);
                        byte b4 = helper.GetCGAByte(c4);

                        curByte += (byte)((b1 << 6) + (b2 << 4) + (b3 << 2) + b4);
                        file_bytes[curPos] = curByte;

                        curPos++;
                    }
                }
            }

            using (BinaryWriter binWriter = new BinaryWriter(File.Open(strOutFile, FileMode.Create)))
            {
                binWriter.Write(file_bytes);
            }
        }

        private void WriteImageU2(byte[] file_bytes, Bitmap b)
        {
            pngHelper helper = new pngHelper();
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

        private void MakeU2PicData(ref byte[] file_bytes, string strPng, string strOutFile)
        {
            try
            {
                Bitmap image = (Bitmap)Image.FromFile(strPng);
                if (image.Height != 200 && image.Width != 320)
                {
                    Debug.WriteLine("Image must be 320x200 pixels!");
                    return;
                }
                WritePicU2(ref file_bytes, image, strOutFile);
            }
            catch (IOException)
            {
                Debug.WriteLine("PNG file does not exist!");
                return;
            }
        }

        private void MakeU2Pic(byte[] file_bytes, string strPng)
        {
            pngHelper helper = new pngHelper();

            using (Bitmap b = new Bitmap(320, 200))
            {
                const int planeSize = 0x2000;
                const int rowSize = 80;
                int curPos = 0;
                int numRows = 200;
                for(int planeIndex = 0; planeIndex < 2; planeIndex++)
                {
                    curPos = planeSize * planeIndex;

                    for (int indexY = 0; indexY < numRows; indexY += 2)
                    {
                        for (int index = 0; index < rowSize; index++)
                        {
                            byte curByte = file_bytes[curPos];
                            byte b1 = (byte)((curByte >> 6) & 0b11);
                            byte b2 = (byte)((curByte >> 4) & 0b11);
                            byte b3 = (byte)((curByte >> 2) & 0b11);
                            byte b4 = (byte)((curByte >> 0) & 0b11);

                            Color c1 = helper.GetCGAColor(b1);
                            Color c2 = helper.GetCGAColor(b2);
                            Color c3 = helper.GetCGAColor(b3);
                            Color c4 = helper.GetCGAColor(b4);

                            b.SetPixel(index * 4 + 0, indexY + planeIndex, c1);
                            b.SetPixel(index * 4 + 1, indexY + planeIndex, c2);
                            b.SetPixel(index * 4 + 2, indexY + planeIndex, c3);
                            b.SetPixel(index * 4 + 3, indexY + planeIndex, c4);

                            curPos++;
                        }
                    }
                }

                b.Save(strPng, System.Drawing.Imaging.ImageFormat.Png);
                System.Diagnostics.Debug.WriteLine("Image Created");
            }
        }
    }
}
