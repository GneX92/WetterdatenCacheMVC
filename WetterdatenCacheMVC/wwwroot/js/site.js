function displayContent(index) {
    $("#" + index).slideToggle("slow");
}

$(".expandable").on("click",
    function () {
        var accordionRow = $(this).next(".hidden_row");
        if (!accordionRow.is(":visible")) {
            accordionRow.slideDown("fast").find(".hidden_cell").slideDown("fast");
        } else {
            accordionRow.find(".hidden_cell").slideUp("fast", function () {
                if (!$(this).is(':visible')) {
                    accordionRow.slideUp("fast");
                }
            });
        }
    });