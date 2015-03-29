using System;
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace CovrIn.Runner
{
    internal class Options
    {
        [Option('o', "outputDirectory", DefaultValue = "covrin.output", HelpText = "Directory name for output binaries.")]
        public string OutputDirectory { get; set; }

        [Option('a', "analysisFile", DefaultValue = "covrin.analysis", HelpText = "File name of the output analysis file.")]
        public string Analysis { get; set; }

        [Option('c', "console", DefaultValue = false, HelpText = "Enable console output.")]
        public bool Console { get; set; }

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
            help.AddPreOptionsLine("Usage: Runner.exe [-o OutputDirectory] [-a AnalysisFileName] AssemblyFileOrDirectory...");
            help.AddOptions(this);
            return help;        
        }
    }

}

