﻿var ddlcart;
var ddlpath;
var slidepath;

function AddItemToCart() {

    var cart = $("div.cart-items ul")
    // How many items are in the cart currently
    var index = cart.children().length - 1;

    var specimen = $(ddlcart).closest("tbody[data-group]");

    // Retrieve the case number
    var id = $(specimen).attr("data-group");

    // Get the current pathologist assigned
    var path = $(ddlcart).val();

    var slidecount = 0;

    slidecount = parseInt($(specimen).attr("data-slidecount"));

    // Calculate the number of slides
    //$(specimen).find("td.slidecount").each(function () {
    //    slidecount += parseInt($(this).text());
    //});

    // Build a template of the new li to be added to the cart
    // This template is defined in the Cart.cshtml page
    var newli = [
        {
            index: index,
            id: id,
            slidecount: slidecount,
            DateAssigned: $("#dteDateAssigned").text(),
            Path: path,
            comment: GetComment()
        }
    ];

    // Render the new li and append it to the cart
    $("#newli").tmpl(newli).appendTo(cart);

    $("#applycart").removeAttr('disabled');
}

function UpdateCounts()
{
    $("div.cart-items[id='moveslides'] table tr[data-content] td:nth-child(2)").text("0");

    var paths = {};

    $("tbody[data-group]").each(function () {
        var path = $(this).find("select").val();

        var slidecount = parseInt($(this).attr("data-slidecount"));
        // var slidecount = 0;

        //// Calculate the number of slides
        //$(this).find("td.slidecount").each(function () {
        //    slidecount += parseInt($(this).text());
        //});
        var currentcount = parseInt(paths[path] || 0);
        paths[path] = currentcount + slidecount;
    });

    $.each(paths, function (path, slidecount) {
        $("div.cart-items[id='moveslides'] table tr[data-content='" + path + "'] td:nth-child(3)").text(slidecount)
    });

    $("div.cart-items ul li[data-content]").each(function () {
        var path = $(this).find("input[id='cases_chrPath']").val();
        var slidecount = parseInt($(this).find("input[id='cases_intSlideCount']").val())
        var counter = $("div.cart-items[id='moveslides'] table tr[data-content='" + path + "'] td:nth-child(2)");
        var counterslidecount = parseInt($(counter).text());
        $(counter).text((counterslidecount + slidecount).toString());
    });
}

function GetComment() {
    return $.trim($("#inputComment").val()).length === 0 ? $("#radiooverlimit").val() : $.trim($("#inputComment").val());
}

function UpdateItemInCart() {
    var cartitem = $("div.cart-items ul li[data-content='" + $(ddlcart).closest("tbody[data-group]").attr("data-group") + "']");

    $(cartitem).find("input[name$='chrPath']").val($(ddlcart).val());
    $(cartitem).find("input[name$='chrComment']").val(GetComment());
}

$(document).ready(function () {

    $.ajaxSetup({ cache: false });

    $("#StartAccession").datepicker({
        minDate: -5,
        maxDate: 0,
        altFormat: "mm/dd/yy",
        dateFormat: 'mm/dd/yy',
    }).on("change", function () {
        var d = new Date($(this).val());
        var ed = new Date(d);
        if (ed.getDay() == 5) {
            var x = new Date();
            ed.setDate(x.getDate() - 1);
        } else {
            ed.setDate(ed.getDate() + 1);
        }
        $("#EndAccession").datepicker("option", "minDate", d)
                          .datepicker("option", "maxDate", ed)
                          .val(ed.toLocaleDateString('en-US', { year: 'numeric', month: '2-digit', day: '2-digit' }));
    });

    $("#EndAccession").datepicker({
        minDate: -5,
        maxDate: 0,
        altFormat: "mm/dd/yy",
        dateFormat: 'mm/dd/yy'
    }).on("change", function () {
        $("#StartAccession").datepicker("option", "maxDate", $(this).val());
    });

    $("#ResetDist").datepicker({
        minDate: -5,
        maxDate: 5,
        altFormat: "mm/dd/yy",
        dateFormat: 'mm/dd/yy'
    }).on("change", function () {
        $("#ResetDist").datepicker("option", "maxDate", $(this).val());
    });

    $("#StartProceed").click(function () {
        $.ajax({
            url: "/Start/IsProcessing",
            type: "GET",
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            data: {
                dte1: (new Date($("#StartAccession").val())).toISOString(),
                dte: (new Date($("#EndAccession").val())).toISOString(),
                Priority: $("#Priority").val()
            },
            success: function (data) {
                switch (data.result) {
                    case 1:
                        if (!confirm("The date selected is currently in process.  Do you wish to continue?")) {
                            $("#StartProceed").prop("disabled", false).text("Proceed");
                            return false;
                        }
                        break;
                    case 2:
                        if (!confirm("The date selected has already been generated.  Do you wish to continue?")) {
                            $("#StartProceed").prop("disabled", false).text("Proceed");
                            return false;
                        }
                        break;
                    case 3:
                        alert("The selected start date has already been generated.  Please reselect.");
                        return false;
                        break;
                    case 4:
                        alert("The selected end date cannot be in the future.  Please reselect.");
                        return false;
                        break;
                }
                $("#StartProceed").prop("disabled", true).text("Working...");
                $("#StartForm").submit();
            }
        });
    });

    $("#StartReset").click(function () {
        if (!confirm("You will only be able to delete if the date has not been yet generated.  Do you wish to continue?")) {
            $("#StartReset").prop("disabled", false).text("Reset");
            return false;
        }
        $("#StartReset").prop("disabled", true).text("Working...");
        $("#ResetForm").submit();
    });

    //click event to expand/collapse:
    //  1. pathologist cases
    //  2. case details
    $("#content table tr.text5").on('click', 'td.pm', function () {
        // Are we working with case details
        if ($(this).closest("tbody[data-group]").length > 0) {
            $(this).toggleClass("plus minus").closest("tbody[data-group]").children("tr:not(:first-child)").toggle();
        }
        else {
            $(this).toggleClass("plus minus").closest("tr").next().toggle();
        }
    });

    // On focus store the current value so that we can revert 
    $("tbody[data-group]").on('focus', "select", function () {
        $(this).data('lastValue', $(this).val());
    });

    // The user is changing the path associated with a case.
    // We need to prompt to ask why
    $("tbody[data-group]").on('change', "select", function () {
        // Show modal boxes
        $("#myModal").css('visibility', 'visible');
        $("#myModal2").css('visibility', 'visible');
        // Keep track of which drop down we are messing with
        ddlcart = this;
        // Put in the comment that was last entered
        var cartitem = $("div.cart-items ul li[data-content='" + $(ddlcart).closest("tbody[data-group]").attr("data-group") + "']");

        if ($(this).siblings("span").text() === "Preassigned") {
            $("#overlimitdiv").hide();
            $("#inputComment").val($(cartitem).find("input[name$='chrComment']").val());
        }
        else {
            $("#overlimitdiv").show();
            if ($(cartitem).find("input[name$='chrComment']").val() === $("#radiooverlimit").val()) {
                $("#radiooverlimit").prop("checked", true);
                $("#inputComment").val("");
            } else {
                $("#radiooverlimit").prop("checked", false);
                $("#inputComment").val($(cartitem).find("input[name$='chrComment']").val());
            }
        }
    });

    // The user is canceling the request to change the path on a case
    $("#closecartmodal").click(function () {
        // Hide the modal dialogs
        $("#myModal").css('visibility', 'hidden');
        $("#myModal2").css('visibility', 'hidden');
        // Revert the select to the prior value
        $(ddlcart).val($(ddlcart).data("lastValue"));
    });

    // The user is applying the comment to the case they just change the
    // path on in the cart
    $("#applycartmodal").click(function () {
        // Make sure there is a comment
        if ($.trim($("#inputComment").val()).length === 0
            && !$("#radiooverlimit").is(":checked")) {
            // Notify user a comment is required
            alert('You must enter a comment!');
        } else {
            // Hide the dialogs
            $("#myModal").css('visibility', 'hidden');
            $("#myModal2").css('visibility', 'hidden');
                    
            if ($("div.cart-items ul li[data-content='" + $(ddlcart).closest("tbody[data-group]").attr("data-group") + "']").length === 0) {
                AddItemToCart();
            } else if ($(ddlcart).closest("table[id^=details").attr("id").indexOf($(ddlcart).val()) > 0) {
                $("div.cart-items ul li[data-content='" + $(ddlcart).closest("tbody[data-group]").attr("data-group") + "']").remove();
            } else {
                UpdateItemInCart();
            }

            UpdateCounts();
        }
    });

    // The user is adding a path to the Off schedule
    $("#addoffpath").click(function () {
        // Clear out the modal dialog
        $("#OffType").val("");
        $("#OffPath").val("");
        $("#OffComment").val("");
        // Show the dialogs
        $("#myModal").css('visibility', 'visible');
        $("#myModal3").css('visibility', 'visible');
    });

    // The user is canceling the addition of the Off schedule
    $("#closeopmodal").click(function () {
        // Hide the modal dialogs
        $("#myModal").css('visibility', 'hidden');
        $("#myModal3").css('visibility', 'hidden');
    });

    // The user is adding a path to the off schedule
    $("#applyopmodal").click(function () {
        //TODO: Add validation to make sure both fields are filled in
        if (!$("#OffType").val()) {
            alert("Please provide the type of Off assignment the path is taking.");
        } else if (!$("#OffPath").val()) {
            alert("Please provide the Path that the Off assignment is for.");
        } else {
            
            if ($("#OffType option:selected").data("isfulloff")) {
                $("#pathoffschedule td:contains(" + $("#OffPath").val() + ")").next(".deleteoff").click();
            }

            // Hide the modal dialogs
            $("#myModal").css('visibility', 'hidden');
            $("#myModal3").css('visibility', 'hidden');

            // How many items are in the cart currently
            var index = $("#pathoffschedule tr.text5").length;

            var newtr = [
                {
                    index: index,
                    OffType: $("#OffType").val(),
                    OffPath: $("#OffPath").val(),
                    OffComment: $("#OffComment").val(),
                    DateAssigned: $("#dteDateAssigned").text()
                }
            ];

            // Insert into the list just before the buttons
            $("#pathoffschedule tr:last").before($("#newtr").tmpl(newtr));
        }
    });

    // On focus store the current value so that we can revert 
    $("#pathschedule").on('focus', "select", function () {
        $(this).data('lastValue', $(this).val());
    });

    // The user is changing the path associated with an assignment.
    // We need to prompt to ask why
    $("#pathschedule").on('change', "select", function () {
        event.stopPropagation();
        // Show modal boxes
        $("#myModal").css('visibility', 'visible');
        $("#myModal4").css('visibility', 'visible');
        // Keep track of which drop down we are messing with
        ddlpath = this;
        // Put in the comment that was last entered
        $("#pathComment").val($(this).closest("td").find("input[name$='chrOverComment']").val());
    });

    // The user is canceling the request to change the path on an assignment
    $("#closepathmodal").click(function () {
        // Hide the modal dialogs
        $("#myModal").css('visibility', 'hidden');
        $("#myModal4").css('visibility', 'hidden');
        // Revert the select to the prior value
        $(ddlpath).val($(ddlpath).data("lastValue"));
        $(ddlpath).prev().show();
        $(ddlpath).hide();
    });

    // The user is applying the comment to the assignment they just change the
    // path on in the schedule
    $("#applypathmodal").click(function () {
        // Make sure there is a comment
        if ($.trim($("#pathComment").val()).length === 0) {
            // Notify user a comment is required
            alert('You must enter a comment!');
        } else {
            // Hide the dialogs
            $("#myModal").css('visibility', 'hidden');
            $("#myModal4").css('visibility', 'hidden');
            // Update the comment in the assignment
            $(ddlpath).closest("td").find("input[name$='chrOverComment']").val($.trim($("#pathComment").val()));
            $(ddlpath).prev().text($.trim($(ddlpath).val()));
            $(ddlpath).next().val($.trim($(ddlpath).val()));
            $(ddlpath).prev().show();
            $(ddlpath).hide();
        }
    });

    // Replace text with dropdown to change path schedule
    $("div.changePath").click(function () {
        // Prevent the event from being used elsewhere
        event.stopPropagation();
        // Hide the div
        $(this).hide();
        // Set the dropdown to match the text
        $(this).next().val($(this).text());
        // Show the dropdown
        $(this).next().show();
    });

    // The user has lost focus of the dropdown so revert back to text
    $("#pathschedule select").focusout(function () {
        // Only do this if the modal dialog is not showing
        if ($("#myModal").css("visibility") === "hidden") {
            // Hide the dropdown and show the text
            if ($(this).prev().val().length > 0) {
                $(this).hide();
                $(this).prev().show();
            }
        }
    });

    // Capture when the user wants to lose focus on the dropdown
    $(document).click(function (event) {
        if (event.target.tagName !== "SELECT") {
            // Revert any currently visible dropdowns back to text
            $("#pathschedule select:visible").each(function () {
                if ($("#myModal").css("visibility") === "hidden") {
                    if ($(this).prev().val().length > 0) {
                        $(this).hide();
                        $(this).prev().show();
                    }
                }
            });
        }
    });

    $("#Generate").click(function () {
        event.stopPropagation();

        if (confirm("Are you sure you want to apply this distribution.  This is permanent and cannot be changed.  This will replace the pathologist already assigned.")) {
            $("#Generate").prop("disabled", true).text("Working...");
            $("#GenerateData").submit();
        }
    });

    $("#applycart").click(function () {
        event.stopPropagation();
        $("#MoveCart").submit();
    });

    $("#pathoffschedule").on('click', "td.deleteoff", function () {

        $(this).find("input[name$='bitOverride']").val("True");
        $(this).find("input[name$='bitDelete']").val("True");
        $(this).closest('tr').hide();
    });

    $('#btnBreakdown').click(function () {
        $("#myModal").css('visibility', 'visible');
        $("#typeBreakdownDiv").css('visibility', 'visible');
    });

    $('#closeBreakdown').click(function () {
        $("#myModal").css('visibility', 'hidden');
        $("#typeBreakdownDiv").css('visibility', 'hidden');
    });

    // The user is changing the path associated with a case.
    // We need to prompt to ask why
    $("#addslides tr[data-content]").on('focusout', "input", function () {
        // Show modal boxes
        $("#myModal").css('visibility', 'visible');
        $("#myModalSlide").css('visibility', 'visible');
        // Keep track of which drop down we are messing with
        slidepath = this;
        $("#inputSlideComment").focus();
        $("#inputSlideComment").val($(slidepath).parent().prev().find("input[name$='chrComment']").val());        
    });

    // The user is applying the comment to the case they just change the
    // path on in the cart
    $("#applyslidemodal").click(function () {
        // Make sure there is a comment
        if ($.trim($("#inputSlideComment").val()).length === 0) {
            // Notify user a comment is required
            alert('You must enter a comment!');
        } else {
            // Hide the dialogs
            $("#myModal").css('visibility', 'hidden');
            $("#myModalSlide").css('visibility', 'hidden');

            // Update the comment in the assignment
            $(slidepath).parent().prev().find("input[name$='chrComment']").val($.trim($("#inputSlideComment").val()));

        }
    });
});