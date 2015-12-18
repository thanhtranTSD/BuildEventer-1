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

using CommandLine;
using CommandLine.Text;

namespace PostBuild.Class
{
    public class CommandLineOption
    {
        [Option("f", "file", Required = true, HelpText = "Input the XML actions configuration file name.")]
        public string FileName { get; set; }

        [Option("o", "overwrite", DefaultValue = false, HelpText = "Overwrite if the files already exist in the destination folder. Default does not overwrite.")]
        public bool Overwrite { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            HelpText help = new HelpText
            {
                Heading = new HeadingInfo(AssemblyInfoExtent.Title, AssemblyInfoExtent.Version),
                Copyright = AssemblyInfoExtent.Copyright + " " + AssemblyInfoExtent.Company,
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            help.AddPreOptionsLine("Usage:");
            help.AddOptions(this);
            return help;
        }
    }
}
