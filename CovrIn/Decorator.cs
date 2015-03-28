﻿using System;
using Mono.Cecil;
using System.Collections.Generic;
using CovrIn.Description;
using System.Linq;
using Mono.Cecil.Cil;
using System.Reflection;
using System.Linq.Expressions;

namespace CovrIn
{
    public class Decorator
    {
        public Decorator(IReadOnlyDictionary<long, BlockEntry> blockEntries)
        {
            this.blockEntries = blockEntries;        
        }

        public void Decorate(AssemblyDefinition assembly, string outputFileName)
        {
            var tokensToMethods = assembly.Modules.SelectMany(x => x.Types).SelectMany(x => x.Methods).ToDictionary(x => x.MetadataToken.ToInt32());

            var blocksInAssembly = blockEntries.Where(x => x.Value.Assembly.Name == assembly.FullName);
                        
            foreach(var blocksInModule in blocksInAssembly.GroupBy(x=>x.Value.Module))
            {
                DecorateModule(assembly, tokensToMethods, blocksInModule);
            }

            assembly.Write(outputFileName);
        }

        private void DecorateModule(AssemblyDefinition assembly, Dictionary<int, MethodDefinition> tokensToMethods, IEnumerable<KeyValuePair<long, BlockEntry>> blocks)
        {
            var blocksByMethod = blocks.GroupBy(x => x.Value.Method); //TODO: After #14 rework to look at specific type (method will not be unique anymore)
            foreach(var blocksInMethod in blocksByMethod)
            {
                var orderedBlocks = blocksInMethod.OrderByDescending(x => x.Value.StartingILOffset);
                var method = tokensToMethods[blocksInMethod.First().Value.Token];

                var writeMethod = method.DeclaringType.Module.Import(WriteMethodInfo);

                var processor = method.Body.GetILProcessor();

                var offsetToInstruction = method.Body.Instructions.ToDictionary(x => x.Offset);

                foreach(var block in orderedBlocks)
                {
                    var id = block.Key;

                    var firstBlockInstruction = offsetToInstruction[block.Value.StartingILOffset];
                    Console.WriteLine(block.Value.StartingILOffset);
                    Console.WriteLine(block.Value.ToString());
                    processor.InsertBefore(firstBlockInstruction,
                        Instruction.Create(OpCodes.Ldc_I8, id));
                    processor.InsertBefore(firstBlockInstruction,
                        Instruction.Create(OpCodes.Call, writeMethod));
                }
            }
        }

        private static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            var methodCall = (MethodCallExpression)expression.Body;
            return methodCall.Method;
        }

        private readonly IReadOnlyDictionary<long, BlockEntry> blockEntries;

        private static MethodInfo WriteMethodInfo = GetMethodInfo(() => Writer.Writer.Write(0));
    }
}