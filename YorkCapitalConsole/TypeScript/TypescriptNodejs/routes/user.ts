/*
 * GET users listing.
 */
import express = require('express');
const router = express.Router();


class getData {
    data: object = {
        'item1': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-76.jpg',
        'item2': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-77.jpg',
        'item3': 'http://public-domain-photos.com/free-stock-photos-1/flowers/cactus-78.jpg'
    }    
}

router.get('/', (req: express.Request, res: express.Response) => {
    res.render('Usersview', { title: 'Users With Images', 'data': new getData().data });
});

export default router;