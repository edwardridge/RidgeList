import React, { useState, useEffect } from "react";
import { useHistory } from "react-router-dom";
import { useSetLogin, useGetLogin } from "../useLogin";
import { useMaterialStyles } from "../useMaterialStyles";
import "./Login.css"
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import Typography from '@material-ui/core/Typography';
import Container from '@material-ui/core/Container';

export const Login : React.FC<any> = (props) => {
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
    
    const loginClicked = () => {
        setLogin(loginEmail, loginName);
        history.push('/wishlists');
    }
     
    return (
        <>
            <div className={classes.paper}>
                <Typography component="h1" variant="h5" color="primary">
                    Welcome to RidgeList!
                </Typography>
                <form className={classes.form} noValidate>
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        id="NameLogin"
                        label="Email Address"
                        name="NameLogin"
                        onChange={(e) => setLoginName(e.target.value)}
                        cypress-name="NameLogin"
                        autoFocus
                    />
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        name="EmailLogin"
                        label="Name"
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
                </form>
            </div>
        </>)
}