using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Charlotte.Properties
{
	// Token: 0x02000179 RID: 377
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06001D70 RID: 7536 RVA: 0x00010F77 File Offset: 0x0000F177
		internal Resources()
		{
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06001D71 RID: 7537 RVA: 0x00044E23 File Offset: 0x00043023
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

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001D72 RID: 7538 RVA: 0x00044E4F File Offset: 0x0004304F
		// (set) Token: 0x06001D73 RID: 7539 RVA: 0x00044E56 File Offset: 0x00043056
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

		// Token: 0x04000CBB RID: 3259
		private static ResourceManager resourceMan;

		// Token: 0x04000CBC RID: 3260
		private static CultureInfo resourceCulture;
	}
}
