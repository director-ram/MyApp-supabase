import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { supabase } from '../supabaseClient';
import styles from '../styles/Login.module.css';

const LoginForm = ({ setIsAuthenticated }) => {
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const handleSubmit = async (event) => {
        event.preventDefault();
        setError('');
        setIsLoading(true);
        try {
            const { error } = await supabase.auth.signInWithPassword({
                email,
                password
            });
            if (error) {
                throw new Error(error.message || 'Login failed');
            }
            // Store the session in localStorage if needed
            setIsAuthenticated(true);
            navigate('/purchaseorders');
        } catch (error) {
            setError(error.message || 'An error occurred during login. Please check your connection and try again.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className={styles.formContainer}>
            <form onSubmit={handleSubmit} className={styles.loginForm}>
                {error && <div className={styles.error}>{error}</div>}
                <div className={styles.formGroup}>
                    <label htmlFor="email">Email:</label>
                    <input
                        type="email"
                        id="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                        className={styles.input}
                    />
                </div>
                <div className={styles.formGroup}>
                    <label htmlFor="password">Password:</label>
                    <input
                        type="password"
                        id="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        className={styles.input}
                    />
                </div>
                <button type="submit" disabled={isLoading} className={styles.button}>
                    {isLoading ? 'Logging in...' : 'Login'}
                </button>
                <div className={styles.registerLink}>
                    Don't have an account? <Link to="/register">Register here</Link>
                </div>
            </form>
        </div>
    );
};

export default LoginForm;