using System;
using Mono.Cecil;
using System.Collections.Generic;
using CovrIn.Utilities;

namespace CovrIn.Description
{
    public sealed class BlockEntry
    {
        public BlockEntry(AssemblyEntry assembly, ModuleDefinition module, MethodDefinition method, int startingILOffset, int length, int token)
        {
            Assembly = assembly;
            Module = module.ToString();
            Namespace = NamespaceTokenizer.Tokenize(method.DeclaringType.Namespace);
            Type = method.DeclaringType.Name;
            Method = method.Name;
            StartingILOffset = startingILOffset;
            Length = length;
            Token = token;
        }

        public AssemblyEntry Assembly { get; private set; }

        public int Token { get; private set; }

        public string Module { get; private set; }

        public IReadOnlyCollection<NamespaceElement> Namespace { get; private set; }

        public string Type { get; private set; }

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

