import React from 'react';
import ReactDOM from 'react-dom/client';
// 1. Remove Semantic UI CSS and add Mantine CSS
import '@mantine/core/styles.css';
import '@mantine/notifications/styles.css';
import '@mantine/dropzone/styles.css'; // Required for your Image Converter
import './index.css';

import reportWebVitals from './reportWebVitals';
import { router } from './Router/Routes';
import { RouterProvider } from 'react-router-dom';
import { AuthProvider } from './Components/User/AuthContext';

// 2. Import Mantine Provider and Notifications
import { MantineProvider } from '@mantine/core';
import { Notifications } from '@mantine/notifications';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    {/* 3. Wrap the app in MantineProvider */}
    <MantineProvider defaultColorScheme="light">
      <Notifications position="top-right" zIndex={2000} />
      <AuthProvider>
        <RouterProvider router={router} />
      </AuthProvider>
    </MantineProvider>
  </React.StrictMode>
);

reportWebVitals();