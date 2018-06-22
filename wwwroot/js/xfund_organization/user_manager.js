$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#user_datagrid').datagrid({
        url: '/XFundOrganization/GetXFundUserListToPage/',
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
                { field: 'userid', title: '用户账号', width: 100 },
                { field: 'company_name', title: '公司名称', width: 200 },
                { field: 'customertype_name', title: '客户类型', width: 100 },
                { field: 'telephone', title: '联系电话', width: 100 },
                { field: 'mailbox', title: '邮箱', width: 100 },
                { field: 'department', title: '部门', width: 100 },
                { field: 'expiretime', title: '到期日期', width: 100 },
                { field: 'person_liable', title: '负责人', width: 100 },
                {
                    field: 'is_person_liable',
                    title: '是否为负责人',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:red;">是</span>';
                        } else {
                            return '否';
                        }
                    }
                },
                {
                    field: 'is_pay',
                    title: '是否为付费用户',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:red;">是</span>';
                        } else {
                            return '否';
                        }
                    }
                },
                {
                    field: 'iscompany_show',
                    title: '消息是否显示公司名称',
                    width: 130,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:red;">是</span>';
                        } else {
                            return '否';
                        }
                    }
                },
                {
                    field: 'status',
                    title: '用户状态',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:green;">可用</span>';
                        } else {
                            return '<span style="color:red;">不可用</span>';
                        }
                    }
                },
                {
                    field: 'islogin',
                    title: '是否可登录',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:green;">可用</span>';
                        } else {
                            return '<span style="color:red;">不可用</span>';
                        }
                    }
                },
                { field: 'macaddr', title: 'MAC地址', width: 100 },
                { field: 'loginip', title: '最后登录IP', width: 100 },
                {
                    field: 'lastlogin',
                    title: '最后登录日期',
                    width: 130,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'create_user_name', title: '创建人', width: 100 },
                {
                    field: 'create_time',
                    title: '创建日期',
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
            param.name_like = $('#txt_name_like').textbox('getValue') != null ? $('#txt_name_like').textbox('getValue') : ''
            param.userid_like = $('#txt_userid_like').textbox('getValue') != null ? $('#txt_userid_like').textbox('getValue') : ''
            param.company_name_like = $('#txt_company_name_like').textbox('getValue') != null ? $('#txt_company_name_like').textbox('getValue') : ''
            param.person_liable_like = $('#txt_person_liable_like').textbox('getValue') != null ? $('#txt_person_liable_like').textbox('getValue') : ''
        },
        onClickRow: function(index, row) {
            loadLineLoginNum(row.login_name, row.login_time_min, row.login_time_max);
            loadPieMenuChart(row.login_name, row.login_time_min, row.login_time_max);
            loadPieFunChart(row.login_name, row.login_time_min, row.login_time_max);
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#user_datagrid').datagrid('reload');
    });
}