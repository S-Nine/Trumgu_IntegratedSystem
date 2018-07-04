var wait_file_list = [];
var url = null, // 上传文件服务接口
    type = null; // 上传文件类型（用于区分文件路径）
$(document).ready(function() {
    var u = GetUrlParam("url");
    var t = GetUrlParam("type");
    if (u != null && u != '') {
        url = u;
    }
    if (t != null && t != '') {
        type = t;
    }
    $('.filess').attr('accept', '.xls,xlsx,.doc,.ppt,.zip,.rar,.pdf,.jpg,.png');
    $('.filess').change(selectFile);
});
/*
 * 上传附件
 */
function addImage() {
    $('.filess').click();
}

/**
 * 监听选择文件事件
 * @param {*} e 
 */
function selectFile(e) {
    var f = $('.filess')[0].files[0];
    var id = 'file_' + uuid();
    var h = '<div class="div_file_block upload_file_block" id="' + id + '">' +
        ' <img style="position:absolute;margin:-5px 0px;width:15px;height:15px;" src="/images/delete.png" onclick="$(this).parent().remove();"/>' +
        ' <p class="file_state">待上传</p>' +
        ' <img src="/images/unknown_type.png" style="width:72px;height:72px;margin-top:3px;" title="文件" />' +
        ' <p class="column-elipsis" style="margin-top:-6px;text-align:center;">' + f.name + '</p>' +
        ' </div>';
    $('#upload_add').before(h);
    f.id = id;
    $('#' + f.id).attr('data-state', 'wait'); // 文件状态
    AjaxFile(f, 0);
    $('.filess').val('');
}

/**
 * 分片异步上传
 * @param {*} file 
 * @param {*} i 
 */
function AjaxFile(file, i) {
    var name = file.name, //文件名
        size = file.size, //总大小shardSize = 2 * 1024 * 1024, 
        shardSize = 2 * 1024 * 1024, //以2MB为一个分片
        shardCount = Math.ceil(size / shardSize); //总片数
    if (i >= shardCount) {
        return;
    }
    //计算每一片的起始与结束位置
    var start = i * shardSize,
        end = Math.min(size, start + shardSize);
    //构造一个表单，FormData是HTML5新增的
    var form = new FormData();
    form.append("data", file.slice(start, end)); //slice方法用于切出文件的一部分
    form.append("lastModified", file.lastModified);
    form.append("fileName", name);
    form.append("total", shardCount); //总片数
    form.append("index", i + 1); //当前是第几片
    if (type != null) {
        form.append("type", type);
    }
    UploadPath = file.lastModified
        //Ajax提交文件
    $.ajax({
        url: "/Public/" + (url != null ? url : "UploadSliceFile"),
        type: "POST",
        data: form,
        async: true, //异步
        processData: false, //很重要，告诉jquery不要对form进行处理
        contentType: false, //很重要，指定为false才能形成正确的Content-Type
        success: function(result) {
            if (result != null) {
                i = result.number++;
                var num = Math.ceil(i * 100 / shardCount);
                $('#' + file.id + ' .file_state').html(num + '%');
                AjaxFile(file, i);
                if (result.mergeOk) {
                    $('#' + file.id + ' .file_state').css('color', 'green');
                    $('#' + file.id + ' .file_state').html('上传成功');
                    $('#' + file.id).attr('data-fileName', file.name); // 文件名称
                    $('#' + file.id).attr('data-fileSize', file.size); // 文件大小
                    $('#' + file.id).attr('data-state', 'success'); // 文件状态
                    $('#' + file.id).attr('data-fileUrl', result.fileUrl); // 文件路径
                }
            }
        }
    });
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
 * 清空文件列表
 */
function cleanUploadFile() {
    $('.upload_file_block').remove();
}

/**
 * 获取文件列表
 */
function getFileList() {
    var files = [];
    if ($('.upload_file_block') != null) {
        for (var i = 0; i < $('.upload_file_block').length; i++) {
            var fileName = $($('.upload_file_block')[i]).attr('data-fileName');
            var fileSize = $($('.upload_file_block')[i]).attr('data-fileSize');
            var state = $($('.upload_file_block')[i]).attr('data-state');
            var fileUrl = $($('.upload_file_block')[i]).attr('data-fileUrl');
            if (state != null && state == 'success') {
                files.push({
                    fileName: fileName != null ? fileName : '', // 文件名称
                    fileSize: fileSize != null ? fileSize : '', // 文件大小
                    fileUrl: fileUrl != null ? fileUrl : '' // 文件路径
                });
            }
        }
    }
    return files;
}

/**
 * 设置历史文件列表
 * @param {*} files 
 */
function setHistoryFileList(files) {
    if (files != null && files.length > 0) {
        for (var i = 0; i < files.length; i++) {
            var h = '<div class="div_file_block upload_file_block" data-fileName="' + files[i].fileName + '" data-state="success" data-fileSize="' + files[i].fileSize + '" data-fileUrl="' + files[i].fileUrl + '">' +
                ' <img style="position:absolute;margin:-5px 0px;width:15px;height:15px;" src="/images/delete.png" onclick="$(this).parent().remove();"/>' +
                ' <p class="file_state" style="color:green">上传完成</p>' +
                ' <img src="/images/unknown_type.png" style="width:72px;height:72px;margin-top:3px;" title="文件" />' +
                ' <p class="column-elipsis" style="margin-top:-6px;text-align:center;">' + files[i].fileName + '</p>' +
                ' </div>';
            $('#upload_add').before(h);
        }
    }
}

/**
 * 获取Url参数
 * @param {参数名称} paraName 
 */
function GetUrlParam(paraName) {
    var url = document.location.toString();
    var arrObj = url.split("?");
    if (arrObj.length > 1) {
        var arrPara = arrObj[1].split("&");
        var arr;
        for (var i = 0; i < arrPara.length; i++) {
            arr = arrPara[i].split("=");
            if (arr != null && arr[0] == paraName) {
                return arr[1];
            }
        }
        return "";
    } else {
        return "";
    }
}