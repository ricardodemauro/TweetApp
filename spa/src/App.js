import React, { useState, useEffect } from 'react';
import './App.css';
import { TweetBody, CreateTweet } from './components/tweet';

const api = 'http://localhost:7071/api/Tweet/';

function App() {

  const [users = [], setUsers] = useState(0);
  useEffect(() => {
    async function getUsersAsync() {
      await getUser();
    }

    (async () => await getUsersAsync())();
  }, []);

  async function getUser() {
    let res = await fetch(api)
    let data = await res.json();

    var mappedUsers = data.map((item, i) => {
      return { name: item.user, tweet: item.message }
    });
    setUsers(mappedUsers);
  }

  async function submitTweet(tweetForm) {
    console.log(tweetForm);

    let res = await fetch(api, {
      method: "POST",
      body: JSON.stringify({ message: tweetForm.tweet, user: tweetForm.username }),
      headers: { "Content-Type": "application/json" }
    });

    if (res.ok) {
      let data = await res.json();
      await getUser();
    }
    console.warn('something bad happened', res.statusText);

  }

  return (
    <div className="App">
      <div className="container my-3 p-3 bg-white rounded shadow-sm">
        <h1>Welcome to POC of Tweets</h1>
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
  );
}

export default App;
