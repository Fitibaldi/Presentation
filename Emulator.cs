using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography;
//Check internet connection
using System.Runtime;
using System.Runtime.InteropServices;

namespace Emul
{
    public partial class mainForm : Form
    {
        #region ***User Defined ***
        //office public String glbDMLService = "http://192.168.35.172:7101/TClassDMLWebService-root/dmlwservlet";
        //global public String glbDMLService = "http://94.26.2.66:7101/TClassDMLWebService-root/dmlwservlet";
        public String glbDMLService = "http://192.168.35.172:7101/TClassDMLWebService-root/dmlwservlet";
        public string glbCredentials = "TCUsername=TcDML@db&TCPassword=TcDML@2012&TCCommand=";
        public String glbGraphServlet = "http://192.168.35.172:7101/TClassDMLWebService-root/dmlwservlet";
        public string glbGraphCredentials = "***PRIVATE***";
        public string glbIDCredentials = "***PRIVATE***&TCTable=TERM.DB_IMAGE_QUERY&TCType=IMAGE&TCGraphID=";
        private String glbInvNom;
        private string msgConcentrator;
        private string msgOutOfRange;
        private string msgWrongNumber;
        public string glbStatus = "Rejected";
        public int glbRows = 0;
        private int glbIterationNo;
        private int glbTerminalId;
        private int glbDispRows;
        private int glbDispColumns;
        private string glbTerminalCode;
        private String deviceType;
        private int graphsFound = 0;
        private bool graphShown = false;
        private bool graphLarge = false;
        private int graphLargeWidth = 0;
        private int graphLargeHeight = 0;
        private Point graphLargeLocation = new Point();
        private bool imageDBLarge = false;
        private int imageLargeWidth = 0;
        private int imageLargeHeight = 0;
        private Point imageLargeLocation = new Point();

        private const int idROW_NR = 0;
        private const int idPOS_NR = 1;
        private const int idFIELD_TYPE = 2;
        private const int idFIELD_DATA = 3;
        private const int idDATA_TYPE = 4;
        private const int idNEGATIVE = 5;
        private const int idFONT_SIZE = 6;
        private const int idSIZE_VAL = 7;
        private const int idDEVICE_ID = 8;
        private const int idMIN_VAL = 9;
        private const int idMAX_VAL = 10;
        private const int idALIGN = 11;
        private const int idREQUIRED = 12;
        private const int idCOLOR = 13;
        private const int idTerminalCode = 14;

        private const int devDEVICE_TYPE = 0;
        private const int devPORT = 1;
        private const int devPORT_TYPE = 2;
        private const int devHANDSHAKE = 3;
        private const int devPARITY = 4;
        private const int devRTS_ENABLE = 5;
        private const int devSPEED = 6;
        private const int devDELIMITER = 7;
        private const int devDATA_BITS = 8;
        private const int devSTOP_BITS = 9;

        class termControl
        {
            Control _contr;
            string _id;
            Double _minVal;
            Double _maxVal;
            string _dataType;
            string _fieldType;
            SerialPort _comPort;
            string _portDelimiter;
            string _required;

            public termControl(Control contr)
            {
                _contr = contr;
            }

            public termControl(Control contr, string id)
            {
                _contr = contr;
                _id = id;
            }

            public termControl(Control contr, Double min, Double max, string dataType)
            {
                _contr = contr;
                _minVal = min;
                _maxVal = max;
                _dataType = dataType;
            }

            public Control Control
            {
                get
                {
                    return this._contr;
                }
                set
                {
                    this._contr = value;
                }
            }

            public string Id
            {
                get
                {
                    return this._id;
                }
                set
                {
                    this._id = value;
                }
            }

            public Double MinVal
            {
                get
                {
                    return this._minVal;
                }
                set
                {
                    this._minVal = value;
                }
            }

            public Double MaxVal
            {
                get
                {
                    return this._maxVal;
                }
                set
                {
                    this._maxVal = value;
                }
            }

            public string DataType
            {
                get
                {
                    return this._dataType;
                }
                set
                {
                    this._dataType = value;
                }
            }

            public string FieldType
            {
                get
                {
                    return this._fieldType;
                }
                set
                {
                    this._fieldType = value;
                }
            }

            public SerialPort ComPort
            {
                get
                {
                    return this._comPort;
                }
                set
                {
                    this._comPort = value;
                }
            }

            public string PortDelimiter
            {
                get
                {
                    return this._portDelimiter;
                }
                set
                {
                    this._portDelimiter = value;
                }
            }

            public string Required
            {
                get
                {
                    return this._required;
                }
                set
                {
                    this._required = value;
                }
            }

        }
        #endregion user defined

        termControl currTB = new termControl(new TextBox());
        int currTbIndex = 0;
        int cntTbxs = 0;
        int printLen = 400;
        bool requiredPassFirstTime = false;
        List<termControl> screenControls = new List<termControl>();
        
        int intervalOperDataTimer = 180*1000;
        int intervalRefreshTimer;

        private ImageButton bmDone;
        private ImageButton bmDown;
        private ImageButton bmEsc;
        private ImageButton bmF1;
        private ImageButton bmF2;
        private ImageButton bmKB;
        private ImageButton bmLeft;
        private ImageButton bmRight;
        private ImageButton bmUp;
        private ImageButton bmUtils;
        private ImageButton bmGraph;
        private ImageButton bmNull1;
        private ImageButton bmTab;

        private ImageButton bnDone;
        private ImageButton bnDot;
        private ImageButton bnEight;
        private ImageButton bnFive;
        private ImageButton bnFour;
        private ImageButton bnMinus;
        private ImageButton bnPlus;	
        private ImageButton bnNine;
        private ImageButton bnOne;
        private ImageButton bnSeven;
        private ImageButton bnSix;
        private ImageButton bnTab;
        private ImageButton bnThree;
        private ImageButton bnTwo;
        private ImageButton bnZero;
        private ImageButton bnBackspace;

        PictureBox graph1PB = new PictureBox();
        PictureBox graph2PB = new PictureBox();
        PictureBox graph3PB = new PictureBox();
        PictureBox graph4PB = new PictureBox();
        PictureBox graph5PB = new PictureBox();
        PictureBox graph6PB = new PictureBox();

        String graph1Index = "1";
        String graph2Index = "2";
        String graph3Index = "3";
        String graph4Index = "4";
        String graph5Index = "5";
        String graph6Index = "6";

        public mainForm()
        {
            InitializeComponent();

            bmDone = new ImageButton(146, 70); bmDone.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_done_7inch.png")); bmDone.Location = new Point(632, 344); bmDone.Click += new EventHandler(bmDone_Click); this.Controls.Add(bmDone);
            bmDown = new ImageButton(70, 70); bmDown.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_down_7inch.png")); bmDown.Location = new Point(632, 268); bmDown.Click += new EventHandler(bmDown_Click); this.Controls.Add(bmDown);
            bmEsc = new ImageButton(146, 70); bmEsc.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_esc_7inch.png")); bmEsc.Location = new Point(556, 40); bmEsc.Click += new EventHandler(bmEsc_Click); this.Controls.Add(bmEsc);
            bmF1 = new ImageButton(70, 70); bmF1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f1_7inch.png")); bmF1.Location = new Point(556, 116); bmF1.Click += new EventHandler(bmF1_Click); this.Controls.Add(bmF1);
            bmF2 = new ImageButton(70, 70); bmF2.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f2_7inch.png")); bmF2.Location = new Point(708, 116); bmF2.Click += new EventHandler(bmF2_Click); this.Controls.Add(bmF2);
            bmKB = new ImageButton(70, 70); bmKB.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_kb_7inch.png")); bmKB.Location = new Point(708, 40); bmKB.Click += new EventHandler(bmKB_Click); this.Controls.Add(bmKB);
            bmLeft = new ImageButton(70, 70); bmLeft.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_left_7inch.png")); bmLeft.Location = new Point(556, 192); bmLeft.Click += new EventHandler(bmLeft_Click); this.Controls.Add(bmLeft);
            bmRight = new ImageButton(70, 70); bmRight.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_right_7inch.png")); bmRight.Location = new Point(708, 192); bmRight.Click += new EventHandler(bmRight_Click); this.Controls.Add(bmRight);
            bmUp = new ImageButton(70, 70); bmUp.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_up_7inch.png")); bmUp.Location = new Point(632, 116); bmUp.Click += new EventHandler(bmUp_Click); this.Controls.Add(bmUp);
            bmUtils = new ImageButton(70, 70); bmUtils.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_utils_7inch.png")); bmUtils.Location = new Point(708, 268); bmUtils.Click += new EventHandler(bmUtils_Click); this.Controls.Add(bmUtils);
            bmGraph = new ImageButton(70, 70); bmGraph.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_graph_7inch.png")); bmGraph.Location = new Point(632, 192); bmGraph.Click += new EventHandler(bmGraph_Click); this.Controls.Add(bmGraph);
            bmNull1 = new ImageButton(70, 70); bmNull1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.null_7inch.png")); bmNull1.Location = new Point(556, 268); this.Controls.Add(bmNull1);
            bmTab = new ImageButton(70, 70); bmTab.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_tab_7inch.png")); bmTab.Location = new Point(556, 344); bmTab.Click += new EventHandler(bnTab_Click); this.Controls.Add(bmTab);
            bnDone = new ImageButton(70, 70); bnDone.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_done_7inch.png")); bnDone.Location = new Point(708, 344); bnDone.Click += new EventHandler(bnDone_Click); this.Controls.Add(bnDone);
            bnDot = new ImageButton(70, 70); bnDot.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_dot_7inch.png")); bnDot.Location = new Point(632, 40); bnDot.Click += new EventHandler(bnDot_Click); this.Controls.Add(bnDot);
            bnEight = new ImageButton(70, 70); bnEight.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_eight_7inch.png")); bnEight.Location = new Point(632, 116); bnEight.Click += new EventHandler(bnEight_Click); this.Controls.Add(bnEight);
            bnFive = new ImageButton(70, 70); bnFive.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_five_7inch.png")); bnFive.Location = new Point(632, 192); bnFive.Click += new EventHandler(bnFive_Click); this.Controls.Add(bnFive);
            bnFour = new ImageButton(70, 70); bnFour.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_four_7inch.png")); bnFour.Location = new Point(556, 192); bnFour.Click += new EventHandler(bnFour_Click); this.Controls.Add(bnFour);
            bnMinus = new ImageButton(70, 70); bnMinus.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_minus_7inch.png")); bnMinus.Location = new Point(556, 40); bnMinus.Click += new EventHandler(bnMinus_Click); this.Controls.Add(bnMinus);
            bnPlus = new ImageButton(70, 70); bnPlus.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_plus.png")); bnPlus.Location = new Point(556, 40); bnPlus.Click += new EventHandler(bnPlus_Click); this.Controls.Add(bnPlus);
            bnNine = new ImageButton(70, 70); bnNine.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_nine_7inch.png")); bnNine.Location = new Point(708, 116); bnNine.Click += new EventHandler(bnNine_Click); this.Controls.Add(bnNine);
            bnOne = new ImageButton(70, 70); bnOne.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_one_7inch.png")); bnOne.Location = new Point(556, 268); bnOne.Click += new EventHandler(bnOne_Click); this.Controls.Add(bnOne);
            bnSeven = new ImageButton(70, 70); bnSeven.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_seven_7inch.png")); bnSeven.Location = new Point(556, 116); bnSeven.Click += new EventHandler(bnSeven_Click); this.Controls.Add(bnSeven);
            bnSix = new ImageButton(70, 70); bnSix.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_six_7inch.png")); bnSix.Location = new Point(708, 192); bnSix.Click += new EventHandler(bnSix_Click); this.Controls.Add(bnSix);
            bnTab = new ImageButton(70, 70); bnTab.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_tab_7inch.png")); bnTab.Location = new Point(632, 344); bnTab.Click += new EventHandler(bnTab_Click); this.Controls.Add(bnTab);
            bnThree = new ImageButton(70, 70); bnThree.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_three_7inch.png")); bnThree.Location = new Point(708, 268); bnThree.Click += new EventHandler(bnThree_Click); this.Controls.Add(bnThree);
            bnTwo = new ImageButton(70, 70); bnTwo.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_two_7inch.png")); bnTwo.Location = new Point(632, 268); bnTwo.Click += new EventHandler(bnTwo_Click); this.Controls.Add(bnTwo);
            bnZero = new ImageButton(70, 70); bnZero.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_zero_7inch.png")); bnZero.Location = new Point(556, 344); bnZero.Click += new EventHandler(bnZero_Click); this.Controls.Add(bnZero);
            bnBackspace = new ImageButton(70, 70); bnBackspace.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_bs_7inch.png")); bnBackspace.Location = new Point(708, 40); bnBackspace.Click += new EventHandler(bnBackspace_Click); this.Controls.Add(bnBackspace);
      
            graph1PB.Click += new EventHandler(graphPB_Click); graph1PB.SizeMode = PictureBoxSizeMode.StretchImage; this.Controls.Add(graph1PB); graph1PB.Hide();
            graph2PB.Click += new EventHandler(graphPB_Click); graph2PB.SizeMode = PictureBoxSizeMode.StretchImage; this.Controls.Add(graph2PB); graph2PB.Hide();
            graph3PB.Click += new EventHandler(graphPB_Click); graph3PB.SizeMode = PictureBoxSizeMode.StretchImage; this.Controls.Add(graph3PB); graph3PB.Hide();
            graph4PB.Click += new EventHandler(graphPB_Click); graph4PB.SizeMode = PictureBoxSizeMode.StretchImage; this.Controls.Add(graph4PB); graph4PB.Hide();
            graph5PB.Click += new EventHandler(graphPB_Click); graph5PB.SizeMode = PictureBoxSizeMode.StretchImage; this.Controls.Add(graph5PB); graph5PB.Hide();
            graph6PB.Click += new EventHandler(graphPB_Click); graph6PB.SizeMode = PictureBoxSizeMode.StretchImage; this.Controls.Add(graph6PB); graph6PB.Hide();

        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        public static extern int GetAdaptersInfo(byte[] info, ref uint size);

        /// <summary>
        /// Gets the Mac Address
        /// </summary>
        /// <returns>the mac address or ""</returns>
        public static string getMacAddress()
        {
            uint num = 0u;
            GetAdaptersInfo(null, ref num);
            byte[] array = new byte[(int)((UIntPtr)num)];
            int adaptersInfo = GetAdaptersInfo(array, ref num);
            if (adaptersInfo == 0)
            {
                string macAddress = "";
                int macLength = BitConverter.ToInt32(array, 400);
                macAddress = BitConverter.ToString(array, 404, macLength);
                macAddress = macAddress.Replace("-", ":");

                return macAddress;
            }
            else
                return "";
        }

        private bool checkInternetConnection()
        {
            //int Desc;
            //bool isConnected = InternetGetConnectedState(out Desc, 0);
            //MessageBox.Show(Convert.ToString(isConnected));
            //return isConnected;
            string strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            foreach (IPAddress ipAddress in ipEntry.AddressList)
            {
                if (ipAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    if (ipAddress.ToString() != "127.0.0.1")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public bool fastCheckLicence(string terminalCode)
        {
            try
            {
                string macId = getMacAddress();
                byte[] buffer = Convert.FromBase64String(terminalCode);
                string macFromDb = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                if (macId.Equals(macFromDb))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception exp)
            {
                //when we have convert error in decode terminalCode
                //this error will be if terminalCode = Date
                return false;
            }
            
        }

        public bool checkLicence(string terminalCode)
        {
            //get mac adress
            string macId = getMacAddress();

            if (macId != "")
            {
                bool errorDate = false;
                try
                {
                    DateTime dateText = DateTime.ParseExact(terminalCode, "MMssddyyyymmHH", null); //Month + sec + day + year + minute + hour 
                }
                catch (FormatException)
                {
                    errorDate = true;
                }
                if (errorDate)
                {
                    //DECODE TERMINAL_CODE and check if TERMINAL_CODE = MAC
                    byte[] buffer = Convert.FromBase64String(terminalCode);
                    string macFromDb = Encoding.ASCII.GetString(buffer,0,buffer.Length);
                    if (macId.Equals(macFromDb))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //this first time when the terminal was starting
                    //Encrypted mac id
                    string macEncrypted = Convert.ToBase64String(Encoding.ASCII.GetBytes(macId));

                    //update TERMINALS.TERMINAL_CODE = macEncrypted
                    String tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"UPDATE\"> <DMLIntegration> <DMLInfo> <Schema>TERM</Schema> <TableName>TERMINALS</TableName> ";
                    tcCommand += "<ListOfRows>";
                    tcCommand += "<ListOfColumns>";
                    tcCommand += "<Column><ColumnName>TERMINAL_CODE</ColumnName><ColumnValue>#MAC_CODE</ColumnValue></Column>";
                    tcCommand += "</ListOfColumns>";
                    tcCommand += "</ListOfRows>";
                    tcCommand += "<WhereCondition>";
                    tcCommand += "<ListOfWhereConditions>";
                    tcCommand += "<WhereColumn><WhereColumnName>ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn>";
                    tcCommand += "</ListOfWhereConditions>";
                    tcCommand += "</WhereCondition>";
                    tcCommand += "</DMLInfo></DMLIntegration></TCMessage>";
                    tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                    tcCommand = tcCommand.Replace("#MAC_CODE", macEncrypted);
                    tcCommand = getDMLResult(tcCommand);
                    
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //SHOW main keyboard
            showMainKb();

            //READ configuration settings:
            // * terminal inv number
            try
            {
                //++ path for WEINTEK terminals
                string filePath = "/ResidentFlash/Emul/conf.xml";
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                if (File.Exists(filePath))
                {
                    doc.Load(filePath);
                }
                else
                {
                    //++ path for mobile devices
                    filePath = "/L-Class/Emul/conf.xml";
                    if (File.Exists(filePath))
                    {
                        doc.Load(filePath);
                    }
                    else
                    {
                        //++ path for PCs
                        doc.Load(@"CONF.XML");
                    }
                }
                XmlNodeList nTSList = doc.GetElementsByTagName("TerminalSettings");
                XmlNode nTS = nTSList.Item(0);
                glbDMLService = nTS.ChildNodes[0].ChildNodes[0].Value;
                glbInvNom = nTS.ChildNodes[1].ChildNodes[0].Value;
                intervalOperDataTimer = Convert.ToInt32(nTS.ChildNodes[2].ChildNodes[0].Value) * 1000;
                if (nTS.ChildNodes.Count > 3)
                {
                    intervalRefreshTimer = Convert.ToInt32(nTS.ChildNodes[3].ChildNodes[0].Value) * 1000;
                }
                else 
                {
                    intervalRefreshTimer = 0;
                }

                XmlNodeList nGSList = doc.GetElementsByTagName("GraphsSettings");
                XmlNode nGS = nGSList.Item(0);
                glbGraphServlet = nGS.ChildNodes[0].ChildNodes[0].Value;
            }
            catch (Exception ee)
            {
                MessageBox.Show("TCError: " + ee.ToString());
                //Application.Exit();
            }

            if (checkInternetConnection())
            {
                if (checkHttpPost(glbDMLService))
                {
                    //SELECT terminal messages
                    String tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINAL_MESSAGES</TableName> <TableAlias>T</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>MSG_CONCENTRATOR</ColumnName> <ColumnAlias>MSG_CONCENTRATOR</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>MSG_OUT_RANGE</ColumnName> <ColumnAlias>MSG_OUT_RANGE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>MSG_WRNG_NUMBER</ColumnName> <ColumnAlias>MSG_WRNG_NUMBER</ColumnAlias> </Column> </ListOfColumns> <WhereCondition> <ListOfWhereConditions>AA</ListOfWhereConditions> <ListOfWhereColumns>AA</ListOfWhereColumns> </WhereCondition> </DMLInfo> </DMLIntegration> </TCMessage>";
                    List<List<string>> terminalRes = new List<List<string>>();

                    terminalRes = getSelectResult(tcCommand, 3);
                    if (glbStatus.Equals("Success"))
                    {
                        msgConcentrator = terminalRes[0][0];
                        msgOutOfRange = terminalRes[0][1];
                        msgWrongNumber = terminalRes[0][2];
                    }
                    else
                    {
                        MessageBox.Show("TC Error: Error in initialization! Check your network connection!");
                        Application.Exit();
                    }
                    //SELECT terminal information
                    tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINALS</TableName> <TableAlias>T</TableAlias> </Table><Table> <Schema>TERM</Schema> <TableName>PROGRAMS</TableName> <TableAlias>P</TableAlias> </Table> </ListOfTables>";
                    tcCommand += "<ListOfColumns>";
                    tcCommand += "<Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>ID</ColumnName> <ColumnAlias>ID</ColumnAlias> </Column>";
                    tcCommand += "<Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DESCRIPTION || ' - ' || P.DESCRIPTION</ColumnName> <ColumnAlias>DESCRIPTION</ColumnAlias> </Column>";
                    tcCommand += "<Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DISP_ROWS</ColumnName> <ColumnAlias>DISP_ROWS</ColumnAlias> </Column>";
                    tcCommand += "<Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DISP_COLUMNS</ColumnName> <ColumnAlias>DISP_COLUMNS</ColumnAlias> </Column>";
                    tcCommand += "<Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>TERMINAL_CODE</ColumnName> <ColumnAlias>TERMINAL_CODE</ColumnAlias> </Column>";
                    tcCommand += "</ListOfColumns>";
                    tcCommand += "<WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>T</WhereColumnPrefix> <WhereColumnName>INV_NOM</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#INV_NOM</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns><WhereColumn> <WhereLeftColumnPrefix>T</WhereLeftColumnPrefix> <WhereLeftColumnName>PROGRAM_ID</WhereLeftColumnName> <WhereCondition>=</WhereCondition> <WhereRightColumnPrefix>P</WhereRightColumnPrefix> <WhereRightColumnName>ID</WhereRightColumnName> </WhereColumn></ListOfWhereColumns> </WhereCondition> </DMLInfo> </DMLIntegration> </TCMessage>";
                    tcCommand = tcCommand.Replace("#INV_NOM", glbInvNom);

                    terminalRes = getSelectResult(tcCommand, 5);
                    if (glbStatus.Equals("Success"))
                    {
                        try
                        {
                            glbTerminalId = Convert.ToInt32(terminalRes[0][0]);
                            this.Text = terminalRes[0][1];
                            glbDispRows = Convert.ToInt32(terminalRes[0][2]);
                            glbDispColumns = Convert.ToInt32(terminalRes[0][3]);
                            glbTerminalCode = Convert.ToString(terminalRes[0][4]);
                         }
                         catch (Exception ex)
                         {
                             errMsg.Text = msgConcentrator;
                             errMsg.Show();
                             Application.Exit();
                         }
                    }
                    else
                    {
                        errMsg.Text = msgConcentrator;
                        errMsg.Show();
                        MessageBox.Show(msgConcentrator);
                        Application.Exit();
                    }

                    //++ 20/09/2013 ++ B1537 ++ Teodor
                    /*
                     * 800x600 > 37x17
                     * 1024x768 > 47x27
                     */
                    int fWidth = 800; // default for 7" terminal
                    int fHeight = 465; // default for 7" terminal
                    switch (glbDispColumns)
                    {
                        case 47:
                            //++ WEINTEK 15"
                            fWidth = 1024;
                            deviceType = "T15";
                            break;
                        case 30:
                            //++ HANDHELD 3.5"
                            fWidth = 240;
                            deviceType = "H3.5";
                            break;
                        default:
                            //++ WEINTEK 7"
                            fWidth = 800;
                            deviceType = "T7";
                            break;
                     }
                     switch (glbDispRows)
                     {
                         case 28:
                             fHeight = 748;
                             break;
                         case 11:
                             fHeight = 295;
                             break;
                         default:
                             fHeight = 465;
                             break;
                     }

                     this.Size = new Size(fWidth, fHeight);

                     if (deviceType.Equals("T15"))
                     {
                         bmDone.Location = new Point(856, 332);
                         bmDone.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_done.png"));
                         bmDown.Location = new Point(856, 256);
                         bmDown.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_down.png"));
                         bmEsc.Location = new Point(780, 28);
                         bmEsc.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_esc.png"));
                         bmF1.Location = new Point(780, 104);
                         bmF1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f1.png"));
                         bmF2.Location = new Point(932, 104);
                         bmF2.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f2.png"));
                         bmKB.Location = new Point(932, 28);
                         bmKB.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_kb.png"));
                         bmLeft.Location = new Point(780, 180);
                         bmLeft.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_left.png"));
                         bmRight.Location = new Point(932, 180);
                         bmRight.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_right.png"));
                         bmUp.Location = new Point(856, 104);
                         bmUp.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_up.png"));
                         bmUtils.Location = new Point(932, 256);
                         bmUtils.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_utils.png"));
                         bmGraph.Location = new Point(856, 180);
                         bmGraph.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_graph.png"));
                         bmNull1.Location = new Point(780, 256);
                         bmNull1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.null.png"));
                         bmTab.Location = new Point(780, 256);
                         bmTab.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_tab.png"));

                         bnDone.Location = new Point(932, 712);
                         bnDone.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_done.png"));
                         bnDot.Location = new Point(856, 408);
                         bnDot.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_dot.png"));
                         bnEight.Location = new Point(856, 484);
                         bnEight.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_eight.png"));
                         bnFive.Location = new Point(856, 560);
                         bnFive.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_five.png"));
                         bnFour.Location = new Point(780, 560);
                         bnFour.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_four.png"));
                         bnMinus.Location = new Point(780, 408);
                         bnMinus.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_minus.png"));
                         bnPlus.Location = new Point(780, 332);
                         bnPlus.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_plus.png"));
                         bnNine.Location = new Point(932, 484);
                         bnNine.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_nine.png"));
                         bnOne.Location = new Point(780, 636);
                         bnOne.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_one.png"));
                         bnSeven.Location = new Point(780, 484);
                         bnSeven.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_seven.png"));
                         bnSix.Location = new Point(932, 560);
                         bnSix.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_six.png"));
                         bnTab.Location = new Point(856, 712);
                         bnTab.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_tab.png"));
                         bnThree.Location = new Point(932, 636);
                         bnThree.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_three.png"));
                         bnTwo.Location = new Point(856, 636);
                         bnTwo.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_two.png"));
                         bnZero.Location = new Point(932, 408);
                         bnZero.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_zero.png"));
                         bnBackspace.Location = new Point(932, 28);
                         bnBackspace.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_bs.png"));

                         bmNull1.Hide();
                         bmKB.Hide();
                         bnTab.Hide();
                         bnDone.Hide();

                         bnDot.Show();
                         bnEight.Show();
                         bnFive.Show();
                         bnFour.Show();
                         bnMinus.Show();
                         bnPlus.Show();
                         bnNine.Show();
                         bnOne.Show();
                         bnSeven.Show();
                         bnSix.Show();
                         bnThree.Show();
                         bnTwo.Show();
                         bnZero.Show();
                         bnBackspace.Show();

                     }
                     else if (deviceType.StartsWith("H"))
                     {
                         int y_pos = 247;
                         bmTab.Size = new Size(20, 20);
                         bmTab.Location = new Point(21, y_pos);
                         bmTab.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.num_tab_20.png"));
                         bmF1.Size = new Size(20, 20);
                         bmF1.Location = new Point(42, y_pos);
                         bmF1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f1_20.png"));
                         bmF2.Size = new Size(20, 20);
                         bmF2.Location = new Point(63, y_pos);
                         bmF2.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f2_20.png"));
                         bmLeft.Size = new Size(20, 20);
                         bmLeft.Location = new Point(84, y_pos);
                         bmLeft.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_left_20.png"));
                         bmRight.Size = new Size(20, 20);
                         bmRight.Location = new Point(105, y_pos);
                         bmRight.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_right_20.png"));
                         bmGraph.Size = new Size(20, 20);
                         bmGraph.Location = new Point(126, y_pos);
                         bmGraph.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_graph_20.png"));
                         bmUp.Size = new Size(20, 20);
                         bmUp.Location = new Point(147, y_pos);
                         bmUp.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_up_20.png"));
                         bmDown.Size = new Size(20, 20);
                         bmDown.Location = new Point(168, y_pos);
                         bmDown.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_down_20.png"));
                         bmUtils.Size = new Size(20, 20);
                         bmUtils.Location = new Point(189, y_pos);
                         bmUtils.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_utils_20.png"));

                         bmDone.Dispose();
                         bmEsc.Dispose();
                         bmKB.Dispose();
                         bmNull1.Dispose();
                         bnDone.Dispose();
                         bnDot.Dispose();
                         bnEight.Dispose();
                         bnFive.Dispose();
                         bnFour.Dispose();
                         bnMinus.Dispose();
                         bnPlus.Dispose();
                         bnNine.Dispose();
                         bnOne.Dispose();
                         bnSeven.Dispose();
                         bnSix.Dispose();
                         bnTab.Dispose();
                         bnThree.Dispose();
                         bnTwo.Dispose();
                         bnZero.Dispose();
                         bnBackspace.Dispose();

                         lTECHNOCLASS.Dispose();
                         errMsg.Dispose();

                         this.FormBorderStyle = FormBorderStyle.FixedSingle;
                         this.WindowState = FormWindowState.Maximized;
                         this.TopMost = false;

                     }
                     else
                     {
                         bnPlus.Hide();
                     }


                     this.Location = new Point(0, 0);

                     if (checkLicence(glbTerminalCode))
                     {
                         //FUNCTION initialization
                         tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"FUNCTION\"> <DMLIntegration> <DMLInfo> <Schema>TERM</Schema> <UnitName>INITIALIZATION</UnitName> <ResultType>NUMBER</ResultType> <ListOfParameters> <Parameter> <ParameterValue>#TERMINAL_ID</ParameterValue> <ParameterType>IN</ParameterType> </Parameter> </ListOfParameters> </DMLInfo> </DMLIntegration> </TCMessage>";
                         tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());

                         glbIterationNo = Convert.ToInt32(getDMLResult(tcCommand));

                         fillTerminalPanel();

                         //========= Strat timers =====================
                         //Timer 1 : refresh timer screen
                         if (intervalRefreshTimer == 0)
                         {
                             refreshTimer.Enabled = false;
                         }
                         else
                         {
                             refreshTimer.Enabled = false;
                             refreshTimer.Interval = intervalRefreshTimer;
                             refreshTimer.Enabled = true;
                         }
                         //Timer 2 : operDataTimer - Message
                         if (intervalOperDataTimer == 0)
                         {
                             operDataTimer.Enabled = false;
                         }
                         else
                         {
                             operDataTimer.Enabled = false;
                             operDataTimer.Interval = intervalOperDataTimer;
                             operDataTimer.Enabled = true;
                         }
                     }
                     else
                     {
                         MessageBox.Show("Your license is disturbed! The program will close!",
                              "Disturbed license",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Hand,
                               MessageBoxDefaultButton.Button1);
                         Application.Exit();
                     }
                }
                else
                {
                    //Terminal no find DML Service
                    MessageBox.Show("You have problem with DML service. The program will close!",
                                    "You have problem with DML service",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Hand,
                                                MessageBoxDefaultButton.Button1);
                    Application.Exit();
                }
            }
            else
            {
                MessageBox.Show("You don't have internet connection. The program will close!",
                                "You don't have Internet connection",
                                                 MessageBoxButtons.OK,
                                                 MessageBoxIcon.Hand,
                                                 MessageBoxDefaultButton.Button1);
                Application.Exit();
            }
            Cursor.Current = Cursors.Default;

        }

        #region *** Events ***
        private void bmDone_Click(object sender, EventArgs e)
        {
            nextCommand("ENT");
        }
        private void bmDown_Click(object sender, EventArgs e)
        {
            nextCommand("DOWN");
        }
        private void bmEsc_Click(object sender, EventArgs e)
        {
            nextCommand("ESC");
        }
        private void bmF1_Click(object sender, EventArgs e)
        {
            nextCommand("F1");
        }
        private void bmF2_Click(object sender, EventArgs e)
        {
            nextCommand("F2");
        }
        private void bmKB_Click(object sender, EventArgs e)
        {
            showNumericKb();
        }
        private void bmLeft_Click(object sender, EventArgs e)
        {
            nextCommand("LEFT");
        }
        private void bmRight_Click(object sender, EventArgs e)
        {
            nextCommand("RIGHT");
        }
        private void bmUp_Click(object sender, EventArgs e)
        {
            nextCommand("UP");
        }
        public int serialCounter = 0;
        private void bmUtils_Click(object sender, EventArgs e) 
        {
            string tooltip = "Emul ver.TC.5.3.1 \n" +
                             "STATUS: " + glbStatus + "\n" +
                             "TERMINAL: " + glbInvNom + " [" + glbTerminalId + "]\n" +
                             "PROGRAM: ";


            string tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINALS</TableName> <TableAlias>T</TableAlias> </Table> <Table> <Schema>TERM</Schema> <TableName>PROGRAMS</TableName> <TableAlias>P</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>P</ColumnPrefix> <ColumnName>NOM || ' ' || P.DESCRIPTION</ColumnName> <ColumnAlias>PRG</ColumnAlias> </Column> </ListOfColumns> <WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>T</WhereColumnPrefix> <WhereColumnName>ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns> <WhereColumn> <WhereLeftColumnPrefix>T</WhereLeftColumnPrefix><WhereLeftColumnName>PROGRAM_ID</WhereLeftColumnName><WhereCondition>=</WhereCondition><WhereRightColumnPrefix>P</WhereRightColumnPrefix><WhereRightColumnName>ID</WhereRightColumnName></WhereColumn> </ListOfWhereColumns> </WhereCondition> <OrderClause>AA</OrderClause> </DMLInfo> </DMLIntegration> </TCMessage>";
            tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
            List<List<string>> terminalRes = new List<List<string>>();

            terminalRes = getSelectResult(tcCommand, 1);
            if (glbStatus.Equals("Success"))
            {
                foreach (List<string> row in terminalRes)
                {
                    tooltip += row[0] + "\n";
                }
            }
            tooltip +=
                             "ITERATION: " + glbIterationNo + "\n" +
                             "----------------------------\n" +
                             "CURRENT PROCEDURE: ";

            tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>PROGRAMS_DATA</TableName> <TableAlias>PD</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>PD</ColumnPrefix> <ColumnName>FIELD_DATA</ColumnName> <ColumnAlias>FIELD_DATA</ColumnAlias> </Column> </ListOfColumns> <WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>PD</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> <WhereColumn> <WhereColumnPrefix>PD</WhereColumnPrefix> <WhereColumnName>STEP</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>999</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns>AA</ListOfWhereColumns> </WhereCondition> <OrderClause>AA</OrderClause> </DMLInfo> </DMLIntegration> </TCMessage>";
            tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
            terminalRes = new List<List<string>>();

            terminalRes = getSelectResult(tcCommand, 1);
            if (glbStatus.Equals("Success"))
            {
                foreach (List<string> row in terminalRes)
                {
                    tooltip += row[0] + "\n";
                }
            }

            tooltip += "NEXT PROCEDURES:\n";

            tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINAL_NEXT_COMMAND</TableName> <TableAlias>TNC</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>TNC</ColumnPrefix> <ColumnName>FNC_KEY</ColumnName> <ColumnAlias>FNC_KEY</ColumnAlias> </Column> <Column> <ColumnPrefix>TNC</ColumnPrefix> <ColumnName>COMMAND</ColumnName> <ColumnAlias>COMMAND</ColumnAlias> </Column> </ListOfColumns> <WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>TNC</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> <WhereColumn> <WhereColumnPrefix>TNC</WhereColumnPrefix> <WhereColumnName>ITERATION_NO</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#ITERATION_NO</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns>AA</ListOfWhereColumns> </WhereCondition> <OrderClause>AA</OrderClause> </DMLInfo> </DMLIntegration> </TCMessage>";
            tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
            tcCommand = tcCommand.Replace("#ITERATION_NO", glbIterationNo.ToString());
            terminalRes = new List<List<string>>();

            terminalRes = getSelectResult(tcCommand, 2);
            if (glbStatus.Equals("Success"))
            {
                foreach (List<string> row in terminalRes)
                {
                    tooltip += "[" + row[0] + "] " + row[1] + "\n";
                }
            }

            tooltip +=
                             "----------------------------\n" +
                             "DEFINED PORTS: \n";

            tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINAL_DEVICES</TableName> <TableAlias>TD</TableAlias> </Table> <Table> <Schema>TERM</Schema> <TableName>DEVICE_TYPES</TableName> <TableAlias>DT</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>TD</ColumnPrefix> <ColumnName>DEVICE_TYPE || ' ' || DT.DESCRIPTION || ' [' || DT.DELIMITER || ']'</ColumnName> <ColumnAlias>DTYPE</ColumnAlias> </Column> <Column> <ColumnPrefix>TD</ColumnPrefix> <ColumnName>PORT</ColumnName> <ColumnAlias>PORT</ColumnAlias> </Column> <Column> <ColumnPrefix>TD</ColumnPrefix> <ColumnName>SPEED</ColumnName> <ColumnAlias>SPEED</ColumnAlias> </Column> </ListOfColumns> <WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>TD</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns> <WhereColumn> <WhereLeftColumnPrefix>TD</WhereLeftColumnPrefix><WhereLeftColumnName>DEVICE_TYPE</WhereLeftColumnName><WhereCondition>=</WhereCondition><WhereRightColumnPrefix>DT</WhereRightColumnPrefix><WhereRightColumnName>TYPE</WhereRightColumnName></WhereColumn> </ListOfWhereColumns> </WhereCondition> <OrderClause>AA</OrderClause> </DMLInfo> </DMLIntegration> </TCMessage>";
            tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
            terminalRes = new List<List<string>>();

            terminalRes = getSelectResult(tcCommand, 3);
            if (glbStatus.Equals("Success"))
            {
                foreach (List<string> row in terminalRes)
                {
                    tooltip += "[" + row[1] + "] " + row[0] + "[" + row[2] + "]\n";
                }
            }

            tooltip +=
                             "----------------------------\n" +
                             "DISPLAY PORTS: \n";
            foreach (termControl c in screenControls)
            {
                if (c.ComPort != null)
                {
                    tooltip += c.ComPort.PortName + "(" + c.ComPort.BaudRate + "): [" +
                                c.Control.Left + ";" + c.Control.Top + "][" + Math.Round(c.Control.Top / 24.4) + "] \n";
                }
            }

            tooltip +=
                             "----------------------------\n" +
                             "GRAPHS: " + graphsFound;
            MessageBox.Show(tooltip);
        }

        private void bmGraph_Click(object sender, EventArgs e) 
        {
            if (graphsFound == 0)
            {
                errMsg.Text = "NO GRAPH";
                errMsg.Show();
                return;
            }

            //TODO: more than one graph
            //TODO: make real sizes
            if (graphShown)
            {
                graph1PB.Hide();
                graph2PB.Hide();
                graph3PB.Hide();
                graph4PB.Hide();
                graph5PB.Hide();
                graph6PB.Hide();
                graphShown = false;
            }
            else
            {
                graphShown = true;
                if (graphsFound >= 1)
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbGraphCredentials + graph1Index);
                        req.Method = "GET";
                        System.Net.WebResponse resp = req.GetResponse();
                        Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                        resp.Close();

                        graph1PB.Image = graphBMP;
                        graph1PB.Show();
                    }
                    catch (Exception grExc) { }
                }
                if (graphsFound >= 2)
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbGraphCredentials + graph2Index);
                        req.Method = "GET";
                        System.Net.WebResponse resp = req.GetResponse();
                        Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                        resp.Close();

                        graph2PB.Image = graphBMP;
                        graph2PB.Show();
                    }
                    catch (Exception grExc) { }
                }
                if (graphsFound >= 3)
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbGraphCredentials + graph3Index);
                        req.Method = "GET";
                        System.Net.WebResponse resp = req.GetResponse();
                        Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                        resp.Close();

                        graph3PB.Image = graphBMP;
                        graph3PB.Show();
                    }
                    catch (Exception grExc) { }
                }
                if (graphsFound >= 4)
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbGraphCredentials + graph4Index);
                        req.Method = "GET";
                        System.Net.WebResponse resp = req.GetResponse();
                        Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                        resp.Close();

                        graph4PB.Image = graphBMP;
                        graph4PB.Show();
                    }
                    catch (Exception grExc) { }
                }
                if (graphsFound >= 5)
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbGraphCredentials + graph5Index);
                        req.Method = "GET";
                        System.Net.WebResponse resp = req.GetResponse();
                        Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                        resp.Close();

                        graph5PB.Image = graphBMP;
                        graph5PB.Show();
                    }
                    catch (Exception grExc) { }
                }
                if (graphsFound >= 6)
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbGraphCredentials + graph6Index);
                        req.Method = "GET";
                        System.Net.WebResponse resp = req.GetResponse();
                        Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                        resp.Close();

                        graph6PB.Image = graphBMP;
                        graph6PB.Show();
                    }
                    catch (Exception grExc) { }
                }
            }
        }
        private void bnDone_Click(object sender, EventArgs e)
        {
            showMainKb();
            //bmDone_Click(sender, e);
        }
        private void bnDot_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += ".";
        }
        private void bnEight_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "8";
        }
        private void bnFive_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "5";
        }
        private void bnFour_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "4";
        }
        private void bnMinus_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "-";
        }
        private void bnPlus_Click(object sender, EventArgs e) 
        {
            currTB.Control.Text += "+";
        }
        private void bnNine_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "9";
        }
        private void bnOne_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "1";
        }
        private void bnSeven_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "7";
        }
        private void bnSix_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "6";
        }
        private void bnTab_Click(object sender, EventArgs e)
        {
            if (cntTbxs == 0)
                return;

            int tbCounter = 1;
            currTbIndex--;
            if (currTbIndex < 1)
                currTbIndex = cntTbxs;

            foreach (termControl c in screenControls)
            {
                if (c.Control is TextBox)
                {
                    if (tbCounter == currTbIndex)
                    {
                        currTB = c;
                        currTB.Control.Focus();
                        return;
                    }
                    else
                    {
                        tbCounter++;
                    }
                }
            }

            currTbIndex = 1;
        }
        private void bnThree_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "3";
        }
        private void bnTwo_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "2";
        }
        private void bnZero_Click(object sender, EventArgs e)
        {
            currTB.Control.Text += "0";
        }
        private void bnBackspace_Click(object sender, EventArgs e)
        {
            try
            {
                currTB.Control.Text = currTB.Control.Text.Substring(0, currTB.Control.Text.Length - 1);
            }
            catch (Exception eex) { }
        }
        private void myTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            EventArgs eee = new EventArgs();
            switch (e.KeyChar)
            {
                case (char)13:
                    bmDone_Click(sender, eee);
                    break;
                case (char)27:
                    bmEsc_Click(sender, eee);
                    break;
            }
        }

        private void dataButton_Click(object sender, EventArgs e)
        {
            Button clickedButon = (Button)sender;
            currTB.Control.Text = clickedButon.Text.Substring(0, clickedButon.Text.IndexOf(" "));
            nextCommand("ENT");
        }
        public string readBuffer = string.Empty;
        public void DoUpdate(object sender, System.EventArgs e)
        {
            currTB.Control.Text = readBuffer;
        }
        private void sp_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            try
            {
                string spName = sp.PortName.ToString();
                string currComName = currTB.ComPort.PortName.ToString();
                string currDelimiter = currTB.PortDelimiter.ToString();
                string fieldType = currTB.FieldType.ToString();
                string delimiter = currTB.PortDelimiter.ToString();

                if (spName.Equals(currComName))
                {
                    if (currDelimiter.Equals("null"))
                    {
                        //barcode reader
                        string indataBC = sp.ReadLine();
                        //remove CR
                        indataBC = indataBC.TrimEnd('\r', '\n');
                        readBuffer = indataBC;
                    }
                    else if (currDelimiter.Equals("+"))
                    {
                        //shubler
                        System.Threading.Thread.Sleep(500);
                        string indataSH = sp.ReadExisting();
                        //remove CR
                        indataSH = indataSH.TrimEnd('\r', '\n');
                        Regex objIntPattern = new Regex("[+|-]");
                        int signIdx = objIntPattern.Match(indataSH).Index;
                        readBuffer = indataSH.Substring(signIdx).TrimStart('+').TrimStart('0').TrimEnd('0');
                    }
                    else if (currDelimiter.Equals("B"))
                    {
                        //scale
                        System.Threading.Thread.Sleep(200);
                        string indataSH = sp.ReadExisting();
                        //remove CR
                        indataSH = indataSH.TrimEnd('\r', '\n');
                        char[] trimChars = new char[2];
                        trimChars[0] = 'A';
                        trimChars[1] = 'B';
                        readBuffer = indataSH.TrimEnd(trimChars).TrimStart('0');
                    }
                    System.Threading.Thread.Sleep(50);

                    this.Invoke(new EventHandler(DoUpdate));

                }
            }
            catch (Exception spExp)
            { }
            finally
            {
                sp.DiscardInBuffer();
            }

        }

        private void graphPB_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            if (graphLarge)
            {
                graphLarge = false;
                pb.Width = graphLargeWidth;
                pb.Height = graphLargeHeight;
                pb.Location = graphLargeLocation;
                showBigKB();
                foreach (termControl c in screenControls)
                {
                    c.Control.Visible = true;
                }

                if (graphsFound >= 1) graph1PB.Show();
                if (graphsFound >= 2) graph2PB.Show();
                if (graphsFound >= 3) graph3PB.Show();
                if (graphsFound >= 4) graph4PB.Show();
                if (graphsFound >= 5) graph5PB.Show();
                if (graphsFound >= 6) graph6PB.Show();
            }
            else
            {
                graphLarge = true;
                graphLargeWidth = pb.Width;
                graphLargeHeight = pb.Height;
                graphLargeLocation = pb.Location;
                if (glbDispColumns >= 47)
                {
                    pb.Width = 1020;
                    pb.Height = 760;
                }
                else
                {
                    pb.Width = 560;
                    pb.Height = 420;
                }
                pb.Location = new Point(0, 0);
                hideBigKB();
                foreach (termControl c in screenControls)
                {
                    c.Control.Visible = false;
                }

                graph1PB.Hide();
                graph2PB.Hide();
                graph3PB.Hide();
                graph4PB.Hide();
                graph5PB.Hide();
                graph6PB.Hide();
                pb.Show();
            }
        }

        private void imageDB_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            if (imageDBLarge)
            {
                imageDBLarge = false;
                pb.Width = imageLargeWidth;
                pb.Height = imageLargeHeight;
                pb.Location = imageLargeLocation;
                showBigKB();
                foreach (termControl c in screenControls)
                {
                    c.Control.Visible = true;
                }
            }
            else
            {
                imageDBLarge = true;
                imageLargeWidth = pb.Width;
                imageLargeHeight = pb.Height;
                imageLargeLocation = pb.Location;
                if (glbDispColumns >= 47)
                {
                    pb.Width = 1020;
                    pb.Height = 760;
                }
                else
                {
                    pb.Width = 560;
                    pb.Height = 420;
                }
                pb.Location = new Point(0, 0);
                hideBigKB();
                foreach (termControl c in screenControls)
                {
                    c.Control.Visible = false;
                }
                pb.Show();
            }
        }
        #endregion Events

        private void showMainKb()
        {
            bnDone.Hide();
            bnDot.Hide();
            bnEight.Hide();
            bnFive.Hide();
            bnFour.Hide();
            bnMinus.Hide();
            bnNine.Hide();
            bnOne.Hide();
            bnSeven.Hide();
            bnSix.Hide();
            bnTab.Hide();
            bnThree.Hide();
            bnTwo.Hide();
            bnZero.Hide();
            bnBackspace.Hide();

            bmDone.Show();
            bmDown.Show();
            bmEsc.Show();
            bmF1.Show();
            bmF2.Show();
            bmKB.Show();
            bmLeft.Show();
            bmRight.Show();
            bmUp.Show();
            bmUtils.Show();
            bmGraph.Show();
            bmNull1.Show();
            bmTab.Show();
        }

        private void showNumericKb()
        {
            bmDone.Hide();
            bmDown.Hide();
            bmEsc.Hide();
            bmF1.Hide();
            bmF2.Hide();
            bmKB.Hide();
            bmLeft.Hide();
            bmRight.Hide();
            bmUp.Hide();
            bmUtils.Hide();
            bmGraph.Hide();
            bmNull1.Hide();
            bmTab.Hide();

            bnDone.Show();
            bnDot.Show();
            bnEight.Show();
            bnFive.Show();
            bnFour.Show();
            bnMinus.Show();
            bnNine.Show();
            bnOne.Show();
            bnSeven.Show();
            bnSix.Show();
            bnTab.Show();
            bnThree.Show();
            bnTwo.Show();
            bnZero.Show();
            bnBackspace.Show();
        }

        private void showBigKB()
        {
            bmDone.Show();
            bmDown.Show();
            bmEsc.Show();
            bmF1.Show();
            bmF2.Show();
            bmLeft.Show();
            bmRight.Show();
            bmUp.Show();
            bmUtils.Show();
            bmGraph.Show();
            bmTab.Show();

            bnDot.Show();
            bnEight.Show();
            bnFive.Show();
            bnFour.Show();
            bnMinus.Show();
            bnNine.Show();
            bnOne.Show();
            bnSeven.Show();
            bnSix.Show();
            bnThree.Show();
            bnTwo.Show();
            bnZero.Show();
            bnBackspace.Show();
            bnPlus.Show();
        }

        private void hideBigKB()
        {
            bmDone.Hide();
            bmDown.Hide();
            bmEsc.Hide();
            bmF1.Hide();
            bmF2.Hide();
            bmLeft.Hide();
            bmRight.Hide();
            bmUp.Hide();
            bmUtils.Hide();
            bmGraph.Hide();
            bmTab.Hide();

            bnDone.Hide();
            bnDot.Hide();
            bnEight.Hide();
            bnFive.Hide();
            bnFour.Hide();
            bnMinus.Hide();
            bnNine.Hide();
            bnOne.Hide();
            bnSeven.Hide();
            bnSix.Hide();
            bnThree.Hide();
            bnTwo.Hide();
            bnZero.Hide();
            bnBackspace.Hide();
            bnPlus.Hide();
        }

        #region *** Positions ***
        private Font getLabelsFont()
        {
            Font rtnFont = new Font("Courier New", 18, FontStyle.Regular);

            if (deviceType.StartsWith("H"))
                rtnFont = new Font("Courier New", 10, FontStyle.Regular);

            if (deviceType.StartsWith("T7"))
                rtnFont = new Font("Courier New", 12, FontStyle.Regular);

            return (rtnFont);
        }

        private Font getButtonsFont()
        {
            Font rtnFont = new Font("Courier New", 14, FontStyle.Regular);

            if (deviceType.StartsWith("H"))
                rtnFont = new Font("Courier New", 8, FontStyle.Regular);

            if (deviceType.StartsWith("T7"))
                rtnFont = new Font("Courier New", 12, FontStyle.Regular);


            return (rtnFont);
        }

        private int getLabelWidth(int _len, int _fontSize)
        {
            int wid = 0;
            //TODO: get real sizes
            switch (_fontSize)
            {
                case 2:
                    wid = (int)Math.Round(_len * 18.0); // every char is 16.5 px
                    break;
                case 1:
                    wid = (int)Math.Round(_len * 10.0); // every char is 16.5 px
                    break;
            }
            if (wid > 1 && wid <= 90)
                wid += 7;
            else if (wid > 90 & wid <= 270)
                wid -= 2;
            else if (wid > 270 && wid <= 576)
                wid -= 50;
            else if (wid > 576)
                wid -= 100;

            if (deviceType.StartsWith("H") && wid > 220)
                wid = 220;

            return (wid);
        }

        private int getTBWidth(int _len)
        {
            //TODO: get real sizes
            return (int)Math.Round(_len * 16.5); // every char is 16.5 px
        }

        private Point getImagePosition(int _pos, int _row)
        {
            int x = (int)Math.Round((_pos - 1) * 15.13);
            int y = (int)Math.Round((_row - 1) * 24.7);
            
            return (new Point(x, y));
        }

        private Size getGraphSize(int _width, int _height)
        {
            int w = 0;
            int h = 0;

            w = _width;
            h = _height;

            return (new Size(w, h));
        }

        private Point getGraphPosition(int _pos, int _row)
        {
            int x = 0;
            int y = 0;
            if (deviceType.Equals("T15"))
            {
                switch (_pos)
                {
                    case 1: x = 0; break;
                    case 2: x = 260; break;
                    default: x = 520; break;
                }

                switch (_row)
                {
                    case 1: y = 50; break;
                    default: y = 250; break;
                }
            }
            else if (deviceType.Equals("T7"))
            {
                switch (_pos)
                {
                    case 1: x = 0; break;
                    case 2: x = 186; break;
                    default: x = 373; break;
                }

                switch (_row)
                {
                    case 1: y = 5; break;
                    default: y = 235; break;
                }
            }
            else if (deviceType.StartsWith("H"))
            {
                x = 20;

                switch (_row)
                {
                    case 1: y = 5; break;
                    default: y = 140; break;
                }
            }
            return (new Point(x, y));
        }

        private int getButtonHeight()
        {
            int h = 45;

            if (deviceType.StartsWith("H"))
            {
                h = 20;
            }

            return (h);
        }

        private int getTextBoxHeight() 
        {
            int h = 32;

            if (deviceType.StartsWith("H")) 
            {
                h = 15;
            }
            if (deviceType.StartsWith("T7"))
            {
                h = 28;
            }

            return h;
        }

        private int getLabelHeight() 
        {
            int h = 24;

            if (deviceType.StartsWith("H"))
            {
                h = 17;
            }
            if (deviceType.StartsWith("T7"))
            {
                h = 21;
            }

            return h;
        }

        private Size getImgSize()
        {
            if (deviceType.StartsWith("H"))
                return new System.Drawing.Size(238, 255);;

            return new Size(560, 420);
        }

        private Point getPositionTermH(int _pos, int _row, char typeElem)
        {   
            //Button and Text components
            int x = (int)Math.Round(3 + (_pos - 1) * 10.0);
            int y = (int)Math.Round((_row - 1) * 21.5);

            if (typeElem == 'L')
            {   //Label component
                x = (int)Math.Round(3 + (_pos - 1) * 10.0);
                y = (int)Math.Round(4 + (_row - 1) * 21.5);
            }
            return (new Point(x, y));
        }

        private Point getPositionTermT(int _pos, int _row, char typeElem)
        {   
            //Button and Label component
            int x = (int)Math.Round(3 + (_pos - 1) * 16.5);
            int y = 1 + (_row - 1) * 25;

            if (typeElem == 'T')
            {   //Text Component
                x = (int)Math.Round(3 + (_pos - 1) * 15.2);
                y = (int)Math.Round(1 + (_row - 1) * 24.4);
            }

            return (new Point(x, y));
        }

        private Point getPositionTermT_SevenInch(int _pos, int _row, char typeElem)
        {
            //Button and Label component
            int x = (int)Math.Round(3 + (_pos - 1) * 16.5);
            int y = 1 + (_row - 1) * 25;

            if (typeElem == 'T')
            {   //Text Component
                x = (int)Math.Round(3 + (_pos - 1) * 15.2);
                y = (int)Math.Round(-3 + (_row - 1) * 24.4);
            }
            if (typeElem == 'L')
            {   //Label component
                x = (int)Math.Round(3 + (_pos - 1) * 15.2);
                y = (int)Math.Round(2 + (_row - 1) * 24.4);
            }

            return (new Point(x, y));
        }

        private Point getPosition(int _pos, int _row, char typeElem)
        {
            if (deviceType.StartsWith("H"))
            {   //Honeywell - Mobile device
                return this.getPositionTermH(_pos, _row, typeElem);
            }
            else
            {   //Device -  T7
                if (deviceType.StartsWith("T7"))
                {
                    return this.getPositionTermT_SevenInch(_pos, _row, typeElem);
                }
                else
                {   //Device - T15 or other
                    return this.getPositionTermT(_pos, _row, typeElem);
                }
            }
        }

        #endregion *** Positions ***

        #region *** DML Operations ***

        public bool checkHttpPost(string url)
        {
            HttpWebResponse response;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                //request.Timeout = 5000; //set the timeout to 5 seconds to keep the user from waiting too long for the page to load
                request.Method = "HEAD"; //Get only the header information -- no need to download any content

                response = request.GetResponse() as HttpWebResponse;

                int statusCode = (int)response.StatusCode;
                if (statusCode >= 100 && statusCode < 400) //Good requests
                {
                    response.Close();
                    return true;
                }
                else if (statusCode >= 500 && statusCode <= 510) //Server Errors
                {
                    MessageBox.Show(String.Format("The remote server has thrown an internal error. Url is not valid: {0}", url));
                    response.Close();
                    return false;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) //400 errors
                {
                    return false;
                }
                else
                {
                    MessageBox.Show(String.Format("Unhandled status [{0}] returned for url: {1}", ex.Status, url));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Could not test url {0}.", url));
            }
            return false;
        }

        public string HttpPost(string URI, string Parameters)
        {
            if (checkInternetConnection())
            {
                if (checkHttpPost(URI))
                {
                    try
                    {
                        System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
                        //++req.Proxy = new System.Net.WebProxy(ProxyString, true);
                        //Add these, as we're doing a POST
                        req.ContentType = "application/x-www-form-urlencoded";
                        req.Method = "POST";
                        //We need to count how many bytes we're sending. 
                        //Post'ed Faked Forms should be name=value&
                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
                        req.ContentLength = bytes.Length;
                        System.IO.Stream os = req.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length); //Push it out there
                        os.Close();

                        System.Net.WebResponse resp = req.GetResponse();

                        if (resp == null) return null;
                        System.IO.StreamReader sr =
                            new System.IO.StreamReader(resp.GetResponseStream());

                        string res = sr.ReadToEnd().Trim();
                        resp.Close();//++
                        return res;
                    }
                    catch (Exception httpEx)
                    {
                        return null;
                    }
                }
                else
                {
                    DialogResult res = new DialogResult();
                    res = MessageBox.Show("Do you want to close the program?",
                                                    "You have problem with your connection to DML Service!",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question,
                                                     MessageBoxDefaultButton.Button2);
                    if (res == DialogResult.Yes)
                    {
                        Application.Exit();
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                DialogResult res = new DialogResult();
                res = MessageBox.Show("Do you want to close the program?",
                                                "You don't have Internet connection",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question,
                                                 MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    Application.Exit();
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        private MemoryStream getMStream(string xmlData)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(xmlData);

            MemoryStream memStream = new MemoryStream(byteArray);

            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }

        private List<List<string>> getSelectResult(string request, int columns)
        {
            Cursor.Current = Cursors.WaitCursor;
            List<List<string>> result = new List<List<string>>();
            String resp;
            resp = HttpPost(glbDMLService,
                glbCredentials + request);

            if (resp == null)
            {
                glbStatus = "Rejected";
                return result;
            }
            MemoryStream mStream = getMStream(resp);

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load(mStream);
                XmlNodeList nDMLInfoList = doc.GetElementsByTagName("DMLInfo");
                XmlNode nDMLInfo = nDMLInfoList.Item(0);

                if (nDMLInfo.ChildNodes[1].ChildNodes[0].Value.Equals("Success")) //status value
                {
                    glbStatus = "Success";
                    glbRows = 0;
                    XmlNodeList listOfRows = doc.GetElementsByTagName("Row");
                    foreach (XmlNode x in listOfRows)
                    {
                        List<string> r = new List<string>();
                        for (int i = 0; i < columns; i++)
                        {
                            XmlNode tmpNode = x.ChildNodes[i];
                            if (tmpNode.ChildNodes.Count > 0)
                            {
                                r.Add(x.ChildNodes[i].ChildNodes[0].Value);
                            }
                            else 
                            {
                                r.Add("null");
                            }
                        }
                        result.Add(r);
                        glbRows++;
                    }
                }
                else
                {
                    glbStatus = "Rejected";
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("TCError: " + ee.ToString() + "\nsuccess rows: " + glbRows);
            }

            Cursor.Current = Cursors.Default;

            return result;

        }

        private string getDMLResult(string request)
        {
            Cursor.Current = Cursors.WaitCursor;
            string result = "";
            String resp;
            resp = HttpPost(glbDMLService,
                glbCredentials + request);

            if (resp == null)
            {
                glbStatus = "Rejected";
                return result;
            }

            MemoryStream mStream = getMStream(resp);

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load(mStream);
                XmlNodeList nDMLInfoList = doc.GetElementsByTagName("DMLInfo");
                XmlNode nDMLInfo = nDMLInfoList.Item(0);

                if (nDMLInfo.ChildNodes[1].ChildNodes[0].Value.Equals("Success")) //status value
                {
                    glbStatus = "Success";
                    result = nDMLInfo.ChildNodes[3].ChildNodes[0].Value;
                    glbRows = 0;
                }
                else
                {
                    glbStatus = "Rejected";
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("TCError: " + ee.ToString());
            }

            Cursor.Current = Cursors.Default;

            return result;

        }
        #endregion DML operations

        private void fillTerminalPanel()
        {
            bool printFound = false;
            bool imageFound = false;
            graphsFound = 0;
            graphShown = false;
            graph1PB.Hide();
            graph2PB.Hide();
            graph3PB.Hide();
            graph4PB.Hide();
            graph5PB.Hide();
            graph6PB.Hide();
            graph1Index = "";
            graph2Index = "";
            graph3Index = "";
            graph4Index = "";
            graph5Index = "";
            graph6Index = "";
            string printText = "";

            currTbIndex = 0;
            cntTbxs = 0;
            requiredPassFirstTime = false;

            this.BackColor = Color.FromArgb(192, 192, 255);

            //SELECT terminal_oper_data
            String tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo>";
            tcCommand += "<ListOfTables>";
            tcCommand += "<Table> <Schema>TERM</Schema> <TableName>TERMINAL_OPER_DATA</TableName> <TableAlias>TOD</TableAlias> </Table>";
            tcCommand += "<Table> <Schema>TERM</Schema> <TableName>TERMINALS</TableName> <TableAlias>TER</TableAlias> </Table>";
            tcCommand += "</ListOfTables>";
            tcCommand += "<ListOfColumns>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>ROW_NR</ColumnName> <ColumnAlias>ROW_NR</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>POS_NR</ColumnName> <ColumnAlias>POS_NR</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FIELD_TYPE</ColumnName> <ColumnAlias>FIELD_TYPE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FIELD_DATA</ColumnName> <ColumnAlias>FIELD_DATA</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>DATA_TYPE</ColumnName> <ColumnAlias>DATA_TYPE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>NEGATIVE</ColumnName> <ColumnAlias>NEGATIVE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FONT_SIZE</ColumnName> <ColumnAlias>FONT_SIZE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>SIZE_VAL</ColumnName> <ColumnAlias>SIZE_VAL</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>DEVICE_ID</ColumnName> <ColumnAlias>DEVICE_ID</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>MIN_VAL</ColumnName> <ColumnAlias>MIN_VAL</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>MAX_VAL</ColumnName> <ColumnAlias>MAX_VAL</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>ALIGN</ColumnName> <ColumnAlias>ALIGN</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>REQUIRED</ColumnName> <ColumnAlias>REQUIRED</ColumnAlias> </Column> ";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>COLOR</ColumnName> <ColumnAlias>COLOR</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TER</ColumnPrefix> <ColumnName>TERMINAL_CODE</ColumnName> <ColumnAlias>TERMINAL_CODE</ColumnAlias></Column>";
            tcCommand += "</ListOfColumns>";
            tcCommand += "<WhereCondition>";
            tcCommand += "<ListOfWhereConditions>";
            tcCommand += "<WhereColumn> <WhereColumnPrefix>TOD</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn>";
            tcCommand += "<WhereColumn> <WhereColumnPrefix>TOD</WhereColumnPrefix> <WhereColumnName>ITERATION_NO</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#ITERATION_NO</WhereColumnValue> </WhereColumn>";
            tcCommand += "</ListOfWhereConditions>";
            tcCommand += "<ListOfWhereColumns>";
            tcCommand += "<WhereColumn><WhereLeftColumnPrefix>TER</WhereLeftColumnPrefix><WhereLeftColumnName>ID</WhereLeftColumnName> <WhereCondition>=</WhereCondition> <WhereRightColumnPrefix>TOD</WhereRightColumnPrefix> <WhereRightColumnName>TERMINAL_ID</WhereRightColumnName></WhereColumn>";
            tcCommand += "</ListOfWhereColumns>";
            tcCommand += "</WhereCondition>";
            tcCommand += "<OrderClause>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FIELD_INDEX</ColumnName> <Direction>ASC</Direction> </Column> ";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>ROW_NR</ColumnName> <Direction>ASC</Direction> </Column>";
            tcCommand += "</OrderClause>";
            tcCommand += "</DMLInfo></DMLIntegration></TCMessage>";
            tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
            tcCommand = tcCommand.Replace("#ITERATION_NO", glbIterationNo.ToString());
            List<List<string>> terminalRes = new List<List<string>>();

            terminalRes = getSelectResult(tcCommand, 15);
            if (glbStatus.Equals("Success"))
            {
                screenControls.Clear();
                foreach (List<string> row in terminalRes)
                {
                    if (fastCheckLicence(row[idTerminalCode]))
                    {
                        if (row[idFIELD_TYPE].Equals("T") && !imageFound)
                        {
                            if (row[idDATA_TYPE].Equals("B"))
                            {
                                //type button (from ver B9)
                                Button myButton = new System.Windows.Forms.Button();
                                myButton.Location = getPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]), 'B');
                                myButton.Font = getButtonsFont();
                                myButton.Click += new EventHandler(dataButton_Click);
                                myButton.Width = getLabelWidth((row[idSIZE_VAL].Equals("null") || row[idSIZE_VAL].Equals("0")) ? row[idFIELD_DATA].Length : Convert.ToInt32(row[idSIZE_VAL]), Convert.ToInt32(row[idFONT_SIZE]));
                                myButton.Height = getButtonHeight();
                                myButton.Text = row[idFIELD_DATA];

                                if (!row[idCOLOR].Equals("null"))
                                {
                                    int r = Int32.Parse(row[idCOLOR].Substring(0, 3));
                                    int g = Int32.Parse(row[idCOLOR].Substring(4, 3));
                                    int b = Int32.Parse(row[idCOLOR].Substring(8, 3));
                                    myButton.BackColor = Color.FromArgb(r, g, b);
                                }

                                this.Controls.Add(myButton);
                                screenControls.Add(new termControl(myButton, row[idFIELD_DATA].Substring(0, row[idFIELD_DATA].IndexOf(" "))));
                            }
                            else
                            {
                                //type label
                                Label myLabel = new Label();
                                myLabel.Text = row[idFIELD_DATA].Equals("null") ? "" : row[idFIELD_DATA];
                                myLabel.Width = getLabelWidth((row[idSIZE_VAL].Equals("null") || row[idSIZE_VAL].Equals("0")) ? row[idFIELD_DATA].Length : Convert.ToInt32(row[idSIZE_VAL]), Convert.ToInt32(row[idFONT_SIZE]));
                                myLabel.Height = getLabelHeight();
                                myLabel.Location = getPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]), 'L');
                                myLabel.Font = getLabelsFont();
                                if (row[idNEGATIVE].Equals("Y"))
                                {
                                    myLabel.BackColor = Color.MediumBlue;
                                    myLabel.ForeColor = Color.White;
                                }
                                if (!row[idCOLOR].Equals("null"))
                                {
                                    int r = Int32.Parse(row[idCOLOR].Substring(0, 3));
                                    int g = Int32.Parse(row[idCOLOR].Substring(4, 3));
                                    int b = Int32.Parse(row[idCOLOR].Substring(8, 3));
                                    myLabel.BackColor = Color.FromArgb(r, g, b);
                                }
                                if (row[idALIGN].Equals("C"))
                                    myLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                                else if (row[idALIGN].Equals("R"))
                                    myLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
                                else if (row[idALIGN].Equals("L"))
                                    myLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;

                                this.Controls.Add(myLabel);
                                screenControls.Add(new termControl(myLabel));
                            }
                        }
                        else if (row[idFIELD_TYPE].Equals("B") || row[idFIELD_TYPE].Equals("F"))
                        {
                            //barcode or field
                            TextBox myTextBox = new TextBox();
                            myTextBox.AcceptsReturn = true;
                            if (row[idNEGATIVE].Equals("Y"))
                            {
                                myTextBox.BackColor = Color.MediumBlue;
                                myTextBox.ForeColor = Color.White;
                            }
                            else
                            {
                                myTextBox.BackColor = Color.FromArgb(192, 192, 255);
                            }
                            if (!row[idCOLOR].Equals("null"))
                            {
                                int r = Int32.Parse(row[idCOLOR].Substring(0, 3));
                                int g = Int32.Parse(row[idCOLOR].Substring(4, 3));
                                int b = Int32.Parse(row[idCOLOR].Substring(8, 3));
                                myTextBox.BackColor = Color.FromArgb(r, g, b);
                            }
                            if (row[idALIGN].Equals("C"))
                                myTextBox.TextAlign = HorizontalAlignment.Center;
                            else if (row[idALIGN].Equals("R"))
                                myTextBox.TextAlign = HorizontalAlignment.Right;
                            if (!row[idFIELD_DATA].Equals("null"))
                            {
                                myTextBox.Text = row[idFIELD_DATA];
                            }
                            myTextBox.Location = getPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]), 'T');//getTBPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                            myTextBox.Width = getTBWidth(row[idSIZE_VAL].Equals("null") ? row[idFIELD_DATA].Length : Convert.ToInt32(row[idSIZE_VAL]));
                            myTextBox.Height = getTextBoxHeight();
                            myTextBox.KeyPress += new KeyPressEventHandler(myTextBox_KeyPress);
                            myTextBox.Font = getLabelsFont();

                            termControl tControl = new termControl(myTextBox,
                                row[idMIN_VAL].Equals("null") ? 0 : Convert.ToDouble(row[idMIN_VAL]),
                                row[idMAX_VAL].Equals("null") ? 0 : Convert.ToDouble(row[idMAX_VAL]),
                                row[idDATA_TYPE]);

                            tControl.FieldType = row[idFIELD_TYPE];
                            tControl.Required = row[idREQUIRED];
                            if (!row[idDEVICE_ID].Equals("null"))
                            {
                                //SELECT terminal_devices
                                String tcTermDevices = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINAL_DEVICES</TableName> <TableAlias>T</TableAlias> </Table> <Table> <Schema>TERM</Schema> <TableName>DEVICE_TYPES</TableName> <TableAlias>DT</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DEVICE_TYPE</ColumnName> <ColumnAlias>DEVICE_TYPE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>PORT</ColumnName> <ColumnAlias>PORT</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>PORT_TYPE</ColumnName> <ColumnAlias>PORT_TYPE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>HANDSHAKE</ColumnName> <ColumnAlias>HANDSHAKE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>PARITY</ColumnName> <ColumnAlias>PARITY</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>RTS_ENABLE</ColumnName> <ColumnAlias>RTS_ENABLE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>SPEED</ColumnName> <ColumnAlias>SPEED</ColumnAlias> </Column> <Column> <ColumnPrefix>DT</ColumnPrefix> <ColumnName>DELIMITER</ColumnName> <ColumnAlias>DELIMITER</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DATA_BITS</ColumnName> <ColumnAlias>DATA_BITS</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>STOP_BITS</ColumnName> <ColumnAlias>STOP_BITS</ColumnAlias> </Column> </ListOfColumns> ";
                                tcTermDevices += "<WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>T</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> <WhereColumn> <WhereColumnPrefix>T</WhereColumnPrefix> <WhereColumnName>ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#DEVICE_ID</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns> <WhereColumn> <WhereLeftColumnPrefix>T</WhereLeftColumnPrefix> <WhereLeftColumnName>DEVICE_TYPE</WhereLeftColumnName> <WhereCondition>=</WhereCondition> <WhereRightColumnPrefix>DT</WhereRightColumnPrefix> <WhereRightColumnName>TYPE</WhereRightColumnName> </WhereColumn> </ListOfWhereColumns> </WhereCondition> </DMLInfo> </DMLIntegration> </TCMessage>";
                                tcTermDevices = tcTermDevices.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                                tcTermDevices = tcTermDevices.Replace("#DEVICE_ID", row[idDEVICE_ID]);
                                List<List<string>> termDevicesRes = new List<List<string>>();

                                termDevicesRes = getSelectResult(tcTermDevices, 10);
                                if (glbStatus.Equals("Success"))
                                {
                                    //every time is only one!!!
                                    foreach (List<string> device in termDevicesRes)
                                    {
                                        if (!device[devPORT].Equals("null"))
                                        {
                                            SerialPort mySPort = new SerialPort(device[devPORT], Convert.ToInt32(device[devSPEED]));
                                            switch (device[devHANDSHAKE])
                                            {
                                                case "NONE": { mySPort.Handshake = Handshake.None; break; }
                                                case "X": { mySPort.Handshake = Handshake.XOnXOff; break; }
                                                case "RTS": { mySPort.Handshake = Handshake.RequestToSend; break; }
                                                case "RX": { mySPort.Handshake = Handshake.RequestToSendXOnXOff; break; }
                                            }
                                            switch (device[devPARITY])
                                            {
                                                case "NONE": { mySPort.Parity = Parity.None; break; }
                                                case "EVEN": { mySPort.Parity = Parity.Even; break; }
                                                case "ODD": { mySPort.Parity = Parity.Odd; break; }
                                                case "MARK": { mySPort.Parity = Parity.Mark; break; }
                                                case "SPACE": { mySPort.Parity = Parity.Space; break; }
                                            }
                                            switch (device[devRTS_ENABLE])
                                            {
                                                case "TRUE": { mySPort.RtsEnable = true; break; }
                                                case "FALSE": { mySPort.RtsEnable = false; break; }
                                            }
                                            switch (device[devSTOP_BITS])
                                            {
                                                case "NONE": { mySPort.StopBits = StopBits.None; break; }
                                                case "ONE": { mySPort.StopBits = StopBits.One; break; }
                                                case "TWO": { mySPort.StopBits = StopBits.Two; break; }
                                                case "OPF": { mySPort.StopBits = StopBits.OnePointFive; break; }
                                            }
                                            mySPort.DataBits = Convert.ToInt32(device[devDATA_BITS]);
                                            mySPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                                            try
                                            {
                                                mySPort.Open();
                                            }
                                            catch (Exception spExc)
                                            { }
                                            tControl.ComPort = mySPort;
                                            tControl.PortDelimiter = device[devDELIMITER];
                                        }
                                    }
                                }
                            }
                            this.Controls.Add(myTextBox);
                            screenControls.Add(tControl);

                            currTB = tControl;
                            currTbIndex++;
                            cntTbxs++;
                        }
                        else if (row[idFIELD_TYPE].Equals("P"))
                        {
                            //PRINTER
                            if (!printFound)
                            {
                                printFound = true;
                            }

                            printText += row[idFIELD_DATA];
                        }
                        else if (row[idFIELD_TYPE].Equals("I"))
                        {
                            //IMAGE FILE
                            PictureBox myPBox = new PictureBox();
                            Image img;
                            string filePath = "/ResidentFlash/Emul/" + row[idFIELD_DATA];
                            if (File.Exists(filePath))
                                img = new Bitmap(filePath);
                            else
                            {
                                filePath = "/L-Class/Emul/" + row[idFIELD_DATA];
                                if (File.Exists(filePath))
                                {
                                    img = new Bitmap(filePath);
                                }
                                else
                                {

                                    img = new Bitmap(row[idFIELD_DATA]);
                                }
                            }
                            myPBox.Image = img;
                            myPBox.Location = getImagePosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                            myPBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            myPBox.Size = getImgSize();
                            this.Controls.Add(myPBox);
                            screenControls.Add(new termControl(myPBox));
                            imageFound = true;
                        }
                        else if (row[idFIELD_TYPE].StartsWith("G") && row[idALIGN].Equals("T"))
                        {
                            //GRAPH TITLE
                            graphsFound++;
                            //TODO: x, y да се вземат сприамо екрана
                            switch (graphsFound)
                            {
                                case 1:
                                    graph1PB.Location = getGraphPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    graph1PB.Size = getGraphSize(Convert.ToInt32(row[idMIN_VAL]), Convert.ToInt32(row[idMAX_VAL]));
                                    graph1Index = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                    break;
                                case 2:
                                    graph2PB.Location = getGraphPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    graph2PB.Size = getGraphSize(Convert.ToInt32(row[idMIN_VAL]), Convert.ToInt32(row[idMAX_VAL]));
                                    graph2Index = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                    break;
                                case 3:
                                    graph3PB.Location = getGraphPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    graph3PB.Size = getGraphSize(Convert.ToInt32(row[idMIN_VAL]), Convert.ToInt32(row[idMAX_VAL]));
                                    graph3Index = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                    break;
                                case 4:
                                    graph4PB.Location = getGraphPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    graph4PB.Size = getGraphSize(Convert.ToInt32(row[idMIN_VAL]), Convert.ToInt32(row[idMAX_VAL]));
                                    graph4Index = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                    break;
                                case 5:
                                    graph5PB.Location = getGraphPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    graph5PB.Size = getGraphSize(Convert.ToInt32(row[idMIN_VAL]), Convert.ToInt32(row[idMAX_VAL]));
                                    graph5Index = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                    break;
                                case 6:
                                    graph6PB.Location = getGraphPosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    graph6PB.Size = getGraphSize(Convert.ToInt32(row[idMIN_VAL]), Convert.ToInt32(row[idMAX_VAL]));
                                    graph6Index = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                    break;
                            }
                        }
                        else if (row[idFIELD_TYPE].Equals("FD"))
                        {
                            //IMAGE FROM DATABASE
                            try
                            {
                                String imageID = glbTerminalId + "." + glbIterationNo + "." + row[idROW_NR] + "." + row[idPOS_NR];
                                System.Net.WebRequest req = System.Net.WebRequest.Create(glbGraphServlet + glbIDCredentials + imageID);
                                req.Method = "GET";
                                System.Net.WebResponse resp = req.GetResponse();

                                if (row[idDATA_TYPE].Equals("PDF") || row[idDATA_TYPE].Equals("DOC"))
                                {
                                    Stream respStream = resp.GetResponseStream();
                                    FileStream wrtr = new FileStream("c:\\temp\\FD." + row[idDATA_TYPE], FileMode.Create);
                                    byte[] inData = new byte[4096];
                                    int bytesRead = respStream.Read(inData, 0, inData.Length);
                                    while (bytesRead > 0)
                                    {
                                        wrtr.Write(inData, 0, bytesRead);
                                        bytesRead = respStream.Read(inData, 0, inData.Length);
                                    }
                                    respStream.Close();
                                    wrtr.Close();

                                    try
                                    {
                                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                                        psi.FileName = "c:\\temp\\FD." + row[idDATA_TYPE];
                                        System.Diagnostics.Process proc;
                                        this.TopMost = false;
                                        proc = System.Diagnostics.Process.Start(psi);
                                        proc.WaitForExit();
                                        this.TopMost = true;
                                    }
                                    catch (Exception graphExc)
                                    { }
                                }
                                else
                                {
                                    Bitmap graphBMP = new Bitmap(resp.GetResponseStream());
                                    PictureBox myPBox = new PictureBox();
                                    myPBox.Image = graphBMP;
                                    myPBox.Location = getImagePosition(Convert.ToInt32(row[idPOS_NR]), Convert.ToInt32(row[idROW_NR]));
                                    myPBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                    myPBox.Width = Convert.ToInt32(row[idMAX_VAL]);
                                    myPBox.Height = Convert.ToInt32(row[idMIN_VAL]);
                                    myPBox.Click += new EventHandler(imageDB_Click);
                                    myPBox.Show();
                                    this.Controls.Add(myPBox);
                                    screenControls.Add(new termControl(myPBox));
                                }
                                resp.Close();
                            }
                            catch (Exception graphExc)
                            { }
                        }
                        else if (row[idFIELD_TYPE].Equals("FL"))
                        {
                            //FILE LINK
                            try
                            {
                                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                                psi.FileName = row[idFIELD_DATA];
                                System.Diagnostics.Process proc;
                                this.TopMost = false;
                                proc = System.Diagnostics.Process.Start(psi);
                                proc.WaitForExit();
                                this.TopMost = true;
                            }
                            catch (Exception graphExc)
                            { }
                        }
                        else if (row[idFIELD_TYPE].Equals("BG"))
                        {
                            if (!row[idCOLOR].Equals("null"))
                            {
                                int r = Int32.Parse(row[idCOLOR].Substring(0, 3));
                                int g = Int32.Parse(row[idCOLOR].Substring(4, 3));
                                int b = Int32.Parse(row[idCOLOR].Substring(8, 3));
                                this.BackColor = Color.FromArgb(r, g, b);
                            }

                        }
                        else if (row[idFIELD_TYPE].Equals("URL"))
                        {
                            //URL
                            try
                            {
                                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                                psi.FileName = row[idFIELD_DATA];
                                System.Diagnostics.Process proc;
                                this.TopMost = false;
                                proc = System.Diagnostics.Process.Start(psi);
                                proc.WaitForExit();
                                this.TopMost = true;
                            }
                            catch (Exception graphExc)
                            { }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your license is disturbed! The program will close!",
                                     "Disturbed license",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Hand,
                                      MessageBoxDefaultButton.Button1);
                        Application.Exit();
                        return;
                    }
                }
                //END FRONT
                currTB.Control.Focus();

                // print on printer
                if (printFound)
                {
                    //SELECT terminal_devices
                    String tcTermDevices = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINAL_DEVICES</TableName> <TableAlias>T</TableAlias> </Table> <Table> <Schema>TERM</Schema> <TableName>DEVICE_TYPES</TableName> <TableAlias>DT</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DEVICE_TYPE</ColumnName> <ColumnAlias>DEVICE_TYPE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>PORT</ColumnName> <ColumnAlias>PORT</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>PORT_TYPE</ColumnName> <ColumnAlias>PORT_TYPE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>HANDSHAKE</ColumnName> <ColumnAlias>HANDSHAKE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>PARITY</ColumnName> <ColumnAlias>PARITY</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>RTS_ENABLE</ColumnName> <ColumnAlias>RTS_ENABLE</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>SPEED</ColumnName> <ColumnAlias>SPEED</ColumnAlias> </Column> <Column> <ColumnPrefix>DT</ColumnPrefix> <ColumnName>DELIMITER</ColumnName> <ColumnAlias>DELIMITER</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>DATA_BITS</ColumnName> <ColumnAlias>DATA_BITS</ColumnAlias> </Column> <Column> <ColumnPrefix>T</ColumnPrefix> <ColumnName>STOP_BITS</ColumnName> <ColumnAlias>STOP_BITS</ColumnAlias> </Column> </ListOfColumns> ";
                    tcTermDevices += "<WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>T</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> <WhereColumn> <WhereColumnPrefix>T</WhereColumnPrefix> <WhereColumnName>PORT_TYPE</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>OUT</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns> <WhereColumn> <WhereLeftColumnPrefix>T</WhereLeftColumnPrefix> <WhereLeftColumnName>DEVICE_TYPE</WhereLeftColumnName> <WhereCondition>=</WhereCondition> <WhereRightColumnPrefix>DT</WhereRightColumnPrefix> <WhereRightColumnName>TYPE</WhereRightColumnName> </WhereColumn> </ListOfWhereColumns> </WhereCondition> </DMLInfo> </DMLIntegration> </TCMessage>";
                    tcTermDevices = tcTermDevices.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                    List<List<string>> termDevicesRes = new List<List<string>>();

                    termDevicesRes = getSelectResult(tcTermDevices, 10);
                    if (glbStatus.Equals("Success"))
                    {
                        //every time max one!!!
                        foreach (List<string> device in termDevicesRes)
                        {
                            if (!device[devPORT].Equals("null"))
                            {
                                SerialPort mySPort = new SerialPort(device[devPORT], Convert.ToInt32(device[devSPEED]));
                                try
                                {
                                    switch (device[devHANDSHAKE])
                                    {
                                        case "NONE": { mySPort.Handshake = Handshake.None; break; }
                                        case "X": { mySPort.Handshake = Handshake.XOnXOff; break; }
                                        case "RTS": { mySPort.Handshake = Handshake.RequestToSend; break; }
                                        case "RX": { mySPort.Handshake = Handshake.RequestToSendXOnXOff; break; }
                                    }
                                    switch (device[devPARITY])
                                    {
                                        case "NONE": { mySPort.Parity = Parity.None; break; }
                                        case "EVEN": { mySPort.Parity = Parity.Even; break; }
                                        case "ODD": { mySPort.Parity = Parity.Odd; break; }
                                        case "MARK": { mySPort.Parity = Parity.Mark; break; }
                                        case "SPACE": { mySPort.Parity = Parity.Space; break; }
                                    }
                                    switch (device[devRTS_ENABLE])
                                    {
                                        case "TRUE": { mySPort.RtsEnable = true; break; }
                                        case "FALSE": { mySPort.RtsEnable = false; break; }
                                    }
                                    switch (device[devSTOP_BITS])
                                    {
                                        case "NONE": { mySPort.StopBits = StopBits.None; break; }
                                        case "ONE": { mySPort.StopBits = StopBits.One; break; }
                                        case "TWO": { mySPort.StopBits = StopBits.Two; break; }
                                        case "OPF": { mySPort.StopBits = StopBits.OnePointFive; break; }
                                    }
                                    mySPort.DataBits = Convert.ToInt32(device[devDATA_BITS]);
                                    mySPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
                                    mySPort.ReadBufferSize = 4096;
                                    mySPort.WriteBufferSize = 4096;
                                    mySPort.WriteTimeout = 1000;
                                    //mySPort.Encoding = Encoding.GetEncoding("CP866"); ++ not supported on WinCE
                                    mySPort.Encoding = Encoding.Default;
                                    mySPort.Open();
                                    int idx = 0;
                                    while (idx <= printText.Length)
                                    {
                                        String printSubText = printText.Substring(idx, printText.IndexOf("^XZ", idx) + 3 - idx);
                                        idx += printSubText.Length;
                                        //MessageBox.Show(printSubText.Length + "\n" + printSubText);
                                        if (printSubText.Length > printLen)
                                        {
                                            int subIdx = 0;
                                            while (subIdx <= printSubText.Length)
                                            {
                                                try 
                                                {
                                                    String printPart1 = printSubText.Substring(subIdx, printSubText.IndexOf("^FS", printLen + subIdx) + 3);
                                                    mySPort.Write(printPart1 + " \r\n");
                                                    //MessageBox.Show(subIdx + ":" + printPart1.Length + "\n" + printPart1);
                                                    subIdx += printPart1.Length;
                                                }
                                                catch (ArgumentOutOfRangeException e) 
                                                {
                                                    String printPart2 = printSubText.Substring(subIdx, printSubText.Length - subIdx);
                                                    mySPort.Write(printPart2 + " \r\n");
                                                    //MessageBox.Show("EX: " + subIdx + ":" + printPart2.Length + "\n" + printPart2);
                                                    subIdx = printSubText.Length + 1;
                                                }            
                                            }
                                        }
                                        else
                                        {
                                            mySPort.Write(printSubText + "\r\n");
                                        }
                                    }
                                    System.Threading.Thread.Sleep(1000);
                                }
                                catch (Exception portExc)
                                { }
                                finally
                                {
                                    mySPort.Close();
                                    mySPort.Dispose();
                                }
                            }
                        }
                    }                    
                }
            }
        }

        public void nextCommand(String pressedButton)
        {
            if (checkInternetConnection())
            {
                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                ///////////// INSERT all fields and delete from screen //////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                string tcCommand = "==";
                if (pressedButton.Equals("ENT") || pressedButton.Equals("F1") || pressedButton.Equals("F2"))
                {
                    //INSERT terminal_read_data
                    tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"INSERT\"> <DMLIntegration> <DMLInfo> <Schema>TERM</Schema> <TableName>TERMINAL_READ_DATA</TableName> <ListOfRows>";
                    int fieldIndex = 0;
                    foreach (termControl c in screenControls)
                    {
                        if (c.Control is TextBox)
                        {
                            try
                            {
                                c.ComPort.Close();
                            }
                            catch (Exception cpEx)
                            { }
                            TextBox t = (TextBox)c.Control;
                            //check for wrong number
                            if (c.DataType.Equals("N") && t.Text.Length > 0)
                            {
                                try
                                {
                                    double a = double.Parse(t.Text);
                                }
                                catch (Exception dEx)
                                {
                                    t.Text = "*N*";
                                    errMsg.Text = msgWrongNumber;
                                    errMsg.Show();
                                    t.Focus();
                                    currTB = c;
                                    return;
                                }
                            }
                            //check for diapazon value
                            if ((c.MaxVal - c.MinVal) > 0.0 && t.Text.Length > 0)
                            {
                                if (c.MinVal > Convert.ToDouble(t.Text) || Convert.ToDouble(t.Text) > c.MaxVal)
                                {
                                    t.Text = "***";
                                    errMsg.Text = msgOutOfRange;
                                    errMsg.Show();
                                    t.Focus();
                                    currTB = c;
                                    return;
                                }
                            }
                            //check for required field Y
                            if (c.Required.Equals("Y") && t.Text == "")
                            {
                                t.Text = "*";
                                t.Focus();
                                currTB = c;
                                return;
                            }
                            //check for required field 
                            if (c.Required.Equals("N") && t.Text == "" && !requiredPassFirstTime)
                            {
                                requiredPassFirstTime = true;
                                t.Focus();
                                currTB = c;
                                return;
                            }
                            string row = "<ListOfColumns> <Column> <ColumnName>TERMINAL_ID</ColumnName> <ColumnValue>#TERMINAL_ID</ColumnValue> </Column> <Column> <ColumnName>ITERATION_NO</ColumnName> <ColumnValue>#ITERATION_NO</ColumnValue> </Column> <Column> <ColumnName>FIELD_INDEX</ColumnName> <ColumnValue>#FIELD_INDEX</ColumnValue> </Column> <Column> <ColumnName>FIELD_TYPE</ColumnName> <ColumnValue>B</ColumnValue> </Column>  <Column> <ColumnName>FIELD_DATA</ColumnName> <ColumnValue>#FIELD_DATA</ColumnValue> </Column> </ListOfColumns>";
                            row = row.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                            row = row.Replace("#ITERATION_NO", glbIterationNo.ToString());
                            row = row.Replace("#FIELD_INDEX", fieldIndex.ToString());
                            row = row.Replace("#FIELD_DATA", t.Text.Replace("+", "%2B"));
                            fieldIndex++;

                            tcCommand += row;
                        }
                    }

                    tcCommand += "</ListOfRows> </DMLInfo> </DMLIntegration> </TCMessage>";
                    tcCommand = getDMLResult(tcCommand);
                }

                //iztirva gi ot ekran na vtori pass zaradi validaciite
                foreach (termControl c in screenControls)
                {
                    if (c.ComPort != null)
                    {
                        c.ComPort.Dispose();
                    }
                    Controls.Remove(c.Control);
                }
                errMsg.Hide();

                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////// FUNCTION execute_next_command ///////////////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"FUNCTION\"> <DMLIntegration> <DMLInfo> <Schema>TERM</Schema> <UnitName>EXECUTE_NEXT_COMMAND</UnitName> <ResultType>NUMBER</ResultType> <ListOfParameters> <Parameter> <ParameterValue>#TERMINAL_ID</ParameterValue> <ParameterType>IN</ParameterType> </Parameter> <Parameter> <ParameterValue>#ITERATION_NO</ParameterValue> <ParameterType>IN</ParameterType> </Parameter> <Parameter> <ParameterValue>#FNC_KEY</ParameterValue> <ParameterType>IN</ParameterType> </Parameter> <Parameter> <ParameterValue>COMMAND_TYPE</ParameterValue> <ParameterType>OUT</ParameterType> </Parameter> </ListOfParameters> </DMLInfo> </DMLIntegration> </TCMessage>";
                tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                tcCommand = tcCommand.Replace("#ITERATION_NO", glbIterationNo.ToString());
                tcCommand = tcCommand.Replace("#FNC_KEY", pressedButton);

                glbIterationNo = Convert.ToInt32(getDMLResult(tcCommand));

                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////// SELECT icons for command buttons ////////////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                if (deviceType.StartsWith("H"))
                {
                    bmF1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f1_20.png"));
                    bmF2.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f2_20.png"));
                }
                else
                {
                    if (deviceType.Equals("T15"))
                    {
                        bmF1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f1.png"));
                        bmF2.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f2.png"));
                    }
                    else
                    {
                        bmF1.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f1_7inch.png"));
                        bmF2.Image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Emul.imgs.main_f2_7inch.png"));
                    }
                }
                tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo> <ListOfTables> <Table> <Schema>TERM</Schema> <TableName>TERMINAL_NEXT_COMMAND</TableName> <TableAlias>TNC</TableAlias> </Table> </ListOfTables> <ListOfColumns> <Column> <ColumnPrefix>TNC</ColumnPrefix> <ColumnName>FNC_KEY</ColumnName> <ColumnAlias>FNC_KEY</ColumnAlias> </Column> <Column> <ColumnPrefix>TNC</ColumnPrefix> <ColumnName>ICON</ColumnName> <ColumnAlias>ICON</ColumnAlias> </Column> </ListOfColumns> <WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnPrefix>TNC</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> <WhereColumn> <WhereColumnPrefix>TNC.ICON IS NOT NULL AND TNC</WhereColumnPrefix> <WhereColumnName>ITERATION_NO</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#ITERATION_NO</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> <ListOfWhereColumns>AA</ListOfWhereColumns> </WhereCondition> <OrderClause>AA</OrderClause> </DMLInfo> </DMLIntegration> </TCMessage>";
                tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                tcCommand = tcCommand.Replace("#ITERATION_NO", glbIterationNo.ToString());
                List<List<string>> terminalRes = new List<List<string>>();

                terminalRes = getSelectResult(tcCommand, 2);
                if (glbStatus.Equals("Success"))
                {
                    foreach (List<string> row in terminalRes)
                    {
                        if (!row[1].Equals("null"))
                        {
                            Image btnImage;
                            string filePath = "/ResidentFlash/Emul/" + row[1].ToUpper();
                            if (File.Exists(filePath))
                                btnImage = new Bitmap(filePath);
                            else
                            {
                                filePath = "/L-Class/Emul/" + row[1].ToUpper();
                                if (File.Exists(filePath))
                                {
                                    btnImage = new Bitmap(filePath);
                                }
                                else
                                {
                                    btnImage = new Bitmap(row[1].ToUpper());
                                }
                            }
                            switch (row[0])
                            {
                                case "F1":
                                    bmF1.Image = btnImage;
                                    break;
                                case "F2":
                                    bmF2.Image = btnImage;
                                    break;
                            }
                        }
                    }

                    this.Refresh();
                }

                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////////////////// fillTerminalPanel ///////////////////////////
                /////////////////////////////////////////////////////////////////////////
                /////////////////////////////////////////////////////////////////////////
                fillTerminalPanel();
            }
            else
            {
                DialogResult res = new DialogResult();
                res = MessageBox.Show("Do you want to close the program?",
                                                "You don't have Internet connection",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question,
                                                 MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    Application.Exit();
                   
                }
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
        }

      
        private void operDataTimer_Tick(object sender, EventArgs e)
        {
            if (checkInternetConnection())
            {
                if (checkHttpPost(glbDMLService))
                {
                    msgFillTerminalPanel();
                }
                else
                {
                    this.operDataTimer.Enabled = false;
                    DialogResult res = new DialogResult();
                    res = MessageBox.Show("Warring massage function:\nDo you want to close the program?",
                                           "You have a proble with DML service!",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question,
                                                     MessageBoxDefaultButton.Button2);
                    if (res == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        this.operDataTimer.Enabled = true;
                    }
                }
            }
            else
            {
                this.operDataTimer.Enabled = false;
                DialogResult res = new DialogResult();
                res = MessageBox.Show("Warring massage function:\nDo you want to close the program?",
                                       "You don't have Internet connection!",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question,
                                                 MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    this.operDataTimer.Enabled = true;
                }
            }
        }

        private void msgFillTerminalPanel()
        {
            //SELECT terminal_oper_data
            String tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"SELECT\"> <DMLIntegration> <DMLInfo>";
            tcCommand += "<ListOfTables>";
            tcCommand += "<Table> <Schema>TERM</Schema> <TableName>TERMINAL_OPER_DATA</TableName> <TableAlias>TOD</TableAlias> </Table>";
            tcCommand += "<Table> <Schema>TERM</Schema> <TableName>TERMINALS</TableName> <TableAlias>TER</TableAlias> </Table>";
            tcCommand += "</ListOfTables>";
            tcCommand += "<ListOfColumns>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>ROW_NR</ColumnName> <ColumnAlias>ROW_NR</ColumnAlias> </Column>"; 
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>POS_NR</ColumnName> <ColumnAlias>POS_NR</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FIELD_TYPE</ColumnName> <ColumnAlias>FIELD_TYPE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FIELD_DATA</ColumnName> <ColumnAlias>FIELD_DATA</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>DATA_TYPE</ColumnName> <ColumnAlias>DATA_TYPE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>NEGATIVE</ColumnName> <ColumnAlias>NEGATIVE</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FONT_SIZE</ColumnName> <ColumnAlias>FONT_SIZE</ColumnAlias> </Column> ";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>SIZE_VAL</ColumnName> <ColumnAlias>SIZE_VAL</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>DEVICE_ID</ColumnName> <ColumnAlias>DEVICE_ID</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>MIN_VAL</ColumnName> <ColumnAlias>MIN_VAL</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>MAX_VAL</ColumnName> <ColumnAlias>MAX_VAL</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>ALIGN</ColumnName> <ColumnAlias>ALIGN</ColumnAlias> </Column>"; 
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>REQUIRED</ColumnName> <ColumnAlias>REQUIRED</ColumnAlias> </Column> ";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>COLOR</ColumnName> <ColumnAlias>COLOR</ColumnAlias> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TER</ColumnPrefix> <ColumnName>TERMINAL_CODE</ColumnName> <ColumnAlias>TERMINAL_CODE</ColumnAlias> </Column>";
            tcCommand += "</ListOfColumns>";
            tcCommand += "<WhereCondition>";
            tcCommand += "<ListOfWhereConditions>";
            tcCommand += "<WhereColumn> <WhereColumnPrefix>TOD</WhereColumnPrefix> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn>";
            tcCommand += "<WhereColumn> <WhereColumnPrefix>TOD</WhereColumnPrefix> <WhereColumnName>ITERATION_NO</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#ITERATION_NO</WhereColumnValue> </WhereColumn>";
            tcCommand += "<WhereColumn> <WhereColumnPrefix>TOD</WhereColumnPrefix> <WhereColumnName>URGENT</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>Y</WhereColumnValue> </WhereColumn>";
            tcCommand += "</ListOfWhereConditions>";
            tcCommand += "<ListOfWhereColumns>";
            tcCommand += "<WhereColumn><WhereLeftColumnPrefix>TER</WhereLeftColumnPrefix><WhereLeftColumnName>ID</WhereLeftColumnName> <WhereCondition>=</WhereCondition> <WhereRightColumnPrefix>TOD</WhereRightColumnPrefix> <WhereRightColumnName>TERMINAL_ID</WhereRightColumnName></WhereColumn>";
            tcCommand += "</ListOfWhereColumns>";
            tcCommand += "</WhereCondition>";
            tcCommand += "<OrderClause>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>FIELD_INDEX</ColumnName> <Direction>ASC</Direction> </Column>";
            tcCommand += "<Column> <ColumnPrefix>TOD</ColumnPrefix> <ColumnName>ROW_NR</ColumnName> <Direction>ASC</Direction> </Column>";
            tcCommand += "</OrderClause>";
            tcCommand += "</DMLInfo></DMLIntegration></TCMessage>";
            tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
            tcCommand = tcCommand.Replace("#ITERATION_NO", glbIterationNo.ToString());

            //get results 
            List<List<string>> terminalRes = new List<List<string>>();
            terminalRes = getSelectResult(tcCommand, 15);

            if (glbStatus.Equals("Success"))
            {
                if (terminalRes.Count > 0)
                {   //show message
                    String msgText = "";
                    foreach (List<string> row in terminalRes)
                    {
                        if (fastCheckLicence(row[idTerminalCode]))
                        {
                            if (row[idFIELD_TYPE].Equals("T"))
                            {
                                msgText = msgText + row[idFIELD_DATA] + '\n';
                            }
                        }
                        else 
                        {
                            MessageBox.Show("Your license is disturbed! The program will close!",
                                     "Disturbed license",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Hand,
                                      MessageBoxDefaultButton.Button1);
                            Application.Exit();
                            return; 
                        }
                    }
                    tcCommand = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <TCMessage MessageId=\"Dummy\" IntObjectName=\"DML Integration\" MessageType=\"UPDATE\"> <DMLIntegration> <DMLInfo> <Schema>TERM</Schema> <TableName>TERMINAL_OPER_DATA</TableName> ";
                    tcCommand += "<ListOfRows> <ListOfColumns> <Column> <ColumnName>URGENT</ColumnName><ColumnValue>N</ColumnValue></Column></ListOfColumns></ListOfRows> ";
                    tcCommand += "<WhereCondition> <ListOfWhereConditions> <WhereColumn> <WhereColumnName>TERMINAL_ID</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#TERMINAL_ID</WhereColumnValue> </WhereColumn> <WhereColumn> <WhereColumnName>ITERATION_NO</WhereColumnName> <WhereCondition>=</WhereCondition> <WhereColumnValue>#ITERATION_NO</WhereColumnValue> </WhereColumn> </ListOfWhereConditions> </WhereCondition></DMLInfo></DMLIntegration></TCMessage>";
                    tcCommand = tcCommand.Replace("#TERMINAL_ID", glbTerminalId.ToString());
                    tcCommand = tcCommand.Replace("#ITERATION_NO", glbIterationNo.ToString());
                    tcCommand = getDMLResult(tcCommand);
                    
                    MessageBox.Show(msgText);

                }
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            if (checkInternetConnection())
            {
                if (checkHttpPost(glbDMLService))
                {
                    this.nextCommand("R");
                }
                else
                {
                    this.refreshTimer.Enabled = false;
                    DialogResult res = new DialogResult();
                    res = MessageBox.Show("Refresh screen function:\nDo you want to close the program?",
                                          "You have a problem with DML service!",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question,
                                                     MessageBoxDefaultButton.Button2);
                    if (res == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        this.refreshTimer.Enabled = true;
                    }
                }

            }
            else
            {
                this.refreshTimer.Enabled = false;
                DialogResult res = new DialogResult();
                res = MessageBox.Show("Refresh screen function:\nDo you want to close the program?",
                                       "You don't have Internet connection!",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question,
                                                 MessageBoxDefaultButton.Button2);
                if (res == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                {
                    this.refreshTimer.Enabled = true;
                }
            }
        }
       
    }
}
