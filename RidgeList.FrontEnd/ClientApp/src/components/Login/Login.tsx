import React, { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import Cookies from 'js-cookie';
import {useSetLogin, useGetLogin} from "../useLogin";

export const Login : React.FC<any> = (props) => {
    const [loginEmail, setLoginEmail] = useState("");    
    const [loginName, setLoginName] = useState("");
    const history = useHistory();
    const getLogin = useGetLogin(true);
    const setLogin = useSetLogin();
    
    useEffect(() => {
       if(getLogin.IsLoggedIn) {
            history.push('/wishlists');
        }
    });
    
    const loginClicked = () => {
        setLogin(loginEmail, loginName);
        history.push('/wishlists');
    }
    
    return <div>
        <input type="text" cypress-name="NameLogin" placeholder="Your name..." onChange={(e) => setLoginName(e.target.value)}/>
        <input type="text" cypress-name="EmailLogin" placeholder="Your email address..." onChange={(e) => setLoginEmail(e.target.value)}/>
        <button className="btn btn-success" onClick={loginClicked} cypress-name="LoginButton">Login</button>
    </div>
}