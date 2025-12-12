import { createBrowserRouter, RouterProvider } from "react-router-dom";
import MainLayout from "../layout/MainLayout";
import { Role } from "../models/User";
import Dashboard from "../pages/Dashboard";
import Home from "../pages/Home";
import Login from "../pages/Login";
import Profile from "../pages/Profile";
import Register from "../pages/Register";
import TeacherDetails from "../pages/TeacherDetails";
import TeacherSearch from "../pages/TeacherSearch";
import MyBookings from "../pages/MyBookings";
import NotFound from "../pages/NotFound";
import ServerError from "../pages/ServerError";
import ProtectedRoute from "./ProtectedRoute";
import { UnauthenticatedRoute } from "./UnauthenticatedRoute";
import MySlots from "../pages/MySlots";
import MySlotDetails from "../pages/MySlotDetails";

export default function Router() {
    const router = createBrowserRouter([
        {
            element: <MainLayout />,
            children: [
                {
                    element: <UnauthenticatedRoute />,
                    children: [
                        {
                            path: '/login',
                            element: <Login />
                        },
                        {
                            path: '/register',
                            element: <Register />
                        }
                    ]
                },
                {
                    children: [
                        {
                            path: '/',
                            element: <Home />
                        },
                        {
                            path: '/teachers',
                            element: <TeacherSearch />
                        },
                        {
                            path: '/teachers/:userId',
                            element: <TeacherDetails />
                        }
                    ]
                },
                {
                    path: '/dashboard',
                    element: <ProtectedRoute allowedRoles={[Role.Student, Role.Teacher]}>
                        <Dashboard />
                    </ProtectedRoute>
                },
                {
                    path: '/profile',
                    element: <ProtectedRoute allowedRoles={[Role.Student, Role.Teacher]}>
                        <Profile />
                    </ProtectedRoute>
                },
                {
                    path: '/my-slots',
                    element: <ProtectedRoute allowedRoles={[Role.Teacher]}>
                        <MySlots />
                    </ProtectedRoute>
                },
                {
                    path: '/teacher/slots/:slotId/details',
                    element: <ProtectedRoute allowedRoles={[Role.Teacher]}>
                        <MySlotDetails />
                    </ProtectedRoute>
                },
                {
                    path: '/my-bookings',
                    element: <ProtectedRoute allowedRoles={[Role.Student]}>
                        <MyBookings />
                    </ProtectedRoute>
                },
                {
                    path: '/server-error',
                    element: <ServerError />
                },
                {
                    path: '*',
                    element: <NotFound />
                }
            ]
        }
    ])

    return (
        <RouterProvider router={router} />
    )
}