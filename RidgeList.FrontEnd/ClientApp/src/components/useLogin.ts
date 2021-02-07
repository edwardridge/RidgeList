import Cookie from "js-cookie";
import { useHistory } from "react-router-dom";
import { UserClient } from "../nswag/api.generated";

export const useGetLogin = (fromLoginPage : boolean) => {
    let history = useHistory();
    
    let cookieLogin = Cookie.get("login") ?? "";
    let login : LoginDetails = new LoginDetails("");
    if(cookieLogin === ""){
        login = LoginDetails.NotLoggedInUser();
    }
    else{
        let json = JSON.parse(cookieLogin);
        login = json as LoginDetails;
    }
    
    if(fromLoginPage === false && login.IsLoggedIn === false){
        history.push('/');
        return login;
    }
    return new LoginDetails(login.UserId);
}

export const useSetLogin = () => {
    return async (email: string, name : string) => {
        var userClient = new UserClient();
        let userId = await userClient.login(email, name);
        const loginDetails = new LoginDetails(userId);
        Cookie.set('login', loginDetails);
        return loginDetails;
    }
}

export const useLogout = () => {
    return () => Cookie.remove('login');
}

export class LoginDetails{
    private static NotLoggedInEmail = "NOT_LOGGED_IN";
    
    constructor(public UserId : string) {
    }
    
    public get IsLoggedIn() {
        return this.UserId !== LoginDetails.NotLoggedInEmail;
    }
    
    public static NotLoggedInUser = ()  => {
        return new LoginDetails(LoginDetails.NotLoggedInEmail);
    }
}