import React, { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import Cookies from 'js-cookie';

export const Login : React.FC<any> = (props) => {
    const [loginEmail, setLoginEmail] = useState("");
    const history = useHistory();
    
    useEffect(() => {
        if(Cookies.get('email')){
            history.push('/wishlists');
        }
    });
    
    const loginClicked = () => {
        Cookies.set('email', loginEmail);
        history.push('/wishlists');
    }
    
    return <div>
        <input type="text" cypress-name="EmailLogin" placeholder="Enter your email address..." onChange={(e) => setLoginEmail(e.target.value)}/>
        <button className="btn btn-success" onClick={loginClicked} cypress-name="LoginButton">Login</button>
    </div>
}