'use strict';
var express = require('express');
var router = express.Router();

var getData = function () {
    var data = {
        'item1': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-76.jpg',
        'item2': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-77.jpg',
        'item3': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-78.jpg'
    }
    return data;
}

/* GET users listing. */
router.get('/', function (req, res) {
    //res.send('respond with a resource');
    res.render('Usersview', { title: 'From user level- Node JS', "data": getData() });    
});

module.exports = router;
