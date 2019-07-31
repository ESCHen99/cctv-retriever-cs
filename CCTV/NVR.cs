using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CCTV;
using CustomPath;
using System.IO;

namespace CCTV
{
    public class NVR
    {
        public static int NVRMaxCams = 4;
        private Timer timerGlobal;
        private Timer timerPing;
        private Process[] ffmpeg;
        private bool first = true;
        private bool recording = false;

        public Cam[] camList;
        public string recordPath;
        public int[] startTime = new int[]{ 0, 0};
        public int[] endTime = new int[]{23, 59};
        public long fileSize = 0;

        public NVR() {
            ffmpeg = new Process[NVRMaxCams];
            camList = new Cam[NVRMaxCams];
            recordPath = null;
            fileSize = 10000000;
        }

        public void changeRecordPath(string recordPath) {
            this.recordPath = recordPath;
        }

        public bool isConfigured() {
            if (recordPath == null) return false;
            if (fileSize == 0) return false;
            return true;
        }
        public void addUpdate(Cam cam) {
            int camInt = Int16.Parse(cam.cam);
            camList[camInt-1] = cam;
        }

        /*
        public NVR()
        {
            string path = @"Test\config\cam";
            string setupBit;
            for (int i = 1; i <= 4; ++i)
            {
                if (File.Exists(path + i.ToString() + @"\setup") || File.Exists(path + i.ToString() + @"\credentials"))
                {
                    setupBit = File.ReadAllText(path + i.ToString() + @"\setup");
                    if (setupBit[0].Equals('1'))
                    {
                        camList.Add(new Cam(i.ToString(), path));
                    }
                }
                else Console.WriteLine("No existeix FILE");
            }
            this.recordPath = null;
            fileSize = 10000000000;
        }
        */
        public void start() {
            timerGlobal = new Timer(check, null, 0, 1000 * 60);
        }

        public void check(object state) {
            TimeSpan startTimeSpan = new TimeSpan(startTime[0], startTime[1], 0);
            TimeSpan endTimeSpan = new TimeSpan(endTime[0], endTime[1], 0);

            if (TimeSpan.Compare(DateTime.Now.TimeOfDay, startTimeSpan) == 0 || (TimeSpan.Compare(DateTime.Now.TimeOfDay, startTimeSpan) == 1 && first)) {
                first = false;
                for (int i = 0; i < NVRMaxCams; ++i) {
                    iRecord(ref camList[i], 120.ToString(), ref ffmpeg[i]);
                }
                recording = true;
            } // Time to start;
            if (TimeSpan.Compare(DateTime.Now.TimeOfDay, endTimeSpan) == 0 && recording) {
                for (int i = 0; i < NVRMaxCams; ++i) {
                    iStop(ref camList[i], ref ffmpeg[i]);
                }
                recording = false;
            } // Time to stop;
            if (TimeSpan.Compare(DateTime.Now.TimeOfDay, new TimeSpan(23, 59, 59)) == 0) {
                
            } // New day
            //check storage
            new CustomP().checkStorage(recordPath);
        }


        public void reset() {
            timerGlobal.Dispose();
            first = true;
            for (int i = 0; i < NVRMaxCams; ++i)
            {
                if(recording)
                iStop(ref camList[i], ref ffmpeg[i]);
            }
            recording = false;
        }
        private void iRecord(ref Cam cam, string interval, ref Process _ffmpeg)
        {
            string user = cam.user;
            string password = cam.password;
            string ipString = cam.ipString;
            if (cam.connection != -1)
            {
                string __dayPath = new CustomP().createDirectoryCamDay(cam.cam, recordPath);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "ffmpeg.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                string __rtsp = "rtsp://" + user + ":" + password + "@" + ipString;
                startInfo.Arguments = "-rtsp_transport tcp -i \"" + __rtsp + "\" -f segment -segment_time " + interval + " -segment_format avi -reset_timestamps 1 -strftime 1 -c  copy -map 0 " + __dayPath + "\\%d%m%Y-%H%M%S.avi";
                try
                {
                    _ffmpeg = Process.Start(startInfo);
                    cam.connection = 1;
                    //  StreamReader stdErr = ffmpeg.StandardError;
                    //  Console.WriteLine(stdErr);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
               
            }
            else Console.WriteLine("Cammera not connected " + cam.connection);
        }
        private void iStop(ref Cam cam, ref Process ffmpeg)
        {
            try
            {
                if (ffmpeg != null)
                {
                    ffmpeg.CloseMainWindow();
                    ffmpeg.Close();
                    cam.connection = 0;
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }

        public void updateSchedule() {

        }

        public async Task<int> ping() {
            Task[] aux = new Task[NVRMaxCams];
            for (int i = 0; i < NVRMaxCams; ++i) {
                aux[i] = new Task(camList[i].test);
                aux[i].Start();
            }
            for (int i = 0; i < NVRMaxCams; ++i) {
                await aux[i];
            }
            return 1;
        }
    }

}
