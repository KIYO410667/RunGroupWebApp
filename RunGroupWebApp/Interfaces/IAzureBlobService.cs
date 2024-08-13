namespace RunGroupWebApp.Interfaces
{
    public interface IAzureBlobService
    {
        Task<string> UploadPhotoAsync(Stream fileStream, string fileName);
    }
}