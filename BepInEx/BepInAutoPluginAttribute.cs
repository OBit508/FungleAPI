using System;
using System.Diagnostics;

namespace BepInEx
{
	// Token: 0x02000006 RID: 6
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	[Conditional("CodeGeneration")]
	internal sealed class BepInAutoPluginAttribute : Attribute
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000021BA File Offset: 0x000003BA
		public BepInAutoPluginAttribute(string id = null, string name = null, string version = null)
		{
		}
	}
}
