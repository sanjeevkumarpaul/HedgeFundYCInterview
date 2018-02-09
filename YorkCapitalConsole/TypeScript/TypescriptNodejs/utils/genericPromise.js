"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ts_promise_1 = require("ts-promise");
var sql = require("../database/Sql");
var catches = require("./promiseCatch");
var ApplicationPromises = /** @class */ (function () {
    function ApplicationPromises() {
    }
    ApplicationPromises.prototype.getDataViaMssqlPromise = function (router, query) {
        var _promise = new ts_promise_1.default(function (resolve, reject) {
            console.log("Promise created at Database Module.");
            var _database = new sql.Database();
            return _database.Query(query)
                .then(function (results) {
                console.log("fetched recordset.");
                if (results != null && results != undefined)
                    resolve(results);
                else
                    reject(new Error("Results can not be fetched at this time"));
            })
                .catch(function (e) { reject(e); });
        });
        new catches.PromiseExceptionCatcher().CatchAndThowToErrorPage(_promise, router);
        return _promise;
    };
    return ApplicationPromises;
}());
exports.AppPromises = ApplicationPromises;
//# sourceMappingURL=genericPromise.js.map