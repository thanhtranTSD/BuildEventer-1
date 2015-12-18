/*
<License>
Copyright 2015 Virtium Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http ://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</License>
 */

using BuildEventerDAL;
using BuildEventerDAL.Data;
using CommandLine;
using PostBuild.Action;
using PostBuild.Class;
using PostBuild.Factories;
using System;
using System.Collections.Generic;
using System.IO;

namespace PostBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineOption options = new CommandLineOption();

            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings
            {
                MutuallyExclusive = true,
                CaseSensitive = true,
                HelpWriter = Console.Error
            });

            bool success = parser.ParseArguments(args, options);

            if (true == success)
            {
                Run(options.FileName, options.Overwrite);
            }
            else
            {
                if (args.Length != 0 && args[0] != "-h")
                {
                    NotifyAndExit("Input arguments are invalid. Type PostBuild.exe -h for help. Press any key to exit.");
                }
            }
        }

        #region Private Functions
        private static void Initialze()
        {
            ActionFactory.Instance().Initialize();
        }

        private static void NotifyAndExit(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(0);
        }

        private static void Run(string configFileName, bool overwrite = false)
        {
            if (false == File.Exists(configFileName))
            {
                NotifyAndExit(String.Format("{0} does not exist. Press any key to exit.", configFileName));
            }
            Initialze();

            List<IActionData> actionsData = DataAccessLayer.LoadFile(configFileName, true);
            ActionManager.ExecuteAllActions(actionsData, overwrite);
            Console.WriteLine("PRESS ANY KEY TO EXIT.");
            Console.ReadLine();
        }
        #endregion
    }
}
