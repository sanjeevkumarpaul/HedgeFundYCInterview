
import express = require('express');
const router = express.Router();

import { AppPromises } from '../utils/genericPromise';

const _query: string = "SELECT Number, Street, City, State FROM [Address]";

new AppPromises().getDataViaMssqlPromise(router, _query)
    .then((resultset) => {

        /*
         * GET home page.
       */
        console.log("Results fetched to the page.");

        router.get('/', (req: express.Request, res: express.Response) => {
            res.render('home', { title: 'Express', 'data': resultset });
        });
    });



export default router; 