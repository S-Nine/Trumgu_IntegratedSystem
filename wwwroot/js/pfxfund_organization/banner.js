$(function () {
    moment.locale("zh-cn");
    initEvent();
    initDatagrid();
});

function initEvent() {
    $("#dlg_add_banner").dialog({
        toolbar: [{
            id: "banner_save",
            text: "保存",
            iconCls: "icon-save",
            handler: function () {
                if (isValidate("dlg_add_banner")) {
                    if ($("#banner_save").linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $("#banner_save").linkbutton("disable"); // 禁用

                    var params = {
                        id: $("#hid_id").val(),
                        banner_title: $("#txt_banner_title").textbox("getValue"),
                        banner_target: $("#txt_banner_target").combobox("getValue"),
                        banner_link: $("#txt_banner_link").textbox("getValue"),
                        banner_sort: $("#txt_banner_sort").textbox("getValue"),
                        is_enable: $("input[name='checkType']:checked").val()
                    };
                    $.ajax({
                        url: params.id == null || params.id === "" ? "/PFXFundOrganization/AddBanner/" : "/PFXFundOrganization/UpdateBanner/",
                        type: "post",
                        dataType: "json",
                        data: params,
                        success: function (data) {
                            if ($("#banner_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#banner_save").linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code === 200) {
                                $("#dlg_add_banner").dialog("close");
                                $.messager.alert("提示", "保存成功！");
                                $("#banner_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "保存失败！");
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                            if ($("#banner_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#banner_save").linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });
}

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $("#banner_datagrid").datagrid({
        url: "/PFXFundOrganization/GetBannerToList/",
        striped: true, // 是否显示斑马线效果。
        loadMsg: "正在加载ing...", // 在从远程站点加载数据的时候显示提示消息。
        pagination: false, // 启用分页
        fit: true,
        singleSelect: true,
        columns: [
            [
                { field: "banner_title", title: "banner标题", width: 100 },
                {
                    field: "banner_sort", title: "轮播顺序", width: 100
                    
                },
                {
                    field: "web_banner_url", title: "轮播图地址", width: 200 ,
                    formatter: function (value, row, index) {
                        if (value == null || value.length < 1) {
                            return "暂无图片";
                        }

                        return "<a  target='blank' href='" + value+"'>预览</a>";
                    }
                },
                
                {
                    field: "banner_target", title: "点击执行事件", width: 100,
                    formatter: function (value, row, index) {
                        if (value === 0) {
                            return "无事件";
                        } else if (value === 1) {
                            return "程序内打开链接";
                        } else if (value === 2) {
                            return "新窗口打开链接";
                        } else {
                            return "未知事件";
                        }
                    }
                },
                { field: "banner_link", title: "链接地址", width: 300 },
                {
                    field: "create_time", title: "创建时间", width: 100,
                    formatter: function (value, row, index) {
                        return moment(value).format("LL");
                    }
                },
                {
                    field: "modify_time", title: "修改时间", width: 100,
                    formatter: function (value, row, index) {
                        return moment(value).format("LL");
                    }
                },
                {
                    field: "is_enable", title: "是否有效", width: 100,
                    formatter: function (value, row, index) {
                        if (value === 0) {
                            return "禁用";
                        } else {
                            return "有效";
                        }
                    }
                }
            ]
        ],
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

function initButtonEvent() {
    $("#btn_add") != null && $("#btn_add").click(function () {
        cleanDialog();
        $("#dlg_add_banner").dialog("open");
    });

    $("#btn_edit") != null && $("#btn_edit").click(function () {
        var rows = $("#banner_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
           cleanDialog();

            $("#hid_id").val(rows[0].id);
            $("#txt_banner_title").textbox("setValue", rows[0].banner_title != null ? rows[0].banner_title : "");
            $("#txt_banner_link").textbox("setValue", rows[0].banner_link != null ? rows[0].banner_link : "");
            $("#txt_banner_sort").textbox("setValue", rows[0].banner_sort != null ? rows[0].banner_sort : "");
            $("#txt_banner_target").combobox("setValue", rows[0].banner_target != null ? rows[0].banner_target : "");
            $("input[name='checkType'][value=" + rows[0].is_enable + "]").prop("checked", true);
            $("#dlg_add_banner").dialog("open");
        } else {
            $.messager.alert("提示", "请选择一条轮播图数据！");
        }
    });

    $("#btn_del") != null && $("#btn_del").click(function () {
        var rows = $("#banner_datagrid").treegrid("getSelections");
        if (rows != null && rows.length === 1) {
            $.messager.confirm("确认", "您确认想要删除轮播图数据吗？", function (r) {
                if (r) {
                    $.ajax({
                        url: "/PFXFundOrganization/DeleteBanner/",
                        type: "post",
                        dataType: "json",
                        data: { id: rows[0].id },
                        success: function (data) {
                            if (data != null && data.code === 200) {
                                $.messager.alert("提示", "删除成功！");
                                $("#banner_datagrid").datagrid("reload");
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
            $.messager.alert("提示", "请选择一条轮播图数据！");
        }
    });

    $("#btn_upload") != null && $("#btn_upload").click(function () {
        var rows = $("#banner_datagrid").datagrid("getSelections");
        if (rows != null && rows.length === 1) {
            uploader.reset();
            
            $("#dlg_banner_file").dialog("open");
            console.log(uploader.getFiles()); 
            uploader.refresh();
            uploader.options.formData = { "uid": rows[0].id}
        } else {
            $.messager.alert("提示", "请选择一条公司数据！");
        }
    });
}

function cleanDialog() {
    $("#hid_id").val("");
    $("#txt_banner_title").textbox("clear");
    $("#txt_banner_target").combobox("clear");
    $("#txt_banner_link").textbox("clear");
    $("#txt_banner_sort").textbox("clear");
    $("input[name='checkType'][value=0]").prop("checked", true);
}
