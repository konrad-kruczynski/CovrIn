using System;
using Mono.Cecil;

namespace CovrIn.Description
{
    public sealed class AssemblyEntry
    {
        public AssemblyEntry(AssemblyDefinition assembly)
        {
            Name = assembly.FullName;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

