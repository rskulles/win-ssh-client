using System;
using System.Diagnostics;
using System.IO;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Skully.SshClient
{
    public class SshConnectionInformation
    {
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Client : IDisposable
    {
        //Unmanaged Resource
        private Process _sshProcess;

        public Client()
        {
            if (!CheckFeatureEnabled())
            {
                System.Console.WriteLine("Telnet Client not found. Enabling");
                EnableTelnetClient();
            }
            else
            {
                System.Console.WriteLine("Found Telnet Client!");
            }
        }


        public  void Connect(SshConnectionInformation connectionInformation)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fullPath = Path.Combine(location, "plink.exe");
            _sshProcess = CreateProcess(fullPath,
                $@"-ssh -pw 3e4r5t6y#E$R%T^Y {connectionInformation.UserName}@{connectionInformation.IpAddress}");
            _sshProcess.ErrorDataReceived += SshProcessOnDataReceived;
            _sshProcess.OutputDataReceived += SshProcessOnDataReceived;
            _sshProcess.Start();
            _sshProcess.BeginErrorReadLine();
            _sshProcess.BeginOutputReadLine();
        }

        public void Send()
        {
            _sshProcess.StandardInput.Write("/home/gpp/bin/test.sh\r\n");
        }

        private void SshProcessOnDataReceived(object sender, DataReceivedEventArgs e)
        {
            System.Console.WriteLine(e.Data);
            var needsToConfirm = e.Data.Contains("If you trust this host");
            if (needsToConfirm)
            {
                _sshProcess.StandardInput.WriteLine("y");
            }
        }


        private void EnableTelnetClient()
        {
            using (var process = CreateProcess("dism.exe", "/online /enable-feature /featurename:TelnetClient"))
            {
                process.Start();
                using (var reader = process.StandardOutput)
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        System.Console.WriteLine(line);
                    }
                }
            }
        }

        private Process CreateProcess(string name, string arguments)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = name;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.
            return process;
        }

        private bool CheckFeatureEnabled()
        {
            var featureEnabled = false;
            using (var process = CreateProcess("dism.exe", "/online /get-features"))
            {
                process.Start();
                using (var reader = process.StandardOutput)
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (!(line?.Contains("TelnetClient") ?? false)) continue;

                        var enabledString = reader.ReadLine();
                        if (enabledString?.Contains("State : Enabled") ?? false)
                        {
                            featureEnabled = true;
                        }
                    }
                }
            }

            return featureEnabled;
        }

        private void ReleaseUnmanagedResources()
        {
            if (!_sshProcess.HasExited)
            {
                _sshProcess?.Kill();
            }

            _sshProcess?.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Client()
        {
            ReleaseUnmanagedResources();
        }
    }
}