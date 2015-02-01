using System;
using CommandLine;
using System.IO;
using Antmicro.Migrant;

namespace CovrIn.Runner
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var options = new Options();
            if(!Parser.Default.ParseArguments(args, options))
            {
                return;
            }
            if(!File.Exists(options.Input))
            {
                Console.WriteLine("Invalid input file name: {0}.", options.Input);
            }
            var analyzer = new Analyzer();
            analyzer.Analyze(options.Input);
            var blocks = analyzer.GetBlocks();
            if(options.Console)
            {
                foreach(var block in blocks)
                {
                    Console.WriteLine(block);
                }
            }
            var serializer = new Serializer();
            serializer.Serialize(blocks, new FileStream(options.Analysis, FileMode.Create));
        }
    }
}
