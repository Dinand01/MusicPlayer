import React from 'react';
import Slider from 'react-slick';
import { withRouter } from 'react-router-dom';

/**
 * @class The home page component containing the main menu.
 */
class Home extends React.Component {

    /**
     * @description Show a folder browser to open music.
     */
    openFolder() {
        MusicPlayer.openFolder().then((res) => {
            if (res) {
                this.props.history.push("/playlist");
            }
        });
    }

    /**
     * @description Show a file browser to open music.
     */
    openFiles() {
        MusicPlayer.openFiles().then((res) => {
            if (res) {
                this.props.history.push("/playlist");
            }
        });
    }

    /**
     * @description Go to the server page.
     */
    gotoServer() {
        this.props.history.push("/server");
    }

    /**
     * @description Go to the client page.
     */
    gotoClient() {
        this.props.history.push("/client");
    }

    /**
     * @description Go to the copy page.
     */
    gotoCopy() {
        this.props.history.push("/copy");
    }

    /**
     * @description Render the component.
     */
    render() {
        if (!this.props.match.isExact) {
            return null;
        } 

        return (
        <div className="menu">
            <div>
                <div className="menu-carousel">
                    <Slider 
                        centerMode={true} 
                        arrows={true} 
                        focusOnSelect={true}
                        autoPlay={true}
                        responsive={[{ breakpoint: 768, settings: { slidesToShow: 3 } },
                                    { breakpoint: 1024, settings: { slidesToShow: 3 }},
                                    { breakpoint: 10000, settings: { slidesToShow: 3 }}]} > 
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.openFolder()} title="Open a folder (Loads all the audio in the folder)">
                                    <i className="Menu-Icon fa fa-folder-open fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.gotoServer()} title="Host a audio server">
                                    <i className="Menu-Icon fa fa-rss fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.openFiles()} title="Open multiple files">
                                    <i className="Menu-Icon fa fa-file-audio-o fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.gotoCopy()} title={"Copy random files"}>
                                    <i className="Menu-Icon fa fa-files-o fa-5x"></i>
                                </div> 
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.gotoClient()} title="Connect to a server">
                                    <i className="Menu-Icon fa fa-podcast fa-5x"></i>
                                </div>
                            </div>
                    </Slider>
                </div>
            </div>
        </div>);
    }
}

export default withRouter(Home);