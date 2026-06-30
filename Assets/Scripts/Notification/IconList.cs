using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Habillage
{

    public enum IconListEnum
    {
        small, large, happy, smile, sad
    }

    public static class IconList
    {
        public static string small => "icon_small";
        public static string large => "icon_large";
        public static string happy => "icon_happy";
        public static string smile => "icon_smile";
        public static string sad => "icon_sad";

        public static string FromEnum(this IconListEnum icon)
        {
            switch (icon)
            {
                case IconListEnum.small:
                    return IconList.small;
                case IconListEnum.large:
                    return IconList.large;
                case IconListEnum.happy:
                    return IconList.happy;
                case IconListEnum.smile:
                    return IconList.smile;
                case IconListEnum.sad:
                    return IconList.sad;
            }
            return IconList.small;
        }
    }
}
