const fs = require("fs");
const path = require("path");

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(function(file) {
        file = dir + '/' + file;
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) { 
            results = results.concat(walk(file));
        } else if (file.endsWith(".js")) { 
            results.push(file);
        }
    });
    return results;
}

const files = walk("VivuCarClient/WebClient/wwwroot/js");

files.forEach(f => {
    let content = fs.readFileSync(f, "utf8");
    let modified = content.replace(/(?<!String\()([a-zA-Z0-9_\?\.]+)\.status\s*===\s*([\x22\x27][a-zA-Z_]+[\x22\x27])/g, "String($1.status).toLowerCase() === $2");
    if (content !== modified) {
        fs.writeFileSync(f, modified);
        console.log("Updated " + f);
    }
});
