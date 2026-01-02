using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima3ImageExtractor
    {
        public void extractImages(string[] images, string strOutDir)
        {
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("SHAPES.ULT"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null && file_bytes.Length == 5120)
                    {
                        string fullPath = Path.Combine(strOutDir, "SHAPES.png");
                        helper.MakePngU3(file_bytes, fullPath);
                    }
                }
            }
        }

        public void compressImages(string[] images, string strOutDir)
        {
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("SHAPES.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeU3(out file_bytes, image);

                    if (file_bytes != null && file_bytes.Length == 5120)
                    {
                        string fullPath = Path.Combine(strOutDir, "SHAPES.ULT");
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
