using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomPath
{
    class CustomP
    {
        public CustomP() { }
        public string createDirectoryCamDay(string cam, string __recordPath) {
            string aux = "cam" + cam;
            __recordPath = __recordPath + @"\" + aux;
            bool exists = System.IO.Directory.Exists(__recordPath);

            if (!exists)
            {
                Console.WriteLine("Creating " + __recordPath);
                System.IO.Directory.CreateDirectory(__recordPath);
            }
            /*
            int __Day = DateTime.Now.Day;
            int __Month = DateTime.Now.Month;
            int __Year = DateTime.Now.Year;
            string __dayPath = __recordPath + @"\";
            string __day = __Day.ToString();
            string __month = __Month.ToString();
            if (__Day < 10) __day = "0" + __day;
            if (__Month < 10) __month = "0" + __month;

            __dayPath += __day + __month + (__Year % 100).ToString();

            exists = System.IO.Directory.Exists(__dayPath);

            if (!exists)
            {
                Console.WriteLine("Creating " + __dayPath);
                System.IO.Directory.CreateDirectory(__dayPath);
            }
    */
            return __recordPath;
        }
        public void checkStorage(string __recordPath) {
            DirectoryInfo dInfo = new DirectoryInfo(__recordPath);
            long sizeOfDir = DirectorySize(dInfo, true);
            Console.WriteLine("Directory size in MB : " +
            "{0:N2} GB", ((double)sizeOfDir) / (1024 * 1024* 1024));
            if(sizeOfDir/(1024 * 1024 * 1024) > 120){

            }
        }

        public string newDay(TimeSpan a) {

            return "a";
        }

        private long DirectorySize(DirectoryInfo dInfo, bool includeSubDir)
        {
            long totalSize = dInfo.EnumerateFiles().Sum(file => file.Length);
            if (includeSubDir)
            {
                totalSize += dInfo.EnumerateDirectories().Sum(dir => DirectorySize(dir, true));
            }
            return totalSize;
        }
    }
}
