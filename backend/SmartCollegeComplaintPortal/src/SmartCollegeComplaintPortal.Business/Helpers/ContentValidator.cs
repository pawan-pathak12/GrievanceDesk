namespace SmartCollegeComplaintPortal.Business.Helpers
{
    public static class ContentValidator
    {
        private static readonly HashSet<string> BlockedWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "fuck", "shit", "idiot", "bastard", "asshole",
            "damn", "bitch", "crap", "stupid", "moron",
            "mugi", "muji", "randi"
        };

        public static (bool IsClean, string? BlockedWord) Validate(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (true, null);

            var words = text.Split(
                new[] { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (BlockedWords.Contains(word.Trim()))
                    return (false, word.Trim());
            }

            return (true, null);
        }

        public static (bool IsClean, string? Message) ValidateComplaint(
            string title, string description)
        {
            var titleCheck = Validate(title);
            if (!titleCheck.IsClean)
                return (false, "Title contains inappropriate content. Please revise.");

            var descCheck = Validate(description);
            if (!descCheck.IsClean)
                return (false, "Description contains inappropriate content. Please revise.");

            return (true, null);
        }
    }
}
