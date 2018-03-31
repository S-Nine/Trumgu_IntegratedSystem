$(function() {
    initEvent();
    initDatagrid();
})

/**
 * 初始化事件
 */
function initEvent() {

}

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
                { field: 'role_state', title: '角色状态', width: 100 },
                { field: 'data_permiss', title: '数据权限', width: 100 },
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
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {

}