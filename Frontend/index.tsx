import React from "react";
import ReactDOM from "react-dom";
import { LoginForm } from "./Login/login";

export const main = () => {
    let rootElement = document.querySelector('#app');
    if (rootElement) {
        ReactDOM.render(<LoginForm />, rootElement);
    }
};

// Roep de main functie aan om de app te starten
main();
