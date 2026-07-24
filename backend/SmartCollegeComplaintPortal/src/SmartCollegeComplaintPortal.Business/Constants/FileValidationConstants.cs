namespace DotNetApiStarterKit.Business.Constants
{
    public static class FileValidationConstants
    {
        public const long MaxImageSizeInBytes =
            5 * 1024 * 1024;

        public const long MaxDocumentSizeInBytes =
            10 * 1024 * 1024;


        public static readonly HashSet<string> AllowedImageExtensions =
            [
                 ".jpg",
                ".jpeg",
                ".png"
            ];

        public static readonly HashSet<string> AllowedDocumentExtensions =
            [
                ".pdf"
            ];

        public static readonly HashSet<string> AllowedMimeTypes =
            [
                "image/jpeg",
                "image/png",
                "application/pdf"
            ];
    }
}
