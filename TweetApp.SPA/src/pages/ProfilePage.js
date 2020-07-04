import { useOktaAuth } from '@okta/okta-react';
import React, { useState, useEffect } from 'react';

const ProfilePage = () => {
    const { authState, authService } = useOktaAuth();
    const [userInfo, setUserInfo] = useState(null);

    useEffect(() => {
        if (!authState.isAuthenticated) {
            // When user isn't authenticated, forget any user info
            setUserInfo(null);
        } else {
            authService.getUser().then((info) => {
                setUserInfo(info);
            });
        }
    }, [authState, authService]); // Update if authState changes

    return (
        <main role="main" className="h-100">
            <div>
                {userInfo &&
                    <div>
                        <p>Welcome back, {userInfo.name}!</p>
                        <p style={{ "wordBreak": "break-all" }}>Access Token, {authState.accessToken}</p>
                    </div>
                }
            </div>
        </main>
    );
}

export default ProfilePage;