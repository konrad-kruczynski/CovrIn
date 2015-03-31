using System;
using Antmicro.Migrant;
using System.IO;
using CovrIn.Writer.Utilities;

namespace CovrIn.Writer
{
    public static class Writer
    {
        static Writer()
        {
            sync = new object();
            var filename = SimpleIniParser.ParseString(SettingsFileName, "traceFileName", DefaultOutputName);
            var fileMode = SimpleIniParser.ParseBool(SettingsFileName, "appendResults", true) ? FileMode.Append : FileMode.Create;
            fileStream = File.Open(filename, fileMode);
            writer = new PrimitiveWriter(fileStream, false);
            alwaysFlush = SimpleIniParser.ParseBool("covrin.settings", "alwaysFlush", false);
        }

        public static void Write(long blockId)
        {
            lock(sync)
            {
                writer.Write(blockId);
                if(alwaysFlush)
                {
                    fileStream.Flush();
                }
            }
        }

        private readonly static FileStream fileStream;
        private readonly static PrimitiveWriter writer;
        private readonly static bool alwaysFlush;
        private readonly static object sync;

        private const string SettingsFileName = "covrin.settings";
        private const string DefaultOutputName = "covrin.report.log";
    }
}

