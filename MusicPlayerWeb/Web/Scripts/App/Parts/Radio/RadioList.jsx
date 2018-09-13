import React from 'react';
import Waypoint from 'react-waypoint';
import { withRouter } from 'react-router-dom';

/**
 * @class Displays the radio station list.
 */
class RadioList extends React.Component {
    constructor(props) {
        super(props);
    }

    /**
     * @desc Requests audio playback from the station. 
     * @param {object} station The radio station object.
     */
    selectRadio(station) {
        MusicPlayer.playFromURL(station.Url);
        this.props.history.push('/playlist');
    }
    
    /**
     * @desc Edit the data of a radio station.
     * @param {object} radio The radio station.
     * @param {object} event The javascript event. 
     */
    editRadio(radio, event) {
        this.props.history.push('/radio/' + radio.ID);
        event.stopPropagation();
    }

    /**
     * @desc Renders the list.
     */
    render() {
        return (
            <div className="col scroll">
                {this.props.radioStations.map(s => 
                    <div className="row highlight" key={s.ID} onClick={() => this.selectRadio(s)}>
                        <div className="col">
                            {s.Name}
                        </div>
                        <div className="col">
                            {s.Genre}
                        </div>
                        <div className="col-1">
                            <button className="iconButton" onClick={(e) => this.editRadio(s, e)}>
                                <i className="far fa-edit"></i>
                            </button>
                        </div>
                    </div>
                )}
                <Waypoint onEnter={() => this.props.radioStations.length > 0 && this.props.requestSongs(this.props.radioStations.length, 25)} bottomOffset="-100px" />
            </div>
        );
    }
}

export default withRouter(RadioList);