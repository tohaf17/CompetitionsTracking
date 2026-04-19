import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext';
import MainLayout from './components/Layout/MainLayout';
import Login from './pages/Auth/Login';
import Register from './pages/Auth/Register';
import CompetitionsList from './pages/Competitions/CompetitionsList';
import CompetitionDetails from './pages/Competitions/CompetitionDetails';
import TeamsList from './pages/Teams/TeamsList';
import PersonsList from './pages/Persons/PersonsList';
import JudgesList from './pages/Judges/JudgesList';
import AppealsList from './pages/Appeals/AppealsList';
import EntriesList from './pages/Entries/EntriesList';
import AdminCategories from './pages/Admin/Categories/AdminCategories';
import AdminDisciplines from './pages/Admin/Disciplines/AdminDisciplines';
import AdminApparatus from './pages/Admin/Apparatus/AdminApparatus';
import AdminUsersList from './pages/Admin/Users/AdminUsersList';
import AdminCompetitions from './pages/Admin/Competitions/AdminCompetitions';
import { Toaster } from 'react-hot-toast';

const ProtectedLayout = () => {
    const { user, loading } = useAuth();
    if (loading) return <div>Завантаження...</div>;
    if (!user) return <Navigate to="/login" />;
    return <MainLayout />;
};

const ProtectedRoute = ({ children, requireAdmin = false }) => {
    const { user, loading, isAdmin } = useAuth();

    if (loading) return <div>Завантаження...</div>;

    if (!user) {
        return <Navigate to="/login" />;
    }

    if (requireAdmin && !isAdmin) {
        return <Navigate to="/competitions" />;
    }

    return children;
};

const App = () => {
    return (
        <>
            <Toaster position="top-right" />
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/" element={<Navigate to="/competitions" />} />


                <Route element={<ProtectedLayout />}>
                    <Route path="/competitions" element={<CompetitionsList />} />
                    <Route path="/competitions/:id" element={<CompetitionDetails />} />
                    <Route path="/teams" element={<TeamsList />} />
                    <Route path="/persons" element={<PersonsList />} />
                    <Route path="/judges" element={<JudgesList />} />
                    <Route path="/appeals" element={<AppealsList />} />
                    <Route path="/entries" element={<EntriesList />} />

                    <Route
                        path="/admin/users"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminUsersList />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/admin/competitions"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminCompetitions />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/admin/categories"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminCategories />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/admin/disciplines"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminDisciplines />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/admin/apparatus"
                        element={
                            <ProtectedRoute requireAdmin={true}>
                                <AdminApparatus />
                            </ProtectedRoute>
                        }
                    />
                </Route>
            </Routes>
        </>
    );
};

export default App;
