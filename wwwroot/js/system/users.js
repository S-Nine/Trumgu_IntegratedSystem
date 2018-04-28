$(function() {
    // initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#users_datagrid').datagrid({
        url: '/System/GetUserToPage/',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: true, // 启用分页
        pageNumber:1,
        pageSize:15,
        pageList:[15,30,50],
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'name', title: '用户名称', width: 100 },
                { field: 'login_name', title: '登录名称', width: 100 },
                { field: 'email', title: '邮箱', width: 100 },
                { field: 'tel', title: '联系电话', width: 100 },
                { field: 'gender', title: '性别', width: 100,
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
                } },
                {
                    field: 'state',
                    title: '状态',
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
            console.log(param);
            param.name_like = $('#txt_name_like').textbox('getValue') != null ? $('#txt_name_like').textbox('getValue') : ''
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#users_datagrid').datagrid('reload');
    });
}