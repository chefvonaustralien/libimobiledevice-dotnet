using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace libimobiledevice_dotnet
{
    public class device_searcher
    {
        #region private members

        private IntPtr devices_ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
        
        private string[] device_list;
        private short ret;
        private IntPtr devicePtr;

        #endregion

        

        public device_searcher()
        {
            int number_of_devices;
            
            short ret = External.idevice_get_device_list(out devices_ptr, out number_of_devices);
            device_list = new string[number_of_devices];
            for (int i = 0; i < number_of_devices; i++)
            {
                IntPtr devicePtr = (IntPtr)Marshal.PtrToStructure(devices_ptr, typeof(IntPtr));
                device_list[i] = Marshal.PtrToStringAnsi(devicePtr);
                devices_ptr += IntPtr.Size;
            }
        }

        public device.idevice_error_t refresh()
        {

            int number_of_devices;

            ret = External.idevice_get_device_list(out devices_ptr, out number_of_devices);
            if (ret != 0)
            {
                return (device.idevice_error_t)ret;
            }
            device_list = new string[number_of_devices];
            for (int i = 0; i < number_of_devices; i++)
            {
                devicePtr = (IntPtr)Marshal.PtrToStructure(devices_ptr, typeof(IntPtr));
                device_list[i] = Marshal.PtrToStringAnsi(devicePtr);
                devices_ptr += IntPtr.Size;
            }
            return (device.idevice_error_t)ret;

        }

        public string[] get_device_list(out int number_of_devices)
        {
            number_of_devices = device_list.Length;
            return device_list;

        }
    }

    public class device_connection
    {
        #region private members

        private IntPtr connection_handle;

        #endregion

        public device_connection(device dev, ushort port)
        {
            External.idevice_connect(dev.ToPointer(), port, out connection_handle);
        }

        public device.idevice_error_t disconnect()
        {
            return (device.idevice_error_t)External.idevice_disconnect(connection_handle);
        }

    }

    public class device
    {
        #region private members

        private IntPtr handle;
       
        private IntPtr uuid;

        #endregion
        public enum idevice_error_t : short
        {
            IDEVICE_E_SUCCESS = 0,
            IDEVICE_E_INVALID_ARG = -1,
            IDEVICE_E_UNKNOWN_ERROR = -2,
            IDEVICE_E_NO_DEVICE = -3,
            IDEVICE_E_NOT_ENOUGH_DATA = -4,
            IDEVICE_E_BAD_HEADER = -5,
            IDEVICE_E_SSL_ERROR = -6
        }
        
        
        /// <summary>
        /// Initializes a new device
        /// </summary>
        /// <param name="uuid">UUID of the device</param>
        public device(string uuid)
        {
            External.idevice_new(out handle, uuid);

        }

        public void set_debug_level(int level)
        {
            External.idevice_set_debug_level(level);   
        }

        
        public string get_uuid()
        {
            External.idevice_get_uuid(handle, out uuid);
            return Marshal.PtrToStringAnsi(uuid);
        }

        public IntPtr ToPointer()
        {
            return handle;
        }
        
    }
}
