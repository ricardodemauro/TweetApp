import React from "react";
import { NavLink } from "react-router-dom";

function Header() {
    return (
        <header className="masthead mb-auto">
            <div className="inner">
                <h3 className="masthead-brand">Cover</h3>
                <nav className="nav nav-masthead justify-content-center">
                    <NavLink className="nav-link" to="/" exact>Home</NavLink>
                    <NavLink className="nav-link" to="/app">Tweets</NavLink>
                    {/* <NavLink className="nav-link" to="/about">About</NavLink> */}
                    <NavLink className="nav-link" to="/profile">Profile</NavLink>
                    <NavLink className="nav-link" to="/logout">LogOut</NavLink>
                </nav>
            </div>
        </header>
    );
}

export default Header;