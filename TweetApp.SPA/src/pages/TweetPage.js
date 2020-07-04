import React, { useState, useEffect } from 'react';
import { useOktaAuth } from '@okta/okta-react';
import { TweetBody, CreateTweet } from '../components/tweet';

const localApi = 'http://localhost:7071/api/Tweet/';
const azApi = 'https://tweetappapi0033.azurewebsites.net/api/tweet/';

const api = () => {
  if (window.location.href.indexOf('localhost') > -1) {
    return localApi;
  }
  return azApi;
}

function TweetPage() {
  const { authState, authService } = useOktaAuth();
  const [users = [], setUsers] = useState(0);

  useEffect(() => {
    (async () => await getTweets())();
  }, [authState, authService]);

  async function getTweets() {
    let res = await fetch(api(), {
      method: 'get',
      headers: {
        'Authorization': `Bearer ${authState.accessToken}`,
        'Content-Type': 'application/json'
      }
    });

    let data = await res.json();

    var mappedUsers = data.map((item, i) => {
      return { name: item.user, tweet: item.message }
    });
    setUsers(mappedUsers);
  }

  async function submitTweet(tweetForm) {
    let res = await fetch(api(), {
      method: "POST",
      body: JSON.stringify({ message: tweetForm.tweet }),
      headers: { "Content-Type": "application/json", 'Authorization': `Bearer ${authState.accessToken}` }
    });

    if (res.ok) {
      await res.json();
      await getTweets();
    }
    console.warn('something bad happened', res.statusText);
  }

  return (
    <main role="main" className="h-auto">
      <div className="App">
        <div className="container my-3 p-3 bg-dark rounded shadow-sm">
          <h1>Mini Tweet</h1>
          <div className="row">
            <div className="col-12">
              <CreateTweet onFormSubmit={submitTweet.bind(this)} />
            </div>
          </div>
          <h6 className="border-bottom border-gray pb-2 mb-0">Recent updates</h6>
          <ul className="list-unstyled">
            {Array.isArray(users) && [...users].map((item, index) => {
              return <li className="media shadow p-1 mb-2 bg-white rounded" key={index}><TweetBody {...item} /></li>
            })}
          </ul>
        </div>
      </div>
    </main>
  );
}

export default TweetPage;
