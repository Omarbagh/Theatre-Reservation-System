import * as React from "react";
import { createRoot } from 'react-dom/client';
import { LoginForm } from './pages/Login'; // Update the path as needed
import { Login } from './pages/Home.state'; // Update the path as needed

// Define a function to handle the login submission.
const handleLogin = (login: Login) => {
    console.log('Login submitted:', login);
    // Add any additional logic for handling the login data here.
};

// Render the LoginForm component and pass the `insertLogin` prop.
createRoot(document.getElementById('root')!)
    .render(
        <React.StrictMode>
            <LoginForm insertLogin={handleLogin} />
        </React.StrictMode>
    );
