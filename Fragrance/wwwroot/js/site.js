$('#liveSearch').on('input', debounce(function () {
    $('#perfumeResults').html('<div class="text-center"><i class="fas fa-spinner fa-spin"></i> Loading...</div>');
    // rest of AJAX call
}, 300));

@section Scripts {
    <script>
        $(document).ready(function() {
            // Handle form submission
            $('#filterForm').on('submit', function (e) {
                e.preventDefault();
                applyFilters();
            });

        // Handle search input changes (with debounce)
        let searchTimer;
        $('#searchInput').on('input', function() {
            clearTimeout(searchTimer);
        searchTimer = setTimeout(applyFilters, 500);
            });

        // Handle filter removal
        $('.remove-filter').on('click', function(e) {
            e.preventDefault();
        const filterType = $(this).data('filter');
        $(`input[name="${filterType}"]`).remove();
        applyFilters();
            });

        // Handle gender/brand/rating links
        $('a[href^="#"]').not('.remove-filter').on('click', function(e) {
            e.preventDefault();
        applyFilters();
            });

        function applyFilters() {
                const formData = $('#filterForm').serialize();

        // Get any additional filters from clicked links
        const urlParams = new URLSearchParams(window.location.search);
                urlParams.forEach((value, key) => {
                    if (!formData.includes(key) && key !== 'search') {
            formData += `&${key}=${encodeURIComponent(value)}`;
                    }
                });

        $.ajax({
            url: '@Url.Action("FilterPerfumes", "Home")',
        type: 'GET',
        data: formData,
        success: function(data) {
            $('#perfumeListContainer').html(data);
        history.pushState(null, '', '?' + formData);
                    },
        error: function() {
            alert('Error applying filters');
                    }
                });
            }

        // Brand search functionality
        $('#brandSearch').on('input', function(e) {
                const searchTerm = e.target.value.toLowerCase();
        const brands = $('#brandList a');

        brands.each(function() {
                    const brandText = $(this).text().toLowerCase();
        $(this).toggle(brandText.includes(searchTerm));
                });
            });
        });
    </script>
}


$(document).ready(function () {
    // Check if DataTable already exists
    if ($.fn.DataTable.isDataTable('#tblData')) {
        // If it exists, destroy it first
        $('#tblData').DataTable().destroy();
    }

    // Initialize DataTable
    var dataTable = $('#tblData').DataTable({
        ajax: {
            url: '/admin/order/getall',
            type: 'GET',
            dataType: 'json',
            dataSrc: 'data'
        },
        columns: [
            { data: 'id', width: "5%" },
            { data: 'name', width: "25%" },
            { data: 'phoneNumber', width: "20%" },
            {
                data: 'applicationUser.email',
                render: function (data) {
                    return data || 'N/A';
                },
                width: "20%"
            },
            { data: 'orderStatus', width: "10%" },
            {
                data: 'orderTotal',
                render: function (data) {
                    return '$' + parseFloat(data).toFixed(2);
                },
                width: "10%"
            }
        ],
        language: {
            emptyTable: "No orders found."
        }
    });
});