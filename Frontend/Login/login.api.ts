import { Login } from "../Home/home.state";

export const submit = async (login: Login): Promise<void> => {
    //this i~s to make an API call
    await fetch("api/v1/Login/Login", {
        method: "POST",
        body: JSON.stringify(login),//stringify = serialize,
        headers: {
            "content-type": "application/json"
        }
    })
}