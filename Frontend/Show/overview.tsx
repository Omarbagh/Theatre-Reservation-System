import React from "react"
import { TheatreShowEntry } from "../Home/home.state"
import { initOverview, OverviewState } from "./overview.state"
import { loadShows } from "./overview.api"
import { DashboardForm } from "../Dashboard/dashboard"



//Changing this Overview component to a stateful component since we need the state

export class Overview extends React.Component<{}, OverviewState & { terug: boolean }>{
    constructor(props: {}) {
        super(props)
        this.state = {
            ...initOverview,
            terug: false,
        }
    }


    loadOverview() {
        if (this.state.overviewLoader.kind == "loading") {
            loadShows() //now show is correctly stored inside the state after the promise resolves.
                .then(show =>
                    this.setState(this.state.updateLoader({
                        kind: "loaded",
                        value: show
                    }))
                )
        }
    }

    //lifecycle method of react
    //special method triggered after the component is rendered
    componentDidMount(): void {
        this.loadOverview()
    }
    handleButtonClick = () => {
        this.setState({ terug: true });
    };

    render(): JSX.Element {
        const { terug } = this.state;
        if (terug) {
            return <DashboardForm />;
        }
        return (
            <div>

                <div>
                    <h2>Overview of shows</h2>
                    <br />
                </div>
                {

                    this.state.overviewLoader.kind == "loading" ?
                        <div>
                            Loading ...
                        </div> :
                        this.state.overviewLoader.value.map(
                            show => (
                                <div>
                                    <br />
                                    Title : {show.title}
                                    <br />
                                    Description : {show.description}
                                    <br />

                                </div>
                            )
                        )
                }
                <br />
                <div>
                    <button onClick={this.handleButtonClick}>Go back to Dashboard</button>
                </div>

            </div>
        )

    }
}