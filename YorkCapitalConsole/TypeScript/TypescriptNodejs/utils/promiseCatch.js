"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var PromiseExceptionCatcher = /** @class */ (function () {
    function PromiseExceptionCatcher() {
    }
    PromiseExceptionCatcher.prototype.CatchAndThowToErrorPage = function (promise, router) {
        promise.catch(function (err) {
            if (router != null)
                router.get('/', function (req, res) { res.render('error', { "message": err.message, "code": err.code }); });
        });
    };
    return PromiseExceptionCatcher;
}());
exports.PromiseExceptionCatcher = PromiseExceptionCatcher;
//# sourceMappingURL=promiseCatch.js.map