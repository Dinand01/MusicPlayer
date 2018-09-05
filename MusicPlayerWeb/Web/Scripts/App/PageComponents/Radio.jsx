import React from 'react';
import { RadioList } from '../Parts/Radio/RadioList.jsx';
import { parseJSON } from '../Helpers/Methods.jsx';

export default class Radio extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            searchText: "",
            stations: []
        };
    }

    componentWillMount() {
        this.getStations();
    }
    
    changeSearchText(text) {
        console.log(text)
        this.setState({
            searchText: text
        }, () => {
            this.getStations();
        });
    } 

    getStations() {
        console.log("Get radio stations: " +this.state.searchText);
        MusicPlayer.getRadioStations(this.state.searchText).then((json) => {
            let stations = parseJSON(json);
            if (stations && stations.length) {
                this.setState({
                    stations: stations
                });
            }
        });
    }

    render() {
        return(
            <div className="row h-100">
                <div className="col">
                    <div className="h-100 d-flex flex-column">
                        <div className="row justify-content-center h-30-px">
                            <div className="col">
                                <input type="text" className="w-100 ml-0" placeholder="Search" value={this.state.searchText} onChange={e => this.changeSearchText(e.target.value)} />
                            </div>
                            <div className="col-1">
                                <button className="iconButton h-100">
                                    <i className="fas fa-plus-square fa-2x"></i>
                                </button>
                            </div>
                        </div>
                        <div className="row justify-content-center flex-grow-1">
                            <RadioList radioStations={this.state.stations} />
                        </div>
                    </div>
                    
                </div>
            </div>
        )
    }
}