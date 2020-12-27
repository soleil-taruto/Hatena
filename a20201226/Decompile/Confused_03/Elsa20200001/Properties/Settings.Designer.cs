using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Charlotte.Properties
{
	// Token: 0x02000178 RID: 376
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06001D6D RID: 7533 RVA: 0x00044DFE File Offset: 0x00042FFE
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x04000CBA RID: 3258
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
