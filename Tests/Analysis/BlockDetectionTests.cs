using System;
using NUnit.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using CovrIn.Description;
using System.Linq;

namespace CovrIn.Tests.Analysis
{
    public class BlockDetectionTests
    {
        [Test]
        public void ShouldDetectOneBlockInEmptyMethod()
        {
            var blocks = CreateAndAnalyzeAssembly(Instruction.Create(OpCodes.Ret));
            Assert.AreEqual(1, blocks.Count);
        }

        [Test]
        public void ShouldDetectOneBlockInSimpleMethod()
        {
            /*
            var i = 5;
            i += 5;
            return;
            */
            var blocks = CreateAndAnalyzeAssembly(
                             Instruction.Create(OpCodes.Ldc_I4_5),
                             Instruction.Create(OpCodes.Stloc_0),
                             Instruction.Create(OpCodes.Ldloc_0),
                             Instruction.Create(OpCodes.Ldc_I4_5),
                             Instruction.Create(OpCodes.Add),
                             Instruction.Create(OpCodes.Stloc_0),
                             Instruction.Create(OpCodes.Ret)
                         );
            Assert.AreEqual(1, blocks.Count);
        }

        [Test]
        public void ShouldDetectOneBlockInFakeJumps()
        {
            var ret = Instruction.Create(OpCodes.Ret);
            var blocks = CreateAndAnalyzeAssembly(
                             Instruction.Create(OpCodes.Br, ret),
                             ret
                         );
            Assert.AreEqual(1, blocks.Count);
        }

        [Test]
        public void ShouldDetectThreeBlocksOnIf()
        {
            /*
            var i = 5; 
            if(i > 0)
            {
                i -= 1;
            }
            */
            var ret = Instruction.Create(OpCodes.Ret);
            var blocks = CreateAndAnalyzeAssembly(
                             Instruction.Create(OpCodes.Ldc_I4_5),
                             Instruction.Create(OpCodes.Stloc_0),
                             Instruction.Create(OpCodes.Ldloc_0),
                             Instruction.Create(OpCodes.Ldc_I4_0),
                             Instruction.Create(OpCodes.Ble, ret),
                             Instruction.Create(OpCodes.Ldloc_0),
                             Instruction.Create(OpCodes.Ldc_I4_1),
                             Instruction.Create(OpCodes.Sub),
                             Instruction.Create(OpCodes.Stloc_0),
                             ret
                         );
         
            Assert.AreEqual(3, blocks.Count);
        }

        [Test]
        public void ShouldDetectFourBlocksOnIfElse()
        {
            /*
            var i = 5;
            if(i > 0)
            {
                i -= 1;
            }
            else
            {
                i -= 2;
            }
            */
            var @else = Instruction.Create(OpCodes.Nop);
            var ret = Instruction.Create(OpCodes.Ret);
            var blocks = CreateAndAnalyzeAssembly(
                             Instruction.Create(OpCodes.Ldc_I4_5),
                             Instruction.Create(OpCodes.Stloc_0),
                             Instruction.Create(OpCodes.Ldloc_0),
                             Instruction.Create(OpCodes.Ldc_I4_0),
                             Instruction.Create(OpCodes.Ble, @else),
                             Instruction.Create(OpCodes.Ldloc_0),
                             Instruction.Create(OpCodes.Ldc_I4_1),
                             Instruction.Create(OpCodes.Sub),
                             Instruction.Create(OpCodes.Stloc_0),
                             Instruction.Create(OpCodes.Br, ret),
                             @else,
                             Instruction.Create(OpCodes.Ldloc_0),
                             Instruction.Create(OpCodes.Ldc_I4_2),
                             Instruction.Create(OpCodes.Sub),
                             Instruction.Create(OpCodes.Stloc_0),
                             ret
                         );

            Assert.AreEqual(4, blocks.Count);
        }

        private void VerifyAnalysisResults(IEnumerable<BlockEntry> blocks, IEnumerable<Instruction> instructions, params int[] boundaries)
        {
            var i = 0;
            var pairs = boundaries.GroupBy(x => i++ / 2).Select(x=>x.ToArray()).ToList();
            Assert.AreEqual(blocks.Count(), pairs.Count());

            var blocksSorted = blocks.OrderBy(x => x.StartingILOffset).ToArray();

            for(i = 0; i < pairs.Count(); ++i)
            {
                Assert.AreEqual(pairs[i][0], blocksSorted[i].StartingILOffset);
                Assert.AreEqual(pairs[i][1], blocksSorted[i].StartingILOffset + blocksSorted[i].Length);
            }
        }

        private IReadOnlyDictionary<long, BlockEntry> CreateAndAnalyzeAssembly(params Instruction[] instructions)
        {
            var assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition("CovrInTest", new Version(1, 0, 0, 0)), "module", ModuleKind.Dll);
            var type = new TypeDefinition("CovrInTest", "TestType", TypeAttributes.Public | TypeAttributes.Class);
            var method = new MethodDefinition("TestMethod", MethodAttributes.Public, assembly.Modules[0].TypeSystem.Int32);
            foreach(var instruction in instructions)
            {
                method.Body.Instructions.Add(instruction);
            }
            type.Methods.Add(method);
            assembly.Modules[0].Types.Add(type);

            var analyzer = new Analyzer();
            analyzer.Analyze(assembly);
            var ret = analyzer.GetBlocks();
            return ret;
        }
    }
}