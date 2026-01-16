using System.Windows.Forms;

namespace UltimaTileEditor
{
    public partial class FormUltimaEditor : Form
    {
        enum UltimaGame
        {
            None = 0,
            Ultima1,
            Ultima2,
            Ultima3,
            Ultima4,
            Ultima5
        }

        UltimaGame m_curGame = UltimaGame.Ultima4;
        List<string> m_DataFiles = [];
        List<string> m_ImageFiles = [];

        public FormUltimaEditor()
        {
            InitializeComponent();
        }

        private void FormUltimaEditor_Load(object sender, EventArgs e)
        {
            //rbUltima5.Checked = true;
            rbUltima4.Checked = true;
            ChangeGame(UltimaGame.Ultima4);
        }

        private void ChangeGame(UltimaGame game)
        {
            m_curGame = game;
            cbFileType.Items.Clear();
            cbPalette.Items.Clear();
            switch (game)
            {
                case UltimaGame.Ultima1:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.Items.Add("Image");
                    cbFileType.Items.Add("Ending");
                    cbPalette.Items.Add("EGA");
                    cbPalette.Items.Add("CGA");
                    cbPalette.Items.Add("Tandy");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima2:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.Items.Add("Pictures");
                    cbPalette.Items.Add("CGA");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima3:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.Items.Add("Character Set");
                    cbFileType.Items.Add("Pictures");
                    cbFileType.Items.Add("Animation");
                    cbFileType.Items.Add("Signature");
                    cbPalette.Items.Add("CGA");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima4:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.Items.Add("Char Set");
                    cbFileType.Items.Add("RLE Images");
                    cbFileType.Items.Add("LZW Images");
                    cbPalette.Items.Add("EGA");
                    cbPalette.Items.Add("CGA");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima5:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.Items.Add("Masked Images");
                    cbFileType.Items.Add("Dungeon Images");
                    cbFileType.Items.Add("Images");
                    cbFileType.Items.Add("CH Files");
                    cbFileType.Items.Add("HCS Files");
                    cbFileType.Items.Add("BIT Files");
                    cbFileType.Items.Add("Path Files");
                    cbFileType.Items.Add("Intro Font");
                    cbPalette.Items.Add("EGA");
                    cbPalette.Items.Add("CGA");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }

        private void BtnGameDataBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            // Optional: Set the initial folder that appears in the dialog
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            // Optional: Add a description at the top of the dialog
            fbd.Description = "Select the Ultima Data File Folder";
            // Optional: Allow or prevent the creation of new folders
            fbd.ShowNewFolderButton = true;

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string selectedFolderPath = fbd.SelectedPath;
                tbGameDataDir.Text = selectedFolderPath;
                DisplayDataFiles();
            }
        }

        private void BtnImageBrowse_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            // Optional: Set the initial folder that appears in the dialog
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            // Optional: Add a description at the top of the dialog
            fbd.Description = "Select the Ultima Extracted Images Folder";
            // Optional: Allow or prevent the creation of new folders
            fbd.ShowNewFolderButton = true;

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                string selectedFolderPath = fbd.SelectedPath;
                tbImagesDir.Text = selectedFolderPath;
                DisplayImageFiles();
            }
        }

        private void BtnExtract_Click(object sender, EventArgs e)
        {
            string strDataDir = tbGameDataDir.Text;
            string strImagesDir = tbImagesDir.Text;

            if (Directory.Exists(strDataDir) && Directory.Exists(strImagesDir))
            {
                if (lbFiles.Items.Count > 0)
                {
                    string[] itemsArray;

                    switch (m_curGame)
                    {
                        case UltimaGame.Ultima1:
                            Ultima1ImageExtractor ie1 = new();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            Ultima1ImageExtractor.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie2.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie3.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            Ultima4ImageExtractor.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie5.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        default:
                            break;
                    }

                    ChangeDataFiles();
                }
            }
            // Force the garbage collector to run.  Otherwise the program could crash if you try extracting
            // and compressing the same images without closing the application
            System.GC.Collect();
        }

        private void BtnCompress_Click(object sender, EventArgs e)
        {
            string strDataDir = tbGameDataDir.Text;
            string strImagesDir = tbImagesDir.Text;

            if (Directory.Exists(strDataDir) && Directory.Exists(strImagesDir))
            {
                if (lbImages.Items.Count > 0)
                {
                    string[] itemsArray;

                    switch (m_curGame)
                    {
                        case UltimaGame.Ultima1:
                            Ultima1ImageExtractor ie1 = new();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            Ultima1ImageExtractor.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie2.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie3.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            Ultima4ImageExtractor.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie5.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        default:
                            break;
                    }

                    ChangeDataFiles();
                }
            }
            // Force the garbage collector to run.  Otherwise the program could crash if you try extracting
            // and compressing the same images without closing the application
            System.GC.Collect();
        }

        private void ChangeDataFiles()
        {
            switch (m_curGame)
            {
                case UltimaGame.Ultima1:

                    switch (cbFileType.SelectedIndex)
                    {
                        case 1: // Castle Image
                            m_DataFiles = DataFiles.Ultima1Image;
                            switch(cbPalette.SelectedIndex)
                            {
                                case 1: // CGA
                                    m_ImageFiles = DataFiles.Ultima1ImageCGA;
                                    break;
                                case 2: // Tandy
                                    m_ImageFiles = DataFiles.Ultima1ImageTandy;
                                    break;
                                default:
                                    m_ImageFiles = DataFiles.Ultima1ImageEGA;
                                    break;
                            }
                            break;
                        case 2: // Ending
                            m_DataFiles = DataFiles.Ultima1Ending;
                            m_ImageFiles = DataFiles.Ultima1Ending;
                            break;
                        default: // Tiles
                            switch(cbPalette.SelectedIndex)
                            {
                                case 1:
                                    m_DataFiles = DataFiles.Ultima1CGAFiles;
                                    m_ImageFiles = DataFiles.Ultima1CGAFiles;
                                    break;
                                case 2:
                                    m_DataFiles = DataFiles.Ultima1T1KFiles;
                                    m_ImageFiles = DataFiles.Ultima1T1KFiles;
                                    break;
                                default:
                                    m_DataFiles = DataFiles.Ultima1EGAFiles;
                                    m_ImageFiles = DataFiles.Ultima1EGAFiles;
                                    break;
                            }
                            break;
                    }
                    break;
                case UltimaGame.Ultima2:
                    switch (cbFileType.SelectedIndex)
                    {
                        case 1:
                            m_DataFiles = DataFiles.Ultima2Pictures;
                            m_ImageFiles = DataFiles.Ultima2Pictures;
                            break;
                        default:
                            m_DataFiles = DataFiles.Ultima2Files;
                            m_ImageFiles = DataFiles.Ultima2Files;
                            break;
                    }
                    break;
                case UltimaGame.Ultima3:
                    switch (cbFileType.SelectedIndex)
                    {
                        case 1: // Character Set
                            m_DataFiles = DataFiles.Ultima3Charset;
                            m_ImageFiles = DataFiles.Ultima3Charset;
                            break;
                        case 2: // Pictures
                            m_DataFiles = DataFiles.Ultima3Pictures;
                            m_ImageFiles = DataFiles.Ultima3Pictures;
                            break;
                        case 3: // Animation
                            m_DataFiles = DataFiles.Ultima3Animate;
                            m_ImageFiles = DataFiles.Ultima3Animate;
                            break;
                        case 4: // Signature
                            m_DataFiles = DataFiles.Ultima3Signature;
                            m_ImageFiles = DataFiles.Ultima3Signature;
                            break;
                        default:
                            m_DataFiles = DataFiles.Ultima3Files;
                            m_ImageFiles = DataFiles.Ultima3Files;
                            break;
                    }
                    break;
                case UltimaGame.Ultima4:
                    switch (cbFileType.SelectedIndex)
                    {
                        case 1: // EGA Char Set
                            m_DataFiles = DataFiles.Ultima4Charset;
                            m_ImageFiles = DataFiles.Ultima4Charset;
                            break;
                        case 2: // RLE Images
                            m_DataFiles = DataFiles.Ultima4RLE;
                            if(cbPalette.SelectedIndex == 1) // CGA
                            {
                                m_ImageFiles = DataFiles.Ultima4CGARLE;
                            }
                            else
                            {
                                m_ImageFiles = DataFiles.Ultima4RLE;
                            }
                                
                            break;
                        case 3: // EGA LZW Images
                            m_DataFiles = DataFiles.Ultima4LZW;
                            m_ImageFiles = DataFiles.Ultima4LZW;
                            break;
                        default: // EGA Tiles
                            m_DataFiles = DataFiles.Ultima4EGATileFiles;
                            m_ImageFiles = DataFiles.Ultima4EGATileFiles;
                            break;
                    }
                    break;
                case UltimaGame.Ultima5:
                    switch (cbFileType.SelectedIndex)
                    {
                        case 0: // .16 Tiles
                            m_DataFiles = DataFiles.Ultima5Tiles;
                            m_ImageFiles = DataFiles.Ultima5Tiles;
                            break;
                        case 1: // .16 Masked Images
                            m_DataFiles = DataFiles.Ultima5Masked;
                            m_ImageFiles = DataFiles.Ultima5MaskedImage;
                            break;
                        case 2: // .16 Dungeon Images
                            m_DataFiles = DataFiles.Ultima5Dng;
                            m_ImageFiles = DataFiles.Ultima5DngImage;
                            break;
                        case 3: // .16 Images
                            m_DataFiles = DataFiles.Ultima5Pict;
                            m_ImageFiles = DataFiles.Ultima5PictImage;
                            break;
                        case 4: // .CH Files
                            m_DataFiles = DataFiles.Ultima5CharFiles;
                            m_ImageFiles = DataFiles.Ultima5CharFiles;
                            break;
                        case 5: // .HCS Files
                            m_DataFiles = DataFiles.Ultima5CharFiles;
                            m_ImageFiles = DataFiles.Ultima5CharFiles;
                            break;
                        case 6: // .BIT Files
                            m_DataFiles = DataFiles.Ultima5BitFiles;
                            m_ImageFiles = DataFiles.Ultima5BitImages;
                            break;
                        case 7: // .PTH Files
                            m_DataFiles = DataFiles.Ultima5PathFiles;
                            m_ImageFiles = DataFiles.Ultima5PathImage;
                            break;
                        case 8: // .PCS Files
                            m_DataFiles = DataFiles.Ultima5Proport;
                            m_ImageFiles = DataFiles.Ultima5Proport;
                            break;
                        default: // .16 Tiles
                            m_DataFiles = DataFiles.Ultima5Tiles;
                            m_ImageFiles = DataFiles.Ultima5Tiles;
                            break;
                    }
                    break;
                default:
                    m_DataFiles = [];
                    break;
            }
            DisplayDataFiles();
            DisplayImageFiles();
        }

        private void DisplayDataFiles()
        {
            string strExt = ".16";
            string strDir = tbGameDataDir.Text;
            lbFiles.Items.Clear();
            if (strDir.Length > 0 && Directory.Exists(strDir))
            {
                switch (m_curGame)
                {
                    case UltimaGame.Ultima1:
                        if (cbFileType.SelectedIndex == 1 && cbPalette.SelectedIndex == 0)
                        {
                            strExt = ".16";
                        }
                        else if (cbFileType.SelectedIndex == 1 && cbPalette.SelectedIndex == 1)
                        {
                            strExt = ".4";
                        }
                        else if (cbFileType.SelectedIndex == 1 && cbPalette.SelectedIndex == 2)
                        {
                            strExt = ".16";
                        }
                        else
                        {
                            strExt = ".BIN";
                        }
                        break;
                    case UltimaGame.Ultima2:
                        if (cbFileType.SelectedIndex == 0)
                        {
                            strExt = ".EXE";
                        }
                        else if(cbFileType.SelectedIndex == 1)
                        {
                            strExt = "";
                        }
                        break;
                    case UltimaGame.Ultima3:
                        if (cbFileType.SelectedIndex == 2)
                        {
                            strExt = ".IBM";
                        }
                        else if (cbFileType.SelectedIndex == 3)
                        {
                            strExt = ".DAT";
                        }
                        else if (cbFileType.SelectedIndex == 4)
                        {
                            strExt = ".DAT";
                        }
                        else
                        {
                            strExt = ".ULT";
                        } 
                        break;
                    case UltimaGame.Ultima4:
                        if (cbPalette.SelectedIndex == 1)
                        {
                            if (cbFileType.SelectedIndex == 0)
                            {
                                strExt = ".CGA";
                            }
                            else if (cbFileType.SelectedIndex == 1)
                            {
                                strExt = ".CGA";
                            }
                            else if (cbFileType.SelectedIndex == 2)
                            {
                                strExt = ".PIC";
                            }
                            else if (cbFileType.SelectedIndex == 3)
                            {
                                strExt = ".PIC";
                            }
                        }
                        else
                        {
                            strExt = ".EGA";
                        } 
                        break;
                    case UltimaGame.Ultima5:
                        if (cbPalette.SelectedIndex == 1)
                        {
                            strExt = ".4";
                        }
                        else
                        {
                            if(cbFileType.SelectedIndex == 4)
                            {
                                strExt = ".CH";
                            }
                            else if (cbFileType.SelectedIndex == 5)
                            {
                                strExt = ".HCS";
                            }
                            else if (cbFileType.SelectedIndex == 6)
                            {
                                strExt = ".BIT";
                            }
                            else if (cbFileType.SelectedIndex == 7)
                            {
                                strExt = ".PTH";
                            }
                            else if (cbFileType.SelectedIndex == 8)
                            {
                                strExt = ".PCS";
                            }
                            else
                            {
                                strExt = ".16";
                            } 
                        }
                        break;
                    default:
                        strExt = "";
                        break;
                }

                string[] files = Directory.GetFiles(strDir);
                foreach (string strFile in files)
                {
                    if (m_DataFiles.Any(x => strFile.EndsWith(x + strExt)))
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(strFile);
                        if(value != null)
                        {
                            if (m_DataFiles.Any(x => value.StartsWith(x)))
                            {
                                string strFileName = System.IO.Path.GetFileName(strFile);
                                lbFiles.Items.Add(strFileName);
                            }
                        }
                    }
                }
            }
        }

        private void DisplayImageFiles()
        {
            string strExt;
            string strDir = tbImagesDir.Text;
            lbImages.Items.Clear();
            if (strDir.Length > 0 && Directory.Exists(strDir))
            {
                switch (m_curGame)
                {
                    case UltimaGame.Ultima1:
                        strExt = ".png";
                        break;
                    case UltimaGame.Ultima2:
                        strExt = ".png";
                        break;
                    case UltimaGame.Ultima3:
                        strExt = ".png";
                        break;
                    case UltimaGame.Ultima4:
                        strExt = ".png";
                        break;
                    case UltimaGame.Ultima5:
                        strExt = ".png";
                        break;
                    default:
                        strExt = "";
                        break;
                }

                string[] files = Directory.GetFiles(strDir);
                foreach (string strFile in files)
                {
                    if (m_ImageFiles.Any(x => strFile.EndsWith(x + strExt)))
                    {
                        string? value = System.IO.Path.GetFileNameWithoutExtension(strFile);
                        if (value != null)
                        {
                            if (m_ImageFiles.Any(x => value.StartsWith(x)))
                            {
                                string strFileName = System.IO.Path.GetFileName(strFile);
                                lbImages.Items.Add(strFileName);
                            }
                        }
                    }
                }
            }
        }

        private void RbUltima1_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima1);
        }

        private void RbUltima2_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima2);
        }

        private void RbUltima3_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima3);
        }

        private void RbUltima4_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima4);
        }

        private void RbUltima5_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima5);
        }

        private void CbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDataFiles();
        }

        private void CbPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDataFiles();
        }
    }
}
