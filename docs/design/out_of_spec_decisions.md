# Out of spec decisions

Some arbitrary decisions I am making that may or may not be within the spec.

- reject anything submitted to an inbox that fails the following
  - is not a known extension of [`Activity`](https://www.w3.org/TR/activitystreams-vocabulary/#dfn-activity)
  - missing an actor
  - has more than one actor