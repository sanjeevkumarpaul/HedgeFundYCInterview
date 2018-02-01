module.exports = function () {
    //SQL Server
    //npm install tedious
    //npm install async

    /*
    var SqlConnection = require('tedious').Connection;
    var Request = require('tedious').Request;
    var TYPES = require('tedious').TYPES;
    var async = require('async');

    // Create connection to database
    var sqlconfig = {
        userName: 'nodejs',
        password: 'Deepak2!',
        server: 'localhost',
        options: {
            database: 'Home'
        }
    };
    */

    this.sql = require('mssql');

    this.config = {
        user: 'nodejs',
        password: 'Deepak2!',
        server: 'localhost',
        database: 'Home'
    }

    
}


