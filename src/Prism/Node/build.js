const fs = require('fs');
const path = require('path');
const webpack = require('webpack');
const webpackConfig = require('./webpack.config.js');
const argv = require('yargs').argv;

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

if (fs.existsSync('./lastBuildData')) {
    // Get last modified time
    let lastBuildData = JSON.parse(fs.readFileSync('./lastBuildData', 'utf8'));
    let lastMode = lastBuildData.mode;
    let lastModifiedTime = lastBuildData.modifiedTime;

    if (lastMode != argv.mode) {
        console.log(`Build mode has changed, rebuilding bundle.js. Last build mode: ${lastMode}, current build mode: ${argv.mode}.`);
    } else if (!Number.isInteger(lastModifiedTime)) {
        console.log(`Last modified time "${lastModifiedTime}" is invalid, rebuilding bundle.js.`);
    } else if (lastModifiedTime == currentLastModifiedTime) {
        // If last modified date is the same as currentLastModifiedTime, no changes have been made, so no need to rebuild.
        console.log('bundle.js is up to date.');
        rebuild = false;
    } else {
        console.log('File(s) changed, rebuilding bundle.js.');
    }
} else {
    console.log('lastBuildData unavailable, rebuilding bundle.js.');
}

if (rebuild) {
    console.log('Rebuilding...');

    webpack(webpackConfig({ mode: argv.mode }), (err, stats) => {
        if (err) {
            console.log(err);
        } else {
            console.log('Rebuild complete.');
            fs.writeFileSync('./lastBuildData', JSON.stringify({ modifiedTime: currentLastModifiedTime, mode: argv.mode }), { encoding: 'utf8' });
        }
    });
}