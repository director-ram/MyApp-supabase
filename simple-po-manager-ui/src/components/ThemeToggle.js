import React from 'react';
import { useTheme } from '../context/ThemeContext';
import styles from './ThemeToggle.module.css';

const ThemeToggle = () => {
    const { isDarkMode, toggleTheme } = useTheme();

    return (
        <button
            className={`${styles.themeToggle} ${isDarkMode ? styles.dark : styles.light}`}
            onClick={toggleTheme}
            aria-label={`Switch to ${isDarkMode ? 'light' : 'dark'} mode`}
        >
            <div className={styles.toggleTrack}>
                <div className={styles.toggleThumb} />
                <span className={styles.sunIcon}>☀️</span>
                <span className={styles.moonIcon}>🌙</span>
            </div>
        </button>
    );
};

export default ThemeToggle; 