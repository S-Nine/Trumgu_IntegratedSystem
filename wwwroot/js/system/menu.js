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
            id: 'addId',
            text: '保存',
            iconCls: 'icon-add',
            handler: function() {
                if ($('#addId').linkbutton("options").disabled) { // 防止重复提交数据
                    return;
                }
                $('#addId').linkbutton("disable"); // 禁用
                if (isValidate('dlg_add_menu')) {
                    var params = {
                        name: $('#txt_name').textbox('getValue'),
                        path: $('#txt_path').textbox('getValue'),
                        icon: $('#txt_icon').textbox('getValue'),
                        level: $('#cmb_parent_id').combo('getValue') != 0 ? 2 : 1,
                        sort: $('#txt_sort').textbox('getValue'),
                        parent_id: $('#cmb_parent_id').combo('getValue'),
                        state: $('#cmb_state').combo('getValue')
                    };
                    $.ajax({
                        url: '/System/AddMenu/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#addId').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#addId').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_menu').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#button_datagrid').datagrid('loadData', { total: 0, rows: [] });
                                $('#menu_treegrid').treegrid('unselectAll');
                                $('#menu_treegrid').treegrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#addId').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#addId').linkbutton("enable"); // 启用
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
        $('#dlg_add_menu').dialog('open');
        if ($('#addId').linkbutton("options").disabled) {
            $('#addId').linkbutton("enable");
        }
    });
}