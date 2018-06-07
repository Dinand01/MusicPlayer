import React from 'react';
import { withRouter } from 'react-router-dom';
import { connect } from 'react-redux';
import { store } from '../../DataStore/Store.jsx';
import { SongActions } from '../../DataStore/StoreActions.jsx';
import Slider from 'rc-slider';
import SongPosition from './SongPosition.jsx';

/**
 * @class The component for rendering the current song and controls.
 */
class Song extends React.Component {
    constructor(props) {
        super(props);
        this.state =  {
            shuffle: false,
            volume: 0,
            prevVolume: 0
        };

        this.volTimeout = null;
    }

    /**
     * @desc Prevent render when songposition changes.
     * @param {object} nextprops The next properties. 
     * @param {*} nextstate The next state.
     */
    shouldComponentUpdate(nextprops, nextstate) {
        if(JSON.stringify(nextstate) !== JSON.stringify(this.state)
            || (nextprops.currentSong && !this.props.currentSong)
            || (nextprops.currentSong && this.props.currentSong && (
                nextprops.currentSong.Location !== this.props.currentSong.Location
                || nextprops.currentSong.Title !== this.props.currentSong.Title
                || nextprops.currentSong.IsPlaying !== this.props.currentSong.IsPlaying))
            || JSON.stringify(this.props.serverInfo) !== JSON.stringify(nextprops.serverInfo)) {
                return true;
            }

        return false;
    }

    /**
     * @description The componnet will mount.
     */
    componentWillMount() {
        Promise.all([MusicPlayer.getShuffle(), MusicPlayer.getVolume(), MusicPlayer.getCurrentSong()]).then((arr) => {
            this.setState({
                shuffle: arr[0],
                volume: arr[1],
                prevVolume: arr[1]
            });

            store.dispatch(SongActions.setCurrentSong(JSON.parse(arr[2])));
        });
    }

    /**
     * @description Advance one song.
     */
    nextSong() {
        MusicPlayer.nextSong();
    }

    /**
     * @description Toggles the play/pause state current song.
     */
    togglePlaySong() {
        MusicPlayer.togglePlay();
    }

    /**
     * @description Scroll to the requested time.
     * @param {int} value The value in seconds.
     */
    moveToTime(value) {
        MusicPlayer.moveToTime(value);
    }

    /**
     * @description Set the volume.
     * @param {int} value The new volume setting. 
     * @param {bool} toggle Toggle the volume.
     */
    setVolume(value, toggle) {
        this.setState({
            prevVolume: toggle ? this.state.prevVolume : value,
            volume: toggle ? (this.state.volume > 0 ? 0 : this.state.prevVolume) : value
        }, () => {
            if (this.volTimeout) {
                clearTimeout(this.volTimeout);
            }

            this.volTimeout = setTimeout(() => {
                MusicPlayer.setVolume(this.state.volume);
            }, 300);
        });
    }

    /**
     * @desc Sets the volum when the user scrolls over the volume slider.
     * @param {object} opt The scroll event. 
     */
    scrollVolume(opt) {
        if (opt.deltaY > 0) {
            this.setVolume(this.state.volume - 2);
        } else {
            this.setVolume(this.state.volume + 2);
        }
    }

    /**
     * @description Stop playing music.
     */
    stop(history) {
        MusicPlayer.stop();
        this.props.history.push("/");
    }

    /**
     * @description Change the shuffle setting.
     */
    shuffle() {
        this.setState({ 
            shuffle: !this.state.shuffle
        }, () => {
            MusicPlayer.shuffle(this.state.shuffle);
        });
    }

    /**
     * @description Render the component.
     */
    render() {
        let controlsDisbled = this.props.serverInfo && !this.props.serverInfo.IsHost;
        return (
            <div className="song-info">
                 <div>
                    {!(this.props.currentSong && this.props.currentSong.Image) && <i className="fa fa-9x fa-picture-o" />}
                    {this.props.currentSong && this.props.currentSong.Image && <img src={"data:image/png;base64," + this.props.currentSong.Image}/>}
                </div>
                <div>
                   <div>
                        <div className="song-controls"> 
                            <div>
                                {this.props.currentSong &&
                                <button onClick={() => this.stop(history)} title="Stop music"><i className={"fa fa-stop"} /></button>}
                                {this.props.currentSong &&
                                <button onClick={() => this.togglePlaySong()} title={this.props.currentSong.IsPlaying ? "Pause" : "Play"}>
                                    <i className={"fa " + (this.props.currentSong.IsPlaying ? "fa-pause" : "fa-play")} />
                                </button>}
                                {this.props.currentSong &&
                                 <SongPosition 
                                    moveToTime={(val) => this.moveToTime(val)}
                                    disabled={controlsDisbled} />}
                                <button onClick={() => this.nextSong()} disabled={controlsDisbled} title={"Next song"}>
                                    <i className="fa fa-step-forward" />
                                    </button>
                                <button onClick={() => this.shuffle()} disabled={controlsDisbled} title="Toggle shuffle">
                                    <i className={"fa " + (this.state.shuffle ? "fa-random" : "fa-exchange")} />
                                </button>
                            </div>
                            <div title="Volume" onWheel={opt => this.scrollVolume(opt)}>
                                <div>
                                    <button onClick={() => this.setVolume(this.state.volume, true)} className="song-volume">
                                        <i className={"fa " + (this.state.volume > 0 ? "fa-volume-up" : "fa-volume-off")} />
                                    </button>
                                    <Slider 
                                        className="song-volume"
                                        value={this.state.volume}
                                        onChange={val => this.setVolume(val)} /> 
                                </div>
                            </div>
                        </div>
                         <div className="song-details">
                            {this.props.currentSong && 
                            <div>
                                <div>
                                    <h2>{this.props.currentSong.Title}</h2>
                                    <h3>{this.props.currentSong.Band}</h3>
                                </div>
                                {this.props.currentSong.Album && <div className="info-pair">
                                    <p>Album: </p>
                                    <p>{this.props.currentSong.Album}</p>
                                </div>}
                                {this.props.currentSong.Gengre && <div className="info-pair">
                                    <p>Gengre: </p>
                                    <p>{this.props.currentSong.Gengre}</p>
                                </div>}
                                {this.props.currentSong.DateCreated && <div className="info-pair">
                                    <p>Created: </p>
                                    <p>{(new Date(this.props.currentSong.DateCreated)).getFullYear()}</p>
                                </div>}
                            </div>}
                        </div> 
                    </div>
                </div> 
            </div>
        )
    }
}

function mapStateToProps(state) {
    return { 
        serverInfo: state.serverInfo,
        currentSong: state.currentSong
      };
  }
  
export default withRouter(connect(mapStateToProps)(Song));

