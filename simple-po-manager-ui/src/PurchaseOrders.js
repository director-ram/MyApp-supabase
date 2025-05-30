import React, { useState, useEffect } from 'react';
import PurchaseOrderList from './components/PurchaseOrderList';
import PurchaseOrderForm from './components/PurchaseOrderForm';
import commonStyles from './styles/common.module.css';
import animations from './styles/animations.module.css';
import { useNavigate } from 'react-router-dom';
import { supabase } from './supabaseClient';

const PurchaseOrders = ({ refresh, setRefresh }) => {
    const navigate = useNavigate();
    const [summary, setSummary] = useState({
        total: 0,
        count: 0,
        avgAmount: 0
    });
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [orders, setOrders] = useState([]);

    useEffect(() => {
        const fetchSummary = async () => {
            setIsLoading(true);
            setError(null);
            try {
                const { data: { user } } = await supabase.auth.getUser();
                if (!user) throw new Error('Not authenticated');
                const { data, error } = await supabase
                    .from('purchase_orders')
                    .select('total_amount')
                    .eq('user_id', user.id);
                if (error) throw error;
                const orders = data || [];
                const total = orders.reduce((sum, po) => sum + (po.total_amount || 0), 0);
                setSummary({
                    total: total.toFixed(2),
                    count: orders.length,
                    avgAmount: orders.length ? (total / orders.length).toFixed(2) : '0.00'
                });
            } catch (error) {
                setError(error.message);
            } finally {
                setIsLoading(false);
            }
        };
        fetchSummary();
    }, [refresh, navigate]);

    useEffect(() => {
        async function fetchOrders() {
            const { data, error } = await supabase
                .from('purchase_orders')
                .select('*');
            if (error) {
                alert('Error fetching orders: ' + error.message);
            } else {
                setOrders(data);
            }
        }
        fetchOrders();
    }, []);

    const renderSummaryCard = (title, value, index) => (
        <div className={`${commonStyles.summaryCard} ${animations.fadeIn}`}
             style={{ animationDelay: `${index * 0.1}s` }}>
            <h3>{title}</h3>
            {isLoading ? (
                <div className={commonStyles.skeletonLoader}></div>
            ) : (
                <p className={commonStyles.summaryValue}>{value}</p>
            )}
        </div>
    );

    return (
        <div className={commonStyles.pageContainer}>
            <h1 className={commonStyles.pageTitle}>Purchase Orders</h1>
            
            {error ? (
                <div className={`${commonStyles.errorMessage} ${animations.fadeIn}`}>
                    <p>{error}</p>
                    {error.includes('session has expired') && (
                        <button 
                            onClick={() => navigate('/login')}
                            className={commonStyles.retryButton}
                        >
                            Go to Login
                        </button>
                    )}
                </div>
            ) : (
                <div className={`${commonStyles.summarySection} ${animations.fadeIn}`}>
                    {renderSummaryCard('Total Orders', summary.count, 0)}
                    {renderSummaryCard('Total Amount', `$${summary.total}`, 1)}
                    {renderSummaryCard('Average Amount', `$${summary.avgAmount}`, 2)}
                </div>
            )}

            <div className={`${commonStyles.card} ${animations.slideIn}`}>
                <PurchaseOrderForm refresh={refresh} setRefresh={setRefresh} />
            </div>
            
            <div className={`${commonStyles.card} ${animations.slideIn}`}>
                <PurchaseOrderList refresh={refresh} setRefresh={setRefresh} />
            </div>

            <div className={`${commonStyles.card} ${animations.slideIn}`}>
                <h2>Purchase Orders (from Supabase)</h2>
                <ul>
                    {orders.map(order => (
                        <li key={order.id}>
                            Order #{order.id} - Amount: {order.total_amount}
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default PurchaseOrders;