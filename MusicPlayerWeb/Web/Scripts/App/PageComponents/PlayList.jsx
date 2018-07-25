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
        this.state = {
            currentSlide: 1
        };
    }

    /**
     * @desc Decides when a render is required.
     */
    shouldComponentUpdate(nextprops, nextstate)  {
        if (this.state.currentSlide !== nextstate.currentSlide) {
            return true;
        }

        return false;
    }

    /**
     * @desc The slide index changed.
     * @param {number} e The new slide index. 
     */
    slideChanged(e) {
        console.log(e);
        this.setState({
            currentSlide: e 
        });
    }

    render() {
        return (
            <div className="row h-100 align-items-center">
                <div className={this.state.currentSlide === 0 ? "col h-100 sliderHeightMax" : "col"}>
                        <Slider 
                            afterChange={e => this.slideChanged(e)}
                            arrows={true} 
                            slidesToShow={1}
                            infinite={false}
                            initialSlide={1}> 
                                <div><SongList /></div>
                                <div><Song /></div>
                        </Slider>
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