import React from 'react';
import { parseJSON } from '../Helpers/Methods.jsx';
import TextField from '../Parts/Form/TextField.jsx';

/**
 * @class Class for editing a radio station.
 */
export default class EditRadio extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            radio: {
                Name: "",
                Genre: "",
                Url: "",
                ImageUrl: "",
                Priority: 200
            },
            validation: {},
            isValid: false
        };
    }

    /**
     * @desc The component mounted, send intial validation state.
     */
    componentDidMount() {
        if (this.props.match.params.id > 0) {
            MusicPlayer.getRadioStation(parseInt(this.props.match.params.id)).then(r => {
                let radio = parseJSON(r);
                if (radio) {
                    this.setState({
                        radio: radio
                    }); 
                }
            })
        }
    }

    /**
     * @desc A form input changed.
     * @param {object} event The javascript event. 
     * @param {boolean} valid Indicates whether the value is valid. 
     */
    handleInputChange(event, valid) {
        this.state.radio[event.target.name] = event.target.value;
        if (valid !== null) {
            this.state.validation[event.target.name] = valid;
        }

        this.setState({
          radio: this.state.radio,
          validation: this.state.validation,
          isValid: this.validateForm(this.state.validation)
        });
    }

    /**
     * @desc Save the form.
     * @param {object} event The javascript event. 
     */
    handleSubmit(event) {
        console.log(this.state.radio);
        if (this.state.isValid) {
            MusicPlayer.updateOrCreateRadioStation(JSON.stringify(this.state.radio)).then(r => {
                this.props.history.push("/radio");
            });
        }

        event.preventDefault();
    }

    /**
     * @desc Checks whether all form elemenets are valid.
     * @param {object} validation The validation state object. 
     */
    validateForm(validation) {
        let isValid = true;
        Object.keys(validation).forEach(k => {
            if (validation[k] === false) {
                isValid = false;
            }
        });

        return isValid;
    }

    /**
     * @desc Renders the form.
     */
    render() {
        return (
            <form onSubmit={e => this.handleSubmit(e)} className="row h-100">
                <div className="col scroll">
                    <div className="row">
                        <div className="col">
                            <h2>{this.state.radio.ID > 0 ? "Edit " + this.state.radio.Name : "Add new internet radio"}</h2>
                        </div>
                    </div>
                    <TextField label="Name" value={this.state.radio.Name} onChange={(e, valid) => this.handleInputChange(e, valid)} required={true} help="The name of the radio station" />
                    <TextField label="Stream Url" name="Url" value={this.state.radio.Url} onChange={(e, valid) => this.handleInputChange(e, valid)} required={true} help="The internet radio stream URL" />
                    <TextField label="Genre" value={this.state.radio.Genre} onChange={e => this.handleInputChange(e)} help="The genre of music" />
                    <TextField label="Image url" name="ImageUrl" value={this.state.radio.ImageUrl} onChange={e => this.handleInputChange(e)} help="The url of an image representing the radio station" />
                    <div className="row">
                        <div className="col">
                            <button type="submit" disabled={!this.state.isValid}>Save</button>
                        </div>
                    </div>
                </div>
            </form>
        )
    }
}