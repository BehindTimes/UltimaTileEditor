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
            ChangeDataFiles();
            DisplayDataFiles();
            DisplayImageFiles();
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
                            ie1.ExtractImages(itemsArray, strImagesDir);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new Ultima2ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie2.ExtractImages(itemsArray, strImagesDir);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new Ultima3ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie3.extractImages(itemsArray, strImagesDir);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new Ultima4ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie4.extractImages(itemsArray, strImagesDir);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new Ultima5ImageExtractor();
                            itemsArray = [.. lbFiles.Items.OfType<string>()];
                            ie5.ExtractImages(itemsArray, strImagesDir);
                            break;
                        default:
                            break;
                    }
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
                            ie1.CompressImages(itemsArray, strDataDir);
                            break;
                        case UltimaGame.Ultima2:
                            Ultima2ImageExtractor ie2 = new Ultima2ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie2.CompressImages(itemsArray, strDataDir);
                            break;
                        case UltimaGame.Ultima3:
                            Ultima3ImageExtractor ie3 = new Ultima3ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie3.compressImages(itemsArray, strDataDir);
                            break;
                        case UltimaGame.Ultima4:
                            Ultima4ImageExtractor ie4 = new Ultima4ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie4.compressImages(itemsArray, strDataDir);
                            break;
                        case UltimaGame.Ultima5:
                            Ultima5ImageExtractor ie5 = new Ultima5ImageExtractor();
                            itemsArray = [.. lbImages.Items.OfType<string>()];
                            ie5.CompressImages(itemsArray, strDataDir);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void ChangeDataFiles()
        {
            switch (m_curGame)
            {
                case UltimaGame.Ultima1:
                    m_DataFiles = DataFiles.Ultima1Files;
                    m_ImageFiles = DataFiles.Ultima1ImageFiles;
                    break;
                case UltimaGame.Ultima2:
                    m_DataFiles = DataFiles.Ultima2Files;
                    m_ImageFiles = DataFiles.Ultima2ImageFiles;
                    break;
                case UltimaGame.Ultima3:
                    m_DataFiles = DataFiles.Ultima3Files;
                    m_ImageFiles = DataFiles.Ultima3ImageFiles;
                    break;
                case UltimaGame.Ultima4:
                    m_DataFiles = DataFiles.Ultima4Files;
                    m_ImageFiles = DataFiles.Ultima4ImageFiles;
                    break;
                case UltimaGame.Ultima5:
                    m_DataFiles = DataFiles.Ultima5Files;
                    m_ImageFiles = DataFiles.Ultima5ImageFiles;
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
            string strPre = "";
            string strExt;
            string strDir = tbGameDataDir.Text;
            lbFiles.Items.Clear();
            if (strDir.Length > 0 && Directory.Exists(strDir))
            {
                switch (m_curGame)
                {
                    case UltimaGame.Ultima1:
                        strPre = "EGA";
                        strExt = ".BIN";
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
                        strExt = ".16";
                        break;
                    default:
                        strExt = "";
                        break;
                }

                string[] files = Directory.GetFiles(strDir);
                foreach (string strFile in files)
                {
                    if (m_DataFiles.Any(x => strFile.EndsWith(strPre + x + strExt)))
                    {
                        lbFiles.Items.Add(strFile);
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
                        lbImages.Items.Add(strFile);
                    }
                }
            }
        }

        private void rbUltima1_CheckedChanged(object sender, EventArgs e)
        {
            m_curGame = UltimaGame.Ultima1;
            ChangeDataFiles();
        }

        private void rbUltima2_CheckedChanged(object sender, EventArgs e)
        {
            m_curGame = UltimaGame.Ultima2;
            ChangeDataFiles();
        }

        private void rbUltima3_CheckedChanged(object sender, EventArgs e)
        {
            m_curGame = UltimaGame.Ultima3;
            ChangeDataFiles();
        }

        private void rbUltima4_CheckedChanged(object sender, EventArgs e)
        {
            m_curGame = UltimaGame.Ultima4;
            ChangeDataFiles();
        }

        private void rbUltima5_CheckedChanged(object sender, EventArgs e)
        {
            m_curGame = UltimaGame.Ultima5;
            ChangeDataFiles();
        }

       
       

    }
}
