using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zodiark;

namespace Zodiark.Test
{
    public class Offsets
    {
        private readonly ZodiarkProcess Process;
        public IntPtr ProcessChatBoxPtr { get; }
        public IntPtr UiModulePtr { get; }
        public IntPtr UiModule => Process.Memory.ReadIntPtr(Process.Memory.ReadIntPtr(UiModulePtr));
        public IntPtr ModuleOffsetPtr { get; }
        public int ModuleOffset { get; }
        public IntPtr RaptureModule => UiModule + ModuleOffset;
        public IntPtr Waymarks { get; }
        public Offsets(ZodiarkProcess process) {
            Process = process;
            //ProcessChatBoxPtr = _scanner.ScanText("40 53 56 57 48 83 EC 70 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B 02");
            ProcessChatBoxPtr = Process.Scanner.ScanText("48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9");
            UiModulePtr = Process.Scanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 48 8B D9 8B 40 14 85 C0");
            ModuleOffsetPtr = Process.Scanner.ScanText("48 8D 8F ?? ?? ?? ?? 4C 8B C7 48 8D 54 24 ??") + 3;
            ModuleOffset = Process.Memory.ReadInt32(ModuleOffsetPtr);
            //uiModule = scanner.ReadIntPtr(scanner.ReadIntPtr(uiModulePtr));
            //raptureModule = uiModule + moduleOffset;
            //Waymarks = Process.Scanner.GetStaticAddressFromSig("48 8B 94 24 ? ? ? ? 48 8D 0D ? ? ? ? 41 B0 01") + 432;
        }
        public override string ToString() {
            return $"{Waymarks.ToHex()}\n{ProcessChatBoxPtr.ToHex()}\n{UiModule.ToHex()}";
        }
    }
}
