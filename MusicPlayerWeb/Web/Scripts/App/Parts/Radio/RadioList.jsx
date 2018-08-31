import React from 'react';

export class RadioList extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            radioStations: []
        };
    }

    componentWillReceiveProps(nextprops) {
        console.log(nextprops.radioStations);
        if (nextprops.radioStations && nextprops.radioStations != this.props.radioStations) {
            console.log(nextprops.radioStations);
            this.setState({
                radioStations: nextprops.radioStations.slice(0, nextprops.radioStations.length > 25 ? 25 : nextprops.radioStations.length)
            });
        }
    }

    selectRadio(station) {
        console.log(station);
        // TODO
    }

    render() {
        return (

            <div className="col">
                {this.state.radioStations.map(s => 
                    <div className="row" key={s.ID} onClick={() => this.selectRadio(s)}>
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