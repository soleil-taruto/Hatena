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
		// Token: 0x06001D3C RID: 7484 RVA: 0x00010258 File Offset: 0x0000E458
		internal Resources()
		{
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001D3D RID: 7485 RVA: 0x000449EF File Offset: 0x00042BEF
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
		// (get) Token: 0x06001D3E RID: 7486 RVA: 0x00044A1B File Offset: 0x00042C1B
		// (set) Token: 0x06001D3F RID: 7487 RVA: 0x00044A22 File Offset: 0x00042C22
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

		// Token: 0x04000C80 RID: 3200
		private static ResourceManager resourceMan;

		// Token: 0x04000C81 RID: 3201
		private static CultureInfo resourceCulture;
	}
}
