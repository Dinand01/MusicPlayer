import React from 'react';
import { connect } from 'react-redux';

class Server extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            port: 8963
        };
    }

    shouldComponentUpdate(nextprops, nextstate) {
        if (JSON.stringify(nextprops) !== JSON.stringify(this.props) || JSON.stringify(nextstate) !== JSON.stringify(this.state)){
            return true;
        }

        return false;
    }

    /**
     * @description Host a server.
     */
    hostServer() {
        MusicPlayer.hostServer(this.state.port);
    }

    /**
     * @description Disconnects the server.
     */
    disconnectServer() {
        MusicPlayer.disconnectServer();
    }

    /**
     * @description Render the connect options.
     */
    renderConnect() {
        return (
            <div className="server-connect">
                <h2>Host music</h2>
                <p>By port forwarding the following port to your pc people can listen in on your music.</p>
                <input type="number" value={this.state.port} onChange={(e) => this.setState({port: e.target.value})} />
                <button className="bigButton" onClick={() => this.hostServer()}>Host</button>
            </div>
        )
    }

    /**
     * @description Render the server info.
     */
    renderServerInfo() {
        return (
            <div className="server-clients">
                <div>
                    <div>
                        <p>Connected Clients:</p>
                        {(() => {
                            let res = [];
                            for (var key in this.props.serverInfo.Clients) {
                                res.push(<p key={key}>{key}: {this.props.serverInfo.Clients[key]}</p>);
                            }

                            return res;
                        })()}
                        <button onClick={() => this.disconnectServer()}>Disconnect</button>
                    </div>
                </div>
            </div>
        )
    }

    render() {
        return (
            <div className="server">
                {!this.props.serverInfo && this.renderConnect()}
                {this.props.serverInfo && this.renderServerInfo()}
            </div>
        );
    }
}

function mapStateToProps(state) {
  return { 
      serverInfo: state.serverInfo
    };
}

export default connect(mapStateToProps)(Server);