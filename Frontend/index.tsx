import React from "react";
import ReactDOM from "react-dom";
import { LoginForm } from "./Login/login";

export const main = () => {
    let rootElement = document.querySelector('#root');

    ReactDOM.render(<LoginForm />, rootElement);
};
