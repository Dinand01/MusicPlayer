import { SongActions } from '../StoreActions.jsx';

export const serverInfo = (state = {}, action) => {  
  switch (action.type) {
    case "SetServerInfo":
      return action.serverInfo;
    default:
      return state;
  }
};