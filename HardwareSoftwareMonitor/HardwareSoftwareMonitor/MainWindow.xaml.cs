using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Management;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using System.Net.NetworkInformation;

namespace HardwareSoftwareMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        PerformanceCounter processorUsage = new PerformanceCounter("Processzor", "A processzor kihasználtsága (%)", "_Total");
        PerformanceCounter availableMemory = new PerformanceCounter("Memória", "Rendelkezésre álló memória (megabájt)");
        PerformanceCounter uptime = new PerformanceCounter("Rendszer", "Rendszerindítás óta eltelt idő (s)");
        DispatcherTimer tick = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            tick.Interval = TimeSpan.FromSeconds(1);
            tick.Tick += Tick_Tick;
            tick.Start();

            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");

            foreach (ManagementObject obj in myProcessorObject.Get())
            {
                CPUName.Content = "Név: " + obj["Name"];
                CPUID.Content = "CPU ID-je: " + obj["DeviceID"];
                CPUManu.Content = "Gyártó: " + obj["Manufacturer"];
                CPUClock.Content = "Órajel: " + obj["CurrentClockSpeed"];
                CPUCores.Content = "Magok száma: " + obj["NumberOfCores"];
                CPUArch.Content = "Architektúra: " + obj["Architecture"];
                CPUFamily.Content = "Család: " + obj["Family"];
            }

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                TabItem tab = new TabItem();
                tab.Header = d.Name;
                Drives.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.Content = "Lemez típusa: " + d.DriveType;
                grid.Children.Add(l1);
                if (d.IsReady == true)
                {
                    Label l2 = new Label();
                    l2.Margin = new Thickness(10, 40, 0, 0);
                    l2.Content = "Fájlrendszer: " + d.DriveFormat;
                    Label l3 = new Label();
                    l3.Margin = new Thickness(10, 70, 0, 0);
                    l3.Content = "Szabad hely: " + Math.Round(d.TotalFreeSpace / 1073741824.0, 2) + "GB";
                    Label l4 = new Label();
                    l4.Margin = new Thickness(10, 100, 0, 0);
                    l4.Content = "Teljes méret: " + Math.Round(d.TotalSize / 1073741824.0, 2) + "GB";
                    grid.Children.Add(l2);
                    grid.Children.Add(l3);
                    grid.Children.Add(l4);
                }
                tab.Content = grid;
            }

            ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");

            foreach (ManagementObject obj in myVideoObject.Get())
            {
                GPUName.Content = "Név: " + obj["Name"];
                GPUStatus.Content = "Állapot: " + obj["Status"];
                GPUID.Content = "Eszköz ID-je: " + obj["DeviceID"];
                GPURAM.Content = "Videóadapter RAM-ja: " + Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / 1048576.0, 2) + "MB";
                GPUDAC.Content = "Videóadapter DAC Típusa: " + obj["AdapterDACType"];
                GPUDriver.Content = "Driver Verziója: " + obj["DriverVersion"];
                GPUVideoProcessor.Content = "Videóprocesszor: " + obj["VideoProcessor"];
                if (Convert.ToInt32(obj["VideoArchitecture"]) == 0)
                {
                    GPUArch.Content = "Videóarchitektúra típusa: " + "x86";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 1)
                {
                    GPUArch.Content = "Videóarchitektúra típusa: " + "MIPS";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 2)
                {
                    GPUArch.Content = "Videóarchitektúra típusa: " + "Alpha";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 3)
                {
                    GPUArch.Content = "Videóarchitektúra típusa: " + "PowerPC";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 5)
                {
                    GPUArch.Content = "Videóarchitektúra típusa: " + "ARM";
                }
                else if (Convert.ToInt32(obj["VideoArchitecture"]) == 6)
                {
                    GPUArch.Content = "Videóarchitektúra típusa: " + "ia64";
                }
                else
                {
                    GPUArch.Content = "Videóarchitektúra típusa: Egyéb";
                }
                if (Convert.ToInt32(obj["VideoMemoryType"]) == 3)
                {
                    GPUMemType.Content = "Videómemória Típusa: " + "VRAM";
                }
                else if (Convert.ToInt32(obj["VideoMemoryType"]) == 4)
                {
                    GPUMemType.Content = "Videómemória Típusa: " + "DRAM";
                }
                else if (Convert.ToInt32(obj["VideoMemoryType"]) == 5)
                {
                    GPUMemType.Content = "Videómemória Típusa: " + "SRAM";
                }
                else
                {
                    GPUMemType.Content = "Videómemória Típusa: " + "Egyéb";
                }
                List<string> drivers = new List<string>();
                foreach (var item in obj["InstalledDisplayDrivers"].ToString().Split(','))
                {
                    GPUDrivers.Items.Add(item);
                }
            }

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            if (nics == null || nics.Length < 1)
            {
                
            }
            else
            {
                foreach (NetworkInterface adapter in nics)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    TabItem tab = new TabItem();
                    tab.Header = adapter.Name;
                    NetworkConnections.Items.Add(tab);
                    Grid grid = new Grid();
                    grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                    Label l1 = new Label();
                    l1.Margin = new Thickness(10, 10, 0, 0);
                    l1.FontSize = 16;
                    l1.Content = adapter.Description;
                    Label l2 = new Label();
                    l2.Margin = new Thickness(10, 50, 0, 0);
                    l2.Content = "Interfész típusa: " + adapter.NetworkInterfaceType;
                    Label l3 = new Label();
                    l3.Margin = new Thickness(10, 80, 0, 0);
                    l3.Content = "Fizikai cím: " + adapter.GetPhysicalAddress().ToString();
                    Label l4 = new Label();
                    l4.Margin = new Thickness(10, 110, 0, 0);
                    l4.Content = "Állapot: " + adapter.OperationalStatus;
                    grid.Children.Add(l1);
                    grid.Children.Add(l2);
                    grid.Children.Add(l3);
                    grid.Children.Add(l4);
                    tab.Content = grid;
                }
            }

            ManagementObjectSearcher myAudioObject = new ManagementObjectSearcher("select * from Win32_SoundDevice");
            
            foreach (ManagementObject obj in myAudioObject.Get())
            {
                TabItem tab = new TabItem();
                tab.Header = obj["Name"];
                AudioDevices.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.FontSize = 16;
                l1.Content = "Termék neve: " + obj["ProductName"];
                Label l2 = new Label();
                l2.Margin = new Thickness(10, 50, 0, 0);
                l2.Content = "Elérhetőség: " + obj["Availability"];
                Label l3 = new Label();
                l3.Margin = new Thickness(10, 80, 0, 0);
                l3.Content = "Eszköz ID-je: " + obj["DeviceID"];
                Label l4 = new Label();
                l4.Margin = new Thickness(10, 110, 0, 0);
                l4.Content = "Támogatott-e az energiamenedzsment: " + obj["PowerManagementSupported"];
                Label l5 = new Label();
                l5.Margin = new Thickness(10, 140, 0, 0);
                l5.Content = "Állapot: " + obj["Status"];
                grid.Children.Add(l1);
                grid.Children.Add(l2);
                grid.Children.Add(l3);
                grid.Children.Add(l4);
                grid.Children.Add(l5);
                tab.Content = grid;
            }

            ManagementObjectSearcher myPrinterObject = new ManagementObjectSearcher("select * from Win32_Printer");

            foreach (ManagementObject obj in myPrinterObject.Get())
            {
                TabItem tab = new TabItem();
                tab.Header = obj["Name"];
                Printers.Items.Add(tab);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Color.FromRgb(229, 229, 229));
                Label l1 = new Label();
                l1.Margin = new Thickness(10, 10, 0, 0);
                l1.Content = "Hálózati nyomtató-e: " + obj["Network"];
                Label l2 = new Label();
                l2.Margin = new Thickness(10, 40, 0, 0);
                l2.Content = "Elérhetőség: " + obj["Availability"];
                Label l3 = new Label();
                l3.Margin = new Thickness(10, 70, 0, 0);
                l3.Content = "Eszköz ID-je: " + obj["DeviceID"];
                Label l4 = new Label();
                l4.Margin = new Thickness(10, 100, 0, 0);
                l4.Content = "Alapértelmezett nyomtató: " + obj["Default"];
                Label l5 = new Label();
                l5.Margin = new Thickness(10, 130, 0, 0);
                l5.Content = "Állapot: " + obj["Status"];
                grid.Children.Add(l1);
                grid.Children.Add(l2);
                grid.Children.Add(l3);
                grid.Children.Add(l4);
                grid.Children.Add(l5);
                tab.Content = grid;
            }

            ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            foreach (ManagementObject obj in myOperativeSystemObject.Get())
            {
                OSType.Content = "Operációs rendszer: " + obj["Caption"];
                OSWinDir.Content = "Windows mappa helye: " + obj["WindowsDirectory"];
                OSSerial.Content = "Sorozatszám: " + obj["SerialNumber"];
                OSSysDir.Content = "Rendszermappa helye: " + obj["SystemDirectory"];
                OSVersion.Content = "Verzió: " + obj["Version"];
            }
        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            cpu.Content = "CPU kihasználtsága: " + (int)processorUsage.NextValue() + " %";
            mem.Content = "Rendelkezésre álló memória: " + (int)availableMemory.NextValue() + " MB";
            sysup.Content = "Rendszerindítás óta eltelt idő: " + (int)uptime.NextValue() / 60 / 60 + " óra";
        }
    }
}
