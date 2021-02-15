// hide or unhide the value selection box if the operator is "BETWEEN"
function reply_click(click_id) {

    var elementId = click_id.toString();
    var selectBox = document.getElementById(elementId);
    var selectedValue = selectBox.options[selectBox.selectedIndex].value;
    var x = document.getElementById(elementId.toString() + "secondText");
    //var y = document.getElementById("fs");
    //var z = document.getElementById("ss");
    x.style.visibility = "hidden";
    //y.style.visibility = "hidden";
    //z.style.visibility = "hidden";
    if (selectedValue == "between") {
        x.style.visibility = "visible";
        //y.style.visibility = "visible";
        //z.style.visibility = "visible";
    }
    else {
        x.style.visibility = "hidden";
        //y.style.visibility = "hidden";
        //z.style.visibility = "hidden";
    }
}

// gather all values selected from the query builder page and pass to the home controller
function queryDataCollector() {
    var selectedItemsToSend = "";
    var selectedColumns = [];
    var selectedColumnTypes = [];
    var selectedTable = $("#tableName").html();
    
    var lastColumn = $("#cCount").html();
        for (i = 0; i < (lastColumn)-1; i++) {
            
            var currentColumn = $("#" + i);
            var currentColumnType = $("#type" + i)
            if (currentColumn.is(':checked')) {
                
                var columnName = currentColumn[0].getAttribute('name');
                var columne = { ColumnName: columnName };
                var columnType = currentColumnType[0].getAttribute('name');
                var cType = { ColumnType: columnType };
                selectedColumns.push(columne);
                selectedColumnTypes.push(cType);
                //var checkboxes = $('input[name=' + i.toString() + ']');
                //alert(checkboxes);
            }
    }
    var queryDictionary = { tableName: selectedTable, ColumnNames: selectedColumns, ColumnTypes: selectedColumnTypes };
    selectedItemsToSend = JSON.stringify(queryDictionary);

    if (selectedColumns == "" || selectedColumns == null) {
        alert("You must select at least one column");
    } else {
        sendParameter(queryDictionary);
    }
}

function sendParameter(data) {
    //$.ajax({
    //    url: '/Home/AddQueryParameters',
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    type: 'POST',
    //    data: JSON.stringify(data),
    //    success: function (result) {
    //        //alert(JSON.stringify(result));
    //        $('#addQueryOptions').html(JSON.stringify(result));
    //        window.location = "/Home/Index";
    //    }
    //    });

    $.post("/Home/AddQueryParameters", { 'data': data }, function (html) {
        var $div = $('<div class="text-center"></div>');
        $div.html(html);
        $("#addQueryOptions").append($div);
    });

    $("#indexToHide").hide();

    //window.location = "/Home/AddQueryParameters?data=" + data;
}

//function sendParameter(data) {
//    $.ajax({
//        url: '/Home/AddQueryParameters',
//        contentType: 'application/json; charset=utf-8',
//        dataType: 'json',
//        type: 'GET',
//        data: JSON.stringify(data)
//        ,
//            success: function () {
//                window.location = "/Home/AddQueryParameters?data="+data ;
//            }
//    });
//    //window.location = "/Home/AddQueryParameters?data=" + data;
//}

function goBack() {
    //var currentTableSelection = $("#tblName").html();
    //window.location = "/Home/QueryBuilder?data=" + currentTableSelection;
    $("#addQueryOptions").hide();
    $("#indexToHide").show();
}

function runQuery() {
    var columnsForQuery = [];
    var columnTypes = [];
    var columnsToInclude = [];
    var columnsToAggregate = [];
    var columnToFilter = "";
    var filterOperator = [];
    var filterValue = [];
    var secondFilterValue = [];
    var totalColumns = $("#columnCount").html();
    var table = $("#tblName").html();
    var queryName = $("#queryName").val();
    var user = $("#user").html();

    for (var i = 0; i < totalColumns; i++) {


        
        var columnSelection = $("#columnName" + i).html();
        columnsForQuery.push({ ColumnName: columnSelection });

        var columnTypeSelection = $("#columnType" + i).html();
        columnTypes.push({ columnTypes: columnTypeSelection });

        var columnToIncludeSelection = document.getElementById("include" + i.toString());
        columnToIncludeSelection = columnToIncludeSelection.options[columnToIncludeSelection.selectedIndex].value;

        var filterField = document.getElementById("filterType" + i.toString());
        filterField = filterField.options[filterField.selectedIndex].value;

        var filterText = document.getElementById("firstFilter" + i.toString());
        filterText = filterText.value;

        var secondFilterText = document.getElementById("secondFilter" + i.toString())
        secondFilterText = secondFilterText.value;

        var aggregateField = document.getElementById("additionalActions" + i.toString());
        aggregateField = aggregateField.options[aggregateField.selectedIndex].value;

        if (filterField != "placeholder") {

            if (filterField == "between") {

                filterOperator.push({ filterOperator: filterField });
                filterValue.push({ filterValue: filterText });
                secondFilterValue.push({ secondFilterValue: secondFilterText });
            }
            else if (filterText == "" || filterText == null) {
                alert("You selected a filter, but didn't specify a value");
                filterOperator.push({ filterOperator: " " });
                filterValue.push({ filterValue: " " });
                secondFilterValue.push({ secondFilterValue: " " });
            }
            else
            {

                filterOperator.push({ filterOperator: filterField });
                filterValue.push({ filterValue: filterText });
            }
        }
        else {
            filterOperator.push({ filterOperator: "None" });
            filterValue.push({ filterValue: "None" });
            secondFilterValue.push({ secondFilterValue: "None" });
        }

        if (columnToIncludeSelection == "Yes") {
            //columnsForQuery = $("#columnName" + i).html();
            columnsToInclude.push({ columnsToInclude: "Yes"});
        }
        else {
            //columnsForQuery = $("#columnName" + i).html();
            columnsToInclude.push({ columnsToInclude: "No" });
        }

        if (aggregateField == null || aggregateField == "Choose") {
            columnsToAggregate.push({ ColumnsToAggregate: "None" } );
        }
        else if (aggregateField != "Choose") {
            columnsToAggregate.push({ ColumnsToAggregate: aggregateField });
        }
        
    }

    if (queryName == "" || queryName == null) {
        alert("You must provide a name for your query.")
        return;
    }

    var runDictionary = { TableName: table, QueryName: queryName, ColumnNames: columnsForQuery, ColumnTypeSelection: columnTypes, FilterOperators: filterOperator, FilterValues: filterValue, SecondFilterValue: secondFilterValue, IncludeInViews: columnsToInclude, AdditionalActions: columnsToAggregate};
    var selectedDataToSend = JSON.stringify(runDictionary);
    sendDataToQueryParser(runDictionary, user);
    
}

function sendDataToQueryParser(data, username) {
    $.post("/Home/DisplayReport", { 'report': data, 'username': username }, function (html) {
        var $div = $('<div class="text-center"></div>');
        $div.html(html);
        $("#displayReport").append($div);
    });

    $("#addQueryOptions").remove();
    $("#indexToHide").remove();
//$.ajax({
//        url: '/Home/RunFinalQuery',
//        contentType: 'application/json',
//        type: 'POST',
//        data: data
//    });
}

function sendId(queryId) {
    //alert(queryId);
    var selectedQueryId = queryId.toString();
    var buttonId = $("#LoadButton" + selectedQueryId + "").html();
    
    $.post("/Home/DisplayLoadedReport", { 'Id': queryId }, function (html) {
        var $div = $('<div class="text-center"></div>');
        $div.html(html);
        $("#displayLoadedReport").append($div);
        
    });
    $("#queriesToHide").remove();
}

//function deleteId(queryId) {
//    var selectedQueryId = queryId.toString();
//    var buttonId = $("#DeleteButton" + selectedQueryId + "").html();

//    $.post("/Home/DeleteReport", { 'Id': queryId }, function (html) {
//        var $div = $('<div class="text-center"></div>');
//        $div.html(html);
//        $("#displayLoadedReport").append($div);

//    });
//    $("#queriesToHide").remove();
//}