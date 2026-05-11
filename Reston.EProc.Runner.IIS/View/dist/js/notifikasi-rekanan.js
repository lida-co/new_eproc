
// Setup CSRF token untuk semua AJAX POST request di halaman rekanan
// (karena dist/site.js tidak memiliki $.ajaxSetup untuk CSRF)
(function () {
    var _csrfToken = "";

    function fetchCsrfToken(callback) {
        if (_csrfToken) {
            callback(_csrfToken);
            return;
        }
        $.ajax({
            url: "/api/security/GetCsrfToken",
            method: "GET",
            success: function (data) {
                if (data && data.csrfToken) {
                    _csrfToken = data.csrfToken;
                    // Setup global agar semua AJAX POST berikutnya otomatis pakai token
                    $.ajaxSetup({
                        beforeSend: function (xhr, settings) {
                            if (settings.type !== 'GET') {
                                xhr.setRequestHeader("X-CSRF-TOKEN", _csrfToken);
                            }
                        }
                    });
                    callback(_csrfToken);
                }
            },
            error: function () {
                console.warn("Gagal mengambil CSRF token untuk notifikasi");
                callback(null);
            }
        });
    }

    $(function () {
        fetchCsrfToken(function () {
            getNotifikasi();
        });
    });

    function getNotifikasi() {
        $.ajax({
            method: "POST",
            url: "Api/PengadaanE/getRiwayatDokumenVendor",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                if (data && data.length > 0) {
                    $("#totalNotif").html(data.length);
                    $("#headerNotif").html("Ada " + data.length + " Notifikasi");
                    var lstHtml = "";
                    for (var index in data) {
                        var judul = DOMPurify.sanitize(data[index].JudulPengadaan || "");
                        var status = DOMPurify.sanitize(data[index].Status || "");
                        lstHtml += '<li>' +
                            '<a href="#">' +
                                '<h4>' + judul + '</h4>' +
                                '<p>Status Pengadaan: ' + status + '</p>' +
                            '</a>' +
                        '</li>';
                    }
                    $("#listNotif").html(lstHtml);
                } else {
                    $("#totalNotif").html("0");
                    $("#headerNotif").html("Tidak ada notifikasi");
                }
            },
            error: function () {
                // Gagal load notifikasi, tidak perlu tampilkan error ke user
            }
        });
    }
})();
