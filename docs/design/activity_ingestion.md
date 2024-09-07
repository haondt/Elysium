# Activity Ingestion

- verify document has a `@type`
  - if not, return 400

- verify the `@type` is a known type that extends https://www.w3.org/TR/activitystreams-vocabulary/#dfn-activity

- if the source is a remote actor
  - verify document has a signature
    - if not, return 401

  - verify document signature
    - if failed, return 401

- if the activity has an id
  - if the source is a remote actor
    - if the id has me as a host
      - return 403 (I shouldn't send an activity to myself)
    - if the id has a different host than the http source


- if object has an id
  - pull object into seperate jobject, replace object with reference in activity
  - if the objects host is me, discard to object body and replace with local document grain
  - if the objects host is the same as

- if object has no id

# ....

# inbox fowarding

links

 - https://www.w3.org/TR/activitypub/#inbox-forwarding

---

- ghost replies problem
  - b is following a
  - a sends activity to b
  - b, replys to a, and sends activity to a+a's followers
    - b pulls a's followers to generate the recepient list, but a's server refuses (401)
    - b therefore only sends reply to a's inbox
  - a replys to b
    - a's followers are notified of a's reply, but are missing b's reply

- note this is not always the case. i.e. you might not want to forward your reply to all of OP's followers.
Mastodon for example only sends the reply to the replyers followers, the the op and the public stream. Then the OP
can provide a reply collection attached to the original object that viewers may dereference if they choose. The reply
colleciton is presumably ordered based on relevancy, and updated perodically (?)

- inbox forwarding
  - when an activity is received in an actors inbox
  - the server must forward the activity to the `to`, `cc` and `audience` fields of the activity, iff _all_ of the following conditions are met
    - the server has never seen this activity before
    - the values of `to`, `cc`, and/or `audience` contain a collection owned by the server
    - the values of `inReplyTo`,`object`,`target` and/or `tag` are objects owned by the server
      - the server will need to recurse through these items to look for linked objects owned by the server, and SHOULD send a max recursion limit
        - during the recursion, the server shouldn't add additional `to`/`cc`/`audience`, we are only interested in the original ones

- ergo, assuming a server implements inbox forwarding,
  - if the activities `to`, `cc` or `audience` contains an object whose host is the same as the target server, we don't need to send seperately
  - this makes public replies easy, just cc the followers of the target