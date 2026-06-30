using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Habillage
{
    [CreateAssetMenu(menuName = "Data/Color")]
    public class ColorDatabase : ScriptableObject
    {
        public SerializedDictionary<string, Color> Colors = new();
    }
    public enum ColorPresetEnum
    {
        Red = 0, Rose = 1, Magenta = 2, Violet = 3, Blue = 4, Azure = 5, Turquoise = 6, Aquamarine = 7, Green = 8, Chartreuse = 9, Turbo = 10, Orange = 11, Gray = 12
    }

    public static class ColorPreset
    {
        public static Color Red => RuntimeData.ColorDatabase.Colors["Red"];
        public static Color Rose => RuntimeData.ColorDatabase.Colors["Rose"];
        public static Color Magenta => RuntimeData.ColorDatabase.Colors["Magenta"];
        public static Color Violet => RuntimeData.ColorDatabase.Colors["Violet"];
        public static Color Blue => RuntimeData.ColorDatabase.Colors["Blue"];
        public static Color Azure => RuntimeData.ColorDatabase.Colors["Azure"];
        public static Color Turquoise => RuntimeData.ColorDatabase.Colors["Turquoise"];
        public static Color Aquamarine => RuntimeData.ColorDatabase.Colors["Aquamarine"];
        public static Color Green => RuntimeData.ColorDatabase.Colors["Green"];
        public static Color Chartreuse => RuntimeData.ColorDatabase.Colors["Chartreuse"];
        public static Color Turbo => RuntimeData.ColorDatabase.Colors["Turbo"];
        public static Color Orange => RuntimeData.ColorDatabase.Colors["Orange"];
        public static Color Gray => RuntimeData.ColorDatabase.Colors["Gray"];

        public static Color FromEnum(this ColorPresetEnum color)
        {
            switch (color)
            {
                case ColorPresetEnum.Red:
                    return ColorPreset.Red;

                case ColorPresetEnum.Rose:
                    return ColorPreset.Rose;

                case ColorPresetEnum.Magenta:
                    return ColorPreset.Magenta;

                case ColorPresetEnum.Violet:
                    return ColorPreset.Violet;

                case ColorPresetEnum.Blue:
                    return ColorPreset.Blue;

                case ColorPresetEnum.Azure:
                    return ColorPreset.Azure;

                case ColorPresetEnum.Turquoise:
                    return ColorPreset.Turquoise;

                case ColorPresetEnum.Aquamarine:
                    return ColorPreset.Aquamarine;

                case ColorPresetEnum.Green:
                    return ColorPreset.Green;

                case ColorPresetEnum.Chartreuse:
                    return ColorPreset.Chartreuse;

                case ColorPresetEnum.Turbo:
                    return ColorPreset.Turbo;

                case ColorPresetEnum.Orange:
                    return ColorPreset.Orange;

                case ColorPresetEnum.Gray:
                    return ColorPreset.Gray;
            }
            return Color.clear;
        }
    }
}

