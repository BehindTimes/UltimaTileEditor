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
            rbUltima5.Checked = true;
            ChangeGame(UltimaGame.Ultima5);
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
                    cbPalette.Items.Add("EGA");
                    cbPalette.Items.Add("CGA");
                    cbPalette.Items.Add("Tandy");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima2:
                    cbFileType.Items.Add("Tiles");
                    cbPalette.Items.Add("CGA");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima3:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.Items.Add("Character Set");
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
                    cbPalette.Items.Add("EGA");
                    cbPalette.Items.Add("CGA");
                    cbFileType.SelectedIndex = 0;
                    cbPalette.SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }

        private void btnGameDataBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
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
        }

        private void btnImageBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
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
        }

        private void btnExtract_Click(object sender, EventArgs e)
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
                            Ultima1ImageExtractor ie1 = new Ultima1ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie1.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new Ultima2ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie2.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new Ultima3ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie3.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new Ultima4ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie4.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new Ultima5ImageExtractor();
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

        private void btnCompress_Click(object sender, EventArgs e)
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
                            Ultima1ImageExtractor ie1 = new Ultima1ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie1.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new Ultima2ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie2.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new Ultima3ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie3.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new Ultima4ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie4.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex, cbPalette.SelectedIndex);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new Ultima5ImageExtractor();
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
                            m_ImageFiles = DataFiles.Ultima1Image;
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
                    m_DataFiles = DataFiles.Ultima2Files;
                    m_ImageFiles = DataFiles.Ultima2Files;
                    break;
                case UltimaGame.Ultima3:
                    switch (cbFileType.SelectedIndex)
                    {
                        case 1: // Character Set
                            m_DataFiles = DataFiles.Ultima3Charset;
                            m_ImageFiles = DataFiles.Ultima3Charset;
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
                        case 2: // EGA RLE Images
                            m_DataFiles = DataFiles.Ultima4RLE;
                            m_ImageFiles = DataFiles.Ultima4RLE;
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
                        else
                        {
                            strExt = ".BIN";
                        }
                        break;
                    case UltimaGame.Ultima2:
                        strExt = ".EXE";
                        break;
                    case UltimaGame.Ultima3:
                        strExt = ".ULT";
                        break;
                    case UltimaGame.Ultima4:
                        strExt = ".EGA";
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

        private void rbUltima1_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima1);
        }

        private void rbUltima2_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima2);
        }

        private void rbUltima3_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima3);
        }

        private void rbUltima4_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima4);
        }

        private void rbUltima5_CheckedChanged(object sender, EventArgs e)
        {
            ChangeGame(UltimaGame.Ultima5);
        }

        private void cbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDataFiles();
        }

        private void cbPalette_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeDataFiles();
        }
    }
}
