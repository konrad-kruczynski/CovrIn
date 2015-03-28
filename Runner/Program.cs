using System;
using CommandLine;
using System.IO;
using Antmicro.Migrant;
using Mono.Cecil;

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

            var assembly = AssemblyDefinition.ReadAssembly(options.Input);

            var analyzer = new Analyzer();
            analyzer.Analyze(assembly);

            var blocks = analyzer.GetBlocks();

            var decorator = new Decorator(blocks);
            decorator.Decorate(assembly, Path.Combine(options.OutputDirectory ?? "build", options.Input));

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
