import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider } from './context/ThemeContext';
import ThemeToggle from './components/ThemeToggle';
import Navbar from './components/Navbar';
import LoginForm from './components/LoginForm';
import RegisterForm from './components/RegisterForm';
import Companies from './Companies';
import PurchaseOrders from './PurchaseOrders';
import Users from './Users';
import './styles/theme.module.css';

function App() {
    const [isAuthenticated, setIsAuthenticated] = React.useState(() => {
        return localStorage.getItem('authToken') !== null;
    });
    const [refresh, setRefresh] = React.useState(false);

    return (
        <ThemeProvider>
        <Router>
                <div className="app">
                    <ThemeToggle />
                    <Navbar isAuthenticated={isAuthenticated} setIsAuthenticated={setIsAuthenticated} />
            <Routes>
                        <Route 
                            path="/login" 
                            element={
                                !isAuthenticated ? (
                        <LoginForm setIsAuthenticated={setIsAuthenticated} />
                                ) : (
                                    <Navigate to="/purchaseorders" replace />
                                )
                            } 
                        />
                        <Route 
                            path="/register" 
                            element={
                                !isAuthenticated ? (
                        <RegisterForm />
                                ) : (
                                    <Navigate to="/purchaseorders" replace />
                                )
                            } 
                        />
                        <Route 
                            path="/companies" 
                            element={
                                isAuthenticated ? (
                                    <Companies refresh={refresh} setRefresh={setRefresh} />
                                ) : (
                                    <Navigate to="/login" replace />
                                )
                            } 
                        />
                <Route
                    path="/purchaseorders"
                            element={
                                isAuthenticated ? (
                        <PurchaseOrders refresh={refresh} setRefresh={setRefresh} />
                    ) : (
                                    <Navigate to="/login" replace />
                                )
                            } 
                        />
                        <Route 
                            path="/users" 
                            element={
                                isAuthenticated ? (
                                    <Users />
                                ) : (
                                    <Navigate to="/login" replace />
                                )
                            } 
                />
                        <Route 
                            path="/" 
                            element={<Navigate to="/purchaseorders" replace />} 
                        />
            </Routes>
            </div>
        </Router>
        </ThemeProvider>
    );
}

export default App;
