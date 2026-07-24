namespace DotNetApiStarterKit.Business.Interfaces.Auths
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(
          Stream fileStream,
          string fileName,
          string contentType,
          string folder,
          CancellationToken cancellationToken = default);

        Task DeleteAsync(
            string blobPath,
            CancellationToken cancellationToken = default);
    }
}
