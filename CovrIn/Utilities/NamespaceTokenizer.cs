using System;
using System.Collections.Generic;
using CovrIn.Description;
using System.Linq;

namespace CovrIn.Utilities
{
    public static class NamespaceTokenizer
    {
        public static IReadOnlyCollection<NamespaceElement> Tokenize(string namespaceAsString)
        {
            var result = new List<NamespaceElement>();

            var nextIsNormal = true;
            var lastIndex = 0;
            while(lastIndex < namespaceAsString.Length)
            {
                var dotPosition = namespaceAsString.IndexOf('.', lastIndex);
                var slashPosition = namespaceAsString.IndexOf('/', lastIndex);
                Minus1ToMaxInt(ref dotPosition);
                Minus1ToMaxInt(ref slashPosition);
                var elementType = nextIsNormal ? NamespaceElementType.Normal : NamespaceElementType.Nested;
                var startIndex = lastIndex;
                if(dotPosition < slashPosition)
                {
                    nextIsNormal = true;
                    lastIndex = dotPosition;
                }
                else if(slashPosition < dotPosition)
                {
                    nextIsNormal = false;
                    lastIndex = slashPosition;
                }
                else
                {
                    // if they are equal, nothing was found
                    lastIndex = namespaceAsString.Length;
                }
                lastIndex++;
                result.Add(new NamespaceElement(elementType, namespaceAsString.Substring(startIndex, lastIndex - startIndex - 1)));
            }
            return result;
        }

        private static void Minus1ToMaxInt(ref int value)
        {
            if(value == -1)
            {
                value = int.MaxValue;
            }
        }
    }
}

