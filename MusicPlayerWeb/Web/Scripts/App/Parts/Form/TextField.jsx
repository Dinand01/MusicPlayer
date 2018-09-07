import React from 'react';

/**
 * @class Text field supporting simple validation.
 */
export default class TextField extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isDirty: null
        }
    }

    /**
     * @desc The component mounted, send initial validation state.
     */
    componentDidMount() {
        const event = {
            target: {
                name: this.props.name ? this.props.name : this.props.label,
                value: this.props.value
            }
        };

        this.props.onChange(event, this.validate(this.props, this.props.value));
    }

    /**
     * @desc the properties changed.
     * @param {object} nextprops The new properties. 
     */
    componentWillReceiveProps(nextprops) {
        this.validate(nextprops, nextprops.value);
    }

    /**
     * @desc validates the value.
     * @param {object} props The properties to use. 
     * @param {*} value The value to use.
     */
    validate(props, value) {
        if (props.required === undefined) {
            return null;
        }

        if (props.required && !value && this.state.isDirty !== true) {
            this.setState({ isDirty: true });
            return false;
        } else if ((!props.required || value) && this.state.isDirty !== false) {
            this.setState({ isDirty: false });
            return true;
        }

        return !this.state.isDirty;
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
                        className={this.state.isDirty ? "invalid" : (this.state.isDirty == null ? "" : "valid")} 
                        name={this.props.name ? this.props.name : this.props.label} value={this.props.value} 
                        onChange={e => this.props.onChange(e, this.validate(this.props, e.target.value))} />
                </div>
            </div>
        );
    }
}