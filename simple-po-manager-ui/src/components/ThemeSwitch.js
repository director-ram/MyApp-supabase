import React from 'react';
import { useTheme } from '../contexts/ThemeContext';
import styles from '../styles/theme.module.css';

const ThemeSwitch = () => {
    const { theme, toggleTheme } = useTheme();

    return (
        <button 
            className={styles.themeSwitch}
            onClick={toggleTheme}
            aria-label={`Switch to ${theme === 'light' ? 'dark' : 'light'} theme`}
        >
            {theme === 'light' ? '🌙' : '☀️'}
        </button>
    );
};

export default ThemeSwitch; 