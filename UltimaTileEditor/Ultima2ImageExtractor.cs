using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima2ImageExtractor
    {
        public void ExtractImages(string[] images, string strOutDir)
        {
            int dataStartOffset = 0x7c40;
            int tileSize = 66;
            int numTiles = 64;

            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("ULTIMAII.EXE"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[] tile_data = new byte[tileSize * numTiles];
                    Array.Copy(file_bytes, dataStartOffset, tile_data, 0, tileSize * numTiles);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "ULTIMAII.png");
                        helper.MakePngU2(tile_data, fullPath, numTiles, tileSize);
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strOutDir)
        {
            lzwDecompressor lzw = new lzwDecompressor();
            pngHelper helper = new pngHelper();

            int dataStartOffset = 0x7c40;

            foreach (string image in images)
            {
                if (image.EndsWith("ULTIMAII.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeU2(out file_bytes, image);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "ULTIMAII.EXE");
                        byte[] exe_bytes = File.ReadAllBytes(fullPath);
                        if(exe_bytes.Length > dataStartOffset + file_bytes.Length)
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
    }
}
