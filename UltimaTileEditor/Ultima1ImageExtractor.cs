using System;
using System.Collections.Generic;
using System.Text;

namespace UltimaTileEditor
{
    internal class Ultima1ImageExtractor
    {
        public void ExtractImages(string[] images, string strOutDir)
        {
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("TILES.BIN"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TILES.png");
                        helper.MakePngU1(file_bytes, fullPath, 13, 4, true);
                    }
                }
                else if (image.EndsWith("MOND.BIN"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "MOND.png");
                        helper.MakePngU1(file_bytes, fullPath, 19, 1, true);
                    }
                }
                else if (image.EndsWith("TOWN.BIN"))
                {
                    byte[] file_bytes = File.ReadAllBytes(image);
                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "TOWN.png");
                        helper.MakePngU1(file_bytes, fullPath, 17, 3, false);
                    }
                }
            }
        }

        public void CompressImages(string[] images, string strOutDir)
        {
            pngHelper helper = new pngHelper();

            foreach (string image in images)
            {
                if (image.EndsWith("TILES.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeU1(out file_bytes, image, 13, 4, true);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "EGATILES.BIN");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
                else if (image.EndsWith("MOND.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeU1(out file_bytes, image, 19, 1, true);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "EGAMOND.BIN");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
                else if (image.EndsWith("TOWN.png"))
                {
                    byte[]? file_bytes;
                    helper.MakeU1(out file_bytes, image, 17, 3, false);

                    if (file_bytes != null)
                    {
                        string fullPath = Path.Combine(strOutDir, "EGATOWN.BIN");
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fullPath, FileMode.Create)))
                        {
                            binWriter.Write(file_bytes);
                        }
                    }
                }
            }
            MessageBox.Show("Files written!");
        }
    }
}
