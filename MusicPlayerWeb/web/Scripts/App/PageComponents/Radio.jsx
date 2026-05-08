import React from 'react';
import RadioList from '../Parts/Radio/RadioList.jsx';
import { parseJSON } from '../Helpers/Methods.jsx';

/**
 * @class The internet radio selection page.
 */
export default class Radio extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: "",
            allStations: [],
            stations: [],
            index: 0
        };
    }

    /**
     * @desc The component will mount.
     */
    componentWillMount() {
        this.getStations();
    }

    /**
     * @desc refresh data when the child component unmounts.
     * @param {*} nextprops The xext properties.
     */
    componentWillReceiveProps(nextprops) {
        if (!this.props.match.isExact && nextprops.match.isExact) {
            this.getStations();
        }
    }

    /**
     * @desc The search text changed.
     * @param {string} text The search text. 
     */
    changeSearchText(text) {
        this.setState({
            searchText: text,
            stations: []
        }, () => {
            this.getStations();
        });
    } 

    /**
     * @desc Gets radio stations from the backend.
     */
    getStations() {
        MusicPlayer.getRadioStations(this.state.searchText).then((json) => {
            let stations = parseJSON(json);
            if (stations && stations.length) {
                this.setState({
                    allStations: stations,
                    index: 0
                }, () => this.requestSongs(this.state.index, 25));
            }
        });
    }

    /**
     * @desc Request more stations.
     * @param {number} skip The amount of stations to skip. 
     * @param {*} amount The amount of stations to take.
     */
    requestSongs(skip, amount) {
        if (this.state.allStations && this.state.allStations.length) {
            skip = skip > 0 ? skip : 0;
            this.setState({
                stations: this.state.stations.concat(this.state.allStations.slice(skip, skip + amount))
            });
        }
    }

    /**
     * @desc Renders the page.
     */
    render() {
        if (!this.props.match.isExact) {
            return null;
        }

        return(
            <div className="row h-100">
                <div className="col">
                    <div className="h-100 d-flex flex-column">
                        <div className="row justify-content-center h-35-px">
                            <div className="col">
                                <input type="text" className="w-100 ml-0" placeholder="Search" value={this.state.searchText} onChange={e => this.changeSearchText(e.target.value)} />
                            </div>
                            <div className="col-1">
                                <button className="iconButton h-100" onClick={() => this.props.history.push("/radio/0")}>
                                    <i className="fas fa-plus-square fa-2x"></i>
                                </button>
                            </div>
                        </div>
                        <div className="row justify-content-center flex-grow-1 pt-2">
                            <RadioList radioStations={this.state.stations} requestSongs={(skip, amount) => this.requestSongs(skip, amount) } />
                        </div>
                    </div>
                </div>
            </div> 
        )
    }
}