module.exports = function () {

    //Handles error for all
    this.errHandler = function (err) {
        console.log(err);
    }


    ////////////////////////////////////////////////////
    /////////// GET DATA FROM URL //////////////////////
    var request = require('request');
    var userDetails;

    this.getData = function (url) {
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
                } else {
                    resolve(body);
                }
            })
        })
    }
    /////////// END - GET DATA FROM URL //////////////////////
    //////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////
    /////////// GET DATA FROM SQL //////////////////////

    this.getSQLDatabaseData = function (query) {

        require('../database/Sql.js')();
        
        return new Promise(function (resolve, reject) {

            var connection = sql.connect(config, function (err) {
                if (err) {
                    reject(err);
                }
                else {

                    var request = new sql.Request();

                    request.query(query, function (err, recordsets) {

                        if (err) {
                            reject(err);
                        }
                        else {
                            resolve(recordsets);
                        }
                    });
                }
            });
        });

    }


    /////////// END - GET DATA FROM SQL //////////////////////
    //////////////////////////////////////////////////////////
}