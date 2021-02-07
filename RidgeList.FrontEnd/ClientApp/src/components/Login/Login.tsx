import React, { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import {useSetLogin, useGetLogin, LoginDetails} from "../useLogin";
import { useMaterialStyles } from "../useMaterialStyles";
import "./Login.css"
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';

interface LoginProps{
    setLoginVal : (loginVal : LoginDetails) => void
}

export const Login = (props : LoginProps) => {
    const [loginEmail, setLoginEmail] = useState("");    
    const [loginName, setLoginName] = useState("");
    const history = useHistory();
    const getLogin = useGetLogin(true);
    const setLogin = useSetLogin();
    const classes = useMaterialStyles();
    
    useEffect(() => {
       if(getLogin.IsLoggedIn) {
            history.push('/wishlists');
        }
    });
    
    const loginClicked = async () => {
        var loginDeets = await setLogin(loginEmail, loginName);
        props.setLoginVal(loginDeets);
        history.push('/wishlists');
    }
     
    return (
        <>
            <div className={classes.paper}>
                <Typography component="h1" variant="h5" color="primary">
                    Welcome to RidgeList!
                </Typography>
                <div className={classes.form}>
                    <TextField
                        autoFocus
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        id="NameLogin"
                        label="Name"
                        name="NameLogin"
                        onChange={(e) => setLoginName(e.target.value)}
                        cypress-name="NameLogin"

                    />
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        name="EmailLogin"
                        label="Email"
                        type="text"
                        id="EmailLogin"
                        cypress-name="EmailLogin"
                        autoComplete="email"
                        onChange={(e) => setLoginEmail(e.target.value)}
                    />
                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        color="primary"
                        className={classes.submit}
                        cypress-name='LoginButton'
                        onClick={loginClicked}
                    >
                        Enter
          </Button>
                </div>
            </div>
        </>)
}