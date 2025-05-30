import React, { useState } from 'react';
import styles from './CompanyForm.module.css';

const CompanyForm = ({ company, onSubmit, refresh, setRefresh }) => {
    const [name, setName] = useState(company ? company.name : '');
    const [address, setAddress] = useState(company ? company.address : '');
    const [errors, setErrors] = useState({});

    const validateForm = () => {
        const newErrors = {};
        if (!name.trim()) {
            newErrors.name = 'Company name is required';
        }
        if (!address.trim()) {
            newErrors.address = 'Company address is required';
        }
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (!validateForm()) {
            return;
        }

        const companyData = {
            name,
            address
        };

        onSubmit(companyData);
        setName('');
        setAddress('');
        if (setRefresh) {
            setRefresh(refresh => !refresh);
        }
    };

    return (
        <div className={styles.formContainer}>
            <form onSubmit={handleSubmit} className={styles.form}>
                <div className={styles.formGroup}>
                    <label htmlFor="name" className={styles.label}>Company Name</label>
                    <input
                        id="name"
                        type="text"
                        className={styles.input}
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        placeholder="Enter company name"
                    />
                    {errors.name && <div className={styles.errorMessage}>{errors.name}</div>}
                </div>

                <div className={styles.formGroup}>
                    <label htmlFor="address" className={styles.label}>Company Address</label>
                    <input
                        id="address"
                        type="text"
                        className={styles.input}
                        value={address}
                        onChange={(e) => setAddress(e.target.value)}
                        placeholder="Enter company address"
                    />
                    {errors.address && <div className={styles.errorMessage}>{errors.address}</div>}
                </div>

                <button type="submit" className={styles.submitButton}>
                    {company ? 'Update Company' : 'Add Company'}
                </button>
            </form>
        </div>
    );
};

export default CompanyForm;
