import React, { useState } from 'react';
import Konami from 'react-konami-code';

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
        <span className="text-dark">{props.tweet}</span>
    )
}

const TweetBody = ({ image, name, handle, tweet }) => {
    return (
        <React.Fragment>
            <Image name={name} image={image} />
            <div className="media-body px-2 text-justify">
                <h6 className="mt-0 d-block text-dark"><Name name={name} /></h6>
                <Tweet tweet={tweet} />
            </div>
        </React.Fragment>
    )
}

const CreateTweet = ({ onFormSubmit }) => {
    const [tweet, setTweet] = useState('');

    function handleSubmit(e) {
        e.preventDefault();

        if (onFormSubmit)
            onFormSubmit({ tweet });

        setTweet('');
    }

    function easterEgg() {
        alert('Well done! Konami code activated');
    }

    return (
        <div className="text-white-50 rounded shadow-sm my-3 bg-purple">
            <div className="row p-3">
                <form className="w-100 col-12" onSubmit={handleSubmit.bind(this)}>
                    <p className="h4 mb-4">Submit your tweet</p>
                    <div className="form-group">
                        <input type="text" className="form-control" value={tweet} onChange={(e) => setTweet(e.target.value)} aria-describedby="tweet" placeholder="your tweet" />
                    </div>
                    <Konami className="blink" action={easterEgg.bind(this)}>
                        {"Hey, I'm an Easter Egg! Look at me!"}
                    </Konami>
                    <div className="row lh-100">
                        <div className="col-12">
                            <button type="submit" className="btn btn-primary" style={{ position: "absolute", "right": "15px" }}>Submit</button>
                            <h6 className="mb-0 text-white lh-100">Bootstrap - with Konami Code</h6>
                            <small>Since 2011</small>
                        </div>
                    </div>
                </form>
            </div>
        </div>

    )
}

export { TweetBody, CreateTweet }