import React from "react";
import { initLogState, LoginState } from "./login.state";
import { login, isAdminLoggedIn } from "./login.api";

export class LoginForm extends React.Component<{}, LoginState> {
    constructor(props: {}) {
        super(props);
        this.state = initLogState;
    }

    render(): JSX.Element {
        const { username, password, loaderState, showMessage, errorMessage } = this.state;

        return (
            <div>
                <h2>Welcome to the Login Page</h2>

                <div>
                    <label>Username:</label>
                    <input
                        value={username}
                        onChange={(e) =>
                            this.setState(this.state.updateUserName(e.currentTarget.value))
                        }
                    />
                </div>
                <div>
                    <label>Password:</label>
                    <input
                        value={password}
                        type="password"
                        onChange={(e) =>
                            this.setState(this.state.updatePassword(e.currentTarget.value))
                        }
                    />
                </div>

                <div>
                    <button
                        disabled={loaderState === "loading"}
                        onClick={async () => {
                            // Set the loader state to loading
                            this.setState(this.state.setLoaderState("loading"));

                            try {
                                // Call login API
                                const loginResponse = await login({
                                    username: this.state.username,
                                    password: this.state.password,
                                });

                                // Check admin login status
                                const adminStatus = await isAdminLoggedIn();
                                console.log("Admin status response:", adminStatus);

                                if (adminStatus.isLoggedIn) {
                                    this.setState({
                                        ...this.state,
                                        showMessage: true,
                                        errorMessage: "",  // Clear any error message
                                    });
                                    alert(`Welcome, Admin ${adminStatus.adminName || "Unknown"}!`);
                                } else {
                                    this.setState({
                                        ...this.state,
                                        showMessage: false,
                                        errorMessage: "Login successful, but not an admin.",  // Display message for non-admin login
                                    });
                                    alert("Login successful!");
                                }

                                // Clear the form fields
                                this.setState(this.state.clearFields);

                            } catch (error) {
                                console.error("Error during login:", error);
                                this.setState({
                                    ...this.state,
                                    errorMessage: "Login failed, please try again.",  // Set error message
                                });
                            } finally {
                                // Reset the loader state
                                this.setState(this.state.setLoaderState("unloaded"));
                            }
                        }}
                    >
                        {loaderState === "loading" ? "Logging in..." : "Login"}
                    </button>
                </div>

                {/* Conditional rendering of success message */}
                {showMessage && <div>Welcome back, {username}!</div>}

                {/* Error Message Display */}
                {errorMessage && <div style={{ color: "red" }}>{errorMessage}</div>}
            </div>
        );
    }
}
