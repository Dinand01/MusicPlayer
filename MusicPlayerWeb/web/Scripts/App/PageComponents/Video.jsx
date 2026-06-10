import React, { useState, useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';

const Video = ({ match }) => {
    const serverInfo = useSelector(state => state.serverInfo);
    const [videoUrl, setVideoUrl] = useState("");
    const [videoInfo, setVideoInfo] = useState([]);
    const [currentVideoIndex, setCurrentVideoIndex] = useState(0);
    const [currentVolume, setCurrentVolume] = useState(0);
    const [isPlaying, setIsPlaying] = useState(false);
    const [previousPlayerState, setPreviousPlayerState] = useState(-1);
    const playerRef = useRef(null);
    const volumeCheckerRef = useRef(null);

    useEffect(() => {
        if (serverInfo && serverInfo.VideoUrl) {
            changeUrl(serverInfo.VideoUrl);
        }

        if (!serverInfo || serverInfo.IsHost) {
            loadVideoInfo();
        }

        MusicPlayer.getVolume().then((v) => {
            setCurrentVolume(v);
        });

        volumeCheckerRef.current = setInterval(() => changeVolume(), 1500);

        return () => {
            clearInterval(volumeCheckerRef.current);
            MusicPlayer.stopVideo();
        };
    }, []);

    useEffect(() => {
        if (serverInfo && !serverInfo.IsHost) {
            if (serverInfo.VideoUrl !== null && videoUrl !== serverInfo.VideoUrl) {
                changeUrl(serverInfo.VideoUrl);
            }

            if (playerRef.current && serverInfo.VideoPosition && 
                Math.abs(playerRef.current.getCurrentTime() - serverInfo.VideoPosition) > 5) {
                playerRef.current.seekTo(serverInfo.VideoPosition, true);
            }
        }
    }, [serverInfo]);

    const addYoutubePlayer = (id) => {
        if (playerRef.current == null) {
            playerRef.current = new YT.Player('youtube-player', {
                videoId: id,
                suggestedQuality: "hd1080",
                events: {
                    'onReady': (event) => {
                        if(!isNaN(currentVolume)) {
                            playerRef.current.setVolume(parseInt(currentVolume));
                        }
                        event.target.playVideo();
                    },
                    'onStateChange': (event) => {
                        if (event.data == YT.PlayerState.BUFFERING && previousPlayerState == YT.PlayerState.PAUSED) {
                            MusicPlayer.seekVideo(playerRef.current.getCurrentTime());
                        }

                        if (event.data == YT.PlayerState.PLAYING || event.data == YT.PlayerState.BUFFERING || event.data == YT.PlayerState.PAUSED) {
                            setIsPlaying(true);
                        } else if (event.data == YT.PlayerState.ENDED && videoInfo.length 
                                    && currentVideoIndex < (videoInfo.length - 1)
                                    && !(serverInfo && !serverInfo.IsHost)) {
                            let newindex = currentVideoIndex + 1;
                            setCurrentVideoIndex(newindex);
                            changeUrl(videoInfo[newindex].Url);
                        } else if (event.data != YT.PlayerState.UNSTARTED) {
                            setIsPlaying(false);
                            MusicPlayer.stopVideo();
                        }

                        setPreviousPlayerState(event.data);
                    }
                }
            });
        } else {
            playerRef.current.loadVideoById(id);
        }
    };

    const changeVolume = () => {
        if (playerRef.current && playerRef.current.isMuted) {
            let vol = playerRef.current.isMuted() ? 0 : null;
            if (vol === null) {
                vol = playerRef.current.getVolume();
            }

            if (vol !== currentVolume) {
                setCurrentVolume(vol);
                MusicPlayer.setVolume(parseInt(vol));
            }
        }
    };

    const resolveVideoUrl = () => {
        if (videoUrl) {
            if (videoUrl.indexOf("list=") > -1) {
                let parts = videoUrl.split("list=");
                let playlistID = parts[parts.length - 1];
                loadVideoInfo(playlistID);
            } else if (videoUrl.indexOf("?v=") > -1) {
                let parts = videoUrl.split("?v=");
                return parts[parts.length - 1].length === 11 ? parts[parts.length - 1] : null;
            }
        }
        return null;
    };

    const playVideo = () => {
        let id = resolveVideoUrl();
        if (id) {
            addYoutubePlayer(id);
            MusicPlayer.startVideo(videoUrl);
        } else if (playerRef.current && playerRef.current.getPlayerState() == YT.PlayerState.PLAYING) {
            playerRef.current.stopVideo();
        }
    };

    const clickVid = (id) => {
        var index = videoInfo.map(v => v.ID).indexOf(id);
        setCurrentVideoIndex(index);
        changeUrl(videoInfo[index].Url);
    };

    const changeUrl = (url) => {
        setVideoUrl(url);
        setIsPlaying(false);
        setPreviousPlayerState(-1);
        playVideo();
    };

    const loadVideoInfo = (playlistID) => {
        if (!serverInfo || serverInfo.IsHost) {
            let func = info => {
                setVideoInfo(JSON.parse(info));
            };

            if (playlistID) {
                MusicPlayer.getVideoInfoFromPlaylist(playlistID).then(func);
                return;
            }

            MusicPlayer.getChannelVideos().then(func);
        }
    };

    const selectAll = (e) => {
        e.target.select();
    };

    return (
        <div className="video">
            <header>
                <input type="text" 
                    value={videoUrl} 
                    readOnly={serverInfo && !serverInfo.IsHost} 
                    onChange={(e) => changeUrl(e.target.value)}
                    onClick={(e) => selectAll(e)}
                    placeholder="Enter video or playlist url" />
            </header>
            <section style={isPlaying ? {} : {display: "none"}} onBlur={() => changeVolume()}>
                <div id="youtube-player"></div>
            </section>
            <section style={!isPlaying ? {} : {display: "none"}}>
                {videoInfo.length == 0 && <i className="fab fa-youtube"></i>}
                <div className="video-thumb-container">
                {(() =>  { 
                    return videoInfo.map(info => {
                        return (
                            <div className="video-thumb" title={info.Description} key={info.ID} onClick={() => clickVid(info.ID)}>
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
};

export default Video;