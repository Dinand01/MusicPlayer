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
            videoInfo: [],
            currentVideoIndex: 0,
            modifiedUrl: null,
            currentVolume: 0,
            isPlaying: false,
            previousPlayerState: -1
        };
    }

    /**
     * @desc The component did mount.
     */
    componentDidMount() {
        if (this.props.serverInfo && this.props.serverInfo.VideoUrl) {
            this.changeUrl(this.props.serverInfo.VideoUrl);
        }

        if (!this.props.serverInfo || this.props.serverInfo.IsHost) {
            this.loadVideoInfo();
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
                this.changeUrl(nextprops.serverInfo.VideoUrl);
            }

            if (this.player && this.props.serverInfo.VideoPosition !== nextprops.serverInfo.VideoPosition && this.player.getCurrentTime &&
                Math.abs(this.player.getCurrentTime() - nextprops.serverInfo.VideoPosition) > 5) {
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
                videoId: id,
                suggestedQuality: "hd1080",
                events: {
                    'onReady': (event) => {
                        if(!isNaN(this.state.currentVolume)) {
                            this.player.setVolume(parseInt(this.state.currentVolume));
                        }

                        event.target.playVideo();
                    },
                    'onStateChange': (event) => {
                        if (event.data == YT.PlayerState.BUFFERING && this.state.previousPlayerState == YT.PlayerState.PAUSED) {
                            MusicPlayer.seekVideo(this.player.getCurrentTime());
                        }

                        if (event.data == YT.PlayerState.PLAYING || event.data == YT.PlayerState.BUFFERING || event.data == YT.PlayerState.PAUSED) {
                            this.setState({isPlaying: true});
                        } else if (event.data == YT.PlayerState.ENDED && this.state.videoInfo.length 
                                    && this.state.currentVideoIndex < (this.state.videoInfo.length - 1)
                                    && !(this.props.serverInfo && !this.props.serverInfo.IsHost)) {
                            let newindex = this.state.currentVideoIndex + 1;
                            this.setState({
                                currentVideoIndex: newindex 
                            }, () => {
                                this.changeUrl(this.state.videoInfo[newindex].Url);
                            });
                        } else  if (event.data != YT.PlayerState.UNSTARTED) {
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
     * @desc Gets the youtube video id from the url when posible.
     */
    resolveVideoUrl() {
        if (this.state.videoUrl) {
            if (this.state.videoUrl.indexOf("list=") > -1) {
                let parts = this.state.videoUrl.split("list=");
                let playlistID = parts[parts.length - 1];
                this.loadVideoInfo(playlistID);
            } else if (this.state.videoUrl.indexOf("?v=") > -1) {
                let parts = this.state.videoUrl.split("?v=");
                return parts[parts.length - 1].length === 11 ? parts[parts.length - 1] : null;
            }
        }

        return null;
    }

    /**
     * @desc Generates an youtube player url from the video url.
     */
    playVideo() {
        let id = this.resolveVideoUrl();
        if (id) {
            this.addYoutubePlayer(id);
            MusicPlayer.startVideo(this.state.videoUrl);
        } else if (this.player && this.player.getPlayerState() == YT.PlayerState.PLAYING) {
            this.player.stopVideo();
        }
    }

    /**
     * @desc Click on a video thumb.
     * @param {string} id The id of the video. 
     */
    clickVid(id) {
        var index = this.state.videoInfo.map(v => v.ID).indexOf(id);
        this.setState({
            currentVideoIndex: index
        }, () => {
            this.changeUrl(this.state.videoInfo[index].Url);
        })
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
     * @desc Loads video info.
     * @param {string} playlistID The youtube playlist id. 
     */
    loadVideoInfo(playlistID) {
        if (!this.props.serverinfo || this.props.serverinfo.IsHost) {
            let func = info => {
                this.setState({
                    videoInfo: JSON.parse(info)
                });
            };

            if (playlistID) {
                MusicPlayer.getVideoInfoFromPlaylist(playlistID).then(func);
                return;
            }

            MusicPlayer.getChannelVideos().then(func);
        }

        return;
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
                        placeholder="Enter video or playlist url" />
                </header>
                <section style={this.state.isPlaying ? {} : {display: "none"}} onBlur={() => this.changeVolume()}>
                    <div id="youtube-player"></div>
                </section>
                <section style={!this.state.isPlaying ? {} : {display: "none"}}>
                    {this.state.videoInfo.length == 0 && <i className="fa fa-youtube-square"></i>}
                    <div className="video-thumb-container">
                    {(() =>  { 
                        return this.state.videoInfo.map(info => {
                            return (
                                <div className="video-thumb" title={info.Description} key={info.ID} onClick={() => this.clickVid(info.ID)}>
                                    <p>{info.Title}</p>
                                    <img src={info.ThumbnailUrl} alt={info.Title} /> 
                                </div>
                            );
                        });
                    })()}
                    </div>
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