using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.ComponentModel;
using CustomPath;

namespace CCTV
{
    public class Cam
    {
        
        public string cam { get; set; }
        public string ipString { get; set; }
        public string user { get; set; }
        public string password { get; set; }

        public int connection = -1;
        /* -1 no conection 0 idle 1 recording*/
        private string configPath = @"Utils\config\cam";
        /**
         * Pre: 
         * Post: 
         */
        public Cam(string cam, string _ip1, string _ip2, string _ip3, string _ip4, string user, string password)
        {
            this.cam = cam;
            this.user = user;
            this.password = password;
            ipString = _ip1 + '.' + _ip2 + '.' + _ip3 + '.' + _ip4;
            configPath = configPath + cam;
        }
        public Cam(string cam, string ipString, string user, string password)
        {
            this.cam = cam;
            this.user = user;
            this.password = password;
            this.ipString = ipString;
            configPath = configPath + cam;
        }

        /**
         * Pre : Setup must be 1. Path must exist
         * Post : Create a object of the class with the credentials in credentials file.
         */
        public Cam(string cam) {
            string[] cred = File.ReadAllLines(configPath + @"\credentials");
            this.cam = cam;
            this.ipString = cred[2];
            this.user = cred[0];
            this.password = cred[1];
            this.configPath = configPath + cam;
        }
        public void setCam() {
            try
            {
                File.WriteAllText(configPath + @"\credentials", user + '\n' + password + '\n' + ipString);
                File.WriteAllText(configPath + @"\setup", "1");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (ex is IOException)
                {
                    Console.WriteLine("Ha hagut un error intern torna a intentar");
                    return;
                }
                throw;
            }
        }

        public void test() {
            if (connection != 1)
            {
                Ping p = new Ping();
                PingReply r = p.Send(ipString);
                Console.WriteLine(ipString);
                if (r.Status == IPStatus.Success) connection = 0;
                else connection = -1;
            }
        }
        public void syncTime() {
            DateTime time = DateTime.Now;
            string year = time.Year.ToString();
            string month = time.Month.ToString();
            string day = time.Day.ToString();

            string H = time.Hour.ToString();
            string M = time.Minute.ToString();
            string S = time.Second.ToString();
            string page = "http://" + user + ":" + password + "@" + ipString + "/cgi-bin/global.cgi?action=setCurrentTime&time=" + year + "-" + month + "-" + day + "%20" + H + ":" + M + ":" + S;
            Console.WriteLine(page);
        }
    }
}
