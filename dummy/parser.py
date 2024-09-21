import json, requests
from datetime import datetime

# this script consumes a reddit post json (reddit.com/some/post.json)
# and pumps it into elysium to generate content
session = requests.Session()

def load_json():
    with open('data/whats_a_pain.json') as f:
        return json.load(f)

def main():
    data = load_json()
    post = data[0]
    comments = data[1]
    elysiumPost = create_post(post)
    create_comments(
        elysiumPost['object']['id'],
        elysiumPost['object']['attributedTo'],
        comments)

def convert_timestamp(ts):
    dt = datetime.fromtimestamp(ts)
    return dt.isoformat() + 'Z'

def create_post(post):
    post = post['data']['children'][0]['data']
    author = post['author']
    post = {
        "@context":  "https://www.w3.org/ns/activitystreams",
        "type": "Page",
        "name": post['title'],
        "to": [
            "https://www.w3.org/ns/activitystreams#Public"
        ],
        "published":  convert_timestamp(post['created_utc'])
    }

    payload = {
        "subjectObject": post,
        "actorName": author
    }

    response = session.post('http://localhost/_dev/as-local',
                json=payload,
                headers={'Accept': 'application/ld+json',
                        'Content-Type': 'application/ld+json'})

    print(response.status_code)
    if response.status_code != 200:
        raise ValueError(response.status_code)
    return response.json()

def create_comments(parentIri, parentAuthorIri, comments):
    comments = comments['data']['children']
    for comment in comments:
        if comment['kind'] != 't1':
            continue
        create_comment_and_replies(parentIri, parentAuthorIri, comment)

def create_comment_and_replies(parentIri, parentAuthorIri, comment):
    author = comment['data']['author']
    obj = {
        "@context":  "https://www.w3.org/ns/activitystreams",
        "type": "Note",
        "inReplyTo": parentIri,
        "content": comment['data']['body'],
        "to": "https://www.w3.org/ns/activitystreams#Public",
        "cc": parentAuthorIri,
        "published":  convert_timestamp(comment['data']['created_utc'])
    }
    print(obj)

    payload = {
        "subjectObject": obj,
        "actorName": author
    }

    response = session.post('http://localhost/_dev/as-local',
                json=payload,
                headers={'Accept': 'application/ld+json',
                        'Content-Type': 'application/ld+json'})

    print(response.status_code)
    if response.status_code != 200:
        raise ValueError(response.status_code)
    elysiumComment = response.json()

    if not isinstance(comment['data']['replies'], dict):
        return elysiumComment
    if comment['data']['replies']['kind'] != 'Listing':
        return elysiumComment

    for child in comment['data']['replies']['data']['children']:
        if child['kind'] != 't1':
            continue
        create_comment_and_replies(
            elysiumComment['object']['id'],
            elysiumComment['object']['attributedTo'],
            child)
    return elysiumComment

if __name__ == '__main__':
    main()