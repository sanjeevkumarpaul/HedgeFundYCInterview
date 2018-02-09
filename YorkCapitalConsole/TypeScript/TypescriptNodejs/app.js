"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var debug = require("debug");
var express = require("express");
var path = require("path");
//Handlebars View Engine
var exphbs = require("express-handlebars");
var handlebars = require("handlebars");
var index_1 = require("./routes/index");
var user_1 = require("./routes/user");
var app = express();
// view engine setup
app.set('views', path.join(__dirname, 'views'));
//app.set('view engine', 'pug');
//-For handlebars View engine --
//https://www.npmjs.com/package/express-handlebars
//http://handlebarsjs.com/builtin_helpers.html
app.engine('handlebars', exphbs({ defaultLayout: 'main' }));
app.set('view engine', 'handlebars');
//hbs helpers
//var helpers = require('./utils/helpers')
//require('./utils/hbsHelpers.js')(handlebars);
//INLINE Helper is good now. boldit is called without any prefix notion at view ( {{boldit 'any text'}} )
handlebars.registerHelper('boldit', function (text) {
    return new handlebars.SafeString("<strong>" + text + "</strong>");
});
//install hbs - extenstion to handlebars view engine for partials.
var hbs = require("hbs");
hbs.registerPartials(__dirname + '/views/partials'); //it means all files within the folder /views/partials will be treated as partials.
//defining global variables 
hbs.localsAsTemplateData(app);
app.locals.application = "Typescript NodeJS via Handlebars";
app.use(express.static(path.join(__dirname, 'public')));
app.use('/', index_1.default);
app.use('/users', user_1.default);
// catch 404 and forward to error handler
app.use(function (req, res, next) {
    var err = new Error('Not Found');
    err['status'] = 404;
    next(err);
});
// error handlers
// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
    app.use(function (err, req, res, next) {
        res.status(err['status'] || 500);
        res.render('error', {
            message: err.message,
            error: err
        });
    });
}
// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
        message: err.message,
        error: {}
    });
});
app.set('port', process.env.PORT || 3000);
var server = app.listen(app.get('port'), function () {
    debug('Express server listening on port ' + server.address().port);
});
//# sourceMappingURL=app.js.map