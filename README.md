# Simple Purchase Order Manager

## Setup Instructions

1.  **Clone the repository:**
    ```bash
    git clone <repository_url>
    ```

2.  **Navigate to the `CompanyManagementSystem` directory:**
    ```bash
    cd CompanyManagementSystem
    ```

3.  **Update the database:**
    ```bash
    dotnet ef database update
    ```

4.  **Run the API:**
    ```bash
    dotnet run
    ```

    The API will be running on `http://localhost:5000`.

5.  **Navigate to the `simple-po-manager-ui` directory:**
    ```bash
    cd ../simple-po-manager-ui
    ```

6.  **Install dependencies:**
    ```bash
    npm install
    ```

7.  **Run the frontend:**
    ```bash
    npm start
    ```

    The frontend will be running on `http://localhost:3000`.

## API Endpoints

The API provides the following endpoints:

*   `/api/companies`:  Manage companies.
*   `/api/users`: Manage users.
*   `/api/purchaseorder`: Manage purchase orders.

## UI Features

The UI provides the following features:

*   Manage companies.
*   Manage users.
*   Manage purchase orders.
*   Login form.
*   Register form.