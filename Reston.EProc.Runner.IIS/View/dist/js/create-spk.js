
//var PksId = DOMPurify.sanitize(gup("id", window.location.href));


// Extract "id" safely
var rawId = gup("id", window.location.href);

// Only allow alphanumeric, dash, underscore
var safeId = /^[a-zA-Z0-9_-]+$/.test(rawId) ? rawId : "";

// Encode for extra safety before DOM use
var SpkId = encodeURIComponent(safeId);


var myDropzoneSPK;
$(function () {
    //$("#pengadaanId").val(PengadaanId);
    //$("#VendorId").val(VendorId);
    
    // 🔒 PERBAIKAN: Hanya load detail jika SpkId valid dan bukan string kosong
    if (SpkId && SpkId !== "" && SpkId !== "null" && SpkId !== "undefined") {
        loadDetail(SpkId);
    } else if ($("#spkId").val() && $("#spkId").val() !== "" && $("#spkId").val() !== "null") {
        loadDetail($("#spkId").val());
    }
    // Jika tidak ada ID, ini adalah SPK baru - tidak perlu load detail
   

    $("#HapusFile").on("click", function () {
        var tipe = $("#konfirmasiFile").attr("attr1");
        var FileId = $("#konfirmasiFile").attr("FileId");
        $.ajax({
            method: "POST",
            url: "Api/Spk/deleteDokumenSpk?Id=" + FileId
        }).done(function (data) {
            if (data.Id == "1") {
                    $.each(myDropzoneSPK.files, function (index, item) {
                        var id = 0;
                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(file.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneSPK.removeFile(item);
                        }
                    });
                }            
            $("#konfirmasiFile").modal("hide");
        });
    });

     myDropzoneSPK = new Dropzone("#dokspk",
             {
                 headers: {
                     'X-CSRF-TOKEN': csrfToken,
                     'X-XSRF-TOKEN': csrfToken
                 },
                 maxFilesize: 10,
                 acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
                 // 🔒 PERBAIKAN: Kirim CSRF token via event sending (token sudah pasti ada)
                 sending: function(file, xhr, formData) {
                     var token = csrfToken;
                     if (!token) {
                         try {
                             var req = new XMLHttpRequest();
                             req.open('GET', '/api/security/GetCsrfToken', false);
                             req.send(null);
                             if (req.status === 200) {
                                 token = JSON.parse(req.responseText).csrfToken;
                                 csrfToken = token;
                             }
                         } catch(e) { console.warn('Gagal ambil CSRF token:', e); }
                     }
                     if (token) {
                         xhr.setRequestHeader("X-CSRF-TOKEN", token);
                         xhr.setRequestHeader("X-XSRF-TOKEN", token);
                     }
                 },
                 accept: function (file, done) {
                     this.options.url = $("#dokspk").attr("action") + "?id=" + $("#spkId").val();
                     
                     var spkId = $("#spkId").val();
                     var isOwner = $("#isOwner").val();
                     var statusSpk = $("#StatusSpk").val();
                     
                     // Blokir upload hanya jika SPK sudah Aktif/Batal (bukan Draft)
                     // Semua role yang berwenang bisa upload selama status masih Draft
                     // Validasi role sudah ada di server (ApiAuthorize)
                     if (statusSpk != 0 && statusSpk != "") {
                         BootstrapDialog.show({
                             title: 'Konfirmasi',
                             message: 'Upload hanya bisa dilakukan saat status SPK masih Draft',
                             buttons: [{
                                 label: 'Close',
                                 action: function (dialog) {
                                     myDropzoneSPK.removeFile(file);
                                     dialog.close();
                                 }
                             }]
                         });
                     } else {
                         var jumFile = myDropzoneSPK.files.length;
                         if ($("#pksId").val() == "") {
                             BootstrapDialog.show({
                                 title: 'Konfirmasi',
                                 message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                                 buttons: [{
                                     label: 'Close',
                                     action: function (dialog) {
                                         myDropzoneSPK.removeFile(file);
                                         dialog.close();
                                     }
                                 }]
                             });
                         } else {
                             if (jumFile > 1) {
                                 BootstrapDialog.show({
                                     title: 'Konfirmasi',
                                     message: 'Berkas Sudah Ada ',
                                     buttons: [{
                                         label: 'Close',
                                         action: function (dialog) {
                                             myDropzoneSPK.removeFile(file);
                                             dialog.close();
                                         }
                                     }]
                                 });
                             } else {
                                 done();
                             }
                         }
                     }
                 },
                 init: function () {
                     this.on("addedfile", function (file) {
                         file.previewElement.addEventListener("click", function () {
                             var id = 0;
                             if (file.Id != undefined)
                                 id = file.Id;
                             else
                                 id = $.parseJSON(DOMPurify.sanitize(file.xhr.response));
                             $("#HapusFile").show();
                             $("#konfirmasiFile").attr("attr1", "PKS");
                             $("#konfirmasiFile").attr("FileId", id);
                             $("#konfirmasiFile").modal("show"); 
                             if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                                 $("#HapusFile").show();
                             } else { $("#HapusFile").hide() }
                         });
                     });

                     this.on("error", function (file) {
                         myDropzoneSPK.removeFile(file);
                     });
                     this.on("success", function (file, responseText) {
                         if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                             myDropzoneSPK.removeFile(file);
                         }
                     });
                 }
             }
         );

    renderDokumenDropzone(myDropzoneSPK, "dokspk");
    Dropzone.options.dokspk = false;
});

$(function () {
   // $("#bingkai-upload-legal").hide();
    table = $('#tbl-PKS').DataTable({
        "serverSide": true,
        "searching": false,
        "ajax": {
            "url": 'api/Spk/ListPks',
            "type": 'POST',
            "data": function (d) {
                d.search = $("#title").val();
                d.klasifikasi = $("#klasifikasi option:selected").val();
            }
        },
        "columns": [
            { "data": "Judul" },
            { "data": "NoPks"},
            { "data": "NoPengadaan" },
            { "data": "JenisPekerjaan" },
            { "data": "Vendor" },
            { "data": null }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    var objData = JSON.stringify(data);
                    return " <a obj='" + objData + "' class='btn btn-success btn-sm pilih-pks '> Pilih </a> ";
                },

                "targets": 5,
                "orderable": false
            }
        ],
        "paging": true,
        "lengthChange": false,
        "ordering": false,
        "info": true,
        "autoWidth": false,
        "responsive": true,
    });
});

$(function () {
    $(".cari-pks").on("click", function () {
        $("#pks_modal").modal("show");
    });

    $("#tbl-PKS").on("click", ".pilih-pks", function () {
        console.log($(this).attr("obj"));
        var obj = jQuery.parseJSON($(this).attr("obj"));       
        waitingDialog.showloading("Proses Harap Tunggu");
        $("#judul").val(obj.Judul);
        $("#no-pengadaan").val(obj.NoPengadaan);
        $("#no-spk").val(obj.NoSpk);
        $("#no-pks").val(obj.NoPks);
        $("#pelaksana").val(obj.Vendor);
        $("#PemenangPengadaanId").val(obj.PemenangPengadaanId);
        $("#pks_modal").modal("hide");
        $("#pksId").val(obj.Id);
        //$(".ajukan").hide();
       // $(".Hapus").hide();
        waitingDialog.hideloading();
    });

    $(".Simpan").on("click", function () {
        var spk = {};
        spk.PksId = $("#pksId").val();
        spk.Id = $("#spkId").val() || "00000000-0000-0000-0000-000000000000"; // 🔒 PERBAIKAN: Set default GUID untuk SPK baru
        spk.TanggalSPKStr = moment($("#tanggal-spk").val(), ["D MMMM YYYY HH:mm"], "id").format("DD/MM/YYYY HH:mm");
        spk.NilaiSPK = $("#nilai-spk").val();
        
        // 🔒 DEBUG: Log data yang akan dikirim
        console.log("Data SPK yang akan disimpan:", spk);
        console.log("PksId:", spk.PksId);
        console.log("SpkId:", spk.Id);
        console.log("isOwner:", $("#isOwner").val());
        
        // Validasi: PksId harus ada untuk SPK baru
        if (!spk.PksId || spk.PksId === "") {
            BootstrapDialog.show({
                title: 'Peringatan',
                message: 'Silakan pilih PKS terlebih dahulu dengan klik tombol "Cari PKS"',
                buttons: [{
                    label: 'OK',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return;
        }
        
        if ($("#isOwner").val() == 1 || $("#spkId").val() == "")
            save(spk);
    });

    if ($("#spkId").val() == "") {
        $(".Simpan").show();
    }
    $(".Hapus").on("click", function () {
        waitingDialog.showloading("Proses Harap Tunggu");
        $.ajax({
            url: "Api/Spk/delete?Id=" + $("#spkId").val(),
            method: "POST",
        }).done(function (data) {
            window.location.replace("http://" + window.location.host + "/spk.html");
            waitingDialog.hideloading();

        });

    });
    
    $("[name=status]").on("change", function () {
        change();
    });

    $(".date").datetimepicker({
        format: "DD MMMM YYYY",
        locale: 'id',
        useCurrent: false,
        minDate: Date.now()

    });

    $("#downloadFile").on("click", function () {
        var FileId = $("#konfirmasiFile").attr("FileId");
        downloadFileUsingForm("/api/Spk/OpenFile?Id=" + FileId);
    });
});

function loadDetail(Id) {
    $.ajax({
        url: "Api/spk/detail?Id=" + Id,
        method: "POST", // Pastikan method POST
        contentType: "application/json" // 🔒 PERBAIKAN: Kirim sebagai JSON
    }).done(function (data) {
        $("#judul").val(data.Judul);
        $("#no-pengadaan").val(data.NoPengadaan);
        $("#no-spk").val(data.NoSpk);
        $("#pelaksana").val(data.Vendor);
        $("#note").val(data.Note);
        $("#title-pks").val(data.Title);
        $("#pksId").val(data.PksId);
        $("#spkId").val(data.Id);
        $("#isOwner").val(data.isOwner);
        $("#Approver").val(data.Approver);
        $("#no-pks").val(data.NoPks);
        $("[name=status][value=" + data.StatusSpk + "]").prop('checked', true);
        $("#PemenangPengadaanId").val(data.PemenangPengadaanId);
        $("#StatusSpk").val(data.StatusSpk);//$("[name=status]:checked").val()
        $("#tanggal-spk").val(moment(data.TanggalSPK).format("DD MMMM YYYY HH:mm"));
        $("#nilai-spk").val(data.NilaiSPK);
        $("#Status").text(data.StatusSpkName);
        if ($("#isOwner").val() == 0 || data.StatusSpk == 1 || data.StatusSpk == 2) {
            $(".Simpan").remove();
            $(".Hapus").remove();
            $(" .input-spk").attr("disabled", true);
            if ($("#isOwner").val() == 1) {
                $("[name=status][value=1]").attr("disabled", false);
                $("[name=status][value=2]").attr("disabled", false);
            }
        }
        if (data.isOwner == 1 && (data.StatusSpk == 0)) {
            if (data.StatusSpk == 0) {
                $(".Simpan").show();
                $(".ajukan").show();
                $(".Hapus").show();
                $(".input-spk").attr("disabled", false);
                $(".Edit").hide();
            }
        }
        



        $("#Status").text(data.StatusPksName);
    }).fail(function(xhr, status, error) {
        // 🔒 PERBAIKAN: Tambah error handling
        console.error("Error loading detail:", error);
        BootstrapDialog.show({
            title: 'Error',
            message: "Gagal memuat detail SPK: " + (xhr.responseText || error),
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    });

}

function renderDokumenDropzone(myDropzone) {
    var rSpkId = SpkId;
    
    if ($("#spkId").val() !== '') rSpkId = $("#spkId").val();
    
    // 🔒 PERBAIKAN: Jangan load dokumen jika tidak ada ID (SPK baru)
    if (!rSpkId || rSpkId === "" || rSpkId === "null" || rSpkId === "undefined") {
        console.log("Tidak ada SPK ID, skip load dokumen");
        return;
    }
    
    $.ajax({
        url: "Api/Spk/getDokumens?Id=" + rSpkId,
        method: "POST", // Pastikan method POST
        contentType: "application/json", // 🔒 PERBAIKAN: Kirim sebagai JSON
        success: function (data) {
            for (var key in data) {
                var file = {
                    Id: data[key].Id, name: data[key].File, accepted: true,
                    status: Dropzone.SUCCESS, processing: true, size: data[key].SizeFile
                };
                //thisDropzone.options.addedfile.call(thisDropzone, file);
                myDropzone.emit("addedfile", file);
                myDropzone.emit("complete", file);
                myDropzone.files.push(file);
            }
        },
        error: function (xhr, status, errormessage) {
            // 🔒 PERBAIKAN: Tambah error handling yang lebih baik
            console.error("Error loading documents:", errormessage);
            // Jangan reload, cukup log error
            //location.reload();

        }
    });
}

function save(spk) {
    
    waitingDialog.showloading("Proses Harap Tunggu");
    
    // Ambil token terbaru - jangan bergantung hanya pada $.ajaxSetup
    var token = csrfToken;
    if (!token) {
        try {
            var req = new XMLHttpRequest();
            req.open('GET', '/api/security/GetCsrfToken', false);
            req.send(null);
            if (req.status === 200) {
                token = JSON.parse(req.responseText).csrfToken;
                csrfToken = token;
            }
        } catch(e) { console.warn('Gagal ambil CSRF token:', e); }
    }
    
    console.log("Save - CSRF Token:", token);
    console.log("Save - Data:", JSON.stringify(spk));
    
    $.ajax({
        url: "Api/spk/Save",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(spk),
        headers: {
            "X-CSRF-TOKEN": token || "",
            "X-XSRF-TOKEN": token || "",
            "RequestVerificationToken": token || ""
        }
    }).done(function (data) {
        console.log("Response dari server:", data);
        loadDetail(data.Id);
        var msg = data.message;
        waitingDialog.hideloading();
        BootstrapDialog.show({
            title: 'Infomasi',
            message: msg,
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        
    }).fail(function(xhr, status, error) {
        console.error("Error response:", xhr.responseText);
        waitingDialog.hideloading();
        var errorMsg = "Terjadi kesalahan: " + (xhr.responseText || error);
        BootstrapDialog.show({
            title: 'Error',
            message: errorMsg,
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    });
}

$("#template-spk").on("click", function () {
    //var tanggal = moment($("#tanggal-spk").val(), ["D MMMM YYYY HH:mm"], "id").format("DD/MM/YYYY HH:mm");
    downloadFileUsingForm("Api/Report/BerkasSPK2?Judul=" + $("#judul").val() + "&Tanggal_SPK=" + $("#tanggal-spk").val() + "&Vendor= " + $("#pelaksana").val() + "&Nilai_SPK=" + $("#nilai-spk").val());
    /*alert("bgafugsafgsg")*/
});

function change() {
    var status = $("[name=status]:checked").val();
    
    if (myDropzoneSPK.files.length == 0 && (status == 1 || status==2)) {
        BootstrapDialog.show({
            title: 'Infomasi',
            message: "Harap Upload Dokumen SPK dahulu",
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        $("[name=status][value=0]").prop('checked', true);
        return;
    }
    waitingDialog.showloading("Proses Harap Tunggu");
    $.ajax({
        url: "Api/Spk/ChangeSatus?Id=" + $("#spkId").val() + "&status=" + status,
        method: "POST",
        contentType: "application/json" // 🔒 PERBAIKAN: Kirim sebagai JSON
    }).done(function (data) {
        loadDetail($("#spkId").val());
        if (data.Id != "") {
            if (status == 1) {
                $(".Hapus").remove();
                $(".Simpan").remove();
              
            }
        }
        var msg = data.message;
        waitingDialog.hideloading();
        BootstrapDialog.show({
            title: 'Infomasi',
            message: msg,
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });

    }).fail(function(xhr, status, error) {
        // 🔒 PERBAIKAN: Tambah error handling
        waitingDialog.hideloading();
        var errorMsg = "Terjadi kesalahan: " + (xhr.responseText || error);
        BootstrapDialog.show({
            title: 'Error',
            message: errorMsg,
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
    });
}
