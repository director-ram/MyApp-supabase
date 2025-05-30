import React, { useState } from 'react';
import UserList from './components/UserList';
import UserForm from './components/UserForm';
import { supabase } from './supabaseClient';

const Users = () => {
    const [refresh, setRefresh] = useState(false);

    const handleUserSubmit = async (userData) => {
        try {
            // Insert or update user in Supabase
            if (userData.id) {
                // Update user
                const { error } = await supabase
                    .from('users')
                    .update({
                        username: userData.username,
                        password_hash: userData.password, // You should hash passwords in production!
                        first_name: userData.firstName,
                        last_name: userData.lastName,
                        email: userData.email
                    })
                    .eq('id', userData.id);
                if (error) throw error;
            } else {
                // Insert new user
                const { error } = await supabase
                    .from('users')
                    .insert([{
                        username: userData.username,
                        password_hash: userData.password, // You should hash passwords in production!
                        first_name: userData.firstName,
                        last_name: userData.lastName,
                        email: userData.email
                    }]);
                if (error) throw error;
            }
            setRefresh(!refresh);
        } catch (error) {
            console.error('Error saving user:', error);
            throw error;
        }
    };

    return (
        <div>
            <h1>Users</h1>
            <UserForm onSubmit={handleUserSubmit} refresh={refresh} setRefresh={setRefresh} />
            <UserList refresh={refresh} />
        </div>
    );
};

export default Users;
