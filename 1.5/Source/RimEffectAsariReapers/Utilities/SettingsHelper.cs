using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimEffectAR
{
    using UnityEngine;
    using Verse;

    internal static class SettingsHelper
	{
		public static void SliderLabeled(this Listing_Standard ls, string label, ref int val, string format, float min = 0f, float max = 100f, string tooltip = null)
		{
			float num = (float)val;
			ls.SliderLabeled(label, ref num, format, min, max, tooltip);
			val = (int)num;
		}

		public static void SliderLabeled(this Listing_Standard ls, string label, ref float val, string format, float min = 0f, float max = 1f, string tooltip = null)
		{
			Rect       rect   = ls.GetRect(Text.LineHeight);
			Rect       rect2  = rect.LeftPart(0.7f).Rounded();
			Rect       rect3  = rect.RightPart(0.3f).Rounded().LeftPart(0.67f).Rounded();
			Rect       rect4  = rect.RightPart(0.1f).Rounded();
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			float num = Widgets.HorizontalSlider(rect3, val, min, max, true, null, null, null, -1f);
			val = num;
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rect4, string.Format(format, val));
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Text.Anchor = anchor;
			ls.Gap(ls.verticalSpacing);
		}

		public static void FloatRange(this Listing_Standard ls, string label, ref FloatRange range, float min = 0f, float max = 1f, string tooltip = null, ToStringStyle valueStyle = ToStringStyle.FloatTwo)
		{
			Rect rect  = ls.GetRect(Text.LineHeight);
			Rect rect2 = rect.LeftPart(0.7f).Rounded();
			Rect rect3 = rect.RightPart(0.3f).Rounded().LeftPart(0.9f).Rounded();
			rect3.yMin -= 5f;
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			Text.Anchor = TextAnchor.MiddleRight;
			int hashCode = ls.CurHeight.GetHashCode();
			Widgets.FloatRange(rect3, hashCode, ref range, min, max, null, valueStyle);
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Text.Anchor = anchor;
			ls.Gap(ls.verticalSpacing);
		}

		public static Rect GetRect(this Listing_Standard listing_Standard, float? height = null)
		{
			return listing_Standard.GetRect(height ?? Text.LineHeight);
		}

		public static void AddLabeledRadioList(this Listing_Standard listing_Standard, string header, string[] labels, ref string val, float? headerHeight = null)
		{
			if (header != string.Empty)
			{
				Widgets.Label(listing_Standard.GetRect(headerHeight), header);
			}
			listing_Standard.AddRadioList(SettingsHelper.GenerateLabeledRadioValues(labels), ref val, null);
		}

		private static void AddRadioList<T>(this Listing_Standard listing_Standard, List<SettingsHelper.LabeledRadioValue<T>> items, ref T val, float? height = null)
		{
			foreach (SettingsHelper.LabeledRadioValue<T> labeledRadioValue in items)
			{
				if (Widgets.RadioButtonLabeled(listing_Standard.GetRect(height), labeledRadioValue.Label, EqualityComparer<T>.Default.Equals(labeledRadioValue.Value, val)))
				{
					val = labeledRadioValue.Value;
				}
			}
		}

		private static List<SettingsHelper.LabeledRadioValue<string>> GenerateLabeledRadioValues(string[] labels)
		{
			List<SettingsHelper.LabeledRadioValue<string>> list = new List<SettingsHelper.LabeledRadioValue<string>>();
			foreach (string text in labels)
			{
				list.Add(new SettingsHelper.LabeledRadioValue<string>(text, text));
			}
			return list;
		}

		public static void AddLabeledTextField(this Listing_Standard listing_Standard, string label, ref string settingsValue, float leftPartPct = 0.5f)
		{
			Rect rect;
			Rect rect2;
			listing_Standard.LineRectSpilter(out rect, out rect2, leftPartPct, null);
			Widgets.Label(rect, label);
			string text = settingsValue.ToString();
			settingsValue = Widgets.TextField(rect2, text);
		}

		public static void AddLabeledNumericalTextField<T>(this Listing_Standard listing_Standard, string label, ref T settingsValue, float leftPartPct = 0.5f, float minValue = 1f, float maxValue = 100000f) where T : struct
		{
			Rect rect;
			Rect rect2;
			listing_Standard.LineRectSpilter(out rect, out rect2, leftPartPct, null);
			Widgets.Label(rect, label);
			string text = settingsValue.ToString();
			Widgets.TextFieldNumeric<T>(rect2, ref settingsValue, ref text, minValue, maxValue);
		}

		public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, float leftPartPct = 0.5f, float? height = null)
		{
			Rect rect = listing_Standard.GetRect(height);
			leftHalf = rect.LeftPart(leftPartPct).Rounded();
			return rect;
		}

		public static Rect LineRectSpilter(this Listing_Standard listing_Standard, out Rect leftHalf, out Rect rightHalf, float leftPartPct = 0.5f, float? height = null)
		{
			Rect rect = listing_Standard.LineRectSpilter(out leftHalf, leftPartPct, height);
			rightHalf = rect.RightPart(1f - leftPartPct).Rounded();
			return rect;
		}

		public class LabeledRadioValue<T>
		{
			public LabeledRadioValue(string label, T val)
			{
				this.Label = label;
				this.Value = val;
			}

			public string Label { get; set; }

			public T Value { get; set; }
		}
	}
}
