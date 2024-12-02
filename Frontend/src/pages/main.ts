import { initLogState } from "./Login.state";

const loginState = initLogState;

// Handle user input and update state
document.getElementById("loginButton")?.addEventListener("click", () => {
    const username = (document.getElementById("username") as HTMLInputElement).value;
    const password = (document.getElementById("password") as HTMLInputElement).value;

    // Update state
    const updatedState = loginState
        .updateUserName(username)(loginState)
        .updatePassword(password)(loginState);

    // Display updated state
    document.getElementById("output")!.textContent = JSON.stringify(updatedState);
});
