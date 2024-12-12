import React from "react";
import { TheatreShowEntry, TheatreShow } from "../Home/home.state";
import { initOverview, OverviewState } from "./overview.state";
import { loadShows, updateShow } from "./overview.api";
import { DashboardForm } from "../Dashboard/dashboard";

export class Overview extends React.Component<{}, OverviewState & { terug: boolean; editingShow: TheatreShow | null }>{
    constructor(props: {}) {
        super(props);
        this.state = {
            ...initOverview,
            terug: false,
            editingShow: null, // Houd bij welke show wordt bewerkt
        };
    }

    loadOverview() {
        if (this.state.overviewLoader.kind === "loading") {
            loadShows()
                .then((shows) =>
                    this.setState({
                        overviewLoader: {
                            kind: "loaded",
                            value: shows,
                        },
                    })
                )
                .catch((err) => console.error("Failed to load shows:", err));
        }
    }

    componentDidMount(): void {
        this.loadOverview();
    }

    handleButtonClick = () => {
        this.setState({ terug: true });
    };

    handleEditClick = (show: TheatreShow) => {
        this.setState({ editingShow: show });
    };

    handleSaveClick = () => {
        const { editingShow } = this.state;

        if (editingShow) {
            updateShow(editingShow.theatreid, editingShow)
                .then(() => {
                    this.setState({ editingShow: null }); // Sluit het formulier na opslaan
                    this.loadOverview(); // Herlaad de lijst van shows
                })
                .catch((err) => console.error("Failed to update show:", err));
        }
    };

    handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;

        this.setState((prevState) => ({
            editingShow: {
                ...prevState.editingShow!,
                [name]: value,
            },
        }));
    };

    render(): JSX.Element {
        const { terug, editingShow } = this.state;
        if (terug) {
            return <DashboardForm />;
        }

        return (
            <div>
                <div>
                    <h2>Overview of shows</h2>
                    <br />
                </div>
                {this.state.overviewLoader.kind === "loading" ? (
                    <div>Loading ...</div>
                ) : (
                    this.state.overviewLoader.value.map((show) => (
                        <div key={show.theatreid}>
                            <br />
                            <div>Title: {show.title}</div>
                            <div>Description: {show.description}</div>
                            <br />
                            <button onClick={() => this.handleEditClick(show)}>Edit</button>
                        </div>
                    ))
                )}

                {editingShow && (
                    <div>
                        <h3>Edit Show</h3>
                        <form>
                            <label>
                                Title:
                                <input
                                    type="text"
                                    name="title"
                                    value={editingShow.title}
                                    onChange={this.handleChange}
                                />
                            </label>
                            <br />
                            <label>
                                Description:
                                <textarea
                                    name="description"
                                    value={editingShow.description}
                                    onChange={this.handleChange}
                                ></textarea>
                            </label>
                            <br />
                            <button type="button" onClick={this.handleSaveClick}>Save</button>
                            <button type="button" onClick={() => this.setState({ editingShow: null })}>
                                Cancel
                            </button>
                        </form>
                    </div>
                )}

                <br />
                <div>
                    <button onClick={this.handleButtonClick}>Go back to Dashboard</button>
                </div>
            </div>
        );
    }
}
