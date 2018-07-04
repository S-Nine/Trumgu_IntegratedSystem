$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#company_datagrid').datagrid({
        url: '/XFundOrganization/GetPrivateCompanyInvestigationToPage/',
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
                { field: 'cn_name', title: '公司名称', width: 200 },
                { field: 'regis_code', title: '公司编号', width: 80 },
                { field: 'jdcs', title: '尽调次数', width: 80 },
                {
                    field: 'gsjs',
                    title: '公司介绍',
                    align: 'center',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && $.trim(value) != '') {
                            return '<a href="/Public/PreviewXFundPDF?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">查看</a>&nbsp;&nbsp;<a href="/Public/DownloadXFundFile?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">下载</a>';
                        } else {
                            return '';
                        }
                    }
                },
                {
                    field: 'jdjl',
                    title: '尽调记录',
                    align: 'center',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && $.trim(value) != '') {
                            return '<a href="/Public/PreviewXFundPDF?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">查看</a>&nbsp;&nbsp;<a href="/Public/DownloadXFundFile?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">下载</a>';
                        } else {
                            return '';
                        }
                    }
                }
            ]
        ],
        onBeforeLoad: function(param) {
            param.cn_name_like = $('#txt_cn_name_like').textbox('getValue') != null ? $('#txt_cn_name_like').textbox('getValue') : ''
            if ($('#chk_yes_1').prop('checked') == true && $('#chk_no_1').prop('checked') != true) {
                param.isIntroduction = true;
            } else if ($('#chk_yes_1').prop('checked') != true && $('#chk_no_1').prop('checked') == true) {
                param.isIntroduction = false;
            }
            if ($('#chk_yes_2').prop('checked') == true && $('#chk_no_2').prop('checked') != true) {
                param.isInvestigation = true;
            } else if ($('#chk_yes_2').prop('checked') != true && $('#chk_no_2').prop('checked') == true) {
                param.isInvestigation = false;
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
    $('#dlg_add_file').dialog({
        toolbar: [{
            id: 'file_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_file')) {
                    var files = getFileList();
                    var title = $('#dlg_add_file').dialog('options').title;
                    if (files != null && files.length != 1) {
                        $.messager.alert('提示', '只能上传一张公司介绍！');
                        return;
                    }
                    if ($('#file_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#file_save').linkbutton("disable"); // 禁用
                    var params = {
                        regis_code: $('#hid_regis_code').val(),
                        files: files
                    };
                    $.ajax({
                        url: title == '上传尽调记录' ? '/XFundOrganization/AddInvestigation' : '/XFundOrganization/AddIntroduce/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#file_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#file_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_file').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#company_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#file_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#file_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#company_datagrid').datagrid('reload');
    });

    $('#btn_upload_introduce') != null && $('#btn_upload_introduce').click(function() {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if (rows[0].regis_code != null && $.trim(rows[0].regis_code) != '') {
                cleanUploadFile();
                $('#hid_regis_code').val(rows[0].regis_code);
                url = "UploadXFundSliceFile";
                type = "PrivateCompanyIntroduction";
                $('#dlg_add_file').dialog({ title: '上传公司介绍' });
                $('#dlg_add_file').dialog('open');
            } else {
                $.messager.alert('提示', '该公司未备案！');
            }
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $('#btn_upload_investigation') != null && $('#btn_upload_investigation').click(function() {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if (rows[0].regis_code != null && $.trim(rows[0].regis_code) != '') {
                cleanUploadFile();
                $('#hid_regis_code').val(rows[0].regis_code);
                url = "UploadXFundSliceFile";
                type = "PrivateCompanyInvestigation";
                $('#dlg_add_file').dialog({ title: '上传尽调记录' });
                $('#dlg_add_file').dialog('open');
            } else {
                $.messager.alert('提示', '该公司未备案！');
            }
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $('#btn_clean_introduce') != null && $('#btn_clean_introduce').click(function() {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要清空公司介绍数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/XFundOrganization/CleanPrivateCompanyIntroduce/',
                        type: 'post',
                        dataType: 'json',
                        data: { regis_code: rows[0].regis_code },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '清空成功！');
                                $('#company_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '清空失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            });
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $('#btn_clean_investigation') != null && $('#btn_clean_investigation').click(function() {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要清空公司尽调数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/XFundOrganization/CleanPrivateCompanyInvestigation/',
                        type: 'post',
                        dataType: 'json',
                        data: { regis_code: rows[0].regis_code },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '清空成功！');
                                $('#company_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '清空失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            });
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });
}