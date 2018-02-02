'use strict';
var express = require('express');
var router = express.Router();

require('../utils/genericPromise.js')();

function main() {
    var query = 'SELECT Number, street, city, State FROM [Address]';

    var dataPromise = getSQLDatabaseData(query);

    dataPromise
        .then(function (recordsets) {

            console.log(recordsets);

            /* GET home page. */
            router.get('/', function (req, res) {
                res.render('home', { title: 'First Azure Compatible - Node JS/Retrive Data from SQL Server', "data": recordsets.recordset });
            });

        }, errHandler);        
}

main();

module.exports = router;
