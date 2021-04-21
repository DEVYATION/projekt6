using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.Diagnostics;
using System.Windows.Threading;

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
        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            cpu.Content = (int)processorUsage.NextValue() + " %";
            mem.Content = (int)availableMemory.NextValue() + " MB";
            sysup.Content = (int)uptime.NextValue() / 60 / 60 + " óra";
        }
    }
}
