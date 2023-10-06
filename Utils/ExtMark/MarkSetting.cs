using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;

namespace TeklaDev
{
    public enum LocationMark
    {
        TopPart,
        MiddlePart,
        BottomPart,
    }
    public enum MarkType
    {
        PROFILE,
        PANEL_NAME,
        TEXT
    }
    public static class MarkSetting
    {
        public const string PROFILE = "PROFILE";
        public const string PANEL_NAME = "PANEL NAME";
        public const string TEXT = "TEXT";

        public static void ConfigMarkSetting(this tsd.Mark mark,tsm.BooleanPart booleanPart, MarkType markType, string text = "")
        {
            var panelName = "";
            booleanPart.GetUserProperty("PANEL NAME", ref panelName);
            mark.Attributes.Content.Clear();
            switch (markType)
            {
                case MarkType.PROFILE:
                    mark.Attributes.Content.Add(new tsd.TextElement("("));
                    mark.Attributes.Content.Add(new tsd.UserDefinedElement(ExtUDA.PROFILE));
                    mark.Attributes.Content.Add(new tsd.TextElement(")"));
                    break;
                case MarkType.TEXT:
                    if (!string.IsNullOrEmpty(text))
                    {
                        mark.Attributes.Content.Add(new tsd.TextElement(text));
                    }
                    else
                    {
                        mark.Attributes.Content.Add(new tsd.UserDefinedElement(ExtUDA.NAME));
                    }
                    break;
                case MarkType.PANEL_NAME:
                    //mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.PANEL_NAME));
                    if (!string.IsNullOrEmpty(panelName))
                    {
                        mark.Attributes.Content.Add(new tsd.TextElement(panelName));
                    }
                    else
                    {
                        mark.Attributes.Content.Add(new tsd.TextElement(PANEL_NAME));
                    }
                    break;
            }
        }

        public static MarkType TransformTextToMarkType(this string markSettingName)
        {
            var result = MarkType.PANEL_NAME;
            switch (markSettingName)
            {
                case MarkSetting.PROFILE:
                    result = MarkType.PROFILE;
                    break;
                case MarkSetting.TEXT:
                    result = MarkType.TEXT;
                    break;
                case MarkSetting.PANEL_NAME:
                    result = MarkType.PANEL_NAME;
                    break;
                default:
                    result = MarkType.PANEL_NAME;
                    break;
            }
            return result;
        }
        public static string TransformMarkTypeToText(this MarkType markType)
        {
            var result = "";
            switch (markType) 
            { 
                case MarkType.PROFILE:
                    result = MarkSetting.PROFILE;
                    break;
                case MarkType.TEXT:
                    result = MarkSetting.TEXT;
                    break;
                case MarkType.PANEL_NAME:
                    result = MarkSetting.PANEL_NAME;
                    break;
            }
            return result;
        }

        public static LocationMark TransformIntToLocationMark(this int locationMark)
        {
            var result = LocationMark.MiddlePart;
            switch (locationMark)
            {
                case 0:
                    result = LocationMark.TopPart;
                    break;
                case 1:
                    result = LocationMark.MiddlePart;
                    break;
                case 2:
                    result = LocationMark.BottomPart;
                    break;
            }
            return result;
        }
        public static int TransformLocationMarkToInt(this LocationMark locationMark)
        {
            var result = 1;
            switch (locationMark)
            {
                case LocationMark.TopPart:
                    result = 0;
                    break;
                case LocationMark.MiddlePart:
                    result = 1;
                    break;
                case LocationMark.BottomPart:
                    result = 2;
                    break;
            }
            return (int)result;
        }

    }
}
