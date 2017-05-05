var chalk = require('chalk');

module.exports = class logger{    

    constructor(){
        this._checkThings = {
            errors: ["error", "failed"],
            good: ["succeeded"],
            warning: ["warn"],
            information: ["Build started", "reloading", "function started"]
        }

        this._errorRegular = chalk.red;
        this._errorBold = chalk.red.bold;
        this._warnRegular = chalk.yellow;
        this._warnBold = chalk.yellow.bold;
        this._infoRegular = chalk.cyan;
        this._goodRegular = chalk.green;
    }

    log(output){
        if(this._check(output, this._checkThings.errors)){
            this.logError(output);
        }else if(this._check(output, this._checkThings.warning)){
            this.logWarning(output);
        }else if(this._check(output, this._checkThings.good)){
            this.logGood(output);
        }else if(this._check(output, this._checkThings.information)){
            this.logInfo(output);
        }else{
            console.log(output);
        }
    }

    logException(output){
        console.log(this._errorRegular(output));
    }

    logError(output){
        console.log(this._errorRegular(output));
    }

    logInfo(output){
        console.log(this._infoRegular(output));
    }

    logWarning(output){
        console.log(this._warnRegular(output));
    }

    logGood(output){
        console.log(this._goodRegular(output));
    }

    _check(output, checkThings)
    {
        var outputLower = output.toLowerCase();
        
        for(var i in checkThings){
            var thing = checkThings[i];
            
            if(output.toLowerCase().indexOf(thing.toLowerCase())!=-1){
                return true;
            }
        }        

        return false;
    }
}