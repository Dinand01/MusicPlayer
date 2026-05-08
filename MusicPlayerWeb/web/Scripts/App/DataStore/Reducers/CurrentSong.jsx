import { SongActions } from '../StoreActions.jsx';

export const currentSong = (state = {}, action) => {  
  switch (action.type) {
    case "SetCurrentSong":
      return action.song;
    default:
      return state;
  }
};