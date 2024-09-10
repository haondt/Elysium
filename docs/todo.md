## todos
- for the front end, we oughta use websockets. see https://v1.htmx.org/extensions/web-sockets/
- whitelist http and https uri schemes https://www.w3.org/TR/activitypub/#security-uri-schemes
- webfinger
- research and implement http caching headers
- research and implement http etag headers
- data import/export
  - following list csv - this is compatible with mastodon
- content sourcing
  - mastodon style timelines (in this case, the objects are toots that were either created or boosted)
    - https://old.reddit.com/r/Mastodon/comments/yrjq1y/seeing_followers_from_other_servers/ivvfsxw/
    - home - objects from your following (your inbox)
    - local - public objects from anyone on your instance  (all local outboxes w/ public)
    - federated - public objects from anyone followed by someone on your instance (all local inboxes w/ public)
- make `IElysiumStorage` transient
- add a settings object to configure htmx & hyperscript versions. This can be run through the `ScriptDescriptor`
- instance actor
- grain streams with something more persistent / fault tolerant
- break up api projects
  - Elysium.WebFinger
  - Elysium.WebFinger.Api
  - Elysium.Api (for mobile app)
  - Elysium.ActivityPub.Api
- add persistent state storage provider for grain
- rename UserCryptoService
- remote StorageKeyGrain... we can just talk to the database. the documents are small and don't need caching
  - although... consider the storagekeygrain provides an interface for the client to interact with a seperate silo db
  - maybe keep it and replace the user store with a grain backing
  - maybe also add option to disable cachine on storagekeygrains? or a really short ttl, maybe both as appsettings per grain type
- build something into the typedactorservice to resolve the data for the LocalActorState (inbox, user scoping url, actor uri, etc)


## currently doing
- get message sending successfully