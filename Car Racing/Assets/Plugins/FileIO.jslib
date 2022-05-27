var FileIO = {

  SaveToLocalStorage : function(key, data) {
    localStorage.setItem(Pointer_stringify(key), Pointer_stringify(data));
},
  SaveToSessionStorage : function(key, data) {
    sessionStorage.setItem(Pointer_stringify(key), Pointer_stringify(data));
},

  LoadFromLocalStorage: function(key) {
    var returnStr = localStorage.getItem(Pointer_stringify(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
},

 LoadFromSessionStorage: function(key) {
    var returnStr = sessionStorage.getItem(Pointer_stringify(key));
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
},


  RemoveFromLocalStorage: function(key) {
    localStorage.removeItem(Pointer_stringify(key));
},

  RemoveFromSessionStorage: function(key) {
    sessionStorage.removeItem(Pointer_stringify(key));
},

  HasKeyInLocalStorage: function(key) {
    if (localStorage.getItem(Pointer_stringify(key)))
    {
        return 1;
    }
    else
    {
        return 0;
    }
},

 HasKeyInSessionStorage: function(key) {
    if (sessionStorage.getItem(Pointer_stringify(key)))
    {
        return 1;
    }
    else
    {
        return 0;
    }
},

copyToClipboard: function (mtext) {
    if (navigator.clipboard && window.isSecureContext) {
        navigator.clipboard.writeText(Pointer_stringify(mtext));
    }
    else
    {
        console.log("Copy code error sor browser not supported");
    }
},

pageReloadCheck : function()
    {
        if (window.performance) {
            console.info("window.performance works fine on this browser");
        }

        if (performance.navigation.type == performance.navigation.TYPE_RELOAD) {
            return 1;
            console.info("This page is reloaded");
        } else {
            return 0;
            console.info("This page is not reloaded");
        }

    }

};

mergeInto(LibraryManager.library, FileIO);