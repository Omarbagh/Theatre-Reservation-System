import { TheatreShowEntry, TheatreShow } from "../Home/home.state";

export const loadShows = (): Promise<TheatreShowEntry[]> =>
    fetch("api/v1/shows", {
        method: "GET",
    })
        .then(response => response.json())
        .then(content => content as TheatreShowEntry[]);

export const updateShow = (id: number, updatedShow: TheatreShow): Promise<Response> =>
    fetch(`api/v1/shows/UpdateShow/${id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(updatedShow),
    });

export const deleteShow = (id: number): Promise<Response> =>
    fetch(`api/v1/shows/DeleteShow/${id}`, {
        method: "DELETE",
    });
