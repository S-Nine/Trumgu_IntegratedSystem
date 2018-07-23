$(function () {
    moment.locale("zh-cn");
    initDatagrid();
    initStatDatagrid();
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
            $("#stat_datagrid").datagrid("loadData", { total: 0, rows: [] });
            if (row.id != null && $.trim(row.id) !== "" && !isNaN(row.id)) {
                $("#stat_datagrid").datagrid({ url: "/PFXFundOrganization/GetStatToList/", queryParams: { voteId: row.id } });
            }
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

function initStatDatagrid() {
    $("#stat_datagrid").datagrid({
        striped: true, // 是否显示斑马线效果。
        loadMsg: "正在加载ing...", // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: "stat_account", title: "账号", width: 100 },
                { field: "stat_user", title: "用户名", width: 100 },
                { field: "stat_option", title: "统计", width: 100 },
                {
                    field: "stat_date", title: "投票时间", width: 200 ,
                    formatter: function (value, row, index) {
                        return moment(value).format("LLL");
                    }
                }
            ]
        ],
        view: DataGridNoDataView,
        emptyMsg: "<span style='color:red;'>暂无数据</span>"
    });
}

function initButtonEvent() {
    $("#btn_search") != null && $("#btn_search").click(function () {
        $("#vote_datagrid").datagrid("reload");
    });
}
