using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zodiark.Memory;
using Zodiark.Injection.Assembly;
using Zodiark.Native;

namespace Zodiark.Injection
{
   public static class FastCall
    {
        public static IntPtr Execute(ZodiarkProcess z,IntPtr address,params dynamic[] args) {
            var returnmem = z.Memory.Allocate(sizeof(long));
            AsmCache asmCache = new AsmCache(address, returnmem, args);
            var bytes = asmCache.asmBytes;
            var mem = z.Memory.Allocate(bytes.Length);
            z.Memory.WriteBytes(mem, bytes);
            var ret = Kernel32.CreateRemoteThread(ProcessMemory.Handle, IntPtr.Zero, 0, mem, (IntPtr)0, 0, out _);
            Kernel32.WaitForSingleObject(ret, unchecked((uint)-1));
            Kernel32.CloseHandle(ret);
            IntPtr retValue = z.Memory.ReadIntPtr(returnmem);
            z.Memory.Free(mem);
            z.Memory.Free(returnmem);
            return retValue; 
        }
    }
}
