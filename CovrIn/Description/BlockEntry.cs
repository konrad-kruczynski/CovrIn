using System;

namespace CovrIn.Description
{
    public sealed class BlockEntry
    {
        public BlockEntry(AssemblyEntry assembly, string module, string method, int startingILOffset, int length)
        {
            Assembly = assembly;
            Module = module;
            Method = method;
            StartingILOffset = startingILOffset;
            Length = length;
        }

        public AssemblyEntry Assembly { get; private set; }

        public string Module { get; private set; }

        public string Method { get; private set; }

        public int StartingILOffset { get; private set; }

        public int Length { get; private set; }

        public override string ToString()
        {
            return string.Format("[BlockEntry: Assembly={0}, Module={1}, Method={2}, ILOffset=<0x{3:X}, 0x{4:X}>]",
                Assembly, Module, Method, StartingILOffset, StartingILOffset + Length);
        }
    }
}

