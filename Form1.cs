using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace qsign
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

        }
        private void label1_Click(object sender, EventArgs e)
        { }
        private void write_config(string config_path)
        {
            string json = File.ReadAllText(config_path);
            JObject jo = new JObject();
            try
            {
                jo = (JObject)JsonConvert.DeserializeObject(json);
            }
            catch
            {
                MessageBox.Show("读取config.json失败，请检查相关配置", "读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            jo["server"]["port"] = Convert.ToInt32(textBox1.Text);
            jo["key"] = textBox3.Text;
            jo["server"]["host"] = textBox2.Text;
            jo["auto_register"] = checkBox1.Checked;
            jo["unidbg"]["dynarmic"] = checkBox2.Checked;
            jo["unidbg"]["unicorn"] = checkBox3.Checked;
            jo["unidbg"]["debug"] = checkBox4.Checked;
            string json1 = jo.ToString();
            File.WriteAllText(config_path, json1);


        }
        private string get_target_id(int port)
        {
            return Tool.PortToPid(port);


        }
        private void check()
        {
            while (flag)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:" + port.ToString());
                    request.Method = "GET";
                    HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                    if (((int)httpWebResponse.StatusCode) == 200)
                    {
                        checknum += 1;
                        label5.Invoke((Action)(() =>
                        {
                            label5.Text = "正在运行";
                            DisplayResourceUsage(port);
                            label5.ForeColor = Color.Green;
                            if (checknum == 1)
                            {
                                Add_log(DateTime.Now.ToString("HH:mm:ss:fff") + " [qsign]在http://127.0.0.1:" + port.ToString() + "上运行");
                                if (MessageBox.Show("http://127.0.0.1:" + port.ToString() + "/sign?key=" + key + "\n点击确定复制Sign地址", "启动成功", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                                {
                                    Clipboard.SetDataObject("http://127.0.0.1:" + port.ToString() + "/sign?key=" + key);
                                }
                            }
                        }));
                    }
                    else
                    {
                        label5.Invoke((Action)(() =>
                        {
                            label5.Text = "状态异常 " + httpWebResponse.StatusCode.ToString();
                            label7.Text = "内存使用量: 0 MB   CPU使用率: 0%";
                            label5.ForeColor = Color.Red;
                        }));
                    }
                    httpWebResponse.Close();
                }
                catch
                {
                    label5.Invoke((Action)(() =>
                    {
                        label5.Text = "状态异常";
                        label7.Text = "内存使用量: 0 MB   CPU使用率: 0%";
                        label5.ForeColor = Color.Red;
                    }));
                }
                try
                {
                    Thread.Sleep(1000);
                }
                catch
                { }
            }
        }
        public static bool ValidateIPAddress(string ipAddress)
        {
            string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(ipAddress);
            return match.Success;
        }
        Thread newThread;
        Thread CheckThread;
        string verison;
        int port;
        string host;
        string key;
        bool flag = false;
        int checknum = 0;
        bool run_state = false;
        [Obsolete]
        private void button1_Click(object sender, EventArgs e)
        {
            if (!run_state)
            {
                string currentDirectory = Environment.CurrentDirectory;
                bool exist = File.Exists(currentDirectory + "\\bin\\unidbg-fetch-qsign.bat");
                if (exist == false)
                {
                    MessageBox.Show("启动脚本不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string path = currentDirectory + "\\txlib";
                string[] folders = Directory.GetDirectories(path);
                List<string> versions = new List<string>();
                foreach (string folder in folders)
                {
                    versions.Add(folder.Replace(currentDirectory + "\\txlib\\", ""));
                    System.Diagnostics.Debug.WriteLine(folder.Replace(currentDirectory + "\\txlib\\", ""));
                }
                if (!versions.Contains<string>(comboBox1.Text))
                {
                    MessageBox.Show("版本不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (textBox1.Text == "" || textBox3.Text == "" || textBox2.Text == "")
                {
                    MessageBox.Show("请输入相应内容", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {
                    port = Convert.ToInt32(textBox1.Text);
                }
                catch
                {
                    MessageBox.Show("端口错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (port < 0 || port > 65535)
                {
                    MessageBox.Show("端口错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (get_target_id(port) != "")
                {
                    MessageBox.Show("端口被占用，请更换端口号再试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!ValidateIPAddress(textBox2.Text))
                {
                    MessageBox.Show("IP地址不合法", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int[] banned_port = {
                    1, 7, 9, 11, 13, 15, 17, 19, 20, 21, 22, 23, 25, 37, 42, 43, 53, 77, 79, 87, 95, 101, 102, 103, 104, 109, 110, 111, 113, 115, 117, 119, 123, 135, 139, 143, 179, 389, 465, 512, 513, 514, 515, 526, 530, 531, 532, 540, 556, 563, 587, 601, 636, 993, 995, 2049, 3659, 4045, 6000, 6665, 6666, 6667, 6668, 6669
                };
                if (banned_port.Contains<int>(port))
                {
                    MessageBox.Show("非安全端口，请更换端口号再试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                verison = comboBox1.Text;
                write_config(currentDirectory + "\\txlib\\" + comboBox1.Text + "\\config.json");
                flag = true;
                checknum = 0;
                newThread = new Thread(new ThreadStart(NewThread));
                CheckThread = new Thread(new ThreadStart(check));
                run_state = true;
                button1.Text = "停止";
                newThread.IsBackground = true;
                newThread.Start();
                CheckThread.IsBackground = true;
                CheckThread.Start();
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                textBox3.Enabled = false;
                textBox2.Enabled = false;
                checkBox1.Enabled = false;
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;

            }
            else
            {
                flag = false;
                if (newThread.IsAlive)
                {
                    string targetPid = get_target_id(port);
                    if (targetPid != "")
                    {
                        System.Diagnostics.Debug.WriteLine(targetPid);
                        var fileName = "cmd";
                        var arguments = "/c taskkill /F /PID " + targetPid;
                        var processStartInfo = new ProcessStartInfo()
                        {
                            FileName = fileName,
                            Arguments = arguments,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        var process1 = Process.Start(processStartInfo);
                        process1.WaitForExit();
                        while (newThread.IsAlive)
                        {
                            Thread.Sleep(100);
                        }
                    }
                    CheckThread.Interrupt();
                    button1.Text = "启动";
                    run_state = false;
                    label5.Text = "未启动";
                    label7.Text = "内存使用量: 0 MB   CPU使用率: 0%";
                    label5.ForeColor = Color.Black;
                    Add_log(DateTime.Now.ToString("HH:mm:ss:fff") + " [qsign]Server已停止");
                    comboBox1.Enabled = true;
                    textBox1.Enabled = true;
                    textBox3.Enabled = true;
                    textBox2.Enabled = true;
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                    checkBox3.Enabled = true;
                    checkBox4.Enabled = true;
                }
            }
        }
        private void NewThread()
        {
            RunCmd();
        }
        private void RunCmd()
        {
            string currentDirectory = Environment.CurrentDirectory;
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    richTextBox1.Invoke((Action)(() =>
                    {
                        Add_log(args.Data + "\n");
                    }
                    ));

                }
            };
            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    richTextBox1.Invoke((Action)(() =>
                    {
                        Add_log(args.Data + "\n");
                    }
                    ));

                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.StandardInput.WriteLine("call bin\\unidbg-fetch-qsign.bat --basePath=txlib\\" + verison);
            while (flag)
            {
                Thread.Sleep(10);
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {


        }
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory;
            string config_path = currentDirectory + "\\txlib\\" + comboBox1.Text + "\\config.json";
            if (!File.Exists(config_path))
            {
                MessageBox.Show("...");
                return;
            }
            string json = File.ReadAllText(config_path);
            JObject jo = new JObject();
            try
            {
                jo = (JObject)JsonConvert.DeserializeObject(json);
                if (jo == new JObject()) { return; }
                port = (int)jo["server"]["port"];
                key = (string)jo["key"];
                host = (string)jo["server"]["host"];
                textBox1.Text = port.ToString();
                textBox3.Text = key;
                textBox2.Text = host;
                checkBox1.Checked = (bool)jo["auto_register"];
                checkBox2.Checked = (bool)jo["unidbg"]["dynarmic"];
                checkBox3.Checked = (bool)jo["unidbg"]["unicorn"];
                checkBox4.Checked = (bool)jo["unidbg"]["debug"];
            }
            catch
            {
                MessageBox.Show("读取config.json失败，请检查相关配置", "读取失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }




        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        { }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (flag)
            {
                if (MessageBox.Show("Server仍在运行，是否关闭窗口", "是否关闭", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    flag = false;
                    if (newThread.IsAlive)
                    {
                        string targetPid = get_target_id(port);
                        if (targetPid != "")
                        {
                            System.Diagnostics.Debug.WriteLine(targetPid);
                            var fileName = "cmd";
                            var arguments = "/c taskkill /F /PID " + targetPid;
                            // 创建进程启动信息
                            var processStartInfo = new ProcessStartInfo()
                            {
                                FileName = fileName,
                                Arguments = arguments,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true,
                                UseShellExecute = false
                            };
                            // 执行命令
                            var process1 = Process.Start(processStartInfo);
                            process1.WaitForExit();
                            while (newThread.IsAlive)
                            {
                                Thread.Sleep(100);
                            }
                        }
                        CheckThread.Interrupt();
                        run_state = false;
                        button1.Text = "启动";
                        label5.Text = "未启动";
                        label5.ForeColor = Color.Black;

                        Add_log(DateTime.Now.ToString("HH:mm:ss:fff") + " [qsign]Server已停止");
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://github.com/fuqiuluo/unidbg-fetch-qsign",
                UseShellExecute = true
            });
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://github.com/CikeyQi/unidbg-fetch-qsign-gui",
                UseShellExecute = true
            });
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://jq.qq.com/?_wv=1027&k=FZUabhdf",
                UseShellExecute = true
            });
        }
        int lenght;
        private void Add_log(string logs)
        {

            string[] logss = logs.Split(new string[] { "\n" }, StringSplitOptions.None);
            if (logs == null || logs == "" || logs.Length == 0) { return; }
            foreach (string log in logss)
            {
                lenght = richTextBox1.Text.Length;
                richTextBox1.AppendText(log + "\n");
                int currentLength = richTextBox1.Lines.Length;
                //int lineFirstCharIndex = richTextBox1.GetFirstCharIndexFromLine(currentLength);
                //if (lineFirstCharIndex == -1) { return; }
                richTextBox1.Select(lenght, (log + "\n").Length);
                if (log.Contains("INFO"))
                {
                    richTextBox1.SelectionColor = Color.DeepSkyBlue;

                    return;
                }
                if (log.Contains("DEBUG"))
                {
                    richTextBox1.SelectionColor = Color.White;
                    return;
                }
                if (log.Contains("WARNING"))
                {
                    richTextBox1.SelectionColor = Color.FromArgb(255, 215, 0);
                    return;
                }

                if (log.Contains("ERROR") || log.Contains("Exception"))
                {
                    richTextBox1.SelectionColor = Color.Red;
                    return;
                }
                if (log.Contains("[qsign]"))
                {
                    richTextBox1.SelectionColor = Color.FromArgb(255, 215, 0);
                    return;
                }
                else
                {
                    richTextBox1.SelectionColor = Color.Black;
                    return;
                }
            }

        }
        private void DrawFont()
        {
            System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();
            pfc.AddFontFile("PingFang.ttf");
            Font myFont = new Font(pfc.Families[0], 12, FontStyle.Regular);
            ///this.Font = myFont;
            textBox1.Font = myFont;
            textBox2.Font = myFont;
            textBox3.Font = myFont;
            comboBox1.Font = myFont;
            checkBox1.Font = myFont;
            checkBox2.Font = myFont;
            checkBox3.Font = myFont;
            checkBox4.Font = myFont;
            label1.Font = myFont;
            label2.Font = myFont;
            label3.Font = myFont;

            Font litemyFont = new Font(pfc.Families[0], 9, FontStyle.Regular);
            richTextBox1.Font = litemyFont;
            linkLabel1.Font = litemyFont;
            linkLabel2.Font = litemyFont;
            linkLabel3.Font = litemyFont;
            label7.Font = litemyFont;
            contextMenuStrip1.Font = litemyFont;

            Font titlemyFont = new Font(pfc.Families[0], 20, FontStyle.Regular);
            label5.Font = titlemyFont;

            Font urlmyFont = new Font(pfc.Families[0], 12, FontStyle.Regular);
            label6.Font = urlmyFont;
            label4.Font = urlmyFont;

        }
        private void Main_Load_1(object sender, EventArgs e)
        {
            DrawFont();
            notifyIcon1.Icon = Icon;
            notifyIcon1.Text = "Windows下免安装环境一键启动qsign签名服务";
            string currentDirectory = Environment.CurrentDirectory;
            string path = currentDirectory + "\\txlib";
            string[] folders = Directory.GetDirectories(path);
#pragma warning disable CS8602 // 解引用可能出现空引用。
            System.Diagnostics.Debug.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
#pragma warning restore CS8602 // 解引用可能出现空引用。
            Text = Text.Replace("%v", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            foreach (string folder in folders)
            {
                System.Diagnostics.Debug.WriteLine(folder);
                comboBox1.Items.Add(folder.Replace(currentDirectory + "\\txlib\\", ""));
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length; //Set the current caret position at the end
            richTextBox1.ScrollToCaret(); //Now scroll it automatically
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = e.LinkText,
                UseShellExecute = true
            });
        }

        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Close();
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)//当窗体设置值为最小化时
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }

            else
            {
                notifyIcon1.Visible = false;//否则该控件不可见
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void Main_MinimumSizeChanged(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
            this.ShowInTaskbar = false;
        }

        private void DisplayResourceUsage(int port)
        {
            int targetPID;
            Process targetProcess;

            // Get the PID of the target process
            string targetPIDString = get_target_id(port);
            if (!int.TryParse(targetPIDString, out targetPID))
            {
                targetPID = -1; // Set to -1 if PID cannot be obtained
            }

            // Get the process instance based on PID
            try
            {
                targetProcess = targetPID != -1 ? Process.GetProcessById(targetPID) : null;
            }
            catch (ArgumentException)
            {
                targetProcess = null; // Set process instance to null if PID is invalid
            }

            // Get memory and CPU information
            double memoryUsageMB = targetProcess != null ? targetProcess.WorkingSet64 / (1024.0 * 1024.0) : 0; // Memory usage in MB
            double cpuUsagePercentage = targetProcess != null ? (targetProcess.TotalProcessorTime.TotalMilliseconds /
                                   (Environment.ProcessorCount * 1000)) * 100 : 0; // CPU usage in percentage

            // Update the information in the Label
            label7.Invoke((Action)(() =>
            {
                label7.Text = string.Format("内存使用量: {0:F2} MB   CPU使用率: {1:F2}%", memoryUsageMB, cpuUsagePercentage);
            }));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}