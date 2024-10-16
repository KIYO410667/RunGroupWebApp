namespace RunGroupWebApp.Interfaces.IService
{
    public interface IAzureBlobService
    {
        Task<string> UploadPhotoAsync(Stream fileStream, string fileName);
        Task<bool> DeletePhotoByUrlAsync(string photoUrl);
    }
}