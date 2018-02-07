/*
 * GET home page.
 */

import express = require('express');
const router = express.Router();

import Promise from "ts-promise";

const promise = new Promise((resolve, reject) => { resolve(1); });


router.get('/', (req: express.Request, res: express.Response) => {
    res.render('index', { title: 'Express' });
});

export default router; 