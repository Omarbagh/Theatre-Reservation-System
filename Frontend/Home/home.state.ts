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


// export type ViewState = "login" | "dashboard"

// export interface HomeState {
//     view: ViewState,
//     updateViewState: (view: ViewState) => (state: HomeState) => HomeState,

// }

// export const initHomeState: HomeState = {
//     view: "login",
//     updateViewState: (view: ViewState) => (state: HomeState): HomeState => ({
//         ...state,
//         view: view
//     })
// }