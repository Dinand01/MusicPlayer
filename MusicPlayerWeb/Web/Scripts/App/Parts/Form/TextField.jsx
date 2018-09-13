import React from 'react';

/**
 * @class Text field supporting simple validation.
 */
export default class TextField extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            valid: null
        }
    }

    /**
     * @desc The component mounted, send initial validation state.
     */
    componentDidMount() {
        const event = this.createEvent(this.props);
        this.props.onChange(event, this.validate(this.props, this.props.value));
    }

    /**
     * @desc the properties changed.
     * @param {object} nextprops The new properties. 
     */
    componentWillReceiveProps(nextprops) {
        var valid = this.validate(nextprops, nextprops.value);
        if (valid !== this.state.valid && valid !== null) {
            this.props.onChange(this.createEvent(nextprops), valid);
        }
    }

    createEvent(props) {
        return event = {
            target: {
                name: props.name ? props.name : props.label,
                value: props.value
            }
        };
    }

    /**
     * @desc validates the value.
     * @param {object} props The properties to use. 
     * @param {*} value The value to use.
     */
    validate(props, value) {
        var valid = null;
        if (props.required === undefined) {
            return null;
        }

        if (props.required && !value) {
            valid = false;
        } else if (!props.required || value) {
            valid = true;
        }

        if (valid !== this.state.valid) {
            this.setState({ valid: valid });
        }

        return valid;
    }
    
    /**
     * @desc Renders the text field.
     */
    render() {
        return (
            <div className="row form-group" title={this.props.help}>
                <div className="col-3">
                    <span className="align-middle">{this.props.label}: </span>
                </div>
                <div className="col-9">
                    <input type="text" 
                        className={this.state.valid ? "valid" : (this.state.valid == null ? "" : "invalid")} 
                        name={this.props.name ? this.props.name : this.props.label} value={this.props.value} 
                        onChange={e => this.props.onChange(e, this.validate(this.props, e.target.value))} />
                </div>
            </div>
        );
    }
}