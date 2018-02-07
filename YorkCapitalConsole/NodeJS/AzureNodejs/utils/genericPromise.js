module.exports = function () {

    require('./promiseCatches.js')();


    //Handles error for all
    this.errHandler = function (err) {
        console.log('Stack : ' + err.stack);
        console.log('--------------------------------------------------------------------------------------------------------------');
        console.log('Code : ' + err.code);
        console.log('Message : ' + err.message);
        console.log('--------------------------------------------------------------------------------------------------------------');
    }


    ////////////////////////////////////////////////////
    /////////// GET DATA FROM URL //////////////////////
    
    this.getData = function (url) {

        var request = require('request');

        // Setting URL and headers for request
        var options = {
            url: url,
            headers: {
                'User-Agent': 'request'
            }
        };
        // Return new promise 
        return new Promise(function (resolve, reject) {
            // Do async job
            request.get(options, function (err, resp, body) {
                if (err) {
                    reject(err);
                } else {resolve(body);
                }
            })
        })
    }
    /////////// END - GET DATA FROM URL //////////////////////
    //////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////
    /////////// GET DATA FROM SQL //////////////////////

    this.getSQLDatabaseData = function (query, router) {

        require('../database/Sql.js')();
        
        var _promise = new Promise(function (resolve, reject) {

            var connection = sql.connect(config, function (err) {
                if (err) {
                    reject(err);
                }
                else {

                    var request = new sql.Request();

                    request.query(query, function (err, results) {

                        if (err) {
                            reject(err);
                        }
                        else {
                            resolve(results);
                        }
                    });
                }
            });
        });

        CatchAndThowToErrorPage(_promise, router);
        return _promise;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~ Another ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    this.getSQLDatabaseDataMssqlPromise = function (query, router) {

        require('../database/Sql.js')();

        var _promise = new Promise(function (resolve, reject) {

            //sql.on('error', err => { console.log(err); });  //kind of a call back for .catch in the promise chain.
           
            sql.connect(config)
                    .then(pool => {

                        return pool.request().query(query);
                    })
                    .then(result => {
                        resolve(result);
                    })
                .catch(function (e) { reject(e);  })    
        });        

        CatchAndThowToErrorPage(_promise, router);
        return _promise;
    }
    
    /////////// END - GET DATA FROM SQL //////////////////////
    //////////////////////////////////////////////////////////
}