"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var express = require("express");
var router = express.Router();
var genericPromise_1 = require("../utils/genericPromise");
var _query = "SELECT Number, Street, City, State FROM [Address]";
new genericPromise_1.AppPromises().getDataViaMssqlPromise(router, _query)
    .then(function (resultset) {
    /*
     * GET home page.
   */
    console.log("Results fetched to the page.");
    router.get('/', function (req, res) {
        res.render('home', { title: 'Express', 'data': resultset });
    });
});
exports.default = router;
//# sourceMappingURL=index.js.map