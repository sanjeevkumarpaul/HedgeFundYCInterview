import promise from "ts-promise";
import express = require('express');

export class PromiseExceptionCatcher
{
    CatchAndThowToErrorPage<T>(promise: Promise<T>, router: express.Router) {

        promise.catch((err) => {
            if (router != null)
                router.get('/', (req: express.Request, res: express.Response) => { res.render('error', { "message": err.message, "code": err.code }); });
        });
    }
}