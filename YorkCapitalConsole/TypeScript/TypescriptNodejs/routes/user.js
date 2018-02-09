"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
/*
 * GET users listing.
 */
var express = require("express");
var router = express.Router();
var getData = /** @class */ (function () {
    function getData() {
        this.data = {
            'item1': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-76.jpg',
            'item2': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-77.jpg',
            'item3': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-78.jpg'
        };
    }
    return getData;
}());
router.get('/', function (req, res) {
    res.render('Usersview', { title: 'Users With Images', 'data': new getData().data });
});
exports.default = router;
//# sourceMappingURL=user.js.map