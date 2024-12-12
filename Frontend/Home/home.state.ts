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

export interface TheatreShow {
    theatreid: number;
    title: string;
    description: string;
    price: number;
    venueid: number;
    theatershow: Date;
}
export type TheatreShowEntry = TheatreShow
