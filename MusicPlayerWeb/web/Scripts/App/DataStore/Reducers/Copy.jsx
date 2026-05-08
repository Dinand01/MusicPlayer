import { SongActions } from '../StoreActions.jsx';

export const copyProgress = (state = {}, action) => {  
  switch (action.type) {
    case "ChangeProgress":
      return action.progress;
    default:
      return state;
  }
};