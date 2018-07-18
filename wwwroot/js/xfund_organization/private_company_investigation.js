$(function () {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#company_datagrid').datagrid({
        url: '/XFundOrganization/GetPrivateCompanyInvestigationToPage/',
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
                { field: 'cn_name', title: '公司名称', width: 200 },
                { field: 'regis_code', title: '公司编号', width: 80 },
                { field: 'jdcs', title: '尽调次数', width: 80 },
                {
                    field: 'gsjs',
                    title: '公司介绍',
                    align: 'center',
                    width: 100,
                    formatter: function (value, row, index) {
                        if (value != null && $.trim(value) != '') {
                            return '<a href="/Public/PreviewXFundPDF?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">查看</a>&nbsp;&nbsp;<a href="/Public/DownloadXFundFile?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">下载</a>';
                        } else {
                            return '';
                        }
                    }
                },
                {
                    field: 'jdjl',
                    title: '尽调记录',
                    align: 'center',
                    width: 100,
                    formatter: function (value, row, index) {
                        if (value != null && $.trim(value) != '') {
                            return '<a href="/Public/PreviewXFundPDF?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">查看</a>&nbsp;&nbsp;<a href="/Public/DownloadXFundFile?filePath=' + (value != null ? value : '') + '" target="_blank" style="text-decoration:none;");">下载</a>';
                        } else {
                            return '';
                        }
                    }
                }
            ]
        ],
        onBeforeLoad: function (param) {
            param.cn_name_like = $('#txt_cn_name_like').textbox('getValue') != null ? $('#txt_cn_name_like').textbox('getValue') : ''
            if ($('#chk_yes_1').prop('checked') == true && $('#chk_no_1').prop('checked') != true) {
                param.isIntroduction = true;
            } else if ($('#chk_yes_1').prop('checked') != true && $('#chk_no_1').prop('checked') == true) {
                param.isIntroduction = false;
            }
            if ($('#chk_yes_2').prop('checked') == true && $('#chk_no_2').prop('checked') != true) {
                param.isInvestigation = true;
            } else if ($('#chk_yes_2').prop('checked') != true && $('#chk_no_2').prop('checked') == true) {
                param.isInvestigation = false;
            }
        },
        view: DataGridNoDataView,
        emptyMsg: '<span style="color:red;">暂无数据</span>'
    });
}

/**
 * 初始化事件
 */
function initEvent() {
    $('#dlg_add_file').dialog({
        toolbar: [{
            id: 'file_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function () {
                if (isValidate('dlg_add_file')) {
                    var files = getFileList();
                    var title = $('#dlg_add_file').dialog('options').title;
                    if (files != null && files.length != 1) {
                        $.messager.alert('提示', '只能上传一张公司介绍！');
                        return;
                    }
                    if ($('#file_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#file_save').linkbutton("disable"); // 禁用
                    var params = {
                        regis_code: $('#hid_regis_code').val(),
                        files: files
                    };
                    $.ajax({
                        url: title == '上传尽调记录' ? '/XFundOrganization/AddInvestigation' : '/XFundOrganization/AddIntroduce/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function (data) {
                            if ($('#file_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#file_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_add_file').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                                $('#company_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '保存失败！');
                            }
                        },
                        error: function () {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#file_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#file_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });

    $('#dlg_add_info').dialog({
        toolbar: [{
            id: 'info_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function () {
                if (isValidate('dlg_add_info')) {
                    var params = {
                        id: $('#hid_id').val(),
                        cn_name: $("#txt_cn_name").textbox("getValue"),
                        regis_code:$("#txt_regis_code").textbox("getValue"),
                        en_name:$("#txt_en_name").textbox("getValue"),
                        address:$("#txt_address").textbox("getValue"),
                        company_scale:$("#txt_company_scale").textbox("getValue"),
                        legal_repre:$("#txt_legal_repre").textbox("getValue"),
                        org_code:$("#txt_org_code").textbox("getValue"),
                        paidin_capital:$("#txt_paidin_capital").textbox("getValue"),
                        regis_address:$("#txt_regis_address").textbox("getValue"),
                        regis_capital:$("#txt_regis_capital").textbox("getValue"),
                        website:$("#txt_website").textbox("getValue"),
                        fund_count:$("#txt_fund_count").textbox("getValue"),
                        staff_count:$("#txt_staff_count").textbox("getValue"),
                        company_intro:$("#txt_company_intro").val(),
                        invest_idea:$("#txt_invest_idea").val(),
                        core_person:$("#txt_core_person").val(),
                        corp_property:$("#txt_corp_property").combobox("getValue"),
                        if_member:$("#txt_if_member").combobox("getValue"),
                        regis_date:$("#txt_regis_date").datebox("getValue"),
                        setup_date:$("#txt_setup_date").datebox("getValue")
                    };
                    $.ajax({
                        url: "/XFundOrganization/UpdateCompanyInfo" ,
                        type: "post",
                        dataType: "json",
                        data: params,
                        success: function (data) {
                            if ($("#info_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#info_save").linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code === 200) {
                                $("#dlg_add_info").dialog("close");
                                $.messager.alert("提示", "保存成功！");
                                $("#company_datagrid").datagrid("reload");
                            } else {
                                $.messager.alert("提示", "保存失败！");
                            }
                        },
                        error: function () {
                            $.messager.alert("错误", "网络连接失败、请稍后再试！");
                            if ($("#info_save").linkbutton("options").disabled) { // 防止重复提交数据
                                $("#info_save").linkbutton("enable"); // 启用
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
    $('#btn_search') != null && $('#btn_search').click(function () {
        $('#company_datagrid').datagrid('reload');
    });

    $('#btn_upload_introduce') != null && $('#btn_upload_introduce').click(function () {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if (rows[0].regis_code != null && $.trim(rows[0].regis_code) != '') {
                cleanUploadFile();
                $('#hid_regis_code').val(rows[0].regis_code);
                url = "UploadXFundSliceFile";
                type = "PrivateCompanyIntroduction";
                $('#dlg_add_file').dialog({ title: '上传公司介绍' });
                $('#dlg_add_file').dialog('open');
            } else {
                $.messager.alert('提示', '该公司未备案！');
            }
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $('#btn_upload_investigation') != null && $('#btn_upload_investigation').click(function () {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            if (rows[0].regis_code != null && $.trim(rows[0].regis_code) != '') {
                cleanUploadFile();
                $('#hid_regis_code').val(rows[0].regis_code);
                url = "UploadXFundSliceFile";
                type = "PrivateCompanyInvestigation";
                $('#dlg_add_file').dialog({ title: '上传尽调记录' });
                $('#dlg_add_file').dialog('open');
            } else {
                $.messager.alert('提示', '该公司未备案！');
            }
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $('#btn_clean_introduce') != null && $('#btn_clean_introduce').click(function () {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要清空公司介绍数据吗？', function (r) {
                if (r) {
                    $.ajax({
                        url: '/XFundOrganization/CleanPrivateCompanyIntroduce/',
                        type: 'post',
                        dataType: 'json',
                        data: { regis_code: rows[0].regis_code },
                        success: function (data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '清空成功！');
                                $('#company_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '清空失败！');
                            }
                        },
                        error: function () {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            });
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $('#btn_clean_investigation') != null && $('#btn_clean_investigation').click(function () {
        var rows = $('#company_datagrid').datagrid('getSelections');
        if (rows != null && rows.length == 1) {
            $.messager.confirm('确认', '您确认想要清空公司尽调数据吗？', function (r) {
                if (r) {
                    $.ajax({
                        url: '/XFundOrganization/CleanPrivateCompanyInvestigation/',
                        type: 'post',
                        dataType: 'json',
                        data: { regis_code: rows[0].regis_code },
                        success: function (data) {
                            if (data != null && data.code == 200) {
                                $.messager.alert('提示', '清空成功！');
                                $('#company_datagrid').datagrid('reload');
                            } else {
                                $.messager.alert('提示', '清空失败！');
                            }
                        },
                        error: function () {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                        }
                    });
                }
            });
        } else {
            $.messager.alert('提示', '请选择一条公司数据！');
        }
    });

    $("#btn_edit") != null && $("#btn_edit").click(function () {
        var rows = $("#company_datagrid").datagrid("getSelections");
        console.log(rows);
        if (rows != null && rows.length === 1) {
            var code = rows[0].regis_code;
            $.ajax({
                url: "/XFundOrganization/GetCompanyInfo/",
                type: "post",
                dataType: "json",
                data: { code: code },
                success: function (data) {
                    clearCompanyInfo();
                    $("#hid_id").val(data.id);
                    $("#txt_cn_name").textbox("setValue", data.cn_name != null ? data.cn_name : "");
                    $("#txt_regis_code").textbox("setValue", data.regis_code != null ? data.regis_code : "");
                    $("#txt_en_name").textbox("setValue", data.en_name != null ? data.en_name : "");
                    $("#txt_address").textbox("setValue", data.address != null ? data.address : "");
                    $("#txt_company_scale").textbox("setValue", data.company_scale != null ? data.company_scale : "");
                    $("#txt_legal_repre").textbox("setValue", data.legal_repre != null ? data.legal_repre : "");
                    $("#txt_org_code").textbox("setValue", data.org_code != null ? data.org_code : "");
                    $("#txt_paidin_capital").textbox("setValue", data.paidin_capital != null ? data.paidin_capital : "");
                    $("#txt_regis_address").textbox("setValue", data.regis_address != null ? data.regis_address : "");
                    $("#txt_regis_capital").textbox("setValue", data.regis_capital != null ? data.regis_capital : "");
                    $("#txt_website").textbox("setValue", data.website != null ? data.website : "");
                    $("#txt_fund_count").textbox("setValue", data.fund_count != null ? data.fund_count : "");
                    $("#txt_staff_count").textbox("setValue", data.staff_count != null ? data.staff_count : "");
                    $("#txt_company_intro").val(data.company_intro != null ? data.company_intro : "");
                    $("#txt_invest_idea").val( data.invest_idea != null ? data.invest_idea : "");
                    $("#txt_core_person").val( data.core_person != null ? data.core_person : "");
                    $("#txt_corp_property").combobox("setValue", data.corp_property);
                    $("#txt_if_member").combobox("setValue", data.if_member);
                    $("#txt_regis_date").datebox("setValue", data.regis_date);
                    $("#txt_setup_date").datebox("setValue", data.setup_date);
                },
                error: function () {
                    $.messager.alert("错误", "网络连接失败、请稍后再试！");
                    if ($("#info_save").linkbutton("options").disabled) { // 防止重复提交数据
                        $("#info_save").linkbutton("enable"); // 启用
                    }
                }
            });
            $("#dlg_add_info").dialog("open");
        } else {
            $.messager.alert("提示", "请选择一条公司数据！");
        }
    });
}

function clearCompanyInfo() {
    $("#txt_cn_name").textbox("setValue", "");
    $("#txt_regis_code").textbox("setValue", "");
    $("#txt_en_name").textbox("setValue", "");
    $("#txt_address").textbox("setValue", "");
    $("#txt_company_scale").textbox("setValue", "");
    $("#txt_legal_repre").textbox("setValue", "");
    $("#txt_org_code").textbox("setValue", "");
    $("#txt_paidin_capital").textbox("setValue", "");
    $("#txt_regis_address").textbox("setValue", "");
    $("#txt_regis_capital").textbox("setValue", "");
    $("#txt_website").textbox("setValue", "");
    $("#txt_fund_count").textbox("setValue", "");
    $("#txt_staff_count").textbox("setValue", "");
    $("#txt_company_intro").val("");
    $("#txt_invest_idea").val("");
    $("#txt_core_person").val("");
    $("#txt_corp_property").combobox("clear");
    $("#txt_if_member").combobox("clear");
    $("#txt_regis_date").datebox("setValue", "");
    $("#txt_setup_date").datebox("setValue", "");
}