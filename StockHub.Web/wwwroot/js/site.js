var table;
var datatable;
var updatedRow;
var exportedCols = [];

function ShowSuccessMessage(message = 'Saved successfully!') {
    
    try {
        Swal.fire({
            icon: 'success',
            title: 'Good Job',
            text: message,
            customClass: {
                confirmButton: "btn btn-primary"
            }
        });
    } catch (e) {
        console.error('SweetAlert2 error: ', e);
    }
}


function ShowErrorMessage(message = 'Something went wrong!') {
    Swal.fire({
        icon: 'error',
        title: 'Oops...',
        text: message,
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
}

function onModalBegin() {
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

function onModalSuccess(row) {
    debugger
    $('#Modal').modal('hide');
    ShowSuccessMessage();


    if (updatedRow !== undefined) {
        datatable.row(updatedRow).remove().draw();
        updatedRow = undefined;
    }


    var newRow = $(row);
    datatable.row.add(newRow).draw();

    KTMenu.init();
    KTMenu.initHandlers();
}


function onModalComplete() {
    $('body :submit').removeAttr('disabled').removeAttr('data-kt-indicator');
}

//DataTables
var headers = $('th');
$.each(headers, function (i) {
    if (!$(this).hasClass('js-no-export'))
        exportedCols.push(i);
});

// Class definition
var KTDatatables = function () {
    // Private functions
    var initDatatable = function () {
        // Init datatable --- more info on datatables: https://datatables.net/manual/
        datatable = $(table).DataTable({
            "info": false,
            'pageLength': 10

        });
    }

    // Hook export buttons
    var exportButtons = () => {
        
        const documentTitle = $('.js-datatables').data('document-title');
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'copyHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'excelHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'csvHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                },
                {
                    extend: 'pdfHtml5',
                    title: documentTitle,
                    exportOptions: {
                        columns: exportedCols
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_example_buttons'));

        // Hook dropdown menu click event to datatable export buttons
        const exportButtons = document.querySelectorAll('#kt_datatable_example_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
           
            exportButton.addEventListener('click', e => {
                
                e.preventDefault();

                // Get clicked export value
                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);

                // Trigger click event on hidden datatable export buttons
                target.click();
            });
        });
    }

    // Search Datatable --- official docs reference: https://datatables.net/reference/api/search()
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        if (filterSearch) {  // Check if the element exists before adding the event listener
            filterSearch.addEventListener('keyup', function (e) {
                datatable.search(e.target.value).draw();
            });
        }
    };


    // Public methods
    return {
        init: function () {
            table = document.querySelector('.js-datatables');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();

function disableSubmitButton() {//Note: this change the submit btn to loading indicator
    $('body :submit').attr('disabled', 'disabled').attr('data-kt-indicator', 'on');
}

$(document).ready(function () {
    $('form').not('#SignOut').on('submit', function () {
        var isValid = $(this).valid();
        if (isValid) disableSubmitButton();
    });
    //SweetAlert
    var message = $('#Message').text();
    if (message !== '') {
        ShowSuccessMessage(message);
    }

    //DataTables
    KTUtil.onDOMContentLoaded(function () {
        KTDatatables.init();
    });

    //Handle bootstrap modal
    $('body').delegate('.js-render-modal', 'click', function () {
        debugger
        var btn = $(this);
        var modal = $('#Modal');

        modal.find('#ModalLabel').text(btn.data('title'));

        if (btn.data('update') !== undefined) {
            updatedRow = btn.parents('tr');
        }

        $.get({
            url: btn.data('url'),
            success: function (form) {
                debugger
                modal.find('.modal-body').html(form);
                if ($.validator && $.validator.unobtrusive) {
                    $.validator.unobtrusive.parse(modal);
                } else {
                    console.warn('Unobtrusive validation not available.');
                }
            },
            error: function () {
                
                ShowErrorMessage();
            }
        });


        modal.modal('show');
    });
    //Handle Toggle Status
    $('body').delegate('.js-toggle-status', 'click', function () {
        var btn = $(this);

        Swal.fire({
            title: "Are you sure you need to toggle this item status?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Yes, change it!"
        }).then((result) => {
            if (result.isConfirmed) {
                $.post({
                    url: btn.data('url'),
                    type: 'post',
                    data: {
                        '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (lastUpdatedOn) {
                        
                        var row = btn.parents('tr')
                        var status = row.find('.js-status');
                        var newStatus = status.text().trim() === 'Deleted' ? 'Available' : 'Deleted';
                        status.text(newStatus).toggleClass('badge-light-success badge-light-danger');
                        row.find('.js-updated-on').html(lastUpdatedOn);
                        ShowSuccessMessage();
                    },
                    error: function () {
                        ShowErrorMessage();

                    }
                });
            }
        });
    });

    //Handle Confirm
    $('body').delegate('.js-confirm', 'click', function () {
        var btn = $(this);
        Swal.fire({
            title: btn.data('message'),
            icon: "warning", // Set icon to warning (optional)
            showCancelButton: true,
            confirmButtonColor: "#3085d6", // Set confirm button color (optional)
            cancelButtonColor: "#d33", // Set cancel button color (optional)
            confirmButtonText: "Yes", // Set confirm button text
            cancelButtonText: "No" // Set cancel button text
        }).then((result) => {
            if (result.isConfirmed) {
                $.post({
                    url: btn.data('url'),
                    data: {
                        '__RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function
                        () {
                        ShowSuccessMessage();
                    },
                    error: function () {
                        ShowErrorMessage();

                    }
                });
            }
        });
    });

    $('.js-signout').on('click', function () {
        $('#SignOut').submit();

    });
});