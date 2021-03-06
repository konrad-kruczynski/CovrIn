﻿using System;
using Mono.Cecil;
using System.Collections.Generic;
using CovrIn.Description;
using Mono.Cecil.Cil;
using CovrIn.Utilities;
using System.Linq;

namespace CovrIn
{
    public class Analyzer
    {
        public Analyzer()
        {
            blocks = new Dictionary<long, BlockEntry>();
        }

        public void Analyze(AssemblyDefinition assembly)
        {            
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
            foreach(var innerType in type.NestedTypes)
            {
                AnalyzeType(innerType);
            }
            // we're only interested in methods that are not abstract
            foreach(var method in type.Methods.Where(x => x.HasBody))
            {
                var intervalCollection = new IntervalCollection();
                AnalyzeBlockFrom(method, method.Body.Instructions[0], intervalCollection);
                foreach(var interval in intervalCollection)
                {
                    blocks.Add(nextBlockId++, new BlockEntry(new AssemblyEntry(method.Module.Assembly),
                        method.Module, method, interval.Start, interval.Length, method.MetadataToken.ToInt32()));
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

            while(NextInstructionShouldBeReached(instruction)
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
                var operandAsInstruction = instruction.Operand as Instruction;
                var operandAsInstructionArray = instruction.Operand as Instruction[];

                if(operandAsInstruction != null)
                {
                    AnalyzeBlockFrom(method, operandAsInstruction, methodBlocks);
                }
                else if(operandAsInstructionArray != null)
                {
                    foreach(var operandInstruction in operandAsInstructionArray)
                    {
                        AnalyzeBlockFrom(method, operandInstruction, methodBlocks);
                    }
                }
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

        private static bool NextInstructionShouldBeReached(Instruction instruction)
        {
            // we effectively treat unconditional jump to next instruction as nop
            return instruction.OpCode.FlowControl == FlowControl.Call || instruction.OpCode.FlowControl == FlowControl.Next
                || (instruction.OpCode.FlowControl == FlowControl.Branch &&
                    instruction.Next.Offset == ((Instruction)instruction.Operand).Offset);
        }

        private long nextBlockId;
        private readonly Dictionary<long, BlockEntry> blocks;
    }
}

