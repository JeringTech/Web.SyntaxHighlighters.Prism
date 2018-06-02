var Prism = require('prismjs');
var PrismLanguageLoader = require('prismjs/components/index.js');
var components = require('prismjs/components.js');

module.exports = {

    highlight: function (callback, code, language, options) {
        // Node caches modules, so its fine if the same language is required more than once - https://nodejs.org/api/modules.html#modules_caching
        PrismLanguageLoader(language);

        var result = Prism.highlight(code, Prism.languages[language], language);

        callback(null /* errors */, result);
    },

    getAliases: function (callback) {
        var result = [];

        for (var language in Object.keys(components.languages)) {
            result.push(language);

            var aliases = components.languages[language].alias;
            if (!aliases) {
                continue;
            }
            if (typeof aliases === 'string') {
                result.push(aliases);
            } else if (Array.isArray(aliases)) {
                for (var alias in aliasese) {
                    result.push(alias);
                }
            }
        }

        return result;
    }
}