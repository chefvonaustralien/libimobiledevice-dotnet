using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SeasideResearch.LibCurlNet;

namespace libimobiledevice_dotnet
{
     class External
    {
        public enum plist_type
        {
            PLIST_BOOLEAN = 0,	/**< Boolean, scalar type */
            PLIST_UINT = 1,	/**< Unsigned integer, scalar type */
            PLIST_REAL = 2,	/**< Real, scalar type */
            PLIST_STRING = 3,	/**< ASCII string, scalar type */
            PLIST_ARRAY = 4,	/**< Ordered array, structured type */
            PLIST_DICT = 5,	/**< Unordered dictionary (key/value pair), structured type */
            PLIST_DATE = 6,	/**< Date, scalar type */
            PLIST_DATA = 7,	/**< Binary data, scalar type */
            PLIST_KEY = 8,	/**< Key in dictionaries (ASCII String), scalar type */
            PLIST_NONE = 9	/**< No type */
        };


        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern void idevice_set_debug_level(int level);

        [DllImport("imobi.dll", CallingConvention=CallingConvention.Cdecl)]
         static public extern short idevice_get_device_list(out IntPtr devices,out int number_of_devices);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short idevice_new(out IntPtr device, string uuid);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short lockdownd_client_new_with_handshake(IntPtr device, out IntPtr client, string label);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short lockdownd_get_device_name(IntPtr client, out IntPtr device_name);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short lockdownd_start_session(IntPtr client, string host_id, out IntPtr session_id, out int ssl_enabled);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short lockdownd_start_service(IntPtr client, string service, out ushort port);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short afc_client_new(IntPtr idevice, ushort port, out IntPtr afc_client);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short afc_read_directory(IntPtr afcclient, string dir, out IntPtr dirlist);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short lockdownd_deactivate(IntPtr client);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short lockdownd_activate(IntPtr client, IntPtr activation_record);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short afc_get_file_info(IntPtr afc_client, string filename, out IntPtr infolist);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short lockdownd_get_value(IntPtr lockdownd_client, string domain, string key, out IntPtr value);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern plist_type plist_get_node_type(IntPtr node);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern void plist_get_string_val(IntPtr node, out IntPtr val);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern void plist_free(IntPtr plist);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern void plist_to_xml(IntPtr plist, out IntPtr plist_xml, out uint length);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern void plist_from_xml(string plist_xml, int length, out IntPtr plist);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern IntPtr plist_dict_get_item(IntPtr node, string key);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short screenshotr_client_new(IntPtr device, ushort port, out IntPtr client);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
         static public  extern short screenshotr_take_screenshot(IntPtr screenshotr_client, out IntPtr imgdata, out ulong imgsize);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short idevice_get_uuid(IntPtr device, out IntPtr uuid);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short idevice_connect(IntPtr device, ushort port, out IntPtr connection);

        [DllImport("imobi.dll", CallingConvention = CallingConvention.Cdecl)]
        static public extern short idevice_disconnect(IntPtr connection);
    }

     #region CURL externals

     class curl
     {
         [DllImport("libcurl.dll",CallingConvention = CallingConvention.Cdecl)]
         static public extern short curl_global_init(long flags);

         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern IntPtr curl_easy_init();

         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern void curl_global_cleanup();

         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern short curl_formadd(out IntPtr httppost,
                                        out IntPtr last_post, int unknown1, string unknown2, int unknown3, string PLATFORM, int unknown4);

         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern IntPtr curl_slist_append(IntPtr header, string text);
         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern short curl_easy_setopt(IntPtr handle, int option, IntPtr extra);
         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern short curl_easy_setopt(IntPtr handle, int option, string extra);
         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern short curl_easy_setopt(IntPtr handle, int option, Easy.WriteFunction wf);


         [DllImport("libcurl.dll", CallingConvention = CallingConvention.Cdecl)]
         static public extern short curl_easy_perform(IntPtr handle);
     }

     #endregion
 }
