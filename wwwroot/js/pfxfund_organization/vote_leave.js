$(function () {
    moment.locale("zh-cn");
    initDatagrid();
});


/**
 * 初始化数据列表
 */
function initDatagrid() {
    $("#leave_datagrid").datagrid({
        url: "/PFXFundOrganization/GetLeaveToList/",
        striped: true, // 是否显示斑马线效果。
        loadMsg: "正在加载ing...", // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: "user_name", title: "用户名", width: 100 },
                { field: "comment_content", title: "留言内容", width: 1100 },
                {
                    field: "comment_date", title: "创建时间", width: 100,
                    formatter: function (value, row, index) {
                        return moment(value).format("LL");
                    }
                }
            ]
        ],
        onBeforeLoad: function (param) {
            param.key = $("#txt_key").textbox("getValue") != null
                ? $("#txt_key").textbox("getValue")
                : "";
            param.vote_id = $("#voteSelect").combobox("getValue");
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}


function initButtonEvent() {
    $("#btn_search") != null && $("#btn_search").click(function () {
        $("#leave_datagrid").datagrid("reload");
    });

    $("#btn_del") != null &&
        $("#btn_del").click(function() {
        var rows = $("#leave_datagrid").treegrid("getSelections");
            if (rows != null && rows.length === 1) {
                $.messager.confirm("确认", "您确认想要删除留言数据吗？", function (r) {
                    if (r) {
                        $.ajax({
                            url: "/PFXFundOrganization/DeleteLeave/",
                            type: "post",
                            dataType: "json",
                            data: { id: rows[0].id },
                            success: function (data) {
                                if (data != null && data.code === 200) {
                                    $.messager.alert("提示", "删除成功！");
                                    $("#leave_datagrid").datagrid("reload");
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
                $.messager.alert("提示", "请选择一条留言数据！");
            }
        });
}


