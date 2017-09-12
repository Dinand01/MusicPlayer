import React from 'react';
import Loader from "react-loader";
import Waypoint from 'react-waypoint';
import SongListItem from './SongListItem.jsx';
import { parseJSON } from '../../Helpers/Methods.jsx';

export default class SongList extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            songs: [],
            querry: "",
            endReached: false,
            isLoaded: false
        }

        this.timeout = null;
    }

    /**
     * @description The componnet will mount.
     */
    componentWillMount() {
        if (this.state.songs.length == 0) {
            this.getSongs();
        }
    }

    /**
     * @description Limit the render cycles to what effect this component.
     */
    shouldComponentUpdate(nextprops, nextstate) {
        if (JSON.stringify(nextstate) !== JSON.stringify(this.state) 
            || !this.props.currentSong
            || (nextprops.currentSong && nextprops.currentSong.Location != this.props.currentSong.Location)
            || this.props.receiveMode != nextprops.receiveMode) {
                return true;
        }

        return false;
    }

    /**
     * @description The componnet will receive new props.
     * @param {object} nextprops 
     */
    componentWillReceiveProps(nextprops) {
        if (nextprops.receiveMode && this.props.currentSong && nextprops.currentSong && nextprops.currentSong.Location != this.props.currentSong.Location) {
            this.setState({
                endReached: false,
                songs: []
            }, () => this.getSongs());
        }
    }

    /**
     * @description Change the querry text.
     * @param {string} text The querry text; 
     */
    changeQuerry(text) {
        this.setState({
            querry: text,
            songs: [],
            endReached: false
        }, () => {
            clearTimeout(this.timeout);
            this.timeout = setTimeout(() => {
                this.getSongs();
            }, 800);
        });
    }

    /**
     * @description Request songs.
     */
    getSongs() {
        if (!this.state.endReached) {
            this.setState({isLoaded: false});
            MusicPlayer.getSongs(this.state.songs.length, this.state.querry).then((songsJSON) => {
                this.setState({isLoaded: true});
                let songs = parseJSON(songsJSON);
                if (songs && songs.length) {
                    this.setState({
                        endReached: false,
                        songs: this.props.receiveMode ? songs : this.state.songs.concat(songs)
                    });
                } else {
                    this.setState({endReached: true});
                }
            });
        }
    }

    /**
     * @description Render the song list.
     */
    render() {
        return (
            <div className="songlist">
                <div className="songlist-head">
                    <input type="text" value={this.state.querry} onChange={(e) => this.changeQuerry(e.target.value)} disabled={this.props.receiveMode} />
                </div>
                <div className="songlist-body">
                    <div>
                        <div className="songlist-header">
                            <div>
                                <div style={{width: '50%'}}>Title</div>
                                <div style={{width: '20%'}}>Band</div>
                                <div style={{width: '20%'}}>Genre</div>
                                <div style={{width: '10%'}}>Time</div>
                            </div>
                        </div>
                        <div className="songlist-songs">
                            <div>
                                <div>
                                    {this.state.songs.map((s, i) => <SongListItem key={i} song={s} />)}
                                    <Waypoint onEnter={() => this.state.songs.length > 0 && this.getSongs()} bottomOffset="-100px" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <Loader loaded={this.state.isLoaded} />
            </div>
        )
    }
}