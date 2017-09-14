import React from 'react';

class Video extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            videoUrl: "https://www.youtube.com/watch?v=ZsYwEV_ge4Y",
            modifiedUrl: null
        };
    }

    componentWillMount() {
        this.generateUrlFromVid();
    }

    /**
     * @desc Generates an youtube player url from the video url.
     */
    generateUrlFromVid() {
        if (this.state.videoUrl) {
            let parts = this.state.videoUrl.split("?v=");
            if (parts.length < 2) {
                parts = this.state.videoUrl.split("/_");
            }

            this.setState({modifiedUrl: "https://www.youtube.com/embed/" + parts[parts.length - 1] + "?autoplay=1"});
        } else {
            this.setState({modifiedUrl: null});
        }
    }

    /**
     * @desc Change the video url.
     * @param {string} url The new url.
     */
    changeUrl(url) {
        this.setState({videoUrl: url}, () => {
            this.generateUrlFromVid();
        });
    }

    render() {
        return (
            <div className="video">
                <header>
                    <input type="text" value={this.state.videoUrl} disabled={false} onChange={(e) => this.changeUrl(e.target.value)} />
                </header>
                <section>
                    <iframe src={this.state.modifiedUrl} frameBorder="0" allowFullScreen="allowfullScreen"></iframe>
                </section>
            </div>
        );
    }
}

export default Video;