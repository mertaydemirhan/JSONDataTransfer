using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using System.Collections;
using System.Configuration;
using System.Globalization;

namespace DataTransfer
{
    public partial class Form1 : Form
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        public readonly string ServerAdress = ConfigurationManager.AppSettings["ServerAdress"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
        public readonly string DatabaseName = ConfigurationManager.AppSettings["DatabaseName"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
        public readonly string UsrName = ConfigurationManager.AppSettings["UsrName"];
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8601 // Possible null reference assignment.
        public readonly string Pw = ConfigurationManager.AppSettings["Pw"];
#pragma warning restore CS8601 // Possible null reference assignment.
        public string Os;
        public class Model
        {
            public string ad { get; set; }
            public string soyad { get; set; }
            public string kartid { get; set; }
            public string mevcutPuan { get; set; }
            public string tldegeri { get; set; }
            public string projetipi { get; set; }
            public string ceptel { get; set; }

        }
        string PCName = Dns.GetHostName();
        public Form1(string _Os)
        {
            InitializeComponent();
            Os = _Os;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(theData.Nodes.Count>0)
                theData.Nodes.Clear();
            ReadJsonValue();

        }

        private void ReadJsonValue()
        { 
          if(Os.Contains("BakiyeSorgula"))
            { 
                // 0155500636 old kart numarasý
                // 123asd123asd old pc adresi
                string url = "https://suleymanpasa.bel.tr/Shared/jsonservice.php" + Os;
                string json = "";
                using (var wc = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    wc.UseDefaultCredentials = true;
                    wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    json = wc.DownloadString(url);
                }
                var res = JsonConvert.DeserializeObject<List<Model>>(json);
                var result = res[0];
                if (!string.IsNullOrEmpty(result.kartid) && !string.IsNullOrEmpty(result.mevcutPuan))
                {
                    theData.Nodes.Add(result.ad);
                    theData.Nodes.Add(result.soyad);
                    theData.Nodes.Add(result.kartid);
                    theData.Nodes.Add(result.mevcutPuan);
                    theData.Nodes.Add(result.tldegeri);
                    theData.Nodes.Add(result.ceptel);
                    SqlConnection conn = new SqlConnection("Server=" + ServerAdress + ";Database=" + DatabaseName + ";User Id=" + UsrName + ";Password=" + Pw + ";");
                    SqlCommand komut = new SqlCommand();
                    conn.Open();
                    komut = new SqlCommand("Insert into AS_BakiyeSorgu (ad,soyad,kartid,mevcutPuan,tldegeri,ceptel,computerName,HDDSerial,ProcessDate)" +
                        " values('" + theData.Nodes[0].Text + "','" + theData.Nodes[1].Text + "','" + theData.Nodes[2].Text + "','" +
                        theData.Nodes[3].Text + "','" + theData.Nodes[4].Text + "','" + theData.Nodes[5].Text + "','" + PCName + "','" + getHddSerial() + "',convert(varchar,'" + DateTime.Now + "',103))", conn);
                    komut.ExecuteNonQuery();
                    conn.Close();
                    Dispose();
                    Application.Exit();
                    GC.SuppressFinalize(this);
                }

            }


            if (Os.Contains("OdemeYapMutlukent"))
            {
                // 0155500636 old kart numarasý
                // 123asd123asd old pc adresi
                string HDDserialNo = getHddSerial();
                HDDserialNo = HDDserialNo.Replace(".", "");
                Os =Os.Replace("%%","&cihazid="+PCName + HDDserialNo);
                string url = "https://suleymanpasa.bel.tr/Shared/jsonservice.php" + Os;
                string json = "";
                int Transaction_Qty = 0;
                int baslangicNo = Os.IndexOf("adet=") + 5;
                int karaktersayi = Os.Length - baslangicNo;
                Transaction_Qty = Convert.ToInt32(Os.Substring(baslangicNo, karaktersayi));
                using (var wc = new WebClient())
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    wc.UseDefaultCredentials = true;
                    wc.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    json = wc.DownloadString(url);
                }
                var res = JsonConvert.DeserializeObject<List<Model>>(json);
                var result = res[0];
                if (!string.IsNullOrEmpty(result.kartid) && !string.IsNullOrEmpty(result.mevcutPuan))
                {
                    theData.Nodes.Add(result.kartid);
                    theData.Nodes.Add(result.mevcutPuan);
                    theData.Nodes.Add(result.tldegeri);
                    theData.Nodes.Add(result.projetipi);
                    SqlConnection conn = new SqlConnection("Server=" + ServerAdress + ";Database=" + DatabaseName + ";User Id=" + UsrName + ";Password=" + Pw + ";");
                    SqlCommand komut = new SqlCommand();
                    conn.Open();
                    komut = new SqlCommand("Insert into AS_Hareket (kartid,mevcutPuan,tldegeri,projetipi,computerName,HDDSerial,ProcessDate,Transaction_Qty)" +
                        " values('" + theData.Nodes[0].Text + "','" + theData.Nodes[1].Text + "','" + theData.Nodes[2].Text + "','" +
                        theData.Nodes[3].Text + "','"  + PCName + "','" + getHddSerial() + "',convert(varchar,'" + DateTime.Now + "',103)," + Transaction_Qty + ")", conn);
                    MessageBox.Show(komut.ToString());
                    komut.ExecuteNonQuery();
                    conn.Close();
                    Dispose();
                    Application.Exit();
                    GC.SuppressFinalize(this);
                }

            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            theData.Enabled = false;    
            if (!string.IsNullOrEmpty(Os))
            ReadJsonValue();
        }


        private void button2_Click(object sender, EventArgs e)
        {

        }



        public static string getHddSerial()
        {
            String hddSerial = null;

            ManagementObjectSearcher hddSearcher = new ManagementObjectSearcher("Select * FROM WIN32_DiskDrive");
            ManagementObjectCollection mObject = hddSearcher.Get();

            foreach (ManagementObject obj in mObject)
            {
                hddSerial = (string)obj["SerialNumber"];
            }
            return hddSerial;
        }

    }
}
