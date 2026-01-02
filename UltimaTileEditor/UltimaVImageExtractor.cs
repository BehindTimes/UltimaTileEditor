using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class UltimaVImageExtractor
    {
        public void extractImages(string[] images, string strOutDir)
        {
            lzwDecompressor lzw = new lzwDecompressor();
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("TILES.16"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    byte[]? lzw_out;
                    lzw.extract(file_bytes, out lzw_out);
                    if (lzw_out != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TILES.png");
                        helper.MakePngU5(lzw_out, fullPath);
                    }
                }    
            }
        }

        public void compressImages(string[] images, string strOutDir)
        {
            lzwDecompressor lzw = new lzwDecompressor();
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("TILES.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeLZWU5(out file_bytes, image);

                    if(file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TILES.16");
                        lzw.compress(file_bytes, fullPath);
                    }         
                }
            }
        }
    }
}
