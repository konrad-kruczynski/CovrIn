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

        [Test]
        public void ShouldTokenizeNestedClass()
        {
            var result = NamespaceTokenizer.Tokenize("Normal/Nested");
            CollectionAssert.AreEqual(new [] { N("Normal"), I("Nested") }, result);
        }

        [Test]
        public void ShouldTokenizeMixed()
        {
            var result = NamespaceTokenizer.Tokenize("A.B/C");
            CollectionAssert.AreEqual(new [] { N("A"), N("B"), I("C") }, result);
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

