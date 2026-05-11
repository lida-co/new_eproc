var id_pengadaan = getIdFromUrl();
var table;
$(function () {
    if (isGuid(id_pengadaan)) {
        $("#pengadaanId").val(id_pengadaan);
        loadData(id_pengadaan);
    }
    else {
        if (isGuid($("#pengadaanId").val())) {
            window.location.hash = $("#pengadaanId").val();
            loadData($("#pengadaanId").val());
        }
        else {            
            window.location.replace( "/pengadaan-rekanan.html");
        }
    }

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
        
    $(".tab-content").show();
    table = $('#example1').DataTable({
        "paging": false,
        "lengthChange": false,
        "searching": false,
        "ordering": false,
        "info": false,
        "autoWidth": true,
        "ajax": "Api/VendorAction/getRksRekanan?id=" + $("#pengadaanId").val(),
        "columns": [
            { "data": "item" },
            { "data": "keteranganItem" },
            { "data": "satuan" },            
            { "data": "jumlah", "className": "tengah" },
            { "data": null },
            { "data": null, "className": "rata_kanan" },
            { "data": null }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    if (row.jumlah > 0) {
                        return accounting.formatNumber(row.jumlah, { thousand: ".", decimal: ",", precision: 2 });
                    }
                    else return '';
                },
                "targets": 3,
                "orderable": false
            },
            {
                "render": function (data, type, row) {
                    if (row.jumlah > 0) {
                        var harga = "0";
                        // Konversi harga dari format angka ke format tampilan
                        var num = parseHargaIndonesia(row.harga);
                        if (num > 0) {                                
                            harga = accounting.formatNumber(num, { thousand: ".", decimal: ",", precision: 2 });
                        }
                        return '<input type="text" class="form-control harga-rekanan" value="' + harga + '" attrId="' + row.Id + '" />';
                    }
                    else return '<input type="hidden" class="harga-rekanan" attrId="' + row.Id + '" />';
                },
                "targets": 4,
                "orderable": false
            },
            {
                "render": function (data, type, row) {
                    if (row.jumlah != null) {
                        var num = parseHargaIndonesia(row.harga);
                        if (row.jumlah > 0 && num > 0) {
                            return accounting.formatNumber(num * row.jumlah, { thousand: ".", decimal: ",", precision: 2 });
                        }
                        else return 0;
                    }
                    else return "";
                },
                "targets": 5,
                "orderable": false
            },
            {
                "render": function (data, type, row) {
                    if (row.jumlah != null) {
                        if (row.jumlah > 0) {
                            return '<textarea class="form-control undangan" attrId="' + row.Id + '">' + (row.keterangan || '') + '</textarea>';
                        } else return "";
                    }
                    else return "";
                },
                "targets": 6,
                "orderable": false
            }
        ],
        "rowCallback": function (row, data, index) {
            // Jangan panggil hitungHargaItem di sini karena menyebabkan loop invalidate
        },
        "initComplete": function (oSettings, json) {
            hitungHargaItem();
        }
    });

    // Update harga di DataTable saat user selesai mengetik (focusout)
    $('#example1').on("focusout", ".harga-rekanan", function () {
        var baris = $(this).parent().closest("tr").index();
        var rowData = table.row(baris).data();
        if (!rowData) return;
        // Simpan nilai harga sebagai angka murni (tanpa format)
        var nilaiHarga = parseHargaIndonesia($(this).val());
        rowData.harga = nilaiHarga;
        table.row(baris).data(rowData);
        // Update tampilan kolom Total tanpa re-render seluruh baris
        hitungHargaItem();
    });

    // Update keterangan di DataTable saat user selesai mengetik (focusout)
    $('#example1').on("focusout", ".undangan", function () {
        var baris = $(this).parent().closest("tr").index();
        var rowData = table.row(baris).data();
        if (!rowData) return;
        rowData.keterangan = $(this).val();
        table.row(baris).data(rowData);
    });

    $(".save-harga").on("click", function () {
        var data = datatableToJson(table);
        var pengadaanId = $("#pengadaanId").val();
        waitingDialog.showloading("Proses Harap Tunggu");
        $.ajax({
            method: "POST",
            url: "Api/VendorAction/addHargaRekanan?PengadaanId=" + pengadaanId,
            dataType: "json",
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            success: function (xdata) {
                waitingDialog.hideloading();
                if (xdata.Id == "1") {
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Harga Tawaran Berhasil di Kirim',
                        buttons: [
                            {
                                label: 'Kembali',
                                action: function (dialog) {
                                    dialog.close();
                                    safeRedirect("rekanan-side-detail-pengadaan", "#", pengadaanId);
                                }
                            },
                            {
                                label: 'Close',
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }
                        ]
                    });
                }
                else {
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Penawaran di tutup',
                        buttons: [
                            {
                                label: 'Kembali',
                                action: function (dialog) {
                                    dialog.close();
                                    safeRedirect("rekanan-side-detail-pengadaan", "#", pengadaanId);
                                }
                            },
                            {
                                label: 'Close',
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }
                        ]
                    });
                }
            },
            error: function (errormessage) {
                waitingDialog.hideloading();
                BootstrapDialog.show({
                    title: 'Error!!',
                    message: 'Terjadi kesalahan saat menyimpan data.',
                    buttons: [
                        {
                            label: 'Kembali',
                            action: function (dialog) {
                                dialog.close();
                                safeRedirect("rekanan-side-detail-pengadaan", "#", pengadaanId);
                            }
                        },
                        {
                            label: 'Close',
                            action: function (dialog) {
                                dialog.close();
                            }
                        }
                    ]
                });
            }
        });
    });
});

// Konversi format harga Indonesia (titik=ribuan, koma=desimal) ke float
function parseHargaIndonesia(value) {
    if (value == null || value === "" || value === 0) return 0;
    var str = value.toString().trim();
    // Hapus titik (separator ribuan), ganti koma (desimal) dengan titik
    str = str.replace(/\./g, "").replace(",", ".");
    var num = parseFloat(str);
    return isNaN(num) ? 0 : num;
}

function updateData(baris, newdata) {
    table.row(baris).data(newdata);
}

function hitungHargaItem() {
    var totalHarga = 0;
    table.rows().every(function () {
        var d = this.data();
        var numHarga = parseHargaIndonesia(d.harga);
        if (numHarga > 0) {
            var numJumlah = parseFloat((d.jumlah || 0).toString().replace(",", "."));
            totalHarga += numHarga * numJumlah;
        }
    });
    $("#total").text(accounting.formatNumber(totalHarga, { thousand: ".", decimal: ",", precision: 2 }));
}

function datatableToJson(table) {
    var data = [];
    table.rows().every(function () {
        var d = this.data();
        // Pastikan harga tersimpan sebagai angka murni (bukan format ribuan)
        d.harga = parseHargaIndonesia(d.harga);
        data.push(d);
    });
    return data;
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

        // Redirect ke halaman detail jika status belum sampai Submit Penawaran
        if (data.Status < 4) {
            safeRedirect("rekanan-side-detail-pengadaan", "#", pengadaanId);
            return;
        }

        // Disable form jika status sudah lewat fase Submit Penawaran (bukan status 4)
        if (data.Status != 4) {
            $(".save-harga").remove();
            // Tunggu DataTable selesai render baru disable
            setTimeout(function () {
                $("#example1").find("input").attr("disabled", "disabled");
                $("#example1").find("textarea").attr("disabled", "disabled");
            }, 500);
        }
    });
}

