$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#user_datagrid').datagrid({
        url: '/PFXFundOrganization/GetPFXFundUserListToPage/',
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
                    field: 'ispass',
                    title: '是否开通九禹系统',
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
                    field: 'special_id',
                    title: '九禹系统身份标识',
                    width: 100
                },
                {
                    field: 'isagree',
                    title: '是否同意协议',
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

                    var params = {};
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/PFXFundOrganization/AddPFXFundUser/' : '/PFXFundOrganization/UpdatePFXFundUser/',
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
}

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