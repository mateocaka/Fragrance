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

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/order/getall',
            type: 'GET',
            dataType: 'json',
            dataSrc: 'data' // Përputhet me { data: [...] }
        },
        "columns": [
            { data: 'id', width: "5%" },
            { data: 'name', width: "25%" }, // 'name' nga OrderHeader
            { data: 'phoneNumber', width: "20%" },
            {
                data: 'applicationUser.email', // Email direkt nga ApplicationUser
                render: function (data) {
                    return data || 'N/A';
                },
                width: "20%"
            },
            { data: 'orderStatus', width: "10%" },
            {
                data: 'orderTotal',
                render: function (data) {
                    return '$' + data.toFixed(2); // Formato si valutë
                },
                width: "10%"
            }
        ],
        "language": {
            "emptyTable": "No orders found."
        }
    });
}