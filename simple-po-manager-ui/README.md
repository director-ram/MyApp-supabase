# Simple PO Manager UI

A React-based frontend for the Company Management System.

## Development

1. Install dependencies:
```bash
npm install
```

2. Start development server:
```bash
npm start
```

The development server will run on http://localhost:5013 and connect to the backend at http://localhost:5014.

## Production Deployment

### Prerequisites
- Node.js 14 or higher
- npm 6 or higher
- Vercel CLI (for deployment)

### Deployment Steps

1. Build the application:
```bash
npm run build
```

2. Test the production build locally:
```bash
npm run serve:prod
```

3. Deploy to Vercel:
```bash
npm run deploy
```

### Environment Variables

The following environment variables are used:

- `REACT_APP_API_BASE_URL`: The base URL of the backend API
  - Development: http://localhost:5014
  - Production: https://company-management-api.onrender.com

### Deployment Configuration

The application uses the following deployment configurations:

- `vercel.json`: Configures routing and build settings for Vercel deployment
- `.env.production`: Production environment variables
- `package.json`: Build and deployment scripts

### Troubleshooting

If you encounter issues with the production build:

1. Clear the build directory:
```bash
rm -rf build
```

2. Clear npm cache:
```bash
npm cache clean --force
```

3. Reinstall dependencies:
```bash
rm -rf node_modules
npm install
```

4. Rebuild:
```bash
npm run build
```

### CORS Configuration

The backend is configured to accept requests from:
- http://localhost:5013 (Development)
- http://localhost:3000 (Alternative development port)
- https://your-production-domain.com (Production)

Make sure to update the CORS configuration in the backend if you change these URLs.

# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in your browser.

The page will reload when you make changes.\
You may also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can't go back!**

If you aren't satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you're on your own.

You don't have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn't feel obligated to use this feature. However we understand that this tool wouldn't be useful if you couldn't customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

### Code Splitting

This section has moved here: [https://facebook.github.io/create-react-app/docs/code-splitting](https://facebook.github.io/create-react-app/docs/code-splitting)

### Analyzing the Bundle Size

This section has moved here: [https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size](https://facebook.github.io/create-react-app/docs/analyzing-the-bundle-size)

### Making a Progressive Web App

This section has moved here: [https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app](https://facebook.github.io/create-react-app/docs/making-a-progressive-web-app)

### Advanced Configuration

This section has moved here: [https://facebook.github.io/create-react-app/docs/advanced-configuration](https://facebook.github.io/create-react-app/docs/advanced-configuration)

### Deployment

This section has moved here: [https://facebook.github.io/create-react-app/docs/deployment](https://facebook.github.io/create-react-app/docs/deployment)

### `npm run build` fails to minify

This section has moved here: [https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify](https://facebook.github.io/create-react-app/docs/troubleshooting#npm-run-build-fails-to-minify)
