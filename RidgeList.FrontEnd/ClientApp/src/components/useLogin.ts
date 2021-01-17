import Cookie from "js-cookie";
import { useHistory } from "react-router-dom";

export const useGetLogin = (fromLoginPage : boolean) => {
    let history = useHistory();
    
    let cookieLogin = Cookie.get("login") ?? "";
    let login : LoginDetails = new LoginDetails("", "");
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
    return new LoginDetails(login.Email, login.Name);
}

export const useSetLogin = () => {
    return (email: string, name: string) => {
        const loginDetails = new LoginDetails(email, name);
        Cookie.set('login', loginDetails);
    }
}

export const useLogout = () => {
    return () => Cookie.remove('login');
}

export class LoginDetails{
    private static NotLoggedInEmail = "NOT_LOGGED_IN";
    
    constructor(public Email : string, public Name : string) {
    }
    
    public get IsLoggedIn() {
        return this.Email !== LoginDetails.NotLoggedInEmail;
    }
    
    public static NotLoggedInUser = ()  => {
        return new LoginDetails(LoginDetails.NotLoggedInEmail, "");
    }
}