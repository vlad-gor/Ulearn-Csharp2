// Вставьте сюда финальное содержимое файла BrainfuckLoopCommands.cs
using System;
using System.Collections.Generic;
using System.Linq;
 
namespace func.brainfuck
{
    public class BrainfuckLoopCommands
    {
        public static Dictionary<int, int> Bracket = new Dictionary<int, int>();
        public static Dictionary<int, int> ClosingBracket = new Dictionary<int, int>();
        public static Stack<int> Stack = new Stack<int>();
 
        public static void BodyLoop(IVirtualMachine vm)
        {
            for (int i = 0; i < vm.Instructions.Length; i++)
            {
                var bracket = vm.Instructions[i];
                switch (bracket)
                {
                    case '[':
                        Stack.Push(i);
                        break;
                    case ']':
                        ClosingBracket[i] = Stack.Peek();
                        Bracket[Stack.Pop()] = i;
                        break;
                }
            }
        }
 
        public static void RegisterTo(IVirtualMachine vm)
        {
            BodyLoop(vm);
 
            vm.RegisterCommand('[', machine =>
            {
                if (machine.Memory[machine.MemoryPointer] == 0)
                {
                    machine.InstructionPointer = Bracket[machine.InstructionPointer];
                }
            });
            vm.RegisterCommand(']', machine =>
            {
                if (machine.Memory[machine.MemoryPointer] != 0)
                {
                    machine.InstructionPointer = ClosingBracket[machine.InstructionPointer];
                }
            });
        }
    }
}