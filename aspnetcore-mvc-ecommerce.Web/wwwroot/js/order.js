// Initializes the DataTable when the document is ready
$(document).ready(function () {
    loadDataTable("all");
});

// Loads and configures the order DataTable with API data filtered by status
function loadDataTable(status) {
    // Destroys existing DataTable instance before reinitializing with new filter
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    let dataTable = $('#tblData').DataTable({
        // Fetches order data from the API endpoint with status filter
        "ajax": { url: `/admin/order/getall?status=${status}` },
        "columns": [
            { data: "id", "width": "5%" },
            { data: "name", "width": "15%" },
            { data: "phoneNumber", "width": "15%" },
            // Accesses nested email via ApplicationUser navigation property
            { data: "applicationUser.email", "width": "15%" },
            { data: "orderStatus", "width": "10%" },
            {
                // Formats order total as currency
                data: "orderTotal",
                "width": "10%",
                "render": function (data) {
                    return `$${parseFloat(data).toFixed(2)}`;
                }
            },
            {
                data: "id",
                // Renders Details action button for each row
                "render": function (data) {
                    return `<div class="btn-group">
                                <a href="/admin/order/details?orderId=${data}"
                                   class="btn btn-primary btn-sm">
                                   <i class="fa-solid fa-eye fa-xs me-1"></i>Details
                                </a>
                            </div>`;
                },
                "width": "15%"
            }
        ]
    });
}