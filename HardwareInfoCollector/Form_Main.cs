using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using Microsoft.Management.Infrastructure;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace HardwareInfoCollector
{
    public partial class Form_Main : Form
    {
        public string DeviceConfigurationString = "";
        public bool Failed = false;
        public string SplittingString = " --- ";

        public string HeaderString = "PC Name;OS;User Name;Manufacturer;Model;Is in Domain;Domain Name;CPU Model;CPU Core Count;Total Threads;CPU Clock Speed;CPU Architecture;RAM;RAM Speed;RAM Type;Motherboard Manufacturer;Motherboard Model;Motherboard Name;GPUs;GPU vRAMs;Network Adapters;MAC Addresses;IP Addresses;DHCP Statuses;Drive 0;Drive 1;Drive 2;Drive 3;Drive 4;Drive 5;Drive 6";

        public Form_Main()
        {
            InitializeComponent();
        }

        private void button_CollectAndSave_Click(object sender, EventArgs e)
        {
            if (textBox_Name.Text.Length > 2)
            {
                Failed = false;
                button_CollectAndSave.Enabled = false;

                collectInfo();

                writeInfo();
                if (!Failed)
                {
                    MessageBox.Show("Successfully wrote to file!");
                }

                button_CollectAndSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please enter an Identifier, 3 characters or more");
            }

        }

        private Dictionary<string, string> getInfo(string WMIClass)
        {
            Dictionary<string,string> properties = new Dictionary<string, string>();

            ManagementClass mc = new ManagementClass(WMIClass);
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                foreach (var item in mo.Properties)
                {
                    if (item.Value != null)
                    {
                        if (properties.ContainsKey(item.Name))
                        {
                            properties[item.Name] += SplittingString + item.Value.ToString();
                        }
                        else
                        {
                            properties.Add(item.Name, item.Value.ToString());
                        }
                    }
                }
            }

            return properties;
        }

        private string[] getNetworkingInfo()
        {
            List<string> adapters = new List<string>();
            List<string> macs = new List<string>();
            List<string> ips = new List<string>();
            List<string> dhcps = new List<string>();


            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

                foreach (NetworkInterface adapter in nics)
                {
                    string adapterName = "N/A", adapterIP = "N/A", macAddress = "N/A", dhcpStatus = "N/A";

                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        adapterName = (adapter.Name);
                        foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                adapterIP = (ip.Address.ToString());
                            }
                        }

                        macAddress = (adapter.GetPhysicalAddress().ToString());
                        dhcpStatus = (adapter.GetIPProperties().GetIPv4Properties().IsDhcpEnabled.ToString());

                        adapters.Add(adapterName);
                        macs.Add(macAddress);
                        ips.Add(adapterIP);
                        dhcps.Add(dhcpStatus);
                    }
                }
            }
            catch (Exception)
            {

            }

            return new string[] { string.Join(SplittingString, adapters), string.Join(SplittingString, macs), string.Join(SplittingString, ips), string.Join(SplittingString, dhcps) };

        }

        private void collectInfo()
        {
            try
            {
                Dictionary<string, string> currentInfo = null;
                string newLine = System.Environment.NewLine;

                //
                // General Info
                //
                currentInfo = getInfo("Win32_ComputerSystem");
                string name = currentInfo["Name"];
                string manufacturer = currentInfo["Manufacturer"];
                string model = currentInfo["Model"];
                string domainStatus = currentInfo["PartOfDomain"];
                string domain = currentInfo["Domain"];
                string ram = Math.Round(double.Parse(currentInfo["TotalPhysicalMemory"]) / (1024 * 1024 * 1024), 2).ToString();

                string cpuCount = currentInfo["NumberOfProcessors"];
                string totalThreads = currentInfo["NumberOfLogicalProcessors"];
                string architecture = currentInfo["SystemType"];

                //
                // CPU
                //
                currentInfo = getInfo("Win32_Processor");

                string cpuName = currentInfo["Name"];
                string cores = currentInfo["NumberOfCores"];
                string clockSpeed = currentInfo["MaxClockSpeed"];


                //
                // RAM
                //
                currentInfo = getInfo("Win32_PhysicalMemory");

                string ramSpeed = currentInfo["Speed"];
                string ramType = currentInfo["MemoryType"].Split(new string[] { SplittingString }, StringSplitOptions.RemoveEmptyEntries)[0];
                switch (ramType)
                {
                    case ("20"): { ramType = "DDR"; break; };
                    case ("21"): { ramType = "DDR2"; break; };
                    case ("22"): { ramType = "FB-DIMM"; break; };
                    case ("24"): { ramType = "DDR3"; break; };
                    case ("25"): { ramType = "FBD2"; break; };
                    case ("26"): { ramType = "DDR4"; break; };
                    case ("27"): { ramType = "LP DDR"; break; };
                    case ("28"): { ramType = "LP DDR2"; break; };
                    case ("29"): { ramType = "LP DDR3"; break; };
                    case ("30"): { ramType = "LP DDR4"; break; };
                    default: { ramType = "UNKNOWN:" + ramType; break; }
                }

                currentInfo = getInfo("Win32_BaseBoard");
                string motherboardManufacturer = currentInfo["Manufacturer"];
                string motherboardName = currentInfo["Name"];
                string motherboardModel = currentInfo["Product"];

                //
                // Storage
                //
                List<string> storageSizes = new List<string>();
                string storageSize = "";

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.IsReady == true)
                    {
                        storageSizes.Add(d.Name + " Free: " + Math.Round((d.TotalFreeSpace / (1024.0 * 1024 * 1024)), 2) + "/" + Math.Round((d.TotalSize / (1024.0 *1024*1024)),2) + " GB");
                    }
                }
                storageSize = string.Join(";", storageSizes);

                //
                // GPU
                //
                currentInfo = getInfo("Win32_VideoController");

                string gpuName = currentInfo["Name"];
                string[] vrams = currentInfo["AdapterRAM"].Split(new string[] { SplittingString }, StringSplitOptions.None);
                string vram = Math.Round((double.Parse(vrams[0]) / (1024 * 1024 * 1024)), 2).ToString();
                for (int i = 1; i < vrams.Length; i++)
                {
                    vram += Math.Round((double.Parse(vrams[i]) / (1024 * 1024 * 1024)), 2).ToString();
                }
                string vramType = currentInfo["VideoMemoryType"];
                switch (vramType)
                {
                    case ("3"): { vramType = "VRAM"; break; };
                    case ("4"): { vramType = "DRAM"; break; };
                    case ("5"): { vramType = "SRAM"; break; };
                    case ("8"): { vramType = "Burst Synchronous DRAM"; break; };
                    case ("12"): { vramType = "SDRAM"; break; };
                    case ("13"): { vramType = "SGRAM"; break; };;
                    default: { vramType = "UNKNOWN:" + vramType; break; }
                }
                string gpuProcessor = currentInfo["VideoProcessor"];
                
                //
                // Networking
                //
                string[] macAndIP = getNetworkingInfo();
                string networkAdapters = macAndIP[0];
                string macAddress = macAndIP[1];
                string ipAddress = macAndIP[2];
                string dhcps = macAndIP[3];

                //
                // Prepare string
                //
                DeviceConfigurationString =
                    textBox_Name.Text + ";" + Environment.OSVersion.ToString() + ";"
                    + name + ";" + manufacturer + ";" + model + ";" + domainStatus + ";" + domain + ";" //Manufacturer and User details
                    + cpuName + ";" + cores + ";" + totalThreads + ";" + clockSpeed + ";" + architecture + ";" //CPU details
                    + ram + ";" + ramSpeed + ";" + ramType + ";" //RAM detail
                    + motherboardManufacturer + ";" + motherboardModel + ";" + motherboardName + ";" // Motherboard detail
                    + gpuName + ";" + vram + ";" //GPU detail
                    + networkAdapters + ";" + macAddress + ";" + ipAddress + ";" + dhcps + ";" //Networking Details
                    + storageSize + ";" // HDD Storage detail
                    ;
            }
            catch (Exception ex)
            {
                Failed = true;
                MessageBox.Show("ERROR: " + ex.Message);
                throw;
            }
        }

        private void writeInfo()
        {

            string path = Directory.GetCurrentDirectory() + "\\data.csv";
            try
            {
                if (File.Exists(path))
                {
                    // Append to file
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(DeviceConfigurationString);
                    }
                }
                else
                {
                    // Create a file
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(HeaderString);
                        sw.WriteLine(DeviceConfigurationString);
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                Failed = true;
                MessageBox.Show("ERROR: " + ex.Message);
                throw;
            }
        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox_Name;
        }
    }
}
