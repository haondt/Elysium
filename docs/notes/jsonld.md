
## json ld

## links

- https://www.w3.org/TR/json-ld11

---

- rdf triple: (conceptually) 3 pieces of data
  - subject: what is being described
  - predicate: what part of the subject is being defined
  - value: the description
  - e.g (s, p, v) = (noah, favorite color, black)
- an rdf "node" considsts of an rdf triple
  - subject: iri or blank node
  - predicate: iri
  - object: iri, blank node or literal
- a jld node object
  - represents 0 or more properties of an rdf node in the graph of the jld document
  - a map ({}) is a node object if
    - it exists outside the @context
    - it does not contain @value, @list or @set
    - it is not the top-most map in the jdl document consisting of no other entries than @graph and @context
- a jld node identifier
  - a node is identified by using the @id keyword
- dereferencing
  - given something like

```json
{
  "@context": {
    "foaf": "http://xmlns.com/foaf/0.1/"
  },
  "@id": "https://example.org/people/alice",
  "foaf:name": "Alice"
}
```
  we would dereference the document by replacing the (entire) object with whatever GET https://example.org/people/alice returns
  since the server may return more detailed object.

  thus the implication here is that the above object contains a trimmed down version of the document at the @id

### expansion

 - https://www.w3.org/TR/json-ld-api/#expansion

> Expansion has two important goals: removing any contextual information from the document, and ensuring all values are represented in a regular form.

- expansion converts all values in the context to "expanded form". i.e. the keys will be iris, and the values will be a json array full of objects
- each key corresponds to the items @id (iri)
- each value in the array corresponds to the items value wrt the context