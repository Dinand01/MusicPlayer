import React from 'react';
import Waypoint from 'react-waypoint';

/**
 * @class Displays the radio station list.
 */
export class RadioList extends React.Component {
    constructor(props) {
        super(props);
    }

    /**
     * @desc Requests audio playback from the station. 
     * @param {object} station The radio station object.
     */
    selectRadio(station) {
        MusicPlayer.playFromURL(station.Url);
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
                    </div>
                )}
                <Waypoint onEnter={() => this.props.radioStations.length > 0 && this.props.requestSongs(this.props.radioStations.length, 25)} bottomOffset="-100px" />
            </div>
        );
    }
}