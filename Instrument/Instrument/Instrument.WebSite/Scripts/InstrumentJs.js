//业务附件表的操作 ----- 开始
//获取业务附件列表
//attachmentType:附件类型
//bkId，加密的业务主键
function fnGetBusinessAttachmentList(attachmentType, bkId, oTable, table) {
    //        var sScrollYWidth = $(window).height() - $("#ProjectList").offset().top - 65 + "px";
    if (oTable != null) oTable.fnDestroy();
    oTable = WrapTables4Ajax(table, "/BusinessAttachment/GetBusinessAttachmentList?attachmentType=" + attachmentType + "&bkId=" + bkId, "800px", true, "auto");
    return oTable;
}
//获取业务附件列表
//attachmentType:附件类型
//bkId，加密的业务主键
function fnGetBusinessAttachmentList4Detail(attachmentType, bkId, oTable, table) {

    //        var sScrollYWidth = $(window).height() - $("#ProjectList").offset().top - 65 + "px";
    //    oTable.fnDestroy();
    if (oTable != null) oTable.fnDestroy();
    oTable = WrapTables4Ajax(table, "/BusinessAttachment/GetBusinessAttachmentList4Detail?attachmentType=" + attachmentType + "&bkId=" + bkId, "10px", false, "auto");
    return oTable;
}

//获取原始记录附件列表
//bkId，加密的业务主键
function fnGetBusinessAttachmentList4Original(bkId, oTable, table) {

    if (oTable != null) oTable.fnDestroy();
    oTable = WrapTables4Ajax(table, "/BusinessAttachment/GetBusinessAttachmentList4Original?bkId=" + bkId, "10px", false, "auto");
    return oTable;
}

//上传业务附件
function fnUploadBusinessAttachment(businessType, businessKeyId, businessNumber, form, fnResponse) {
    form = "#" + form;
    var url = "/BusinessAttachment/UploadBusinessAttachment?businessType=" + businessType + "&businessKeyId=" + businessKeyId + "&businessNumber=" + businessNumber + "&ajax=true";

    var options = {
        success: fnResponse,
        url: url
    };
    if ($(form).valid()) {
        $(form).ajaxSubmit(options);
        return false;
    }
}

//批量删除业务附件
//chkName：
function fnDeleteBusinessAttachment(chkName, oTable) {
    var ids = fnGetAllCheckedValue(chkName);
    if ("" == ids) {
        ymPrompt.alert("请选择要删除的附件");
        return;
    }
    ymPrompt.confirmInfo({ message: "确定要删除？", title: '确认对话框', handler: function (tp) {
        if (tp == 'ok') {
            $.post("/BusinessAttachment/DeleteBusinessAttachment?ids=" + ids, function (data) {
                if (data == "OK") {
                    oTable.api().ajax.reload(function () {
                        ymPrompt.alert("删除成功！");
                    }, false);
                }
                else {
                    ymPrompt.alert(data);
                }
            });
        }
    }
    });
}

//更新备注
function fnUpdateAttachmentRemark(id) {
    var remak = $("#Remark" + id).val();
    if (remak.length > 120) {
        ymPrompt.alert("备注过长，请精简后再更新！");
        return;
    }
    $.post("/BusinessAttachment/UpdateRemark?id=" + id + "&remark=" + encodeURI($("#Remark" + id).val()), function (data) {
        if (data == "OK") {
            ymPrompt.alert("已更新");
        }
        else ymPrompt.alert(data);
    });
}

//申请受控
function fnUpdateAttachmentApplyControlled(obj, id) {
    ymPrompt.confirmInfo({ message: "确定要申请受控？", title: '确认对话框', handler: function (tp) {
        if (tp == 'ok') {
            $.post("/BusinessAttachment/UpdateApplyControlled?id=" + id, function (data) {
                if (data == "OK") {
                    var td = $(obj).parent().parent().find("td");
                    $(td).eq(2).text("");
                    $(td).eq(5).text("非受控(已申请)");
                }
                else ymPrompt.alert(data);
            });
        }
    }
    });
}