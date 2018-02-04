var handlebars = require('handlebars');

module.exports = {

        boldit: function (text) {
            return new handlebars.SafeString("<strong>" + text + "</strong>");
        }
        //MORE HELPERS TO PROCEED
    }

module.exports = hbsHelpers;