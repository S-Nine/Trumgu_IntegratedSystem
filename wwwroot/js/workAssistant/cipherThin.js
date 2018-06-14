$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#cipher_treegrid').datagrid({
        url: '/WorkAssistant/GetCipherThinToPage/',
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
                { field: 'title', title: '标题', width: 100 },
                { field: 'account_number', title: '账号', width: 100 },
                { field: 'account_pwd', title: '密码', width: 100 },
                { field: 'account_url', title: '网址', width: 100 },
                { field: 'account_email', title: '邮箱', width: 100 },
                { field: 'account_tel', title: '电话', width: 100 },
                {
                    field: 'account_register_date',
                    title: '注册日期',
                    width: 130,
                    formatter: function(value, row, index) {
                        if (value != null) {
                            return value.replace('T', ' ');
                        } else {
                            return '';
                        }
                    }
                },
                { field: 'last_modify_name', title: '最后修改人', width: 132 },
                {
                    field: 'create_time',
                    title: '创建时间',
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
                    field: 'last_modify_time',
                    title: '最后修改时间',
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
            param.title_like = $('#txt_title_like').textbox('getValue') != null ? $('#txt_title_like').textbox('getValue') : ''
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
        $('#cipher_treegrid').datagrid('reload');
    });

    $('#btn_add') != null && $('#btn_add').click(function() {
        cleanCipherThinDialog();
        $('#dlg_add_cipher').dialog('open');
    });
}

function cleanCipherThinDialog() {

}

/**
 * 添加Q&A
 */
function addQA() {
    var qa = $('.qa');
    $(qa[qa.length - 1]).parent().append("<div class=\"qa\" style=\"margin-top:5px;\">" +
        " <span>Q：</span>" +
        " <input class=\"easyui-textbox easyui-validatebox\" data-options=\"validType:[\'length[0,100]\']\" style=\"width:100px\">&nbsp;&nbsp;&nbsp;" +
        " <span>A：</span>" +
        " <input class=\"easyui-textbox easyui-validatebox\" data-options=\"validType:[\'length[0,100]\']\" style=\"width:100px\">" +
        " <a href=\"#\" onclick=\"removeQA(this);\" class=\"easyui-linkbutton\" data-options=\"iconCls:'icon-remove',plain:true\"></a>" +
        " </div>");
    $.parser.parse('.qa');
}

/**
 * 移除Q&A
 */
function removeQA(t) {
    $(t).parent().remove();
}