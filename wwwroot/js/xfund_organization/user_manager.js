$(function () {
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
                {
                    field: 'expiretime', title: '到期日期', width: 100,
                    formatter: function (value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'person_liable', title: '负责人', width: 100 },
                {
                    field: 'is_person_liable',
                    title: '是否为负责人',
                    width: 100,
                    formatter: function (value, row, index) {
                        if (value != null && value === 1) {
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
                    formatter: function (value, row, index) {
                        if (value != null && value === 1) {
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
                    formatter: function (value, row, index) {
                        if (value != null && value === 1) {
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
                    formatter: function (value, row, index) {
                        if (value != null && value === 1) {
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
                    formatter: function (value, row, index) {
                        if (value != null && value === 1) {
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
                    formatter: function (value, row, index) {
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
                    formatter: function (value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                }
            ]
        ],
        onBeforeLoad: function (param) {
            param.name_like = $('#txt_name_like').textbox('getValue') != null ? $('#txt_name_like').textbox('getValue') : ''
            param.userid_like = $('#txt_userid_like').textbox('getValue') != null ? $('#txt_userid_like').textbox('getValue') : ''
            param.company_name_like = $('#txt_company_name_like').textbox('getValue') != null ? $('#txt_company_name_like').textbox('getValue') : ''
            param.person_liable_like = $('#txt_person_liable_like').textbox('getValue') != null ? $('#txt_person_liable_like').textbox('getValue') : ''
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {

    $("#dlg_add_user").dialog({
        toolbar: [{
            id: "user_save",
            text: "保存",
            iconCls: "icon-save",
            handler: function () {
                console.log(isValidate("dlg_add_user"));
                if (isValidate("dlg_add_user")) {
                    if ($("#user_save").linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $("#user_save").linkbutton("disable"); // 禁用

                    var params = {
                        id: $("#hid_id").val(),
                        parents_id: $("#hid_parents_id").val(),

                        name: $("#txt_name").textbox("getValue"),
                        userid: $("#txt_userid").textbox("getValue"),
                        password: $("#txt_password").passwordbox("getValue"),
                        company_name: $("#txt_company_name").textbox("getValue"),
                        telephone: $("#txt_telephone").textbox("getValue"),
                        mailbox: $("#txt_mailbox").textbox("getValue"),
                        department: $("#txt_department").textbox("getValue"),
                        expiretime: $("#date_expiretime").datebox("getValue"),
                        person_liable: $("#txt_person_liable").textbox("getValue"),
                        is_person_liable: $("#cmb_is_person_liable").combobox("getValue"),
                        is_pay: $("#cmb_is_pay").combobox("getValue"),
                        status: $("#cmb_status").combobox("getValue"),
                        islogin: $("#cmb_islogin").combobox("getValue"),
                        customertype_name: $("#cmb_cus_type").combobox("getValue")


                    };
                    var roleIdAry = $("#cmb_role").combobox("getValues");
                    console.log(params.id);
                    $.ajax({
                        url: params.id == null || params.id === "" ? "/XFundOrganization/AddXFundUser/" : "/XFundOrganization/UpdateXFundUser/",
                        type: "post",
                        dataType: "json",
                        data: { mdl: params, role_id_ary: roleIdAry },
                        success: function (data) {
                            if ($("#user_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#user_save").linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code === 200) {
                                $("#dlg_add_user").dialog("close");
                                $.messager.alert("提示", "保存成功！");
                                $("#user_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "保存失败！" + data.data);
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                            if ($("#user_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#user_save").linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $("#dlg_add_users").dialog({
        toolbar: [{
            id: "users_save",
            text: "保存",
            iconCls: "icon-save",
            handler: function () {
                if (isValidate("dlg_add_users")) {
                    if ($("#users_save").linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $("#users_save").linkbutton("disable"); // 禁用

                    var params = {
                        userid: $("#txt_userid_s").textbox("getValue"),
                        startlist: $("#txt_startlist_s").textbox("getValue"),
                        endlist: $("#txt_endlist_s").textbox("getValue"),
                        password: $("#txt_password_s").passwordbox("getValue"),
                        company_name: $("#txt_company_name_s").textbox("getValue"),
                        expiretime: $("#date_expiretime_s").datebox("getValue"),
                        person_liable: $("#txt_person_liable_s").textbox("getValue")
                    };
                    var roleId = $("#cmb_role").combobox("getValue");
                    $.ajax({
                        url: "/XFundOrganization/AddBatchXFundUser/",
                        type: "post",
                        dataType: "json",
                        data: { mdls: params, role_id: roleId },
                        success: function (data) {
                            if ($("#users_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#users_save").linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code === 200) {
                                $("#dlg_add_users").dialog("close");
                                $.messager.alert("提示", "保存成功！");
                                $("#user_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "保存失败！" + data.data);
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                            if ($("#users_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#users_save").linkbutton("enable"); // 启用
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
    $("#btn_search") != null && $("#btn_search").click(function () {
        $("#user_datagrid").datagrid("reload");
    });

    $("#btn_add") != null && $("#btn_add").click(function () {
        cleanAddUserDialog();
        //获取机构版推荐用户名
        SeriesCode();
        var d = new Date();
        d.setDate(d.getDate() + 30);
        $("#date_expiretime").datebox("setValue", d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate());
        var rows = $("#user_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
            $("#hid_parents_id").val(rows[0].id);
            $("#txt_parents_name").textbox("setValue", rows[0].name);
        } else {
            $("#txt_parents_name").textbox("setValue", "--");
        }
        $("#dlg_add_user").dialog("open");
    });

    $("#btn_edit") != null && $("#btn_edit").click(function () {
        var rows = $("#user_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
            cleanAddUserDialog();
            $(".pwd").hide();
            $("#hid_id").val(rows[0].id);

            $("#hid_parents_id").val(rows[0].parents_id != null ? rows[0].parents_id : "");
            $("#txt_parents_name").textbox("setValue", rows[0].parents_id != null ? rows[0].parents_name : "--");
            $("#txt_name").textbox("setValue", rows[0].name);
            $("#txt_userid").textbox("setValue", rows[0].userid);
            $("#txt_password").passwordbox("setValue", rows[0].password);   
            $("#txt_company_name").textbox("setValue", rows[0].company_name);
            $("#txt_telephone").textbox("setValue", rows[0].telephone);
            $("#txt_mailbox").textbox("setValue", rows[0].mailbox);
            $("#txt_department").textbox("setValue", rows[0].department);
            $("#date_expiretime").datebox("setValue", rows[0].expiretime);
            $("#txt_person_liable").textbox("setValue", rows[0].person_liable);
            $("#cmb_is_person_liable").textbox("setValue", rows[0].is_person_liable);
            $("#cmb_is_pay").combobox("setValue", rows[0].is_pay);
            $("#cmb_cus_type").combobox("setValue", rows[0].customertype_name);
            $("#cmb_status").combobox("setValue", rows[0].status);
            $("#cmb_islogin").combobox("setValue", rows[0].islogin);
            $("#txt_userid").textbox("disable");
            

            if (rows[0].role_id_str != null) {
                var roleIdAry = rows[0].role_id_str.split(",");
                $("#cmb_role").combobox("setValues", roleIdAry);
            }
            $("#dlg_add_user").dialog("open");

        } else {
            $.messager.alert("提示", "请选择一条用户数据！");
        }
    });

    $("#btn_del") != null && $("#btn_del").click(function () {
        var rows = $("#user_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
            $.messager.confirm("确认", "您确认想要删除用户数据吗？", function (r) {
                if (r) {
                    $.ajax({
                        url: "/XFundOrganization/DeleteXFundUser/",
                        type: "post",
                        dataType: "json",
                        data: { id: rows[0].id },
                        success: function (data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert("提示", "删除成功！");
                                $("#user_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "删除失败！");
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                        }
                    });
                }
            });
        } else {
            $.messager.alert("提示", "请选择一条用户数据！");
        }
    });

    $("#btn_adds") != null && $("#btn_adds").click(function () {
        cleanAddUsersDialog();
        var d = new Date();
        d.setDate(d.getDate() + 30);
        $("#date_expiretime_s").datebox("setValue", d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate());
        $("#dlg_add_users").dialog("open");
    });

    $("#btn_reset") != null && $("#btn_reset").click(function () {
        var rows = $("#user_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
            $.ajax({
                url: "/XFundOrganization/UpdateUserState/",
                type: "post",
                dataType: "json",
                data: { id: rows[0].id },
                success: function (data) {
                    if (data != null && data.code === 200) {
                        $.messager.alert("提示", "修改成功！");
                        $("#user_datagrid").datagrid("reload");
                    } else {
                        $.messager.alert("提示", "修改失败！");
                    }
                },
                error: function () {
                    $.messager.alert("错误", "网络连接失败、请稍后再试！");
                }
            });
        } else {
            $.messager.alert("提示", "请选择一条用户数据！");
        }
    });
}

//获取机构版推荐的用户名
function SeriesCode() {
    $.ajax({
        url: "/XFundOrganization/GetXFundNewUserid/",
        type: "post",
        dataType: "json",
        success: function (data) {
            if (data != null && data.code === 200) {
                $("#txt_userid").textbox("setValue", data.data);
            }
        },
        error: function () { }
    });
}

function cleanAddUserDialog() {
    if ($("#user_save").linkbutton("options").disabled) {
        $("#user_save").linkbutton("enable");
    }

    $("#hid_id").val("");
    $("#hid_parents_id").val("");
    $("#txt_parents_name").textbox("clear");
    $("#txt_name").textbox("clear");
    $("#txt_userid").textbox("clear");
    $("#txt_password").passwordbox("clear");
    $("#txt_company_name").textbox("clear");
    $("#txt_telephone").textbox("clear");
    $("#txt_mailbox").textbox("clear");
    $("#txt_department").textbox("clear");
    $("#date_expiretime").datebox("clear");
    $("#txt_person_liable").textbox("clear");
    $("#cmb_role").combobox("clear");
    $("#cmb_cus_type").combobox("clear");
    $("#cmb_is_person_liable").combobox("setValue", 0);
    $("#cmb_is_pay").combobox("setValue", 1);
    $("#cmb_status").combobox("setValue", 1);
    $("#cmb_islogin").combobox("setValue", 1);


    $("#txt_userid").textbox("enable");
    $(".pwd").show();
}

function cleanAddUsersDialog() {
    if ($("#users_save").linkbutton("options").disabled) {
        $("#users_save").linkbutton("enable");
    }

    $("#txt_userid_s").textbox("clear");
    $("#txt_startlist_s").textbox("clear");
    $("#txt_endlist_s").textbox("clear");
    $("#txt_password_s").passwordbox("clear");
    $("#cmb_role_s").combobox("clear");
    $("#txt_person_liable_s").textbox("clear");
    $("#date_expiretime_s").datebox("clear");
    $("#txt_company_name_s").textbox("clear");
    $("#txt_userid_s").textbox("enable");
    $(".pwd").show();
}