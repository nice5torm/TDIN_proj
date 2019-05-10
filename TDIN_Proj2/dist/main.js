/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./src/main.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./node_modules/electron-cgi/connection-builder.js":
/*!*********************************************************!*\
  !*** ./node_modules/electron-cgi/connection-builder.js ***!
  \*********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

const { spawn } = __webpack_require__(/*! child_process */ "child_process");
const Connection = __webpack_require__(/*! ./connection */ "./node_modules/electron-cgi/connection.js");

function ConnectionBuilder() {
    var spawnArguments = null;
    this.connectTo = (command, ...args) => {
        spawnArguments = {
            command,
            args
        };
        return this;
    };
    this.build = () => {
        if (!spawnArguments) {
            throw new Error('Use connectTo(pathToExecutable, [arguments]) to specify to which executable to connect');
        }
        const executable = spawn(spawnArguments.command, spawnArguments.args);
        return new Connection(executable.stdin, executable.stdout);
    };
}

exports.ConnectionBuilder = ConnectionBuilder;


/***/ }),

/***/ "./node_modules/electron-cgi/connection.js":
/*!*************************************************!*\
  !*** ./node_modules/electron-cgi/connection.js ***!
  \*************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

const Request = __webpack_require__(/*! ./request */ "./node_modules/electron-cgi/request.js");
const TabSeparatedInputStreamParser = __webpack_require__(/*! ./tab-separated-input-stream-parser */ "./node_modules/electron-cgi/tab-separated-input-stream-parser.js");

function Connection(outStream, inStream) {
    const responseHandlersQueue = [];
    const inputStreamParser = new TabSeparatedInputStreamParser();

    inStream.setEncoding('utf8');
    inStream.on('data', (chunk) => {
        inputStreamParser.addPartial(chunk);
    });

    inStream.on('close', () => {
        if (this.onDisconnect) {
            this.onDisconnect();
        }
    });

    inputStreamParser.onResponse(response => {
        const responseIds = responseHandlersQueue.map(r => r.id);
        if (responseIds.indexOf(response.id) !== -1) {
            responseHandlersQueue.splice(responseIds.indexOf(response.id), 1)[0].onResponse(response.result);
        }
    });

    const send = (request, onResponse) => {
        outStream.write(`${JSON.stringify(request)}\t`);
        if (onResponse) {
            responseHandlersQueue.push({
                id: request.id,
                onResponse
            });
        }
    };

    this.onDisconnect = null;

    this.send = (type, args = {}, onResponse = null) => {
        send(new Request(type, args), onResponse);
    };

    this.close = () => {
        outStream.end();
    };
}

module.exports = Connection;

/***/ }),

/***/ "./node_modules/electron-cgi/index.js":
/*!********************************************!*\
  !*** ./node_modules/electron-cgi/index.js ***!
  \********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

const { ConnectionBuilder } = __webpack_require__(/*! ./connection-builder */ "./node_modules/electron-cgi/connection-builder.js");
const { Connection } = __webpack_require__(/*! ./connection */ "./node_modules/electron-cgi/connection.js");

module.exports = {
    ConnectionBuilder,
    Connection
};

/***/ }),

/***/ "./node_modules/electron-cgi/request.js":
/*!**********************************************!*\
  !*** ./node_modules/electron-cgi/request.js ***!
  \**********************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

const uuidv4 = __webpack_require__(/*! uuid/v4 */ "./node_modules/uuid/v4.js");

function Request(type, args) {
    this.type = type;
    this.id = uuidv4();
    this.args = JSON.stringify(args);
}

module.exports = Request;

/***/ }),

/***/ "./node_modules/electron-cgi/tab-separated-input-stream-parser.js":
/*!************************************************************************!*\
  !*** ./node_modules/electron-cgi/tab-separated-input-stream-parser.js ***!
  \************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

const { EventEmitter } = __webpack_require__(/*! events */ "events");

function TabSeparatedInputStreamParser() {
    const responseEmitter = new EventEmitter();
    let streamInput = '';

    this.addPartial = streamContent => {
        streamInput += streamContent;
        while (streamInput.indexOf('\t') !== -1) {
            const responseStr = streamInput.substring(0, streamInput.indexOf('\t'));
            streamInput = streamInput.substring(streamInput.indexOf('\t') + 1);
            let response = null;
            try{            
                response = JSON.parse(responseStr);
            } catch (e){
                throw new Error(`Invalid incoming JSON: ${responseStr}`);
            }
            responseEmitter.emit('response', response);
        }
    };

    this.onResponse = handleResponseCallback => {
        responseEmitter.on('response', handleResponseCallback);
    };
}

module.exports = TabSeparatedInputStreamParser;

/***/ }),

/***/ "./node_modules/uuid/lib/bytesToUuid.js":
/*!**********************************************!*\
  !*** ./node_modules/uuid/lib/bytesToUuid.js ***!
  \**********************************************/
/*! no static exports found */
/***/ (function(module, exports) {

/**
 * Convert array of 16 byte values to UUID string format of the form:
 * XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
 */
var byteToHex = [];
for (var i = 0; i < 256; ++i) {
  byteToHex[i] = (i + 0x100).toString(16).substr(1);
}

function bytesToUuid(buf, offset) {
  var i = offset || 0;
  var bth = byteToHex;
  // join used to fix memory issue caused by concatenation: https://bugs.chromium.org/p/v8/issues/detail?id=3175#c4
  return ([bth[buf[i++]], bth[buf[i++]], 
	bth[buf[i++]], bth[buf[i++]], '-',
	bth[buf[i++]], bth[buf[i++]], '-',
	bth[buf[i++]], bth[buf[i++]], '-',
	bth[buf[i++]], bth[buf[i++]], '-',
	bth[buf[i++]], bth[buf[i++]],
	bth[buf[i++]], bth[buf[i++]],
	bth[buf[i++]], bth[buf[i++]]]).join('');
}

module.exports = bytesToUuid;


/***/ }),

/***/ "./node_modules/uuid/lib/rng.js":
/*!**************************************!*\
  !*** ./node_modules/uuid/lib/rng.js ***!
  \**************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

// Unique ID creation requires a high quality random # generator.  In node.js
// this is pretty straight-forward - we use the crypto API.

var crypto = __webpack_require__(/*! crypto */ "crypto");

module.exports = function nodeRNG() {
  return crypto.randomBytes(16);
};


/***/ }),

/***/ "./node_modules/uuid/v4.js":
/*!*********************************!*\
  !*** ./node_modules/uuid/v4.js ***!
  \*********************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

var rng = __webpack_require__(/*! ./lib/rng */ "./node_modules/uuid/lib/rng.js");
var bytesToUuid = __webpack_require__(/*! ./lib/bytesToUuid */ "./node_modules/uuid/lib/bytesToUuid.js");

function v4(options, buf, offset) {
  var i = buf && offset || 0;

  if (typeof(options) == 'string') {
    buf = options === 'binary' ? new Array(16) : null;
    options = null;
  }
  options = options || {};

  var rnds = options.random || (options.rng || rng)();

  // Per 4.4, set bits for version and `clock_seq_hi_and_reserved`
  rnds[6] = (rnds[6] & 0x0f) | 0x40;
  rnds[8] = (rnds[8] & 0x3f) | 0x80;

  // Copy bytes to buffer, if provided
  if (buf) {
    for (var ii = 0; ii < 16; ++ii) {
      buf[i + ii] = rnds[ii];
    }
  }

  return buf || bytesToUuid(rnds);
}

module.exports = v4;


/***/ }),

/***/ "./src/main.ts":
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var electron__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! electron */ "electron");
/* harmony import */ var electron__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(electron__WEBPACK_IMPORTED_MODULE_0__);
var url = __webpack_require__(/*! url */ "url");

var path = __webpack_require__(/*! path */ "path");

var _require = __webpack_require__(/*! electron-cgi */ "./node_modules/electron-cgi/index.js"),
    ConnectionBuilder = _require.ConnectionBuilder;


var window;

var createWindow = function createWindow() {
  window = new electron__WEBPACK_IMPORTED_MODULE_0__["BrowserWindow"]({
    width: 800,
    height: 600
  });
  window.loadURL(url.format({
    pathname: path.join(__dirname, "index.html"),
    protocol: "file:",
    slashes: true
  }));
  window.on("closed", function () {
    window = null;
  });
};

electron__WEBPACK_IMPORTED_MODULE_0__["app"].on("ready", createWindow);
electron__WEBPACK_IMPORTED_MODULE_0__["app"].on("window-all-closed", function () {
  if (process.platform !== "darwin") {
    electron__WEBPACK_IMPORTED_MODULE_0__["app"].quit();
  }
});
electron__WEBPACK_IMPORTED_MODULE_0__["app"].on("activate", function () {
  if (window === null) {
    createWindow();
  }
});
var connection = new ConnectionBuilder().connectTo("dotnet", "run", "--project", "./core/Core").build();

connection.onDisconnect = function () {
  console.log("lost");
};

connection.send("greeting", "Mom", function (response) {
  console.log(response);
  connection.close();
});

/***/ }),

/***/ "child_process":
/*!********************************!*\
  !*** external "child_process" ***!
  \********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = require("child_process");

/***/ }),

/***/ "crypto":
/*!*************************!*\
  !*** external "crypto" ***!
  \*************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = require("crypto");

/***/ }),

/***/ "electron":
/*!***************************!*\
  !*** external "electron" ***!
  \***************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = require("electron");

/***/ }),

/***/ "events":
/*!*************************!*\
  !*** external "events" ***!
  \*************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = require("events");

/***/ }),

/***/ "path":
/*!***********************!*\
  !*** external "path" ***!
  \***********************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = require("path");

/***/ }),

/***/ "url":
/*!**********************!*\
  !*** external "url" ***!
  \**********************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = require("url");

/***/ })

/******/ });
//# sourceMappingURL=main.js.map