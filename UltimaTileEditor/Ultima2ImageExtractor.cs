using System;
using System.Collections.Generic;
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
            }
        }

        public void CompressImages(string[] images, string strDataDir, string strImageDir, int imageType, int palette)
        {
            int dataStartOffset = 0x7c40;

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
                            MessageBox.Show("File written!");
                        }
                    }
                }
            }
        }

        public void LoadImageU2(byte[] file_bytes, Bitmap b)
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

        public void MakePngU2(byte[] lzw, string strPng, int numTiles, int tileSize)
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

        public void WriteImageU2(byte[] file_bytes, Bitmap b)
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

        public void MakeU2(out byte[]? file_bytes, string strPng)
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
                    Console.WriteLine("Image must be 128x128 pixels!");
                    return;
                }
                WriteImageU2(destination, image);
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
