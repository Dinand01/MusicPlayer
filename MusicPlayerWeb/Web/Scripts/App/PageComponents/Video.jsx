import React from 'react';
import { connect } from 'react-redux';

/**
 * @class The component for interacting with youtube.
 */
class Video extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            videoUrl: "",
            modifiedUrl: null,
            currentVolume: 0,
            isPlaying: false,
            previousPlayerState: -1
        };
    }

    /**
     * @desc The componnet did mount.
     */
    componentDidMount() {
        if (this.props.serverInfo && this.props.serverInfo.VideoUrl) {
            this.changeUrl(this.props.serverInfo.VideoUrl);
        }

        MusicPlayer.getVolume().then((v) => {
            this.setState({
                currentVolume: v
            });
        });
        
        this.volumeChecker = setInterval(() => this.changeVolume(), 1500);
    }

    /**
     * @desc Receive the next properties.
     * @param {object} nextprops The next properties. 
     */
    componentWillReceiveProps(nextprops) {
        if (nextprops.serverInfo && !nextprops.serverInfo.IsHost) {
            if (nextprops.serverInfo.VideoUrl !== null && this.state.videoUrl !== nextprops.serverInfo.VideoUrl) {
                console.log("change url: ");
                console.log(nextprops.serverInfo);
                this.changeUrl(nextprops.serverInfo.VideoUrl);
            }

            if (this.player && !nextprops.serverInfo.VideoUrl && this.props.serverInfo.VideoPosition !== nextprops.serverInfo.VideoPosition &&
                Math.abs(this.player.getCurrentTime() - nextprops.serverInfo.VideoPosition) > 5) {
                console.log("Seek to ");
                console.log(nextprops.serverInfo);
                this.player.seekTo(nextprops.serverInfo.VideoPosition, true);
            }
        }
    }

    /**
     * @desc The component will unmount.
     */
    componentWillUnmount() {
        clearInterval(this.volumeChecker);
        MusicPlayer.stopVideo();
    }

    /**
     * @desc Adds the youtube player to the page.
     * @param {string} id 
     */
    addYoutubePlayer(id) {
        if (this.player == null) {
            this.player = new YT.Player('youtube-player', {
                // height: '390',
                // width: '640',
                videoId: id,
                suggestedQuality: "hd1080",
                events: {
                    'onReady': (event) => {
                        this.player.setVolume(this.state.currentVolume);
                        event.target.playVideo();
                    },
                    'onStateChange': (event) => {
                        if (event.data == YT.PlayerState.BUFFERING && this.state.previousPlayerState == YT.PlayerState.PAUSED) {
                            MusicPlayer.seekVideo(this.player.getCurrentTime());
                        }

                        if (event.data == YT.PlayerState.PLAYING || event.data == YT.PlayerState.BUFFERING || YT.PlayerState.PAUSED) {
                            this.setState({isPlaying: true});
                        } else {
                            this.setState({isPlaying: false});
                            MusicPlayer.stopVideo();
                        }

                        this.setState({previousPlayerState: event.data});
                    }
                }
            });
        } else {
            this.player.loadVideoById(id);
        }
    }

    /**
     * @desc change the volume to that of the player.
     */
    changeVolume() {
        if (this.player && this.player.isMuted) {
            let vol = this.player.isMuted() ? 0 : null;
            if (vol === null) {
                vol = this.player.getVolume();
            }

            if (vol !== this.state.currentVolume) {
                this.setState({
                    currentVolume: vol
                }, () => {
                    MusicPlayer.setVolume(parseInt(vol));
                });
            }
        }
    }

    /**
     * @desc Gets the youtube video id from the url.
     */
    getVideoID() {
        if (this.state.videoUrl) {
            let parts = this.state.videoUrl.split("?v=");
            if (parts.length < 2) {
                parts = this.state.videoUrl.split("/_");
            }

            if (parts.length >= 2) {
                return parts[1];
            }
        }

        return null;
    }

    /**
     * @desc Generates an youtube player url from the video url.
     */
    playVideo() {
        let id = this.getVideoID();
        if (id) {
            this.addYoutubePlayer(id);
            MusicPlayer.startVideo(this.state.videoUrl);
        } else if (this.player && this.player.getPlayerState() == YT.PlayerState.PLAYING) {
            this.player.stopVideo();
        }
    }

    /**
     * @desc Change the video url.
     * @param {string} url The new url.
     */
    changeUrl(url) {
        this.setState({
            videoUrl: url,
            isPlaying: false,
            previousPlayerState: -1
        }, () => {
            this.playVideo();
        });
    }

    /**
     * @desc Select all text in the input.
     * @param {object} e The event. 
     */
    selectAll(e) {
        e.target.select();
    }

    /**
     * @desc Render the component.
     */
    render() {
        return (
            <div className="video">
                <header>
                    <input type="text" 
                        value={this.state.videoUrl} 
                        readOnly={this.props.serverInfo && !this.props.serverInfo.IsHost} 
                        onChange={(e) => this.changeUrl(e.target.value)}
                        onClick={(e) => this.selectAll(e)}
                        placeholder="Enter video url" />
                </header>
                <section style={this.state.isPlaying ? {} : {display: "none"}} onBlur={() => this.changeVolume()}>
                    <div id="youtube-player"></div>
                </section>
                <section style={!this.state.isPlaying ? {} : {display: "none"}}>
                    <i className="fa fa-youtube-square"></i>
                </section>
            </div>
        );
    }
}

function mapStateToProps(state) {
    return { 
        serverInfo: state.serverInfo
      };
  }

export default connect(mapStateToProps)(Video);