import React from "react";
import { initLogState, LoginState } from "./login.state";
import { submit } from "./login.api";

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
                        onChange={e => this.setState(this.state.updateUserName(e.currentTarget.value))}
                    />
                </div>
                <div>
                    Password:
                    <input
                        value={this.state.password}
                        type="password"
                        onChange={e => this.setState(this.state.updatePassword(e.currentTarget.value))}
                    />
                </div>

                <div>
                    <button
                        disabled={this.state.loaderState === "loading"}
                        onClick={() => {
                            this.setState(this.state.setLoaderState("loading"), async () => {
                                try {
                                    await submit({
                                        username: this.state.username,
                                        password: this.state.password,
                                    });
                                    this.setState(this.state.clearFields);
                                    alert("Login successful!");
                                } catch (error) {
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
                        {this.state.showMessage && <div>Welcome back, {this.state.username}!</div>}
                    </div>
                </div>
            </div>
        );
    }
}
