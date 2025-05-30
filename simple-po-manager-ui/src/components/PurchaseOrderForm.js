import React, { useState, useEffect } from 'react';
import styles from './PurchaseOrderForm.module.css';
import animations from '../styles/animations.module.css';
import focusStyles from '../styles/focus.module.css';
import { supabase } from '../supabaseClient';

const PurchaseOrderForm = ({ refresh, setRefresh }) => {
    const [formData, setFormData] = useState({
        companyId: '',
        orderDate: '',
        notificationEmail: '',
        notificationPhone: '',
        countryCode: '+91',
        lineItems: []
    });
    const [companies, setCompanies] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [errors, setErrors] = useState({});

    const countryCodes = [
        { code: '+91', country: 'India' },
        { code: '+1', country: 'United States' },
        { code: '+44', country: 'United Kingdom' },
        { code: '+61', country: 'Australia' },
        { code: '+86', country: 'China' },
        { code: '+81', country: 'Japan' },
        { code: '+49', country: 'Germany' },
        { code: '+33', country: 'France' },
        { code: '+39', country: 'Italy' },
        { code: '+34', country: 'Spain' }
    ];

    useEffect(() => {
        const fetchCompanies = async () => {
            try {
                const { data, error } = await supabase
                    .from('companies')
                    .select('*');
                if (error) throw error;
                setCompanies(data || []);
            } catch (error) {
                setCompanies([]);
            }
        };
        fetchCompanies();
    }, [refresh]);

    const handleLineItemChange = (index, event) => {
        const { name, value } = event.target;
        const updatedLineItems = [...formData.lineItems];
        updatedLineItems[index][name] = value;
        setFormData({ ...formData, lineItems: updatedLineItems });
    };

    const addLineItem = () => {
        setFormData({ ...formData, lineItems: [...formData.lineItems, { productId: '', quantity: '', unitPrice: '' }] });
    };

    const removeLineItem = (index) => {
        const updatedLineItems = [...formData.lineItems];
        updatedLineItems.splice(index, 1);
        setFormData({ ...formData, lineItems: updatedLineItems });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!validateForm()) return;
        try {
            setIsLoading(true);
            setErrors({});
            const validLineItems = formData.lineItems.map(item => ({
                product_id: parseInt(item.productId),
                quantity: parseInt(item.quantity),
                unit_price: parseFloat(item.unitPrice)
            })).filter(item =>
                !isNaN(item.product_id) &&
                !isNaN(item.quantity) &&
                !isNaN(item.unit_price) &&
                item.product_id > 0 &&
                item.quantity > 0 &&
                item.unit_price > 0
            );
            if (validLineItems.length === 0) {
                throw new Error('Please add at least one valid line item');
            }
            const totalAmount = validLineItems.reduce((total, item) => {
                return total + (item.quantity * item.unit_price);
            }, 0);
            const { data: { user } } = await supabase.auth.getUser();
            if (!user) throw new Error('Not authenticated');
            const purchaseOrderData = {
                company_id: parseInt(formData.companyId),
                order_date: new Date(formData.orderDate).toISOString(),
                notification_email: formData.notificationEmail || null,
                notification_phone: formData.notificationPhone ? `${formData.countryCode}${formData.notificationPhone}` : null,
                total_amount: totalAmount,
                status: 'Pending',
                user_id: user.id
            };
            const { error } = await supabase
                .from('purchase_orders')
                .insert([{ ...purchaseOrderData }]);
            if (error) throw error;
            // Optionally insert line items if you have a separate table
            setFormData({
                companyId: '',
                orderDate: '',
                notificationEmail: '',
                notificationPhone: '',
                countryCode: '+91',
                lineItems: []
            });
            setErrors({});
            setRefresh(prev => !prev);
        } catch (error) {
            setErrors({ submit: error.message });
        } finally {
            setIsLoading(false);
        }
    };

    function validateForm() {
        const errs = {};
        if (!formData.companyId) errs.companyId = 'Company is required';
        if (!formData.orderDate) errs.orderDate = 'Order date is required';
        setErrors(errs);
        return Object.keys(errs).length === 0;
    }

    return (
        <div className={styles.formContainer}>
            <h2 className={styles.sectionTitle}>Create Purchase Order</h2>
            <form onSubmit={handleSubmit}>
                <div className={styles.formGroup}>
                    <label htmlFor="companyId" className={styles.required}>Company:</label>
                    <select 
                        id="companyId"
                        name="companyId" 
                        value={formData.companyId} 
                        onChange={(e) => setFormData({ ...formData, companyId: e.target.value })}
                        className={`${styles.formControl} ${focusStyles.inputFocus} ${errors.companyId ? styles.isInvalid : ''}`}
                        disabled={isLoading}
                    >
                        <option value="">Select a company</option>
                        {companies.map(company => (
                            <option key={company.id || company.Id} value={company.id || company.Id}>
                                {company.name || company.Name}
                            </option>
                        ))}
                    </select>
                    {errors.companyId && <div className={styles.errorMessage}>{errors.companyId}</div>}
                </div>

                <div className={styles.formGroup}>
                    <label htmlFor="orderDate" className={styles.required}>Order Date:</label>
                    <input 
                        type="date" 
                        id="orderDate"
                        name="orderDate" 
                        value={formData.orderDate} 
                        onChange={(e) => setFormData({ ...formData, orderDate: e.target.value })}
                        className={`${styles.formControl} ${focusStyles.inputFocus} ${errors.orderDate ? styles.isInvalid : ''}`}
                        disabled={isLoading}
                    />
                    {errors.orderDate && <div className={styles.errorMessage}>{errors.orderDate}</div>}
                </div>

                <div className={styles.formGroup}>
                    <label htmlFor="notificationEmail">Email Notification:</label>
                    <input 
                        type="email" 
                        id="notificationEmail"
                        name="notificationEmail" 
                        value={formData.notificationEmail} 
                        onChange={(e) => setFormData({ ...formData, notificationEmail: e.target.value })}
                        placeholder="Enter email for order notifications"
                        className={`${styles.formControl} ${focusStyles.inputFocus}`}
                        disabled={isLoading}
                    />
                    <small className={styles.helpText}>Optional: Receive email notifications about this purchase order</small>
                </div>

                <div className={styles.formGroup}>
                    <label htmlFor="notificationPhone">Mobile Number:</label>
                    <div className={styles.phoneInputGroup}>
                        <select
                            id="countryCode"
                            name="countryCode"
                            value={formData.countryCode}
                            onChange={(e) => setFormData({ ...formData, countryCode: e.target.value })}
                            className={`${styles.formControl} ${styles.countryCode} ${focusStyles.inputFocus}`}
                            disabled={isLoading}
                        >
                            {countryCodes.map(({ code, country }) => (
                                <option key={code} value={code}>
                                    {code} {country.length > 10 ? country.substring(0, 10) + '...' : country}
                                </option>
                            ))}
                        </select>
                        <input
                            type="tel"
                            id="notificationPhone"
                            name="notificationPhone"
                            value={formData.notificationPhone || ''}
                            onChange={(e) => setFormData({ ...formData, notificationPhone: e.target.value })}
                            placeholder="Enter mobile number"
                            pattern="[0-9]{10}"
                            title="Please enter a valid 10-digit mobile number"
                            className={`${styles.formControl} ${styles.phoneNumber} ${focusStyles.inputFocus} ${errors.notificationPhone ? styles.isInvalid : ''}`}
                            disabled={isLoading}
                        />
                    </div>
                    {errors.notificationPhone && (
                        <div className={styles.errorMessage}>{errors.notificationPhone}</div>
                    )}
                    <small className={styles.helpText}>Optional: Receive SMS notifications about this purchase order</small>
                </div>

                {formData.lineItems.map((item, index) => (
                    <div key={index} className={`${styles.lineItem} ${animations.scaleIn}`}>
                        <div className={styles.formGroup}>
                            <label htmlFor={`productId-${index}`} className={styles.required}>Product ID:</label>
                            <input 
                                type="number" 
                                id={`productId-${index}`}
                                name="productId" 
                                value={item.productId} 
                                onChange={(e) => handleLineItemChange(index, e)}
                                className={`${styles.formControl} ${focusStyles.inputFocus}`}
                                disabled={isLoading}
                            />
                        </div>
                        <div className={styles.formGroup}>
                            <label htmlFor={`quantity-${index}`} className={styles.required}>Quantity:</label>
                            <input 
                                type="number" 
                                id={`quantity-${index}`}
                                name="quantity" 
                                value={item.quantity} 
                                onChange={(e) => handleLineItemChange(index, e)}
                                className={`${styles.formControl} ${focusStyles.inputFocus}`}
                                disabled={isLoading}
                                min="1"
                            />
                        </div>
                        <div className={styles.formGroup}>
                            <label htmlFor={`unitPrice-${index}`} className={styles.required}>Unit Price:</label>
                            <input 
                                type="number" 
                                id={`unitPrice-${index}`}
                                name="unitPrice" 
                                value={item.unitPrice} 
                                onChange={(e) => handleLineItemChange(index, e)}
                                className={`${styles.formControl} ${focusStyles.inputFocus}`}
                                disabled={isLoading}
                                step="0.01"
                                min="0"
                            />
                        </div>
                        <button 
                            type="button" 
                            className={`${styles.dangerButton} ${focusStyles.buttonFocus}`}
                            onClick={() => removeLineItem(index)}
                            disabled={isLoading}
                        >
                            Remove
                        </button>
                        {errors.lineItems?.[index] && (
                            <div className={styles.errorMessage}>{errors.lineItems[index]}</div>
                        )}
                    </div>
                ))}

                <div className={styles.buttons}>
                    <button 
                        type="button" 
                        className={`${styles.secondaryButton} ${focusStyles.buttonFocus}`}
                        onClick={addLineItem}
                        disabled={isLoading}
                    >
                        Add Line Item
                    </button>
                    <button 
                        type="submit" 
                        className={`${styles.primaryButton} ${focusStyles.buttonFocus}`}
                        disabled={isLoading}
                    >
                        {isLoading ? (
                            <>
                                Creating...
                                <span className={styles.loadingSpinner} />
                            </>
                        ) : (
                            'Create Purchase Order'
                        )}
                    </button>
                </div>
                {errors.submit && (
                    <div className={`${styles.errorMessage} ${styles.submitError}`}>
                        {errors.submit}
                    </div>
                )}
            </form>
        </div>
    );
};

export default PurchaseOrderForm;
