using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
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
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            jo["server"]["port"] = Convert.ToInt32(textBox1.Text);
            jo["key"] = textBox3.Text;
            string json1 = jo.ToString();
            File.WriteAllText(config_path, json1);
        }
        private string get_target_id(int port)
        {
            string output;
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd ";
                process.StartInfo.Arguments = $"/c netstat -ano | findstr \"{port}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                output = process.StandardOutput.ReadToEnd();
            }
            System.Diagnostics.Debug.WriteLine(output);
            string[] lines = output.Split("\n");
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 4)
                {
                    result.Add(new Dictionary<string, string>()
                    {
                        {
                            "pid", parts[4]
                        },
                        {
                            "address", parts[1]
                        },
                        {
                            "state", parts[3]
                        }
                    });
                }
            }
            string targetPid = "";
            foreach (var item in result)
            {
                if (item["state"] == "LISTENING")
                {
                    targetPid = item["pid"];
                    break;
                }
            }
            return targetPid;
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
                            label5.ForeColor = Color.Green;
                            if (checknum == 1)
                            {
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
        Thread newThread;
        Thread CheckThread;
        string verison;
        int port;
        string key;
        bool flag = false;
        int checknum = 0;
        [Obsolete]
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "启动")
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
                if (textBox1.Text == "" || textBox3.Text == "")
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
                if (get_target_id(port) != "")
                {
                    MessageBox.Show("端口被占用，请更换端口号再试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                button1.Text = "停止";
                newThread.IsBackground = true;
                newThread.Start();
                CheckThread.IsBackground = true;
                CheckThread.Start();
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
                    label5.Text = "未启动";
                    label5.ForeColor = Color.Black;
                    textBox2.AppendText("\nServer已停止");
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
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "cmd.exe";
            process.Start();
            process.StandardInput.WriteLine("call bin\\unidbg-fetch-qsign.bat --basePath=txlib\\" + verison);
            while (!process.StandardOutput.EndOfStream & flag)
            {
                string t = "";
                try
                {
                    t = process.StandardOutput.ReadLine();
                }
                catch
                {
                    continue;
                }
                finally
                {
                    textBox2.Invoke((Action)(() =>
                    {
                        textBox2.AppendText(t + "\n");
                    }));
                }
            }
            process.Kill();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            string currentDirectory = Environment.CurrentDirectory;
            string path = currentDirectory + "\\txlib";
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                comboBox1.Items.Add(folder.Replace(currentDirectory + "\\txlib\\", ""));
            }
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
            JObject jo = (JObject)JsonConvert.DeserializeObject(json);
            port = (int)jo["server"]["port"];
            key = (string)jo["key"];
            textBox1.Text = port.ToString();
            textBox3.Text = key;
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
                        button1.Text = "启动";
                        label5.Text = "未启动";
                        label5.ForeColor = Color.Black;
                        textBox2.AppendText("\nServer已停止");
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
    }
}