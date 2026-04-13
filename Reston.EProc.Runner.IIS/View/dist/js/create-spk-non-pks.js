
var rawId = gup("id", window.location.href);
var safeId = /^[a-zA-Z0-9_-]+$/.test(rawId) ? rawId : "";
var SpkId = encodeURIComponent(safeId);


//var rawId = gup("id", window.location.href);
//var SpkId = DOMPurify.sanitize(rawId, { ALLOWED_TAGS: [], ALLOWED_ATTR: [] });
//if (!/^[a-zA-Z0-9-]+$/.test(SpkId)) {
//    SpkId = null; // atau handle error
//}

var myDropzoneSPK;
$(function () {
    //$("#pengadaanId").val(PengadaanId);
    //$("#VendorId").val(VendorId);
    if (SpkId != "" && SpkId != null) { loadDetail(SpkId); loadDetailDokNonPks(SpkId); }
    if ($("#pksId").val() != "" && (SpkId == null || SpkId == "")) { loadDetail($("#spkId").val()); loadDetailDokNonPks($("#spkId").val()); }
       //window.location.href("http://" + window.location.host + "/pks.html");
    if (SpkId == "" || SpkId == null) {
        SetListVendorWNon("");
    }

    $("#HapusFile").on("click", function () {
        var tipe = $(this).parent().parent().parent().parent().attr("attr1");
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
        $.ajax({
            method: "POST",
            url: "Api/Spk/deleteDokumenPks?Id=" + FileId
        }).done(function (data) {
            if (data.Id == "1") {
                    $.each(myDropzoneSPK.files, function (index, item) {
                        var id = 0;
                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneSPK.removeFile(item);
                        }
                    });
                }            
            $("#konfirmasiFile").modal("hide");
        });
    });

    $("#HapusFileNonPks").on("click", function () {
        waitingDialog.showloading("Proses Harap Tunggu");
        var tipe = $(this).parent().parent().parent().parent().attr("attr1");
        var klasifikasi = $(this).parent().parent().parent().parent().attr("attr2");
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
        $.ajax({
            method: "POST",
            url: "Api/Spk/deleteDokumenSpkNonPks?Id=" + FileId + "&klasifikasi=" + klasifikasi
        }).done(function (data) {
            if (klasifikasi == "Aanwijzing") {
                if (data.Id == "1") {
                    $.each(myDropzoneAnwijzing.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneAnwijzing.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            else if (klasifikasi == "Memo") {
                if (data.Id == "1") {
                    $.each(myDropzoneMemo.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneMemo.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            else if (klasifikasi == "Klarifikasi") {
                if (data.Id == "1") {
                    $.each(myDropzoneKlarifikasi.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneKlarifikasi.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            else if (klasifikasi == "KlarifikasiLanjutan") {
                if (data.Id == "1") {
                    $.each(myDropzoneKlarifikasiLanjutan.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneKlarifikasiLanjutan.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            else if (klasifikasi == "Penilaian") {
                if (data.Id == "1") {
                    $.each(myDropzonePenilaian.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzonePenilaian.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            else if (klasifikasi == "UsulanPemenang") {
                if (data.Id == "1") {
                    $.each(myDropzoneUsulanPemenang.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneUsulanPemenang.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            else if (klasifikasi == "DokumenLain") {
                if (data.Id == "1") {
                    $.each(myDropzoneDokumenLain.files, function (index, item) {
                        var id = 0;

                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneDokumenLain.removeFile(item);
                        }
                    });
                    $("#konfirmasiFileNonPks").modal("hide");
                }
            }
            waitingDialog.hideloading();
        });
    });

    myDropzoneSPK = new Dropzone("#dokspk",
             {
                 maxFilesize: 10,
                 acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
                 accept: function (file, done) {
                     this.options.url = $("#dokspk").attr("action") + "?id=" + $("#spkId").val();
                     if ($("#isOwner").val() != 1)
                         BootstrapDialog.show({
                             title: 'Konfirmasi',
                             message: 'Anda Tidak Punya Akses ',
                             buttons: [{
                                 label: 'Close',
                                 action: function (dialog) {
                                     myDropzoneSPK.removeFile(file);
                                     dialog.close();
                                 }
                             }]
                         });
                     else {
                         var jumFile = myDropzoneSPK.files.length;
                         if (SpkId == "") {
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

    myDropzoneAnwijzing = new Dropzone("#dokaanwijzing",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokaanwijzing").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneAnwijzing.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneAnwijzing.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneAnwijzing.removeFile(file);
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
                                        myDropzoneAnwijzing.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "Aanwijzing";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static"});
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "Aanwijzing");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneAnwijzing.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneAnwijzing.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneAnwijzing, "Aanwijzing");
    Dropzone.options.dokaanwijzing = false;

    myDropzoneMemo = new Dropzone("#dokMemo",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokMemo").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneMemo.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneMemo.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneMemo.removeFile(file);
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
                                        myDropzoneMemo.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "Memo";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "Memo");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneMemo.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneMemo.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneMemo, "Memo");
    Dropzone.options.dokMemo = false;

    myDropzoneSubmitPenawaran = new Dropzone("#dokSubmitPenawaran",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokSubmitPenawaran").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneSubmitPenawaran.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneSubmitPenawaran.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneSubmitPenawaran.removeFile(file);
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
                                        myDropzoneSubmitPenawaran.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "SubmitPenawaran";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "SubmitPenawaran");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneSubmitPenawaran.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneSubmitPenawaran.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneSubmitPenawaran, "SubmitPenawaran");
    Dropzone.options.dokSubmitPenawaran = false;

    myDropzoneKlarifikasi = new Dropzone("#dokKlarifikasi",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokKlarifikasi").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneKlarifikasi.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneKlarifikasi.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneKlarifikasi.removeFile(file);
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
                                        myDropzoneKlarifikasi.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "Klarifikasi";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "Klarifikasi");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneKlarifikasi.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneKlarifikasi.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneKlarifikasi, "Klarifikasi");
    Dropzone.options.dokKlarifikasi = false;

    myDropzoneKlarifikasiLanjutan = new Dropzone("#dokKlarifikasiLanjutan",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokKlarifikasiLanjutan").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneKlarifikasiLanjutan.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneKlarifikasiLanjutan.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneKlarifikasiLanjutan.removeFile(file);
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
                                        myDropzoneKlarifikasiLanjutan.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "KlarifikasiLanjutan";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "KlarifikasiLanjutan");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneKlarifikasiLanjutan.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneKlarifikasiLanjutan.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneKlarifikasiLanjutan, "KlarifikasiLanjutan");
    Dropzone.options.dokKlarifikasiLanjutan = false;

    myDropzonePenilaian = new Dropzone("#dokPenilaian",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokPenilaian").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzonePenilaian.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzonePenilaian.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzonePenilaian.removeFile(file);
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
                                        myDropzonePenilaian.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "Penilaian";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "Penilaian");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzonePenilaian.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzonePenilaian.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzonePenilaian, "Penilaian");
    Dropzone.options.dokPenilaian = false;

    myDropzoneUsulanPemenang = new Dropzone("#dokUsulanPemenang",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokUsulanPemenang").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneUsulanPemenang.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneUsulanPemenang.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneUsulanPemenang.removeFile(file);
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
                                        myDropzoneUsulanPemenang.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "UsulanPemenang";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "UsulanPemenang");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneUsulanPemenang.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneUsulanPemenang.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneUsulanPemenang, "UsulanPemenang");
    Dropzone.options.dokUsulanPemenang = false;

    myDropzoneDokumenLain = new Dropzone("#dokDokumenLain",
        {
            maxFilesize: 10,
            acceptedFiles: ".png,.jpg,.pdf,.xls,.jpeg,.doc,.xlsx",
            accept: function (file, done) {
                this.options.url = $("#dokDokumenLain").attr("action") + "&id=" + $("#spkId").val();
                if ($("#isOwner").val() != 1)
                    BootstrapDialog.show({
                        title: 'Konfirmasi',
                        message: 'Anda Tidak Punya Akses ',
                        buttons: [{
                            label: 'Close',
                            action: function (dialog) {
                                myDropzoneDokumenLain.removeFile(file);
                                dialog.close();
                            }
                        }]
                    });
                else {
                    var jumFile = myDropzoneDokumenLain.files.length;
                    if (SpkId == "") {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Simpan Dahulu Dokumen Sebelum Upload File',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    myDropzoneDokumenLain.removeFile(file);
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
                                        myDropzoneDokumenLain.removeFile(file);
                                        dialog.close();
                                    }
                                }]
                            });
                        } else {
                            var Id = $("#spkId").val();
                            var Klasifikasi = "DokumenLain";
                            $("#IdforAttribut").val(Id);
                            $("#KlasifikasiforAttribut").val(Klasifikasi);
                            $("#addatribut").modal({ "backdrop": "static" });
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
                        $("#konfirmasiFileNonPks").attr("attr1", "PKS");
                        $("#konfirmasiFileNonPks").attr("attr2", "DokumenLain");
                        $("#konfirmasiFileNonPks").attr("FileId", id);
                        $("#konfirmasiFileNonPks").modal("show");
                        if ($("#isOwner").val() == 1 && $("#StatusSpk").val() == 0) {
                            $("#HapusFile").show();
                        } else { $("#HapusFile").hide() }
                    });
                });

                this.on("error", function (file) {
                    myDropzoneDokumenLain.removeFile(file);
                });
                this.on("success", function (file, responseText) {
                    if (responseText == "00000000-0000-0000-0000-000000000000" || responseText == null) {
                        myDropzoneDokumenLain.removeFile(file);
                    }
                });
            }
        }
    );

    renderDokumenDropzoneNonPks(myDropzoneDokumenLain, "DokumenLain");
    Dropzone.options.dokDokumenLain = false;

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
        alert($("#tanggal-spk").val());
        var spk = {};
        spk.Id = $("#spkId").val();
        spk.Judul = $("#judul").val();
        spk.VendorIdNonPKS = $("[name = 'Vendor']").val();
        if ($("#tanggal-spk").val() != "" || $("#tanggal-spk").val() != "Invalid date") spk.TanggalSPKStr = moment($("#tanggal-spk").val(), ["D MMMM YYYY HH:mm"], "id").format("DD/MM/YYYY HH:mm");
        spk.NilaiSPK = $("#nilai-spk").val();
        //spk.VendorNonReg = $("#VendorNonReg").val();
        console.log(spk);
        //if ($("#isOwner").val() == 1 || $("#spkId").val() == "")
        //    save(spk);
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
        useCurrent: false
        //, minDate: Date.now()

    });

    $("#downloadFile").on("click", function () {
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
        downloadFileUsingForm("/api/Spk/OpenFile?Id=" + FileId);
    });

    $("#downloadFileNonPks").on("click", function () {
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
        var klasifikasi = $(this).parent().parent().parent().parent().attr("attr2");
        downloadFileUsingForm("/api/Spk/OpenFileNonPks?Id=" + FileId + "&klasifikasi=" + klasifikasi);
    });

});

function loadDetail(Id) {
    $.ajax({
        url: "Api/spk/DetailNonPKS?Id=" + Id
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
        $("#idVendor").val(data.VendorId);
        //alert("id nya" + $("#spkId").val());
        //if (data.TipeVendor == 4) {
        //    SetListVendorWNon($("#idVendor").val());
        //} else {
        //    SetListVendor($("#idVendor").val());
        //}
        SetListVendorWNon($("#idVendor").val());
        $("#Status").text(data.StatusPksName);

    });

}

function loadDetailDokNonPks(Id) {
    $.ajax({
        url: "Api/spk/detaildoknonpks?Id=" + Id
    }).done(function (data) {
        $("#no-aanwijzing").val(data.NoAanwijzing);
        $("#tanggal-aanwijzing").val(moment(data.TglAanwijzing).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
        $("#no-memo").val(data.NoMemo);
        $("#no-submit").val(data.NoSubPen);
        $("#tanggal-submit").val(moment(data.TglSubPen).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
        $("#no-klarifikasi").val(data.NoKlarfNeg);
        $("#tanggal-klarifikasi").val(moment(data.TglKlarfNeg).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
        $("#no-klarifikasianjutan").val(data.NoKlarfNegLan);
        $("#tanggal-klarifikasianjutan").val(moment(data.TglKlarfNegLan).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
        $("#no-penilaian").val(data.NoPenilai);
        $("#tanggal-penilaian").val(moment(data.TglPenilai).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
        $("#no-usulanpemenang").val(data.NoUsPen);
        $("#tanggal-usulanpemenang").val(moment(data.TglUsPen).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
        $("#no-dokumenlain").val(data.NoDokLain);
        $("#tanggal-dokumenlain").val(moment(data.TglDokLain).format("DD MMMM YYYY HH:mm")).attr("disabled", true);
    });
}

function renderDokumenDropzone(myDropzone) {
    var rSpkId = $("#spkId").val() || (typeof SpkId !== 'undefined' ? SpkId : null);
    if (!rSpkId) {
        console.warn("SPK ID tidak ditemukan");
        return;
    }
    $.ajax({
        url: "Api/Spk/getDokumens?Id=" + rSpkId,
        success: function (data) {
			//data = DOMPurify.sanitize(data);
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
        error: function (errormessage) {

            //location.reload();

        }
    });
}

function renderDokumenDropzoneNonPks(myDropzone, tipe) {
    var rSpkId = $("#spkId").val() || (typeof SpkId !== 'undefined' ? SpkId : null);
    if (!rSpkId) return;

    $.ajax({
        url: "Api/Spk/getDokumensNonPks?klasifikasi=" + tipe + "&id=" + rSpkId,
        method: "GET",
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
        error: function (errormessage) {
            console.error("Gagal mengambil dokumen:", errormessage);
            alert("Gagal memuat dokumen.");
        }
    });
}


function save(spk) {
    
    waitingDialog.showloading("Proses Harap Tunggu");
    $.ajax({
        url: "Api/spk/SaveNonPKS" ,
        method: "POST",
        data: spk ///JSON.stringify(pks)
    }).done(function (data) {
        loadDetail(data.Id);
        loadDetailDokNonPks(data.Id);
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

$("#template-spk").on("click", function () {
    //var tanggal = moment($("#tanggal-spk").val(), ["D MMMM YYYY HH:mm"], "id").format("DD/MM/YYYY HH:mm");
    downloadFileUsingForm("Api/Report/BerkasSPK2?Judul=" + $("#judul").val() + "&Tanggal_SPK=" + $("#tanggal-spk").val() + "&Vendor= " + $(".namaVendor").val() + "&Nilai_SPK=" + $("#nilai-spk").val());
    //alert("bgafugsafgsg")
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
    }).done(function (data) {
        loadDetail($("#spkId").val());
        loadDetailDokNonPks($("spkId").val());
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

    });
}

function SetListVendor(vendor) {
    $.ajax({
        url: "/api/Vendor/GetAllVendor",
        success: function (data) {
			//data = DOMPurify.sanitize(data);
            for (var i in data) {
                if (vendor == DOMPurify.sanitize(data[i].id)) {
                    $("[name = 'Vendor']").append("<option value='" + DOMPurify.sanitize(data[i].id) + "' selected>" + DOMPurify.sanitize(data[i].Nama) + "</option>");
                    $(".namaVendor").val(DOMPurify.sanitize(data[i].Nama));
                    //alert("namanya " + $(".namaVendor").val());
                }
                else {
                    $("[name = 'Vendor']").append("<option value='" + DOMPurify.sanitize(data[i].id) + "'>" + DOMPurify.sanitize(data[i].Nama) + "</option>");
                    //$("#idVendor").val(data[i].Nama);
                }
            }
            //$("[name = 'Vendor']").append("<option value='" + 0 + "'> -- Rekanan Tidak Terdaftar -- </option>");
        }
    });
}

function SetListVendorWNon(vendor) {
    $.ajax({
        url: "/api/Vendor/GetAllVendorWNon",
        success: function (data) {
			//data = DOMPurify.sanitize(data);
            for (var i in data) {
                if (vendor == DOMPurify.sanitize(data[i].id)) {
                    $("[name = 'Vendor']").append("<option value='" + DOMPurify.sanitize(data[i].id) + "' selected>" + DOMPurify.sanitize(data[i].Nama) + "</option>");
                    $(".namaVendor").val(DOMPurify.sanitize(data[i].Nama));
                    //alert("namanya " + $(".namaVendor").val());
                }
                else {
                    $("[name = 'Vendor']").append("<option value='" + DOMPurify.sanitize(data[i].id) + "'>" + DOMPurify.sanitize(data[i].Nama) + "</option>");
                    //$("#idVendor").val(data[i].Nama);
                }
            }
            //$("[name = 'Vendor']").append("<option value='" + 0 + "'> -- Rekanan Tidak Terdaftar -- </option>");
        }
    });
}

$(function () {
    $("[name = 'Vendor']").change(function () {
        var nv = $("[name = 'Vendor']").val();
        //if (nv == 0) $(".tidak-terdaftar").show(); else $(".tidak-terdaftar").hide();
        $.ajax({
            url: "api/Vendor/GetVendorDetail/" + nv,
            success: function (data) {
				//data = DOMPurify.sanitize(data);
                $("#idVendor").val(DOMPurify.sanitize(data.id));
                $(".namaVendor").val(DOMPurify.sanitize(data.Nama));
                //alert("id nya " + $("[name = 'Vendor']").val() + " " + $(".namaVendor").val());
            }
        });
        //}
    });

    //$(cek).on("click", function () {
    //});

    $("#adddokattribut").on("click", function () {
        //alert('idnya ' + $("#IdforAttribut").val() + ' klasifikasi ' + $("#KlasifikasiforAttribut").val());
        var TanggalDokStr = moment($("#tanggal-dokumen").val(), ["D MMMM YYYY HH:mm"], "id").format("DD/MM/YYYY HH:mm");
        waitingDialog.showloading("Proses Harap Tunggu");
        $.ajax({
            url: "Api/Spk/SimpanAtributDokumenNonPks?Id=" + $("#IdforAttribut").val() + "&klasifikasi=" + $("#KlasifikasiforAttribut").val() + "&NoDok=" + $("#nomor-dokumen").val() + "&TglDokString=" + TanggalDokStr,
            method: "POST",
        }).done(function (data) {
            loadDetailDokNonPks($("#IdforAttribut").val());
            $("#addatribut").modal("hide");
            $("#IdforAttribut").val("");
            $("#KlasifikasiforAttribut").val("");
            waitingDialog.hideloading();
            //alert('idnya ' + $("#IdforAttribut").val() + ' klasifikasi ' + $("#KlasifikasiforAttribut").val());
        });
    });


});