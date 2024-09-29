mergeInto(LibraryManager.library, {

    GetTelegramWebAppInitData: function() {
        if (window.Telegram.WebApp.initData === undefined)
        {
            return null;
        }

        let initData = window.Telegram.WebApp.initData;
        let returnStr = decodeURIComponent(initData);
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    GetWebUrl: function () {
        let returnStr = window.location.href;
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    GetLoginToken: function () {
        var url = new URL(window.location.href);
        console.log(url);
        var params = new URLSearchParams(url.search);
        var token = params.get('token');
        if (token != null)
        {
            var bufferSize = lengthBytesUTF8(token) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(token, buffer, bufferSize);
            console.log(buffer);
            return buffer;
        }
    },

    GetUserId: function () {
        var token = window.userId;
        console.log(token);
        if (token != null)
        {
            var bufferSize = lengthBytesUTF8(token) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(token, buffer, bufferSize);
            console.log(buffer);
            return buffer;
        }
    },

    GetUserName: function () {
        var token = window.userName;
        console.log(token);
        if (token != null)
        {
            var bufferSize = lengthBytesUTF8(token) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(token, buffer, bufferSize);
            console.log(buffer);
            return buffer;
        }
    },

    ShareViaTelegram: function (_shareUrl, _shareCommoent) {
        var shareComment = encodeURIComponent(UTF8ToString(_shareCommoent));
        var shareUrl = encodeURIComponent(UTF8ToString(_shareUrl));
        var openUrl = "https://t.me/share/url?url=" + shareUrl + "&text=" + shareComment;
        if (window.Telegram.WebApp.openTelegramLink === undefined) {
            window.open(url, '_blank');
        } else {
            window.Telegram.WebApp.openTelegramLink(openUrl);
        }
    },
});
