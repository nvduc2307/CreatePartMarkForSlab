using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;

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
        ASSEMBLY_POSITION,
        ASSEMBLY_POSITION_PROFILE,
        PROFILE,
        TEXT,
        PANEL_NAME
    }
    public static class MarkSetting
    {
        public const string ASSEMBLY_POSITION = "ASSEMBLY_POSITION";
        public const string ASSEMBLY_POSITION_PROFILE = "ASSEMBLY_POSITION_PROFILE";
        public const string PROFILE = "PROFILE";
        public const string TEXT = "TEXT";
        public const string PANEL_NAME = "PANEL_NAME";

        public static void ConfigMarkSetting(this tsd.Mark mark, MarkType markType, string text = "")
        {
            mark.Attributes.Content.Clear();
            switch (markType)
            {
                case MarkType.ASSEMBLY_POSITION:
                    mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.ASSEMBLY_POSITION));
                    break;
                case MarkType.ASSEMBLY_POSITION_PROFILE:
                    mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.ASSEMBLY_POSITION));
                    mark.Attributes.Content.Add(new TextElement("("));
                    mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.PROFILE));
                    mark.Attributes.Content.Add(new TextElement(")"));
                    break;
                case MarkType.PROFILE:
                    mark.Attributes.Content.Add(new TextElement("("));
                    mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.PROFILE));
                    mark.Attributes.Content.Add(new TextElement(")"));
                    break;
                case MarkType.TEXT:
                    if (!string.IsNullOrEmpty(text))
                    {
                        mark.Attributes.Content.Add(new TextElement(text));
                    }
                    else
                    {
                        mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.NAME));
                    }
                    break;
                case MarkType.PANEL_NAME:
                    mark.Attributes.Content.Add(new UserDefinedElement(ExtUDA.PANEL_NAME));
                    break;
            }
        }

        public static MarkType TransformTextToMarkType(this string markSettingName)
        {
            var result = MarkType.PANEL_NAME;
            switch (markSettingName)
            {
                case MarkSetting.ASSEMBLY_POSITION:
                    result = MarkType.ASSEMBLY_POSITION;
                    break;
                case MarkSetting.ASSEMBLY_POSITION_PROFILE:
                    result = MarkType.ASSEMBLY_POSITION_PROFILE;
                    break;
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
                case MarkType.ASSEMBLY_POSITION:
                    result = MarkSetting.ASSEMBLY_POSITION;
                    break;
                case MarkType.ASSEMBLY_POSITION_PROFILE:
                    result = MarkSetting.ASSEMBLY_POSITION_PROFILE;
                    break;
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
