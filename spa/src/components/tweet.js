import React, { useState } from 'react';

const Image = ({ name }) => {
    return (
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50" width="36" height="36">
            <circle cx="25" cy="25" r="20" stroke="black" strokeWidth="1" fill="#343a40" />
            <text x="13.5" y="30" className="textSvg" fill="white">{name ? name.substring(0, 2).toUpperCase() : '??'}</text>
        </svg>
    )
}

const Name = (props) => {
    return (<span >{`@${props.name}`}</span>)
}

const Tweet = (props) => {
    return (
        <span>{props.tweet}</span>
    )
}

const TweetBody = ({ image, name, handle, tweet }) => {
    return (
        <React.Fragment>
            <Image name={name} image={image} />
            <div className="media-body px-2">
                <h6 className="mt-0 d-block text-gray-dark"><Name name={name} /></h6>
                <Tweet tweet={tweet} />
            </div>
        </React.Fragment>
    )
}

const CreateTweet = ({ onFormSubmit }) => {
    const [tweet, setTweet] = useState('');
    const [username, setUsername] = useState('');

    function handleSubmit(e) {
        e.preventDefault();
        console.log(tweet, username);

        if (onFormSubmit)
            onFormSubmit({ tweet, username });

        setTweet('');
    }

    return (
        <div className="text-white-50 bg-purple rounded shadow-sm my-3">
            <div className="row p-3">
                <form className="w-100 col-12" onSubmit={handleSubmit.bind(this)}>
                    <p className="h4 mb-4">Submit your tweet</p>
                    <div className="form-group">
                        <input type="text" className="form-control" value={tweet} onChange={(e) => setTweet(e.target.value)} aria-describedby="tweet" placeholder="your tweet" />
                    </div>
                    <div className="form-group">
                        <input type="text" className="form-control" value={username} onChange={(e) => setUsername(e.target.value)} aria-describedby="username" placeholder="your username" />
                    </div>

                    <div className="row lh-100">
                        <div className="col-12">
                            <button type="submit" className="btn btn-primary float-right">Submit</button>
                            <h6 className="mb-0 text-white lh-100">Bootstrap</h6>
                            <small>Since 2011</small>
                        </div>
                    </div>
                </form>
            </div>

        </div>
    )
}

export { TweetBody, CreateTweet }