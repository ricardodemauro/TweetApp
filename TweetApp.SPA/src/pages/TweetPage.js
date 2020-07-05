import React, { useState, useEffect } from 'react';
import { useOktaAuth } from '@okta/okta-react';
import { TweetBody, CreateTweet } from '../components/tweet';
import LoadingContainer from '../components/loadingContainer'
import { apiUri } from '../constants';

const api = () => apiUri();

function TweetPage() {
  const { authState, authService } = useOktaAuth();
  const [tweets = [], setTweets] = useState(0);
  const [loaded, setLoaded] = useState(false);

  useEffect(() => {
    (async () => await getTweets())();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [authState, authService]);

  async function getTweets() {
    setLoaded(false);

    let res = await fetch(api(), {
      method: 'get',
      headers: {
        'Authorization': `Bearer ${authState.accessToken}`,
        'Content-Type': 'application/json'
      }
    });

    const data = await res.json();

    var mappedTweets = data.map((item, i) => {
      return { name: item.user, tweet: item.message }
    });
    setTweets(mappedTweets);

    setLoaded(true);
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
          <div className="row">
            <div className="col-12">
              <h6 className="border-bottom border-gray pb-2 mb-0">Recent updates</h6>

              {!loaded &&
                <div className="col-12 mt-4">
                  <LoadingContainer />
                </div>}
              {loaded && <ul className="list-unstyled">
                {Array.isArray(tweets) && [...tweets].map((item, index) => {
                  return <li className="media shadow p-1 mb-2 bg-white rounded" key={index}><TweetBody {...item} /></li>
                })}
              </ul>}
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}

export default TweetPage;
