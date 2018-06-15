$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#cipher_dategrid').datagrid({
        url: '/WorkAssistant/GetCipherThinToPage/',
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
                { field: 'title', title: '标题', width: 100 },
                { field: 'account_number', title: '账号', width: 100 },
                { field: 'account_pwd', title: '密码', width: 100 },
                { field: 'account_url', title: '网址', width: 100 },
                { field: 'account_email', title: '邮箱', width: 100 },
                { field: 'account_tel', title: '电话', width: 100 },
                {
                    field: 'account_register_date',
                    title: '注册日期',
                    width: 130,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'remarks', title: '备注', width: 300 },
                {
                    field: 'id',
                    title: '查看Q&A',
                    width: 61,
                    formatter: function(value, row, index) {
                        return '<a href="javascript:void(0);" style="text-decoration:none;" onClick="seeQA(' + (value != null ? value : '') + ');">查看Q&A</a>';
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
                }
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
    $('#dlg_add_cipher').dialog({
        toolbar: [{
            id: 'cipher_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_cipher')) {
                    if ($('#cipher_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#cipher_save').linkbutton("disable"); // 禁用
                    var qa = [];
                    for (var i = 0; i < $('.q').length; i++) {
                        var tQV = $($('.q')[i]).textbox('getValue');
                        var tAV = $($('.a')[i]).textbox('getValue');

                        if ((tQV != null && $.trim(tQV) != '') || (tAV != null && $.trim(tAV) != '')) {
                            qa.push({
                                question: tQV,
                                answer: tAV
                            });
                        }
                    }
                    var params = {
                        id: $('#hid_id').val(),
                        title: $('#txt_title').textbox('getValue'),
                        account_number: $('#txt_account_number').textbox('getValue'),
                        account_pwd: $('#txt_account_pwd').textbox('getValue'),
                        account_url: $('#txt_account_url').textbox('getValue'),
                        account_email: $('#txt_account_email').textbox('getValue'),
                        account_tel: $('#txt_account_tel').textbox('getValue'),
                        account_register_date: $('#txt_account_register_date').datebox('getValue'),
                        remarks: $('#txt_remarks').textbox('getValue'),
                        qa: qa
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/WorkAssistant/AddCipher/' : '/WorkAssistant/UpdateCipher/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#cipher_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#cipher_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_cipher').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#cipher_dategrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#cipher_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#cipher_save').linkbutton("enable"); // 启用
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
        $('#cipher_dategrid').datagrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function() {
        cleanCipherThinDialog();
        $('#dlg_add_cipher').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        var rows = $('#cipher_dategrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanCipherThinDialog();

            $('#hid_id').val(rows[0].id);
            $('#txt_title').textbox('setValue', rows[0].title != null ? rows[0].title : '');
            $('#txt_account_number').textbox('setValue', rows[0].account_number != null ? rows[0].account_number : '');
            $('#txt_account_pwd').textbox('setValue', rows[0].account_pwd != null ? rows[0].account_pwd : '');
            $('#txt_account_url').textbox('setValue', rows[0].account_url != null ? rows[0].account_url : '');
            $('#txt_account_email').textbox('setValue', rows[0].account_email != null ? rows[0].account_email : '');
            $('#txt_account_tel').textbox('setValue', rows[0].account_tel != null ? rows[0].account_tel : 0);
            $('#txt_account_register_date').datebox('setValue', rows[0].account_register_date != null ? rows[0].account_register_date : '');
            $('#txt_remarks').textbox('setValue', rows[0].remarks != null ? rows[0].remarks : 0);
            $('#dlg_add_cipher').dialog('open');

            $.ajax({
                url: '/WorkAssistant/GetCipherThinSecurity/',
                type: 'post',
                dataType: 'json',
                data: { id: rows[0].id },
                success: function(data) {
                    if (data != null && data.code == 200 && data.data != null && data.data.length > 0) {
                        for (var i = 0; i < data.data.length; i++) {
                            if (i != 0) {
                                addQA();
                            }
                            $($('.qa .q')[i]).textbox('setValue', data.data[i].question);
                            $($('.qa .a')[i]).textbox('setValue', data.data[i].answer);
                        }
                    }
                }
            });
        } else {
            $.messager.alert('提示', '请选择一条密码数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#cipher_dategrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除密码数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/WorkAssistant/DeleteCipher/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#cipher_dategrid').datagrid('reload');
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
            $.messager.alert('提示', '请选择一条密码数据！');
        }
    });
}

function cleanCipherThinDialog() {
    if ($('#cipher_save').linkbutton("options").disabled) {
        $('#cipher_save').linkbutton("enable");
    }

    $('#hid_id').val('');
    $('#txt_title').textbox('clear');
    $('#txt_account_number').textbox('clear');
    $('#txt_account_pwd').textbox('clear');
    $('#txt_account_url').textbox('clear');
    $('#txt_account_email').textbox('clear');
    $('#txt_account_tel').textbox('clear');
    $('#txt_account_register_date').datebox('clear');
    $('#txt_remarks').textbox('clear');
    $('.more-qa') != null && $('.more-qa').length > 0 && $('.more-qa').remove();
    $($('.qa .q')).textbox('clear');
    $($('.qa .a')).textbox('clear');
}

/**
 * 添加Q&A
 */
function addQA() {
    var qa = $('.qa');
    $(qa[qa.length - 1]).parent().append("<div class=\"qa more-qa\" style=\"margin-top:5px;\">" +
        " <span>Q：</span>" +
        " <input class=\"q easyui-textbox easyui-validatebox\" data-options=\"validType:[\'length[0,100]\']\" style=\"width:100px\">&nbsp;&nbsp;&nbsp;" +
        " <span>A：</span>" +
        " <input class=\"a easyui-textbox easyui-validatebox\" data-options=\"validType:[\'length[0,100]\']\" style=\"width:100px\">" +
        " <a href=\"#\" onclick=\"removeQA(this);\" class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-remove',plain:true\"></a>" +
        " </div>");
    $.parser.parse('.qa');
}

/**
 * 移除Q&A
 */
function removeQA(t) {
    $(t).parent().remove();
}

function seeQA(id) {
    $('#dlg_qa').dialog('open');

    $('#qa_datagrid').datagrid({
        url: '/WorkAssistant/GetCipherThinSecurityToList/',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'question', title: 'Q', width: 285 },
                { field: 'answer', title: 'A', width: 285 }
            ]
        ],
        onBeforeLoad: function(param) {
            param.id = id
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}