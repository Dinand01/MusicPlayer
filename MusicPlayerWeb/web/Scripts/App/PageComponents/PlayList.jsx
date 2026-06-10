import React, { useState } from 'react';
import Slider from 'react-slick';
import Song from '../Parts/SongInfo/Song.jsx';
import SongList from '../Parts/SongList/SongList.jsx';

const PlayList = () => {
    const [currentSlide, setCurrentSlide] = useState(1);

    const slideChanged = (e) => {
        console.log(e);
        setCurrentSlide(e);
    };

    return (
        <div className="row h-100 align-items-center scroll">
            <div className={currentSlide === 0 ? "col h-100 sliderHeightMax" : "col"}>
                <Slider 
                    afterChange={e => slideChanged(e)}
                    arrows={true} 
                    slidesToShow={1}
                    infinite={false}
                    initialSlide={1}> 
                    <div><SongList /></div>
                    <div><Song /></div>
                </Slider>
            </div>
        </div>
    );
};

export default PlayList;