using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima4ImageExtractor
    {
        public void extractImages(string[] images, string strOutDir)
        {
            lzwDecompressor lzw = new lzwDecompressor();
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("SHAPES.EGA"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "SHAPES.png");
                        helper.MakePngU4(file_bytes, fullPath);
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
                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeU4(out file_bytes, image);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "SHAPES.EGA");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                            MessageBox.Show("File written!");
                        }
                    }
                }
            }
        }
    }
}
