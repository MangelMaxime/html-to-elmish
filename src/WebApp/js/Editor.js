import React from 'react';
import MonacoEditor from 'react-monaco-editor';
import PropTypes from 'prop-types';

class Editor extends React.Component {
    constructor(props) {
        super(props);
    }

    editorDidMount = (editor, monaco) => {
        this.props.editorDidMount();
    }

    onChange = (newValue, e) => {
        this.props.onChange(newValue);
    }

    render() {
        const options = {
            selectOnLineNumbers: true,
            lineNumbers: false,
            readOnly: this.props.isReadOnly,
            minimap: {
                enabled: false
            }
        };
        const requireConfig = {
            url:  "/libs/requirejs/require.js",
            paths: {
                vs: "/libs/vs"
            }
        };
        return (
            <MonacoEditor
                language={this.props.language}
                value={this.props.value}
                options={options}
                onChange={this.onChange}
                editorDidMount={this.editorDidMount}
                requireConfig={requireConfig}
            />
        );
    }
}

function noop() { }

Editor.propTypes = {
    onChange: PropTypes.func,
    value: PropTypes.string,
    language: PropTypes.string,
    isReadOnly: PropTypes.bool,
    editorDidMount: PropTypes.func
};

Editor.defaultProps = {
    onChange: noop,
    value: "",
    language: "html",
    isReadOnly: false,
    editorDidMount: noop
};

export default Editor;
