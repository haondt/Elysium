# Authorization

links

- https://www.w3.org/wiki/ActivityPub/Primer/Authentication_Authorization#Authorization

---

- if an activity is trying to create an object
  - the activity's `actor` must be the same as the objects `attributedTo` actor

- if an activity is trying modify an object (Update/Delete/Undo/etc)
  - if the _original_ object is itself an activity
    - the activity's `actor` must be the same as the _original_ objects `actor`
  - if the _original_ object is not an activity
    - the activity's `actor` must be the same as the _original_ objects `attributedTo`
