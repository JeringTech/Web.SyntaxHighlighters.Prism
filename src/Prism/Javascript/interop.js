var Prism = require('prismjs');
var components = require('prismjs/components.js');

// Get all language names for use in getAliases
var languageNames = Object.keys(components.languages).filter(languageName => languageName !== 'meta');
loadLanguages(languageNames);

module.exports = {
    highlight: function (callback, code, languageAlias) {
        var result = Prism.highlight(code, Prism.languages[languageAlias], languageAlias);

        callback(null /* errors */, result);
    },
    getAliases: function (callback) {
        var result = [];

        for (var languageName of languageNames) {
            result.push(languageName);

            var aliases = components.languages[languageName].alias;
            if (!aliases) {
                continue;
            }
            if (typeof aliases === 'string') {
                result.push(aliases);
            } else if (Array.isArray(aliases)) {
                for (var alias of aliases) {
                    result.push(alias);
                }
            }
        }

        callback(null /* errors */, result);
    }
};

// prismjs/compontents/index.js loadLanguages broke for webpack in 1.15.0 (https://github.com/PrismJS/prism/issues/1486). Since we're loading all 
// languages, use this method to a) get webpack to include all languages, b) load each language at runtime.
function loadLanguages(names) {
    if (!Array.isArray(names)) {
        names = [names];
    }

    names.forEach(languageName => {
        if (components.languages[languageName] && components.languages[languageName].require) {
            loadLanguages(components.languages[languageName].require);
        }
        require('prismjs/components/prism-' + languageName);
    });
}