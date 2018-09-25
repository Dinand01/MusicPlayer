import React from "react";
import { connect } from "react-redux";
import ProgressCircle from "react-progress-circle";

/**
 * @class The class with the copy page form.
 */
class Copy extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            source: "",
            destination: "",
            amount: 500
        };
    }

    /**
     * @description Select a folder.
     * @param {string} folder The state folder property. 
     */
    selectFolder(folder) {
        MusicPlayer.selectFolder().then(res => {
            let state = {};
            state[folder] = res;
            this.setState(state);
        });
    }

    /**
     * @description Start the copy process.
     */
    copyFiles() {
        MusicPlayer.copySongs(this.state.source, this.state.destination, parseInt(this.state.amount));
    }

    /**
     * @description Render the page.
     */
    render() {
        return (
            <div className="copy-page row h-100">
                {this.props.progress &&
                <div className="copy-progress">
                    <div>
                        <ProgressCircle
                            backgroundColor="#400080"
                            color="#699D35"
                            labelColor="#0e1314"
                            labelSize="26px"
                            size="150"
                            status={parseInt(this.props.progress)}
                            />
                    </div>
                </div>}
                {!this.props.progress &&
                <div className="copy-files-form">
                    <div className="flex-1x">
                        <h2>Copy random audio files</h2>
                    </div>
                    <div className="flex-2x">
                        <button onClick={() => this.selectFolder("source")} title="The folder to copy files from" className="primary-button">
                            {this.state.source ? "Change source" :  "Source"}
                        </button>
                        <p>{this.state.source}</p>
                    </div>
                    <div className="flex-2x">
                        <button onClick={() => this.selectFolder("destination")} title="The folder to copy files to" className="primary-button">
                            {this.state.destination ? "Change destination" : "Destination"}
                        </button>
                        <p>{this.state.destination}</p>
                    </div>
                    <div className="flex-1x">
                        <input type="number" value={this.state.amount} onChange={(e) => this.setState({amount : e.target.value})} />
                    </div>
                    <div className="flex-1x">
                        <button disabled={!this.state.source || !this.state.destination || !this.state.amount} onClick={() => this.copyFiles()} title="Copy the files" className="primary-button">
                            Copy
                        </button>
                    </div>
                </div>}
            </div>
        );
    }
}
function mapStateToProps(state) {
  return { 
      progress: state.copyProgress
    };
}

export default connect(mapStateToProps)(Copy);