import React, {useEffect} from "react";
import {useLogout} from "../useLogin";

export const Logout : React.FC<any> = (props) => {
    const logout = useLogout();

    useEffect(() => {
        logout();
    });

    return <div>You have been logged out.</div>
}