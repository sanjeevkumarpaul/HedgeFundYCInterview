///
///  Other utilities for promise functionality.
///
module.exports = function () {

    this.CatchAndThowToErrorPage = function (promise, router) {

        promise.catch(function (err) {
            if (router != null)
                router.get('/', function (req, res) { res.render('error', { "message": err.message, "code": err.code }); });
        });
    }
}