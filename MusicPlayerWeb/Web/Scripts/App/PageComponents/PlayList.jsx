import React from 'react';
import { connect } from 'react-redux';
import Slider from 'react-slick';
import Song from '../Parts/SongInfo/Song.jsx';
import SongList from '../Parts/SongList/SongList.jsx';

/**
 * @class Contains the playist and current song.
 */
class PlayList extends React.Component {

    render() {
        return (
            <div className="playlist">
                <div>
                    <div className="playlist-pages">
                        <Slider 
                            arrows={true} 
                            slidesToShow={1}> 
                                <div><Song serverInfo={this.props.serverInfo} currentSong={this.props.currentSong} /></div>
                                <div><SongList receiveMode={this.props.serverInfo && !this.props.serverInfo.IsHost} currentSong={this.props.currentSong} /></div>
                        </Slider>
                    </div>
                </div>
            </div>
        )
    }
}

function mapStateToProps(state) {
  return { 
      serverInfo: state.serverInfo,
      currentSong: state.currentSong
    };
}

export default connect(mapStateToProps)(PlayList);