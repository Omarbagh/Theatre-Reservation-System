import { TheatreShowEntry } from "../Home/home.state";

export const loadShows = (): Promise<TheatreShowEntry[]> =>
    fetch("api/v1/shows", {
        method: "GET"
    })
        .then(response => response.json())
        .then(content => content as TheatreShowEntry[])
