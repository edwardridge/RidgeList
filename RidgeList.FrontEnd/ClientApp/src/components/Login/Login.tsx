import React, { useState, useEffect } from "react";
import { Button } from "react-bootstrap";
import { useHistory } from "react-router-dom";
import {useSetLogin, useGetLogin} from "../useLogin";
import "./Login.css"

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
     
    return (
        <div className='loginWrapper'>
            <h5>Welcome to RidgeList! To login, please enter your name and email address - note that the wishlists are linked to your email address.</h5>
            <div className='form-group mt-lg-4 input-group-lg'>
                <input type="text" className='form-control' id='NameLogin' cypress-name="NameLogin" placeholder="Your name..." onChange={(e) => setLoginName(e.target.value)}/>
            </div>
            <div className='form-group input-group-lg'>
                <input type="text" className='form-control' id='EmailLogin' cypress-name="EmailLogin" placeholder="Your email address..." onChange={(e) => setLoginEmail(e.target.value)}/>
            </div>
            <Button size='lg' variant="success" cypress-name='LoginButton' onClick={loginClicked}>
                Login
            </Button>
        </div>)
}