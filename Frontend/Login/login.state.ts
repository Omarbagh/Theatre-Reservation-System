import { Login } from "../Home/home.state";

export type LoaderState = "unloaded" | "loading" | "loaded";

export type LoginState = Login & {
    loaderState: LoaderState;
    updateUserName: (name: string) => (state: LoginState) => LoginState;
    updatePassword: (name: string) => (state: LoginState) => LoginState;
    clearFields: (state: LoginState) => LoginState;
    showMessage: boolean;
    setLoaderState: (loaderState: LoaderState) => (state: LoginState) => LoginState;
    errorMessage: string;
};

export const initLogState: LoginState = {
    loaderState: "unloaded",
    username: "",
    password: "",
    showMessage: false,
    errorMessage: "",
    updateUserName: (name: string) => (state: LoginState): LoginState => ({
        ...state,
        username: name,
    }),
    updatePassword: (name: string) => (state: LoginState): LoginState => ({
        ...state,
        password: name,
    }),
    clearFields: (state: LoginState): LoginState => ({
        ...state,
        username: "",
        password: "",
    }),
    setLoaderState: (loaderState: LoaderState) => (state: LoginState): LoginState => ({
        ...state,
        loaderState: loaderState,
    }),
};
