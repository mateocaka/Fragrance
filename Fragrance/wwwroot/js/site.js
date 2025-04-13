$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/order/getall',
            type: 'GET',
            dataType: 'json',
            dataSrc: 'data'
        },
        columns: [
            { data: 'id', width: "5%" },
            { data: 'name', width: "15%" },
            { data: 'phoneNumber', width: "10%" },
            {
                data: 'applicationUser.email',
                render: function (data) {
                    return data || 'N/A';
                },
                width: "20%"
            },
            {
                data: 'orderStatus',
                render: function (data) {
                    if (data === 'Approved') {
                        return `<span class="badge bg-success">${data}</span>`;
                    } else if (data === 'Pending') {
                        return `<span class="badge bg-warning text-dark">${data}</span>`;
                    } else if (data === 'Cancelled') {
                        return `<span class="badge bg-danger">${data}</span>`;
                    }
                    return `<span class="badge bg-secondary">${data}</span>`;
                },
                width: "10%"
            },
            {
                data: 'orderTotal',
                render: function (data) {
                    return '$' + parseFloat(data).toFixed(2);
                },
                width: "10%"
            },
            { data: 'orderDate', width: "15%" },
            {
                data: 'id',
                render: function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-info mx-2">
                                <i class="bi bi-pencil-square"></i> Details
                            </a>
                        </div>
                    `;
                },
                width: "15%"
            }
        ],
        language: {
            emptyTable: "No orders found."
        }
    });
}

function updateOrderStatus(orderId, status) {
    Swal.fire({
        title: 'Are you sure?',
        text: `You are about to ${status.toLowerCase()} this order`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, proceed!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/Order/UpdateOrderStatus',
                type: 'POST',
                data: {
                    orderId: orderId,
                    status: status
                },
                success: function (response) {
                    if (response.success) {
                        Swal.fire(
                            'Success!',
                            response.message,
                            'success'
                        );
                        dataTable.ajax.reload();
                    } else {
                        Swal.fire(
                            'Error!',
                            response.message,
                            'error'
                        );
                    }
                }
            });
        }
    });
}