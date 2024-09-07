# The instance actor

## links

- https://swicg.github.io/activitypub-http-signature/#instance-actor
- https://seb.jambor.dev/posts/understanding-activitypub-part-4-threads/#the-instance-actor

---

- purpose: avoid signature deadlocks:
  - a sends activity to b, signed with a's private key
  - b wants to verify the signature, so it sends a request to a.com/a to get a's public key
    - b signs this request with b's private key
  - a receives b's request to a.com/a, and wants to verify the signature, so it sends a request to b.com/b, and signs it with a's private key
  - b receives a's request to b.com/b, and wants to verify the signature, so it sends a request to a.com/a ...

- service level actor can perform requests, and this actors public key can be retrieved with an unsigned request


@my-domain.com@my-domain.com

- this actor's profile can be read without any auth