import json, requests
from datetime import datetime

def load_json():
    with open('data/whats_a_pain.json') as f:
        return json.load(f)

def main():
    data = load_json()
    post = data[0]
    replies = data[1]
    create_post(post)

def convert_timestamp(ts):
    dt =datetime.fromtimestamp(ts)
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

    response = requests.post('http://localhost/_dev/as-local',
                json=payload,
                headers={'Accept': 'application/ld+json',
                         'Content-Type': 'application/ld+json'})

    print(response.status_code, response.text)


if __name__ == '__main__':
    main()