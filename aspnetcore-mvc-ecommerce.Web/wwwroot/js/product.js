// Initializes the DataTable when the document is ready
$(document).ready(function () {
    loadDataTable();
});

// Loads and configures the product DataTable with API data
function loadDataTable() {
    let dataTable = $('#tblData').DataTable({
        // Fetches product data from the API endpoint
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: "title", "width": "25%" },
            { data: "author", "width": "15%" },
            { data: "isbn", "width": "15%" },
            // Accesses nested category name via dot notation
            { data: "category.name", "width": "10%" },
            { data: "listPrice", "width": "10%" },
            {
                data: "id",
                // Renders Edit and Delete action buttons for each row
                "render": function (data) {
                    return `<div class="btn-group">
                                <a href="/admin/product/upsert?id=${data}" class="btn btn-primary btn-sm">Edit</a>
                                <a onclick="Delete('/Admin/Product/DeleteAjax/${data}')" class="btn btn-sm btn-danger mx-1">Delete</a>
                            </div>`;
                },
                "width": "25%"
            }
        ]
    });
}

// Handles product deletion via SweetAlert2 confirmation dialog and AJAX
function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        // Reloads the DataTable without full page refresh
                        $('#tblData').DataTable().ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    // Handles unexpected errors during deletion
                    toastr.error("An unexpected error occurred. Please try again.");
                }
            });
        }
    });
}