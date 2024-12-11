import React from "react";
import { initLogState, LoginState } from "./login.state";
import { login, isAdminLoggedIn } from "./login.api";
import { DashboardForm } from "../Dashboard/dashboard";


export class LoginForm extends React.Component<{}, LoginState & { isAdminLoggedInn: boolean }> {
    constructor(props: {}) {
        super(props);
        this.state = {
            ...initLogState,
            isAdminLoggedInn: false,
        }
    }

    render(): JSX.Element {
        const { username, password, loaderState, showMessage, errorMessage, isAdminLoggedInn } = this.state;

        if (isAdminLoggedInn) {
            return <DashboardForm />;
        }

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
                            this.setState(this.state.setLoaderState("loading"));

                            try {
                                const loginResponse = await login({
                                    username: this.state.username,
                                    password: this.state.password,
                                });

                                const adminStatus = await isAdminLoggedIn();

                                if (adminStatus.isLoggedIn) {
                                    this.setState({
                                        isAdminLoggedInn: true,
                                        showMessage: true,
                                        errorMessage: "",
                                    });
                                    alert(`Welcome, Admin ${adminStatus.adminName || "Unknown"}!`);
                                    // this.setState(this.state.updateViewState("dashboard"));
                                } else {
                                    this.setState({
                                        ...this.state,
                                        showMessage: false,
                                        errorMessage: "Login successful, but not an admin.",
                                    });
                                    alert("Login successful!");
                                }

                                this.setState(this.state.clearFields);

                            } catch (error: any) {
                                const errorMessage =
                                    error.message || "An unexpected error occurred. Please try again.";
                                this.setState({
                                    ...this.state,
                                    errorMessage,
                                });
                            } finally {
                                this.setState(this.state.setLoaderState("unloaded"));
                            }
                        }}
                    >
                        {loaderState === "loading" ? "Logging in..." : "Login"}
                    </button>
                </div>


                {/* Error Message */}
                {errorMessage && <div style={{ color: "red" }}>{errorMessage}</div>}
            </div>
        );
    }
}
