$(function () {
    initEvent();
    initDatagrid();
});

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function () {
        $('#notice_datagrid').datagrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function () {
        // cleanAddUserDialog();
        uploaderReset();
        
        $('#dlg_add_notice').dialog('open');
    });
}

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#notice_datagrid').datagrid({
        url: '/ProcessAssets/GetNoticeToPage/',
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
                { field: 'title', title: '标题', width: 200 },
                {
                    field: 'is_settop',
                    title: '是否置顶',
                    width: 60,
                    formatter: function (value, row, index) {
                        if (value != null) {
                            if (value == 1) {
                                return '是';
                            } else {
                                return '否';
                            }
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'remarks', title: '备注', width: 300 },
                {
                    field: 'id',
                    title: '查看附件',
                    width: 61,
                    formatter: function (value, row, index) {
                        return '<a href="javascript:void(0);" style="text-decoration:none;">查看附件</a>';
                    }
                },
                { field: 'last_modify_name', title: '最后修改人', width: 132 },
                {
                    field: 'create_time',
                    title: '创建时间',
                    width: 130,
                    formatter: function (value, row, index) {
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
                    formatter: function (value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
            ]
        ],
        onBeforeLoad: function (param) {
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
}