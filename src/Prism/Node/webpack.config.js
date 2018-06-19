const Path = require('path');

module.exports = env => {
    let mode = env.mode === 'release' ? 'production' : 'development'; // Default to development, production mode minifies scripts
    console.log(`Webpack mode: ${mode}.`);

    return {
        mode: mode,
        target: 'node',
        entry: './interop.js',
        output: {
            libraryTarget: 'commonjs2',
            path: Path.join(__dirname, 'bin'),
            filename: 'JeremyTCD.WebUtils.SyntaxHighlighters.Prism.bundle.js'
        }
    };
};
