namespace Elysium.GrainInterfaces.Services
{
    public interface IHttpMessageAuthor
    {
        Task<string> GetKeyIdAsync();
        Task<string> SignAsync(string stringToSign);
    }
}