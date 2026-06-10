import React, { useState, useEffect } from 'react';
import { withRouter } from 'react-router-dom';
import RadioList from '../Parts/Radio/RadioList.jsx';
import { parseJSON } from '../Helpers/Methods.jsx';

const Radio = ({ match, history }) => {
    const [searchText, setSearchText] = useState("");
    const [allStations, setAllStations] = useState([]);
    const [stations, setStations] = useState([]);
    const [index, setIndex] = useState(0);

    useEffect(() => {
        getStations();
    }, [searchText]);

    useEffect(() => {
        // When returning to this route (isExact changes from false to true)
        if (match.isExact) {
            getStations();
        }
    }, [match.isExact]);

    const changeSearchText = (text) => {
        setSearchText(text);
    };

    const getStations = () => {
        MusicPlayer.getRadioStations(searchText).then((json) => {
            let stations = parseJSON(json);
            if (stations && stations.length) {
                setAllStations(stations);
                setIndex(0);
                requestSongs(0, 25);
            }
        });
    };

    const requestSongs = (skip, amount) => {
        if (allStations && allStations.length) {
            skip = skip > 0 ? skip : 0;
            setStations(prevStations => prevStations.concat(allStations.slice(skip, skip + amount)));
        }
    };

    if (!match.isExact) {
        return null;
    }

    return(
        <div className="row h-100">
            <div className="col">
                <div className="h-100 d-flex flex-column">
                    <div className="row justify-content-center h-35-px">
                        <div className="col">
                            <input type="text" className="w-100 ml-0" placeholder="Search" value={searchText} onChange={e => changeSearchText(e.target.value)} />
                        </div>
                        <div className="col-1">
                            <button className="iconButton h-100" onClick={() => history.push("/radio/0")}>
                                <i className="fas fa-plus-square fa-2x"></i>
                            </button>
                        </div>
                    </div>
                    <div className="row justify-content-center flex-grow-1 pt-2">
                        <RadioList radioStations={stations} requestSongs={(skip, amount) => requestSongs(skip, amount)} />
                    </div>
                </div>
            </div>
        </div> 
    );
};

export default withRouter(Radio);