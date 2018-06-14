$(function() {
    initEvent();
    initDatagrid();
})

function initEvent() {
    $('#dlg_add_depart').dialog({
        toolbar: [{
            id: 'depart_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_depart')) {
                    if ($('#depart_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#depart_save').linkbutton("disable"); // 禁用

                    var params = {
                        id: $('#hid_id').val(),
                        parent_id: $('#cmb_parent_id').combotree('getValue'),
                        name: $('#txt_name').textbox('getValue'),
                        remarks: $('#txt_remarks').textbox('getValue'),
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/System/AddDepartment/' : '/System/UpdateDepartment/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#depart_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#depart_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_depart').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#department_treegrid').treegrid('reload');
                                $('#cmb_parent_id').combotree('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#depart_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#depart_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });
}

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#department_treegrid').treegrid({
        url: '/System/GetDepartmentToList/',
        idField: 'id',
        treeField: 'name',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'name', title: '部门名称', width: 200 },
                { field: 'remarks', title: '部门备注', width: 200 },
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
        ]
    });
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_refresh') != null && $('#btn_refresh').click(function() {
        $('#department_treegrid').treegrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function() {
        cleanAddDepartDialog();
        var rows = $('#department_treegrid').treegrid('getSelections');
        if (rows != null && rows.length > 0) {
            $('#cmb_parent_id').combotree('setValue', rows[0].id);
        } else {
            $('#cmb_parent_id').combotree('setValue', 0);
        }
        $('#dlg_add_depart').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        cleanAddDepartDialog();
        var rows = $('#department_treegrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $('#hid_id').val(rows[0].id);
            $('#cmb_parent_id').combotree('setValue', rows[0].parent_id);
            $('#txt_name').textbox('setValue', rows[0].name);
            $('#txt_remarks').textbox('setValue', rows[0].remarks);
            $('#dlg_add_depart').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条部门数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#department_treegrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '删除部门数据后可能导致部门下用户无法登陆、确认删除吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/System/DeleteDepartment/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#department_treegrid').treegrid('uncheckAll');
                                $('#department_treegrid').treegrid('reload');
                                $('#cmb_parent_id').combotree('reload');
                            } else if (data != null && data.code == 406) {
                                $.messager.alert('提示', '请先删除此部门下的子部门！');
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
            $.messager.alert('提示', '请选择一条部门数据！');
        }
    });
}

/**
 * 清空新增部门弹出窗
 */
function cleanAddDepartDialog() {
    if ($('#depart_save').linkbutton("options").disabled) {
        $('#depart_save').linkbutton("enable");
    }
    $('#hid_id').val('');
    $('#cmb_parent_id').combotree('clear');
    $('#txt_name').textbox('clear');
    $('#txt_remarks').textbox('clear');
}