import { TheatreShowEntry } from "../Home/home.state"

export type OverviewLoader = {
    kind: "loading"
} | {
    kind: "loaded",
    value: TheatreShowEntry[]
}

export type DeleteLoader = "unloaded" | "loading" | "loaded"

//implementing the state for the overview
export type Mode = "edit" | "view"
export interface OverviewState {
    overviewLoader: OverviewLoader
    updateLoader: (overviewLoader: OverviewLoader) => (state: OverviewState) => OverviewState
}

export const initOverview: OverviewState = {
    overviewLoader: { kind: "loading" },

    updateLoader: (overviewLoader: OverviewLoader) => (state: OverviewState): OverviewState => ({
        ...state,
        overviewLoader: overviewLoader
    })
}
