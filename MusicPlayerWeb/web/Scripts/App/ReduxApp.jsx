import React from 'react';
import { render } from 'react-dom';
import { Provider } from 'react-redux';
import { store } from './DataStore/Store.jsx';
import App from './App.jsx';

/**
 * @class The main react app with the redux store.
 */
class ReduxApp extends React.Component{
    render() {
        return (
            <Provider store={store}>
                <App />
            </Provider>
        );
    }
}

render(<ReduxApp />, document.getElementById("react-root"));