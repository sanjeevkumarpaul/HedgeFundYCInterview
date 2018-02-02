'use strict';
var express = require('express');
var router = express.Router();

require('../utils/genericPromise.js')();

function main() {
    var query = 'SELECT Number, Street, City, State FROM [Address]';

    var dataPromise = getSQLDatabaseDataMssqlPromise(query, router); //getSQLDatabaseData(query);
    
    dataPromise
        .then(function (results) {

            console.log(results.recordset.columns);

            /* GET home page. */
            router.get('/', function (req, res) {
                res.render('home', { title: 'First Azure Compatible - Node JS/Retrive Data from SQL Server', "data": results.recordset });
            });
        }, errHandler);
         //Below can also hanled error to avoid unhandled errors.
        //.catch(function (err) { console.log(err.message); });   
}

main();

module.exports = router;
