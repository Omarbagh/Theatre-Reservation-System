import React from "react";
import { LoginProps, LoginState, initLogState } from "../pages/Login.state"; // Ensure correct path

export class LoginForm extends React.Component<LoginProps, LoginState> {
    constructor(props: LoginProps) {
        super(props);
        this.state = initLogState;
    }

    async handleLogin() {
        const { Username, Password } = this.state;
        try {
            const response = await fetch('http://localhost:5097/api/v1/Login/Login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ Username, Password }),
            });

            if (response.ok) {
                const result = await response.text();
                alert(result); // Display success message or handle it accordingly.
            } else {
                const errorText = await response.text();
                alert(errorText); // Display error message.
            }
        } catch (error) {
            console.error('Error logging in:', error);
            alert('An error occurred while logging in.');
        }
    }

    render(): JSX.Element {
        return (
            <div>
                <div> Welcome to our Login Page </div>
                <div>
                    Username:
                    <input
                        value={this.state.Username}
                        onChange={e => this.setState(this.state.updateUserName(e.currentTarget.value))}
                    />
                </div>
                <div>
                    Password:
                    <input
                        value={this.state.Password}
                        onChange={e => this.setState(this.state.updatePassword(e.currentTarget.value))}
                        type="password"
                    />
                </div>
                <div>
                    <button
                        onClick={_ => {
                            this.props.insertLogin({
                                Username: this.state.Username,
                                Password: this.state.Password
                            });
                            this.handleLogin(); // Trigger the API call
                        }}
                    >
                        Submit
                    </button>
                </div>
            </div>
        );
    }
}
