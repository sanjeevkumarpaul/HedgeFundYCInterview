'use strict';
var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function (req, res) {
    res.render('home', { title: 'First Azure Compatible - Node JS' });
});

module.exports = router;
