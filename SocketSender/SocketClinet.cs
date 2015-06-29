using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketSender
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using System.Net;
    using System.Net.Sockets;



    class SocketClient
    {
        #region private property

        /// 
        /// 遠端 socket server IP 位址
        /// 
        private string _RemoteIpAddress;

        /// 
        /// 遠端 socket server 所監聽的 port number
        /// 
        private int _RemotePortNumber;


        /// 
        /// socket client 物件(連接遠端 socket server 用)
        /// 
        private TcpClient _TcpClient;

        #endregion


        #region public static property

        public static List<string> sMessages = new List<string>();
        private delegate void SetListBox(string s);

        SetListBox setListBox;


        #endregion


        #region constructor

        Form1 f;

        public SocketClient(Form1 form,string inRemoteIpAddr, int inRemotePortNum)
        {
            this._RemoteIpAddress = inRemoteIpAddr;
            this._RemotePortNumber = inRemotePortNum;
            f = form;
            setListBox = f.addListBoxString;
        }

        #endregion


        #region public method

        /// 
        /// 連線至 socket server
        /// 
        public void Connect()
        {
            //初始化 socket client
            _TcpClient = new TcpClient();
            _TcpClient.Connect(_RemoteIpAddress, _RemotePortNumber);
            f.Invoke(setListBox, new object[] { "Client Socket Program - Server Connected ..." });
          //  sMessages.Add("Client Socket Program - Server Connected ...");
        }


        public string SendByByte(byte[] outStream) 
        {
            try
            {
                //取得用來傳送訊息至 socket server 的 stream 物件
                NetworkStream serverStream = _TcpClient.GetStream();

     

                //將資料寫入 stream object (表示傳送資料至 socket server)
                serverStream.Write(outStream, 0, outStream.Length);
              //  serverStream.Flush();

                //讀取 socket server 回傳值並轉為 string
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)_TcpClient.ReceiveBufferSize);
                string returndata = System.Text.Encoding.UTF8.GetString(inStream);
                f.Invoke(setListBox, new object[] { "Data from Server : " + returndata });
                //sMessages.Add("Data from Server : " + returndata);
                return returndata;
            }
            catch (Exception exp)
            {
                sMessages.Add(exp.ToString());
                return exp.ToString();
            }
        
        }


        public string Send(string inMessage)
        {
            try
            {
                //取得用來傳送訊息至 socket server 的 stream 物件
                NetworkStream serverStream = _TcpClient.GetStream();

                //將資料轉為 byte[]
                byte[] outStream = System.Text.Encoding.UTF8.GetBytes(inMessage);

                //將資料寫入 stream object (表示傳送資料至 socket server)
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                //讀取 socket server 回傳值並轉為 string
                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)_TcpClient.ReceiveBufferSize);
                string returndata = System.Text.Encoding.UTF8.GetString(inStream);
                f.Invoke(setListBox, new object[] { "Data from Server : " + returndata });
                //sMessages.Add("Data from Server : " + returndata);
                return returndata;
            }
            catch (Exception exp)
            {
                sMessages.Add(exp.ToString());
                return exp.ToString();
            }
        }

        #endregion
    }
}

