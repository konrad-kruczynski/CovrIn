using System;
using NUnit.Framework;
using CovrIn.Utilities;
using CovrIn.Description;

namespace CovrIn.Tests.Utilities
{
    [TestFixture]
    public class NamespaceTokenizerTests
    {
        [Test]
        public void ShouldTokenizeSimpleNamespace()
        {
            var result = NamespaceTokenizer.Tokenize("Outer.Inner");
            CollectionAssert.AreEqual(new [] { N("Outer"), N("Inner") }, result);
        }

        private NamespaceElement N(string name)
        {
            return new NamespaceElement(NamespaceElementType.Normal, name);
        }


        private NamespaceElement I(string name)
        {
            return new NamespaceElement(NamespaceElementType.Nested, name);
        }
    }
}

