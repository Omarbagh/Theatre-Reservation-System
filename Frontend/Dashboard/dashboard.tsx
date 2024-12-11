import React from "react";
import { Overview } from "../Show/overview"


export class DashboardForm extends React.Component<{}, { terug: boolean }> {
    constructor(props: {}) {
        super(props);
        this.state = { terug: false };
    }

    handleButtonClick = () => {
        this.setState({ terug: true });
    };

    render(): JSX.Element {
        const { terug } = this.state;
        if (terug) {
            return <Overview />;
        }
        return (
            <div>
                <h2>Welcome to the Dashboard!</h2>
                <p>You are now logged in and can view your dashboard.</p>
                <button onClick={this.handleButtonClick}>Go to Overview</button>
            </div>
        );
    }
}
