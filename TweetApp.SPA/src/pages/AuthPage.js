import React from "react";
import { useOktaAuth } from '@okta/okta-react';

const AuthPage = () => {
    const { authState, authService } = useOktaAuth();
    const login = () => authService.login('/profile');

    if (authState.isPending) {
        return (
            <div>Loading authentication...</div>
        );
    } else if (!authState.isAuthenticated) {
        return (
            <div>
                <button className="btn btn-primary" onClick={login}>Login</button>
            </div>
        );
    }
    return (<h1 className="bg-white">Something</h1>)
}

export default AuthPage;