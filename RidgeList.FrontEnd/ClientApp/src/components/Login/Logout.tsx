import React, {useEffect} from "react";
import {LoginDetails, useLogout} from "../useLogin";

interface LogoutProps{
    setLoginVal : (loginVal : LoginDetails) => void;
}

export const Logout = (props : LogoutProps) => {
    const logout = useLogout();

    useEffect(() => {
        logout();
        props.setLoginVal(LoginDetails.NotLoggedInUser());
    });

    return <div>You have been logged out.</div>
}