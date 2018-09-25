import React from 'react';
import { connect } from 'react-redux';

class Client extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            port: 8963, 
            ip: "127.0.0.1"
        };
    }

    /**
     * @description Load defaults.
     */
    componentWillMount() {
        MusicPlayer.getDefaultIP().then((ip) => {
            this.setState({ 
                ip: ip
            });
        });
    }

    shouldComponentUpdate(nextprops, nextstate) {
        if (JSON.stringify(nextprops) !== JSON.stringify(this.props) || JSON.stringify(nextstate) !== JSON.stringify(this.state)){
            return true;
        }

        return false;
    }

    /**
     * @description Connect to a music server.
     */
    connectToServer() {
        MusicPlayer.connectToServer(this.state.ip, this.state.port);
    }

    /**
     * @description Disconnects the server.
     */
    disconnectServer() {
        MusicPlayer.disconnectServer();
    }

    /**
     * @desc The ip changed.
     * @param {string} ip 
     */
    onChangeIp(ip) {
        this.setState({
            ip: ip
        });
    }

    /**
     * @description Render the connect options.
     */
    renderConnect() {
        return (
            <div className="server-connect col h-100">
                <h2>Connect to a stream</h2>
                <p>By connecting to a stream you can listen to another user's music.</p>
                <input type="text" value={this.state.ip} onChange={(e) => this.onChangeIp(e.target.value)} />
                <input type="number" value={this.state.port} onChange={(e) => this.setState({port: e.target.value})} />
                <button className="primary-button" onClick={() => this.connectToServer()}>Connect</button>
            </div>
        )
    }

    /**
     * @description Render the server info.
     */
    renderServerInfo() {
        return (
            <div className="server-clients col h-100">
                <div>
                    <div>
                        <p>Connected to {this.props.serverInfo.Host}:{this.props.serverInfo.Port}</p>
                        <button onClick={() => this.disconnectServer()} className="primary-button">Disconnect</button>
                    </div>
                </div>
            </div>
        )
    }

    render() {
        return (
            <div className="server row h-100">
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

export default connect(mapStateToProps)(Client);