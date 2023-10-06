namespace TeklaDev
{
    public static class ExtMainWindowViewModel
    {
        private const int _defaultValueInt = -2147483648;
        private const int _defaultValueDouble = -2147483648;
        private const string _defaultValueString = "";
        public static bool IsDefaultValue(this int value)
        {
            return value == _defaultValueInt ? true : false;
        }
        public static bool IsDefaultValue(this double value)
        {
            return value == _defaultValueDouble ? true : false;
        }
        public static bool IsDefaultValue(this string value)
        {
            return value == _defaultValueString ? true : false;
        }
    }
}
