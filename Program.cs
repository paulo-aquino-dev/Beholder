using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static int SW_SHOW = 5;
        static int SW_HIDE = 0;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        public static void Main(string[] args)
        {
            IntPtr myWindow = GetConsoleWindow();
            ShowWindow(myWindow, SW_HIDE);

            // Create a Timer object that knows to call our TimerCallback
            // method once every 2000 milliseconds.
            Timer t = new Timer(TimerCallback, null, 0, 2000);
            // Wait for the user to hit <Enter>
            Console.ReadLine();
        }
        
        private static void TimerCallback(Object o)
        {
            //List all processes with names similar to WORD

            //List all processes with names similar to WINZIP
            SearchProcesses("WinoverDesktop");
            //List all processes with names similar to ADMIN

            Console.ReadLine();
            GC.Collect();
        }

        public static void SearchProcesses(string processName)
        {
            try
            {
                Console.WriteLine("*****************************************************************************");
                Console.WriteLine("");
                Console.WriteLine("PROCURANDO O PROCESSO COM O NOME: {0}", processName);
                Console.WriteLine("");
                //initialize the select query with command text
                SelectQuery query = new SelectQuery(@"SELECT * FROM Win32_Process where Name LIKE '%"+ processName + "%'");
                //initialize the searcher with the query it is
                //supposed to execute
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    //execute the query
                    ManagementObjectCollection processes = searcher.Get();
                    if (processes.Count <= 0)
                    {
                        Console.WriteLine("O PROCESSO ESTA ENCERRADO: {0}", processName);
                        try
                        {
                            Console.WriteLine("");
                            Console.WriteLine("***************INICIANDO O SISTEMA****************");
                            Console.WriteLine("");
                            System.Diagnostics.Process.Start("C:\\winover\\WinoverDesktop\\bin\\WinoverDesktop.exe");
                            Process.Start("taskkill", "/F /IM explorer.exe");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao iniciar o sistema: {0}", ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("O PROCESSO ESTÁ RODANDO: {0}", processName);
                        foreach (ManagementObject process in processes)
                        {
                            //print process properties
                            process.Get();
                            PropertyDataCollection processProperties = process.Properties;
                            Console.WriteLine("ProcessID:{0}\tCommandLine:{1}",
                            processProperties["ProcessID"].Value,
                            processProperties["CommandLine"].Value);
                        }
                    }
                }
                Console.WriteLine("");
                Console.WriteLine("***************BUSCA COMPLETA****************");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while executing the query: {0}",   ex.Message);
            }
        }
    }

}

