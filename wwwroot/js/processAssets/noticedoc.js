$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#notice_datagrid').datagrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function() {
        cleanAddNoticeDialog();
        $('#dlg_add_notice').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        var rows = $('#notice_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddNoticeDialog();
            $.ajax({
                url: '/ProcessAssets/GetNoticeFiles/',
                type: 'post',
                dataType: 'json',
                data: { id: rows[0].id },
                success: function(data) {
                    if (data != null && data.code == 200 && data.data != null) {
                        var files = [];
                        for (var i = 0; i < data.data.length; i++) {
                            files.push({
                                fileName: data.data[i].file_name,
                                fileSize: data.data[i].file_size,
                                fileUrl: data.data[i].file_path
                            });
                        }
                        setHistoryFileList(files);
                    }
                }
            });
            $('#hid_id').val(rows[0].id);
            $('#txt_title').textbox('setValue', rows[0].title != null ? rows[0].title : '');
            $('#cmb_is_settop').combobox('setValue', rows[0].is_settop != null ? rows[0].is_settop : 0);
            $('#txt_remarks').textbox('setValue', rows[0].remarks != null ? rows[0].remarks : '');
            $('#dlg_add_notice').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条公告文档数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#notice_datagrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除公告文档数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/ProcessAssets/DeleteNotice/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#notice_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '删除失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            });
        } else {
            $.messager.alert('提示', '请选择一条公告文档数据！');
        }
    });
}

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#notice_datagrid').datagrid({
        url: '/ProcessAssets/GetNoticeToPage/',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: true, // 启用分页
        pageList: [15, 30, 50],
        pageSize: 15,
        pageNumber: 1,
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'title', title: '标题', width: 200 },
                {
                    field: 'is_settop',
                    title: '是否置顶',
                    width: 60,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            if (value == 1) {
                                return '是';
                            } else {
                                return '否';
                            }
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'remarks', title: '备注', width: 300 },
                {
                    field: 'id',
                    title: '查看附件',
                    width: 61,
                    formatter: function(value, row, index) {
                        return '<a href="javascript:void(0);" style="text-decoration:none;" onClick="seeFiles(' + (value != null ? value : '') + ');">查看附件</a>';
                    }
                },
                { field: 'last_modify_name', title: '最后修改人', width: 132 },
                {
                    field: 'create_time',
                    title: '创建时间',
                    width: 130,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
                {
                    field: 'last_modify_time',
                    title: '最后修改时间',
                    width: 130,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
            ]
        ],
        onBeforeLoad: function(param) {
            param.title_like = $('#txt_title_like').textbox('getValue') != null ? $('#txt_title_like').textbox('getValue') : ''
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {
    $('#dlg_add_notice').dialog({
        toolbar: [{
            id: 'notice_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_role')) {
                    if ($('#notice_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#notice_save').linkbutton("disable"); // 禁用

                    var params = {
                        id: $('#hid_id').val(),
                        title: $('#txt_title').textbox('getValue'),
                        is_settop: $('#cmb_is_settop').combobox('getValue'),
                        remarks: $('#txt_remarks').textbox('getValue'),
                        files: getFileList()
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/ProcessAssets/AddNotice/' : '/ProcessAssets/UpdateNotice/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#notice_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#notice_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_notice').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#notice_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#notice_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#notice_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });
}

function cleanAddNoticeDialog() {
    cleanUploadFile();
    if ($('#notice_save').linkbutton("options").disabled) {
        $('#notice_save').linkbutton("enable");
    }

    $('#hid_id').val('');
    $('#txt_title').textbox('clear');
    $('#cmb_is_settop').combobox('setValue', 0);
    $('#txt_remarks').textbox('clear');
}

/**
 * 根据主键查询附件信息
 * @param {主键} id 
 */
function seeFiles(id) {
    $('#dlg_notice_file').dialog('open');

    $('#file_datagrid').datagrid({
        url: '/ProcessAssets/GetNoticeFileList/',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'file_name', title: '名称', width: 453 },
                {
                    field: 'file_size',
                    title: '大小',
                    align: 'right',
                    halign: 'center',
                    width: 80,
                    formatter: function(value, row, index) {
                        return (value != null ? (value / (1024 * 1024)).toFixed(2) : '') + 'MB';
                    }
                },
                {
                    field: 'file_path',
                    title: '操作',
                    width: 40,
                    formatter: function(value, row, index) {
                        return '<a href="/Public/DownloadFile?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">下载</a>';
                    }
                }
            ]
        ],
        onBeforeLoad: function(param) {
            param.id = id
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}