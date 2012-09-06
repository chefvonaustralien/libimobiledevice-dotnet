using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SeasideResearch.LibCurlNet;

namespace libimobiledevice_dotnet
{
    public class lockdownd
    {
        #region private members

        private IntPtr lockdownd_handle;
        private short ret;
        private string output = "";
        private System.IO.StreamWriter giftWriter;
        private IntPtr devname;

        #endregion

        public lockdownd(device dev, string label)
        {
            External.lockdownd_client_new_with_handshake(dev.ToPointer(), out lockdownd_handle, label);
        }

        public short get_device_name(out string device_name)
        {
            ret = External.lockdownd_get_device_name(lockdownd_handle, out devname);
            device_name = Marshal.PtrToStringAnsi(devname);
            return ret;
        }

        public short start_service(string service_id, out ushort port)
        {
            return External.lockdownd_start_service(lockdownd_handle, service_id, out port);
        }
        public short deactivate()
        {
            return External.lockdownd_deactivate(lockdownd_handle);
        }

        private Int32 OnWriteData(Byte[] buf, Int32 size, Int32 nmemb, Object extraData)
        {
            output += System.Text.Encoding.UTF8.GetString(buf);
            return size * nmemb;
        }


        private string lockdownd_get_string_value(IntPtr lockdownd_handle, string what)
        {

            string val = null;
            IntPtr val_node;
            IntPtr ptrval;
            External.lockdownd_get_value(lockdownd_handle, null, what, out val_node);

            if (val_node == null || External.plist_get_node_type(val_node) != External.plist_type.PLIST_STRING)
            {
                MessageBox.Show("Unable to get " + what + " from lockdownd.");
                return null;
            }
            External.plist_get_string_val(val_node, out ptrval);
            val = Marshal.PtrToStringAnsi(ptrval);
            External.plist_free(val_node);
            return val;

        }
        public short activate_via_apple_servers()
        {
            IntPtr activation_info_node;
            string activation_info;
            string activation_info_pre;


            string ICCID = lockdownd_get_string_value(lockdownd_handle, "IntegratedCircuitCardIdentity");
            string IMEI = lockdownd_get_string_value(lockdownd_handle, "InternationalMobileEquipmentIdentity");
            string IMSI = lockdownd_get_string_value(lockdownd_handle, "InternationalMobileSubscriberIdentity");
            string serialNumber = lockdownd_get_string_value(lockdownd_handle, "SerialNumber");

            External.lockdownd_get_value(lockdownd_handle, null, "ActivationInfo", out activation_info_node);

            int type = (int)External.plist_get_node_type(activation_info_node);

            if (activation_info_node == null || External.plist_get_node_type(activation_info_node) != External.plist_type.PLIST_DICT)
            {
                MessageBox.Show("Unable to get ActivationInfo from lockdownd");
                return -1;
            }

            uint activation_info_size = 0;
            IntPtr activation_info_data; //char*

            External.plist_to_xml(activation_info_node, out activation_info_data, out activation_info_size);
            External.plist_free(activation_info_node);

            string activation_info_data_s = Marshal.PtrToStringAnsi(activation_info_data);


            //string activation_info_start = strstr(activation_info_data_s, "<dict>");
            int index_start = activation_info_data_s.IndexOf("<dict>");
            int index_end = activation_info_data_s.IndexOf("</dict>");
            activation_info_pre = activation_info_data_s.Remove(0, index_start);
            activation_info = activation_info_pre.Replace(activation_info_data_s.Remove(0, index_end + 7), "");
            if (index_start == -1)
            {
                MessageBox.Show("Unable to locate beginning of ActivationData.");
                return -1;
            }
            if (index_end == -1)
            {
                MessageBox.Show("Unable to locate end of ActivationData.");
                return -1;
            }
            index_end = index_end + 7;
            activation_info_size = Convert.ToUInt32(index_end - index_start);
            IntPtr activationInfo = Marshal.StringToHGlobalAnsi(activation_info);
            Curl.GlobalInit(1);
            Easy easy_handle = new Easy();
            //easy_handle.SetOpt(CURLoption.CURLOPT_POSTFIELDS, "machine=linux&InStoreActivation=false&IMEI="+ IMEI+ "&IMSI=" + IMSI + "&ICCID=" + ICCID + "&AppleSerialNumber=" + serialNumber + "&activation-info=" + activation_info);
            MultiPartForm mf = new MultiPartForm();
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "machine", CURLformoption.CURLFORM_COPYCONTENTS, "win32", CURLformoption.CURLFORM_END });
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "InStoreActivation", CURLformoption.CURLFORM_COPYCONTENTS, "false", CURLformoption.CURLFORM_END });
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "IMEI", CURLformoption.CURLFORM_COPYCONTENTS, IMEI, CURLformoption.CURLFORM_END });
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "IMSI", CURLformoption.CURLFORM_COPYCONTENTS, IMSI, CURLformoption.CURLFORM_END });
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "ICCID", CURLformoption.CURLFORM_COPYCONTENTS, ICCID, CURLformoption.CURLFORM_END });
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "AppleSerialNumber", CURLformoption.CURLFORM_COPYCONTENTS, serialNumber, CURLformoption.CURLFORM_END });
            mf.AddSection(new object[] { CURLformoption.CURLFORM_COPYNAME, "activation-info", CURLformoption.CURLFORM_COPYCONTENTS, activation_info, CURLformoption.CURLFORM_END });


            IntPtr curl_slist = new IntPtr();
            curl_slist = curl.curl_slist_append(curl_slist, "X-Apple-Tz: -14400");
            curl_slist = curl.curl_slist_append(curl_slist, "X-Apple-Store-Front: 143441-1");

            easy_handle.SetOpt(CURLoption.CURLOPT_HTTPHEADER, curl_slist);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            giftWriter = new System.IO.StreamWriter(ms);
            Easy.WriteFunction wf = new Easy.WriteFunction(OnWriteData);
            easy_handle.SetOpt(CURLoption.CURLOPT_HTTPPOST, mf);
            easy_handle.SetOpt(CURLoption.CURLOPT_USERAGENT, "iTunes/9.1 (Macintosh; U; Intel Mac OS X 10.5.6)");
            easy_handle.SetOpt(CURLoption.CURLOPT_URL, "https://albert.apple.com/WebObjects/ALUnbrick.woa/wa/deviceActivation");
            easy_handle.SetOpt(CURLoption.CURLOPT_SSL_VERIFYPEER, false);
            easy_handle.SetOpt(CURLoption.CURLOPT_WRITEFUNCTION, wf);
            ret = (short)easy_handle.Perform();



            index_start = output.IndexOf("<plist");
            index_end = output.IndexOf("</plist>");

            string ticket_data_pre = output.Remove(0, index_start);
            string ticket_data = ticket_data_pre.Replace(output.Remove(0, index_end + 8), "");

            int ticket_length = ticket_data.Length;
            IntPtr ticket_dict;
            External.plist_from_xml(ticket_data, ticket_length, out ticket_dict);

            IntPtr iphone_activation_node = External.plist_dict_get_item(ticket_dict, "iphone-activation");
            if (iphone_activation_node.ToInt32() == 0)
            {
                iphone_activation_node = External.plist_dict_get_item(ticket_dict, "device-activation");
                if (iphone_activation_node.ToInt32() == 0)
                {
                    MessageBox.Show("Unable to find activation node.");
                    return -2;
                }
            }

            IntPtr activation_record = External.plist_dict_get_item(iphone_activation_node, "activation-record");
            if (activation_record.ToInt32() == 0)
            {
                MessageBox.Show("Unable to find activation record.");
                return -2;
            }

            ret = External.lockdownd_activate(lockdownd_handle, activation_record);
            return ret;
        }

    }
}
