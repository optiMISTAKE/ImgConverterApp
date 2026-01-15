import React from 'react';
import './App.css';
import { Outlet } from 'react-router-dom';
import { Box } from '@mantine/core';

function App() {
  return (
    <>
      {/* Wrapping the Outlet in a Box with a top margin 
         ensures content doesn't get hidden behind a fixed NavBar 
      */}
      <Box component="main" py="md">
        <Outlet />
      </Box>
    </>
  );
}

export default App;