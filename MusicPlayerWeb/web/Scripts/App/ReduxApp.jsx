import React from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import { store } from './DataStore/Store.jsx';
import App from './App.jsx';

const root = createRoot(document.getElementById("react-root"));
root.render(<Provider store={store}><App /></Provider>);