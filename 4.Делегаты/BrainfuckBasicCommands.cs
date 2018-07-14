// Вставьте сюда финальное содержимое файла BrainfuckBasicCommands.cs
using System;
using System.Collections.Generic;
using System.Linq;
 
namespace func.brainfuck
{
    public class Constant
    {
        static readonly char[] constant = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890".ToCharArray();
 
        public static void Constants(IVirtualMachine vm)
        {
            foreach (var chars in constant)
            {
                vm.RegisterCommand(chars, machine => machine.Memory[machine.MemoryPointer] = (byte)chars);
            }
        }
    }
 
    public class BrainfuckBasicCommands
    {
        public static void RightOrLeftByteShift(IVirtualMachine vm)
        {
            vm.RegisterCommand('<', machine =>
            {
                machine.MemoryPointer = Calc(machine.MemoryPointer, -1, machine.Memory.Length);
            });
 
            vm.RegisterCommand('>', machine =>
            {
                machine.MemoryPointer = Calc(machine.MemoryPointer, 1, machine.Memory.Length);
            });
        }
 
        public static void IncOrDecByte(IVirtualMachine vm)
        {
            vm.RegisterCommand('+', machine =>
            {
                var bytes = machine.Memory[machine.MemoryPointer];
                var length = machine.Memory.Length;
 
                machine.Memory[machine.MemoryPointer] = bytes == 255
                    ? machine.Memory[machine.MemoryPointer] = 0
                    : (byte)Calc(bytes, 1, length);
            });
 
            vm.RegisterCommand('-', machine =>
            {
                var bytes = machine.Memory[machine.MemoryPointer];
                var length = machine.Memory.Length;
 
                machine.Memory[machine.MemoryPointer] = bytes == 0
                    ? machine.Memory[machine.MemoryPointer] = 255
                    : (byte)Calc(bytes, -1, length);
            });
        }
 
        public static int Calc(int a, int b, int modulus)
        {
            return (a + modulus + b % modulus) % modulus;
        }
 
        public static void RegisterTo(IVirtualMachine vm, Func<int> read, Action<char> write)
        {
            Constant.Constants(vm);
 
            RightOrLeftByteShift(vm);
 
            IncOrDecByte(vm);
 
            vm.RegisterCommand('.', machine => write((char)machine.Memory[machine.MemoryPointer]));
 
            vm.RegisterCommand(',', machine => machine.Memory[machine.MemoryPointer] = (byte) read());
        }
    }
}