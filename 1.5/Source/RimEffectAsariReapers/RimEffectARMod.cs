﻿using HarmonyLib;
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

            Log.Message($":: Rim-Effect Reborn: Asari & Reapers :: {CurrentVersion} ::".Colorize(Color.cyan));

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            HarmonyLib.Harmony harmony = new HarmonyLib.Harmony("Neronix17.RimEffectAsariReapers.RimWorld");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Rim-Effect Reborn: Asari & Reapers";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
            //base.DoSettingsWindowContents(inRect);
            //bool flag = optionsViewRectHeight > inRect.height;
            //Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            //Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            //Listing_Standard listing = new Listing_Standard();
            //Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            //listing.Begin(rect);
            //// ============================ CONTENTS ================================
            //DoOptionsCategoryContents(listing);
            //// ======================================================================
            //optionsViewRectHeight = listing.CurHeight;
            //listing.End();
            //Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {

        }
    }
}
