import React from 'react';

export class RadioList extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            radioStations: []
        };
    }

    componentWillReceiveProps(nextprops) {
        if (nextprops.radioStations 
            && nextprops.radioStations.length != this.props.radioStations.length
            && JSON.stringify(nextprops.radioStations) != JSON.stringify(this.props.radioStations)) {
                this.setState({
                    radioStations: nextprops.radioStations.slice(0, nextprops.radioStations.length > 25 ? 25 : nextprops.radioStations.length)
                });
        }
    }

    selectRadio(station) {
        console.log(station);
        MusicPlayer.playFromURL(station.Url);
        // TODO
    }

    render() {
        return (

            <div className="col scroll">
                {this.state.radioStations.map(s => 
                    <div className="row highlight" key={s.ID} onClick={() => this.selectRadio(s)}>
                        <div className="col">
                            {s.Name}
                        </div>
                        <div className="col">
                            {s.Genre}
                        </div>
                    </div>
                )}
            </div>

        );
    }
}