using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Charlotte.Properties
{
	// Token: 0x02000179 RID: 377
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001D43 RID: 7491 RVA: 0x00044B18 File Offset: 0x00042D18
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x04000C95 RID: 3221
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
