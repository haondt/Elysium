# Authorization

links

- https://www.w3.org/wiki/ActivityPub/Primer/Authentication_Authorization#Authorization
- https://docs.joinmastodon.org/admin/config/#authorized_fetch
- https://w3c-ccg.github.io/security-vocab/#publicKey

---

- if an activity is trying to create an object
  - the activity's `actor` must be the same as the objects `attributedTo` actor

- if an activity is trying modify an object (Update/Delete/Undo/etc)
  - if the _original_ object is itself an activity
    - the activity's `actor` must be the same as the _original_ objects `actor`
  - if the _original_ object is not an activity
    - the activity's `actor` must be the same as the _original_ objects `attributedTo`

- if an activity is being sent or received, it must be signed by it's `actor`
  - OR, in some cases the activity can't be signed by the actor (e.g. inbox forwarding)
  - option 1: if the object id indicates it is from server x, AND the activity author is from server x, then
    - the sender can (optionally) send just the id
    - the receiver can (optionall) retrieve the object by the id to verify the content
  - option 2: verify a linked data signature against the activity's `actor` https://w3c.github.io/vc-data-integrity/

- if an actor is trying to read an object
  - if the actor is an addressee of the object (in the `to`, `cc` or `audience` collections)
    - the actor can read the object
  - otherwise, the actor cannot read the object