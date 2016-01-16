using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using WfdbCsharpWrapper;

namespace EKG_Project.IO
{
    public class WFDBLoader
    {
        private readonly string _oldEnvironmentVariables;

        private WFDBLoader()
        {
            _oldEnvironmentVariables = Environment.GetEnvironmentVariable("Path");
        }

        private void AddToPath(IECGPath pathBuilder)
        {
            var environmentVariableName = "PATH";
            var valueToAdd = Path.Combine(pathBuilder.getBasePath(), "DLL");
            var finalValue = _oldEnvironmentVariables + ";" + valueToAdd;
            Console.WriteLine(finalValue);
            var target = EnvironmentVariableTarget.Machine;
            Environment.SetEnvironmentVariable(environmentVariableName, finalValue, target);
        }

        private void RemoveFromPath()
        {
            var environmentVariableName = "PATH";
            var target = EnvironmentVariableTarget.Machine;
            Environment.SetEnvironmentVariable(environmentVariableName, _oldEnvironmentVariables, target);
        }

        private void RestartExplorer(IECGPath pathBuilder)
        {
            foreach (Process exe in Process.GetProcesses())
            {
                if (exe.ProcessName == "explorer")
                    exe.Kill();
            }
            string path = Path.Combine(pathBuilder.getBasePath(), "DLL\\");
            path = path.Substring(0, path.Length - 1);
            Process cmd = new Process();
            cmd.StartInfo.FileName = @"cmd.exe";
            cmd.StartInfo.Arguments = @"/K " + path + @"/resetvars.bat";
            cmd.Start();
            cmd.WaitForExit();
            Process.Start(Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"));
        }

        /// <summary>
        /// Metoda ta listuje wszystkie zmienne środowiskowe
        /// </summary>
        private static void EnvironmentalVariablesCheck()
        {
            Console.WriteLine("GetEnvironmentVariables: ");
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine("  {0} = {1}", de.Key, de.Value);
            }
        }

        /// <summary>
        /// Metoda ta listuje tylko 1 wskazaną zmienną środowiskową
        /// Np. Path
        /// </summary>
        /// <param name="name">Nazwa zmiennej środowiskowej</param>
        private static void EnvironmentalVariableCheck(string name)
        {
            Console.WriteLine("GetEnvironmentVariable " + name + ": ");
            Console.WriteLine(Environment.GetEnvironmentVariable(name));
        }

        public static void Main()
        {
            IECGPath pathBuilder = new DebugECGPath();
            Console.WriteLine(Path.Combine(pathBuilder.getBasePath(), "DLL\\"));

            WFDBLoader wfdbinstance = new WFDBLoader();
            wfdbinstance.AddToPath(pathBuilder);
            wfdbinstance.RestartExplorer(pathBuilder);
            WFDBLoader.EnvironmentalVariableCheck("Path");

            //Assembly.LoadFrom(Path.Combine(pathBuilder.getBasePath(), "DLL", "wfdb.dll"));

            string datFileName = "100.dat";
            string heaFileName = "100.hea";
            string atrFileName = "100.atr";
            string recordName = "100";
            string directory = pathBuilder.getDataPath();

            Console.WriteLine(".dat file exists: " + File.Exists(Path.Combine(directory, datFileName)));
            Console.WriteLine(".hea file exists: " + File.Exists(Path.Combine(directory, heaFileName)));
            Console.WriteLine(".atr file exists: " + File.Exists(Path.Combine(directory, atrFileName)));

            wfdbinstance.RemoveFromPath();
            wfdbinstance.RestartExplorer(pathBuilder);
            WFDBLoader.EnvironmentalVariableCheck("Path");
            Console.ReadLine();
        }
    }
}