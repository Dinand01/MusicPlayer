import React from 'react';
import { useSelector } from 'react-redux';
import { HashRouter, Route, Link } from 'react-router-dom';
import '../../Style/App.scss';
import { csharpDispatcher } from './CSharpDispatcher.jsx';

import Home from './PageComponents/Home.jsx';
import PlayList from './PageComponents/PlayList.jsx';
import Server from './PageComponents/Server.jsx';
import Client from './PageComponents/Client.jsx';
import Copy from './PageComponents/Copy.jsx';
import Video from './PageComponents/Video.jsx';
import Radio from './PageComponents/Radio.jsx';
import EditRadio from './PageComponents/EditRadio.jsx';

window.CSSharpDispatcher = csharpDispatcher;

const App = () => {
    const currentSong = useSelector(state => state.currentSong);
    const serverInfo = useSelector(state => state.serverInfo);
    const copyProgress = useSelector(state => state.copyProgress);

    return (
        <HashRouter>
            <div className="h-100">
                <div className="navbar p-0">
                    <ul className="navigation">
                        <li><Link to="/"><i className="fa fa-home" /></Link></li>
                        {currentSong && <li><Link to="/playlist"><i className="fa fa-music" /></Link></li>}
                        {(currentSong && currentSong.IsInternetRadio) && <li><Link to="/radio"><i className="fab fa-soundcloud" /></Link></li>}
                        {serverInfo && serverInfo.IsHost && <li title={"Currently connected clients: " + serverInfo.Count}>
                            <Link to="/server"><i className="fas fa-broadcast-tower" /></Link>
                        </li>}
                        {serverInfo && !serverInfo.IsHost && <li title={"Currently connected to: " + serverInfo.Host}>
                            <Link to="/client"><i className="fas fa-signal" /></Link>
                        </li>}
                        {serverInfo && serverInfo.VideoUrl && <li title={"Currently playing: " + serverInfo.VideoUrl}>
                            <Link to="/video"><i className="fab fa-youtube" /></Link>
                        </li>}
                        {copyProgress != null && copyProgress != undefined && <li title={"Currently copying files: " + parseInt(copyProgress) + "%"}>
                            <Link to="/copy"><i className="far fa-copy" /></Link>
                        </li>}
                    </ul>
                </div>
                <div className="container-fluid mainContainer">
                    <Route exact path="/" component={Home}></Route>
                    <Route path="/playlist" component={PlayList}></Route>
                    <Route path="/server" component={Server}></Route>
                    <Route path="/client" component={Client}></Route>
                    <Route path="/copy" component={Copy}></Route>
                    <Route path="/video" component={Video}></Route>
                    <Route path="/radio" component={Radio}></Route>
                    <Route path="/radio/:id" component={EditRadio}></Route>
                </div>
            </div>
        </HashRouter>
    );
};

export default App;