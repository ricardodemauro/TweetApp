import React from "react";
import {
  BrowserRouter as Router,
  Route,
} from "react-router-dom";
import { Security, SecureRoute, LoginCallback } from '@okta/okta-react';

import Header from './components/header';
import Footer from './components/footer';

import HomePage from "./pages/HomePage";
import TweetPage from "./pages/TweetPage";
import AuthPage from "./pages/AuthPage";
import ProfilePage from "./pages/ProfilePage";
import LogoutPage from "./pages/LogoutPage";

import config, { CALLBACK_PATH } from './constants';

export default function App() {
  return (
    <div className="text-center" style={{height: "100%"}}>
      <div className="container d-flex h-100 p-3 mx-auto flex-column">
        <Router>
          <Header />

          <Security {...config}>
            <Route path='/' exact={true} component={HomePage} />

            <SecureRoute path='/app' component={TweetPage} />
            <SecureRoute path='/auth' component={AuthPage} />
            <SecureRoute path='/profile' component={ProfilePage} />
            <Route path={CALLBACK_PATH} component={LoginCallback} />
            <Route path='/logout' component={LogoutPage} />
          </Security>

          <Footer />
        </Router >
      </div>
    </div>

  );
}