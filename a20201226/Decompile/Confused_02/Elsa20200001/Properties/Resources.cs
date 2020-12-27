using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Charlotte.Properties
{
	// Token: 0x02000178 RID: 376
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06001D3F RID: 7487 RVA: 0x000113DD File Offset: 0x0000F5DD
		internal Resources()
		{
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001D40 RID: 7488 RVA: 0x00044ADD File Offset: 0x00042CDD
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("Charlotte.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06001D41 RID: 7489 RVA: 0x00044B09 File Offset: 0x00042D09
		// (set) Token: 0x06001D42 RID: 7490 RVA: 0x00044B10 File Offset: 0x00042D10
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x04000C93 RID: 3219
		private static ResourceManager resourceMan;

		// Token: 0x04000C94 RID: 3220
		private static CultureInfo resourceCulture;
	}
}
