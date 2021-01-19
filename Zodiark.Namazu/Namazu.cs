using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Zodiark.Namazu
{
    [StructLayout(LayoutKind.Explicit, Size = 0x68)]
    internal struct ChatBoxString
    {
        [FieldOffset(0x0)] public IntPtr StringPtr;
        [FieldOffset(0x8)] public long BufSize;
        [FieldOffset(0x10)] public long StringLength;
        [FieldOffset(0x18)] public long Ukn;
    }

    public class Namazu
    {
        private Process process;
        private ZodiarkProcess Mordion;
        private Offsets Offsets;

        public Namazu()
        {
            var list = Process.GetProcessesByName("ffxiv_dx11");
            if (list.Length == 0) throw new Exception("未找到游戏进程。");
            process = list[0];
            Mordion = new ZodiarkProcess(process);
            Offsets = new Offsets(Mordion);
        }

        public void SendCommand(string cmd)
        {
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

        public void WriteWaymark(Waymark waymark, int id = -1)
        {
            // Ensure the waymark isn't null.
            if (waymark == null)
                return;

            var wID = (id == -1 ? (byte)waymark.ID : id);

            // Initialize pointers and addresses to the memory we're going to read.
            //var ffxiv = GameProcess.BaseProcess.MainModule.BaseAddress;

            // pointers for waymark positions
            var wayA = Offsets.Waymarks + 0x00;
            var wayB = Offsets.Waymarks + 0x20;
            var wayC = Offsets.Waymarks + 0x40;
            var wayD = Offsets.Waymarks + 0x60;
            var wayOne = Offsets.Waymarks + 0x80;
            var wayTwo = Offsets.Waymarks + 0xA0;
            var wayThree = Offsets.Waymarks + 0xC0;
            var wayFour = Offsets.Waymarks + 0xE0;

            IntPtr markAddr = IntPtr.Zero;
            if (wID == (int)WaymarkID.A)
                markAddr = wayA;
            else if (wID == (int)WaymarkID.B)
                markAddr = wayB;
            else if (wID == (int)WaymarkID.C)
                markAddr = wayC;
            else if (wID == (int)WaymarkID.D)
                markAddr = wayD;
            else if (wID == (int)WaymarkID.One)
                markAddr = wayOne;
            else if (wID == (int)WaymarkID.Two)
                markAddr = wayTwo;
            else if (wID == (int)WaymarkID.Three)
                markAddr = wayThree;
            else if (wID == (int)WaymarkID.Four)
                markAddr = wayFour;

            // Write the X, Y and Z coordinates
            Mordion.Memory.Write(markAddr, waymark.X);
            Mordion.Memory.Write(markAddr + 0x4, waymark.Y);
            Mordion.Memory.Write(markAddr + 0x8, waymark.Z);

            Mordion.Memory.Write(markAddr + 0x10, (int)(waymark.X * 1000));
            Mordion.Memory.Write(markAddr + 0x14, (int)(waymark.Y * 1000));
            Mordion.Memory.Write(markAddr + 0x18, (int)(waymark.Z * 1000));

            // Write the active state
            Mordion.Memory.Write(markAddr + 0x1C, (byte)(waymark.Active ? 1 : 0));

            // Return out of this function
            return;
        }

        public void WriteWaymark(Preset preset)
        {
            Thread WaymarkThread = new Thread(() =>
            {
                WriteWaymark(preset.A, 0);
                WriteWaymark(preset.B, 1);
                WriteWaymark(preset.C, 2);
                WriteWaymark(preset.D, 3);
                WriteWaymark(preset.One, 4);
                WriteWaymark(preset.Two, 5);
                WriteWaymark(preset.Three, 6);
                WriteWaymark(preset.Four, 7);
            });

            WaymarkThread.Start();
        }

        public Preset ReadWaymark()
        {
            var preset = new Preset();

            // pointers for waymark positions
            var wayA = Offsets.Waymarks + 0x00;
            var wayB = Offsets.Waymarks + 0x20;
            var wayC = Offsets.Waymarks + 0x40;
            var wayD = Offsets.Waymarks + 0x60;
            var wayOne = Offsets.Waymarks + 0x80;
            var wayTwo = Offsets.Waymarks + 0xA0;
            var wayThree = Offsets.Waymarks + 0xC0;
            var wayFour = Offsets.Waymarks + 0xE0;

            // ReadWaymark local function to read multiple waymarks with.
            Waymark ReadWaymark(IntPtr addr, WaymarkID id) => new Waymark
            {
                X = Mordion.Memory.Read<float>(addr),
                Y = Mordion.Memory.Read<float>(addr + 0x4),
                Z = Mordion.Memory.Read<float>(addr + 0x8),
                Active = Mordion.Memory.Read<Byte>(addr + 0x1C) == 1,
                ID = id
            };

            // Read waymarks in with our function.
            preset.A = ReadWaymark(wayA, WaymarkID.A);
            preset.B = ReadWaymark(wayB, WaymarkID.B);
            preset.C = ReadWaymark(wayC, WaymarkID.C);
            preset.D = ReadWaymark(wayD, WaymarkID.D);
            preset.One = ReadWaymark(wayOne, WaymarkID.One);
            preset.Two = ReadWaymark(wayTwo, WaymarkID.Two);
            preset.Three = ReadWaymark(wayThree, WaymarkID.Three);
            preset.Four = ReadWaymark(wayFour, WaymarkID.Four);

            return preset;
        }
    }
}