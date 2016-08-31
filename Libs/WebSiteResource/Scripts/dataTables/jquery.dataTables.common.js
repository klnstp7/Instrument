function WrapTables4Dom() {
    var tableWidth = "100%";
    var isShowPaging = true;
    var bStateSave = false;
    var tableHight = "";
    var iDisplayLength = 25;

    if (arguments.length == 1) {
        return WrapTables4DomDefault(arguments[0], isShowPaging, tableWidth, bStateSave, tableHight, iDisplayLength);
    }
    else if (arguments.length == 2) {
        return WrapTables4DomDefault(arguments[0], arguments[1], tableWidth, bStateSave, tableHight, iDisplayLength);
    }
    else if (arguments.length == 3) {
        return WrapTables4DomDefault(arguments[0], arguments[1], arguments[2], bStateSave, tableHight, iDisplayLength);
    }
    else if (arguments.length == 4) {
        return WrapTables4DomDefault(arguments[0], arguments[1], arguments[2], arguments[3], tableHight, iDisplayLength);
    }
    else if (arguments.length == 5) {
        return WrapTables4DomDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], iDisplayLength);
    }
    else if (arguments.length == 6) {
        return WrapTables4DomDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
    }
}
/**
* @tableId Table对应的Id值，字符串类型
* @isShowPaging 是否显示分页，默认显示
* @tableWidth 表格宽度，可填写数字或百分比，如"1400px;" or "100%"
* @bStateSave true/false，记住状态
* @tableHight 表格高度 400px; 当设为"auto"时,系统将自动计算(计算原理:从table的表头到屏幕底部之间的距离;若从table表头到屏幕底部的距离小于或等于0,请不要设为"auto")
* @iDisplayLength：每页加载记录数
*/
function WrapTables4DomDefault(tableId, isShowPaging, tableWidth, bStateSave, tableHight, iDisplayLength) {
    setTableWidth(tableId, tableWidth);

    var oTable = $('#' + tableId).dataTable({
        "bJQueryUI": true,
        "bAutoWidth": false,
        "scrollX": true,
        "sScrollY": tableHight,
        "bStateSave": bStateSave,
        "bPaginate": isShowPaging,
        "bLengthChange": isShowPaging,
        "sPaginationType": "full_numbers",
        //"sScrollY": "380px",
        //"sScrollX": "120%",
        //"bScrollCollapse": true,
        "iDisplayLength": iDisplayLength,
        //"aoColumnDefs": [{ "bSortable": false, "bSearchable": false, "aTargets": [-1]}],
        "oLanguage": {
            "sLengthMenu": "每页 _MENU_ 条",
            "sZeroRecords": "sorry - 没有您要搜索的内容",
            "sInfo": "共 _TOTAL_ 条记录",
            "sInfoEmpty": "查找结果为 0",
            "sInfoFiltered": "(原始记录_MAX_条)",
            "sSearch": "搜索All：",
            "oPaginate": {
                "sFirst": "首页",
                "sPrevious": "上一页",
                "sNext": "下一页",
                "sLast": "尾页"
            }
        },
        "sProcessing": "数据正在加载中...",
        "sInfoPostFix": ""//,
        //"fnDrawCallback": function (oSettings) {
        //    rowSelectedCSS(tableId);
        //}
    });

    rowSelectedCSS(oTable);

    return oTable;
}


function WrapTables4Ajax() {
    var tableWidth = "100%";
    var tableHight = "";
    var isShowPaging = true;
    var bStateSave = false;
    var iDisplayLength = 25;

    if (arguments.length == 2) {
        return WrapTables4AjaxDefault(arguments[0], arguments[1], tableWidth, isShowPaging, tableHight, bStateSave, iDisplayLength);
    }
    else if (arguments.length == 3) {
        return WrapTables4AjaxDefault(arguments[0], arguments[1], arguments[2], isShowPaging, tableHight, bStateSave, iDisplayLength);
    }
    else if (arguments.length == 4) {
        return WrapTables4AjaxDefault(arguments[0], arguments[1], arguments[2], arguments[3], tableHight, bStateSave, iDisplayLength);
    }
    else if (arguments.length == 5) {
        return WrapTables4AjaxDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], bStateSave, iDisplayLength);
    }
    else if (arguments.length == 6) {
        return WrapTables4AjaxDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], iDisplayLength);
    }
    else if (arguments.length == 7) {
        return WrapTables4AjaxDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
    }
}
/**
* @tableId Table对应的Id值，字符串类型
* @ajaxSource ajax方式加载的数据源地址
* @tableWidth 表格宽度，可填写数字或百分比，如"1400px;" or "100%"
* @isShowPaging 是否显示分页，默认显示
* @tableHight 表格高度 400px; 当设为"auto"时,系统将自动计算(计算原理:从table的表头到屏幕底部之间的距离;若从table表头到屏幕底部的距离小于或等于0,请不要设为"auto")
* @bStateSave true/false，记住状态
* @iDisplayLength：每页加载记录数
*/
function WrapTables4AjaxDefault(tableId, ajaxSource, tableWidth, isShowPaging, tableHight, bStateSave, iDisplayLength) {
    setTableWidth(tableId, tableWidth);

    if (tableHight == "auto") tableHight = ($(window).height() - $("#" + tableId).offset().top - 95) + "px";

    var oTable = $('#' + tableId).dataTable({
        "bJQueryUI": true,
        "bAutoWidth": false,
        "scrollX": true,
        "bStateSave": bStateSave,
        "sScrollY": tableHight,
//        "bScrollCollapse": true,
        "bPaginate": isShowPaging,
        "bLengthChange": isShowPaging,
        "sPaginationType": "full_numbers",
        "iDisplayLength": iDisplayLength,
        "ajax": ajaxSource,
        "oLanguage": {
            "sLengthMenu": "每页 _MENU_ 条",
            "sZeroRecords": "sorry - 没有您要搜索的内容",
            "sInfo": "共 _TOTAL_ 条记录",
            "sInfoEmpty": "查找结果为 0",
            "sInfoFiltered": "(原始记录_MAX_条)",
            "sSearch": "搜索All：",
            "oPaginate": {
                "sFirst": "首页",
                "sPrevious": "上一页",
                "sNext": "下一页",
                "sLast": "尾页"
            }
        },
        "sProcessing": "数据正在加载中...",
        "sInfoPostFix": ""//,
        //"fnDrawCallback": function (oSettings) {
        //    rowSelectedCSS(tableId);
        //}
    });
    //alert("load");
    rowSelectedCSS(oTable);

    return oTable;
}




////后续作废函数，不建议使用
//function WrapNoPagingTables4Dom(tableId) {
//    var oTable = $('#' + tableId).dataTable({
//        "bJQueryUI": true,
//        "bPaginate": false,
//        "bLengthChange": false,
//        "bFilter": true,
//        "bSort": true,
//        "bInfo": true,
//        "bAutoWidth": false,
//        "scrollX": true,
//        //"sScrollY": "380px",
//        "oLanguage": {
//            "sZeroRecords": "sorry - 没有您要搜索的内容",
//            "sInfo": "共 _TOTAL_ 条记录",
//            "sInfoEmpty": "查找结果为 0",
//            "sSearch": "搜索All："
//        },
//        "fnDrawCallback": function (oSettings) {
//            rowSelectedCSS(tableId);
//        }

//    });

//    return oTable;
//}

////后续作废函数，不建议使用
//function WrapOnlyTables4Dom(tableId) {
//    var oTable = $('#' + tableId).dataTable({
//        "bJQueryUI": true,
//        "bPaginate": false,
//        "bLengthChange": false,
//        "bFilter": false,
//        "bSort": true,
//        "bInfo": false,
//        "bAutoWidth": false,
//        "scrollX": true,
//        "sDom": 't',
//        "fnDrawCallback": function (oSettings) {
//            rowSelectedCSS(tableId);
//        }

//    });

//    return oTable;
//}

//function WrapOnlyTables4Dom2(tableId, url) {
//    var oTable = $('#' + tableId).dataTable({
//        "bJQueryUI": true,
//        "bDestroy": true,
//        "bPaginate": false,
//        "bLengthChange": false,
//        "bFilter": false,
//        "bSort": true,
//        "bInfo": false,
//        "bAutoWidth": false,
//        "sDom": 't',
//        "sAjaxSource": url,
//        "fnInitComplete": function (oSettings, json) {
//            rowSelectedCSS(tableId);
//        }

//    });

//    return oTable;
//}



function WrapTables4JsArray() {
    var tableHight = "";
    var tableWidth = "100%";
    var isShowPaging = true;
    var bStateSave = false;
    var iDisplayLength = 25;

    if (arguments.length == 2) {
        return WrapTables4JsArrayDefault(arguments[0], arguments[1], tableWidth, isShowPaging, tableHight, bStateSave, iDisplayLength);
    }
    else if (arguments.length == 3) {
        return WrapTables4JsArrayDefault(arguments[0], arguments[1], arguments[2], isShowPaging, tableHight, bStateSave, iDisplayLength);
    }
    else if (arguments.length == 4) {
        return WrapTables4JsArrayDefault(arguments[0], arguments[1], arguments[2], arguments[3], tableHight, bStateSave, iDisplayLength);
    }
    else if (arguments.length == 5) {
        return WrapTables4JsArrayDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], bStateSave, iDisplayLength);
    }
    else if (arguments.length == 6) {
        return WrapTables4JsArrayDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], iDisplayLength);
    }
    else if (arguments.length == 7) {
        return WrapTables4JsArrayDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6]);
    }
}

/**
* @tableId Table对应的Id值，字符串类型
* @aaData 表格加载的数据
* @tableWidth 表格宽度，可填写数字或百分比，如"1400px;" or "100%"
* @isShowPaging 是否显示分页，默认显示
* @tableHight：表格高度 400px; 当设为"auto"时,系统将自动计算(计算原理:从table的表头到屏幕底部之间的距离;若从table表头到屏幕底部的距离小于或等于0,请不要设为"auto")
* @bStateSave true/false，记住状态
* @iDisplayLength：每页加载记录数
*/
function WrapTables4JsArrayDefault(tableId, aaData, tableWidth, isShowPaging, tableHight, bStateSave, iDisplayLength) {

    setTableWidth(tableId, tableWidth);

    if (tableHight == "auto") tableHight = ($(window).height() - $("#" + tableId).offset().top - 100) + "px";

    var oTable = $('#' + tableId).dataTable({
        "bJQueryUI": true,
        "bAutoWidth": false,
        "scrollX": true,
        "bStateSave": bStateSave,
        "sScrollY": tableHight,
//        "bScrollCollapse": true,
        "bPaginate": isShowPaging,
        "bLengthChange": isShowPaging,
        "sPaginationType": "full_numbers",
        "iDisplayLength": iDisplayLength,
        "aaData": aaData,
        "oLanguage": {
            "sLengthMenu": "每页 _MENU_ 条",
            "sZeroRecords": "sorry - 没有您要搜索的内容",
            "sInfo": "共 _TOTAL_ 条记录",
            "sInfoEmpty": "查找结果为 0",
            "sInfoFiltered": "(原始记录_MAX_条)",
            "sSearch": "搜索All：",
            "oPaginate": {
                "sFirst": "首页",
                "sPrevious": "上一页",
                "sNext": "下一页",
                "sLast": "尾页"
            }
        },
        "sProcessing": "数据正在加载中...",
        "sInfoPostFix": ""//,
        //"fnDrawCallback": function (oSettings) {
        //    rowSelectedCSS(tableId);
        //}
    });

    rowSelectedCSS(oTable);

    return oTable;
}


function GetJsondataFromServ() {
    var tableWidth = "100%";
    var col = null;
    var myQuerySettingInfo = {};
    myQuerySettingInfo.ColNum = 4;

    var tableHight = "";
    var isQuickSearch = true;
    var enableSort = false;
    var saveCookie = true;

    if (arguments.length == 2) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], tableWidth, col, myQuerySettingInfo, tableHight, isQuickSearch, enableSort, saveCookie);
    }
    else if (arguments.length == 3) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], col, myQuerySettingInfo, tableHight, isQuickSearch, enableSort, saveCookie);
    }
    else if (arguments.length == 4) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], arguments[3], myQuerySettingInfo, tableHight, isQuickSearch, enableSort, saveCookie);
    }
    else if (arguments.length == 5) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], tableHight, isQuickSearch, enableSort, saveCookie);
    }
    else if (arguments.length == 6) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], isQuickSearch, enableSort, saveCookie);
    }
    else if (arguments.length == 7) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], enableSort, saveCookie);
    }
    else if (arguments.length == 8) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], saveCookie);
    }
    else if (arguments.length == 9) {
        return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2], arguments[3], arguments[4], arguments[5], arguments[6], arguments[7], arguments[8]);
    }
}

/**
* @tableId Table对应的Id值，字符串类型
* @url 服务器端地址，字符串类型
* @width_str 表格宽度，可填写数字或百分比，如"1400px;" or "100%"
* @tableHight：表格高度 400px; 当设为"auto"时,系统将自动计算(计算原理:从table的表头到屏幕底部之间的距离;若从table表头到屏幕底部的距离小于或等于0,请不要设为"auto")
* @isQuickSearch 是否开启快速查询输入框
* @enableSort 是否开启字段排序功能
* @saveCookie 是否保存Cookie状态
*/
function GetJsondataFromServDefault(tableId, url, tableWidth, columnInfo, querySettingInfo, tableHight, isQuickSearch, enableSort, saveCookie) {
    setTableWidth(tableId, tableWidth);

    var chkHeight = isQuickSearch ? 100 : 72;
    var chkTop = $("#" + tableId).offset().top;
    

    var frameHeight = $(window).height();
    if (tableHight == "auto") tableHight = (frameHeight - chkTop - chkHeight) + "px";
    var isAutoAddDiv = $("#" + tableId + "AVSearch").length > 0 ? false : true;

    var myFrameTopScreen = 0;
    if (querySettingInfo != null && querySettingInfo.IsRLType != null && querySettingInfo.IsRLType == true && isAutoAddDiv) {
        var tb = $('#' + tableId);
        var tbhtml = tb.parent().html();
        var leftHtml = '<div id="' + tableId + 'AVSearch" style="float: left"></div><div id="' + tableId + 'AVScroll" IsExtend="true" ContentChangeWidth="0" style="border-right:4px solid #0997F7; width:1px; cursor:e-resize;float: left;" ondblclick="fnChangeSearchWidth(\'' + tableId + '\')"></div>';
        var rightHtml = '<div id="' + tableId + 'AVContent" style="float:left">' + tbhtml + '</div>';
        tb.parent().html(leftHtml + rightHtml);
    }


    var oTable = $('#' + tableId).dataTable({
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "bAutoWidth": false,
        "bLengthChange": false,
        //"sScrollX": "140%",
        "scrollX": true,
        "bStateSave": saveCookie,
        "sScrollY": tableHight,
        "bFilter": isQuickSearch,
        //"bPaginate": false,
        //"sScrollY": "380px",
        //"bScrollCollapse": true,
        "bDestroy": true,
//        "searching": true,
        "iDisplayLength": 25,
        //"aoColumnDefs": [{ "bSortable": false , "bSearchable": false, "aTargets": [-1]}],
        "oLanguage": {
            "sLengthMenu": "每页 _MENU_ 条",
            "sZeroRecords": "sorry - 没有您要搜索的内容",
            "sInfo": "共 _TOTAL_ 条记录",
            "sInfoEmpty": "查找结果为 0",
            "sInfoFiltered": "(原始记录_MAX_条)",
            "sSearch": "搜索：",
            "oPaginate": {
                "sFirst": "首页",
                "sPrevious": "上一页",
                "sNext": "下一页",
                "sLast": "尾页"
            }
        },
        "bSort": enableSort,
        "sProcessing": true,
        "bServerSide": true,
        "aoColumns": columnInfo, //排序时后台列名
        "sAjaxSource": url//,
        //"fnDrawCallback": function (settings) {
        //    rowSelectedCSS(tableId);
        //}
    });

    rowSelectedCSS(oTable);

    if (isAutoAddDiv) {
        if (columnInfo != null) {
            if (querySettingInfo == null || querySettingInfo.IsRLType == null || querySettingInfo.IsRLType == false) {
                advancedSearch(tableId, oTable, columnInfo, querySettingInfo);
            }
            else {
                advancedSearch4LR(tableId, oTable, columnInfo, querySettingInfo, frameHeight);
            }
            oTable.extend({ fnGetAdvCondition: function (columnsInfo) {
                var myParams = AddAdvanceSearchCondition(columnsInfo);
                var myUrlParams = "";
                for (var i = 0; i < myParams.Name.length; i++) {
                    if (i > 0) myUrlParams += "&";
                    myUrlParams += (myParams.Name[i] + "=" + encodeURI(myParams.Value[i]));
                }
                return myUrlParams + myParams.ParamsStr;
            }
            });

            oTable.extend({ reloadData: function () {
//                var sUrl = oTable.api().ajax.url();
//                var settings = oTable.fnSettings();
//                var joinStr = sUrl.indexOf("?") > 0 ? "&" : "?";
//                sUrl += (joinStr + "iDisplayStart=" + settings._iDisplayStart + "&iDisplayLength=" + settings._iDisplayLength);
//                oTable.api().ajax.url(sUrl);
//                alert(oTable.api().ajax.url());
                oTable.api().ajax.reload();
            }
            });

        }
    }
    return oTable;
}

function fnChangeSearchWidth(tableId) {
    var myScroll = $("#" + tableId + "AVScroll");
    var myContent = $("#" + tableId + "AVContent");
    var tableObject = $("#" + tableId);
    var IsExtend = myScroll.attr("IsExtend");
    var myWidth = $("#" + tableId + "AVContent").width();

    if (IsExtend == "true") {
        myScroll.attr("IsExtend", "false");
        $("#" + tableId + "AVSearch").hide();
        if (tableObject.width() < myWidth + avSearchWidth) {
            myScroll.attr("ContentChangeWidth", myWidth + avSearchWidth - tableObject.width());
            $("table[role='grid']").width(myWidth + avSearchWidth);
        }
        myContent.width(myWidth + avSearchWidth);
    }
    else {
        myScroll.attr("IsExtend", "true");
        $("#" + tableId + "AVSearch").show();
        myContent.width(myWidth - avSearchWidth);
        if(myScroll.attr("ContentChangeWidth") > 0){
            tableObject.width(tableObject.width() - myScroll.attr("ContentChangeWidth"));
        }
    }
}
//var GetJsondataFromServ = FunctionH.overload({
//    'string,string': function () {
//        //return GetJsondataFromServDefault(arguments[0], arguments[1], "1400px;");
//        return arguments[0] + arguments[1];
//    },
//    'string,string,string': function () {
//        //return GetJsondataFromServDefault(arguments[0], arguments[1], arguments[2]);
//        return arguments[0] + arguments[1] + arguments[2];
//    }
//});

//function setTableWidthDefault(tableId) {
//    setTableWidth(tableId, "1400px;");
//}
function setTableWidth(tableId, width_str) {
    var width = $(window).width();
    if (width_str != "100%") {
        if (width > parseInt(width_str.replace("px;", "").replace("px", ""))) {
            var leftWidth = $("#" + tableId).offset().left;
            width_str = (width - leftWidth -3) + "px;";
        }
    }
    
    //alert(tableId + "," + width);
    $("#" + tableId).removeAttr("style");
    $("#" + tableId).attr("style", "width: " + width_str);
    //alert($("#" + tableId).attr("style"));
}

function rowSelectedCSS(table) {
        //alert("ss");

    $(table).dataTable().on('click', 'tr', function () {
        if ($(this).hasClass('row_selected')) {
            $(this).removeClass('row_selected');
        }
        else {
            table.$('tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');
        }
    });



//    var table = $.fn.dataTable.fnTables(true);
//    if (table.length > 0) {
//        $("#" + tableId + " tbody tr").click(function (e) {
//            if ($(this).hasClass('row_selected')) {
//                $(this).removeClass('row_selected');
//            }
//            else {
//                $(table).dataTable().$('tr.row_selected').removeClass('row_selected');
//                $(this).addClass('row_selected');
//            }
//        });
//    }
}

/******************************************************
高级查询功能:
tableId:dataTableID; oTable:dataTable对象;  
columnsInfo:列信息(sName,sType,sTitle,aliasTitle,expression,relationField,searchDataSource,CustomParam)
querySettingInfo:自定义查询列表信息,可用于设置每行显示几个查询条件等(如:var querySettingInfo = {}; querySettingInfo.ColNum = 4;)
aliasTitle:高级查询显示的名称,可为空,为空时高级查询显示sTitle信息.
bVisible:字段是否可以见
expression:可为空,为空时为系统默认查询表达式;
1).表达式其值可为:=,>,>=,<,<=,like not like等;
2).表达式其值也可为:
查询折扣时设置为:sName为空,expression:FieldA/FieldB;
查询超时时设置为:sName为空,expression:DATEDIFF(day, FieldA, FieldB) > 0
searchDataSource对象信息:sTitle,sValue,selected,expression; 当sValue==''时,不作为条件查询.
relationField:用于date,int类型的区域内查找,如服务开始时间,服务结束时间,价格范围之类的辅助查询.
CustomParam:用于自定义参数,用于后台接收参数,多个列信息的CustomParam可同名,后台接收到的参数为where的条件语句.
注:
1.当sName == ''时,不被高级查询识别.
2.sType信息:string,int ,date
3.searchDataSource为下拉框数据源信息.
columnInfo = [
{ sName: '', sType: 'string', sTitle: '操作' },
{ sName: 'CertificateNumber', sType: 'string', sTitle: '证书编号'},
{ sName: 'CertRecordStae', sType: 'string', sTitle: '证书状态',
searchDataSource: [
{ sTitle: '所有', sValue: '', selected: true },
{ sTitle: '草稿', sValue: '0', selected: false },
{ sTitle: '送审', sValue: '1', selected: false },
{ sTitle: '审核通过', sValue: '2', selected: false },
{ sTitle: '修改申请', sValue: '3', selected: false }
]
},
{ sName: 'InstrumentName', sType: 'string', sTitle: '器具名称' },
{ sName: 'Specification', sType: 'string', sTitle: '规格型号' },
{ sName: 'MadeNumber', sType: 'string', sTitle: '出厂编号' }
]
******************************************************/
function advancedSearch(tableId, oTable, columnsInfo, querySettingInfo) {
    var tb = $("#" + tableId + "_filter");
    var tbhtml = tb.parent().html();
    var bthHtml = "<div id='divAVsearch' style='float:right; display:none;'><input type='button' id='btnAdvanceReSet' class='submit_btn' value='重置' /><input type='button' id='btnAdvanceSearch' class='submit_btn' value='查询' /><input type='hidden' id='myAdVanceSearchCondition' name='myAdVanceSearchCondition' value='' /></div>";
    var pHtml = "<div style='float:left; padding: 5px 15px;'> <a id='btnShowSearch' href='#' onclick='showSearch(\"" + tableId + "\");'>高级查询</a><input type='hidden' id='txtShowSearch' value='0' /> </div>";
    pHtml += "<input type='hidden' id='OriginalUrlStr' value='' />";
    var searchHtml = "<div style='display:none;clear:both;' id='divSearch' >";
    searchHtml += "<table>";
    if (querySettingInfo == null)
        querySettingInfo = new Object();
    if(querySettingInfo.ColNum == null)
        querySettingInfo.ColNum = 4;
    var colCount = querySettingInfo.ColNum;
    var colIndex = 0;
    var colWidth = 50 / colCount;
    var colWidthStr = " style='width:240px;'";
    var sTitle = "";
    for (var i = 0; i < columnsInfo.length; i++) {
        var sName = $.trim(columnsInfo[i].sName);
        if (sName == "" && (columnsInfo[i].expression == null || columnsInfo[i].expression == ""))
            continue;

        //if (sName == "") sName = columnsInfo[i].expression.replace(/[^a-zA-Z\d]/g, "");
        if (sName == "") sName = "advancedSearch" + i.toString();
        var dataType = columnsInfo[i].sType;
        var searchDataSource = columnsInfo[i].searchDataSource;
        sTitle = (columnsInfo[i].aliasTitle == null || columnsInfo[i].aliasTitle == "") ? columnsInfo[i].sTitle : columnsInfo[i].aliasTitle;

        if (colIndex % colCount == 0)
            searchHtml += "<tr>";

        searchHtml += "<td>" + sTitle + ":</td>";

        if (searchDataSource != null) {
            searchHtml += "<td" + colWidthStr + "><select id='search" + sName + "' name='search" + sName + "' class='SearchInput' >";
            for (var d = 0; d < searchDataSource.length; d++) {
                searchHtml += "<option value='" + searchDataSource[d].sValue + "'>" + searchDataSource[d].sTitle + "</option>";
            }
            searchHtml += "</select>";
        }
        else if (dataType == "int") {
            searchHtml += "<td" + colWidthStr + "><input id='search" + sName + "Start'  class='SearchInput' name='search" + sName + "Start' type='number'  style='width:82px;' />至";
            searchHtml += "<input id='search" + sName + "End'  class='SearchInput' name='search" + sName + "End' type='number' style='width:82px;' /></td>";
        }
        else if (dataType == "date") {
            searchHtml += "<td" + colWidthStr + ">";
            searchHtml += "<input id=\"search" + sName + "Start\" name=\"search" + sName + "Start\" type=\"text\" style=\"width:84px;\" class=\"Wdate SearchInput\" onfocus=\"WdatePicker({isShowClear:true,readOnly:true,maxDate:'#F{$dp.$D(\\'search" + sName + "End\\',{d:-0})}'})\" ></input>至";
            searchHtml += "<input id=\"search" + sName + "End\" name=\"search" + sName + "End\" type=\"text\" style=\"width:84px;\" class=\"Wdate SearchInput\" onfocus=\"WdatePicker({isShowClear:true,readOnly:true,minDate:'#F{$dp.$D(\\'search" + sName + "Start\\',{d:+0})}'})\" ></input></td>";

        }
        else
            searchHtml += "<td" + colWidthStr + "><input id='search" + sName + "' class='SearchInput' name='search" + sName + "' type='text' style='width:184px;' /></td>";

        if (colIndex % colCount == colCount - 1)
            searchHtml += "</tr>";

        colIndex++;
    }
    var addColCount = (colIndex + 1) % colCount;
    for (var j = 0; j < addColCount; j++) {
        searchHtml += "<td></td><td></td>";
    }
    if (addColCount != 0)
        searchHtml += "</tr>";
    searchHtml += "</table></div>";
    tb.parent().append(pHtml + bthHtml + searchHtml);


    addAdvanceSearchClick(columnsInfo, oTable);

}

var avSearchWidth = 260;
function advancedSearch4LR(tableId, oTable, columnsInfo, querySettingInfo, divHeight) {
    if (querySettingInfo.ColNum == null)
        querySettingInfo.ColNum = 4;

    //var myFrame = $(window.parent.frames["MiddleFrame"]);
    //var myWidth = myFrame.width();
    var myWidth = $(window).width();
    var myFrameTopScreen = $("#" + tableId + "AVSearch").offset().top;
    //var myTableTopScreen = $("#" + tableId + "_wrapper").offset().top;
    var myHeight = divHeight - myFrameTopScreen;

    $("#" + tableId + "AVSearch").height(myHeight);
    $("#" + tableId + "AVSearch").width(avSearchWidth);
    $("#" + tableId + "AVSearch").css("overflow", "auto");
    $("#" + tableId + "AVContent").height(myHeight);
    $("#" + tableId + "AVContent").width(myWidth - avSearchWidth - 5);
    $("#" + tableId + "AVContent").css("overflow", "auto");
    $("#" + tableId + "AVScroll").height(myHeight);
    

    var custonQuery = $("#" + tableId + "AVSearch");
    var bthHtml = "<div id='divAVsearch' style='float:right;'><input type='button' id='btnAdvanceReSet' class='submit_btn' value='重置' /><input type='button' id='btnAdvanceSearch' class='submit_btn' value='查询' /><input type='hidden' id='myAdVanceSearchCondition' name='myAdVanceSearchCondition' value='' /></div>";
    var pHtml = "<input type='hidden' id='OriginalUrlStr' value='' />";
    var searchHtml = "<div style='clear:both;' id='divSearch' >";
    searchHtml += "<table class='form_table' style='width:242px;' >";
    searchHtml += "<tr><td colspan='" + querySettingInfo.ColNum * 2 + "'>";
    searchHtml += bthHtml;
    searchHtml += pHtml;
    searchHtml += "</td></tr>";

    var colCount = querySettingInfo.ColNum;
    var colIndex = 0;
    var colWidthStr = " style='width:240px;'";
    var inputWidthStr = "width:184px;";
    if (querySettingInfo.ColNum == 1) {
        colWidthStr = " style='width:150px;'";
        inputWidthStr = "width:140px;";
    }

    var sTitle = "";
    for (var i = 0; i < columnsInfo.length; i++) {
        var sName = $.trim(columnsInfo[i].sName);
        if (sName == "" && (columnsInfo[i].expression == null || columnsInfo[i].expression == ""))
            continue;

        if (sName == "") sName = "advancedSearch" + i.toString();
        var dataType = columnsInfo[i].sType;
        var searchDataSource = columnsInfo[i].searchDataSource;
        sTitle = (columnsInfo[i].aliasTitle == null || columnsInfo[i].aliasTitle == "") ? columnsInfo[i].sTitle : columnsInfo[i].aliasTitle;

        if (colIndex % colCount == 0)
            searchHtml += "<tr>";

        searchHtml += "<td>" + sTitle + ":</td>";

        if (searchDataSource != null) {
            searchHtml += "<td" + colWidthStr + "><select id='search" + sName + "' name='search" + sName + "' class='SearchInput' >";
            for (var d = 0; d < searchDataSource.length; d++) {
                searchHtml += "<option value='" + searchDataSource[d].sValue + "'>" + searchDataSource[d].sTitle + "</option>";
            }
            searchHtml += "</select>";
        }
        else if (dataType == "int") {
            searchHtml += "<td" + colWidthStr + "><input id='search" + sName + "Start'  class='SearchInput' name='search" + sName + "Start' type='number'  style='width:82px;' />";
            if (querySettingInfo.ColNum == 1) {
                searchHtml += "</tr><tr><td>至</td><td>";
            }
            else {
                searchHtml += "至";
            }
            searchHtml += "<input id='search" + sName + "End'  class='SearchInput' name='search" + sName + "End' type='number' style='width:82px;' /></td>";
        }
        else if (dataType == "date") {
            searchHtml += "<td" + colWidthStr + ">";
            searchHtml += "<input id=\"search" + sName + "Start\" name=\"search" + sName + "Start\" type=\"text\" style=\"width:84px;\" class=\"Wdate SearchInput\" onfocus=\"WdatePicker({isShowClear:true,readOnly:true,maxDate:'#F{$dp.$D(\\'search" + sName + "End\\',{d:-0})}'})\" ></input>";
            if (querySettingInfo.ColNum == 1) {
                searchHtml += "</tr><tr><td>至</td><td>";
            }
            else {
                searchHtml += "至";
            }
            searchHtml += "<input id=\"search" + sName + "End\" name=\"search" + sName + "End\" type=\"text\" style=\"width:84px;\" class=\"Wdate SearchInput\" onfocus=\"WdatePicker({isShowClear:true,readOnly:true,minDate:'#F{$dp.$D(\\'search" + sName + "Start\\',{d:+0})}'})\" ></input></td>";

        }
        else
            searchHtml += "<td" + colWidthStr + "><input id='search" + sName + "' class='SearchInput'  name='search" + sName + "' type='text' style='" + inputWidthStr + "' /></td>";

        if (colIndex % colCount == colCount - 1)
            searchHtml += "</tr>";

        colIndex++;
    }
    var addColCount = (colIndex + 1) % colCount;
    for (var j = 0; j < addColCount; j++) {
        searchHtml += "<td></td><td></td>";
    }
    if (addColCount != 0)
        searchHtml += "</tr>";
    searchHtml += "</table></div>";
    custonQuery.append(searchHtml);

    addAdvanceSearchClick(columnsInfo, oTable);


}

function addAdvanceSearchClick(columnsInfo, oTable) {
    //为输入框绑定回车查询事件
        $(".SearchInput").keyup(function (e) {
            if (e.keyCode == 13) {
                $("#btnAdvanceSearch").click();
            }
        });
        $("#btnAdvanceSearch").click(function () {
            var myParams = AddAdvanceSearchCondition(columnsInfo);
            var sUrl = oTable.api().ajax.url();
            oTable.api().context[0].oPreviousSearch.sSearch = ""
            if ($("#OriginalUrlStr").val() == "")
                $("#OriginalUrlStr").val(sUrl);

            sUrl = $("#OriginalUrlStr").val();

            var myUrlParams = "";
            for (var i = 0; i < myParams.Name.length; i++) {
                if (i > 0) myUrlParams += "&";
                myUrlParams += (myParams.Name[i] + "=" + encodeURI(myParams.Value[i]));
            }
            myUrlParams += myParams.ParamsStr;
            
            if (sUrl.indexOf("?") > 0)
                oTable.api().ajax.url(sUrl + "&" + myUrlParams);
            else
                oTable.api().ajax.url(sUrl + "?" + myUrlParams);
            oTable.api().ajax.reload();
        });
    $("#btnAdvanceReSet").click(function () {
        ReSetAdvanceSearchCondition(columnsInfo);
    });
}

/***************************************



****************************************/
function showSearch(tableId) {
    var vShow = $("#txtShowSearch");
    if (vShow.val() == 0) {
        vShow.val(1);
        $("#divSearch").show();
        $("#divAVsearch").show();
        $("#" + tableId + "_filter").hide();
        $("#" + tableId + "_filter").find("input").each(function () {
            //this.value = "";
            this.disabled = true;
        });
    }
    else {
        vShow.val(0);
        $("#divSearch").hide();
        $("#divAVsearch").hide();
        $("#" + tableId + "_filter").show();
        $("#" + tableId + "_filter").find("input").each(function () {
            this.disabled = false;
        });
    }
}

function AddAdvanceSearchCondition(aoColumns) {
    var ConditionStr = "";
    var myCustomParamsArr = new Array();
    var myCustomParamsIndexArr = new Array();
    myCustomParamsIndexArr.push("myAdVanceSearchCondition");
    myCustomParamsArr.push("");
    var myExpression = "";
    var ParamsStr = "";
    for (var i = 0; i < aoColumns.length; i++) {
        var sName = $.trim(aoColumns[i].sName);
        if (sName == "" && (aoColumns[i].expression == null || aoColumns[i].expression == ""))
            continue;

        ConditionStr = "";

        //var myName = sName != "" ? sName : aoColumns[i].expression.replace(/[^a-zA-Z\d]/g, "");
        myName = sName != "" ? sName : "advancedSearch" + i.toString();
        var dataType = aoColumns[i].sType;

        var searchDataSource = aoColumns[i].searchDataSource;
        if (searchDataSource != null) {
            var tmpValue = $("#search" + myName).val();
            if (tmpValue != "") {
                ParamsStr += ("&search" + myName + "=" + tmpValue);
                for (var x = 0; x < searchDataSource.length; x++) {
                    if (tmpValue != searchDataSource[x].sValue)
                        continue;

                    myExpression = searchDataSource[x].expression == null || searchDataSource[x].expression == "" ? "" : searchDataSource[x].expression;
                    if (myExpression == "")
                        ConditionStr += " and " + sName + " = '" + tmpValue + "'";
                    else if (myExpression.indexOf("#value#") > 0)
                        ConditionStr += " and " + myExpression.replace("#value#", tmpValue);
                    else if ($.trim(myExpression.toLowerCase()) == "like" || (myExpression.length > 5 && myExpression.toLowerCase().substring(myExpression.length - 5, 5) == " like"))
                        ConditionStr += " and " + sName + ' ' + myExpression + " '%" + tmpValue + "%'";
                    else
                        ConditionStr += " and " + sName + ' ' + myExpression + " '" + tmpValue + "'";
                }
            }
        }
        else if (dataType == "int") {
            var tmpValue1 = $.trim($("#search" + myName + "Start").val());
            var tmpValue2 = $.trim($("#search" + myName + "End").val());
            if (parseFloat(tmpValue1) > parseFloat(tmpValue2)) {
                tmpValue = tmpValue1;
                tmpValue1 = tmpValue2;
                $("#search" + myName + "Start").val(tmpValue1)
                tmpValue2 = tmpValue;
                $("#search" + myName + "End").val(tmpValue)                 
            } 
            var tmpName = aoColumns[i].relationField == null || $.trim(aoColumns[i].relationField) == "" ? sName : aoColumns[i].relationField;
            if (tmpValue1 != "") {
                ConditionStr += " and " + tmpName + " >= " + tmpValue1;

                ParamsStr += ("&search" + myName + "1=" + tmpValue);
            }

            if (tmpValue2 != "") {
                ConditionStr += " and " + sName + " <= " + tmpValue2;
                ParamsStr += ("&search" + myName + "2=" + tmpValue);
            }
        }
        else if (dataType == "date") {
            var tmpValue = $.trim($("#search" + myName + "Start").val());
            var tmpName = aoColumns[i].relationField == null || $.trim(aoColumns[i].relationField) == "" ? sName : aoColumns[i].relationField;
            if (tmpValue != "") {
                ConditionStr += " and " + tmpName + " >= '" + tmpValue + "'";
                ParamsStr += ("&search" + myName + "1=" + tmpValue);
            }

            var tmpValue = $.trim($("#search" + myName + "End").val());
            if (tmpValue != "") {
                var d = new Date(tmpValue);
                if (isNaN(d)) {
                    d = new Date(tmpValue.replace('-', '/'));
                }
                d.setDate(d.getDate() + 1);
                var dateStr = d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
                ConditionStr += " and " + sName + " < '" + dateStr + "'";
                ParamsStr += ("&search" + myName + "2=" + tmpValue);
            }
        }
        else {
            var tmpValue = $.trim($("#search" + myName).val());
            if (tmpValue != "") {
                ParamsStr += ("&search" + myName + "=" + tmpValue);
                myExpression = aoColumns[i].expression == null || $.trim(aoColumns[i].expression) == "" ? "like" : $.trim(aoColumns[i].expression);
                if (myExpression.indexOf("#value#") > 0)
                    ConditionStr += " and " + myExpression.replace("#value#", tmpValue);
                else if (myExpression.toLowerCase() == "like" || (myExpression.length > 5 && myExpression.toLowerCase().substring(myExpression.length - 5, 5) == " like"))
                    ConditionStr += " and " + sName + ' ' + myExpression + " '%" + tmpValue + "%'";
                else
                    ConditionStr += " and " + sName + ' ' + myExpression + " '" + tmpValue + "'";
            }
        }
        var tmpArrIndex = aoColumns[i].CustomParam == null || aoColumns[i].CustomParam == "" ? 0 : $.inArray(aoColumns[i].CustomParam, myCustomParamsIndexArr);
        if(tmpArrIndex == -1) {
            myCustomParamsIndexArr.push(aoColumns[i].CustomParam);
            myCustomParamsArr.push("");
            tmpArrIndex = myCustomParamsIndexArr.length - 1;
        }
        if (ConditionStr != "") {
            var addAndStr = myCustomParamsArr[tmpArrIndex] == "" ? " 1=1 " : "";
            myCustomParamsArr[tmpArrIndex] += (addAndStr + ConditionStr);
        }
    }
    //$("#myAdVanceSearchCondition").val(ConditionStr);
    var myObject = {};
    myObject.Name = myCustomParamsIndexArr;
    myObject.Value = myCustomParamsArr;
    myObject.ParamsStr = ParamsStr;
    return myObject;
}

function ReSetAdvanceSearchCondition(aoColumns) {
    for (var i = 0; i < aoColumns.length; i++) {
        var sName = $.trim(aoColumns[i].sName);
        if (sName == "" && (aoColumns[i].expression == null || aoColumns[i].expression == ""))
            continue;

        myName = sName != "" ? sName : "advancedSearch" + i.toString();
        var dataType = aoColumns[i].sType;
        var searchDataSource = aoColumns[i].searchDataSource;
        if (searchDataSource != null) {
            $("#search" + myName).val("");
        }
        else if (dataType == "int") {
            $("#search" + myName + "Start").val("");
            $("#search" + myName + "End").val("");
        }
        else if (dataType == "date") {
            $("#search" + myName + "Start").val("");
            $("#search" + myName + "End").val("");
        }
        else {
            $("#search" + myName).val("");
        }
    }
}


