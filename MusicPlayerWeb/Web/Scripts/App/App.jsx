import React from 'react';
import { connect } from 'react-redux';
import Router from 'react-router';
import { HashRouter, Route, Link } from 'react-router-dom';
import '../../Style/App.scss';
import { csharpDispatcher } from './CSharpDispatcher.jsx';

import Home from './PageComponents/Home.jsx';
import PlayList from './PageComponents/PlayList.jsx';
import Server from './PageComponents/Server.jsx';
import Client from './PageComponents/Client.jsx';
import Copy from './PageComponents/Copy.jsx';


/**
 * @class The React app.
 */
class App extends React.Component {
    constructor(props) {
        super(props);
        window.CSSharpDispatcher = csharpDispatcher;
    }

    /**
     * @description Prevent unescesary render cycles.
     * @param {object} nextprops 
     * @param {object} nextstate 
     */
    shouldComponentUpdate(nextprops, nextstate) {
        if (nextprops.currentSong != this.props.currentSong 
            || JSON.stringify(nextprops.serverInfo) != JSON.stringify(this.props.serverInfo) 
            || this.props.copyProgress != nextprops.copyProgress) {
            return true;
        }

        return false;
    }

    /**
     * @description Render navigation and router.
     */
    render() {
        return (
            <HashRouter>
                <div className="appRoot">
                    <ul className="navigation">
                        <li><Link to="/"><i className="fa fa-home" /></Link></li>
                        {this.props.currentSong && <li><Link to="/playlist"><i className="fa fa-music" /></Link></li>}
                        {this.props.serverInfo && this.props.serverInfo.IsHost && <li title={"Currently connected clients: " + this.props.serverInfo.Count}>
                            <Link to="/server"><i className="fa fa-rss" /></Link>
                        </li>}
                        {this.props.serverInfo && !this.props.serverInfo.IsHost && <li title={"Currently connected to: " + this.props.serverInfo.Host}>
                            <Link to="/client"><i className="fa fa-podcast" /></Link>
                        </li>}
                        {this.props.copyProgress != null && this.props.copyProgress != undefined && <li title={"Currently copying files: " + parseInt(this.props.copyProgress) + "%"}>
                            <Link to="/copy"><i className="fa fa-files-o" /></Link>
                        </li>}
                    </ul>
                    
                    <div className="appContent">
                        <Route path="/" component={Home}></Route>
                        <Route path="/playlist" component={PlayList}></Route>
                        <Route path="/server" component={Server}></Route>
                        <Route path="/client" component={Client}></Route>
                        <Route path="/copy" component={Copy}></Route>
                    </div>
                </div>
            </HashRouter>
        );
    }
}

function mapStateToProps(state) {
  return { 
      currentSong: state.currentSong,
      serverInfo: state.serverInfo,
      copyProgress: state.copyProgress
    };
}

export default connect(mapStateToProps)(App);

