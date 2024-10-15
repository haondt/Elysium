using Elysium.ActivityPub.Helpers;
using Elysium.ActivityPub.Models;
using Elysium.Core.Models;
using Elysium.Domain.Services;
using Elysium.GrainInterfaces.Collections;
using Elysium.GrainInterfaces.LocalActor;
using Elysium.GrainInterfaces.Services.GrainFactories;
using Elysium.Grains.Queueing;
using Elysium.Hosting.Services;
using System.Collections.Concurrent;

namespace Elysium.Grains.LocalActor
{
    public class LocalActorOutgoingProcessingQueueConsumer(
        IGrainFactory<LocalIri> localIriGrainFactory,
        IGrainFactory grainFactory,
        IDocumentService documentService,
        IHostingService hostingService) : IQueueConsumer<LocalActorOutgoingProcessingData>
    {
        private readonly IPublicCollectionGrain _publicCollectionGrain = grainFactory.GetGrain<IPublicCollectionGrain>(Guid.Empty);

        public async Task ConsumeAsync(LocalActorOutgoingProcessingData payload)
        {
            var authorGrain = localIriGrainFactory.GetGrain<ILocalActorAuthorGrain>(payload.ActorIri);

            // create list of inboxes
            List<LocalIri> localRecipientIris = [];
            List<RemoteIri> remoteRecipientInboxes = [];
            List<Func<Task>> sendTasks = [];
            ConcurrentBag<(Iri Target, string Reason)> failures = [];

            // old todo: for each recepient, look them up
            // the lookup should be done by the instance actor
            // then retrieve the inbox(es)
            // if the inbox is a collection, recursively resolve it
            // but limit the recursion depth
            // also remove me from the final list of inboxes
            // this will be handled by a worker grain? or a dispatcher grain maybe... it will have both local and remote targets
            foreach (var recipient in payload.Recipients)
            {
                // https://www.w3.org/TR/activitypub/#delivery
                // we need to 1) lookup the recepient (using instance actor)
                // 0) if any recepient is me, ignore that
                // 2) if the recepient is an actor (meaning it has an inbox, outbox, following and followers)
                // then deliver to the inbox property
                // 2.1) if the recepient is a collection or ordered collection
                // 3) if the collection is the public collection
                // 4) do not deliver it
                // 3.1) otherwise, recurse on the items of the collection, with a depth limit
                // 2.2) if the recepient is something else?
                // 3) idk bro throw an error
                // all the GETs should be authored by the instance grain

                if (recipient == ActivityPubConstants.PUBLIC_COLLECTION.Iri)
                {
                    await _publicCollectionGrain.IngestReferenceAsync(payload.ActivityType, payload.ActivityIri.Iri);
                    continue;
                }

                if (hostingService.Host == recipient.Host)
                {
                    var localIri = new LocalIri { Iri = recipient };
                    var document = await documentService.GetExpandedDocumentAsync(authorGrain, localIri);
                    if (!document.IsSuccessful)
                    {
                        failures.Add((recipient, $"retriving local document {recipient} failed with reason {document.Reason}"));
                        continue;
                    }

                    if (ActivityPubJsonNavigator.IsActor(document.Value))
                    {
                        localRecipientIris.Add(localIri);
                        continue;
                    }

                    // todo: recursion
                    throw new NotImplementedException();





                }
                else
                {
                    throw new NotImplementedException(); // this is implemented wrong
                    //var remoteUri = new RemoteIri { Iri = recipient };
                    //sendTasks.Add(async () =>
                    //{
                    //    var actorState = await _httpService.GetAsync(new HttpGetData
                    //    {
                    //        Author = _instanceAuthorGrain,
                    //        Target = remoteUri
                    //    });

                    //    // todo: recursively resolve inboxes
                    //    // and pass inbox post job over to dispatch grain
                    //    throw new NotImplementedException(); 
                    //});
                }
            }

            localRecipientIris = localRecipientIris.Distinct().Where(r => r.Iri != payload.ActorIri).ToList();
            remoteRecipientInboxes = remoteRecipientInboxes.Distinct().ToList();

            await Task.WhenAll(localRecipientIris.Select(r =>
            {
                try
                {

                    var localActorGrain = localIriGrainFactory.GetGrain<ILocalActorGrain>(r);
                    return localActorGrain.IngestActivityAsync(payload.ActorIri, payload.ActivityType, payload.Activity);
                }
                catch (Exception ex)
                {
                    failures.Add((r.Iri, ex.ToString()));
                    return Task.CompletedTask;
                }

            }).Concat(remoteRecipientInboxes.Select(async r =>
            {
                throw new NotImplementedException();
            })));


            // todo: log failures

        }
    }
}
