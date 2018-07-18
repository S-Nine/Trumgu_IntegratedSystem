var timer = null; // 隐藏设置菜单的timeout事件
$(document).ready(function() {
    initMenu();
    initEvent();
    $('#btn_setting').hover(showSettingMenu, hideSettingMenu);
    $('.profile-drop-div').hover(showSettingMenu, hideSettingMenu);
});

function initEvent() {
    $('#dlg_update_pwd').dialog({
        toolbar: [{
            id: 'pwd_save',
            text: '保存',
            iconCls: 'icon-save',
            handler: function() {
                if (isValidate('dlg_update_pwd')) {
                    var params = {
                        o_pwd: $('#txt_login_o_pwd').passwordbox('getValue'),
                        n_pwd: $('#txt_login_n_pwd').passwordbox('getValue'),
                        q_pwd: $('#txt_login_q_pwd').passwordbox('getValue')
                    };

                    if (params.n_pwd != params.q_pwd) {
                        $.messager.alert('提示', '确认密码与新密码不一致！');
                        return;
                    }

                    if ($('#pwd_save').linkbutton("options").disabled) { // 防止重复提交数据
                        return;
                    }
                    $('#pwd_save').linkbutton("disable"); // 禁用

                    $.ajax({
                        url: '/System/UpdatePWD/',
                        type: 'post',
                        dataType: 'json',
                        data: params,
                        success: function(data) {
                            if ($('#pwd_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#pwd_save').linkbutton("enable"); // 启用
                            }
                            if (data != null && data.code == 200) {
                                $('#dlg_update_pwd').dialog('close');
                                $.messager.alert('提示', '保存成功！');
                            } else {
                                $.messager.alert('提示', '保存失败！' + data.data);
                            }
                        },
                        error: function() {
                            $.messager.alert('错误', '网络连接失败、请稍后再试！');
                            if ($('#pwd_save').linkbutton("options").disabled) { // 防止重复提交数据
                                $('#pwd_save').linkbutton("enable"); // 启用
                            }
                        }
                    });
                }
            }
        }]
    });
}

/**
 * 初始化菜单
 */
function initMenu() {
    $.ajax({
        url: '/Home/GetMenu/',
        type: 'post',
        dataType: 'json',
        success: function(data) {
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var h = '';
                    if (data[i].children != null && data[i].children.length > 0) {
                        for (var j = 0; j < data[i].children.length; j++) {
                            h += '<div class="menu_2" data-id="' + (data[i].children[j].id != null ? data[i].children[j].id : '') + '" data-name="' + (data[i].children[j].name != null ? data[i].children[j].name : '') + '" data-icon="' + (data[i].children[j].icon != null ? data[i].children[j].icon : 'icon-menu-2') + '" data-path="' + (data[i].children[j].path != null ? data[i].children[j].path : '') + '"><div class="' + (data[i].children[j].icon == null || data[i].children[j].icon == '' ? 'icon-menu-2' : data[i].children[j].icon) + '" style="height:16px;width:16px;float: left;margin:4px 4px 0 4px"></div><div>' + data[i].children[j].name + '</div></div>';
                        }
                    } else {
                        h = '<div class="no_menu">暂无数据</div>';
                    }
                    $('#navigation').accordion('add', {
                        title: data[i].name,
                        iconCls: data[i].icon == null || data[i].icon == '' ? 'icon-menu-1' : data[i].icon,
                        selected: i == 0 ? true : false,
                        content: h
                    });
                }
            }
            $('.menu_2').click(onClickMenu2);
        }
    });
}

/**
 * 二级菜单单击事件
 */
function onClickMenu2() {
    $('.menu_2').removeClass('select_menu');
    $(this).addClass('select_menu');

    var id = $(this).attr('data-id');
    var name = $(this).attr('data-name');
    var path = $(this).attr('data-path');
    var icon = $(this).attr('data-icon');


    var tab = $('#tab_content').tabs('getTab', name);
    console.log(tab);
    if (tab != null) {
        $('#tab_content').tabs('select', name);
    } else if (path != null && $.trim(path) !== '') {
        console.log(path);
        $('#tab_content').tabs('add', {
            title: name,
            content: '<div style="width:100%;height:100%;overflow:hidden;"><iframe style="width:100%;height:100%;border:0px;" src="' + path + '?menu_id=' + id + '"></iframe></div>',
            iconCls: icon,
            closable: true
        });
    }
}

/**
 * 显示设置菜单
 */
function showSettingMenu() {
    if (timer != null) {
        try {
            clearTimeout(timer);
        } catch (e) {

        }
    }
    $('.profile-drop-div').css('display', 'inline-table');
}

/**
 * 隐藏设置菜单
 */
function hideSettingMenu() {
    timer = setTimeout(function() {
        $('.profile-drop-div').css('display', 'none');
    }, 200);
}

/**
 * 登出
 */
function loginout() {
    window.location.href = "/login/Loginout?t=" + uuid();
}

function showUpdatePwd() {
    if ($('#pwd_save').linkbutton("options").disabled) {
        $('#pwd_save').linkbutton("enable");
    }
    $('#txt_login_o_pwd').passwordbox('clear');
    $('#txt_login_n_pwd').passwordbox('clear');
    $('#txt_login_q_pwd').passwordbox('clear');
    $('#dlg_update_pwd').dialog('open');
}