"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var _sql = require("mssql");
//import * as _sql from 'mssql';
//import { PoolConfiguration, TediousConfiguration, NodeSqlServerConfiguration, Request } from '~mssql/lib/main';
var Config = /** @class */ (function () {
    function Config() {
        this.server = "SANPC-MACMINI";
        this.user = "nodejs";
        this.password = "nodejs123";
        this.database = "Home";
    }
    return Config;
}());
var SqlDatabase = /** @class */ (function () {
    function SqlDatabase() {
    }
    SqlDatabase.prototype.Query = function (query) {
        var connection = new _sql.Connection(new Config());
        return connection.connect()
            .then(function (conn) {
            return conn.request()
                .query(query)
                .then(function (resultset) {
                console.log("connection after execution Resolver");
                return resultset;
            })
                .catch(function (e) { throw e; });
        })
            .then(function (resultset) { console.log("connection release Resolver"); return resultset; })
            .catch(function (e) { connection.close(); throw e; });
    };
    return SqlDatabase;
}());
exports.Database = SqlDatabase;
exports.default = Config;
//# sourceMappingURL=Sql.js.map