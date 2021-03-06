/**
 * 获取URL参数
 * @param {参数key} name 
 */
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]);
    return null;
}

/**
 * 验证指定元素下所有easyui-validatebox是否满足条件
 */
function isValidate(id) {
    try {
        var v = $('#' + id + ' .easyui-validatebox');
        if (v != null && v.length > 0) {
            for (var i = 0; i < v.length; i++) {
                if (!$(v[i]).validatebox('isValid')) {
                    return false;
                }
            }
            return true;
        } else {
            return true;
        }
    } catch (e) {
        return false;
    }
}

/**
 * uuid
 */
function uuid() {
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4"; // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1); // bits 6-7 of the clock_seq_hi_and_reserved to 01
    s[8] = s[13] = s[18] = s[23] = "-";

    var uuid = s.join("");
    return uuid;
}

/**
 * 判断数组是否包含指定元素
 * @param {*} needle 
 */
Array.prototype.contains = function(needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}