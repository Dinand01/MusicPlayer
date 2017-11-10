import React from 'react';
import { connect } from 'react-redux';
import Slider from 'react-slick';
import Song from '../Parts/SongInfo/Song.jsx';
import SongList from '../Parts/SongList/SongList.jsx';

/**
 * @class Contains the playist and current song.
 */
class PlayList extends React.Component {
    constructor(props) {
        super(props);
    }

    /**
     * @desc This component is static.
     */
    shouldComponentUpdate(nextprops, nextstate)  {
        return false;
    }

    render() {
        return (
            <div className="playlist">
                <div>
                    <div className="playlist-pages">
                        <Slider 
                            arrows={true} 
                            slidesToShow={1}
                            infinite={false}
                            initialSlide={1}> 
                                <div><SongList /></div>
                                <div><Song /></div>
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

export default PlayList;