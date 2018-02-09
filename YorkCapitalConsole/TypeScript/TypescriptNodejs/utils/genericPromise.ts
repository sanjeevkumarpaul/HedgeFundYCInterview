import Promise from "ts-promise";
import Express = require('express');

import * as sql from "../database/Sql";
import * as catches from "./promiseCatch";

class ApplicationPromises {

    getDataViaMssqlPromise(router: Express.Router, query: string) {
        
        var _promise = new Promise((resolve, reject) => {
            console.log("Promise created at Database Module.");

            var _database = new sql.Database();
            
            return _database.Query(query)
                .then((results) => {

                    console.log("fetched recordset.");
                    if (results != null && results != undefined)
                        resolve(results);
                    else
                        reject(new Error("Results can not be fetched at this time"));
                })
                .catch((e) => { reject(e); });
        });
               
        new catches.PromiseExceptionCatcher().CatchAndThowToErrorPage(_promise, router);

        return _promise;
    }
}

export { ApplicationPromises as AppPromises };

