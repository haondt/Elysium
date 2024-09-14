# ActivityPub

## links

- https://www.w3.org/TR/activitypub
- https://www.w3.org/ns/activitystreams
- https://www.w3.org/TR/activitystreams-vocabulary
- https://www.w3.org/TR/json-ld11
- https://json-ld.org/playground/
- https://www.w3.org/TR/controller-document/#dfn-publickeymultibase
- https://www.w3.org/TR/json-ld-api

---

## audience targeting

- https://www.w3.org/TR/activitystreams-vocabulary/#audienceTargeting

- charlie follows bob
- bob sends dms to alice
  - bob, alice are primary audience
  - no secondary audience

- bob sends like to alice
  - bob, alice are primary audience
  - charlie is secondary audience


- to/cc/bto/bcc are OPTIONAL properties
  - to: public primary audience
  - bto: private primary audience
  - cc: public secondary audience
  - bcc: private secondary audience

- to & cc should be targets of delivery, as well as delivered remotely inside the activity
- bto & bcc should be targets of delivery, but should be removed from the activity

- if something is desired to be hidden
  - we can verify permission with http header validation
  - we can return a 404 or a 403

## types

- activities are objects https://www.w3.org/TR/activitystreams-vocabulary/#dfn-object
- all documents are either objects or links
- a document is a link if either
  - the object contains a type property whose value includes "Link"
  - any of the types included in the type property are defined as extensions of link (e.g. Mention)
  - https://www.w3.org/TR/activitystreams-core/#model
- a link must have an href property https://www.w3.org/TR/activitystreams-core/#link

- objects must have type https://www.w3.org/TR/activitypub/#obj-id
  - all other properties are optional https://www.w3.org/TR/activitystreams-core/#object
- actor must have type (inherited), inbox, outbox, following, followers https://www.w3.org/TR/activitypub/#actor-objects

## serialization

- must use UTF-8 encoding
- must use application/activity+json or application/ld+json mimetype (?)
  - https://www.w3.org/TR/activitystreams-core/#syntaxconventions
- absent properties are represented  by setting the property to null or by omtting the property