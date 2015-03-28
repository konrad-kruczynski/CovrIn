using System;
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace CovrIn.Runner
{
    internal class Options
    {
        [Option('o', "outputDirectory", DefaultValue = "build", HelpText = "Directory name for output binaries.")]
        public string OutputDirectory { get; set; }

        [Option('a', "analysisFile", DefaultValue = "covrin.analysis", HelpText = "File name of the output analysis file.")]
        public string Analysis { get; set; }

        [Option('c', "console", DefaultValue = false, HelpText = "Enable console output.")]
        public bool Console { get; set; }

        [Option('s', "settingsFile", DefaultValue = "covrin.settings", HelpText = "File name forgenerated  settings file.")]
        public string SettingsFile { get; set; }

        [ValueList(typeof(List<string>))]
        public IList<string> InputFiles { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText 
            {
                Heading = new HeadingInfo("CovrIn Runner", "0.1"),
                Copyright = new CopyrightInfo("Konrad M. Kruczyński, Piotr Zierhoffer", 2015),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("");
            help.AddPreOptionsLine("Usage: Runner.exe [-o OutputDirectory] [-a AnalysisFileName] [-s SettingsFileName] AssemblyFileOrDirectory...");
            help.AddOptions(this);
            return help;        
        }
    }

}

