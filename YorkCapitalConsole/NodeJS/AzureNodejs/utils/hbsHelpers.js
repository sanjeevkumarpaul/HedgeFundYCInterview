var handlebars = require("handlebars");

function hbsHelpers(hbs) {
    return hbs.create({
        helpers: { // This was missing
            boldit: function (text) {
                return new handlebars.SafeString("<strong>" + text + "</strong>");
            }            
        }

    });
}

module.exports = hbsHelpers;
