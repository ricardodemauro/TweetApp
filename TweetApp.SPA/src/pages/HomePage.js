import React, { useState, useEffect } from 'react';
import { useOktaAuth } from '@okta/okta-react';
import Button from 'react-bootstrap/Button';

const HomePage = () => {
    const { authState, authService } = useOktaAuth();

    const [userInfo, setUserInfo] = useState(null);

    useEffect(() => {
        (async () => await updateUserInfo())();
    }, [authState, authService]); 

    const updateUserInfo = async () => {
        if (!authState.isAuthenticated) {
            // When user isn't authenticated, forget any user info
            setUserInfo(null);
        } else {
            const info = await authService.getUser();
            setUserInfo(info);
        }
    }

    const login = async () => {
        if (authState.isAuthenticated)
            console.log('you`re already logged in');

        else {
            await authService.login('/');
        }
    };

    return (
        <main role="main" className="inner cover">
            <div className="lead">
                {userInfo &&
                    <p>Welcome back, {userInfo.name}!</p>
                }
                {!userInfo &&
                    <Button onClick={login} className="btn btn-lg btn-secondary">Login with OKTA</Button>
                }
            </div>
        </main >
    )
}

export default HomePage;