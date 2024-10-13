using Elysium.GrainInterfaces.Services.GrainFactories;

namespace Elysium.GrainInterfaces.Queueing
{
    public class QueueWorkerGrainFactory(IGrainFactory grainFactory) : IGrainFactory<QueueWorkerIdentity>
    {
        public TGrain GetGrain<TGrain>(QueueWorkerIdentity identity) where TGrain : IGrain<QueueWorkerIdentity>
        {
            var stringIdentity = $"{identity.Id}+{identity.Queue}";
            return grainFactory.GetGrain<TGrain>(stringIdentity);
        }

        public QueueWorkerIdentity GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<QueueWorkerIdentity>
        {
            var stringIdentity = grain.GetPrimaryKeyString();
            var splitIndex = stringIdentity.IndexOf('+');
            if (splitIndex == -1)
                throw new ArgumentException($"invalid identity format: {stringIdentity}");

            var idPart = stringIdentity[..splitIndex];
            var queuePart = stringIdentity[(splitIndex + 1)..];
            var id = int.Parse(idPart);

            return new QueueWorkerIdentity
            {
                Id = id,
                Queue = queuePart
            };
        }
    }
}
