# Webfinger

## links

- https://blog.joinmastodon.org/2018/06/how-to-implement-a-basic-activitypub-server/
- https://datatracker.ietf.org/doc/html/rfc7033

---

Provides information about a resource identifier, in the format expected by the site

user handle is @foo@bar.com, where foo is the username and bar.com is the domain.

we can translate this to a webfinger lookup

https://bar.com/.well-known/webfinger?resource=acct:foo%40bar.com

where the sections of the url are
- `https://` - assumed to be on https, though we can try http too
- `bar.com` - the domain
- `/.well-known/webfinger?resource=acct:` - standardized webfinger route
- `foo%40bar.com` - the url escaped handle, foo@bar.com. notice that we took off the leading `@`.

- the webfinger will have info on how to translate the user handle to a uri for activitypub