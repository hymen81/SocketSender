using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCam_Capture;

namespace SocketSender
{
    //Design by Pongsakorn Poosankam
    class WebCam
    {
        private WebCamCapture webcam;
        private System.Windows.Forms.PictureBox _FrameImage;
        private int FrameNumber = 30;
        public void InitializeWebCam(ref System.Windows.Forms.PictureBox ImageControl)
        {
            webcam = new WebCamCapture();
            webcam.FrameNumber = ((ulong)(0ul));
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.ImageCaptured += new WebCamCapture.WebCamEventHandler(webcam_ImageCaptured);
            _FrameImage = ImageControl;
        }

        void webcam_ImageCaptured(object source, WebcamEventArgs e)
        {
            _FrameImage.Image = e.WebCamImage;
        }

        public void Start()
        {
            webcam.TimeToCapture_milliseconds = FrameNumber;
            webcam.Start(0);
        }

        public void Stop()
        {
            webcam.Stop();
        }

        public void Continue()
        {
            // change the capture time frame
            webcam.TimeToCapture_milliseconds = FrameNumber;

            // resume the video capture from the stop
            webcam.Start(this.webcam.FrameNumber);
        }

        public void ResolutionSetting()
        {
            webcam.Config();
        }

        public void AdvanceSetting()
        {
            webcam.Config2();
        }

    }


    public partial class Form1 : Form
    {
        SocketClient sc;
        public Form1()
        {
            InitializeComponent();
             sc = new SocketClient(this,"192.168.1.107", 5000);
             webcam = new WebCam();
             webcam.InitializeWebCam(ref pictureBox2);
        }
        WebCam webcam;
        public void addListBoxString(string s) 
        {
            listBox1.Items.Add(s);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            pictureBox1.Image = pictureBox2.Image;
           // sc.Connect();
          //
           // sc.SendByByte(rgbValues);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sc.Connect();
            //sc.Send(textBox1.Text);
            sc.SendByByte(rgbValues);
           // sc.Send(imageString);
        }

        string imageString = "";
        static byte[] rgbValues;
        unsafe void bmpToChar(Image image)
        {
            Bitmap labeledBitmap = null;
            try
            {
                labeledBitmap = new Bitmap(image);
            }
            catch (Exception)
            {
                image.Dispose();
                return;
            }
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bitmapdata = labeledBitmap.LockBits(rect, ImageLockMode.ReadWrite, labeledBitmap.PixelFormat);
            IntPtr ptr = bitmapdata.Scan0;
            int bytes = Math.Abs(bitmapdata.Stride) * image.Height;
            rgbValues = new byte[bytes];
            Marshal.Copy(ptr, rgbValues, 0, bytes);
          /*  for (int i = 0; i < rgbValues.Length; i++)
            {
                imageString += rgbValues[i].ToString();
                listBox1.Items.Add(rgbValues[i].ToString());
            }*/
            imageString =System.Text.Encoding.Default.GetString(rgbValues);

            StringBuilder Sb = new StringBuilder();
            for (int i = 0; i < rgbValues.Length; i++)
            {
                if (rgbValues[i] == 0)
                    rgbValues[i] = 1;
            }
          //  Bitmap oBitmap = BitmapFactory
           // pictureBox2.Image = BufferToImage(rgbValues);

            listBox1.Items.Add(Sb);
        }

        /// <summary>
        /// 將 Image 轉換為 Byte 陣列。
        /// </summary>
        /// <param name="Image">Image 。</param>
        /// <param name="imageFormat">指定影像格式。</param>        
        public  byte[] ImageToBuffer(Image Image, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            if (Image == null) { return null; }
            byte[] data = null;
            using (MemoryStream oMemoryStream = new MemoryStream())
            {
                //建立副本
                using (Bitmap oBitmap = new Bitmap(Image))
                {
                    //儲存圖片到 MemoryStream 物件，並且指定儲存影像之格式
                    oBitmap.Save(oMemoryStream, imageFormat);
                    //設定資料流位置
                    oMemoryStream.Position = 0;
                    //設定 buffer 長度
                    data = new byte[oMemoryStream.Length];
                    
                    //將資料寫入 buffer
                    oMemoryStream.Read(data, 0, Convert.ToInt32(oMemoryStream.Length));
                    //將所有緩衝區的資料寫入資料流
                    oMemoryStream.Flush();
                }
            }
            return data;
        }

        /// <summary>
        /// 將 Byte 陣列轉換為 Image。
        /// </summary>
        /// <param name="Buffer">Byte 陣列。</param>        
        public static Image BufferToImage(byte[] Buffer)
        {
            if (Buffer == null || Buffer.Length == 0) { return null; }
            byte[] data = null;
            Image oImage = null;
            Bitmap oBitmap = null;
            //建立副本
            data = (byte[])Buffer.Clone();
            try
            {
                MemoryStream oMemoryStream = new MemoryStream(Buffer);
                //設定資料流位置
                oMemoryStream.Position = 0;
                oImage = System.Drawing.Image.FromStream(oMemoryStream);
                //建立副本
                oBitmap = new Bitmap(oImage);
            }
            catch
            {
                throw;
            }
            //return oImage;
            return oBitmap;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string fileName = openFileDialog1.FileName;
            pictureBox1.Image = Image.FromFile(fileName);
           // byte[] ss = ImageToBuffer(pictureBox1.Image, System.Drawing.Imaging.ImageFormat.Png);

            
            /*
            string byteStr = "";
            StringBuilder Sb = new StringBuilder();
            for (int i = 0; i < ss.Length; i++)
            {
                Sb.Append(ss[i].ToString());
            }

            listBox1.Items.Add(Sb);*/


            bmpToChar(pictureBox1.Image);
           
           // pictureBox2.Image = BufferToImage(ss);
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webcam.Start();
         //   webcam.Continue();
        }

        
    }
}
