import React from 'react';

/**
 * @class The song in a list.
 */
export default class SongListItem extends React.Component {

    /**
     * @description Play the song.
     */
    play() {
        MusicPlayer.play(JSON.stringify(this.props.song));
    }

    /**
     * @description Render the song.
     */
    render() {
        let duration = new Date(this.props.song.Duration * 1000);
        return (
            <div className="songlist-item" onClick={() => this.play()}>
                <div style={{width: '50%'}}>{this.props.song.Title}</div>
                <div style={{width: '20%'}}>{this.props.song.Band}</div>
                <div style={{width: '20%'}}>{this.props.song.Gengre}</div>
                <div style={{width: '10%'}}>{duration.getMinutes() + ":" + duration.getSeconds()}</div>
            </div>
        );
    }
}