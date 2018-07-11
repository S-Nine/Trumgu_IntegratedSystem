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
        $('#cmb_edit_type').combobox('reload');
        $('#dlg_add_notice').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        var rows = $('#notice_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddNoticeDialog();
            $.ajax({
                url: '/Public/GetFiles/',
                type: 'post',
                dataType: 'json',
                data: { id: rows[0].id, belong_modular: 't_assets_organizational_process_assets' },
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
            $('#cmb_edit_type').combobox('setValue', rows[0].type);
            $('#txt_remarks').textbox('setValue', rows[0].remarks != null ? rows[0].remarks : '');
            $('#dlg_add_notice').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条组织过程资产数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#notice_datagrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除组织过程资产数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/ProcessAssets/DeleteOrganizationalProcessAssets/',
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
            $.messager.alert('提示', '请选择一条组织过程资产数据！');
        }
    });
}

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#notice_datagrid').datagrid({
        url: '/ProcessAssets/GetOrganizationalProcessAssestsToPage/',
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
                { field: 'title', title: '资产标题', width: 200 },
                { field: 'type_name', title: '资产类型', width: 60 },
                { field: 'remarks', title: '资产备注', width: 300 },
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
            param.type = $('#cmb_type').combobox('getValue');
            if (param.type == 0) {
                param.type = null;
            }
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
                        type: $('#cmb_edit_type').combobox('getValue'),
                        remarks: $('#txt_remarks').textbox('getValue'),
                        files: getFileList()
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/ProcessAssets/AddOrganizationalProcessAssets/' : '/ProcessAssets/UpdateOrganizationalProcessAssets/',
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

    $('#cmb_type').combobox({
        onLoadSuccess: function() {
            $('#cmb_type').combobox('setValue', 0);
        }
    });
    $('#cmb_edit_type').combobox({
        onLoadSuccess: function() {
            var data = $('#cmb_edit_type').combobox('getData');
            if (data != null && data.length > 0) {
                $('#cmb_edit_type').combobox('setValue', data[0].code);
            }
        }
    });
}

function cleanAddNoticeDialog() {
    cleanUploadFile();
    if ($('#notice_save').linkbutton("options").disabled) {
        $('#notice_save').linkbutton("enable");
    }

    $('#hid_id').val('');
    $('#txt_title').textbox('clear');
    $('#txt_remarks').textbox('clear');
}

/**
 * 根据主键查询附件信息
 * @param {主键} id 
 */
function seeFiles(id) {
    $('#dlg_notice_file').dialog('open');

    $('#file_datagrid').datagrid({
        url: '/Public/GetFilesToList/',
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
            param.id = id;
            param.belong_modular = 't_assets_organizational_process_assets';
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}