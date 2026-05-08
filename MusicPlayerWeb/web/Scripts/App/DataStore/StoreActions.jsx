
export const SongActions = {
    setCurrentSong: song => {
        return {  
          type: "SetCurrentSong",
          song: song,
        }
    },
    setServerinfo: serverInfo => {
        return {
            type: "SetServerInfo",
            serverInfo: serverInfo
        };
    },
    changeCopyProgress: progress => {
        return {
            type: "ChangeProgress",
            progress: progress
        }
    }
}
