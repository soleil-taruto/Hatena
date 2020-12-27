using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Charlotte
{
	// Token: 0x020000FB RID: 251
	public class FailManualKalykeMap
	{
		// Token: 0x06000C58 RID: 3160 RVA: 0x000211C8 File Offset: 0x0001F3C8
		public static byte[] Load(string BindStandaloneWhiteTerms)
		{
			if (FailManualKalykeMap.ReadMaximumOrthosieSection)
			{
				return FailManualKalykeMap.TerminateOpenKaleModification(FailManualKalykeMap.SearchVerboseEpimetheusProduct[BindStandaloneWhiteTerms]);
			}
			string LabelExistingEtoileGeneration = Path.Combine(FailManualKalykeMap.ListLatestCarbonRegistry, BindStandaloneWhiteTerms);
			if (!File.Exists(LabelExistingEtoileGeneration))
			{
				LabelExistingEtoileGeneration = Path.Combine(FailManualKalykeMap.SeeAccessibleSaoPort, BindStandaloneWhiteTerms);
				if (!File.Exists(LabelExistingEtoileGeneration))
				{
					throw new Exception(LabelExistingEtoileGeneration);
				}
			}
			return File.ReadAllBytes(LabelExistingEtoileGeneration);
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x00021222 File Offset: 0x0001F422
		public void MoveUndefinedBiancaProgress()
		{
			this.CountTruePotassiumForm(this.StartPublicHerseSession());
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x00021230 File Offset: 0x0001F430
		private static byte[] TerminateOpenKaleModification(string DisablePublicBiancaStatus, long SaveGeneralIridiumAccess, int ReleaseUnknownBerryDamage)
		{
			byte[] result;
			using (FileStream LabelEqualSinopeEditor = new FileStream(DisablePublicBiancaStatus, FileMode.Open, FileAccess.Read))
			{
				LabelEqualSinopeEditor.Seek(SaveGeneralIridiumAccess, SeekOrigin.Begin);
				result = DistributeInternalStarCore.BindInnerWhiteRange(DestroyInitialBerylliumThirdparty.Read(LabelEqualSinopeEditor, ReleaseUnknownBerryDamage));
			}
			return result;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0002127C File Offset: 0x0001F47C
		public void ConflictCurrentIsonoeSystem(FailManualKalykeMap.InstallUsefulTenderCharacter LaunchEmptyAceComponent)
		{
			FailManualKalykeMap.ChooseSecureTrinculoUsage = LaunchEmptyAceComponent;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x00021284 File Offset: 0x0001F484
		public static IEnumerable<string> GetFiles()
		{
			IEnumerable<string> ChangeAdditionalKalykeDocumentation;
			if (FailManualKalykeMap.ReadMaximumOrthosieSection)
			{
				ChangeAdditionalKalykeDocumentation = FailManualKalykeMap.SearchVerboseEpimetheusProduct.Keys;
			}
			else
			{
				IEnumerable<string>[] array = new IEnumerable<string>[2];
				array[0] = from BindStandaloneWhiteTerms in Directory.GetFiles(FailManualKalykeMap.ListLatestCarbonRegistry, FailManualKalykeMap.BindPreferredRadonCompletion(), SearchOption.AllDirectories)
				select DestroyInitialBerylliumThirdparty.IgnoreUnablePhilophrosyneCleanup(BindStandaloneWhiteTerms, FailManualKalykeMap.ListLatestCarbonRegistry);
				array[1] = from BindStandaloneWhiteTerms in Directory.GetFiles(FailManualKalykeMap.SeeAccessibleSaoPort, FailManualKalykeMap.ClarifySpecificCadmiumUpdate(), SearchOption.AllDirectories)
				select DestroyInitialBerylliumThirdparty.IgnoreUnablePhilophrosyneCleanup(BindStandaloneWhiteTerms, FailManualKalykeMap.SeeAccessibleSaoPort);
				ChangeAdditionalKalykeDocumentation = DestroyInitialBerylliumThirdparty.Concat<string>(array);
				ChangeAdditionalKalykeDocumentation = from BindStandaloneWhiteTerms in ChangeAdditionalKalykeDocumentation
				where Path.GetFileName(BindStandaloneWhiteTerms)[0] != '_'
				select BindStandaloneWhiteTerms;
			}
			return DestroyInitialBerylliumThirdparty.Sort<string>(ChangeAdditionalKalykeDocumentation, new Comparison<string>(DestroyInitialBerylliumThirdparty.FireLatestOrthosieFunctionality));
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x0002135C File Offset: 0x0001F55C
		public static void CompleteBinaryPalladiumNull(string BindStandaloneWhiteTerms, byte[] RepresentUnavailableFleroviumArchive)
		{
			if (FailManualKalykeMap.ReadMaximumOrthosieSection)
			{
				throw new TweakNativeCallistoStack();
			}
			File.WriteAllBytes(Path.Combine(FailManualKalykeMap.SeeAccessibleSaoPort, BindStandaloneWhiteTerms), RepresentUnavailableFleroviumArchive);
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0002137C File Offset: 0x0001F57C
		public void ThrowGlobalSeaborgiumCache(int RequireValidTennessineHeader)
		{
			if (RequireValidTennessineHeader != this.ThrowExpectedSetebosExecutable())
			{
				this.RepresentInlineUranusClipboard(RequireValidTennessineHeader);
				return;
			}
			this.CountTruePotassiumForm(RequireValidTennessineHeader);
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x00021396 File Offset: 0x0001F596
		public static string BindPreferredRadonCompletion()
		{
			if (FailManualKalykeMap.ConnectExecutableFluorineInstallation == null)
			{
				FailManualKalykeMap.ConnectExecutableFluorineInstallation = FailManualKalykeMap.FailLatestSunsetVersion();
			}
			return FailManualKalykeMap.ConnectExecutableFluorineInstallation;
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x000213AE File Offset: 0x0001F5AE
		public FailManualKalykeMap.InstallUsefulTenderCharacter UpgradeAutomaticErisCredential()
		{
			return FailManualKalykeMap.ChooseSecureTrinculoUsage;
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x000213B5 File Offset: 0x0001F5B5
		public static IEnumerable<int> EnterUnusedMermaidDigit()
		{
			yield return 1432507746;
			yield return 1401181060;
			yield return 1991538356;
			yield return 2108652975;
			yield return 1277840426;
			yield return 1835560296;
			yield return 1872523164;
			yield return 1622433972;
			yield return 1017003211;
			yield return 1273318373;
			yield return 1122648843;
			yield break;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x000213BE File Offset: 0x0001F5BE
		private static byte[] TerminateOpenKaleModification(FailManualKalykeMap.ViewPrivateProsperoSearch AccessTemporaryEukeladeConfiguration)
		{
			return FailManualKalykeMap.TerminateOpenKaleModification(AccessTemporaryEukeladeConfiguration.RedirectUnresolvedChaldeneHandling, AccessTemporaryEukeladeConfiguration.DocumentStaticBergelmirAccount, AccessTemporaryEukeladeConfiguration.Size);
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x000213D7 File Offset: 0x0001F5D7
		public void InspectExpectedFranciscoResult(int SelectSpecificMoscoviumComment, int ScheduleConstantElaraPool)
		{
			this.DownloadInlineSunnyNumber(SelectSpecificMoscoviumComment, ScheduleConstantElaraPool, this.StartPublicHerseSession());
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x000213E7 File Offset: 0x0001F5E7
		public void DownloadInlineSunnyNumber(int SelectSpecificMoscoviumComment, int ScheduleConstantElaraPool, int ConnectEmptyHermippeProvider)
		{
			this.PreparePhysicalLithiumTask(SelectSpecificMoscoviumComment, ScheduleConstantElaraPool, ConnectEmptyHermippeProvider, this.UpgradeAutomaticErisCredential().FindBasedContinentalHash, this.UpgradeAutomaticErisCredential().StopRawCressidaButton, this.UpgradeAutomaticErisCredential().WarnInvalidPerditaOperation);
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x00021413 File Offset: 0x0001F613
		public static string ClarifySpecificCadmiumUpdate()
		{
			if (FailManualKalykeMap.ReleaseSuccessfulMermaidIdentifier == null)
			{
				FailManualKalykeMap.ReleaseSuccessfulMermaidIdentifier = FailManualKalykeMap.DownloadBooleanAtlasTermination();
			}
			return FailManualKalykeMap.ReleaseSuccessfulMermaidIdentifier;
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x0002142B File Offset: 0x0001F62B
		public static string FailNullPhilophrosyneTarget()
		{
			if (FailManualKalykeMap.ReferAlternativePrometheusStorage == null)
			{
				FailManualKalykeMap.ReferAlternativePrometheusStorage = FailManualKalykeMap.DumpInitialCarpoPool();
			}
			return FailManualKalykeMap.ReferAlternativePrometheusStorage;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x00021443 File Offset: 0x0001F643
		public void RepresentInlineUranusClipboard(int RecommendEmptyMagicianBit)
		{
			FailManualKalykeMap.MoveNativeAlbiorixRow = RecommendEmptyMagicianBit;
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x0002144B File Offset: 0x0001F64B
		public static IEnumerable<int> CalculateExtraMagicalWrapper()
		{
			yield return 1286818995;
			yield return 1882878010;
			yield return 2105179514;
			yield return 1291275554;
			yield break;
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x00021454 File Offset: 0x0001F654
		public static string DumpInitialCarpoPool()
		{
			return new string((from ContinueVerboseTitaniaTouch in FailManualKalykeMap.EnterUnusedMermaidDigit()
			where ContinueVerboseTitaniaTouch % 65537 != 0
			select ContinueVerboseTitaniaTouch into DestroyNativeArielTermination
			select (char)(DestroyNativeArielTermination % 65537 - 1)).ToArray<char>());
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x000214B8 File Offset: 0x0001F6B8
		public static IEnumerable<int> FollowGeneralBrightPopup()
		{
			yield return 1313689165;
			yield return 2119401043;
			yield return 1372213706;
			yield return 1662018320;
			yield return 1097351528;
			yield return 2119794265;
			yield return 1656054496;
			yield break;
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x000214C4 File Offset: 0x0001F6C4
		public static void SuppressAnonymousMakemakeAccess()
		{
			if (File.Exists(HackUnavailableDioneHandle.ExcludeMaximumEuropaCredential))
			{
				FailManualKalykeMap.ReadMaximumOrthosieSection = true;
			}
			else if (Directory.Exists(HackUnavailableDioneHandle.TypeStandaloneEuropiumManual))
			{
				FailManualKalykeMap.ListLatestCarbonRegistry = HackUnavailableDioneHandle.TypeStandaloneEuropiumManual;
				FailManualKalykeMap.SeeAccessibleSaoPort = HackUnavailableDioneHandle.WaitStaticErsaDamage;
			}
			else
			{
				FailManualKalykeMap.ListLatestCarbonRegistry = HackUnavailableDioneHandle.ReserveMockArsenicPrefix;
				FailManualKalykeMap.SeeAccessibleSaoPort = HackUnavailableDioneHandle.TestNormalAutonoeStartup;
			}
			if (FailManualKalykeMap.ReadMaximumOrthosieSection)
			{
				foreach (string DisablePublicBiancaStatus in new string[]
				{
					HackUnavailableDioneHandle.ExcludeMaximumEuropaCredential,
					HackUnavailableDioneHandle.LockPrivateBerylliumTodo
				})
				{
					List<FailManualKalykeMap.ViewPrivateProsperoSearch> TypeEmptyActiniumSwitch = new List<FailManualKalykeMap.ViewPrivateProsperoSearch>();
					using (FileStream LabelEqualSinopeEditor = new FileStream(DisablePublicBiancaStatus, FileMode.Open, FileAccess.Read))
					{
						while (LabelEqualSinopeEditor.Position < LabelEqualSinopeEditor.Length)
						{
							int ReleaseUnknownBerryDamage = DestroyInitialBerylliumThirdparty.BumpAnonymousTaygeteModification(DestroyInitialBerylliumThirdparty.Read(LabelEqualSinopeEditor, 4), 0);
							if (ReleaseUnknownBerryDamage < 0)
							{
								throw new TweakNativeCallistoStack();
							}
							TypeEmptyActiniumSwitch.Add(new FailManualKalykeMap.ViewPrivateProsperoSearch
							{
								RedirectUnresolvedChaldeneHandling = DisablePublicBiancaStatus,
								DocumentStaticBergelmirAccount = LabelEqualSinopeEditor.Position,
								Size = ReleaseUnknownBerryDamage
							});
							LabelEqualSinopeEditor.Seek((long)ReleaseUnknownBerryDamage, SeekOrigin.Current);
						}
					}
					string[] ChangeAdditionalKalykeDocumentation = DestroyInitialBerylliumThirdparty.RestoreOptionalKerberosStep(DestroyInitialBerylliumThirdparty.RefreshEmptyTritonArea.GetString(FailManualKalykeMap.TerminateOpenKaleModification(TypeEmptyActiniumSwitch[0])));
					if (ChangeAdditionalKalykeDocumentation.Length != TypeEmptyActiniumSwitch.Count)
					{
						throw new TweakNativeCallistoStack(ChangeAdditionalKalykeDocumentation.Length.ToString() + FailManualKalykeMap.FailNullPhilophrosyneTarget() + TypeEmptyActiniumSwitch.Count.ToString());
					}
					for (int TypeUnsupportedUranusDevelopment = 1; TypeUnsupportedUranusDevelopment < ChangeAdditionalKalykeDocumentation.Length; TypeUnsupportedUranusDevelopment++)
					{
						string BindStandaloneWhiteTerms = ChangeAdditionalKalykeDocumentation[TypeUnsupportedUranusDevelopment];
						if (FailManualKalykeMap.SearchVerboseEpimetheusProduct.ContainsKey(BindStandaloneWhiteTerms))
						{
							throw new TweakNativeCallistoStack(BindStandaloneWhiteTerms);
						}
						FailManualKalykeMap.SearchVerboseEpimetheusProduct.Add(BindStandaloneWhiteTerms, TypeEmptyActiniumSwitch[TypeUnsupportedUranusDevelopment]);
					}
				}
			}
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x0002167C File Offset: 0x0001F87C
		public int StartPublicHerseSession()
		{
			return FailManualKalykeMap.MoveNativeAlbiorixRow++;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x0002168B File Offset: 0x0001F88B
		public int TestSecureHydrogenVersion()
		{
			if (this.DistributeUnnecessaryBohriumHighlight() != 0)
			{
				return FailManualKalykeMap.RestoreExtraMintCrash;
			}
			return 0;
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x0002169C File Offset: 0x0001F89C
		public void CountTruePotassiumForm(int SelectSpecificMoscoviumComment)
		{
			this.InspectExpectedFranciscoResult(SelectSpecificMoscoviumComment, this.StartPublicHerseSession());
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x000216AB File Offset: 0x0001F8AB
		public void CompleteAlternativeHalimedeMenu()
		{
			this.RepresentInlineUranusClipboard(0);
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x000216B4 File Offset: 0x0001F8B4
		public void PreparePhysicalLithiumTask(int SelectSpecificMoscoviumComment, int ScheduleConstantElaraPool, int ConnectEmptyHermippeProvider, int PushUniqueEchoUse, int SaveInvalidRoseInformation, int AssociateSpecificOpheliaContact)
		{
			var FilterMinimumZincDigit = new <>f__AnonymousType56<int, int>[]
			{
				new
				{
					IncludeAvailableEtoileChild = SelectSpecificMoscoviumComment,
					ForceUniqueMermaidScript = PushUniqueEchoUse
				},
				new
				{
					IncludeAvailableEtoileChild = ScheduleConstantElaraPool,
					ForceUniqueMermaidScript = PushUniqueEchoUse
				},
				new
				{
					IncludeAvailableEtoileChild = ConnectEmptyHermippeProvider,
					ForceUniqueMermaidScript = PushUniqueEchoUse
				}
			};
			this.ConflictCurrentIsonoeSystem(new FailManualKalykeMap.InstallUsefulTenderCharacter
			{
				FindBasedContinentalHash = SelectSpecificMoscoviumComment,
				StopRawCressidaButton = ScheduleConstantElaraPool,
				WarnInvalidPerditaOperation = ConnectEmptyHermippeProvider
			});
			if (FilterMinimumZincDigit[0].IncludeAvailableEtoileChild == PushUniqueEchoUse)
			{
				this.ThrowGlobalSeaborgiumCache(FilterMinimumZincDigit[0].ForceUniqueMermaidScript);
			}
			if (FilterMinimumZincDigit[1].IncludeAvailableEtoileChild == SaveInvalidRoseInformation)
			{
				this.ThrowGlobalSeaborgiumCache(FilterMinimumZincDigit[1].ForceUniqueMermaidScript);
			}
			if (FilterMinimumZincDigit[2].IncludeAvailableEtoileChild == AssociateSpecificOpheliaContact)
			{
				this.ThrowGlobalSeaborgiumCache(FilterMinimumZincDigit[2].ForceUniqueMermaidScript);
			}
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00021757 File Offset: 0x0001F957
		public int ThrowExpectedSetebosExecutable()
		{
			return FailManualKalykeMap.MoveNativeAlbiorixRow;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x00021760 File Offset: 0x0001F960
		public static string DownloadBooleanAtlasTermination()
		{
			return new string((from ConnectUsefulYtterbiumPlaceholder in FailManualKalykeMap.CalculateExtraMagicalWrapper()
			where ConnectUsefulYtterbiumPlaceholder % 65537 != 0
			select ConnectUsefulYtterbiumPlaceholder into RedirectCorrectSoleilRequest
			select (char)(RedirectCorrectSoleilRequest % 65537 - 1)).ToArray<char>());
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000217C4 File Offset: 0x0001F9C4
		public int DistributeUnnecessaryBohriumHighlight()
		{
			return FailManualKalykeMap.IncludePhysicalAnankeDescriptor;
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x000217CC File Offset: 0x0001F9CC
		public static string FailLatestSunsetVersion()
		{
			return new string((from ResizeExpressArielCore in FailManualKalykeMap.FollowGeneralBrightPopup()
			where ResizeExpressArielCore % 65537 != 0
			select ResizeExpressArielCore into DefaultNormalKalykeUpload
			select (char)(DefaultNormalKalykeUpload % 65537 - 1)).ToArray<char>());
		}

		// Token: 0x04000540 RID: 1344
		public static string ReferAlternativePrometheusStorage;

		// Token: 0x04000541 RID: 1345
		public static int IncludePhysicalAnankeDescriptor;

		// Token: 0x04000542 RID: 1346
		private static string SeeAccessibleSaoPort;

		// Token: 0x04000543 RID: 1347
		public static string ReleaseSuccessfulMermaidIdentifier;

		// Token: 0x04000544 RID: 1348
		public static int RestoreExtraMintCrash;

		// Token: 0x04000545 RID: 1349
		private static bool ReadMaximumOrthosieSection = false;

		// Token: 0x04000546 RID: 1350
		private static string ListLatestCarbonRegistry;

		// Token: 0x04000547 RID: 1351
		public static int MoveNativeAlbiorixRow;

		// Token: 0x04000548 RID: 1352
		private static Dictionary<string, FailManualKalykeMap.ViewPrivateProsperoSearch> SearchVerboseEpimetheusProduct = DestroyInitialBerylliumThirdparty.RepresentAbstractSunnyModule<FailManualKalykeMap.ViewPrivateProsperoSearch>();

		// Token: 0x04000549 RID: 1353
		public static FailManualKalykeMap.InstallUsefulTenderCharacter ChooseSecureTrinculoUsage;

		// Token: 0x0400054A RID: 1354
		public static string ConnectExecutableFluorineInstallation;

		// Token: 0x02000322 RID: 802
		public class InstallUsefulTenderCharacter
		{
			// Token: 0x04001392 RID: 5010
			public int FindBasedContinentalHash;

			// Token: 0x04001393 RID: 5011
			public int StopRawCressidaButton;

			// Token: 0x04001394 RID: 5012
			public int WarnInvalidPerditaOperation;
		}

		// Token: 0x02000323 RID: 803
		private class ViewPrivateProsperoSearch
		{
			// Token: 0x04001395 RID: 5013
			public string RedirectUnresolvedChaldeneHandling;

			// Token: 0x04001396 RID: 5014
			public long DocumentStaticBergelmirAccount;

			// Token: 0x04001397 RID: 5015
			public int Size;
		}
	}
}
