import Cookie from "js-cookie";

export const useGetLogin = () => {
    let email = Cookie.get("email") ?? "__unknown__";
    return email;
}