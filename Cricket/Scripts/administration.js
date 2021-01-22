$(function () {

    var type = "CURRENT";

    $("#linkViewLeagues").on('click', function () {

        $.ajax({
            type: 'post',
            url: '../Administration/GetLeagues',
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });

        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $(this).addClass('active');
    });

    $("#linkLeagueCreator").on('click', function () {
        $("#creationDiv").load('../Administration/League');
    });

    $("#leagueRulesBtn").on('click', function () {
        $("#creationDiv").load('../Administration/LeagueRules');
    });

    $('input[name="daterange"]').daterangepicker({
        locale: {
            format: 'DD/MM/YY'
        }
    });

    $('input[name="daterange"]').data('daterangepicker').setStartDate(moment().subtract(29, 'days'));
    $('input[name="daterange"]').data('daterangepicker').setEndDate(moment());

    $('input[name="daterange"]').on('apply.daterangepicker', function (ev, picker) {
//        alert(picker.startDate.format('YYYY/MM/DD'));
//        alert(picker.endDate.format('YYYY/MM/DD'));

        // activating all leads with new set dates value
        type = "CURRENT";

        // updating session values for from and to dates
        $.ajax({
            type: 'post',
            url: '../Lead/UpdateDates',
            data: {
                from: picker.startDate.format('YYYY/MM/DD'),
                to: picker.endDate.format('YYYY/MM/DD')
            },
            success: function (result) {
            }
        });

        $.ajax({
            type: 'post',
            url: '../Lead/GetAllLeads',
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });


        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $('#linkAll').addClass('active');
    });

    $("#btnMarkImp").on('click', function () {
        var _queueIds = [];

        if ($(".lead-check:checked").length == 0)
            return;

        $(".lead-check:checked").each(function () {
            _queueIds.push($(this).closest('tr').attr('data-id'));

        });

        $.ajax({
            type: 'post',
            url: '../Lead/MarkImportant',
            data: {
                queueIds: _queueIds.toString()
            },
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });

        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $("#linkAll").addClass('active');
    });
    $("#btnViewHistory").on('click', function () {
        type = 'HISTORY';
        var _queueIds = [];

        if ($(".lead-check:checked").length == 0)
            return;

        $(".lead-check:checked").each(function () {
            _queueIds.push($(this).closest('tr').attr('data-id'));

        });

        $.ajax({
            type: 'post',
            url: '../Lead/GetLeadHistory',
            data: {
                queueIds: _queueIds.toString()
            },
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });

        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        //$(this).addClass('active');
    });





    $("#linkStarred").on('click', function () {
        type = "CURRENT";
        $.ajax({
            type: 'post',
            url: '../Lead/GetStarredLeads',
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });

        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $(this).addClass('active');
    });

    $("#linkInitiated").on('click', function () {
        type = "CURRENT";
        $.ajax({
            type: 'post',
            url: '../Lead/GetInitiatedLeads',
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });
        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $(this).addClass('active');
    });

    $("#linkAll").on('click', function () {
        type = "CURRENT";
        $.ajax({
            type: 'post',
            url: '../Lead/GetAllLeads',
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });
        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $(this).addClass('active');
    });

    $("#listFilter a").on('click', function () {
        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#linkAll").addClass('active');
        $("#txtSearch").val("");
        $(this).parent('li').addClass('active');

        $.ajax({
            type: 'post',
            url: '../Lead/FilterLeads',
            data: {
                'searchText': $(this).text()
            },
            success: function (result) {
                $('#tableDataDiv').html(result);
            }
        });
    });

    var timer;
    $("#txtSearch").on('input', function () {
        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#linkAll").addClass('active');
        clearTimeout(timer);
        timer = setTimeout(function () {
            $.ajax({
                type: 'post',
                url: '../Lead/SearchLeads',
                data: {
                    'searchText': $("#txtSearch").val(),
                    type: type
                },
                success: function (result) {
                    $('#tableDataDiv').html(result);
                }
            });
        }, 500);

    });

    //    $(document).on('change', ".lead-check", function () {
    //        var selLen = $('.lead-check:checked').length;
    //        var Len = $('.lead-check').length;

    //        if (selLen == 0) {
    //            $("#selectAllChk").prop('checked', false);
    //        }
    //        else if (selLen == Len) {
    //            $("#selectAllChk").prop('checked', true);
    //        }
    //    });

    //    $("#selectAllChk").on('change', function () {
    //        if ($(this).prop('checked') == true) {
    //            $(".lead-check").prop('checked', true);
    //        }
    //        else {
    //            $(".lead-check").prop('checked', false);
    //        }
    //    });

    $("#btnPrevious").on('click', function () {
        if ($(this).parent('li').hasClass('disabled')) {
            return;
        }
        $("#btnNext").parent('li').removeClass('disabled');

        var index = parseInt($(".table").attr('data-index'));
        index = index + 1;
        $.ajax({
            type: 'post',
            url: '../Lead/Paging',
            data: {
                pageIndex: index,
                type: type
            },
            success: function (result) {
                $('#tableDataDiv').html(result);
                $('.table').attr('data-index', index);
            }
        });
    });

    $("#btnNext").on('click', function () {
        if ($(this).parent('li').hasClass('disabled')) {
            return;
        }
        $("#btnPrevious").parent('li').removeClass('disabled');

        var index = parseInt($(".table").attr('data-index'));
        index = index - 1;
        $.ajax({
            type: 'post',
            url: '../Lead/Paging',
            data: {
                pageIndex: index,
                type: type
            },
            success: function (result) {
                $('#tableDataDiv').html(result);
                if (index == 1) {
                    $("#btnNext").parent('li').addClass('disabled');
                }
                $('.table').attr('data-index', index);
            }
        });
    });

    $(".UpdateStatus").on("click", function () {
        var _queueIds = [];
        if ($(".lead-check:checked").length == 0)
            return;

        $(".lead-check:checked").each(function () {
            _queueIds.push($(this).closest('tr').attr('data-id'));

        });

        $.ajax({
            type: 'post',
            url: '../Lead/UpdateLeadStatus',
            data: {
                queueIds: _queueIds.toString(),
                status: $(this).html()
            },
            success: function (result) {
                ajaxSuccessCall(result, function () {
                    $('#tableDataDiv').html(result);
                });
            }
        });

        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $("#linkAll").addClass('active');
    });

    $(".AssignRo").on("click", function () {

        var RoUserIdClient = "me.nib.nig.com/" + $("#txtAssignToRo").val();

        var _queueIds = [];
        if ($(".lead-check:checked").length == 0)
            return;

        $(".lead-check:checked").each(function () {
            _queueIds.push($(this).closest('tr').attr('data-id'));

        });

        $.ajax({
            type: 'post',
            url: '../Lead/AssignRo',
            data: {
                queueIds: _queueIds.toString(),
                RoUserId: RoUserIdClient
            },
            success: function (result) {
                ajaxSuccessCall(result, function () {
                    $('#tableDataDiv').html(result);
                    $("#txtAssignToRo").val("");
                });
            }
        });

        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $("#linkAll").addClass('active');
    });

    $(".UntagRo").on("click", function () {

        var _queueIds = [];
        if ($(".lead-check:checked").length == 0)
            return;

        $(".lead-check:checked").each(function () {
            _queueIds.push($(this).closest('tr').attr('data-id'));

        });

        $.ajax({
            type: 'post',
            url: '../Lead/UntagRo',
            data: {
                queueIds: _queueIds.toString()
            },
            success: function (result) {
                ajaxSuccessCall(result, function () {
                    $('#tableDataDiv').html(result);
                    $("#txtAssignToRo").val("");
                });
            }
        });
        $('a.list-group-item,#listFilter li').removeClass('active');
        $("#txtSearch").val("");
        $("#linkAll").addClass('active');
    });


});
