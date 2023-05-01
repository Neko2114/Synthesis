namespace Penumbra_.Localization
{
    public class Localization
    {
        public enum StringErrors
        {
            MetaNotFound
        }
        public enum StringMessages
        {
            ReplaceSimilarAuthors,
            ReplaceSimilarAuthorsHeader
        }
        public static string GetString(StringErrors value)
        {
            switch (value)
            {
                case StringErrors.MetaNotFound:
                    return "???";
                default:
                    return "Bad Enum";
            }
        }
        public static string GetString(StringMessages value)
        {
            switch (value)
            {
                case StringMessages.ReplaceSimilarAuthors:
                    return "Do you want to replace all duplicate instances with the header?";
                case StringMessages.ReplaceSimilarAuthorsHeader:
                    return "Replace All Duplicates";
                default:
                    return "Bad Enum";
            }
        }
    }
}
