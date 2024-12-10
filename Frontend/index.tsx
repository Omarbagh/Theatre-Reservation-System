import React from "react";
import ReactDOM from "react-dom";
import { LoginForm } from "./Login/login";

export const main = () => {
    let rootElement = document.querySelector('#root');

    ReactDOM.render(<LoginForm />, rootElement);
};

// Roep de main functie aan om de app te starten
//main();




// import React from 'react'
// import ReactDOM from 'react-dom'
// import { Home } from './Home/home'

// export const main = () => {
//   let rootElement = document.querySelector('#root')

//   ReactDOM.render(
//     <Home />,
//     rootElement
//     )
// }