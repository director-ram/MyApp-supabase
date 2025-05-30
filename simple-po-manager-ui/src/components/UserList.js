import React, { useEffect, useState } from 'react';
import { supabase } from '../supabaseClient';
import styles from '../styles/common.module.css';

function UserList({ refresh }) {
    const [users, setUsers] = useState([]);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                setIsLoading(true);
                setError(null);
                const { data, error } = await supabase
                    .from('users')
                    .select('*');
                if (error) throw error;
                setUsers(data || []);
            } catch (error) {
                setError(error.message);
            } finally {
                setIsLoading(false);
            }
        };
        fetchUsers();
    }, [refresh]);

    if (isLoading) {
        return (
            <div className={styles.loadingContainer}>
                <div className={styles.loadingSpinner}></div>
                <p>Loading users...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className={styles.errorMessage}>
                <p>{error}</p>
            </div>
        );
    }

    return (
        <div className={styles.listContainer}>
            <h2>Users</h2>
            <ul>
                {users.map(user => (
                    <li key={user.id}>{user.username} ({user.email})</li>
                ))}
            </ul>
        </div>
    );
}

export default UserList;
