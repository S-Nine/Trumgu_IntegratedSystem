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
                {
                    field: 'parents_name',
                    title: '父级账号',
                    width: 100,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value;
                        } else {
                            return '--';
                        }
                    }
                },
                {
                    field: 'is_pay',
                    title: '正式用户',
                    width: 60,
                    formatter: function(value, row, index) {
                        if (value != null && value == 1) {
                            return '<span style="color:red;">是</span>';
                        } else {
                            return '否';
                        }
                    }
                },
                { field: 'company_name', title: '公司名称', width: 200 },
                { field: 'telephone', title: '联系电话', width: 100 },
                { field: 'mailbox', title: '邮箱', width: 100 },
                { field: 'department', title: '部门', width: 100 },
                {
                    field: 'expiretime',
                    title: '到期日期',
                    width: 120,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'person_liable', title: '负责人', width: 100 },
                { field: 'role_str', title: '所属角色', width: 100 },
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
                {
                    field: 'id',
                    title: '查看附件',
                    width: 61,
                    formatter: function(value, row, index) {
                        return '<a href="javascript:void(0);" style="text-decoration:none;" onClick="seeFiles(' + (value != null ? value : '') + ');">查看附件</a>';
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
            var is_pay = $('#cmb_is_pay').combobox('getValue');
            if (is_pay != null && is_pay != -1) {
                param.is_pay = is_pay;
            }
        },
        onBeforeSelect: function(index, row) {
            var row = $('#user_datagrid').datagrid('getSelected');
            var curRowindex = $('#user_datagrid').datagrid('getRowIndex', row);
            if (curRowindex != index) { return true; } else { $('#user_datagrid').datagrid('unselectRow', index); return false; }
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {
    var buttons = $.extend([], $.fn.datebox.defaults.buttons);
    buttons.splice(1, 0, {
        text: '清空',
        handler: function(target) {
            $(target).datebox('clear');
            $(target).datebox('hidePanel');
        }
    });
    $('#date_expiretime').datebox({
        buttons: buttons
    });

    $('#dlg_upwd').dialog({
        toolbar: [{
            id: 'upwd_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                var p = $('#txt_upwd_password').passwordbox('getValue');
                var q = $('#txt_upwd_q_password').passwordbox('getValue');
                if (isValidate('dlg_upwd') && p == q) {
                    if ($('#upwd_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#upwd_save').linkbutton("disable"); // 禁用

                    $.ajax({
                        url: '/PFXFundOrganization/UPWDPFXFundUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: $('#hid_upwd_id').val(), p: p },
                        success: function(data) {
                            if ($('#upwd_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#upwd_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_upwd').dialog('close');
                                $.messager.alert('提示', '重置成功！');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '重置失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#upwd_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#upwd_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                } else {
                    $.messager.alert('提示', '两次密码不一致！');
                }
            }
        }]
    });

    $('#dlg_add_files').dialog({
        toolbar: [{
            id: 'files_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                var p = $('#txt_upwd_password').passwordbox('getValue');
                var q = $('#txt_upwd_q_password').passwordbox('getValue');
                if (isValidate('dlg_add_files') && p == q) {
                    if ($('#files_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#files_save').linkbutton("disable"); // 禁用

                    $.ajax({
                        url: '/PFXFundOrganization/UploadFilesPFXFundUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: $('#hid_files_id').val(), files: getFileList() },
                        success: function(data) {
                            if ($('#files_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#files_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_files').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#files_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#files_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                } else {
                    $.messager.alert('提示', '两次密码不一致！');
                }
            }
        }]
    });

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

                    var params = {
                        id: $('#hid_id').val(),
                        name: $('#txt_name').textbox('getValue'),
                        userid: $('#txt_userid').textbox('getValue'),
                        password: $('#txt_password').passwordbox('getValue'),
                        company_name: $('#cmb_company_name').combobox('getText'),
                        hpcompany_id: $('#cmb_company_name').combobox('getValue'),
                        telephone: $('#txt_telephone').textbox('getValue'),
                        mailbox: $('#txt_mailbox').textbox('getValue'),
                        department: $('#txt_department').textbox('getValue'),
                        expiretime: $('#date_expiretime').datebox('getValue'),
                        person_liable: $('#txt_person_liable').textbox('getValue'),
                        iscompany_show: $('#cmb_iscompany_show').combobox('getValue'),
                        status: $('#cmb_status').combobox('getValue'),
                        islogin: $('#cmb_islogin').combobox('getValue'),
                        parents_id: $('#hid_parents_id').val()
                    };
                    var role_id_ary = $('#cmb_role').combobox('getValues');
                    $.ajax({
                        url: params.id == null || params.id == '' ? '/PFXFundOrganization/AddPFXFundUser/' : '/PFXFundOrganization/UpdatePFXFundUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { mdl: params, role_id_ary: role_id_ary },
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

    $('#cmb_company_name').combobox({
        onHidePanel: function() {
            var data = $('#cmb_company_name').combobox('getData');
            var t = $('#cmb_company_name').combobox('getText');
            if (data != null && data.length > 0 && t != null) {
                for (var i = 0; i < data.length; i++) {
                    if (t == data[i].cn_name) {
                        $('#cmb_company_name').combobox('setValue', data[i].hpcompany_id);
                        break;
                    }
                }
            }
        }
    });

    $('#dlg_delay').dialog({
        toolbar: [{
            id: 'delay_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_delay')) {
                    if ($('#delay_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#delay_save').linkbutton("disable"); // 禁用

                    $.ajax({
                        url: '/PFXFundOrganization/DelayPFXFundUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: $('#hid_delay_id').val(), t: $('#date_delay_expiretime').datebox('getValue') },
                        success: function(data) {
                            if ($('#delay_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#delay_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_delay').dialog('close');
                                $.messager.alert('提示', '延期成功！');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '延期失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#delay_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#delay_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $('#dlg_formal').dialog({
        toolbar: [{
            id: 'formal_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_formal')) {
                    if ($('#formal_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#formal_save').linkbutton("disable"); // 禁用

                    $.ajax({
                        url: '/PFXFundOrganization/formalPFXFundUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: $('#hid_formal_id').val(), t: $('#date_formal_expiretime').datebox('getValue') },
                        success: function(data) {
                            if ($('#formal_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#formal_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_formal').dialog('close');
                                $.messager.alert('提示', '转正成功！');
                                $('#user_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '转正失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#formal_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#formal_save').linkbutton("enable"); // 启用
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
        $('#cmb_company_name').combobox('reload');
        $.ajax({
            url: '/PFXFundOrganization/GetPFXFundNewUserid/',
            type: 'post',
            dataType: 'json',
            success: function(data) {
                if (data != null && data.code == 200) {
                    $('#txt_userid').textbox('setValue', data.data);
                }
            },
            error: function() {}
        });
        var d = new Date();
        d.setDate(d.getDate() + 30);
        $('#date_expiretime').datebox('setValue', d.getFullYear() + '-' + (d.getMonth() + 1) + '-' + d.getDate());
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $('#hid_parents_id').val(rows[0].id);
            $('#txt_parents_name').textbox('setValue', rows[0].name);
        } else {
            $('#txt_parents_name').textbox('setValue', '--');
        }
        $('#dlg_add_user').dialog('open');
    });

    $('#btn_edit') != null && $('#btn_edit').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            cleanAddUserDialog();
            $('.pwd').hide();
            $('#hid_id').val(rows[0].id);
            $('#txt_name').textbox('setValue', rows[0].name);
            $('#txt_userid').textbox('setValue', rows[0].userid);
            $('#txt_password').passwordbox('setValue', rows[0].password);
            $('#cmb_company_name').combobox('setText', rows[0].company_name);
            $('#txt_telephone').textbox('setValue', rows[0].telephone);
            $('#txt_mailbox').textbox('setValue', rows[0].mailbox);
            $('#txt_department').textbox('setValue', rows[0].department);
            $('#date_expiretime').datebox('setValue', rows[0].expiretime);
            $('#txt_person_liable').textbox('setValue', rows[0].person_liable);
            $('#cmb_iscompany_show').combobox('setValue', rows[0].iscompany_show);
            $('#cmb_status').combobox('setValue', rows[0].status);
            $('#cmb_islogin').combobox('setValue', rows[0].islogin);
            $('#txt_userid').textbox('disable');
            $('#hid_parents_id').val(rows[0].parents_id != null ? rows[0].parents_id : '');
            $('#txt_parents_name').textbox('setValue', rows[0].parents_name != null ? rows[0].parents_name : '--');
            $('#dlg_add_user').dialog('open');
            if (rows[0].hpcompany_id != null && $.trim(rows[0].hpcompany_id) != '') {
                $.ajax({
                    url: '/PFXFundOrganization/GetPrivateCompanyNameByHPCompanyID/',
                    type: 'post',
                    dataType: 'json',
                    data: { hpcompany_id: rows[0].hpcompany_id },
                    success: function(data) {
                        if (data != null) {
                            $('#cmb_company_name').combobox('loadData', [data]);
                            $('#cmb_company_name').combobox('setValue', data.hpcompany_id);
                        }
                    }
                });
            }
            if (rows[0].role_id_str != null) {
                var role_id_ary = rows[0].role_id_str.split(',');
                $('#cmb_role').combobox('setValues', role_id_ary);
            }
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_del') != null && $('#btn_del').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要删除用户数据吗？', function(r) {
                if (r) {
                    $.ajax({
                        url: '/PFXFundOrganization/DeletePFXFundUser/',
                        type: 'post',
                        dataType: 'json',
                        data: { id: rows[0].id },
                        success: function(data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '删除成功！');
                                $('#user_datagrid').datagrid('reload');
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
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_upwd') != null && $('#btn_upwd').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#upwd_save').linkbutton("options").disabled) {
                $('#upwd_save').linkbutton("enable");
            }

            $('#p_upwd_name').html(rows[0].name != null ? rows[0].name : '');
            $('#hid_upwd_id').val(rows[0].id);
            $('#txt_upwd_password').textbox('clear');
            $('#txt_upwd_q_password').textbox('clear');
            $('#dlg_upwd').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_files') != null && $('#btn_files').click(function() {
        cleanUploadFile();
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#files_save').linkbutton("options").disabled) {
                $('#files_save').linkbutton("enable");
            }

            $.ajax({
                url: '/PFXFundOrganization/GetUserFiles/',
                type: 'post',
                dataType: 'json',
                data: { id: rows[0].id },
                success: function(data) {
                    if (data != null && data.code == 200 && data.data != null) {
                        var files = [];
                        for (var i = 0; i < data.data.length; i++) {
                            files.push({
                                fileName: data.data[i].file_name,
                                fileSize: data.data[i].file_size,
                                fileUrl: data.data[i].file_path
                            });
                        }
                        setHistoryFileList(files);
                    }
                }
            });

            $('#hid_files_id').val(rows[0].id);
            $('#dlg_add_files').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_delay') != null && $('#btn_delay').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#delay_save').linkbutton("options").disabled) { // 防止重复提交数据
                $('#delay_save').linkbutton("enable"); // 启用
            }

            $('#hid_delay_id').val(rows[0].id);
            $('#p_delay_name').html(rows[0].name != null ? rows[0].name : '');
            $('#date_delay_expiretime').datebox('clear');
            var v = null;
            if (rows[0].expiretime != null && $.trim(rows[0].expiretime) != '') {
                v = rows[0].expiretime.replace('T', ' ');
            }

            $('#date_delay_expiretime').datebox().datebox('calendar').calendar({
                validator: function(date) {
                    var now = null;
                    if (v != null && $.trim(v) != '') {
                        now = new Date(v);
                    } else {
                        now = new Date();
                    }
                    var d1 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    var d2 = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 30);
                    return d1 <= date && date <= d2;
                }
            });
            if (v != null && v != '') {
                $('#date_delay_expiretime').datebox('setValue', v);
            }

            $('#dlg_delay').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });

    $('#btn_formal') != null && $('#btn_formal').click(function() {
        var rows = $('#user_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if ($('#formal_save').linkbutton("options").disabled) { // 防止重复提交数据
                $('#formal_save').linkbutton("enable"); // 启用
            }

            $('#hid_formal_id').val(rows[0].id);
            $('#p_formal_name').html(rows[0].name != null ? rows[0].name : '');
            $('#date_formal_expiretime').datebox('clear');

            $('#dlg_formal').dialog('open');
        } else {
            $.messager.alert('提示', '请选择一条用户数据！');
        }
    });
}

function cleanAddUserDialog() {
    if ($('#user_save').linkbutton("options").disabled) {
        $('#user_save').linkbutton("enable");
    }

    $('#hid_id').val('');
    $('#hid_parents_id').val('');
    $('#txt_parents_name').textbox('clear');
    $('#txt_name').textbox('clear');
    $('#txt_userid').textbox('clear');
    $('#txt_password').passwordbox('clear');
    $('#cmb_company_name').combobox('clear');
    $('#txt_telephone').textbox('clear');
    $('#txt_mailbox').textbox('clear');
    $('#txt_department').textbox('clear');
    $('#date_expiretime').datebox('clear');
    $('#cmb_person_liable').combobox('clear');
    $('#cmb_is_person_liable').combobox('setValue', 0);
    $('#cmb_iscompany_show').combobox('setValue', 1);
    $('#cmb_status').combobox('setValue', 1);
    $('#cmb_islogin').combobox('setValue', 1);
    $('#cmb_role').combobox('clear');

    $('#txt_userid').textbox('enable');
    $('.pwd').show();
}


/**
 * 根据主键查询附件信息
 * @param {主键} id 
 */
function seeFiles(id) {
    $('#dlg_files').dialog('open');

    $('#file_datagrid').datagrid({
        url: '/PFXFundOrganization/GetUserFileList/',
        striped: true, // 是否显示斑马线效果。
        loadMsg: '正在加载ing...', // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: 'file_name', title: '名称', width: 453 },
                {
                    field: 'file_size',
                    title: '大小',
                    align: 'right',
                    halign: 'center',
                    width: 80,
                    formatter: function(value, row, index) {
                        return (value != null ? (value / (1024 * 1024)).toFixed(2) : '') + 'MB';
                    }
                },
                {
                    field: 'file_path',
                    title: '操作',
                    width: 40,
                    formatter: function(value, row, index) {
                        return '<a href="/Public/DownloadFile?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">下载</a>';
                    }
                }
            ]
        ],
        onBeforeLoad: function(param) {
            param.id = id
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}