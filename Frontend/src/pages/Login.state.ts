export interface Login {
    Username: string;
    Password: string;
}

export type LoginState = Login & {
    updateUserName: (name: string) => (state: LoginState) => LoginState;
    updatePassword: (name: string) => (state: LoginState) => LoginState;
};

export const initLogState: LoginState = {
    Username: "Enter username",
    Password: "Enter password",
    updateUserName: (name: string) => (state: LoginState): LoginState => ({
        ...state,
        Username: name,
    }),
    updatePassword: (name: string) => (state: LoginState): LoginState => ({
        ...state,
        Password: name,
    }),
};

// Add and export LoginProps
export interface LoginProps {
    insertLogin: (login: Login) => void;
}
