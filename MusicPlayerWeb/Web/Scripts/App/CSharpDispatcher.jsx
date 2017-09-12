import { store } from './DataStore/Store.jsx';
import { SongActions } from './DataStore/StoreActions.jsx';

class CSharpDispatcher {

    /**
     * @description Helper function to dispatch the song to the currentSong state.
     * @param {string} jsonSong The json string of the song. 
     */
    dispatchSetCurrentSong(jsonSong) {
        store.dispatch(SongActions.setCurrentSong(JSON.parse(jsonSong)));
    }

    /**
     * @description Helper function to dispatch server info.
     * @param {string} jsonInfo 
     */
    dispatchSetServerInfo(jsonInfo) {
        console.log("New server info " + jsonInfo);
        store.dispatch(SongActions.setServerinfo(JSON.parse(jsonInfo)));
    }

    /**
     * @description Set the copy progress.
     * @param {number} progress The current progress. 
     */
    dispatchSetCopyProgress(progress) {
        store.dispatch(SongActions.changeCopyProgress(progress));
    }
}

export const csharpDispatcher = new CSharpDispatcher();