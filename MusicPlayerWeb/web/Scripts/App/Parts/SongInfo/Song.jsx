import React, { useState, useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import { store } from '../../DataStore/Store.jsx';
import { SongActions } from '../../DataStore/StoreActions.jsx';
import Slider from 'rc-slider';
import SongPosition from './SongPosition.jsx';

const Song = ({ history }) => {
    const serverInfo = useSelector(state => state.serverInfo);
    const currentSong = useSelector(state => state.currentSong);
    const [shuffle, setShuffle] = useState(false);
    const [volume, setVolumeState] = useState(0);
    const [prevVolume, setPrevVolume] = useState(0);
    const volTimeoutRef = useRef(null);

    let controlsDisbled = (serverInfo && !serverInfo.IsHost) || (currentSong && currentSong.IsInternetRadio);

    useEffect(() => {
        Promise.all([MusicPlayer.getShuffle(), MusicPlayer.getVolume(), MusicPlayer.getCurrentSong()]).then((arr) => {
            setShuffle(arr[0]);
            setVolumeState(arr[1]);
            setPrevVolume(arr[1]);
            store.dispatch(SongActions.setCurrentSong(JSON.parse(arr[2])));
        });
    }, []);

    const nextSong = () => {
        MusicPlayer.nextSong();
    };

    const togglePlaySong = () => {
        MusicPlayer.togglePlay();
    };

    const moveToTime = (value) => {
        MusicPlayer.moveToTime(value);
    };

    const setVolume = (value, toggle) => {
        setPrevVolume(toggle ? prevVolume : value);
        setVolumeState(toggle ? (volume > 0 ? 0 : prevVolume) : value);

        if (volTimeoutRef.current) {
            clearTimeout(volTimeoutRef.current);
        }

        volTimeoutRef.current = setTimeout(() => {
            MusicPlayer.setVolume(toggle ? (volume > 0 ? 0 : prevVolume) : value);
        }, 300);
    };

    const scrollVolume = (opt) => {
        if (opt.deltaY > 0) {
            setVolume(volume - 2);
        } else {
            setVolume(volume + 2);
        }
    };

    const stop = () => {
        MusicPlayer.stop();
        history.push("/");
    };

    const shuffleToggle = () => {
        setShuffle(!shuffle);
        MusicPlayer.shuffle(!shuffle);
    };

    return (
        <div className="row">
             <div className="col-5 songImage">
                {!(currentSong && (currentSong.Image || currentSong.ImageUrl)) 
                    && <i className="far fa-9x fa-image" />}
                {currentSong && (currentSong.Image || currentSong.ImageUrl) 
                    && <img src={currentSong.Image ? "data:image/png;base64," + currentSong.Image : currentSong.ImageUrl} alt="Song image" />}
            </div>
            <div className="col-7">
                <div className="row pb-3"> 
                    <div className="col-8">
                        <div className="row align-items-center">
                            {currentSong &&
                            <button onClick={() => stop()} title="Stop music" className="col-1 iconButton">
                                <i className={"fa fa-stop"} />
                            </button>}
                            {currentSong &&
                            <button onClick={() => togglePlaySong()} title={currentSong.IsPlaying ? "Pause" : "Play"} className="col-1 iconButton">
                                <i className={"fa " + (currentSong.IsPlaying ? "fa-pause" : "fa-play")} />
                            </button>}
                            {currentSong &&
                            <SongPosition 
                                moveToTime={(val) => moveToTime(val)}
                                disabled={controlsDisbled} />}
                            <button onClick={() => nextSong()} disabled={controlsDisbled} title={"Next song"} className="col-1 iconButton">
                                <i className="fa fa-step-forward" />
                                </button>
                            <button onClick={() => shuffleToggle()} disabled={controlsDisbled} title="Toggle shuffle" className="col-1 iconButton">
                                <i className={"fas " + (shuffle ? "fa-random" : "fa-exchange-alt")} />
                            </button>
                        </div>
                    </div>
                    <div className="col-4" title="Volume" onWheel={opt => scrollVolume(opt)}>
                        <div className="volumeSlider">
                            <Slider 
                                className=""
                                value={volume}
                                onChange={val => setVolume(val)} /> 
                            <button onClick={() => setVolume(volume, true)} className="iconButton">
                                <i className={"fa " + (volume > 0 ? "fa-volume-up" : "fa-volume-off")} />
                            </button>
                        </div>
                    </div>
                </div>
                 <div className="row">
                    {currentSong && 
                    <div className="col">
                        <div className="row">
                            <div className="col">
                                <h2>{currentSong.Title}</h2>
                                <h4>{currentSong.Band}</h4>
                            </div>
                        </div>
                        {currentSong.Album && <div className="row">
                            <p className="col-4">Album: </p>
                            <p className="col-8">{currentSong.Album}</p>
                        </div>}
                        {currentSong.Gengre && <div className="row">
                            <p className="col-4">Gengre: </p>
                            <p className="col-8">{currentSong.Gengre}</p>
                        </div>}
                        {currentSong.DateCreated && <div className="row">
                            <p className="col-4">Created: </p>
                            <p className="col-8">{(new Date(currentSong.DateCreated)).getFullYear()}</p>
                        </div>}
                    </div>}
                </div> 

            </div>
        </div>
    );
};

export default Song;