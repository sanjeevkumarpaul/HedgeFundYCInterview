'use strict';
var express = require('express');
var router = express.Router();

require('../database/Sql.js')();

/* GET home page. */
router.get('/', function (req, res) {
    res.render('home', { title: 'First Azure Compatible - Node JS', "data": Read() });
});

function Read() {

    var data = null;

    var connection = sql.connect(config, function (err) {

        if (err) return;

        var request = new sql.Request();

        request.query('SELECT Number, street, city, State FROM [Address]', function (err, recordset) {

            if (err) return;

            data = recordset;
        });
    });    

    return data;
}

module.exports = router;
