using System;
using Mono.Cecil;
using System.Collections.Generic;
using CovrIn.Description;
using Mono.Cecil.Cil;
using CovrIn.Utilities;

namespace CovrIn
{
    public class Analyzer
    {
        public Analyzer()
        {
            blocks = new Dictionary<long, BlockEntry>();
        }

        public void Analyze(string assemblyPath)
        {
            var assembly = AssemblyDefinition.ReadAssembly(assemblyPath);
            foreach(var module in assembly.Modules)
            {
                foreach(var type in module.Types)
                {
                    AnalyzeType(type);
                }
            }
        }

        public IReadOnlyDictionary<long, BlockEntry> GetBlocks()
        {
            return blocks;
        }

        private void AnalyzeType(TypeDefinition type)
        {
            // we're only interested in methods
            foreach(var method in type.Methods)
            {
                var intervalCollection = new IntervalCollection();
                AnalyzeBlockFrom(method, method.Body.Instructions[0], intervalCollection);
                foreach(var interval in intervalCollection)
                {
                    blocks.Add(nextBlockId++, new BlockEntry(new AssemblyEntry(method.Module.Assembly.FullName),
                        method.Module.ToString(), method.ToString(), interval.Start, interval.Length));
                }
            }
        }

        private void AnalyzeBlockFrom(MethodDefinition method, Instruction instruction, IntervalCollection methodBlocks)
        {
            int nextIntervalStart;

            var startingOffset = instruction.Offset;
            // if our entry point can be found in some existing block
            // we have to divide that block
            if(methodBlocks.DivideIfNecessary(startingOffset, out nextIntervalStart))
            {
                return;
            }
            if(nextIntervalStart == -1)
            {
                nextIntervalStart = int.MaxValue;
            }

            while(NextInstructionShouldBeReached(instruction.OpCode.FlowControl)
                && (instruction.Offset + instruction.GetSize()) < nextIntervalStart)
            {
                instruction = instruction.Next;
                if(instruction == null)
                {
                    throw new InvalidOperationException("Invalid IL code.");
                }
            }

            var instructionSize = instruction.GetSize();
            methodBlocks.Insert(startingOffset, instruction.Offset - startingOffset + instructionSize);

            switch(instruction.OpCode.FlowControl)
            {
            case FlowControl.Return:
            case FlowControl.Throw:
                return;
            case FlowControl.Branch:
                AnalyzeBlockFrom(method, ((Instruction)instruction.Operand), methodBlocks);
                break;
            case FlowControl.Cond_Branch:
                AnalyzeBlockFrom(method, ((Instruction)instruction.Operand), methodBlocks);
                AnalyzeBlockFrom(method, instruction.Next, methodBlocks);
                break;
            case FlowControl.Break:
            case FlowControl.Meta:
            case FlowControl.Phi:
                throw new NotImplementedException();
            case FlowControl.Call:
            case FlowControl.Next:
                // we've reached here due to the nextIntervalStart that finished our scan prematurely
                return;
            }
        }

        private static bool NextInstructionShouldBeReached(FlowControl flowControl)
        {
            return flowControl == FlowControl.Call || flowControl == FlowControl.Next;
        }

        private long nextBlockId;
        private readonly Dictionary<long, BlockEntry> blocks;
    }
}

