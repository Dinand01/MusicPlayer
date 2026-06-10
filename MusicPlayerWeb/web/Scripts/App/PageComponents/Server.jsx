import React, { useState } from 'react';
import { useSelector } from 'react-redux';

const Server = () => {
    const serverInfo = useSelector(state => state.serverInfo);
    const [port, setPort] = useState(8963);

    const hostServer = () => {
        MusicPlayer.hostServer(port);
    };

    const disconnectServer = () => {
        MusicPlayer.disconnectServer();
    };

    const renderConnect = () => {
        return (
            <div className="server-connect">
                <h2>Host music</h2>
                <p>By port forwarding the following port to your pc people can listen in on your music.</p>
                <input type="number" value={port} onChange={(e) => setPort(e.target.value)} />
                <button className="primary-button" onClick={hostServer}>Host</button>
            </div>
        );
    };

    const renderServerInfo = () => {
        return (
            <div className="server-clients col">
                <div>
                    <div>
                        <p>Connected Clients:</p>
                        {(() => {
                            let res = [];
                            for (var key in serverInfo.Clients) {
                                res.push(<p key={key}>{key}: {serverInfo.Clients[key]}</p>);
                            }
                            return res;
                        })()}
                        <button onClick={disconnectServer} className="primary-button">Disconnect</button>
                    </div>
                </div>
            </div>
        );
    };

    return (
        <div className="row h-100">
            {!serverInfo && renderConnect()}
            {serverInfo && renderServerInfo()}
        </div>
    );
};

export default Server;