const Path = require('path');

module.exports = {
    mode: 'production', // Automatically enables minification
    target: 'node',
    entry: './interop.js',
    output: {
        libraryTarget: 'commonjs',
        path: Path.join(__dirname, 'bin'),
        filename: 'JeremyTCD.WebUtils.SyntaxHighlighters.Prism.bundle.js'
    }
};
