
import Promise from "ts-promise";
import _sql = require('mssql');
//import * as _sql from 'mssql';
//import { PoolConfiguration, TediousConfiguration, NodeSqlServerConfiguration, Request } from '~mssql/lib/main';

class Config implements _sql.Configuration {
    server: string;
    user: string;
    password: string;    
    database: string;

    constructor() {
        this.server = "SANPC-MACMINI";
        this.user="nodejs";
        this.password= "nodejs123";
        this.database= "Home";

    }   
}

class SqlDatabase {


    Query(query: string) {
             
        var connection = new _sql.Connection(new Config());
        
        return connection.connect()            
            .then((conn) => {

                return conn.request()
                    .query(query )                    
                    .then((resultset) => {

                        console.log("connection after execution Resolver");
                        return resultset;
                    })
                    .catch((e) => { throw e; })
            })
            .then((resultset) => { console.log("connection release Resolver");  return resultset; })
            .catch((e) => { connection.close(); throw e; })                      
    }    
}

export { SqlDatabase as Database };
export default Config;
