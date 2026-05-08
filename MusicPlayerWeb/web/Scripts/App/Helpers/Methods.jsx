/**
 * @description Parses a string to JSON.
 * @param {string} str The JSON string.
 */
export function parseJSON(str) {
    let obj = null;
    str.replace(/\\n/g, "\\n")  
               .replace(/\\'/g, "\\'")
               .replace(/\\"/g, '\\"')
               .replace(/\\&/g, "\\&")
               .replace(/\\r/g, "\\r")
               .replace(/\\t/g, "\\t")
               .replace(/\\b/g, "\\b")
               .replace(/\\f/g, "\\f");
    // remove non-printable and other non-valid JSON chars
    str = str.replace(/[\u0000-\u0019]+/g,""); 
    try {
        obj = JSON.parse(str);
    } catch (e) {
        console.log(e);
    }

    return obj;
}