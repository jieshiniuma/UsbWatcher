using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using System.Security.Cryptography;

namespace UsbWatcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                
                //查询所有设备的插拔事件
                //Win32_DeviceChangeEvent  Win32_VolumeChangeEvent
                ManagementEventWatcher watcher = new ManagementEventWatcher();
                //WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent  WHERE EventType = 2 or EventType = 3");
                WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent  WHERE EventType = 2");
                //WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
                watcher.EventArrived += (s, e) =>
                {
                    watcher.Stop();
                    exec("adb", "wait-for-device");
                    exec("adb", "reverse tcp:10445 tcp:445");
                    Console.WriteLine("已映射");
                    watcher.Start();


                    var txt = "";
                    foreach (var p in e.NewEvent.Properties)
                    {
                        txt +=  "name " + p.Name + " val " + p.Value + "\r\n" ;
                    }
                    Console.WriteLine(txt);
                    //string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
                    //EventType eventType = (EventType)(Convert.ToInt16(e.NewEvent.Properties["EventType"].Value));
                    //string eventName = Enum.GetName(typeof(EventType), eventType);
                    //Console.WriteLine("{0}: {1} {2}", DateTime.Now, driveName, eventName);
                  

                };
                watcher.Query = query;
                watcher.Start();
                Console.Read();
                //ServicesManager.Instance.StartServices();
                //Thread.CurrentThread.IsBackground = false;
                //Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }

        public static void exec(String file,String args) {
            var process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit(2000);
            string output = process.StandardOutput.ReadToEnd();
            Console.WriteLine(output);
        }
    }
}
