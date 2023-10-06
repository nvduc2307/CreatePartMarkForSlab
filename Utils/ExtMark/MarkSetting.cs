using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;

namespace TeklaDev
{
    public enum MarkType
    {
        ASSEMBLY_POSITION,
        ASSEMBLY_POSITION_PROFILE,
        TEXT_PROFILE,
        TEXT
    }
    public static class MarkSetting
    {
        public const string ASSEMBLY_POSITION = "ASSEMBLY_POSITION";
        public const string ASSEMBLY_POSITION_PROFILE = "ASSEMBLY_POSITION_PROFILE";
        public const string TEXT_PROFILE = "TEXT_PROFILE";
        public const string TEXT = "TEXT";

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
                case MarkType.TEXT_PROFILE:
                    if (!string.IsNullOrEmpty(text)) mark.Attributes.Content.Add(new TextElement(text));
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
            }
        }

    }
}
