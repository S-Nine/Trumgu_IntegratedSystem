var line_login_num = null; // 用户登录走势折线图
var pie_menu_num = null; // 用户菜单使用饼图
var pie_fun_num = null; // 用户功能使用饼图
$(function() {
    initEvent();
    initDatagrid();
});

/**
 * 初始化数据列表
 */
function initDatagrid() {
    $('#user_datagrid').datagrid({
        url: '/XFundOrganization/GetXFundUserToPage/',
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
                { field: 'user_name', title: '用户名称', width: 100 },
                { field: 'login_name', title: '登录账号', width: 100 },
                { field: 'sum_login_num', title: '总登录次数', width: 70 },
                { field: 'call_num', title: '总请求次数', width: 70 },
                { field: 'person_liable', title: '负责人', width: 70 },
                {
                    field: 'login_time_min',
                    title: '最早登录日期',
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
                    field: 'login_time_max',
                    title: '最晚登录日期',
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
            param.user_name_like = $('#txt_user_name_like').textbox('getValue') != null ? $('#txt_user_name_like').textbox('getValue') : ''
            param.login_name_like = $('#txt_login_name_like').textbox('getValue') != null ? $('#txt_login_name_like').textbox('getValue') : ''
            param.login_time_min = $('#date_login_time_min').textbox('getValue') != null ? $('#date_login_time_min').textbox('getValue') : ''
            param.login_time_max = $('#date_login_time_max').textbox('getValue') != null ? $('#date_login_time_max').textbox('getValue') : ''
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
    line_login_num = echarts.init(document.getElementById('line_login_num'));
    clearLineLoginNumChart();
    pie_menu_num = echarts.init(document.getElementById('pie_menu_num'));
    clearPieMenuChart();
    pie_fun_num = echarts.init(document.getElementById('pie_fun_num'));
    clearPieFunChart();
}

/**
 * 初始化按钮事件(_ButtonTools.cshtml会自动加载)
 */
function initButtonEvent() {
    $('#btn_search') != null && $('#btn_search').click(function() {
        $('#user_datagrid').datagrid('reload');
    });
}

/**
 * 加载用户登录月度折线图
 * @param {登录账号} login_name 
 * @param {最早登录时间} login_time_min 
 * @param {最晚登录时间} login_time_max 
 */
function loadLineLoginNum(login_name, login_time_min, login_time_max) {
    clearLineLoginNumChart();
    $.ajax({
        url: '/XFundOrganization/GetXFundUserLoginDetailToList/',
        type: 'POST',
        data: { login_name: login_name, login_time_min: login_time_min, login_time_max: login_time_max },
        success: function(data) {
            if (data != null && data.length > 0) {
                var xAxis_data = [];
                var series_data = [];
                for (var i = 0; i < data.length; i++) {
                    xAxis_data.push(data[i].year_month);
                    series_data.push(data[i].sum_login_num);
                }
                var option = {
                    title: {
                        left: 'center',
                        text: '用户月度登录频率'
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    xAxis: {
                        type: 'category',
                        data: xAxis_data
                    },
                    yAxis: {
                        type: 'value'
                    },
                    series: [{
                        data: series_data,
                        type: 'line'
                    }]
                };
                line_login_num.setOption(option);
            }
        }
    });
}

/**
 * 加载用户功能使用饼图
 * @param {登录账号} login_name 
 * @param {最早登录时间} login_time_min 
 * @param {最晚登录时间} login_time_max 
 */
function loadPieMenuChart(login_name, call_time_min, call_time_max) {
    clearPieMenuChart();
    $.ajax({
        url: '/XFundOrganization/GetXFundUserMenuDetailToList/',
        type: 'POST',
        data: { login_name: login_name, call_time_min: call_time_min, call_time_max: call_time_max },
        success: function(data) {
            if (data != null && data.length > 0) {
                var legend_data = [];
                var series_data = [];
                for (var i = 0; i < data.length; i++) {
                    legend_data.push(data[i].menu_name);
                    series_data.push({ value: data[i].sum_login_num, name: data[i].menu_name });
                }
                var option = {
                    title: {
                        left: 'center',
                        text: '用户菜单使用情况'
                    },
                    tooltip: {
                        trigger: 'item',
                        formatter: "{a} <br/>{b}: {c} ({d}%)"
                    },
                    series: [{
                        name: '',
                        type: 'pie',
                        radius: ['50%', '70%'],
                        avoidLabelOverlap: false,
                        label: {
                            normal: {
                                show: false,
                                position: 'center'
                            },
                            emphasis: {
                                show: true,
                                textStyle: {
                                    fontSize: '30',
                                    fontWeight: 'bold'
                                }
                            }
                        },
                        labelLine: {
                            normal: {
                                show: false
                            }
                        },
                        data: series_data
                    }]
                };
                pie_menu_num.setOption(option);
            }
        }
    });
}

function loadPieFunChart(login_name, call_time_min, call_time_max) {
    clearPieFunChart();
    $.ajax({
        url: '/XFundOrganization/GetXFundUserFunDetailToList/',
        type: 'POST',
        data: { login_name: login_name, call_time_min: call_time_min, call_time_max: call_time_max },
        success: function(data) {
            if (data != null && data.length > 0) {
                var legend_data = [];
                var series_data = [];
                for (var i = 0; i < data.length; i++) {
                    legend_data.push(data[i].operation_desc);
                    series_data.push({ value: data[i].sum_login_num, name: data[i].operation_desc });
                }
                var option = {
                    title: {
                        left: 'center',
                        text: '用户功能使用情况'
                    },
                    tooltip: {
                        trigger: 'item',
                        formatter: "{a} <br/>{b}: {c} ({d}%)"
                    },
                    series: [{
                        name: '',
                        type: 'pie',
                        radius: ['50%', '70%'],
                        avoidLabelOverlap: false,
                        label: {
                            normal: {
                                show: false,
                                position: 'center'
                            },
                            emphasis: {
                                show: true,
                                textStyle: {
                                    fontSize: '30',
                                    fontWeight: 'bold'
                                }
                            }
                        },
                        labelLine: {
                            normal: {
                                show: false
                            }
                        },
                        data: series_data
                    }]
                };
                pie_fun_num.setOption(option);
            }
        }
    });
}

/**
 * 清空用户登录数量折线图
 */
function clearLineLoginNumChart() {
    var option = {
        title: {
            left: 'center',
            text: '用户月度登录频率'
        },
        tooltip: {
            trigger: 'axis'
        },
        xAxis: {
            type: 'category',
            data: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
        },
        yAxis: {
            type: 'value'
        },
        series: [{
            data: [0],
            type: 'line'
        }]
    };
    line_login_num.setOption(option);
}

/**
 * 清空用户使用菜单饼图
 */
function clearPieMenuChart() {
    var option = {
        title: {
            left: 'center',
            text: '用户菜单使用情况'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b}: {c} ({d}%)"
        },
        series: [{
            name: '',
            type: 'pie',
            radius: ['50%', '70%'],
            avoidLabelOverlap: false,
            label: {
                normal: {
                    show: false,
                    position: 'center'
                },
                emphasis: {
                    show: true,
                    textStyle: {
                        fontSize: '30',
                        fontWeight: 'bold'
                    }
                }
            },
            labelLine: {
                normal: {
                    show: false
                }
            },
            data: [
                { value: 0, name: '菜单' },
            ]
        }]
    };
    pie_menu_num.setOption(option);
}

/**
 * 清空用户使用功能饼图
 */
function clearPieFunChart() {
    var option = {
        title: {
            left: 'center',
            text: '用户功能使用情况'
        },
        tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b}: {c} ({d}%)"
        },
        series: [{
            name: '',
            type: 'pie',
            radius: ['50%', '70%'],
            avoidLabelOverlap: false,
            label: {
                normal: {
                    show: false,
                    position: 'center'
                },
                emphasis: {
                    show: true,
                    textStyle: {
                        fontSize: '30',
                        fontWeight: 'bold'
                    }
                }
            },
            labelLine: {
                normal: {
                    show: false
                }
            },
            data: [
                { value: 0, name: '功能' },
            ]
        }]
    };
    pie_fun_num.setOption(option);
}