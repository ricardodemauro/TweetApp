import React, { useState, useEffect } from 'react';
import { useOktaAuth } from '@okta/okta-react';
import Button from 'react-bootstrap/Button';
import LoadingContainer from '../components/loadingContainer'

const HomePage = () => {
    const { authState, authService } = useOktaAuth();
    const [userInfo, setUserInfo] = useState(null);
    const [loaded, setLoaded] = useState(false);

    useEffect(() => {
        const updateUserInfo = async () => {
            if (!authState.isAuthenticated) {
                // When user isn't authenticated, forget any user info
                setUserInfo(null);

                setLoaded(true);
            } else {
                const info = await authService.getUser();
                setUserInfo(info);

                setLoaded(true);
            }
        }

        (async () => await updateUserInfo())();
    }, [authState, authService]);

    const login = async () => {
        if (authState.isAuthenticated)
            console.log('you`re already logged in. Do nothing');
        else {
            await authService.login('/');
        }
    };

    return (
        <main role="main" className="inner cover">
            <div className="lead">
                {!loaded && <LoadingContainer />}

                {loaded && userInfo &&
                    <p>Welcome back, {userInfo.name}!</p>
                }
                {loaded && !userInfo &&
                    <Button onClick={login} className="btn btn-lg btn-secondary">Login with OKTA</Button>
                }
            </div>
        </main >
    )
}

export default HomePage;