using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace idevice_test
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        libimobiledevice_dotnet.device device;
        private void button1_Click(object sender, EventArgs e)
        {

            libimobiledevice_dotnet.device_searcher searcher = new libimobiledevice_dotnet.device_searcher();
            int number_of_devices;
            string[] list = searcher.get_device_list(out number_of_devices);
            if(number_of_devices > 0) {

                device= new libimobiledevice_dotnet.device(list[0]);

                if (device != null)
                {
                    button1.Enabled = false;
                    button2.Enabled = true;
                    button3.Enabled = true;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (device != null)
            {
                libimobiledevice_dotnet.lockdownd lockdownd = new libimobiledevice_dotnet.lockdownd(device, "test");
                lockdownd.deactivate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (device != null)
            {
                libimobiledevice_dotnet.lockdownd lockdownd = new libimobiledevice_dotnet.lockdownd(device, "test");
                lockdownd.activate_via_apple_servers();
            }
        }

       
    }
}
