// Default fetch options
export const defaultFetchOptions = {
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
    }
};

// Helper function to handle API errors
export const handleApiError = async (response) => {
    if (!response.ok) {
        const contentType = response.headers.get('content-type');
        let errorMessage;
        
        try {
            if (contentType && contentType.includes('application/json')) {
                const error = await response.json();
                errorMessage = error.message || `HTTP error! status: ${response.status}`;
            } else {
                const text = await response.text();
                errorMessage = text || `HTTP error! status: ${response.status}`;
            }
        } catch (e) {
            errorMessage = `HTTP error! status: ${response.status}`;
        }
        
        throw new Error(errorMessage);
    }
    return response;
};