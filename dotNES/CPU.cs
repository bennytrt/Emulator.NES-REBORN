using System;
using System.Linq;
using System.Reflection;

namespace dotNES
{
    sealed partial class CPU : Addressable
    {
        private readonly byte[] _ram = new byte[0x800];
        public int Cycle;
        private uint _currentOpcode;

        public delegate void Opcode();

        private readonly Opcode[] _opcodes = new Opcode[256];
        private readonly string[] _opcodeNames = new string[256];
        private readonly OpcodeDef[] _opcodeDefs = new OpcodeDef[256];

        public CPU(Emulator emulator) : base(emulator, 0xFFFF)
        {
            InitializeOpcodes();
            InitializeMemoryMap();
            Initialize();
        }

        private void InitializeOpcodes()
        {
            var opcodeBindings = from opcode in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                                 let defs = opcode.GetCustomAttributes(typeof(OpcodeDef), false)
                                 where defs.Length > 0
                                 select new
                                 {
                                     binding = (Opcode)Delegate.CreateDelegate(typeof(Opcode), this, opcode.Name),
                                     name = opcode.Name,
                                     defs = (from d in defs select (OpcodeDef)d)
                                 };

            foreach (var opcode in opcodeBindings)
            {
                foreach (var def in opcode.defs)
                {
                    _opcodes[def.Opcode] = opcode.binding;
                    _opcodeNames[def.Opcode] = opcode.name;
                    this._opcodeDefs[def.Opcode] = def;
                }
            }
        }

        private void ProcessStringFromMemory(ushort address)
        {
            uint w;
            string z = "";

            while ((w = ReadByte(address)) != '\0')
            {
                z += (char)w;
            }

            // Do something with 'z' if needed.
        }


        public void Execute()
        {
            const int maxInstructions = 5000;

            for (int i = 0; i < maxInstructions; i++)
            {
                ExecuteSingleInstruction();
            }

            ProcessStringFromMemory(6000);
            Console.WriteLine(">>> " + ReadByte(0x02));
        }
    }
}
