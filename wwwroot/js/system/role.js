$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#role_datagrid').datagrid({
        url: '/System/GetRoleToList/',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'name', title: '角色名称', width: 100 },
                {
                    field: 'role_state',
                    title: '角色状态',
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
                {
                    field: 'data_permiss',
                    title: '数据权限',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            if (value == 0) {
                                return '全部';
                            } else if (value == 100) {
                                return '本部门及下属部门数据';
                            } else if (value == 200) {
                                return '自建数据';
                            } else {
                                return value;
                            }
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'remarks', title: '角色备注', width: 200 },
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
    $('#dlg_add_role').dialog({
        toolbar: [{
            id: 'role_save',
            text: '保存',
            iconCls: 'icon-add',
            handler: function() {
                if (isValidate('dlg_add_role')) {
                    if ($('#role_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#role_save').linkbutton("disable"); // 禁用

                    var params = {
                        id: $('#hid_id').val(),
                        name: $('#txt_name').textbox('getValue'),
                        data_permiss: $('#cmb_data_permiss').combobox('getValue'),
                        state: $('#cmb_state').combobox('getValue'),
                        remarks: $('#txt_remarks').textbox('getValue')
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/System/AddRole/' : '/System/UpdateRole/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#role_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#role_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_role').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#role_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#role_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#role_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $('#dlg_add_right').dialog({
        toolbar: [{
            id: 'right_save',
            text: '保存',
            iconCls: 'icon-add',
            handler: function() {
                if (isValidate('dlg_add_right')) {
                    if ($('#right_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#right_save').linkbutton("disable"); // 禁用
                    var role_id = $('#hid_right_id').val();
                    var data_checked = $('#tree_right').tree('getChecked');
                    var data_indeterminate = $('#tree_right').tree('getChecked', 'indeterminate');
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
                            if (data_checked[i].id.indexOf('menu_') == 0) {
                                data.push({
                                    menu_id: data_checked[i].id.substr(data_checked[i].id.indexOf('_') + 1),
                                    role_id: role_id,
                                });
                            } else {
                                data.push({
                                    menu_id: data_checked[i].attributes,
                                    btn_id: data_checked[i].id.substr(data_checked[i].id.indexOf('_') + 1),
                                    role_id: role_id,
                                });
                            }
                        }
                    }

                    $.ajax({
                        url: '/System/DistributionRight/',
                        type: 'post',
                        dataType: 'json',
                        data: { role_id: role_id, list: data },
                        success: function(data) {
                            if ($('#right_save').linkbutton("options").disabled) {
                                $('#right_save').linkbutton("enable");
                            }
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '分配成功！');
                                $('#dlg_add_right').dialog('close');
                                $('#role_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '分配失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#right_save').linkbutton("options").disabled) {
                                $('#right_save').linkbutton("enable");
                            }
                        }
                    });
                }
            }
        }]
    });

    $('#tree_right').tree({
        checkbox: true,
        lines: true
    });
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#role_datagrid').datagrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function() {
        cleanAddRoleDialog();
        $('#dlg_add_role').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        var rows = $('#role_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddRoleDialog();

            $('#hid_id').val(rows[0].id);
            $('#txt_name').textbox('setValue', rows[0].name != null ? rows[0].name : '');
            $('#cmb_data_permiss').combobox('setValue', rows[0].data_permiss != null ? rows[0].data_permiss : 0);
            $('#cmb_state').combobox('setValue', rows[0].role_state != null ? rows[0].role_state : 1);
            $('#txt_remarks').textbox('setValue', rows[0].remarks != null ? rows[0].remarks : '');

            $('#dlg_add_role').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条角色数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#role_datagrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除角色数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/System/DeleteRole/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#role_datagrid').datagrid('reload');
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
            $.messager.alert('提示', '请选择一条角色数据！');
        }
    });

    $('#btn_right') != null && $('#btn_right').click(function() {
        var rows = $('#role_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#right_save').linkbutton("options").disabled) {
                $('#right_save').linkbutton("enable");
            }
            $('#hid_right_id').val(rows[0].id);
            $('#tree_right').tree({ data: [] });
            $('#tree_right').tree({
                url: '/System/GetRoleMenuRight/',
                method: 'POST',
                animate: true,
                checkbox: true,
                queryParams: { id: rows[0].id },
                loadFilter: function(data) {
                    data = recursionChecked(data);
                    return data;
                }

            });
            $('#dlg_add_right').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条角色数据！');
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
 * 清空角色Dialog
 */
function cleanAddRoleDialog() {
    if ($('#role_save').linkbutton("options").disabled) {
        $('#role_save').linkbutton("enable");
    }

    $('#hid_id').val('');
    $('#txt_name').textbox('clear');
    $('#cmb_data_permiss').combobox('setValue', 0);
    $('#cmb_state').combobox('setValue', 1);
    $('#txt_remarks').textbox('clear');
}