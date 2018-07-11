$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#user_datagrid').datagrid({
        url: '/System/GetUserToPage/',
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
                { field: 'name', title: '用户名称', width: 100 },
                { field: 'login_name', title: '登录名称', width: 100 },
                { field: 'email', title: '联系邮箱', width: 100 },
                { field: 'tel', title: '联系电话', width: 100 },
                {
                    field: 'gender',
                    title: '用户性别',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            if (value == 1) {
                                return '男';
                            } else {
                                return '女';
                            }
                        } else {
                            return '';
                        }
                    }
                },
                {
                    field: 'state',
                    title: '用户状态',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            if (value == 1) {
                                return '启用';
                            } else {
                                return '禁用';
                            }
                        } else {
                            return '';
                        }
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
            param.name_like = $('#txt_name_like').textbox('getValue') != null ? $('#txt_name_like').textbox('getValue') : ''
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {
    $('#dlg_add_user').dialog({
        toolbar: [{
            id: 'user_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_user')) {
                    if ($('#user_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#user_save').linkbutton("disable"); // 禁用

                    var params = {
                        id: $('#hid_id').val(),
                        name: $('#txt_name').textbox('getValue'),
                        login_name: $('#txt_login_name').textbox('getValue'),
                        login_pwd: $('#txt_login_pwd').passwordbox('getValue'),
                        email: $('#txt_email').textbox('getValue'),
                        tel: $('#txt_tel').textbox('getValue'),
                        gender: $('#cmb_gender').combobox('getValue'),
                        state: $('#cmb_state').combobox('getValue')
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/System/AddUser/' : '/System/UpdateUser/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#user_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#user_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_user').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#user_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#user_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $('#dlg_add_delp').dialog({
        toolbar: [{
            id: 'delp_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_delp')) {
                    if ($('#delp_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#delp_save').linkbutton("disable"); // 禁用
                    var user_id = $('#hid_user_id').val();
                    var data_checked = $('#tree_delp').tree('getChecked');
                    var data_indeterminate = $('#tree_delp').tree('getChecked', 'indeterminate');
                    if (data_checked == null) {
                        data_checked = [];
                    }
                    if (data_indeterminate != null && data_indeterminate.length > 0) {
                        for (var i = 0; i < data_indeterminate.length; i++) {
                            data_checked.push(data_indeterminate[i]);
                        }
                    }

                    var data = [];
                    if (data_checked != null) {
                        for (var i = 0; i < data_checked.length; i++) {
                            data.push(data_checked[i].id);
                        }
                    }

                    $.ajax({
                        url: '/System/DistributionUserDelp/',
                        type: 'post',
                        dataType: 'json',
                        data: { user_id: user_id, list: data },
                        success: function(data) {
                            if ($('#delp_save').linkbutton("options").disabled) {
                                $('#delp_save').linkbutton("enable");
                            }
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '分配成功！');
                                $('#dlg_add_delp').dialog('close');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '分配失败！');
                            }
                        },
                        error: function() {
                            if ($('#delp_save').linkbutton("options").disabled) {
                                $('#delp_save').linkbutton("enable");
                            }
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            }
        }]
    });

    $('#dlg_add_role').dialog({
        toolbar: [{
            id: 'role_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_role')) {
                    if ($('#role_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#role_save').linkbutton("disable"); // 禁用
                    var user_id = $('#hid_user_id2').val();
                    var data_checked = $('#tree_role').tree('getChecked');
                    var data_indeterminate = $('#tree_role').tree('getChecked', 'indeterminate');
                    if (data_checked == null) {
                        data_checked = [];
                    }
                    if (data_indeterminate != null && data_indeterminate.length > 0) {
                        for (var i = 0; i < data_indeterminate.length; i++) {
                            data_checked.push(data_indeterminate[i]);
                        }
                    }

                    var data = [];
                    if (data_checked != null) {
                        for (var i = 0; i < data_checked.length; i++) {
                            data.push(data_checked[i].id);
                        }
                    }

                    $.ajax({
                        url: '/System/DistributionUserRole/',
                        type: 'post',
                        dataType: 'json',
                        data: { user_id: user_id, list: data },
                        success: function(data) {
                            if ($('#role_save').linkbutton("options").disabled) {
                                $('#role_save').linkbutton("enable");
                            }
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '分配成功！');
                                $('#dlg_add_role').dialog('close');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '分配失败！');
                            }
                        },
                        error: function() {
                            if ($('#role_save').linkbutton("options").disabled) {
                                $('#role_save').linkbutton("enable");
                            }
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            }
        }]
    });

    $('#tree_role').tree({
        url: '/System/GetUserRole/',
        method: 'POST',
        animate: true,
        checkbox: true,
        lines: true,
        loadFilter: function(data) {
            data = recursionChecked(data);
            return data;
        }
    });
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#user_datagrid').datagrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function() {
        cleanAddUserDialog();
        $('#dlg_add_user').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddUserDialog();

            $('#hid_id').val(rows[0].id);
            $('#txt_name').textbox('setValue', rows[0].name != null ? rows[0].name : '');
            $('#txt_login_name').textbox('setValue', rows[0].login_name != null ? rows[0].login_name : '');
            $('#txt_login_pwd').passwordbox('setValue', rows[0].login_pwd != null ? rows[0].login_pwd : '');
            $('#txt_email').textbox('setValue', rows[0].email != null ? rows[0].email : '');
            $('#txt_tel').textbox('setValue', rows[0].tel != null ? rows[0].tel : '');
            $('#cmb_gender').combobox('setValue', rows[0].gender != null ? rows[0].gender : 0);
            $('#cmb_state').combobox('setValue', rows[0].state != null ? rows[0].state : 1);
            $('#txt_login_name').textbox('disable');
            $('#dlg_add_user').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#user_datagrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除用户数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/System/DeleteUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#user_datagrid').datagrid('reload');
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
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_delpment') != null && $('#btn_delpment').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#delp_save').linkbutton("options").disabled) {
                $('#delp_save').linkbutton("enable");
            }
            $('#hid_user_id').val(rows[0].id);
            $('#tree_delp').tree({ data: [] });
            $('#tree_delp').tree({
                url: '/System/GetUserDelpartment/',
                method: 'POST',
                animate: true,
                checkbox: true,
                queryParams: { id: rows[0].id },
                loadFilter: function(data) {
                    data = recursionChecked(data);
                    return data;
                }

            });
            $('#dlg_add_delp').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_role') != null && $('#btn_role').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#role_save').linkbutton("options").disabled) {
                $('#role_save').linkbutton("enable");
            }
            $('#hid_user_id2').val(rows[0].id);
            $('#tree_role').tree({
                queryParams: { id: rows[0].id }
            });
            $('#dlg_add_role').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });
}

/**
 * core中class声明checked属性报错，故此处需要递归转化
 * @param {Tree数据} data 
 */
function recursionChecked(data) {
    if (data != null) {
        for (var i = 0; i < data.length; i++) {
            data[i].checked = data[i].check;
            if (data[i].children != null && data[i].children.length > 0) {
                data[i].children = recursionChecked(data[i].children)

                for (var j = 0; j < data[i].children.length; j++) {
                    if (!data[i].children[j].checked) {
                        data[i].checked = false;
                        break;
                    }
                }
            }
        }
    }
    return data;
}

/**
 * 清空编辑用户弹出框
 */
function cleanAddUserDialog() {
    if ($('#user_save').linkbutton("options").disabled) {
        $('#user_save').linkbutton("enable");
    }

    $('#hid_id').val('');
    $('#txt_name').textbox('clear');
    $('#txt_login_name').textbox('clear');
    $('#txt_login_pwd').passwordbox('clear');
    $('#txt_email').textbox('clear');
    $('#txt_tel').textbox('clear');
    $('#cmb_gender').combobox('setValue', 0);
    $('#cmb_state').combobox('setValue', 1);

    $('#txt_login_name').textbox('enable');
}