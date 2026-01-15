import { createBrowserRouter, Navigate, RouteObject } from "react-router-dom";
import App from "../App";
import MainPage from "../Components/MainPage/MainPage"; // This will be your main Converter UI
import AppUserLogin from "../Components/User/AppUserLogin";
import AppUserRegistration from "../Components/User/AppUserRegistration";
import ProtectedRoute from "../Components/User/ProtectedRoute"; // Your existing file is fine
import NotFound from "../Components/NotFound/NotFound";

export const routes: RouteObject[] = [
    {
        path: "/",
        element: <App />,
        children: [
            // Public Landing or Redirect to Login? 
            // For now, just protect the main home page
            { 
                path: "", 
                element: <ProtectedRoute><MainPage /></ProtectedRoute> 
            },
            { path: "account/login", element: <AppUserLogin /> },
            { path: "account/register", element: <AppUserRegistration /> },
            { path: "not-found", element: <NotFound /> },
            { path: "*", element: <Navigate replace to="/not-found" /> }
        ]
    }
];

export const router = createBrowserRouter(routes);