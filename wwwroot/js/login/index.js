$(function() {
    $('#btn_login').click(login);
    $('body').bind('keyup', function(event) {
        if (event.keyCode == "13") {
            login();
        }
    });
});

function login() {
    if ($('#img_loading').is(":hidden")) {
        var n = $('#username').val();
        var p = $('#password').val();
        var c = $('#vcode').val();

        if (n == null || $.trim(n) == '') {
            // 请输入账号
            $('#username').focus();
            shake(document.getElementById('username'), null, 8, 600);
            return;
        }
        if (p == null || $.trim(p) == '') {
            // 请输入密码
            $('#password').focus();
            shake(document.getElementById('password'), null, 8, 600);
            return;
        }
        if (c == null || $.trim(c) == '') {
            // 请输入验证码
            $('#vcode').focus();
            shake(document.getElementById('vcode'), null, 8, 600);
            return;
        }
        $('#img_loading').show();

        $.ajax({
            url: '/Login/VerificationLogin/',
            type: 'post',
            dataType: 'json',
            data: { n: n, p: p, c: c },
            success: function(data) {
                $('#img_loading').hide();
                if (data != null && data.code == 200) {
                    window.location.href = "/Home/Index?t=" + Date.parse(new Date());
                } else {
                    $('#img_code').click();
                    alert(data.msg);
                }
            },
            error: function(XMLHttpRequest, textStatus, errorThrown) {
                $('#img_loading').hide();
                alert('网络连接失败、请稍后再试！');
            }
        });
    }
}

function shake(e, onComplete, distance, interval) {
    if (typeof e === "string") {
        e = document.getElementById(e);
    } // end if
    distance = distance || 8;
    interval = interval || 800;
    var originalStyle = e.style.cssText;
    e.style.position = "relative";
    var start = (new Date()).getTime();
    animate();

    function animate() {
        var now = (new Date()).getTime();
        var elapsed = now - start;
        var progress = elapsed / interval;
        if (progress < 1) {
            var y = distance * Math.sin(Math.PI * progress * 4);
            var x = distance * Math.cos(Math.PI * progress * 4);
            e.style.left = x + "px";
            e.style.top = y + "px";
            console.log(e.style.cssText);
            setTimeout(animate, Math.min(25, elapsed));
        } // end if
        else {
            e.style.cssText = originalStyle;
            if (onComplete) {
                onComplete(e);
            } // end if
        } // end else
    } // end animate()
} // end shake()