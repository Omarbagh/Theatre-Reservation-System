import { Login } from "../Home/home.state";
const BASE_URL = "http://localhost:5097/api/v1/Login";

export const login = async (data: any) => {
    const response = await fetch(`${BASE_URL}/Login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    });
    return response.json();
};

export const isAdminLoggedIn = async () => {
    const response = await fetch(`${BASE_URL}/IsAdminLoggedIn`, {
        method: "GET",
        credentials: "include", // Use this if cookies/session are required
    });
    if (!response.ok) {
        throw new Error("Failed to fetch admin login status");
    }
    return response.json();
};

