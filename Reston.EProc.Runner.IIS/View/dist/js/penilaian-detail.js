$(function () {
    
    function gup(name, url) {
        if (!url) url = location.href;
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(url);
        return results == null ? null : results[1];
    }

    $.ajax({
        method: "POST",
        url: "Api/NilaiVendor/detailSPKNilaiVendor?Id=" + gup("Id"),
        dataType: "json"
    }).done(function (data) {
        $("#judul").text(data.JudulPengadaan);
        $("#nomerspk").text(data.IdSPK);
        $("#keterangan").text(data.Deskripsi);
        var html = '';
            html = html + '<label>Total Point Penilaian : ' + data.total +'</label>';
        $("#total").append(html);
        var html2 = '';
        html2 = html2 + '<label>Rata Point Penilaian : ' + data.average + '</label>';
        $("#average").append(html2);

        var html3 = '';
        html3 = html3 + '<label>Pemberi Point Penilaian : ' + data.counter + ' dari ' + data.counterAll + '</label>';
        $("#counter").append(html3);
        //$("#total").text(data.total);
        //$("#average").text(data.average);
        //$("#counter").text(data.counter);
        //$("#counterAll").text(data.counterAll);
        $("#pengadaanId").val(data.pengadaanId);
        //cekcreatepertanyaan();
        //document.getElementById('vendor-pemenang-pengadaan').innerHTML = '<select class="form-control pemenang" id="pemenang"><option value="' + data.VendorId + '">' + data.PemenangPengadaan + '</option></select>';


        //<select class="form-control pemenang" id="pemenang"><option value="63">PT Pejuang Solusi</option></select>
    });
    
});


//var id_pengadaan = window.location.hash.replace("#", "");
//
//$(function () {
//    if (isGuid(id_pengadaan)) {
//        $("#pengadaanId").val(id_pengadaan);
//        loadData(id_pengadaan);
//    }
//    else {
//        if (isGuid($("#pengadaanId").val())) {
//            window.location.hash = $("#pengadaanId").val();
//            loadData($("#pengadaanId").val());
//            GetUsingCOA();
//        }
//        else {
//            window.location.replace("http://" + window.location.host + "/pengadaan-list.html");
//        }
//    }
//
//});


//download berkas 
//$(function () {
//    $(".download-berkas").on("click", function () {
//        if ($(this).attr("attr1") == "berkas-penilaian")
//            downloadFileUsingForm("/api/report/BerkasPenilaian?Id=" + $("#pengadaanId").val());
//        if ($(this).attr("attr1") == "assessment-penilaian")
//            downloadFileUsingForm("/api/report/AssessmentPenilaian?Id=" + $("#pengadaanId").val());
//    });
//});
//
//$(function () {
//   
//    $("body").on("click", ".box-rekanan", function () {
//        var id = $(this).attr("vendorId");
//        var pengadaanId = $("#pengadaanId").val();
//        BootstrapDialog.show({
//            title: 'Konfirmasi',
//            buttons: [{
//                label: 'Lihat Informasi Rekanan',
//                action: function (dialog) {
//                    window.open("http://" + window.location.host + "/rekanan-detail.html?id=" + id);
//                    dialog.close();
//                }
//            }, {
//                label: 'Close',
//                action: function (dialog) {
//                    dialog.close();
//                }
//            }]
//        });
//    }); 
//});
//
//function loadData(pengadaanId) {
//    $.ajax({
//        method: "POST",
//        url: "Api/PengadaanE/detailPengadaan?Id=" + pengadaanId,
//        dataType: "json"
//    }).done(function (data) {
//        $("#judul").text(data.Judul);
//        $("#deskripsi").text((data.NoPengadaan == null ? "" : (data.NoPengadaan + ", ")) + data.AturanPengadaan + ", " + data.AturanBerkas + ", " + data.AturanPenawaran);
//        if (data.AturanPengadaan == "Pengadaan Terbuka") $("#jadwal_pendaftaran").show();
//        
//        $("#keterangan").text(data.Keterangan);
//       
//        $("#State").val(data.Status);
//        $("#StatusName").val(data.StatusName);
//        $("#Branch").text(data.Branch);
//        $("#Department").text(data.Department);
//        if (data.isPIC == 0) {
//            $(".action-pelaksanaan").attr("disabled", "disabled");
//            $(".bingkai-pic-pelaksanaan").remove();
//            $("button.action-pelaksanaan").remove();
//            $(".action-pelaksanaan2").attr("disabled", "disabled");
//            $(".next-step").attr("disabled", "disabled");
//            $(".lewati-tahapan").attr("disabled", "disabled");
//            //$("#hapus-question").remove();
//        }
//        if (data.isPIC == 1) {
//            $("#hapus-question").show();
//        }
//
//        
//        
//
//        loadListKandidat(data.Id);
//        loadOpsiKandidat(data.Id);
//        
//       
//        if (data.StatusName == "DRAFT") {
//            $("#Status").text("Status Pengadaan : Draft");
//            
//        }
//
//        //lihat-penilaian-buka-amplop 
//        if (data.StatusName == "DITOLAK") {
//            $("#Status").text("Status Pengadaan : Pengajuan Ditolak");
//            loadKeteranganDitolak(data.Id);
//        }
//        if (data.StatusName == "AJUKAN") {
//            $("#Status").text("Status Pengadaan : Dalam Pengajuan");
//        }
//        if (data.Status > 1 && data.StatusName != "DITOLAK") {
//            $("#tab-rk").attr("data-toggle", "tab");
//            $("#tab-pelakasanaan").attr("data-toggle", "tab");
//            $("#tab-berkas").attr("data-toggle", "tab");
//            $("#tab-pelakasanaan").show();
//            $("#myTab li:eq(1)").addClass("active");
//            $("#myTab li:eq(0)").removeClass("active");
//            $("#tab-berkas").show();
//            if (data.isPIC == 1) {
//                $("#dibatalkan").show();
//            }
//            $("#rencana_kerja").removeClass("active");
//            $("#perlaksanaan").addClass("active");
//            $("#tab-rk").removeClass("active");
//            $("#tab-pelakasanaan").addClass("active");
//
//            if (data.StatusName == "DIBATALKAN") $("#dibatalkan").hide();
//        }
//        if (data.StatusName == "DISETUJUI") {
//            $("#Status").text("Status Pengadaan : Disetujui");
//        }
//        if (data.StatusName == "AANWIJZING") {
//            $("#Status").text("Status Pengadaan : Aanwijzing");
//        }
//        if (data.StatusName == "SUBMITPENAWARAN") {
//            $("#Status").text("Status Pengadaan : Submit Penawaran");
//        }
//        if (data.StatusName == "BUKAAMPLOP") {
//            $("#Status").text("Status Pengadaan : Buka Amplop");
//        }
//
//        if (data.StatusName == "KLARIFIKASI") {
//            $("#Status").text("Status Pengadaan : Klarifikasi");
//        }
//        if (data.StatusName == "KLARIFIKASILANJUTAN") {
//            $("#Status").text("Status Pengadaan : Klarifikasi Lanjutan");
//        }
//        if (data.StatusName == "PENILAIAN") {
//            $("#Status").text("Status Pengadaan : Penilaian Kandidat");
//        }
//        if (data.StatusName == "PEMENANG") {
//            $("#Status").text("Status Pengadaan : Penentuan Pemenang");
//        }
//        if (data.StatusName == "DIBATALKAN") {
//            $("#Status").text("Status Pengadaan : DiBatalkan");
//            loadKeteranganDiBatalkan(data.Id);
//        }
//        
//        userid();
//    });
//}
//
function userid() {
    $.ajax({
        method: "GET",
        url: "Api/PengadaanE/GetUserId"
    }).done(function (data) {
        renderuserid(data);
    });
}

function renderuserid(data) {
    var html = '';
    html = html + '<input type="hidden" value="' + data.Id + '" id="user-id">';
    $(".userid").append(html);
}
//
//function loadListKandidat(Id) {
//    $(".listkandidat").html("");
//    $.ajax({
//        method: "POST",
//        url: "Api/PengadaanE/GetKandidats?PId=" + Id
//    }).done(function (data) {
//        $(".listkandidat").html("");
//        $.each(data, function (index, item) {
//            var html = '<div class="col-md-3"><div class="box box-rekanan" vendorId="' + item.VendorId + '">' +
//                '<div class="box-tools pull-right">' +
//                '</div>' +
//                '<div class="box-body box-profile">' +
//                '<p class="profile-username title-header">' + item.Nama + '</p>' +
//                //'<p class="text-muted text-center deskripsi">' + item.kontak + '</p>' + 
//                '<p class="text-muted text-center deskripsi">' + item.Telepon + '</p>' +
//                '</div>' +
//                '</div></div>';
//            $(".listkandidat").append(html);
//        });
//    });
//}
//
//function loadOpsiKandidat(Id) {
//    $(".opsikandidat").html("");
//    $.ajax({
//        method: "POST",
//        url: "Api/PengadaanE/GetKandidats?PId=" + Id
//    }).done(function (data) {
//        $(".opsikandidat").html("");
//        $.each(data, function (index, item) {
//            var html = '<option value="' + item.NoPengajuan + '">' + item.Nama + '</option>';
//            $(".opsikandidat").append(html);
//        });
//    });
//}
//
//
//function loadKeteranganDitolak(Id) {
//    $.ajax({
//        method: "GET",
//        url: "Api/PengadaanE/getAlasanPenolakan?Id=" + Id,
//        success: function (data) {
//            $("#AlasanPenolakan").text("Alasan Penolakan: " + (data == "" ? "-" : data));
//            $("#AlasanPenolakan").show();
//        },
//        error: function (errormessage) {
//        }
//    });
//}
//
//function loadKeteranganDiBatalkan(Id) {
//    $.ajax({
//        method: "GET",
//        url: "Api/PengadaanE/getAlasanDiBatalkan?Id=" + Id,
//        success: function (data) {
//            $("#AlasanDiBatalkan").text("Alasan DiBatalkan: " + (data == "" ? "-" : data));
//            $("#AlasanDiBatalkan").show();
//        },
//        error: function (errormessage) {
//        }
//    });
//}
//
//
//function getHeaderPengadaan() {
//    var viewPengadaan = {};
//    viewPengadaan.Judul = $("[name=Judul]").val();
//    viewPengadaan.Keterangan = $("[name=Keterangan]").val();
//    viewPengadaan.AturanPengadaan = $("[name=AturanPengadaan]").val();
//    viewPengadaan.AturanBerkas = $("[name=AturanBerkas]").val();
//    viewPengadaan.AturanPenawaran = $("[name=AturanPenawaran]").val();
//    viewPengadaan.MataUang = $("[name=MataUang]").val();
//    viewPengadaan.PeriodeAnggaran = $("[name=PeriodeAnggaran]").val();
//    viewPengadaan.JenisPembelanjaan = $("[name=JenisPembelanjaan]").val();
//    viewPengadaan.PeriodeAnggaranString = $("[name=PeriodeAnggaranString]").val();
//    viewPengadaan.JenisPembelanjaanString = $("[name=JenisPembelanjaanString]").val();
//    viewPengadaan.HpsId = $("[name=HpsId]").val();
//    viewPengadaan.TitleDokumenNotaInternal = $("[name=TitleDokumenNotaInternal]").val();
//    viewPengadaan.TitleDokumenLain = $("[name=TitleDokumenLain]").val();
//    viewPengadaan.TitleBerkasRujukanLain = $("[name=TitleBerkasRujukanLain]").val();
//    viewPengadaan.UnitKerjaPemohon = $("[name=UnitKerjaPemohon]").val();
//    viewPengadaan.NoCOA = $("#noCoa").val();
//    viewPengadaan.Region = $("[name=Region]").val();
//    viewPengadaan.Provinsi = $("[name=Provinsi]").val();
//    viewPengadaan.KualifikasiRekan = $("[name=KualifikasiRekan]").val();
//    viewPengadaan.JenisPekerjaan = $("[name=JenisPekerjaan]").val();
//    viewPengadaan.PengadaanLangsung = $('#pengadaan-langsung').val();
//    if ($("#pengadaanId").val() != "") viewPengadaan.Id = $("#pengadaanId").val();
//    return viewPengadaan;
//}
