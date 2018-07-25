import React from 'react';
import { connect } from 'react-redux';
import Slider from 'rc-slider';

/**
 * @desc Displays the current song position.
 */
class SongPosition extends React.Component {
    render() {
        return (
            <div className="song-position col" >
                <Slider 
                    disabled={this.props.disabled}
                    value={this.props.currentSong.Position} 
                    max={this.props.currentSong.Duration}
                    onChange={(val) => this.props.moveToTime(val)}
                    title={"Change song position"} />
            </div>
        );
    }
}

function mapStateToProps(state) {
    return {
        currentSong: state.currentSong
    };
}

export default connect(mapStateToProps)(SongPosition);