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
		// (get) Token: 0x06001D40 RID: 7488 RVA: 0x00044A2A File Offset: 0x00042C2A
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x04000C82 RID: 3202
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
