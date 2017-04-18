namespace Parliament.Search.OpenSearch
{
    using Parliament.Search.Properties;
    using System;
    using System.Globalization;

    internal static class Validation
    {
        internal static void ValidateLength(string value, int length)
        {
            if (value.Length > length)
            {
                var message = string.Format(Resources.LengthInvalid, length);
                throw new ArgumentOutOfRangeException("value", message);
            }
        }

        // TODO: Implement
        internal static void ValidatePlainText(string value)
        {
            if (false)
            {
                throw new FormatException(Resources.MarkupInvalid);
            }
        }

        internal static void ValidateNonNegative(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, Resources.NonNegativeInvalid);
            }
        }

        // TODO: Implement
        internal static void ValidateUrlEncoded(string value)
        {
            if (false)
            {
                throw new FormatException(Resources.UrlEncodedInvalid);
            }
        }

        internal static void ValidateLanguage(string value)
        {
            try
            {
                CultureInfo.CreateSpecificCulture(value).ToString();
            }
            catch (CultureNotFoundException e)
            {
                throw new FormatException(Resources.LanguageInvalid, e);
            }
        }
    }
}
