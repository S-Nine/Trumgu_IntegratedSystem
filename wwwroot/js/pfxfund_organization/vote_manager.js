$(function () {
    moment.locale("zh-cn");
    initEvent();
    initDatagrid();
    initOption();
});


/**
 * 初始化数据列表
 */
function initDatagrid() {
    $("#vote_datagrid").datagrid({
        url: "/PFXFundOrganization/GetVoteToList/",
        striped: true, // 是否显示斑马线效果。
        loadMsg: "正在加载ing...", // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: "vote_title", title: "投票标题", width: 100 },
                { field: "vote_content", title: "投票内容", width: 100 },
                {
                    field: "vote_createdate", title: "创建时间", width: 100,
                    formatter: function (value, row, index) {
                        return moment(value).format("LL");
                    }
                },
                {
                    field: "vote_startdate", title: "开始时间", width: 100,
                    formatter: function (value, row, index) {
                        return moment(value).format("LL");
                    }
                },
                {
                    field: "vote_enddate", title: "结束时间", width: 100,
                    formatter: function (value, row, index) {
                        return moment(value).format("LL");
                    }
                },
                {
                    field: "vote_checktype", title: "选中类型", width: 100,
                    formatter: function (value, row, index) {
                        if (value === 0) {
                            return "多选";
                        } else {
                            return "单选";
                        }
                    }
                },
                {
                    field: "vote_isclosed", title: "是否关闭", width: 100,
                    formatter: function (value, row, index) {
                        if (value === 0) {
                            return "启用";
                        } else {
                            return "关闭";
                        }
                    }
                }
            ]
        ],
        onBeforeLoad: function (param) {
            param.title_like = $("#txt_vote_like").textbox("getValue") != null
                ? $("#txt_vote_like").textbox("getValue")
                : "";
        },
        onClickRow: function () {
            var row = $("#vote_datagrid").datagrid("getSelected");
            $("#option_datagrid").datagrid("loadData", { total: 0, rows: [] });
            if (row.id != null && $.trim(row.id) !== "" && !isNaN(row.id)) {
                $("#option_datagrid").datagrid({ url: "/PFXFundOrganization/GetVoteOptionToList/", queryParams: { vote_id: row.id } });
            }
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

function initOption() {
    $("#option_datagrid").datagrid({
        striped: true, // 是否显示斑马线效果。
        loadMsg: "正在加载ing...", // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: "option_header", title: "选项", width: 100 },
                { field: "option_title", title: "选项内容", width: 400 }
            ]
        ],
        view: DataGridNoDataView,
        emptyMsg: "<span style='color:red;'>暂无数据</span>"
    });
}

/**
 * 初始化 dialog窗口的保存按钮
 */
function initEvent() {
    $("#dlg_add_vote").dialog({
        toolbar: [{
            id: "vote_save",
            text: "保存",
            iconCls: "icon-save",
            handler: function () {
                if (isValidate("dlg_add_vote")) {
                    if ($("#vote_save").linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $("#vote_save").linkbutton("disable"); // 禁用

                    var params = {
                        id: $("#hid_id").val(),
                        vote_title: $("#txt_title").textbox("getValue"),
                        vote_content: $("#txt_content").val(),
                        vote_startdate: $("#txt_strat").val(),
                        vote_enddate: $("#txt_end").val(),
                        vote_checktype: $("input[name='checkType']:checked").val(),
                        vote_isclosed: $("input[name='isColse']:checked").val()
                    };
                    $.ajax({
                        url: params.id == null || params.id === "" ? "/PFXFundOrganization/AddVote/" : "/PFXFundOrganization/UpdateVote/",
                        type: "post",
                        dataType: "json",
                        data: params,
                        success: function (data) {
                            if ($("#vote_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#vote_save").linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code === 200) {
                                $("#dlg_add_vote").dialog("close");
                                $.messager.alert("提示", "保存成功！");
                                $("#vote_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "保存失败！");
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                            if ($("#vote_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#vote_save").linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $("#dlg_add_option").dialog({
        toolbar: [{
            id: "option_save",
            text: "保存",
            iconCls: "icon-save",
            handler: function () {
                if (isValidate("dlg_add_option")) {
                    if ($("#option_save").linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $("#option_save").linkbutton("disable"); // 禁用

                    var params = {
                        id: $("#hid_oid").val(),
                        option_title:$("#txt_option_title").val(),
                        option_header: $("#txt_option_header").val(),
                        vote_id:$("#hid_vote_id").val()
                    };
                    $.ajax({
                        url: params.id == null || params.id === "" ? "/PFXFundOrganization/AddVoteOption/" : "/PFXFundOrganization/UpdateVoteOption/",
                        type: "post",
                        dataType: "json",
                        data: params,
                        success: function (data) {
                            if ($("#option_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#option_save").linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code === 200) {
                                $("#dlg_add_option").dialog("close");
                                $.messager.alert("提示", "保存成功！");
                                $("#option_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "保存失败！");
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                            if ($("#option_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#option_save").linkbutton("enable"); // 启用
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
        $("#vote_datagrid").datagrid("reload");
    });

    $("#btn_add") != null && $("#btn_add").click(function () {
        cleanDialog();
        $("#dlg_add_vote").dialog("open");
    });

    $("#btn_edit") != null && $("#btn_edit").click(function () {
        var rows = $("#vote_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
            cleanDialog();

            $("#hid_id").val(rows[0].id);
            $("#txt_title").textbox("setValue", rows[0].vote_title != null ? rows[0].vote_title : "");
            $("#txt_content").val(rows[0].vote_content != null ? rows[0].vote_content : "");
            $("#txt_strat").val(rows[0].vote_startdate != null ? moment(rows[0].vote_startdate).format("L") : "");
            $("#txt_end").val(rows[0].vote_enddate != null ? moment(rows[0].vote_enddate).format("L") : "");
            $("input[name='checkType'][value=" + rows[0].vote_checktype + "]").prop("checked", true); 
            $("input[name='isColse'][value=" + rows[0].vote_isclosed + "]").prop("checked", true); 
            $("#dlg_add_vote").dialog("open");
        } else {
            $.messager.alert("提示", "请选择一条角色数据！");
        }
    });

    $("#btn_del") != null && $("#btn_del").click(function () {
        var rows = $("#vote_datagrid").treegrid("getSelections");
        if (rows != null && rows.length === 1) {
            $.messager.confirm("确认", "您确认想要删除投票数据吗？", function (r) {
                if (r) {
                    $.ajax({
                        url: "/PFXFundOrganization/DeleteVote/",
                        type: "post",
                        dataType: "json",
                        data: { id: rows[0].id },
                        success: function (data) {
                            if (data != null && data.code === 200) {
                                $.messager.alert("提示", "删除成功！");
                                $("#vote_datagrid").datagrid("reload");
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
            $.messager.alert("提示", "请选择一条投票数据！");
        }
    });

    $("#btn_add_option") != null &&
        $("#btn_add_option").click(function() {
        var rows = $("#vote_datagrid").treegrid("getSelections");
        if (rows != null && rows.length === 1) {
            cleanOptionDialog();
                $("#hid_vote_id").val(rows[0].id);
                $("#dlg_add_option").dialog("open");
            } else {
                $.messager.alert("提示", "请选择一条投票数据！");
            }
        });

    $("#btn_edit_option") != null && $("#btn_edit_option").click(function () {

        var rows = $("#option_datagrid").treegrid("getSelections");
        if (rows != null && rows.length === 1) {
            cleanOptionDialog();
            console.log(rows);
            $("#hid_oid").val(rows[0].id);
            $("#txt_option_title").textbox("setValue", rows[0].option_title != null ? rows[0].option_title : "");
            $("#txt_option_header").textbox("setValue", rows[0].option_header != null ? rows[0].option_header : "");
            $("#hid_vote_id").val(rows[0].vote_id);
            $("#dlg_add_option").dialog("open");
        } else {
            $.messager.alert("提示", "请选择一条选项数据！");
        }
    });

    $("#btn_del_option") != null && $("#btn_del_option").click(function () {
        var rows = $("#option_datagrid").treegrid("getSelections");
        if (rows != null && rows.length === 1) {
            $.messager.confirm("确认", "您确认想要删除选项数据吗？", function (r) {
                if (r) {
                    $.ajax({
                        url: "/PFXFundOrganization/DeleteVoteOption/",
                        type: "post",
                        dataType: "json",
                        data: { id: rows[0].id },
                        success: function (data) {
                            if (data != null && data.code === 200) {
                                $.messager.alert("提示", "删除成功！");
                                $("#option_datagrid").datagrid("reload");
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
            $.messager.alert("提示", "请选择一条选项数据！");
        }
    });
}

/**
 * 清空dialog窗口
 */
function cleanDialog() {
    if ($("#vote_save").linkbutton("options").disabled) {
        $("#vote_save").linkbutton("enable");
    }

    $("#hid_id").val("");
    $("#txt_title").textbox("clear");
    $("#txt_content").val("");
    $("#txt_strat").val("");
    $("#txt_end").val("");
    $("input[name='checkType'][value=0]").prop("checked", true);
    $("input[name='isColse'][value=0]").prop("checked", true); 
};

function cleanOptionDialog() {
    if ($("#option_save").linkbutton("options").disabled) {
        $("#option_save").linkbutton("enable");
    }
    $("#txt_option_header").textbox("clear");
    $("#txt_option_header").textbox("clear");
}

