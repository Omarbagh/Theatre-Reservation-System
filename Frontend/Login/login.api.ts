const BASE_URL = "api/v1/Login";

export const login = async (data: any) => {
    const response = await fetch(`${BASE_URL}/Login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
        credentials: "include",
    });

    if (!response.ok) {
        const errorResponse = await response.json();
        throw new Error(errorResponse.message || "Login failed");
    }

    return response.json();
};

export const isAdminLoggedIn = async () => {
    const response = await fetch(`${BASE_URL}/IsAdminLoggedIn`, {
        method: "GET",
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to fetch admin login status");
    }

    return response.json();
};
