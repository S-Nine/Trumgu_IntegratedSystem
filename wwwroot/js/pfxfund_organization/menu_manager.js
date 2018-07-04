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
        url: '/PFXFundOrganization/GetMenuToList/',
        idField: 'id',
        treeField: 'menu',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'menu', title: '菜单名称', width: 200 },
                { field: 'code', title: '菜单编码', width: 200 },
                {
                    field: 'menu_level',
                    title: '菜单等级',
                    width: 100
                },
                {
                    field: 'status',
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

                { field: 'pathweb', title: '菜单路径', width: 200 },
                {
                    field: 'path',
                    title: '图标',
                    width: 100
                },
                { field: 'seq', title: '菜单排序', width: 60, }
            ]
        ],
        onClickRow: function(row) {
            $('#button_datagrid').datagrid('loadData', { total: 0, rows: [] });
            if (row.id != null && $.trim(row.id) != '' && !isNaN(row.id)) {
                $('#button_datagrid').datagrid({ url: '/PFXFundOrganization/GetMenuButton/', queryParams: { menu_id: row.id } });
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
            [
                { field: 'btn_js_id', title: '按钮Code', width: 100 },
                { field: 'btn_name', title: '按钮名称', width: 100 },
                { field: 'sort', title: '按钮排序', width: 60 }
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
                        menu: $('#txt_name').textbox('getValue'),
                        pathweb: $('#txt_path').textbox('getValue'),
                        path: $('#txt_icon').textbox('getValue'),
                        menu_level: $('#cmb_parent_id').combo('getValue') != 0 ? 2 : 1,
                        seq: $('#txt_sort').textbox('getValue'),
                        parent_id: $('#cmb_parent_id').combo('getValue'),
                        status: $('#cmb_state').combo('getValue'),
                        code: $('#txt_code').textbox('getValue')
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/PFXFundOrganization/AddMenu/' : '/PFXFundOrganization/UpdateMenu/',
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
                        btn_js_id: $('#txt_btn_code').textbox('getValue'),
                        btn_name: $('#txt_btn_name').textbox('getValue'),
                        sort: $('#txt_btn_sort').textbox('getValue')
                    };
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/PFXFundOrganization/AddButton/' : '/PFXFundOrganization/UpdateButton/',
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
                                $('#button_datagrid').datagrid({ url: '/PFXFundOrganization/GetMenuButton/', queryParams: { menu_id: params.menu_id } });
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
            $('#txt_name').textbox('setValue', rows[0].menu != null ? rows[0].menu : '');
            $('#txt_icon').textbox('setValue', rows[0].path != null ? rows[0].path : '');
            $('#txt_sort').textbox('setValue', rows[0].seq != null ? rows[0].seq : '');
            $('#cmb_state').combobox('setValue', rows[0].status != null ? rows[0].status : 1);
            $('#txt_path').textbox('setValue', rows[0].pathweb != null ? rows[0].pathweb : '');
            $('#txt_code').textbox('setValue', rows[0].code != null ? rows[0].code : '');

            var data = $('#cmb_parent_id').combobox('getData');
            if (rows[0].classes != null && rows[0].classes != rows[0].id) {
                if (data != null && data.length > 0) {
                    var parent_id = rows[0].classes.split('.')[0];
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].id == parent_id) {
                            $('#cmb_parent_id').combobox('setValue', parent_id);
                            break;
                        }
                        if (i == data.length - 1) {
                            $('#cmb_parent_id').combobox('setValue', 0);
                        }
                    }
                } else {
                    $('#cmb_parent_id').combobox('setValue', 0);
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
                        url: '/PFXFundOrganization/DeleteMenu/',
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
            $('#txt_btn_menu_name').textbox('setValue', rows[0].menu);
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

            $('#txt_btn_menu_name').textbox('setValue', rows_menu[0].menu != null ? rows_menu[0].menu : '');
            $('#txt_btn_code').textbox('setValue', rows_button[0].btn_js_id != null ? rows_button[0].btn_js_id : '');
            $('#txt_btn_name').textbox('setValue', rows_button[0].btn_name != null ? rows_button[0].btn_name : '');
            $('#txt_btn_sort').textbox('setValue', rows_button[0].sort != null ? rows_button[0].sort : '');

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
                        url: '/PFXFundOrganization/DeleteButton/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#button_datagrid').datagrid({ url: '/PFXFundOrganization/GetMenuButton/', queryParams: { menu_id: rows_menu[0].id } });
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
    $('#txt_code').textbox('clear');
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
    $('#txt_btn_sort').textbox('clear');
}