import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import styles from './Navbar.module.css';

const Navbar = ({ isAuthenticated, setIsAuthenticated }) => {
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('user');
        setIsAuthenticated(false);
        navigate('/login');
    };

    if (!isAuthenticated) {
        return null;
    }

    return (
        <nav className={styles.navbar}>
            <div className={styles.navLinks}>
                <Link to="/purchaseorders" className={styles.navLink}>Purchase Orders</Link>
                <Link to="/companies" className={styles.navLink}>Companies</Link>
                <Link to="/users" className={styles.navLink}>Users</Link>
            </div>
            <button onClick={handleLogout} className={styles.logoutButton}>
                Logout
            </button>
        </nav>
    );
};

export default Navbar; 