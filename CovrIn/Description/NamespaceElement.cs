using System;

namespace CovrIn.Description
{
    public struct NamespaceElement
    {
        public NamespaceElementType Type { get; private set; }
        public string Name { get; private set; }

        public NamespaceElement(NamespaceElementType type, string name) : this()
        {
            Type = type;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("[NamespaceElement: Type={0}, Name={1}]", Type, Name);
        }
        
    }
}

