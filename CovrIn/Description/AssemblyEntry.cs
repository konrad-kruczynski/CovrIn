using System;

namespace CovrIn.Description
{
    public sealed class AssemblyEntry
    {
        public AssemblyEntry(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}

