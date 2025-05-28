var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "responsive": true, 
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "email", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "company",
                "render": function (data, type, row) {
                    return data ? data.name : (row.companyId ? "Company ID: " + row.companyId : "None");
                },
                "width": "10%"
            },
            { "data": "role", "width": "10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = data.lockoutEnd ? new Date(data.lockoutEnd).getTime() : 0;
                    var isLocked = lockout > today;

                    return `
                        <div class="text-center d-flex justify-content-center gap-2">
                            <a onclick="LockUnlock('${data.id}')" class="btn ${isLocked ? 'btn-danger' : 'btn-success'} text-white btn-sm" style="min-width:100px;">
                                <i class="bi ${isLocked ? 'bi-lock-fill' : 'bi-unlock-fill'}"></i> ${isLocked ? 'Lock' : 'Unlock'}
                            </a>
                            <a href="/admin/user/RoleManagment?userId=${data.id}" class="btn btn-primary text-white btn-sm" style="min-width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>
                    `;
                },
                "width": "25%",
                "orderable": false 
            }
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        },
        error: function () {
            toastr.error("An error occurred while processing the request.");
        }
    });
}