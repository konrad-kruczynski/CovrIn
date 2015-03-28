using System;
using Antmicro.Migrant;
using System.IO;

namespace CovrInWriter
{
    public static class Writer
    {
        static Writer()
        {
            var filename = "covrin.report.log"; //TODO: Add option
            var fileMode = FileMode.Append; //TODO: Add option
            fileStream = File.Open(filename, fileMode);
            writer = new PrimitiveWriter(fileStream, false);
        }

        public static void Write(long blockId)
        {
            writer.Write(blockId);
            //TODO: Add option to flush
        }

        private readonly static FileStream fileStream;
        private readonly static PrimitiveWriter writer;
    }
}

