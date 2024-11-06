namespace Web.Support
{
    using CsvHelper.Configuration;
    using CsvHelper;
    using CsvHelper.TypeConversion;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System;

    public partial class CSVConverter
    {
#nullable enable
        static string ReplaceSeparators(string? text)
        {
            if (text is null || text.Equals("NULL", StringComparison.CurrentCultureIgnoreCase))
            {
                return string.Empty;
            }
            if (text.Contains(',') && text.Contains('.'))
            {
                return text.Replace(",", "").Replace('.', ',');
            }
            else if (text.Contains('.'))
            {
                return text.Replace('.', ',');
            }
            return text;
        }
#nullable disable

        public static string AsIntString(string text) => AsInt32(text).ToString();

        public static int AsInt32(decimal value) => (int)value;
        public static int AsInt32(object value) => AsInt32(value.ToString() ?? "");
        public static int AsInt32(string text)
        {
            int numeric;
            try
            {
                text = ReplaceSeparators(text);

                if (text == "")
                {
                    text = "0";
                }

                numeric = Convert.ToInt32(IsNumericRegex().Match(text).Value);
            }
            catch (FormatException)
            {
                // numeric = Convert.ToInt32(text, new CultureInfo("sv-SE"));
                numeric = (int)Convert.ToDecimal(text, new CultureInfo("en-US"));
            }
            return numeric;
        }

        public static decimal AsDecimal(string text)
        {
            decimal numeric;
            try
            {
                text = ReplaceSeparators(text);

                if (text == "")
                {
                    text = "0";
                }

                numeric = Convert.ToDecimal(text);
            }
            catch (FormatException)
            {
                numeric = Convert.ToDecimal(text, new CultureInfo("en-US"));
            }
            return numeric;
        }

        public class ToDecimal : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
                => AsDecimal(text);
        }

        public class ToInt32 : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
                => AsInt32(text);
        }

        public class ToIntString : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
                => AsIntString(text);
        }

        public class ToDateTime : DateTimeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (text.Equals("NULL", StringComparison.CurrentCultureIgnoreCase))
                {
                    return null;
                }
                return base.ConvertFromString(text, row, memberMapData);
            }
        }

        [GeneratedRegex(@"-?\d+")]
        private static partial Regex IsNumericRegex();
    }
}
