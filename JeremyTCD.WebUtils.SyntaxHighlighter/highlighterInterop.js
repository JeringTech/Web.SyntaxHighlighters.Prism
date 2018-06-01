var Prism = require('prismjs');
var PrismLanguageLoader = require('prismjs/components/index.js');

module.exports = function (callback, code, language) {
    // Node caches modules, so its fine if the same language is required more than once - https://nodejs.org/api/modules.html#modules_caching
    PrismLanguageLoader(language);

    var result = Prism.highlight(code, Prism.languages[language], language);

    callback(null /* errors */, result);
}