var rawId = gup("id", window.location.href);
var safeId = /^[a-zA-Z0-9_-]+$/.test(rawId) ? rawId : "";
var PksId = encodeURIComponent(safeId);

var csrfToken = "";

let csrfRetryCount = 0;
const MAX_RETRIES = 3;

async function initCsrf() {
    try {
        const res = await fetch('/api/security/GetCsrfToken');

        if (!res.ok) {
            throw new Error(`HTTP ${res.status}: ${res.statusText}`);
        }

        const data = await res.json();

        if (!data.csrfToken) {
            throw new Error('CSRF token tidak ditemukan dalam response');
        }

        csrfToken = data.csrfToken;
        csrfRetryCount = 0; // Reset retry count
        //console.log('CSRF token berhasil diambil');

    } catch (e) {
        console.error("Gagal mengambil CSRF token:", e.message);
        csrfRetryCount++;

        if (csrfRetryCount <= MAX_RETRIES) {
            setTimeout(initCsrf, 5000);
        } else {
            console.error('Gagal mengambil CSRF token setelah 3 kali percobaan');
        }
    }
}

$.ajaxSetup({
    beforeSend: async function (xhr, settings) {
        if (settings.type === 'GET') return;

        // tunggu token kalau belum ada
        let waitCount = 0;
        while (!csrfToken && waitCount < 10) {
            await new Promise(r => setTimeout(r, 200));
            waitCount++;
        }

        if (!csrfToken) {
            console.warn('CSRF token tetap belum tersedia');
            return;
        }

        xhr.setRequestHeader("X-CSRF-TOKEN", csrfToken);
    }
});


$(function () {
    //console.log("-----------ID-------" + PksId);
    //$("#pengadaanId").val(PengadaanId);
    //$("#VendorId").val(VendorId);
    if (PksId != "" && PksId != null) loadDetail(PksId);
    cekisstaff();
    
    var myDropzonePKS = new Dropzone("#DraftPKS",
             {
                 maxFilesize: 10,
                 acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
                 accept: function (file, done) {
                     this.options.url = $("#DraftPKS").attr("action") + "&id=" + $("#pksId").val();
                     if ($("#isOwner").val() != 1 && $("#StatusPks").val() >1 )
                         BootstrapDialog.show({
                             title: 'Konfirmasi',
                             message: 'Anda Tidak Punya Akses ' ,
                             buttons: [{
                                 label: 'Close',
                                 action: function (dialog) {
                                     myDropzonePKS.removeFile(file);
                                     dialog.close();
                                 }
                             }]
                         });
                     else {
                         var jumFile = myDropzonePKS.files.length;
                         if ($("#pksId").val() == "") {
                             BootstrapDialog.show({
                                 title: 'Konfirmasi',
                                 message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                                 buttons: [{
                                     label: 'Close',
                                     action: function (dialog) {
                                         myDropzonePKS.removeFile(file);
                                         dialog.close();

                                     }
                                 }]
                             });

                         }
                         if ($("#StatusPks").val() != 3) {
                             if (jumFile > 1) {
                                 BootstrapDialog.show({
                                     title: 'Konfirmasi',
                                     message: 'Berkas Sudah Ada ',
                                     buttons: [{
                                         label: 'Close',
                                         action: function (dialog) {
                                             myDropzonePKS.removeFile(file);
                                             dialog.close();
                                         }
                                     }]
                                 });
                             } else {
                                 done();
                             }
                         }
                         else {
                             done();
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
                             $("#konfirmasiFile").attr("attr1", "DraftPKS");
                             $("#konfirmasiFile").attr("FileId", id);
                             $("#konfirmasiFile").modal("show");
                             if ($("#isOwner").val() == 1 && $("#StatusPks").val() <3) {
                                 $("#HapusFile").show();
                             }
                             else { $("#HapusFile").hide() }
                         });
                     });
                    
                     this.on("error", function (file) {
                         myDropzonePKS.removeFile(file);
                     });
                     this.on("success", function (file, responseText) {
                         if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                             myDropzonePKS.removeFile(file);
                         }
                     });
                 }
             }
         );

    renderDokumenDropzone(myDropzonePKS, "DraftPKS");
    Dropzone.options.DraftPKS = false;

    var myDropzoneFinalPKS = new Dropzone("#FinalLegalPks",
             {
                 maxFilesize: 10,
                 acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
                 accept: function (file, done) {
                     this.options.url = $("#FinalLegalPks").attr("action") + "&id=" + $("#pksId").val();
                     if ($("#isOwner").val() == 1) $("#Approver").val(1);
                     if ($("#Approver").val() != 1 )
                         BootstrapDialog.show({
                             title: 'Konfirmasi',
                             message: 'Anda Tidak Punya Akses ',
                             buttons: [{
                                 label: 'Close',
                                 action: function (dialog) {
                                     myDropzonePKS.removeFile(file);
                                     dialog.close();
                                 }
                             }]
                         });
                     else {
                         var jumFile = myDropzoneFinalPKS.files.length;
                         if ($("#pksId").val() == "") {
                             BootstrapDialog.show({
                                 title: 'Konfirmasi',
                                 message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                                 buttons: [{
                                     label: 'Close',
                                     action: function (dialog) {
                                         myDropzoneFinalPKS.removeFile(file);
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
                                             myDropzoneFinalPKS.removeFile(file);
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
                             
                             $("#konfirmasiFile").attr("attr1", "FinalLegalPks");
                             $("#konfirmasiFile").attr("FileId", id);
                             $("#konfirmasiFile").modal("show");
                             if ($("#Approver").val() == 1 && $("#StatusPks").val() == 2) {
                                 $("#HapusFile").show();
                             } else { $("#HapusFile").hide() }
                         });
                     });

                     this.on("error", function (file) {
                         myDropzoneFinalPKS.removeFile(file);
                     });
                     this.on("success", function (file, responseText) {
                         if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                             myDropzoneFinalPKS.removeFile(file);
                         }
                     });
                 }
             }
         );

    renderDokumenDropzone(myDropzoneFinalPKS, "FinalLegalPks");
    Dropzone.options.FinalLegalPks = false;

    var myDropzoneAssignedPks = new Dropzone("#AssignedPks",
             {
                 url: $("#AssignedPks").attr("action") + "&id=" + $("#pksId").val(),
                 maxFilesize: 10,
                 acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
                 accept: function (file, done) {
                     this.options.url = $("#AssignedPks").attr("action") + "&id=" + $("#pksId").val();
                     if ($("#isOwner").val() != 1)
                         BootstrapDialog.show({
                             title: 'Konfirmasi',
                             message: 'Anda Tidak Punya Akses ',
                             buttons: [{
                                 label: 'Close',
                                 action: function (dialog) {
                                     myDropzonePKS.removeFile(file);
                                     dialog.close();
                                 }
                             }]
                         });
                     else {
                         var jumFile = myDropzoneAssignedPks.files.length;
                         if ($("#pksId").val() == "") {
                             BootstrapDialog.show({
                                 title: 'Konfirmasi',
                                 message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                                 buttons: [{
                                     label: 'Close',
                                     action: function (dialog) {
                                         myDropzoneAssignedPks.removeFile(file);
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
                                             myDropzoneAssignedPks.removeFile(file);
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
                            
                             $("#konfirmasiFile").attr("attr1", "AssignedPks");
                             $("#konfirmasiFile").attr("FileId", id);
                             $("#konfirmasiFile").modal("show");
                             if ($("#isOwner").val() == 1 && $("#StatusPks").val() == 2) {
                                 $("#HapusFile").show();
                             } else { $("#HapusFile").hide() }
                         });
                     });

                     this.on("error", function (file) {
                         myDropzoneAssignedPks.removeFile(file);
                     });
                     this.on("success", function (file, responseText) {
                         if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                             myDropzoneAssignedPks.removeFile(file);
                         }
                     });
                 }
             }
         );

    renderDokumenDropzone(myDropzoneAssignedPks, "AssignedPks");
    Dropzone.options.AssignedPks = false;

    


});

$(function () {
    // $("#bingkai-upload-legal").hide();

    if (!csrfToken) {
        alert("CSRF token belum siap, refresh halaman");
        return;
    }

    var headers = {};
    if (csrfToken) {
        headers['X-CSRF-Token'] = csrfToken;
        headers['X-XSRF-TOKEN'] = csrfToken;
    }

    table = $('#tbl-pengadaan').DataTable({
        "serverSide": true,
        "searching": false,
        "ajax": {
            "url": 'api/Pks/ListPengadaan',
            "type": 'POST',
            "headers": headers,
            "data": function (d) {
                d.search = $("#title").val();
                d.klasifikasi = $("#klasifikasi option:selected").val();
            },
            "error": function (xhr) {
                console.error("Gagal load data:", xhr.responseText);
            }
        },
        "columns": [
            { "data": "Judul" },
            { "data": "HPS", "className": "rata_kanan" },
            { "data": "JenisPekerjaan" },
            { "data": "Vendor" },
            { "data": null }
        ],
        "columnDefs": [
            {
                "render": function (data, type, row) {
                    if (data != null)
                        return accounting.formatNumber(data, { thousand: ".", decimal: ",", precision: 2 });
                    else
                        return "";
                },
                "targets": 1,
                "orderable": false
            },
            {
                "render": function (data, type, row) {
                    if (!data) return "";
                    var objData = encodeURIComponent(JSON.stringify(data));

                    return "<a obj='" + objData + "' class='btn btn-success btn-sm pilih-pengadaan'>Pilih</a>";
                },
                "targets": 4,
                "orderable": false
            }
        ],
        "paging": true,
        "lengthChange": false,
        "ordering": false,
        "info": true,
        "autoWidth": false,
        "responsive": true
    });
});

$(function () {
    $(".cari-pengadaan").on("click", function () {
        $("#pengadaan_modal").modal("show");
    });

    $("#tbl-pengadaan").on("click", ".pilih-pengadaan", function () {

        var raw = $(this).attr("obj");

        if (!raw) {
            alert("Data tidak ditemukan");
            return;
        }

        var obj;

        try {
            obj = JSON.parse(decodeURIComponent(raw)); // ✅ FIX DI SINI
        } catch (e) {
            console.error("Gagal parse JSON:", e);
            alert("Format data tidak valid");
            return;
        }

        waitingDialog.showloading("Proses Harap Tunggu");

        $("#judul").val(obj.Judul);
        $("#no-pengadaan").val(obj.NoPengadaan);
        $("#no-spk").val(obj.NoSpk);
        $("#pelaksana").val(obj.Vendor);
        $("#PemenangPengadaanId").val(obj.PemenangPengadaanId);

        $("#pengadaan_modal").modal("hide");
        $("#pksId").val("");
        $(".ajukan").hide();
        $(".Hapus").hide();

        waitingDialog.hideloading();
    });

    $(".Simpan").on("click", function () {
        var pks = {};
        pks.Note = $("#note").val();
        pks.TanggalMulaiStr = moment($("#tanggal-mulai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        pks.TanggalSelesaiStr = moment($("#tanggal-selesai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        pks.Title = $("#title-pks").val();
        pks.PemenangPengadaanId = $("#PemenangPengadaanId").val();
        pks.Id = $("#pksId").val();
        if ($("#isOwner").val() == 1 || $("#pksId").val()=="")
            save(pks);

    });

    $(".Simpan-Setuju").on("click", function () {
        var pks = {};
        pks.Note = $("#note").val();
        pks.TanggalMulaiStr = moment($("#tanggal-mulai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        pks.TanggalSelesaiStr = moment($("#tanggal-selesai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        pks.Title = $("#title-pks").val();
        pks.PemenangPengadaanId = $("#PemenangPengadaanId").val();
        pks.Id = $("#pksId").val();
        if ($("#isOwner").val() == 1 || $("#pksId").val() == "")
            savesetuju(pks);
    });

    $(".ajukan").on("click", function () {
        var pks = {};
        pks.Note = $("#note").val();
        pks.Title = $("#title-pks").val();
        pks.PemenangPengadaanId = $("#PemenangPengadaanId").val();
        pks.Id = $("#pksId").val();
        if ($("#StatusPks").val() == "0")
            ajukan();
    });

    $(".Edit").on("click", function () {
        waitingDialog.showloading("Proses Harap Tunggu");
        $.ajax({
            url: "Api/Pks/Edit?Id=" + $("#pksId").val(),
            method: "POST",
        }).done(function (data) {
            loadDetail($("#pksId").val());
            waitingDialog.hideloading();
        });

    });
    if ($("#pksId").val() == "") {
        $(".Simpan-Setuju").show();
        $(".Simpan").show();
    }
    $(".Hapus").on("click", function () {

        if (!csrfToken) {
            alert("CSRF token belum siap, refresh halaman");
            return;
        }

        waitingDialog.showloading("Proses Harap Tunggu");
        $.ajax({
            url: "Api/Pks/delete?Id=" + $("#pksId").val(),
            method: "POST",
        }).done(function (data) {
            loadDetail($("#pksId").val());
            window.location.replace("http://" + window.location.host + "/pks.html");
            waitingDialog.hideloading();

        });

    });
    $(".done").on("click", function () {
        $("#KonfirmasiDone").modal("show");
    });
});

$(function () {
    $("#HapusFile").on("click", function () {
        if (!csrfToken) {
            alert("CSRF token belum siap, refresh halaman");
            return;
        }

        var headers = {};
        if (csrfToken) {
            headers['X-CSRF-Token'] = csrfToken;
            headers['X-XSRF-TOKEN'] = csrfToken;
        }

        var parent = $(this).closest("[attr1]");

        var tipe = parent.attr("attr1");
        var FileId = parent.attr("FileId");

        if (!FileId) {
            alert("FileId tidak ditemukan");
            return;
        }

        $.ajax({
            method: "POST",
            url: "Api/Pks/deleteDokumenPks?Id=" + encodeURIComponent(FileId),
            headers: headers,
            error: function (xhr) {
                alert("Gagal menghapus file");
                console.error(xhr.responseText);
            }
        }).done(function (data) {

            if (data && data.Id == "1") {

                if (tipe == "DraftPKS") {
                    $.each(myDropzoneDraftPKS.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        } else if (item.xhr && item.xhr.response) {
                            try {
                                id = JSON.parse(DOMPurify.sanitize(item.xhr.response));
                            } catch (e) {
                                console.warn("Gagal parse DraftPKS", e);
                            }
                        }

                        if (id == FileId) {
                            myDropzoneDraftPKS.removeFile(item);
                        }
                    });
                }

                if (tipe == "FinalLegalPks") {
                    $.each(myDropzoneFinalPKS.files, function (index, item) {
                        var id;

                        if (item.Id != undefined) {
                            id = item.Id;
                        } else if (item.xhr && item.xhr.response) {
                            try {
                                id = JSON.parse(DOMPurify.sanitize(item.xhr.response));
                            } catch (e) {
                                console.warn("Gagal parse FinalPKS", e);
                            }
                        }

                        if (id == FileId) {
                            myDropzoneFinalPKS.removeFile(item);
                        }
                    });
                }

                if (tipe == "AssignedPks") {
                    $.each(myDropzoneAssignedPks.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        } else if (item.xhr && item.xhr.response) {
                            try {
                                id = JSON.parse(DOMPurify.sanitize(item.xhr.response));
                            } catch (e) {
                                console.warn("Gagal parse AssignedPks", e);
                            }
                        }

                        if (id == FileId) {
                            myDropzoneAssignedPks.removeFile(item);
                        }
                    });
                }
            }

            $("#konfirmasiFile").modal("hide");

            // tetap reload seperti behavior awal
            window.location.reload();
        });
    });

    $("#downloadFile").on("click", function () {
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
        downloadFileUsingForm("/api/Pks/OpenFile?Id=" + FileId);
    });
});

function loadDetail(Id) {
    $.ajax({
        url: "Api/pks/detail?Id=" + Id
    }).done(function (data) {
        $("#judul").val(data.Judul);
        $("#no-pengadaan").val(data.NoPengadaan);
        $("#noPks").val(data.NoPks);
        $("#pelaksana").val(data.Vendor);
        $("#note").val(data.Note);
        $("#title-pks").val(data.Title);
        $("#tanggal-mulai").val(moment(data.TanggalMulai).format("DD MMMM YYYY"));
        $("#tanggal-selesai").val(moment(data.TanggalSelesai).format("DD MMMM YYYY"));
        $("#pksId").val(Id);
        $("#isOwner").val(data.isOwner);
        $("#Approver").val(data.Approver);
        
        $("#PemenangPengadaanId").val(data.PemenangPengadaanId);
        $("#StatusPks").val(data.StatusPks);
        if ($("#isOwner").val() == 0) {
            $(".Simpan-Setuju").remove();
            $(".Simpan").remove();
            $(".Stop").remove();
            $(".ajukan").remove();
            $(".Hapus").remove();
            $(".input-pks").attr("disabled", true);
            $(".done").remove();

        }
        if (data.isOwner == 1 && (data.StatusPks == 0 || data.StatusPks == 3)) {
            $("#bingkai-upload-legal").hide();
            if (data.StatusPks == 0) {
                //$(".Simpan-Setuju").show();
                $(".Simpan").show();
                $(".Stop").show();
                $(".ajukan").hide();
                $(".Hapus").show();
                $(".input-pks").attr("disabled", false);
                $(".Edit").hide();
                $(".done").show();
            }
            if (data.StatusPks == 3) {
                //$(".Edit").show();
            }
        }
        if (data.StatusPks == 0 || data.StatusPks == 3)
            $("#bingkai-upload-legal").hide();
        if (data.StatusPks != 0 && data.StatusPks != 3) {
            if (data.isOwner== 0 && data.Approver==0) $("#HapusFile").remove();                
            $(".input-pks").attr("disabled", true);
            $(".cari-pengadaan").remove();
            $(".Simpan-Setuju").remove();
            $(".Simpan").remove();
            $(".Stop").remove();
            $(".ajukan").remove();
            $(".Hapus").remove();
            if (data.StatusPks > 1) { //kalo udah approve 
                $("#bingkai-upload-legal").show();
            }
        }
        if (data.Approver == 1) {
            $(".pending").show();
        }//off-pending
        else {
            $(".pending").hide();
        }
        if (data.StatusPks == 2 && data.isOwner == 1) {
            
            $("#dok-assign").show();
        }
        if (data.StatusPks == 2 && data.Approver == 1) {
            $(".setujui").show();
        }
        if (data.StatusPks == 3) {
            //alert("lala");
            $("#dok-assign").show();
            $("#bingkai-upload-legal").hide();
            //document.getElementById("note").disabled = true;
            $("#note").attr("disabled", true);
            $("#title-pks").attr("disabled", true);
            $("#tanggal-mulai").attr("disabled", true);
            $("#tanggal-selesai").attr("disabled", true);
        }
        if (data.StatusPksName == "Approve") { $("#Status").text("Done"); }
        else { $("#Status").text(data.StatusPksName);}
        //setujui from staff 1.Pending
        if (data.isOwner == 1 && data.StatusPks == 1) {
            $(".pending").show();
            $(".done").show();
        }
        //setujui from staff 2.Setujui
        if (data.StatusPks == 2 && data.isOwner == 1) {
            $(".setujui").show();
        }
        if ($("#isStaffProc").val() == 1 && data.StatusPks == 3) {
            $(".simpan").show();
            $(".Stop").show();
            $("#tanggal-selesai").attr("disabled", false);
            $("#tanggal-mulai").attr("disabled", false);
        }
        if ($("#isStaffProc").val() != 1 && data.StatusPks == 3) {
            $(".simpan").remove();
            $(".Stop").remove();
            $("#tanggal-selesai").attr("disabled", false);
            $("#tanggal-mulai").attr("disabled", false);
        }
        //alert("Status " + data.StatusPksName + " Kodenya " + $("#StatusPks").val() + " Owner " + $("#isOwner").val());
        loadCatatan();
    });

}

function renderDokumenDropzone(myDropzone, tipe) {
    $.ajax({
        url: "Api/Pks/getDokumens?Id=" + PksId + "&tipe="+tipe,
        success: function (data) {
          if(data.length>0 && typeof(data)=='object') {
            //data = DOMPurify.sanitize(data);
            for (var key in data) {
                var file = {
                    Id: DOMPurify.sanitize(data[key].Id), name: DOMPurify.sanitize(data[key].File), accepted: true,
                    status: Dropzone.SUCCESS, processing: true, size: DOMPurify.sanitize(data[key].SizeFile)
                };
                //thisDropzone.options.addedfile.call(thisDropzone, file);
                myDropzone.emit("addedfile", file);
                myDropzone.emit("complete", file);
                myDropzone.files.push(file);
            }
          }
        },
        error: function (errormessage) {
            //location.reload();
        }
    });
}

function save(pks) {

    if (!csrfToken) {
        alert("CSRF token belum siap, refresh halaman");
        return;
    }

    var headers = {};
    if (csrfToken) {
        headers['X-CSRF-Token'] = csrfToken;
        headers['X-XSRF-TOKEN'] = csrfToken;
    }

    waitingDialog.showloading("Proses Harap Tunggu");

    $.ajax({
        url: "Api/Pks/Save",
        method: "POST",
        contentType: "application/json",
        headers: headers,
        data: JSON.stringify(pks), 
        success: function (data) {

            loadDetail(data.data?.Id || data.Id);

            waitingDialog.hideloading();

            BootstrapDialog.show({
                title: 'Informasi',
                message: data.message || "Sukses",
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        },
        error: function (xhr) {

            waitingDialog.hideloading();

            let msg = "Terjadi kesalahan";

            if (xhr.responseJSON && xhr.responseJSON.Message) {
                msg = xhr.responseJSON.Message;
            }

            BootstrapDialog.show({
                title: 'Error',
                message: msg,
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
        }
    });
}

function savesetuju(pks) {

    if (!csrfToken) {
        alert("CSRF token belum siap, refresh halaman");
        return;
    }

    var headers = {};
    if (csrfToken) {
        headers['X-CSRF-Token'] = csrfToken;
        headers['X-XSRF-TOKEN'] = csrfToken;
    }

    waitingDialog.showloading("Proses Harap Tunggu");

    $.ajax({
        url: "Api/Pks/SaveSetuju",
        method: "POST",
        data: pks,
        headers: headers,
        error: function (xhr) {
            waitingDialog.hideloading();
            alert("Gagal save setuju");
            console.error(xhr.responseText);
        }
    }).done(function (data) {

        if (data && data.Id) {
            loadDetail(data.Id);
        }

        var msg = data && data.message ? data.message : "Berhasil";

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
    });
}

function ajukan() {
    waitingDialog.showloading("Proses Harap Tunggu");
    $.ajax({
        url: "Api/Pks/ajukan?Id=" + $("#pksId").val(),
        method: "POST",
    }).done(function (data) {
        if (data.Id != "") {
            $(".ajukan").remove();
            $(".Hapus").remove();
            $(".Simpan-Setuju").remove();
            $(".Simpan").remove();
            $(".Stop").remove();
            $("input").attr("disabled", true);
            $("textarea").attr("disabled", true);
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

    });
}

$(function () {
    $(".pending").on("click", function () {
        $("#konfirmasiPending").modal("show");

    });

    $("#pending-pks").on("click", function () {
        $("#konfirmasiPending").modal("hide");
        waitingDialog.showloading("Proses Harap Tunggu");
        var catatan = $("#note-pending").val();
        var Id = $("#pksId").val();
        $.ajax({
            url: "Api/Pks/Pending?note=" + catatan + "&Id=" + Id,
            method: "GET",
        }).done(function (data) {
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

        });
    });
    
    $(".setujui").on("click", function () {        
        $("#KonfirmasiSetujui").modal("show");
    });

    $("#setujui-pks").on("click", function () {

        if (!csrfToken) {
            alert("CSRF token belum siap, refresh halaman");
            return;
        }

        var headers = {};
        if (csrfToken) {
            headers['X-CSRF-Token'] = csrfToken;
            headers['X-XSRF-TOKEN'] = csrfToken;
        }

        var IdPks = $("#pksId").val();
        var NoPks = $("#no-pks").val();
        var Note = $("#note-setujui").val();

        waitingDialog.showloading("Proses Harap Tunggu");

        $.ajax({
            url: "Api/Pks/Setujui",
            method: "POST",
            contentType: "application/json",
            headers: headers,
            data: JSON.stringify({
                Id: IdPks,
                Note: Note,
                NoPks: NoPks
            }),
            success: function (data) {

                table.draw();
                $("#KonfirmasiSetujui").modal("hide");
                waitingDialog.hideloading();

                BootstrapDialog.show({
                    title: 'Informasi',
                    message: data.message || "Sukses",
                    buttons: [{
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });

                loadDetail(IdPks);
            },
            error: function (xhr) {

                waitingDialog.hideloading();

                let msg = "Terjadi kesalahan";

                if (xhr.responseJSON && xhr.responseJSON.Message) {
                    msg = xhr.responseJSON.Message;
                }

                BootstrapDialog.show({
                    title: 'Error',
                    message: msg,
                    buttons: [{
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            }
        });
    });

    $(".kirim-catatan").on("click", function () {
        if ($("#isOwner").val() == 1 || $("#Approver").val() == 1) {
            sendNote($("#pksId").val(), $("#catatan").val());
        }
        $("#catatan").val("");
    });

    $("#done-pks").on("click", function () {

        if (!csrfToken) {
            alert("CSRF token belum siap, refresh halaman");
            return;
        }

        var headers = {};
        if (csrfToken) {
            headers['X-CSRF-Token'] = csrfToken;
            headers['X-XSRF-TOKEN'] = csrfToken;
        }

        var pks = {};
        pks.Note = $("#note").val();
        pks.TanggalMulaiStr = moment($("#tanggal-mulai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        pks.TanggalSelesaiStr = moment($("#tanggal-selesai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        pks.Title = $("#title-pks").val();
        pks.PemenangPengadaanId = $("#PemenangPengadaanId").val();
        pks.Id = $("#pksId").val();

        var IdPks = $("#pksId").val();
        var NoPks = $("#no-pks-done").val();
        var Note = $("#note-setujui-done").val();

        waitingDialog.showloading("Proses Harap Tunggu");

        // 🔹 1. Save
        $.ajax({
            url: "Api/Pks/Save",
            method: "POST",
            data: pks,
            headers: headers,
            error: function (xhr) {
                waitingDialog.hideloading();
                alert("Gagal save PKS");
                console.error(xhr.responseText);
            }
        }).done(function () {

            // 🔹 2. Ajukan
            $.ajax({
                url: "Api/Pks/ajukan?Id=" + encodeURIComponent(IdPks),
                method: "POST",
                headers: headers,
                error: function (xhr) {
                    waitingDialog.hideloading();
                    alert("Gagal ajukan PKS");
                    console.error(xhr.responseText);
                }
            }).done(function () {

                // 🔹 3. Setujui
                $.ajax({
                    url: "Api/Pks/Setujui?Id=" + encodeURIComponent(IdPks) +
                        "&Note=" + encodeURIComponent(Note) +
                        "&NoPks=" + encodeURIComponent(NoPks),
                    method: "POST",
                    headers: headers,
                    error: function (xhr) {
                        waitingDialog.hideloading();
                        alert("Gagal setujui PKS");
                        console.error(xhr.responseText);
                    }
                }).done(function (data) {

                    table.draw();
                    $("#KonfirmasiSetujui").modal("hide");

                    var msg = data && data.message ? data.message : "Berhasil";

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

                    loadDetail(IdPks);
                    $(".Hapus").remove();
                    $(".done").remove();
                });
            });
        });
    });
});


function loadCatatan() {
    var Id = $("#pksId").val();
    $.ajax({
        url: "Api/Pks/ListCatatan?Id=" + Id,
        method: "GET",
    }).done(function (data) {
        renderCatatan(data);
    });
}
function renderCatatan(data) {
    var html = "";
    for(var i in data){        
        html+= '<li class="item" style="padding-top:1px">'+
                 '<div class="riwayat-info" data-toggle="tooltip" title="'+data[i].Nama+'">'+
                    ' <span class="title"><a href="#" >' + data[i].Status + '</a></span>' +
                     '<span class="pegadaan-description" >' + moment(data[i].CreatedOn).format("DD/MM/YYYY hh:mm:ss") + '</span>' +
                     '<span class="pegadaan-item" >' + data[i].Catatan + '</span>' +
                 '</div>'+
             '</li>';
    }
    $(".list-catatan").html("");
    $(".list-catatan").append(html);
}

function sendNote(pksId, note) {

    if (!csrfToken) {
        alert("CSRF token belum siap, refresh halaman");
        return;
    }

    var headers = {};
    if (csrfToken) {
        headers['X-CSRF-Token'] = csrfToken;
        headers['X-XSRF-TOKEN'] = csrfToken;
    }

    waitingDialog.showloading("Proses Harap Tunggu");

    $.ajax({
        url: "Api/Pks/SendNote",
        method: "POST",
        data: {
            Id: pksId,
            note: note
        },
        headers: headers,
        error: function (xhr) {
            waitingDialog.hideloading();
            alert("Gagal kirim catatan");
            console.error(xhr.responseText);
        }
    }).done(function () {
        waitingDialog.hideloading();
        loadCatatan();
    });
}