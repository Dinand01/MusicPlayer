import React, { useState } from 'react';
import { useSelector } from 'react-redux';
import ProgressCircle from "react-progress-circle";

const Copy = () => {
    const progress = useSelector(state => state.copyProgress);
    const [source, setSource] = useState("");
    const [destination, setDestination] = useState("");
    const [amount, setAmount] = useState(500);

    const selectFolder = (folder) => {
        MusicPlayer.selectFolder().then(res => {
            if (folder === "source") {
                setSource(res);
            } else {
                setDestination(res);
            }
        });
    };

    const copyFiles = () => {
        MusicPlayer.copySongs(source, destination, parseInt(amount));
    };

    return (
        <div className="copy-page row h-100">
            {progress &&
            <div className="copy-progress">
                <div>
                    <ProgressCircle
                        backgroundColor="#400080"
                        color="#699D35"
                        labelColor="#0e1314"
                        labelSize="26px"
                        size="150"
                        status={parseInt(progress)}
                        />
                </div>
            </div>}
            {!progress &&
            <div className="copy-files-form">
                <div className="flex-1x">
                    <h2>Copy random audio files</h2>
                </div>
                <div className="flex-2x">
                    <button onClick={() => selectFolder("source")} title="The folder to copy files from" className="primary-button">
                        {source ? "Change source" :  "Source"}
                    </button>
                    <p>{source}</p>
                </div>
                <div className="flex-2x">
                    <button onClick={() => selectFolder("destination")} title="The folder to copy files to" className="primary-button">
                        {destination ? "Change destination" : "Destination"}
                    </button>
                    <p>{destination}</p>
                </div>
                <div className="flex-1x">
                    <input type="number" value={amount} onChange={(e) => setAmount(e.target.value)} />
                </div>
                <div className="flex-1x">
                    <button disabled={!source || !destination || !amount} onClick={copyFiles} title="Copy the files" className="primary-button">
                        Copy
                    </button>
                </div>
            </div>}
        </div>
    );
};

export default Copy;