var DataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    DataTable = $('#myTable').DataTable({
        "ajax": { url: '/admin/user/getall' },
        "columns":
            [
                { data: 'name', "width": "15%" },
                { data: 'email', "width": "15%" },
                { data: 'phoneNumber', "width": "15%" },
                { data: 'company.name', "width": "15%" },
                { data: 'role', "width": "10%" },
                {
                    data: {id:"id", lockoutEnd:"lockoutEnd"},
                    "render": function (data) {
                        var today = new Date().getTime();
                        var lockoutEnd = new Date(data.lockoutEnd).getTime();
                        if (lockoutEnd > today) {
                            return `
                            <div class="text-center">
                                <a onclick=Lockunlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    <i class="bi bi-lock-fill"></i> Locked
                                </a>
                                <a href="/admin/user/roleManagment?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                            
                            `
                        }
                        else {
                            return `
                            <div class="text-center">
                                <a onclick=Lockunlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="bi bi-unlock-fill"></i> Unlock
                                </a>
                                <a href="/admin/user/roleManagment?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                            
                            `
                        }
                        
                    },
                    "width": "30%"
                },
            ]
    });
}
function Lockunlock(id) {
    $.ajax({
        type: "POST",
        url: '/admin/user/lockunlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                DataTable.ajax.reload();
            }
        }
    })
}