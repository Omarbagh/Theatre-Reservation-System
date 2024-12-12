import React from "react";
import { TheatreShowEntry, TheatreShow } from "../Home/home.state";
import { initOverview, OverviewState } from "./overview.state";
import { loadShows, updateShow, deleteShow } from "./overview.api";
import { DashboardForm } from "../Dashboard/dashboard";

export class Overview extends React.Component<{}, OverviewState & { terug: boolean; editingShow: TheatreShow | null, showToDelete: TheatreShow | null }>{
    constructor(props: {}) {
        super(props);
        this.state = {
            ...initOverview,
            terug: false,
            editingShow: null,
            showToDelete: null, // Track which show is selected for deletion
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
                    this.setState({ editingShow: null }); // Close the form after saving
                    this.loadOverview(); // Reload the list of shows
                })
                .catch((err) => console.error("Failed to update show:", err));
        }
    };

    handleDeleteClick = (show: TheatreShow) => {
        // Debugging step: Log the show and its theatreid
        console.log("Deleting show:", show);

        // Ensure that show.theatreid is correctly passed to deleteShow
        if (show.theatreid) {
            this.setState({ showToDelete: show });
        } else {
            console.error("Invalid show ID:", show);
        }
    };

    handleConfirmDelete = () => {
        const { showToDelete } = this.state;

        // Debugging step: Check showToDelete and its theatreid
        console.log("Confirming delete for show:", showToDelete);

        if (showToDelete && showToDelete.theatreid) {
            deleteShow(showToDelete.theatreid)  // Make sure you use showToDelete.theatreid
                .then(() => {
                    this.setState({ showToDelete: null });
                    this.loadOverview(); // Reload the list of shows after deletion
                })
                .catch((err) => console.error("Failed to delete show:", err));
        } else {
            console.error("Show ID is missing for deletion", showToDelete);
        }
    };

    handleCancelDelete = () => {
        this.setState({ showToDelete: null }); // Close the confirmation dialog without deleting
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
        const { terug, editingShow, showToDelete } = this.state;
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
                        <div key={show.theatreid}>  {/* Make sure theatreid is unique */}
                            <br />
                            <div>Title: {show.title}</div>
                            <div>Description: {show.description}</div>
                            <br />
                            <button onClick={() => this.handleEditClick(show)}>Edit</button>
                            <button onClick={() => this.handleDeleteClick(show)}>Delete</button>
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

                {showToDelete && (
                    <div className="confirmation-dialog">
                        <p>Are you sure you want to delete this show?</p>
                        <button onClick={this.handleConfirmDelete}>Yes, delete</button>
                        <button onClick={this.handleCancelDelete}>Cancel</button>
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
