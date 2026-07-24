namespace DotNetApiStarterKit.Domain.Entities.Auths
{
    public class FileMetadata
    {
        public int FileMetadataId { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string RelativePath { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long Size { get; set; }

        public bool IsPublic { get; set; }

        public DateTime UploadedAt { get; set; }

        public int UploadedByUserId { get; set; }

        public string EntityType { get; set; } = string.Empty;

        public int EntityId { get; set; }
        public bool IsActive { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
