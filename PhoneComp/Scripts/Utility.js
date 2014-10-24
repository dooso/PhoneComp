
//消息框
function messageBox(message) {
    $("#Message").html(message);
    var dialogOpts = {
        modal: true,
        position: "center",
        height: 120,
        width: 350,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", $(this).parent()).hide();
            $(".ui-dialog-titlebar", $(this).parent()).hide();
        }
    };
    $("#Message").dialog(dialogOpts);
}

//提交表单
function submitForm(formId, url, params) {
    if ($("#" + formId).validationEngine("validate")) {
        messageBox("正在提交，请稍候……");
        ajaxPost(url, params);
    }
}
//ajaxPost提交
function ajaxPost(url, params) {
    $.post(url, params, function (data) {
        formResult(data);
    });
}

//返回值处理
function formResult(data) {
    var arr = data.split('|');
    var count = arr.length;
    if (count == 3) {
        messageBox(arr[1]);
        setTimeout(function () { location.href = arr[2]; }, 1000);
    }
    if (count == 2) {
        messageBox(arr[1]);
        setTimeout(function () {
            $("#Message").dialog("close");
            if (arr[0] == 1) {
                window.location.reload();
            }
        }, 2000);
    }
}
//ajaxPost提交删除
function ajaxPostDel(url, id) {
    if (confirm("删除后将不可恢复，你确认删除?")) {
        messageBox("正在提交，请稍候……");
        $.post(url, { id: id }, function (data) {
            formResult(data);
        });
    };
}

//ajaxPost提交删除
function ajaxPostDelWithTips(url, id,Tips) {
    if (confirm(Tips + "删除后将不可恢复，你确认删除?")) {
        messageBox("正在提交，请稍候……");
        $.post(url, { id: id }, function (data) {
            formResult(data);
        });
    };
}

//ajaxGet提交
function ajaxGet(url, params) {
    $.get(url, params, function (data) {
        formResult(data);
    });
}
//返回值处理
function formResult(data) {
    var arr = data.split('|');
    var count = arr.length;
    if (count == 3) {
        messageBox(arr[1]);
        setTimeout(function () { location.href = arr[2]; }, 1000);
    }
    if (count == 2) {
        messageBox(arr[1]);
        setTimeout(function () {
            $("#Message").dialog("close");
            if (arr[0] == 1) {
                window.location.reload();
            }
        }, 2000);
    }
}

$(function () {
    // Chosen.js 下拉选择
    var config = {
        '.chosen-select': {},
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-width': { width: "95%" }
    }
    for (var selector in config) {
        $(selector).chosen(config[selector]);
    }
});

//验证表单
function myFormValidation(formId) {
    $("#" + formId).validationEngine({
        validationEventTriggers: "blur",  //触发的事件  validationEventTriggers:"keyup blur",   
        success: false,//为true时即使有不符合的也提交表单,false表示只有全部通过验证了才能提交表单,默认false   
        promptPosition: "centerRight",//提示所在的位置，topLeft, topRight, bottomLeft,  centerRight, bottomRight
    });
}
