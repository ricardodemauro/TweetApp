import React, { useEffect } from 'react';
import { withRouter } from "react-router-dom";
import { useOktaAuth } from '@okta/okta-react';
import Button from 'react-bootstrap/Button';

import { ISSUER } from '../constants';

const redirectUri = `${window.location.origin}/logout`;

// Basic component with logout button
const Logout = withRouter(({ history }) => {
    const { authState, authService } = useOktaAuth();

    useEffect(() => {
        if (authState.isAuthenticated) {
            (async () => await logout())();
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [authState, authService]);

    if (!authState.isAuthenticated) {
        history.push("/");
    }

    const logout = async () => {
        // Read idToken before local session is cleared
        const idToken = authState.idToken;
        await authService.logout('/');

        // Clear remote session
        window.location.href = `${ISSUER}/v1/logout?id_token_hint=${idToken}&post_logout_redirect_uri=${redirectUri}`;
    };

    return (
        <main role="main" className="inner cover">
            <div className="lead">
                <Button onClick={logout} className="btn btn-lg btn-secondary">Logout</Button>
            </div>
        </main>
    );
});

export default Logout;