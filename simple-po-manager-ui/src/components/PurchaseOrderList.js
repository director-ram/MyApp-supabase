import React, { useState, useEffect } from 'react';
import commonStyles from '../styles/common.module.css';
import animations from '../styles/animations.module.css';
import { supabase } from '../supabaseClient';

const PurchaseOrderList = ({ refresh, setRefresh }) => {
    const [purchaseOrders, setPurchaseOrders] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchPurchaseOrders = async () => {
            setIsLoading(true);
            setError(null);
            try {
                const { data: { user } } = await supabase.auth.getUser();
                if (!user) throw new Error('Not authenticated');
                const { data, error } = await supabase
                    .from('purchase_orders')
                    .select('*')
                    .eq('user_id', user.id);
                if (error) {
                    throw error;
                }
                setPurchaseOrders(data || []);
            } catch (error) {
                setError(error.message);
            } finally {
                setIsLoading(false);
            }
        };
        fetchPurchaseOrders();
    }, [refresh]);

    const handleDelete = async (id) => {
        try {
            const { data: { user } } = await supabase.auth.getUser();
            if (!user) throw new Error('Not authenticated');
            const { error } = await supabase
                .from('purchase_orders')
                .delete()
                .eq('id', id)
                .eq('user_id', user.id);
            if (error) {
                throw error;
            }
            setRefresh(prev => !prev);
        } catch (error) {
            setError(error.message);
        }
    };

    if (isLoading) {
        return (
            <div className={commonStyles.loadingContainer}>
                <div className={commonStyles.loadingSpinner}></div>
                <p>Loading purchase orders...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className={`${commonStyles.errorMessage} ${animations.fadeIn}`}>
                <p>{error}</p>
            </div>
        );
    }

    return (
        <div className={commonStyles.listContainer}>
            <h2>Purchase Orders</h2>
            {purchaseOrders.length === 0 ? (
                <p className={commonStyles.noData}>No purchase orders found</p>
            ) : (
                <div className={commonStyles.tableContainer}>
                    <table>
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Company ID</th>
                                <th>Order Date</th>
                                <th>Total Amount</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {purchaseOrders.map(po => (
                                <tr key={po.id}>
                                    <td>{po.id}</td>
                                    <td>{po.company_id}</td>
                                    <td>{po.order_date}</td>
                                    <td>{po.total_amount}</td>
                                    <td>{po.status}</td>
                                    <td>
                                        <button onClick={() => handleDelete(po.id)}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default PurchaseOrderList;
