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
            switch (game)
            {
                case UltimaGame.Ultima1:
                    cbFileType.Items.Add("EGA Tiles");
                    cbFileType.Items.Add("CGA Tiles");
                    cbFileType.Items.Add("T1K Tiles");
                    cbFileType.Items.Add(".16 Image");
                    cbFileType.Items.Add(".4 Image");
                    cbFileType.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima2:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima3:
                    cbFileType.Items.Add("Tiles");
                    cbFileType.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima4:
                    cbFileType.Items.Add("EGA Tiles");
                    cbFileType.Items.Add("EGA Char Set");
                    cbFileType.Items.Add("EGA RLE Images");
                    cbFileType.Items.Add("EGA LZW Images");
                    cbFileType.SelectedIndex = 0;
                    break;
                case UltimaGame.Ultima5:
                    cbFileType.Items.Add(".16 Tiles");
                    cbFileType.Items.Add(".16 Masked Images");
                    cbFileType.Items.Add(".16 Dungeon Images");
                    cbFileType.Items.Add(".16 Images");
                    cbFileType.Items.Add(".4 Tiles");
                    cbFileType.Items.Add(".4 Masked Images");
                    cbFileType.Items.Add(".4 Dungeon Images");
                    cbFileType.Items.Add(".4 Images");
                    cbFileType.SelectedIndex = 0;
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
                            ie1.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new Ultima2ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie2.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new Ultima3ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie3.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new Ultima4ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie4.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new Ultima5ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie5.ExtractImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        default:
                            break;
                    }

                    ChangeDataFiles();
                }
            }
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
                            ie1.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new Ultima2ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie2.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new Ultima3ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie3.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new Ultima4ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie4.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new Ultima5ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie5.CompressImages(itemsArray, strDataDir, strImagesDir, cbFileType.SelectedIndex);
                            break;
                        default:
                            break;
                    }

                    ChangeDataFiles();
                }
            }
        }

        private void ChangeDataFiles()
        {
            switch (m_curGame)
            {
                case UltimaGame.Ultima1:

                    switch (cbFileType.SelectedIndex)
                    {
                        case 1:
                            m_DataFiles = DataFiles.Ultima1CGAFiles;
                            m_ImageFiles = DataFiles.Ultima1CGAFiles;
                            break;
                        case 2:
                            m_DataFiles = DataFiles.Ultima1T1KFiles;
                            m_ImageFiles = DataFiles.Ultima1T1KFiles;
                            break;
                        case 3:
                            m_DataFiles = DataFiles.Ultima1Image;
                            m_ImageFiles = DataFiles.Ultima1Image;
                            break;
                        case 4:
                            m_DataFiles = DataFiles.Ultima1Image;
                            m_ImageFiles = DataFiles.Ultima1Image;
                            break;
                        default: // EGA Tiles
                            m_DataFiles = DataFiles.Ultima1EGAFiles;
                            m_ImageFiles = DataFiles.Ultima1EGAFiles;
                            break;
                    }
                    break;
                case UltimaGame.Ultima2:
                    m_DataFiles = DataFiles.Ultima2Files;
                    m_ImageFiles = DataFiles.Ultima2Files;
                    break;
                case UltimaGame.Ultima3:
                    m_DataFiles = DataFiles.Ultima3Files;
                    m_ImageFiles = DataFiles.Ultima3Files;
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
                            m_ImageFiles = DataFiles.Ultima5Masked;
                            break;
                        case 2: // .16 Dungeon Images
                            m_DataFiles = DataFiles.Ultima5DngImage;
                            m_ImageFiles = DataFiles.Ultima5DngImage;
                            break;
                        case 3: // .16 Images
                            m_DataFiles = DataFiles.Ultima5Images;
                            m_ImageFiles = DataFiles.Ultima5Images;
                            break;
                        case 4: // .4 Tiles
                            m_DataFiles = DataFiles.Ultima5Tiles;
                            m_ImageFiles = DataFiles.Ultima5Tiles;
                            break;
                        case 5: // .4 Masked Images
                            m_DataFiles = DataFiles.Ultima5Masked;
                            m_ImageFiles = DataFiles.Ultima5Masked;
                            break;
                        case 6: // .4 Dungeon Images
                            m_DataFiles = DataFiles.Ultima5DngImage;
                            m_ImageFiles = DataFiles.Ultima5DngImage;
                            break;
                        case 7: // .4 Images
                            m_DataFiles = DataFiles.Ultima5Images;
                            m_ImageFiles = DataFiles.Ultima5Images;
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
                        if (cbFileType.SelectedIndex == 3)
                        {
                            strExt = ".16";
                        }
                        else if (cbFileType.SelectedIndex == 4)
                        {
                            strExt = ".16";
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
                        if(cbFileType.SelectedIndex >= 0 &&  cbFileType.SelectedIndex <= 3)
                        {
                            strExt = ".16";
                        }
                        else if (cbFileType.SelectedIndex >= 4 && cbFileType.SelectedIndex <= 7)
                        {
                            strExt = ".4";
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
                        string strFileName = System.IO.Path.GetFileName(strFile);

                        lbFiles.Items.Add(strFileName);
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
                        string strFileName = System.IO.Path.GetFileName(strFile);

                        lbImages.Items.Add(strFileName);
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
    }
}
