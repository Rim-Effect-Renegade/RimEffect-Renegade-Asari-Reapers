using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimEffectAR
{
    public class RimEffectARMod : Mod
    {
        public static RimEffectARMod mod;
        public static RimEffectARSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(mod.Content.ModMetaData.RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public RimEffectARMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<RimEffectARSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            Log.Message($":: Rim-Effect Renegade: Asari & Reapers :: {CurrentVersion} ::".Colorize(Color.cyan));

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }
        }

        public override string SettingsCategory() => "Rim-Effect Renegade: Asari & Reapers";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }
    }
}
