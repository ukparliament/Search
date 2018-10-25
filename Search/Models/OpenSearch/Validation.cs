namespace OpenSearch
{
    using System;
    using System.Globalization;

    internal static class Validation
    {
        internal static void ValidateLength(string value, int length)
        {
            if (value.Length > length)
            {
                var message = string.Format("The value must conform to the XML 1.0 Language Identification, as specified by RFC 5646. In addition, the value of '*' will signify that the search engine does not restrict search results to any particular language.", length);
                throw new ArgumentOutOfRangeException("value", message);
            }
        }

        // TODO: Implement
        internal static void ValidatePlainText(string value)
        {
            if (false)
            {
                throw new FormatException("The value must not contain HTML or other markup.");
            }
        }

        internal static void ValidateNonNegative(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "The value must be a non-negative integer.");
            }
        }

        // TODO: Implement
        internal static void ValidateUrlEncoded(string value)
        {
            if (false)
            {
                throw new FormatException("The value must be URL-encoded.");
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
                throw new FormatException("The value must conform to the XML 1.0 Language Identification, as specified by RFC 5646. In addition, the value of '*' will signify that the search engine does not restrict search results to any particular language.", e);
            }
        }
    }
}
