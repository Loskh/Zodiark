//using Binarysharp.Assemblers.Fasm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Zodiark;
using GreyMagic;
using Iced.Intel;
using static Iced.Intel.AssemblerRegisters;
using System.IO;

namespace Zodiark.Test
{
    [StructLayout(LayoutKind.Explicit, Size = 0x68)]
    struct ChatBoxString
    {
        [FieldOffset(0x0)] public IntPtr StringPtr;
        [FieldOffset(0x8)] public long BufSize;
        [FieldOffset(0x10)] public long StringLength;
        [FieldOffset(0x18)] public long Ukn;
    }

    class Program
    {

        static Process process;
        static ZodiarkProcess Mordion;
        static Offsets Offsets;
        static ExternalProcessMemory Memory;
        static void Main(string[] args) {
            process = Process.GetProcessesByName("ffxiv_dx11")[0];
            Mordion = new ZodiarkProcess(process);
            Offsets = new Offsets(Mordion);

            var _entrancePtr = Mordion.Scanner.ScanText("4C 8B DC 53 56 48 81 EC 18 02 00 00 48 8B 05");
            //var _entrancePtr = zodiark.BaseAddress + 0x92DA0; ;
            //Memory = new ExternalProcessMemory(process, false, false);
            //Memory.WriteBytes(_entrancePtr, new byte[] { 76, 139, 220, 83, 86 });
            //Memory = new ExternalProcessMemory(process, true, false, _entrancePtr, false, 5, true);
            ZodiarkTest();
            //GreyMagicTest();
            //var fasm = new FasmNet();
            Memory.Dispose();
            Console.ReadLine();
        }

        static void GreyMagicTest() {

            var command = "/e 123";
            var array = Encoding.UTF8.GetBytes(command);
            using (AllocatedMemory allocatedMemory = Memory.CreateAllocatedMemory(400), allocatedMemory2 = Memory.CreateAllocatedMemory(array.Length + 30)) {
                allocatedMemory2.AllocateOfChunk("cmd", array.Length);
                allocatedMemory2.WriteBytes("cmd", array);
                allocatedMemory.AllocateOfChunk<IntPtr>("cmdAddress");
                allocatedMemory.AllocateOfChunk<long>("t1");
                allocatedMemory.AllocateOfChunk<long>("tLength");
                allocatedMemory.AllocateOfChunk<long>("t3");
                allocatedMemory.Write("cmdAddress", allocatedMemory2.Address);
                allocatedMemory.Write("t1", 0x40);
                allocatedMemory.Write("tLength", array.Length + 1);
                allocatedMemory.Write("t3", 0x00);
                //_ = Memory.CallInjected64<int>(Offsets.ProcessChatBoxPtr, Offsets.RaptureModule,
                //    allocatedMemory.Address, Offsets.UiModule);
                _ = Memory.CallInjected64<int>(Offsets.ProcessChatBoxPtr, Offsets.UiModule, allocatedMemory.Address, IntPtr.Zero, 0);
            }

        }
        static void ZodiarkTest() {
            var cmd = "/e 123";
            ChatBoxString cmdString;
            var cmdBytes = Encoding.UTF8.GetBytes(cmd);
            var stringMemPtr = Mordion.Memory.Allocate(cmdBytes.Length + 30);
            Mordion.Memory.WriteBytes(stringMemPtr, cmdBytes);
            cmdString.StringPtr = stringMemPtr;
            cmdString.BufSize = 0x40;
            cmdString.StringLength = cmdBytes.Length + 1;
            cmdString.Ukn = 0;
            var stringMem = Mordion.Memory.Allocate(400);
            Mordion.Memory.Write<ChatBoxString>(stringMem, cmdString);
            Mordion.Execute(Offsets.ProcessChatBoxPtr, Offsets.UiModule, stringMem, IntPtr.Zero, 0);
        }
    }
}
