using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NetworkTables;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalVision
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private static string ROBOT_ADDR = "roboRIO-6406-FRC.local";

        public MainPage()
        {
            this.InitializeComponent();

            testText.Text = "Hello World!";

            NetworkTable.SetIPAddress(ROBOT_ADDR);
            NetworkTable tbl = NetworkTable.GetTable("test");
            tbl.PutNumber("testKey", 64.06);

            Debug.WriteLine(tbl.GetNumber("test", 0.0));

        }
    }
}
