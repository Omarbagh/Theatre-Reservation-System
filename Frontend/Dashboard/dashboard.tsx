import React from "react";

export class DashboardForm extends React.Component<{}, {}> {
    constructor(props: {}) {
        super(props);
        this.state = {};
    }

    render(): JSX.Element {
        return (
            <div>
                <h2>Welcome to the Dashboard!</h2>
                <p>You are now logged in and can view your dashboard.</p>
            </div>
        );
    }
}
