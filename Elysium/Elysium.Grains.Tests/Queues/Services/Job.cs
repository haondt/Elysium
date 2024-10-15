namespace Elysium.Grains.Tests.Queues.Services
{
    [GenerateSerializer]
    public class Job
    {
        [Id(0)]
        public required string SourceQueue { get; set; }
        [Id(1)]
        public required string Payload { get; set; }
        [Id(2)]
        public required Guid Id { get; set; }
    }
}
