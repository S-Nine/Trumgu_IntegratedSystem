$(document).ready(function () {
    initMenu();
});

/**
 * 初始化菜单
 */
function initMenu() {
    $.ajax({
        url: '/Home/GetMenu/',
        type: 'post',
        dataType: 'json',
        success: function (data) {
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
    if (tab != null) {
        $('#tab_content').tabs('select', name);
    } else {
        $('#tab_content').tabs('add', {
            title: name,
            content: '<div style="width:100%;height:100%;overflow:hidden;"><iframe style="width:100%;height:100%;border:0px;" src="' + path + '?menu_id=' + id + '"></iframe></div>',
            iconCls: icon,
            closable: true,
        });
    }
}