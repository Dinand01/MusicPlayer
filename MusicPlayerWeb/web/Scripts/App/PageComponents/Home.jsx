import React from 'react';
import Slider from 'react-slick';
import Loader from 'react-loader';
import { withRouter } from 'react-router-dom';
import { connect } from 'react-redux';

/**
 * @class The home page component containing the main menu.
 */
class Home extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            isLoaded: true
        };
    }

    /**
     * @description Show a folder browser to open music.
     */
    openFolder() {
        this.setState({ isLoaded: false }, () => {
            setTimeout(() => {
                MusicPlayer.openFolder().then((res) => {
                    this.setState({ isLoaded: true }, () => {
                        if (res) {
                            this.props.history.push("/playlist");
                        }
                    });
                 });
            }, 50);
        });
    }

    /**
     * @description Show a file browser to open music.
     */
    openFiles() {
        MusicPlayer.openFiles().then((res) => { });
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
     * @desc Goto a page.
     * @param {string} url The relative url to go to. 
     */
    goto(url) {
        this.props.history.push(url);
    }

    /**
     * @description Render the component.
     */
    render() {
        if (!this.props.match.isExact) {
            return null;
        } 

        return (
        <div className="row h-100 align-items-center scroll">
            <div className="col">
                {!this.state.isLoaded && <Loader loaded={this.state.isLoaded} options={{color: "#FFF"}} />}
                {this.state.isLoaded && <div className="menu-carousel">
                    <Slider 
                        centerMode={true} 
                        dots={true}
                        slidesToShow={3}
                        slidesPerRow={1}
                        rows={2}
                        responsive={[{ breakpoint: 400, settings: { slidesToShow: 1, rows: 2 } },
                                    { breakpoint: 550, settings: { slidesToShow: 2, rows: 2 } },
                                    { breakpoint: 768, settings: { slidesToShow: 3, rows: 2 } },
                                    { breakpoint: 1024, settings: { slidesToShow: 3, rows: 2}}]} > 
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.openFolder()} title="Open a folder (Loads all the audio in the folder)">
                                    <i className="Menu-Icon fas fa-folder-open fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.openFiles()} title="Open multiple files">
                                    <i className="Menu-Icon far fa-file-audio fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.gotoServer()} title="Host a audio server">
                                    <i className="Menu-Icon fas fa-broadcast-tower fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.goto('/radio')} title="Connect to internet radio">
                                    <i className="Menu-Icon fab fa-soundcloud fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.gotoCopy()} title={"Copy random files"}>
                                    <i className="Menu-Icon far fa-copy fa-5x"></i>
                                </div> 
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.goto("/video")} title={"Play youtube videos"}>
                                    <i className="Menu-Icon fab fa-youtube fa-5x"></i>
                                </div> 
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.gotoClient()} title="Connect to a server">
                                    <i className="Menu-Icon fas fa-signal fa-5x"></i>
                                </div>
                            </div>
                            <div className="menu-carousel-slide">
                                <div onClick={() => this.goto("/video")} title={"Play youtube videos"}>
                                    <i className="Menu-Icon fab fa-youtube fa-5x"></i>
                                </div> 
                            </div>
                    </Slider>
                </div>}
            </div>
        </div>);
    }
}

export default withRouter(Home);