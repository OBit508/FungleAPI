using System;
using System.Diagnostics;

namespace BepInEx.Preloader.Core.Patching
{
	// Token: 0x02000007 RID: 7
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	[Conditional("CodeGeneration")]
	internal sealed class PatcherAutoPluginAttribute : Attribute
	{
		// Token: 0x0600000B RID: 11 RVA: 0x000021C2 File Offset: 0x000003C2
		public PatcherAutoPluginAttribute(string id = null, string name = null, string version = null)
		{
		}
	}
}
