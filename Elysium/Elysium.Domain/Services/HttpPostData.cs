namespace Elysium.Domain.Services
{
    public class HttpPostData : HttpRequestData
    {
        public required string JsonLdPayload { get; set; }
    }
}
