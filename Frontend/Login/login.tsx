import React from "react";
import { initLogState, LoginState } from "./login.state";
import { login, isAdminLoggedIn } from "./login.api";

export class LoginForm extends React.Component<{}, LoginState> {
    constructor(props: {}) {
        super(props);
        this.state = initLogState;
    }

    render(): JSX.Element {
        return (
            <div>
                <div>Welcome to the Login Page</div>

                <div>
                    Username:
                    <input
                        value={this.state.username}
                        onChange={(e) =>
                            this.setState(this.state.updateUserName(e.currentTarget.value))
                        }
                    />
                </div>
                <div>
                    Password:
                    <input
                        value={this.state.password}
                        type="password"
                        onChange={(e) =>
                            this.setState(this.state.updatePassword(e.currentTarget.value))
                        }
                    />
                </div>

                <div>
                    <button
                        disabled={this.state.loaderState === "loading"}
                        onClick={() => {
                            this.setState(this.state.setLoaderState("loading"), async () => {
                                try {
                                    await login({
                                        username: this.state.username,
                                        password: this.state.password,
                                    });

                                    // Check admin status
                                    const adminStatus = await isAdminLoggedIn();
                                    console.log("Admin status response:", adminStatus);

                                    if (adminStatus.isLoggedIn) {
                                        alert(`Welcome, Admin ${adminStatus.adminName || "Unknown"}!`);
                                    } else {
                                        alert("Login successful!");
                                    }

                                    this.setState(this.state.clearFields);
                                } catch (error) {
                                    console.error("Error during login:", error);
                                    alert("Login failed, please try again.");
                                } finally {
                                    this.setState(this.state.setLoaderState("unloaded"));
                                }

                            });
                        }}
                    >
                        Login
                    </button>

                    <div>
                        {this.state.showMessage && (
                            <div>Welcome back, {this.state.username}!</div>
                        )}
                    </div>
                </div>
            </div>
        );
    }
}
