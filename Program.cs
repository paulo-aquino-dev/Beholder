using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Beholder
{
    class Program
    {
        static int SW_HIDE = 0;

        static int i;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        public static void Main()
        {
            IntPtr myWindow = GetConsoleWindow();
            ShowWindow(myWindow, SW_HIDE);
            Timer t = new Timer(TimerCallback, null, 0, 60000);
            Console.ReadLine();
        }
        private static void ajustaAudio()
        {
            CoreAudioDevice fone = new CoreAudioController().DefaultPlaybackDevice;
            CoreAudioDevice mic = new CoreAudioController().DefaultCaptureDevice;
            fone.Volume = 100;
            mic.Volume = 100;
        }
        private static void reiniciar()
        {
            if (i.Equals(10))
            {
                var fileName = Assembly.GetExecutingAssembly().Location;
                System.Diagnostics.Process.Start(fileName);
                Environment.Exit(0);
            }

        }

        private static void TimerCallback(Object o)
        {
            i += 1;
            Console.WriteLine(i);
            SearchProcesses("WinoverDesktop");
            Console.ReadLine();
            GC.Collect();

        }
        public static void SearchProcesses(string processName)
        {
            try
            {
                SelectQuery query = new SelectQuery(@"SELECT * FROM Win32_Process where Name LIKE '%" + processName + "%'");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    //execute the query
                    ManagementObjectCollection processes = searcher.Get();
                    if (processes.Count <= 0)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start("C:\\winover\\WinoverDesktop\\bin\\WinoverDesktop.exe");
                            ///Process.Start("taskkill", "/F /IM explorer.exe");
                            reiniciar();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao iniciar o sistema: {0}", ex.Message);
                            reiniciar();
                        }
                    }
                    else
                    {
                        foreach (ManagementObject process in processes)
                        {
                            //print process properties
                            process.Get();
                            PropertyDataCollection processProperties = process.Properties;
                            //Console.WriteLine("ProcessID:{0}\tCommandLine:{1}",
                            //processProperties["ProcessID"].Value,
                            //processProperties["CommandLine"].Value);
                            ajustaAudio();
                            //Console.WriteLine(t)
                            reiniciar();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while executing the query: {0}", ex.Message);
                reiniciar();
            }
        }
    }
}