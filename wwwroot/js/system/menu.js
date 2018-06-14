$(function() {
    initEvent();
    initMenuTreegrid();
    initMenuButton();
})

/**
 * 初始化菜单控件&加载数据
 */
function initMenuTreegrid() {
    $('#menu_treegrid').treegrid({
        url: '/System/GetMenuToList/',
        idField: 'id',
        treeField: 'name',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [{
                    field: 'icon',
                    title: '图标',
                    width: 32,
                    formatter: function(value, row, index) {
                        return '<div style="width:16px;margin-left:3px;height:16px;" class="' + (value != null ? value : '') + '"></div>';
                    }
                },
                { field: 'name', title: '菜单名称', width: 200 },
                {
                    field: 'level',
                    title: '菜单等级',
                    width: 100
                },
                {
                    field: 'btn_state',
                    title: '菜单状态',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:green;">启用</span>';
                        } else {
                            return '<span style="color:red">禁用</span>';
                        }
                    }
                },
                { field: 'sort', title: '菜单排序', width: 60, },
                { field: 'path', title: '菜单路径', width: 200 },
                { field: 'last_modify_name', title: '最后修改人', width: 130 },
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
        onClickRow: function(row) {
            $('#button_datagrid').datagrid('loadData', { total: 0, rows: [] });
            if (row.id != null && $.trim(row.id) != '' && !isNaN(row.id)) {
                $('#button_datagrid').datagrid({ url: '/System/GetMenuButton/', queryParams: { menu_id: row.id } });
            }
        }
    });
}

/**
 * 初始化菜单按钮控件
 */
function initMenuButton() {
    $('#button_datagrid').datagrid({
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [{
                    field: 'btn_img',
                    title: '图标',
                    width: 32,
                    formatter: function(value, row, index) {
                        return '<div style="width:16px;margin-left:3px;height:16px;" class="' + (value != null ? value : '') + '"></div>';
                    }
                },
                { field: 'btn_code', title: '按钮Code', width: 100 },
                { field: 'btn_name', title: '按钮名称', width: 100 },
                { field: 'btn_sort', title: '按钮排序', width: 60 },
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
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {
    $('#dlg_add_menu').dialog({
        toolbar: [{
            id: 'menu_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_menu')) {
                    if ($('#menu_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#menu_save').linkbutton("disable"); // 禁用

                    var params = {
                        id: $('#hid_menu_id').val(),
                        name: $('#txt_name').textbox('getValue'),
                        path: $('#txt_path').textbox('getValue'),
                        icon: $('#txt_icon').textbox('getValue'),
                        level: $('#cmb_parent_id').combo('getValue') != 0 ? 2 : 1,
                        sort: $('#txt_sort').textbox('getValue'),
                        parent_id: $('#cmb_parent_id').combo('getValue'),
                        state: $('#cmb_state').combo('getValue')
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/System/AddMenu/' : '/System/UpdateMenu/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#menu_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#menu_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_menu').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#button_datagrid').datagrid('loadData', { total: 0, rows: [] });
                                $('#menu_treegrid').treegrid('unselectAll');
                                $('#menu_treegrid').treegrid('reload');
                                $('#cmb_parent_id').combobox('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#menu_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#menu_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $('#dlg_add_button').dialog({
        toolbar: [{
            id: 'button_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_add_button')) {
                    if ($('#button_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#button_save').linkbutton("disable"); // 禁用

                    var params = {
                        id: $('#hid_button_id').val(),
                        menu_id: $('#hid_btn_menu_id').val(),
                        btn_code: $('#txt_btn_code').textbox('getValue'),
                        btn_name: $('#txt_btn_name').textbox('getValue'),
                        btn_img: $('#txt_btn_img').textbox('getValue'),
                        btn_sort: $('#txt_btn_sort').textbox('getValue')
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/System/AddButton/' : '/System/UpdateButton/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#button_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#button_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_button').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#button_datagrid').datagrid({ url: '/System/GetMenuButton/', queryParams: { menu_id: params.menu_id } });
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#button_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#button_save').linkbutton("enable"); // 启用
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
    $('#btn_refresh') != null && $('#btn_refresh').click(function() {
        $('#button_datagrid').datagrid('loadData', { total: 0, rows: [] });
        $('#menu_treegrid').treegrid('unselectAll');
        $('#menu_treegrid').treegrid('reload');
    });

    $('#btn_add_menu') != null && $('#btn_add_menu').click(function() {
        cleanAddMenuDialog();
        $('#dlg_add_menu').dialog('open');

        var rows = $('#menu_treegrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            var data = $('#cmb_parent_id').combobox('getData');
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].id == rows[0].id) {
                        $('#cmb_parent_id').combobox('setValue', rows[0].id);
                        return;
                    }
                }
            }
            $('#cmb_parent_id').combobox('setValue', 0);
        }
    });

    $('#btn_edit_menu') != null && $('#btn_edit_menu').click(function() {
        var rows = $('#menu_treegrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddMenuDialog();

            $('#hid_menu_id').val(rows[0].id);
            $('#txt_name').textbox('setValue', rows[0].name != null ? rows[0].name : '');
            $('#txt_icon').textbox('setValue', rows[0].icon != null ? rows[0].icon : '');
            $('#txt_sort').textbox('setValue', rows[0].sort != null ? rows[0].sort : '');
            $('#cmb_state').combobox('setValue', rows[0].btn_state != null ? rows[0].btn_state : 1);
            $('#txt_path').textbox('setValue', rows[0].path != null ? rows[0].path : '');

            var data = $('#cmb_parent_id').combobox('getData');
            if (data != null && data.length > 0 && rows[0].parent_id != null) {
                for (var i = 0; i < data.length; i++) {
                    if (data[i].id == rows[0].parent_id) {
                        $('#cmb_parent_id').combobox('setValue', rows[0].parent_id);
                        break;
                    }
                    if (i == data.length - 1) {
                        $('#cmb_parent_id').combobox('setValue', 0);
                    }
                }
            } else {
                $('#cmb_parent_id').combobox('setValue', 0);
            }

            $('#dlg_add_menu').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条菜单数据！');
        }
    });

    $('#btn_del_menu') != null && $('#btn_del_menu').click(function() {
        var rows = $('#menu_treegrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除菜单数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/System/DeleteMenu/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#button_datagrid').datagrid('loadData', { total: 0, rows: [] });
                                $('#menu_treegrid').treegrid('unselectAll');
                                $('#menu_treegrid').treegrid('reload');
                                $('#cmb_parent_id').combobox('reload');
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
            $.messager.alert('提示', '请选择一条菜单数据！');
        }
    });

    $('#btn_add_button') != null && $('#btn_add_button').click(function() {
        var rows = $('#menu_treegrid').treegrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddButtonDialog();
            $('#hid_btn_menu_id').val(rows[0].id);
            $('#txt_btn_menu_name').textbox('setValue', rows[0].name);
            $('#dlg_add_button').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条菜单数据！');
        }
    });

    $('#btn_edit_button') != null && $('#btn_edit_button').click(function() {
        var rows_menu = $('#menu_treegrid').treegrid('getSelections');
        var rows_button = $('#button_datagrid').datagrid('getSelections');
        if (rows_button != null && rows_button.length == 1) {
            cleanAddButtonDialog();

            $('#hid_button_id').val(rows_button[0].id);
            $('#hid_btn_menu_id').val(rows_menu[0].id);

            $('#txt_btn_menu_name').textbox('setValue', rows_menu[0].name != null ? rows_menu[0].name : '');
            $('#txt_btn_code').textbox('setValue', rows_button[0].btn_code != null ? rows_button[0].btn_code : '');
            $('#txt_btn_name').textbox('setValue', rows_button[0].btn_name != null ? rows_button[0].btn_name : '');
            $('#txt_btn_img').textbox('setValue', rows_button[0].btn_img != null ? rows_button[0].btn_img : '');
            $('#txt_btn_sort').textbox('setValue', rows_button[0].btn_sort != null ? rows_button[0].btn_sort : '');

            $('#dlg_add_button').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条按钮数据！');
        }
    });

    $('#btn_del_button') != null && $('#btn_del_button').click(function() {
        var rows_menu = $('#menu_treegrid').treegrid('getSelections');
        var rows = $('#button_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除按钮数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/System/DeleteButton/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#button_datagrid').datagrid({ url: '/System/GetMenuButton/', queryParams: { menu_id: rows_menu[0].id } });
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
            $.messager.alert('提示', '请选择一条按钮数据！');
        }
    });
}

/**
 * 清空添加菜单Dialog
 */
function cleanAddMenuDialog() {
    if ($('#menu_save').linkbutton("options").disabled) {
        $('#menu_save').linkbutton("enable");
    }
    $('#hid_menu_id').val('');
    $('#cmb_parent_id').combobox('setValue', 0);
    $('#txt_name').textbox('clear');
    $('#txt_icon').textbox('clear');
    $('#txt_sort').textbox('clear');
    $('#cmb_state').combobox('setValue', 1);
    $('#txt_path').textbox('clear');
}

/**
 * 清空添加按钮Dialog
 */
function cleanAddButtonDialog() {
    if ($('#button_save').linkbutton("options").disabled) {
        $('#button_save').linkbutton("enable");
    }

    $('#hid_button_id').val('');
    $('#hid_btn_menu_id').val('');
    $('#txt_btn_menu_name').val('');
    $('#txt_btn_code').textbox('clear');
    $('#txt_btn_name').textbox('clear');
    $('#txt_btn_img').textbox('clear');
    $('#txt_btn_sort').textbox('clear');
}