const fs = require('fs');
const path = require('path');
const webpack = require('webpack');
const webpackConfig = require('./webpack.config.js');
const argv = require('yargs').argv;

let build = true;
let files = ['interop.js', 'package.json', 'webpack.config.js']; // Build when these files change. Note: could use fs.readdir to recursively find files instead
let bundleName = `${argv.assemblyName}.bundle.js`;
let currentLastModifiedTime = 0;

for (let file of files) {
    let stats = fs.statSync(file);
    let fileLastModifiedTime = new Date(stats.mtime.toISOString()).getTime();

    // We want the most recent file modification time
    if (fileLastModifiedTime > currentLastModifiedTime) {
        currentLastModifiedTime = fileLastModifiedTime;
    }
}

if (!fs.existsSync(`./bin/${bundleName}`)) {
    console.log(`${bundleName} does not exist, building bundle.`);
} else if (fs.existsSync('./lastBuildData')) {
    // Get last modified time
    let lastBuildData = JSON.parse(fs.readFileSync('./lastBuildData', 'utf8'));
    let lastMode = lastBuildData.mode;
    let lastModifiedTime = lastBuildData.modifiedTime;

    if (lastMode !== argv.mode) {
        console.log(`Build mode has changed, rebuilding ${bundleName}. Last build mode: ${lastMode}, current build mode: ${argv.mode}.`);
    } else if (!Number.isInteger(lastModifiedTime)) {
        console.log(`Last modified time "${lastModifiedTime}" is invalid, rebuilding ${bundleName}.`);
    } else if (lastModifiedTime !== currentLastModifiedTime) {
        console.log(`File(s) changed, rebuilding ${bundleName}.`);
    } else {
        // If last modified date is the same as currentLastModifiedTime, no changes have been made, so no need to rebuild.
        console.log(`${bundleName} is up to date.`);
        build = false;
    }
} else {
    console.log(`lastBuildData unavailable, rebuilding ${bundleName}.`);
}

if (build) {
    webpack(webpackConfig({ mode: argv.mode, bundleName: bundleName }), (err, stats) => {
        if (err) {
            console.log(err);
        } else {
            console.log('Rebuild complete.');
            fs.writeFileSync('./lastBuildData', JSON.stringify({ modifiedTime: currentLastModifiedTime, mode: argv.mode }), { encoding: 'utf8' });
        }
    });
}