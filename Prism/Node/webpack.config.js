const Path = require('path');

module.exports = {
    mode: 'development',
    target: 'node',
    resolve: {
        modules: ["node_modules"],
        extensions: [ '.js' ]
    },
    entry: './interop.js',
    output: {
        libraryTarget: 'commonjs',
        path: Path.join(__dirname, 'bin'),
        filename: 'JeremyTCD.WebUtils.SyntaxHighlighters.Prism.bundle.js'
    }
};
