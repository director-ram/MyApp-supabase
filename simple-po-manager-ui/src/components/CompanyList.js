import React, { useState, useEffect } from 'react';
import styles from './CompanyList.module.css';
import { supabase } from '../supabaseClient';

function CompanyList({ refresh, setRefresh }) {
    const [companies, setCompanies] = useState([]);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    useEffect(() => {
        const fetchCompanies = async () => {
            try {
                setIsLoading(true);
                setError(null);
                const { data: { user } } = await supabase.auth.getUser();
                if (!user) throw new Error('Not authenticated');
                const { data, error } = await supabase
                    .from('companies')
                    .select('*')
                    .eq('user_id', user.id);
                if (error) throw error;
                setCompanies(data || []);
            } catch (error) {
                setError(error.message);
            } finally {
                setIsLoading(false);
            }
        };
        fetchCompanies();
    }, [refresh]);

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this company?')) {
            return;
        }
        try {
            const { data: { user } } = await supabase.auth.getUser();
            if (!user) throw new Error('Not authenticated');
            const { error } = await supabase
                .from('companies')
                .delete()
                .eq('id', id)
                .eq('user_id', user.id);
            if (error) throw error;
            setRefresh(prev => !prev);
        } catch (error) {
            setError(error.message);
        }
    };

    if (isLoading) {
        return <div className={styles.loading}>Loading companies...</div>;
    }

    if (error) {
        return <div className={styles.error}>{error}</div>;
    }

    return (
        <div className={styles.listContainer}>
            <h2>Companies</h2>
            <ul className={styles.list}>
                {companies.map(company => (
                    <li key={company.id} className={styles.listItem}>
                        <div className={styles.itemInfo}>
                            <div className={styles.itemName}>{company.name}</div>
                            <div className={styles.itemDetails}>{company.address}</div>
                        </div>
                        <button onClick={() => handleDelete(company.id)} className={styles.deleteButton}>
                            Delete
                        </button>
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default CompanyList;
