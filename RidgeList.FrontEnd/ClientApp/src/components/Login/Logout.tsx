import React, {useEffect, useState} from "react";
import {useHistory} from "react-router-dom";
import {useLogout} from "../useLogin";

export const Logout : React.FC<any> = (props) => {
    const logout = useLogout();

    useEffect(() => {
        logout();
    });

    return <div>You have been logged out.</div>
}