// Baca ID dari hash. Jika hash bukan GUID (misal berubah karena klik accordion),
// fallback ke sessionStorage agar tidak redirect ke list saat accordion diklik.
var id_pengadaan = DOMPurify.sanitize(window.location.hash).replace(/^#/, '');
if (!isGuid(id_pengadaan)) {
    var _stored = sessionStorage.getItem("rekanan_pengadaan_id");
    if (_stored && isGuid(_stored)) {
        id_pengadaan = _stored;
    }
}

$(function () {

    if (isGuid(id_pengadaan)) {
        sessionStorage.setItem("rekanan_pengadaan_id", id_pengadaan);
        $("#pengadaanId").val(id_pengadaan);
        loadData(id_pengadaan);
        loadStatus(id_pengadaan);
    } else {
        var existingId = $("#pengadaanId").val();
        if (isGuid(existingId)) {
            sessionStorage.setItem("rekanan_pengadaan_id", existingId);
            window.location.hash = existingId;
            loadData(existingId);
            loadStatus(existingId);
        } else {
            window.location.replace("/pengadaan-rekanan.html");
        }
    }

    // cek asuransi
    cekrksbiasapaasuransi();

    $("#tab-pelakasanaan").on("click", function () {
        $("#side-kanan").find(".rk").hide();
        $("#side-kanan").find(".pl").show();
    });
    $("#tab-berkas").on("click", function () {
        $("#side-kanan").find(".rk").hide();
        $("#side-kanan").find(".pl").hide();
    });
    $("#tab-rk").on("click", function () {
        $("#side-kanan").find(".rk").show();
        $("#side-kanan").find(".pl").hide();
    });

    $("#side-kanan").find(".pl").hide();

    $("#myNav").affix({
        offset: {
            top: 100
        }
    });

    $(".tab-content").show();

    $("#side-kanan").on("click", "li", function () {
        $("#side-kanan").find("li").each(function () {
            $(this).removeClass("active");
        });
        $(this).addClass("active");
    });

    // Fix: Cegah accordion links mengubah window.location.hash ke nilai non-GUID
    // yang menyebabkan redirect ke list saat halaman di-refresh.
    $("#accordion").on("click", "a[data-parent='#accordion']", function (e) {
        e.preventDefault();
        var target = $(this).attr("href");
        if (!target) return;
        var $target = $(target);
        if ($target.length === 0) return;

        var isOpen = $target.hasClass("in");
        // Tutup semua panel accordion
        $("#accordion .panel-collapse.in").collapse("hide");
        // Buka panel yang diklik jika sebelumnya tertutup
        if (!isOpen) {
            $target.collapse("show");
        }
    });

    //dropzone
    var myDropzoneBerkasRujukanLain = new Dropzone("#BerkasRujukanLain", {
        url: $("#BerkasRujukanLain").attr("action") + "&id=" + $("#pengadaanId").val(),
        maxFilesize: 5,
        acceptedFiles: "",
        clickable: false,
        dictDefaultMessage: "Tidak Ada Dokumen",
        init: function () {
            this.on("addedfile", function (file) {
                file.previewElement.addEventListener("click", function () {
                    var fileId = null;
                    if (file.Id != undefined) {
                        fileId = file.Id;
                    } else {
                        try {
                            var parsed = JSON.parse(file.xhr.response);
                            fileId = (parsed && parsed.Id !== undefined) ? parsed.Id : null;
                        } catch (e) {
                            console.error("Invalid JSON response", e);
                            fileId = null;
                        }
                    }
                    $("#HapusFile").hide();
                    $("#konfirmasiFile").attr("attr1", "BerkasRujukanLain");
                    $("#konfirmasiFile").attr("FileId", fileId);
                    $("#konfirmasiFile").modal("show");
                });
            });
        }
    });
    renderDokumenDropzone(myDropzoneBerkasRujukanLain, "BerkasRujukanLain");
    Dropzone.options.BerkasRujukanLain = false;

    var myDropzoneDokumenLain = new Dropzone("#DokumenLain", {
        url: $("#DokumenLain").attr("action") + "&id=" + $("#pengadaanId").val(),
        maxFilesize: 5,
        acceptedFiles: "",
        clickable: false,
        dictDefaultMessage: "Tidak Ada Dokumen",
        init: function () {
            this.on("addedfile", function (file) {
                file.previewElement.addEventListener("click", function () {
                    var fileId = null;
                    if (file.Id != undefined) {
                        fileId = file.Id;
                    } else {
                        try {
                            var parsed = JSON.parse(file.xhr.response);
                            fileId = (parsed && parsed.Id !== undefined) ? parsed.Id : null;
                        } catch (e) {
                            console.error("Invalid JSON response", e);
                            fileId = null;
                        }
                    }
                    $("#HapusFile").hide();
                    $("#konfirmasiFile").attr("attr1", "DokumenLain");
                    $("#konfirmasiFile").attr("FileId", fileId);
                    $("#konfirmasiFile").modal("show");
                });
            });
        }
    });
    renderDokumenDropzone(myDropzoneDokumenLain, "DOKUMENLAIN");
    Dropzone.options.DokumenLain = false;

    var myDropzoneBerkasRekanan = new Dropzone("#BerkasRekanan", {
        url: $("#BerkasRekanan").attr("action") + "&id=" + $("#pengadaanId").val(),
        maxFilesize: 5,
        acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.docx,.xlsx",
        dictDefaultMessage: "Tidak Ada Dokumen",
        init: function () {
            this.on("addedfile", function (file) {
                file.previewElement.addEventListener("click", function () {
                    var fileId = null;
                    if (file.Id != undefined) {
                        fileId = file.Id;
                    } else {
                        try {
                            var parsed = JSON.parse(file.xhr.response);
                            fileId = (parsed && parsed.Id !== undefined) ? parsed.Id : null;
                        } catch (e) {
                            console.error("Invalid JSON response", e);
                            fileId = null;
                        }
                    }
                    $("#konfirmasiFile").attr("attr1", "BerkasRekanan");
                    $("#konfirmasiFile").attr("FileId", fileId);
                    $("#konfirmasiFile").modal("show");
                });
            });
            this.on("success", function (file, responseText) {
                if (responseText == 0) {
                    myDropzoneBerkasRekanan.removeFile(file);
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Waktu Penawaran Sudah Habis',
                        buttons: [{ label: 'Close', action: function (dialog) { dialog.close(); } }]
                    });
                }
            });
        }
    });
    renderDokumenDropzone(myDropzoneBerkasRekanan, "BerkasRekanan");
    Dropzone.options.BerkasRekanan = false;

    var myDropzoneBerkasRekananKlarifikasi = new Dropzone("#BerkasRekananKlarifikasi", {
        url: $("#BerkasRekananKlarifikasi").attr("action") + "&id=" + $("#pengadaanId").val(),
        maxFilesize: 5,
        acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.docx,.xlsx",
        dictDefaultMessage: "Tidak Ada Dokumen",
        init: function () {
            this.on("addedfile", function (file) {
                file.previewElement.addEventListener("click", function () {
                    var fileId = null;
                    if (file.Id != undefined) {
                        fileId = file.Id;
                    } else {
                        try {
                            var parsed = JSON.parse(file.xhr.response);
                            fileId = (parsed && parsed.Id !== undefined) ? parsed.Id : null;
                        } catch (e) {
                            console.error("Invalid JSON response", e);
                            fileId = null;
                        }
                    }
                    $("#konfirmasiFile").attr("attr1", "BerkasRekananKlarifikasi");
                    $("#konfirmasiFile").attr("FileId", fileId);
                    $("#konfirmasiFile").modal("show");
                });
            });
            this.on("success", function (file, responseText) {
                if (responseText == 0) {
                    myDropzoneBerkasRekananKlarifikasi.removeFile(file);
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Waktu Penawaran Sudah Habis',
                        buttons: [{ label: 'Close', action: function (dialog) { dialog.close(); } }]
                    });
                }
            });
        }
    });
    renderDokumenDropzone(myDropzoneBerkasRekananKlarifikasi, "BerkasRekananKlarifikasi");
    Dropzone.options.BerkasRekananKlarifikasi = false;

    var myDropzoneBerkasRekananKlarifikasiLanjutan = new Dropzone("#BerkasRekananKlarifikasiLanjutan", {
        url: $("#BerkasRekananKlarifikasiLanjutan").attr("action") + "&id=" + $("#pengadaanId").val(),
        maxFilesize: 5,
        acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.docx,.xlsx",
        dictDefaultMessage: "Tidak Ada Dokumen",
        init: function () {
            this.on("addedfile", function (file) {
                file.previewElement.addEventListener("click", function () {
                    var fileId = null;
                    if (file.Id != undefined) {
                        fileId = file.Id;
                    } else {
                        try {
                            var parsed = JSON.parse(file.xhr.response);
                            fileId = (parsed && parsed.Id !== undefined) ? parsed.Id : null;
                        } catch (e) {
                            console.error("Invalid JSON response", e);
                            fileId = null;
                        }
                    }
                    $("#konfirmasiFile").attr("attr1", "BerkasRekananKlarifikasiLanjutan");
                    $("#konfirmasiFile").attr("FileId", fileId);
                    $("#konfirmasiFile").modal("show");
                });
            });
            this.on("success", function (file, responseText) {
                if (responseText == 0) {
                    myDropzoneBerkasRekananKlarifikasiLanjutan.removeFile(file);
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Waktu Penawaran Sudah Habis',
                        buttons: [{ label: 'Close', action: function (dialog) { dialog.close(); } }]
                    });
                }
            });
        }
    });
    renderDokumenDropzone(myDropzoneBerkasRekananKlarifikasiLanjutan, "BerkasRekananKlarifikasiLanjutan");
    Dropzone.options.BerkasRekananKlarifikasiLanjutan = false;

    // Tombol Buat Penawaran
    $("#penawaran").on("click", function () {
        var pid = $("#pengadaanId").val();
        if ($("#CekAsuransi").val() === "true") {
            safeRedirect("rekanan-rks-asuransi", "#", pid);
        } else {
            safeRedirect("rekanan-rks", "#", pid);
        }
    });

    // Tombol Buat Penawaran Klarifikasi
    $("#penawaran-klarifikasi").on("click", function () {
        var pid = $("#pengadaanId").val();
        if ($("#CekAsuransi").val() === "true") {
            safeRedirect("rekanan-klarifikasi-harga-asuransi", "#", pid);
        } else {
            safeRedirect("rekanan-klarifikasi-harga", "#", pid);
        }
    });

    // Tombol Buat Penawaran Klarifikasi Lanjutan
    $("#penawaran-klarifikasi-lanjutan").on("click", function () {
        var pid = $("#pengadaanId").val();
        if ($("#CekAsuransi").val() === "true") {
            safeRedirect("rekanan-klarifikasi-lanjutan-harga-asuransi", "#", pid);
        } else {
            safeRedirect("rekanan-klarifikasi-lanjutan-harga", "#", pid);
        }
    });

    // Download file dari modal konfirmasi
    $("#downloadFile").on("click", function () {
        var FileId = $("#konfirmasiFile").attr("FileId");
        downloadFileUsingForm("/api/pengadaane/OpenFile?Id=" + FileId);
        $("#konfirmasiFile").modal("hide");
    });

    // Hapus file dari modal konfirmasi
    $("#HapusFile").on("click", function () {
        var tipe = $("#konfirmasiFile").attr("attr1");
        var FileId = parseInt($("#konfirmasiFile").attr("FileId"));
        $.ajax({
            method: "POST",
            url: "Api/VendorAction/deleteDokumenPelaksanaan?Id=" + FileId
        }).done(function (data) {
            if (data.Id == "1") {
                var dropzoneMap = {
                    "BerkasRekanan": myDropzoneBerkasRekanan,
                    "BerkasRekananKlarifikasi": myDropzoneBerkasRekananKlarifikasi,
                    "BerkasRekananKlarifikasiLanjutan": myDropzoneBerkasRekananKlarifikasiLanjutan
                };
                var dz = dropzoneMap[tipe];
                if (dz) {
                    $.each(dz.files, function (index, item) {
                        var id = item.Id != undefined ? item.Id : null;
                        if (!id && item.xhr) {
                            try { id = JSON.parse(DOMPurify.sanitize(item.xhr.response)); } catch (e) { }
                        }
                        if (id == FileId) {
                            dz.removeFile(item);
                        }
                    });
                }
            }
            $("#konfirmasiFile").modal("hide");
        });
    });
});

function cekrksbiasapaasuransi() {
    $.ajax({
        method: "post",
        url: "api/pengadaane/cekRKSBiasaAtauAsuransi?PengadaanId=" + $("#pengadaanId").val()
    }).done(function (data) {
        if (data.RKSBiasa == true) {
            $("#CekAsuransi").val("false");
            $(".total-penawaran").show();
            $("#total_penawaran-klarifikasi").show();
            $("#total-penawaran-klarifikasi-lanjutan").show();
        } else if (data.RKSAsuransi == true) {
            $("#CekAsuransi").val("true");
            $(".total-penawaran").hide();
            $("#total_penawaran-klarifikasi").hide();
            $("#total-penawaran-klarifikasi-lanjutan").hide();
        }
    });
}

function loadData(pengadaanId) {
    $.ajax({
        method: "POST",
        url: "Api/VendorAction/detailPengadaanForRekanan?Id=" + pengadaanId,
        dataType: "json"
    }).done(function (data) {
        $("#judul").text(data.Judul);
        $("#deskripsi").text(data.AturanPengadaan + ", " + data.AturanBerkas + ", " + data.AturanPenawaran);
        $("#keterangan").text(data.Keterangan);
        $("#MataUang").text(data.MataUang);
        $("#UnitKerjaPemohon").text(data.UnitKerjaPemohon);
        $("#Region").text(data.Region);
        $("#Provinsi").text(data.Provinsi);
        $("#JenisPekerjaan").text(data.JenisPekerjaan);
        $("#pengadaanId").val(data.Id);

        hitungTawaranRekanan(data.Id, data.AturanPenawaran);
        loadJadwal(data.JadwalPengadaans);
        loadKualifikas(data.KualifikasiKandidats);

        // Aktifkan accordion sesuai status
        if (data.Status >= 4) {
            $("#tab-penawaran-rekanan").attr("data-toggle", "collapse");
            if (data.Status == 4) {
                $("#collapseOne").addClass("in");
            }
        }

        if (data.Status >= 7) {
            $("#tab-klarifikasi-rekanan").attr("data-toggle", "collapse");
            $("#tab-penawaran-rekanan").attr("data-toggle", "collapse");
            if (data.Status == 7) {
                $("#collapseTwo").addClass("in");
            }
        }

        if (data.Status >= 6) {
            $("#tab-klarifikasi-rekanan").attr("data-toggle", "collapse");
            $("#tab-penawaran-rekanan").attr("data-toggle", "collapse");
            $("#tab-klarifikasi-lanjutan-rekanan").attr("data-toggle", "collapse");
            if (data.Status == 6) {
                $("#panel-klarifikasi-lanjutan").addClass("in");
            }
        }

        // Sembunyikan panel klarifikasi lanjutan jika rekanan tidak masuk
        if (data.cekisMasukKlarifikasiLanjutan == 0) {
            $("#panel-klarifikasi-lanjutan").remove();
            $("#tab-klarifikasi-lanjutan-rekanan").closest(".panel").remove();
        }

        if (data.Status == 12) {
            $("#tab-klarifikasi-rekanan").attr("data-toggle", "collapse");
            $("#tab-penawaran-rekanan").attr("data-toggle", "collapse");
            $("#tab-klarifikasi-lanjutan-rekanan").attr("data-toggle", "collapse");
            $("#panel-klarifikasi-lanjutan").addClass("in");
        }

        if (data.Status == 8) {
            $("#tab-klarifikasi-rekanan").attr("data-toggle", "collapse");
            $("#tab-penawaran-rekanan").attr("data-toggle", "collapse");
            $("#tab-klarifikasi-lanjutan-rekanan").attr("data-toggle", "collapse");
        }

        // Disable input total penawaran (read-only)
        $("#total_penawaran").attr("disabled", "disabled");
        $("#row_penawaran_open_price").remove();
        $("#total_penawaran-klarifikasi").attr("disabled", "disabled");

        // Load jadwal aktual setelah pengadaanId terisi
        getDateSubmitPenawaran();
        getKlarifikasi();
    });
}

function loadStatus(pengadaanId) {
    $.ajax({
        method: "POST",
        url: "Api/VendorAction/statusVendor?Id=" + pengadaanId,
        dataType: "json"
    }).done(function (data) {
        $("#Status").html("Status: " + data);
    });
}

function renderDokumenDropzone(myDropzone, tipe) {
    var pid = $("#pengadaanId").val();
    if (!pid) return;
    $.ajax({
        url: "Api/VendorAction/getDokumens?Id=" + pid + "&tipe=" + tipe,
        success: function (data) {
            for (var key in data) {
                var file = {
                    Id: data[key].Id,
                    name: data[key].File,
                    accepted: true,
                    status: Dropzone.SUCCESS,
                    processing: true,
                    size: data[key].SizeFile
                };
                myDropzone.emit("addedfile", file);
                myDropzone.emit("complete", file);
                myDropzone.files.push(file);
            }
        },
        error: function () { }
    });
}

function hitungTawaranRekanan(pengadaanId, aturanPenawaran) {
    $.ajax({
        url: "Api/VendorAction/getRksRekanan?id=" + pengadaanId
    }).done(function (data) {
        var rksdetail = data.data;
        var total = 0;
        for (var key in rksdetail) {
            if (rksdetail[key].harga > 0 && rksdetail[key].jumlah > 0) {
                total += rksdetail[key].jumlah * rksdetail[key].harga;
            }
        }
        $("#total_penawaran").val(accounting.formatNumber(total, { thousand: ".", decimal: ",", precision: 2 }));
        if (total <= 0) { $("#amplop-merah-penawaran").show(); } else { $("#amplop-hijau-penawaran").show(); }
    });

    $.ajax({
        url: "Api/VendorAction/getRKSForKlarifikasiRekanan?id=" + pengadaanId
    }).done(function (data) {
        var rksdetail = data.data;
        var total = 0;
        for (var key in rksdetail) {
            if (rksdetail[key].harga > 0 && rksdetail[key].jumlah > 0) {
                total += rksdetail[key].jumlah * rksdetail[key].harga;
            }
        }
        $("#total_penawaran-klarifikasi").val(accounting.formatNumber(total, { thousand: ".", decimal: ",", precision: 2 }));
        if (total <= 0) { $("#amplop-merah-klarifikasi").show(); } else { $("#amplop-hijau-klarifikasi").show(); }
    });

    $.ajax({
        url: "Api/VendorAction/getRKSForKlarifikasiLanjutanRekanan?Id=" + pengadaanId
    }).done(function (data) {
        var rksdetail = data.data;
        var total = 0;
        for (var key in rksdetail) {
            if (rksdetail[key].harga > 0 && rksdetail[key].jumlah > 0) {
                total += rksdetail[key].jumlah * rksdetail[key].harga;
            }
        }
        $("#total-penawaran-klarifikasi-lanjutan").val(accounting.formatNumber(total, { thousand: ".", decimal: ",", precision: 2 }));
        if (total <= 0) { $("#amplop-merah-lanjutan").show(); } else { $("#amplop-hijau-lanjutan").show(); }
    });
}

function loadKualifikas(kualifikasiKandidat) {
    $(".checkbox-kualifikasi").removeAttr("checked");
    $.each(kualifikasiKandidat, function (index, value) {
        $(".checkbox-kualifikasi[value='" + value.kualifikasi + "']").prop("checked", "true");
        $(".checkbox-kualifikasi[value='" + value.kualifikasi + "']").attr("attrId", value.Id);
    });
}

function loadJadwal(data) {
    for (var i in data) {
        var tgl = "";
        if (data[i].Mulai != null && moment(data[i].Mulai).isValid()) {
            tgl += moment(data[i].Mulai).format("DD/MM/YYYY");
        }
        if (data[i].Sampai != null && moment(data[i].Sampai).isValid()) {
            tgl += " s/d " + moment(data[i].Sampai).format("DD/MM/YYYY");
        }
        if (data[i].tipe == "Aanwijzing" && data[i].Mulai != null) {
            $("#Aanwijzing").text(tgl);
        }
        if (data[i].tipe == "pengisian_harga") {
            $("#PengisianHarga").text(tgl);
        }
        if (data[i].tipe == "buka_amplop") {
            $("#BukaAmplop").text(tgl);
        }
        if (data[i].tipe == "penilaian") {
            $("#penilaian").text(tgl);
        }
        if (data[i].tipe == "klarifikasi") {
            $("#Klarifikasi").text(tgl);
        }
        if (data[i].tipe == "penentuan_pemenang") {
            $("#PenentuanPemenang").text(tgl);
        }
    }
}

function getDateSubmitPenawaran() {
    var pid = $("#pengadaanId").val();
    if (!pid) return;
    $.ajax({
        method: "POST",
        url: "Api/VendorAction/GetSubmitPenawran?PId=" + pid,
        success: function (data) {
            var mulai = moment(data.Mulai);
            var sampai = moment(data.Sampai);
            if (mulai.isValid() && sampai.isValid()) {
                $("#pengisian_harga_aktual").html(
                    "( " + mulai.format("DD MMMM YYYY HH:mm") + " s/d " + sampai.format("DD MMMM YYYY HH:mm") + " )"
                );
            }
        },
        error: function () { }
    });
}

function getKlarifikasi() {
    var pid = $("#pengadaanId").val();
    if (!pid) return;
    $.ajax({
        method: "POST",
        url: "Api/VendorAction/GetKlarifikasi?PId=" + pid,
        success: function (data) {
            var mulai = moment(data.Mulai);
            var sampai = moment(data.Sampai);
            if (mulai.isValid() && sampai.isValid()) {
                $("#klarifikasi_aktual").html(
                    "( " + mulai.format("DD MMMM YYYY HH:mm") + " s/d " + sampai.format("DD MMMM YYYY HH:mm") + " )"
                );
            }
        },
        error: function () { }
    });
}
