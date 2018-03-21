$(document).ready(function () {
    initMenu();
    initEvent();
});

/**
 * 初始化菜单
 */
function initMenu() {
    $.ajax({
        url: '/home/GetMenu/',
        type: 'post',
        dataType: 'json',
        success: function (data) {
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    var h = '';
                    if (data[i].children != null && data[i].children.length > 0) {
                        for (var j = 0; j < data[i].children.length; j++) {
                            h += '<div class="menu_2"><div class="' + (data[i].children[j].icon == null || data[i].children[j].icon == '' ? 'icon-menu-2' : data[i].children[j].icon) + '" style="height:16px;width:16px;float: left;margin:4px 4px 0 4px"></div><div>' + data[i].children[j].name + '</div></div>';
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
            $('.menu_2').click(function () {
                $('.menu_2').removeClass('select_menu');
                $(this).addClass('select_menu');
            });
        }
    });
}

/**
 * 注册事件
 */
function initEvent() {

}