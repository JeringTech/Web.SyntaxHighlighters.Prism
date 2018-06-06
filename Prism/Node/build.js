const fs = require('fs');
const path = require('path');
const webpack = require('webpack');
const webpackConfig = require('./webpack.config.js');

// TODO run webpack in production mode when doing a release build so that scripts are minified

let rebuild = true;
let files = ['interop.js', 'package.json', 'webpack.config.js']; // Build when these files change. Note: could use fs.readdir to recursively find files instead

let currentLastModifiedTime = 0;

for (let file of files) {
    let stats = fs.statSync(file);
    let fileLastModifiedTime = new Date(stats.mtime.toISOString()).getTime();

    // We want the most recent file modification time
    if (fileLastModifiedTime > currentLastModifiedTime) {
        currentLastModifiedTime = fileLastModifiedTime;
    }
}

if (fs.existsSync('./lastModifiedTime')) {
    // Get last modified time
    let lastModifiedTime = parseInt(fs.readFileSync('./lastModifiedTime', 'utf8'));

    // If lastModifiedTime is NaN, we cannot proceed since the file has been currupted - a rebuild is required
    if (!isNaN(lastModifiedTime)) {
        // If last modified date is the same as currentLastModifiedTime, no changes have been made, so no need to rebuild.
        if (lastModifiedTime == currentLastModifiedTime) {
            console.log('bundle.js up to date, not running webpack.');
            rebuild = false;
        }
    }
}

if (rebuild) {
    //  - if any have changed
    //      - if debug build, run webpack in development mode
    //      - if release build, run webpack in production mode
    console.log('Running webpack...');

    webpack(webpackConfig, (err, stats) => {
        if (err) {
            console.log(err);
        } else {
            console.log('Webpack complete.');
            fs.writeFileSync('./lastModifiedTime', currentLastModifiedTime, { encoding: 'utf8' });
        }
    });
}