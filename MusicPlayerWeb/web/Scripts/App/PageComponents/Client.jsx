import React, { useState, useEffect } from 'react';
import { useSelector } from 'react-redux';

const Client = () => {
    const serverInfo = useSelector(state => state.serverInfo);
    const [port, setPort] = useState(8963);
    const [ip, setIp] = useState("127.0.0.1");

    useEffect(() => {
        MusicPlayer.getDefaultIP().then((defaultIp) => {
            setIp(defaultIp);
        });
    }, []);

    const connectToServer = () => {
        MusicPlayer.connectToServer(ip, port);
    };

    const disconnectServer = () => {
        MusicPlayer.disconnectServer();
    };

    const renderConnect = () => {
        return (
            <div className="server-connect col h-100">
                <h2>Connect to a stream</h2>
                <p>By connecting to a stream you can listen to another user's music.</p>
                <input type="text" value={ip} onChange={(e) => setIp(e.target.value)} />
                <input type="number" value={port} onChange={(e) => setPort(e.target.value)} />
                <button className="primary-button" onClick={connectToServer}>Connect</button>
            </div>
        );
    };

    const renderServerInfo = () => {
        return (
            <div className="server-clients col h-100">
                <div>
                    <div>
                        <p>Connected to {serverInfo.Host}:{serverInfo.Port}</p>
                        <button onClick={disconnectServer} className="primary-button">Disconnect</button>
                    </div>
                </div>
            </div>
        );
    };

    return (
        <div className="server row h-100">
            {!serverInfo && renderConnect()}
            {serverInfo && renderServerInfo()}
        </div>
    );
};

export default Client;