using System;
using CommandLine;
using System.IO;
using Antmicro.Migrant;
using Mono.Cecil;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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
            if(options.InputFiles == null || options.InputFiles.Count == 0)
            {
                Console.Error.WriteLine("You must provide input files or directories.");
                Console.WriteLine(options.GetUsage());
                return;
            }

            var filesToAnalyze = new List<string>();
            foreach(var inputElement in options.InputFiles)
            {
                if(File.Exists(inputElement))
                {
                    filesToAnalyze.Add(inputElement);
                }
                else if(Directory.Exists(inputElement))
                {
                    filesToAnalyze.AddRange(LoadFilesFromDirectory(inputElement));
                }
                else
                {
                    Console.Error.WriteLine("File or directory {0} does not exist.", inputElement);
                    Console.WriteLine(options.GetUsage());
                    return;
                }
            }

            var outputDirectory = PrepareOutputDirectory(options.OutputDirectory);           

            var serializer = new Serializer();
            using(var analysisFileStream = new FileStream(Path.Combine(outputDirectory.FullName, options.Analysis), FileMode.Create))
            {
                using(var serializerStream = serializer.ObtainOpenStreamSerializer(analysisFileStream))
                {
                    foreach(var inputFile in filesToAnalyze)
                    {
                        var assembly = AssemblyDefinition.ReadAssembly(inputFile);

                        var analyzer = new Analyzer();
                        analyzer.Analyze(assembly);

                        var blocks = analyzer.GetBlocks();
                        serializerStream.Serialize(blocks);

                        var decorator = new Decorator(blocks);
                        var decoratedAssembly = decorator.Decorate(assembly);
                        var assemblyTargetPath = Path.Combine(outputDirectory.FullName, inputFile);
                        EnsurePathExists(assemblyTargetPath);
                        decoratedAssembly.Write(assemblyTargetPath);
            
                        if(options.Console)
                        {
                            foreach(var block in blocks)
                            {
                                Console.WriteLine(block);
                            }
                        }
                    }
                }
            }
        }

        private static void EnsurePathExists(string assemblyTargetPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(assemblyTargetPath));
        }

        private static IEnumerable<string> LoadFilesFromDirectory(string inputElement)
        {
            var result = new List<string>();
            var entries = Directory.GetFileSystemEntries(inputElement);
            foreach(var entry in entries)
            {
                var attributes = File.GetAttributes(entry);
                if((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    result.AddRange(LoadFilesFromDirectory(entry));
                }
                else
                {
                    result.Add(entry);
                }
            }
            return result;
        }

        ///Creates the output directory if it does not exist. Referenced libraries are always overwritten.
        ///Output files are always overwritten. Settings file is preserved if it already exists.
        private static DirectoryInfo PrepareOutputDirectory(string outputDirectory)
        {
            var directory = Directory.CreateDirectory(outputDirectory);

            var writerLocation = typeof(Writer.Writer).Assembly.Location;
            var writerFileName = Path.GetFileName(writerLocation);
            File.Copy(writerLocation, Path.Combine(directory.FullName, writerFileName), true);

            var migrantLocation = typeof(Antmicro.Migrant.PrimitiveWriter).Assembly.Location;
            var migrantFileName = Path.GetFileName(migrantLocation);
            File.Copy(migrantLocation, Path.Combine(directory.FullName, migrantFileName), true);

            var settingsFile = Path.Combine(directory.FullName, SettingsFileName);
            if(!File.Exists(settingsFile))
            {
                using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(DefaultSettingsResource))
                {
                    using(var file = new FileStream(settingsFile, FileMode.Create, FileAccess.Write))
                    {
                        resource.CopyTo(file);
                    } 
                }
            }

            return directory;
        }

        private const string SettingsFileName = "covrin.settings";
        private const string DefaultSettingsResource = "default.covrin.settings";
    }
}
