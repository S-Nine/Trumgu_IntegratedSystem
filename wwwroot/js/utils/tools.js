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
 * 判断数组是否包含指定元素
 * @param {*} needle 
 */
Array.prototype.contains = function(needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}