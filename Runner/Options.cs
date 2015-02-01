using System;
using CommandLine;
using CommandLine.Text;

namespace CovrIn.Runner
{
    internal class Options
    {
        [Option('o', "outputFile", DefaultValue = "CovrProject", HelpText = "File name of the result assembly.")]
        public string Output { get; set; }

        [Option('a', "analysisFile", DefaultValue = "covrin.analysis", HelpText = "File name of the output analysis file.")]
        public string Analysis { get; set; }

        [Option('i', "inputFile", Required = true, HelpText = "File name of the input assembly.")]
        public string Input { get; set; }

        [Option('c', "console", DefaultValue = false, HelpText = "Enable console output.")]
        public bool Console { get; set; }

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
            help.AddPreOptionsLine("Usage: Runner.exe -i InputFile [-o OutputFile] [-a AnalysisFile]");
            help.AddOptions(this);
            return help;
        
        }
    }

}

