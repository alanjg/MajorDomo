using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DominionUWP
{
	public static class Cpu
	{
		public enum ProcessorArchitecture : ushort
		{
			INTEL = 0,
			MIPS = 1,
			ALPHA = 2,
			PPC = 3,
			SHX = 4,
			ARM = 5,
			IA64 = 6,
			ALPHA64 = 7,
			MSIL = 8,
			AMD64 = 9,
			IA32_ON_WIN64 = 10,
			UNKNOWN = 0xFFFF
		}

		[StructLayout(LayoutKind.Sequential)]
		struct _SYSTEM_INFO
		{
			public ushort wProcessorArchitecture;
			public ushort wReserved;
			public uint dwPageSize;
			public IntPtr lpMinimumApplicationAddress;
			public IntPtr lpMaximumApplicationAddress;
			public UIntPtr dwActiveProcessorMask;
			public uint dwNumberOfProcessors;
			public uint dwProcessorType;
			public uint dwAllocationGranularity;
			public ushort wProcessorLevel;
			public ushort wProcessorRevision;
		};

		[DllImport("kernel32.dll")]
		static extern void GetNativeSystemInfo(ref _SYSTEM_INFO lpSystemInfo);

		public static bool IsARM()
		{
			_SYSTEM_INFO info = new _SYSTEM_INFO();
			GetNativeSystemInfo(ref info);
			return info.wProcessorArchitecture == (ushort)ProcessorArchitecture.ARM;
		}
	}
}
