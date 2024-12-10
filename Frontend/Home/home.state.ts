// home.state.ts

export interface Login {
    username: string;
    password: string;
}

// Initieel login state
export const initLoginState: Login = {
    username: "",
    password: "",

};
