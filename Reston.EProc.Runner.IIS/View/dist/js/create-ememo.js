var id_rks = DOMPurify.sanitize(window.location.hash);

function getListKandidat() {
    arrKandidatPengadaan = [];
    $(".list-kandidat").each(function () {
        objKandidatPengadaan = {};
        objKandidatPengadaan.VendorId = $(this).val();
        arrKandidatPengadaan.push(objKandidatPengadaan);
    });
    return arrKandidatPengadaan;
}

$("#tambah-klarifikasi-lanjut").on("click", function () {
    if ($("#tambah-klarifikasi-lanjut").prop("checked") == true) {
        $("#grp_klarifikasi_lanjutan").show();
    }
    else if ($("#tambah-klarifikasi-lanjut").prop("checked") == false) {
        $("#grp_klarifikasi_lanjutan").hide();
    }
});

$("#tambah-penilaian").on("click", function () {
    if ($("#tambah-penilaian").prop("checked") == true) {
        $("#grp_penilaian").show();
    }
    else if ($("#tambah-penilaian").prop("checked") == false) {
        $("#grp_penilaian").hide();
    }
});


function loadListKandidat(Id, status,xisTEam) {
    $(".listkandidat").html("");
        $.ajax({
            method:"POST",
            url: "Api/EMemo/GetKandidats?PId=" + Id
        }).done(function (data) {
			data = DOMPurify.sanitize(data);
            $.each(data, function (index, item) {
                var html = '<div class="col-md-3"><div class="box box-rekanan">';
                if (status == 0 && xisTEam==1) {
                    html = html+'<div class="box-tools pull-right">' +
                            '<span class="badge bg-red remove-vendor" attr="' + item.Id + '">' +
                               '<i class="fa fa-remove"></i>' +
                          '</span>' +
                            '</div>';                   
                }
               html=html+ '<div class="box-body box-profile">' +
                 '<input type="hidden" class="list-kandidat" value="' + item.VendorId + '" />' +
                 '<p class="profile-username title-header">' + item.Nama + '</p>' +
                 //'<p class="text-muted text-center deskripsi">' + item.kontak + '</p>' +
                 '<p class="text-muted text-center deskripsi">' + item.Telepon + '</p>' +
               '</div>' +
             '</div></div>';
            
                $(".listkandidat").append(html);
            });
        });
    
}

function getListPersonil() {
    arrPersonilPengadaan = [];
    $(".list-personil").each(function () {
        objPersonilPengadaan = {};
        objPersonilPengadaan.PersonilId = $(this).val();
        objPersonilPengadaan.tipe = $(this).attr("attr1");
        objPersonilPengadaan.Nama = $(this).attr("attr2");
        objPersonilPengadaan.Jabatan = $(this).attr("attr3");
        arrPersonilPengadaan.push(objPersonilPengadaan);
    });
    return arrPersonilPengadaan;
}

function LoadListPersonil(Personil,Status,xisTeam) {
    $(".listperson-pic").html("");
    $(".listperson-staff").html("");
    $(".listperson-controller").html("");
    $(".listperson-compliance").html("");
    $(".listperson-tim").html("");
    for (var item in Personil) {
        if (Personil[item].tipe == "pic")
            addLoadPersonil(Personil[item], ".listperson-pic", Status, xisTeam);
        if (Personil[item].tipe == "staff")
            addLoadPersonil(Personil[item], ".listperson-staff", Status, xisTeam);
        if (Personil[item].tipe == "tim")
            addLoadPersonil(Personil[item], ".listperson-tim", Status, xisTeam);
        if (Personil[item].tipe == "controller")
            addLoadPersonil(Personil[item], ".listperson-controller", Status, xisTeam);
        if (Personil[item].tipe == "compliance")
            addLoadPersonil(Personil[item], ".listperson-compliance", Status, xisTeam);
    }
}

function getBerkas() {
    var arrBerkas = [];
    var Berkas = {};
    Berkas.Title = $("[name=TitleDokumenNotaInternal]").val();
    Berkas.File = $("[name=FileDokumenNotaInternal]").val();
    Berkas.ContentType = $("[name=FileDokumenNotaInternal]").attr("attr1");
    if ($("[name=FileDokumenNotaInternal]").attr("attr2") != "") Berkas.Id = $("[name=FileDokumenNotaInternal]").attr("attr2");
    Berkas.Tipe = "NOTA";
    arrBerkas.push(Berkas);

    Berkas = {};
    Berkas.Title = $("[name=TitleDokumenLain]").val();
    Berkas.File = $("[name=FileDokumenLain]").val();
    Berkas.ContentType = $("[name=FileDokumenLain]").attr("attr1");
    if ($("[name=FileDokumenLain]").attr("attr2") != "") Berkas.Id = $("[name=FileDokumenNotaInternal]").attr("attr2");
    Berkas.Tipe = "DOKUMENLAIN";
    arrBerkas.push(Berkas);
    Berkas = {};
    Berkas.Title = $("[name=TitleBerkasRujukanLain]").val();
    Berkas.File = $("[name=FileBerkasRujukanLain]").val();
    if ($("[name=FileBerkasRujukanLain]").attr("attr2") != "") Berkas.Id = $("[name=FileDokumenNotaInternal]").attr("attr2");
    Berkas.ContentType = $("[name=FileBerkasRujukanLain]").attr("attr1");
    Berkas.Tipe = "BerkasRujukanLain";
    arrBerkas.push(Berkas);
   
    return arrBerkas;
}

function silensave(pengadaanHeader,attr1) {
    var data = {};
    data.Pengadaan = pengadaanHeader;
    pengadaanHeader.Status = "Draft";
    $.ajax({
        method: "POST",
        url: "Api/EMemo/save?status=Draft",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        if (data.status == 200) {
            $("#ememoId").val(data.Id);
            window.location.hash = data.Id;
        }
        else {
            alert("error");
        }
    });
}

function saveawal(pengadaanHeader, attr1) {
    var data = {};
    data.Pengadaan = pengadaanHeader;
    pengadaanHeader.Status = "Draft";
    waitingDialog.showloading("Proses Harap Tunggu");
    $.ajax({
        method: "POST",
        url: "Api/EMemo/save?status=Draft",
        dataType: "json",
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        waitingDialog.hideloading();
        if (attr1 == "showmodal") {
            $("#format-rks").attr("href", "rks.html#" + data.Id);
            $("#hpsmodal-list").modal("show");
            if (data.status == 200) {
                $("#ememoId").val(data.Id);
                window.location.hash = data.Id;
                loadData(data.Id);
            }
        } else {
            //$("#ememoId").val(data.Id);
            if (data.status == 200) {
                $("#ememoId").val(data.Id);
                window.location.hash = data.Id;
            }
            if (status == "Draft") {
                $("#TitleKonfirmasi").html("Info");
                $("#KontenConfirmasi").html(data.message);
                $("#konfirmasi").modal("show");
                loadData(data.Id);
            }
            else if (status == "Ajukan") {
                window.location.replace(HOME_PAGE + "/pengadaan-list.html");
            }
        }
    });
}

function save(eMemo,attr1,status) {
    var data = {};
    data.EMemo = eMemo;
    eMemo.IsDraft = status;
    waitingDialog.showloading("Proses Harap Tunggu");
    $.ajax({
        method: "POST",
        url: "Api/EMemo/save?status=" + status,
        dataType: "json",
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        waitingDialog.hideloading();
        if (attr1 == "showmodal") {
            $("#format-rks").attr("href", "rks.html#" + data.Id);
            $("#hpsmodal-list").modal("show");
        } else {
            //$("#ememoId").val(data.Id);
            if (data.status == 200) {                
                window.location.hash = data.Id;
                if (!$("#ememoId").val().trim()) {
                    window.location.reload();
                }
                $("#ememoId").val(data.Id);
            }
            if (status == "Draft") {
                $("#TitleKonfirmasi").html("Info");
                $("#KontenConfirmasi").html(data.message);
                $("#konfirmasi").modal("show");
                loadData(data.Id);
            }
            else if (status == "Ajukan") {
                window.location.replace(HOME_PAGE + "/pengadaan-list.html");
            }
        }
    });
}


function getHeaderPengadaan() {
    var viewEMemo = {};
    viewEMemo.Subject = $("[name=Subject]").val();
    viewEMemo.DocumentNo = $("[name=DocumentNo]").val();
    viewEMemo.WorkUnitCode = $("[name=WorkUnitCode]").val();;
    viewEMemo.InternalRefNo = $("[name=InternalRefNo]").val();
    viewEMemo.HPSAmount = $("[name=HPSAmount]").val();
    viewEMemo.HPSCurrency = $("[name=HPSCurrency]").val();
    viewEMemo.IsDraft = $("[name=IsDraft]").val();
    viewEMemo.Owner = $("[name=Owner]").val();
    if ($("#ememoId").val() != "") viewEMemo.Id = $("#ememoId").val();
    return viewEMemo;
}

function addPersonilz(item, el) {
    var peran = el.replace(".listperson-", "");
    var objParticipant = {};
    objParticipant.UserId = item.UserId;
    objParticipant.EmployeeName = item.EmployeeName;
    objParticipant.Ordered = item.Ordered;
    objParticipant.ParticipantRole = item.ParticipantRole;
    objParticipant.EMemoId = $("#ememoId").val();

    if (isPersonileEksisPerPeran(item.UserId, el) == 1) {
        BootstrapDialog.show({
            title: 'Konfirmasi',
            message: 'User yang Ingin ditambahkan Sudah Ada!',
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        return false;
    }

    $.ajax({
        method: "POST",
        url: "Api/EMemo/savePersonil",
        dataType: "json",
        data: JSON.stringify(objParticipant),
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        if (data.status == 200) {
            html = '<a class="btn btn-app btn-person">' +
                    '<input name="list-array[]" type="hidden" class="list-personil" attrId="'
                + data.Id + '" attr1="' + item.Ordered + '" attr2="' + item.EmployeeName + '" attr3="'
                    + item.Jabatan + '" value="' + item.UserId + '" />' +
                    item.Nama +
                    '<span class="badge bg-red remove-person"><i class="fa fa-remove"></i></span>' +
                  '</a>';
            $(el).append(html);
        }
        else {
            alert("error");
        }
    });

}



function addPersonil(item, el) {
    var peran = el.replace(".listperson-", "");
    var objPersonilPengadaan = {};
    objPersonilPengadaan.PersonilId = item.PersonilId;
    objPersonilPengadaan.Nama = item.Nama;
    objPersonilPengadaan.Jabatan = item.Jabatan;
    objPersonilPengadaan.PengadaanId = $("#ememoId").val();

    if (isPersonileEksisPerPeran(item.PersonilId, el) == 1) {
        BootstrapDialog.show({
            title: 'Konfirmasi',
            message: 'User yang Ingin ditambahkan Sudah Ada!',
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        return false;
    }

    $.ajax({
        method: "POST",
        url: "Api/EMemo/savePersonil",
        dataType: "json",
        data: JSON.stringify(objPersonilPengadaan),
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        if (data.status == 200) {
            html = '<a class="btn btn-app">' +
                '<input type="hidden" class="list-personil" attrId="'
                + data.Id + '" attr1="' + peran + '" attr2="' + item.Nama + '" attr3="'
                + item.Jabatan + '" value="' + item.PersonilId + '" />' +
                item.Nama +
                '<span class="badge bg-red remove-person"><i class="fa fa-remove"></i></span>' +
                '</a>';
            $(el).append(html);
        }
        else {
            alert("error");
        }
    });

}

function loadHeaderPengadaan(viewEMemo) {
    var xPermision = viewEMemo.isCreated == 1 ? 1 : viewEMemo.isTEAM == 1 ? 1 : 0;
    LoadKriteriaPembobotan(viewEMemo.Id, xPermision);
    if (viewEMemo.Status == 0 || viewEMemo.Status == 10) {
        if (viewEMemo.isPIC == 1 && viewEMemo.Status != 10) {
            $(".SimpanAjukan").show();
            
        }
        $("#isTeam").val(viewEMemo.isTEAM);
        $("#isCreated").val(viewEMemo.isCreated);
        $("#State").val(viewEMemo.Status);
        if (viewEMemo.isTEAM == 1 || viewEMemo.isCreated == 1)
            $(".Simpan").show();
        if (viewEMemo.isTEAM == 0 && viewEMemo.isCreated==0) {
            $("input").attr("disabled", "disabled");
            $("select").attr("disabled", "disabled");
            $("checkbox").attr("disabled", "disabled");
            $("#buat_hps").text("Lihat HPS");
            $("textarea").attr("disabled", "disabled");
            $("#HapusFile").hide();
            $("#addVendorItem").hide();
            $("#addPerson-pic").parent().text("PIC");
            $("#addPerson-tim").parent().text("Tim");
            $("#addPerson-staff").parent().text("User");
            $("#addPerson-controller").parent().text("Controller");
            $("#addPerson-complaince").parent().text("Complaince");
        }
        if ((viewEMemo.isPIC == 1 || viewEMemo.isCreated)) {
            $(".Hapus").show();
        }
    }

    if (viewEMemo.Status == 1) {
        //buat_hps
        $("input").attr("disabled", "disabled");
        $("select").attr("disabled", "disabled");
        $("textarea").attr("disabled", "disabled");
        $("#buat_hps").text("Lihat HPS");
        $("#buat_hps").attr("status", viewEMemo.Status);
        $(".SimpanAjukan").show();
    }

    if (viewEMemo.Status == 10) {
        
        $(".SimpanAjukan").hide();
    }

    if (viewEMemo.Approver == 1) {
        $(".Setujui").show();
        $(".Tolak").show();
    }
        
    $("[name=Subject]").val(viewEMemo.Subject);
    $("[name=DocumentNo]").val(viewEMemo.DocumentNo) ;
    $("[name=InternalRefNo]").val(viewEMemo.InternalRefNo);    
    $("[name=HPSAmount]").val(viewEMemo.HPSAmount) ;
    $("[name=HPSCurrency]").val(viewEMemo.HPSCurrency);
    $("[name=IsDraft]").val(viewEMemo.IsDraft);
    $("[name=Owner]").val(viewEMemo.PeriodeAnggaran);
    $("[name=WorkUnitCode]").val(viewEMemo.JenisPembelanjaan);
    $("#ememoId").val(viewEMemo.Id);

    aturanPenawaranView();
    aturanPengadaanView();

    loadListKandidat(viewEMemo.Id, viewEMemo.Status, xPermision);
    LoadListPersonil(viewEMemo.PersonilPengadaans, viewEMemo.Status, xPermision);
    loadKualifikas(viewEMemo.KualifikasiKandidats)
    hitungHPS(id_rks);

}

function loadData(id_rks) {
    $.ajax({
        method: "POST",
        url: "Api/EMemo/detailEMemo?Id=" + id_rks
    }).done(function (data) {  
data = DOMPurify.sanitize(data);	
        loadHeaderPengadaan(data);
        
    });
}

function addVendor(item) {
    var kandidat = {};
    kandidat.PengadaanId = $("#ememoId").val();
    kandidat.VendorId = item.id;
    if (isRekananExsis(item.id) == 1) {
        BootstrapDialog.show({
            title: 'Konfirmasi',
            message: 'Kandidat Sudah Ada!',
            buttons: [{
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        return false;
    }
    $.ajax({
        method: "POST",
        url: "Api/EMemo/saveKandidat",
        dataType: "json",
        data: JSON.stringify(kandidat),
        contentType: 'application/json; charset=utf-8'
    }).done(function (data) {
		data = DOMPurify.sanitize(data);

        if (data.status == 200) {
            
            var html = '<div class="col-md-3"><div class="box box-rekanan">' +
             '<div class="box-tools pull-right">';

            if ($("#isTeam").val() == "1" || $("#isCreated").val()=="1") {
                html = html + '<span class="badge bg-red remove-vendor" attr="' + data.Id + '">' +
                         '<i class="fa fa-remove"></i>' +
                    '</span>';
            }
            
                //'<button class="remove-vendor btn btn-box-tool" attr="' + data.Id + '"><i class="fa fa-times"></i></button>' +
            html = html + '</div>' +
               '<div class="box-body box-profile">' +
                '<input type="hidden" class="list-kandidat" value="' + item.id + '" />' +
                '<p class="profile-username title-header">' + item.Nama + '</p>' +
                //'<p class="text-muted text-center deskripsi">' + item.kontak + '</p>' +
                '<p class="text-muted text-center deskripsi">' + item.Telepon + '</p>' +
              '</div>' +
            '</div></div>';
            $(".listkandidat").append(html);
        }
        else {
            alert("error");
        }
    });
}

function isRekananExsis(rekananId) {
    var status = 0;
    $(".list-kandidat").each(function (index, el) {
        if ($.trim($(el).val()) == $.trim(rekananId)) {
            status = 1;
            return false;
        }
    });
    return status;
}

function hapusKandidat(Id,_this) {

    $.ajax({
        method: "POST",
        url: "Api/EMemo/deleteKandidat?Id="+Id
       
    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        if (data.status == 200) {
            _this.parent().parent().parent().remove();
        }
        else {
            alert("error");
        }
    });
}

function hapusPersonil(Id, _this) {

    $.ajax({
        method: "POST",
        url: "Api/EMemo/deleteParticipant?Id=" + Id

    }).done(function (data) {
		data = DOMPurify.sanitize(data);
        if (data.status == 200) {
            // _this.parent().parent().parent().remove();
            _this.parent().remove();
        }
        else {
            alert("error");
        }
    });
}

function addVendorLoad(item, kandidat) {
    //var html = '<div class="col-md-4"><div class="box box-primary">' +
    //        '<div class="box-tools pull-right">' +
    //            '<button class="remove-vendor btn btn-box-tool" attr="' + kandidat.Id + '"><i class="fa fa-times"></i></button>' +
    //          '</div>' +
    //         '<div class="box-body box-profile">' +
    //          '<input type="hidden" class="list-kandidat" value="' + item.id + '" />' +
    //          '<p class="profile-username title-header">' + item.Nama + '</p>' +
    //          //'<p class="text-muted text-center deskripsi">' + item.kontak + '</p>' +
    //          '<p class="text-muted text-center deskripsi">' + item.Telepon + '</p>' +
    //        '</div>' +
    //      '</div></div>';
    var html = '<div class="col-md-3"><div class="box box-rekanan">' +
            '<div class="box-tools pull-right">' +
                   '<span class="badge bg-red remove-vendor" attr="' + kandidat.Id + '">' +
                        '<i class="fa fa-remove"></i>' +
                   '</span>' +
                //'<button class="remove-vendor btn btn-box-tool" attr="' + data.Id + '"><i class="fa fa-times"></i></button>' +
              '</div>' +
             '<div class="box-body box-profile">' +
              '<input type="hidden" class="list-kandidat" value="' + item.id + '" />' +
              '<p class="profile-username title-header">' + item.Nama + '</p>' +
              //'<p class="text-muted text-center deskripsi">' + item.kontak + '</p>' +
              '<p class="text-muted text-center deskripsi">' + item.Telepon + '</p>' +
            '</div>' +
          '</div></div>';
    $(".listkandidat").append(html);
}

function addLoadPersonil(item, el, status, xisTeam) {
    var peran = el.replace(".listperson-", "");
    var removeEL = '';
    if (status == 0 && xisTeam == 1) {
        //removeEL = '<a class="pull-right btn-box-tool remove-person"><i class="fa fa-times"></i></a>';
        removeEL = '<span class="badge bg-red remove-person"><i class="fa fa-remove"></i></span>';
    }
    //if (item.isReady == 1) {
    //    if (item.isMine == 1 && $("#State").val() == 0)
    //        removeEL = removeEL + '<span class="badge-left check-person"><input type="checkbox" class="ready-checkbox" checked/></span>';
    //    else removeEL = removeEL + '<span class="badge-left check-person"><input type="checkbox" class="ready-checkbox" checked disabled /></span>';
    //}
    //else {
    //    if (item.isMine == 1 &&  $("#State").val() == 0)
    //        removeEL = removeEL + '<span class="badge-left check-person"><input type="checkbox" class="ready-checkbox"/></span>';
    //    else removeEL = removeEL + '<span class="badge-left check-person"><input type="checkbox" class="ready-checkbox" disabled/></span>';
    //}

    html = '<a class="btn btn-app">' +
                   '<input type="hidden" class="list-personil" attrId="'
                       + item.Id + '" attr1="' + peran + '" attr2="' + item.Nama + '" attr3="'
                       + item.Jabatan + '" value="' + item.PersonilId + '" />' +
                   removeEL +
                   '<i class="fa fa-user"></i>' +
                   item.Nama +
                 '</a>';
    $(el).append(html);
}

function isPersonileEksis(idPersonil,elm) {
    var status = 0;
    $(".list-personil").each(function (index, el) {
        if ($.trim($(el).val()) == $.trim(idPersonil)) {
            status = 1;
            return false;
        }
    });
    return status;
}

function isPersonileEksisPerPeran(idPersonil, elm) {
    var status = 0;
    $(elm).find(".list-personil").each(function (index, el) {
        if ($.trim($(el).val()) == $.trim(idPersonil)) {
            status = 1;
            return false;
        }
    });
    return status;
}


function viewFile(File) {
    $.ajax({
        url: "Api/EMemo/ViewFile?file=" + File,
        success: function (response) {
            $("#docViewFrame").attr("src", response);
            $("#view-doc-modal").modal("show");
        }
    });
}

function viewDocDetail(File) {
    $.ajax({
        url: "api/EMemo/OpenFile?file=" + File
    });
}

function loadKualifikas(kualifikasiKandidat) {
    $(".checkbox-kualifikasi").removeAttr("checked");
    $.each(kualifikasiKandidat, function (index, value) {
        $(".checkbox-kualifikasi[value='" + value.kualifikasi + "']").prop("checked", "true");
        $(".checkbox-kualifikasi[value='" + value.kualifikasi + "']").attr("attrId", value.Id);
    });
}

function LoadKriteriaPembobotan(PengadaanId,xIsTeam) {
    $("#kreteriaPembobotan").html("");
    $.ajax({
        method: "POST",
        url: "Api/EMemo/getKriteriaPembobotan?PengadaanId=" + PengadaanId,
        success:function(data){
            $.each(data, function (index, val) {
                html = '<div class="row">' +
			        '<div class="form-group col-md-4">' +
				        '<label class="title">' + val.NamaKreteria + ':</label>';
                if (xIsTeam == 1) html = html + '<input id="bobot-harga" attrId="' + val.Id + '" type="text" class="form-control input-bobot-pengadaan" value=' + val.Bobot + ' >';
                else html = html + '<input id="bobot-harga" disabled="disabled" attrId="' + val.Id + '" type="text" class="form-control input-bobot-pengadaan" value=' + val.Bobot + ' >';
                html=html+'</div>' +
		         '</div>';	
				$("#kreteriaPembobotan").append(html);
            });
        }
    });

    
}

function addPembobotanPengadaan(el) {
    var totalBobot = 0;
    $(".input-bobot-pengadaan").each(function () {
        totalBobot += $.isNumeric($(this).val()) == true ? parseInt($(this).val()) : 0;        
    });

    if (totalBobot > 100) {
        BootstrapDialog.show({
            title: 'Konfirmasi',
            message: 'Total Bobot Tidak Boleh Lebih dari 100%',
            buttons: [ {
                label: 'Batal',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        $(el).val(0);
        return false;
    }
    var oData = {};
    oData.KreteriaPembobotanId = el.attr("attrId");
    oData.Bobot = el.val();
    oData.PengadaanId = $("#ememoId").val();
    $.ajax({
        method: "POST",
        url: "Api/EMemo/addPembobotanPengadaan",
        dataType: "json",
        data: JSON.stringify(oData),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
			data = DOMPurify.sanitize(data);
            if(data==0)
                el.val(0);
        }
    });
}

function addLstPembobotanPengadaan() {
    var totalBobot = 0;
    var lstData = [];
    $(".input-bobot-pengadaan").each(function () {
        totalBobot += $.isNumeric($(this).val()) == true ? parseInt($(this).val()) : 0;
        var oData = {};
        oData.KreteriaPembobotanId = $(this).attr("attrId");
        oData.Bobot = $(this).val();
        oData.PengadaanId = $("#ememoId").val();
        lstData.push(oData);
    });

    if (totalBobot > 100) {
        BootstrapDialog.show({
            title: 'Konfirmasi',
            message: 'Total Bobot Tidak Boleh Lebih dari 100%',
            buttons: [{
                label: 'Batal',
                action: function (dialog) {
                    dialog.close();
                }
            }]
        });
        $(".input-bobot-pengadaan").val(0);
        return false;
    }

    
    $.ajax({
        method: "POST",
        url: "Api/EMemo/addLstPembobotanPengadaan",
        dataType: "json",
        data: JSON.stringify(lstData),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
			data = DOMPurify.sanitize(data);
            if (data == 0)
                $(".input-bobot-pengadaan").val(0);
        }
    });
}

$(function () {
    $("#grp_klarifikasi_lanjutan").hide();
    $("#grp_penilaian").hide();
    SetListUnitKerja("[name=WorkUnitCode]");
    SetListCurrency("[name=HPSCurrency]");
    $(".listkandidat").on("click", ".remove-vendor", function () {
        var Id = $(this).attr("attr");
        hapusKandidat(Id, $(this));
    });

    $("body").on("click", ".remove-person", function () {
        hapusPersonil($(this).parent().find("input").val(), $(this));
    });

    $("body").on("click", ".Hapus", function () {
        BootstrapDialog.show({
            title: 'Konfirmasi',
            message: 'Apakah Anda Yakin Ingin Menghapus Ini?',
            buttons: [{
                label: 'Lanjutkan',
                action: function (dialog) {
                    waitingDialog.showloading("Proses Harap Tunggu");
                    $.ajax({
                        method: "POST",
                        url: "Api/EMemo/deleteParticipant?Id=" + $("#ememoId").val()
                    }).done(function (data) {
						data = DOMPurify.sanitize(data);
                        window.location.replace(HOME_PAGE + "/ememo-list.html");
                        waitingDialog.hideloading();
                    });
                    
                    dialog.close();
                },
            }, {
                label: 'Close',
                action: function (dialog) {
                    dialog.close();
                },
            }]
        });
       
    });

    $(".Simpan").on("click", function () {  
        save(getHeaderPengadaan(), "", "Draft");
    });

    $(".SimpanAjukan").on("click", function () {
        if ($("[name=TitleDokumenNotaInternal]").val() == ""||myDropzoneDokumenNotaInternal.files.length==0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'Dokumen Nota Internal Wajib Diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }
        if ($('.ready-checkbox').not(':checked').length > 0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'Semua Personil Wajib Menceklis Kesiapan',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }
        if ($("div.listkandidat .col-md-3").length == 0&&$("#AturanPengadaan").val()=="Pengadaan Tertutup") {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'Kandidat Wajib diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }
        var cek = 1;
        $.each($(".dateJadwal"), function (index, el) {
            if ($(el).val() == "") {
                if ($(el).hasClass("klarifikasi_lanjutan") || $(el).hasClass("penilaian")) {
                    if ($("#tambah-klarifikasi-lanjut").prop("checked") && $(el).hasClass("klarifikasi_lanjutan")) {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Semua Jadwal Wajib diisi!',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }]
                        });

                        cek = 0;
                        return false;
                    } if ($("#tambah-penilaian").prop("checked") == true && $(el).hasClass("penilaian")) {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Semua Jadwal Wajib diisi!',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }]
                        });

                        cek = 0;
                        return false;
                    }
                }
                else {
                    if ($("[name=AturanPengadaan]").val() == 'Pengadaan Tertutup') {
                        if (index > 1) {
                            BootstrapDialog.show({
                                title: 'Konfirmasi',
                                message: 'Semua Jadwal Wajib diisi!',
                                buttons: [{
                                    label: 'Close',
                                    action: function (dialog) {
                                        dialog.close();
                                    }
                                }]
                            });
                            cek = 0;
                            return false;
                        }
                    }
                    else {
                        BootstrapDialog.show({
                            title: 'Konfirmasi',
                            message: 'Semua Jadwal Wajib diisi!',
                            buttons: [{
                                label: 'Close',
                                action: function (dialog) {
                                    dialog.close();
                                }
                            }]
                        });
                        cek = 0;
                        return false;
                    }
                }

            }
        });

        if (cek == 0) return false;
        
        if ($("div.listperson-pic .btn-app").length == 0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'PIC Wajib diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }

        if ($("div.listperson-tim .btn-app").length == 0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'Tim Wajib diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }

        if ($("div.listperson-staff .btn-app").length == 0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'User Wajib diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }

        if ($("div.listperson-controller .btn-app").length == 0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'Controller Wajib diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }

        if ($("div.listperson-compliance .btn-app").length == 0) {
            BootstrapDialog.show({
                title: 'Konfirmasi',
                message: 'Compliance Wajib diisi!',
                buttons: [{
                    label: 'Close',
                    action: function (dialog) {
                        dialog.close();
                    }
                }]
            });
            return false;
        }
        $("#modal-persetujuan").modal("show");
        // save(getHeaderPengadaan(), "", "Ajukan");
    });

    $("#anjukan-lanjutkan").on("click", function () {
        $("#modal-persetujuan").modal("hide");
        save(getHeaderPengadaan(), "", "Ajukan");
    });

    $("#buat_hps").on("click", function () {
        
        if ($(this).attr("status") == 0) {
            $("#hpsmodal").modal("show");
        }
        else window.location.replace(HOME_PAGE + "/rks.html#" + parseInt($("#ememoId").val()));
    });

    $("#simpanhps").on("click", function () {
        $("#hpsmodal").modal("hide");
        var dataPengadaan = getHeaderPengadaan();
        if (dataPengadaan.hasOwnProperty("Id")) {
            save(dataPengadaan, "showmodal", "Draft");
        }
        else {
            saveawal(dataPengadaan, "showmodal");
        }
        //save(getHeaderPengadaan(), "showmodal", "Draft");
    });

    if (isGuid(id_rks)) {
        $("#ememoId").val(id_rks);
        loadData(id_rks);
    }
    else {
        if (isGuid($("#ememoId").val())) {
            window.location.hash = $("#ememoId").val();
           
        } else {
            //$(".SimpanAjukan").show();
            $(".Simpan").show();
            //$(".Hapus").show();
           // silensave(getHeaderPengadaan());//save dulu
        }
    }
   // }

    $(".buat-baru").on("click", function () {
        window.location.replace("http://" + HOME_PAGE + window.location.pathname);
    });
    var acceptedFiles = ".png,.jpg,.pdf,.xls,.xlsx,.jpeg,.doc,.docx";
    
    $("#HapusFile").on("click", function () {
        var tipe = $(this).parent().parent().parent().parent().attr("attr1");
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
       
        $.ajax({
            method: "POST",
            url: "Api/EMemo/deleteDokumen?Id=" + FileId
        }).done(function (data) {
			data = DOMPurify.sanitize(data);
            if (data.Id == "1") {
                if (tipe == "DokumenNotaInternal") {

                    $.each(myDropzoneDokumenNotaInternal.files, function (index, item) {
                        var id = 0;
                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(item.xhr.response));
                        }
                        
                        if (id == FileId) {
                            myDropzoneDokumenNotaInternal.removeFile(item);
                            $("#konfirmasiFile").modal("hide");
                        }
                    });
                }
                if (tipe == "DokumenLain") {
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
                            $("#konfirmasiFile").modal("hide");
                        }
                    });
                }
                if (tipe == "BerkasRujukanLain") {
                    $.each(myDropzoneBerkasRujukanLain.files, function (index, item) {
                        myDropzoneBerkasRujukanLain.removeFile(item);
                        var id = 0;
                        if (item.Id != undefined) {
                            id = item.Id;
                        }
                        else {
                            id = $.parseJSON(DOMPurify.sanitize(file.xhr.response));
                        }

                        if (id == FileId) {
                            myDropzoneBerkasRujukanLain.removeFile(item);
                            $("#konfirmasiFile").modal("hide");
                        }
                    });
                }
            }

        });
    });

    $(".Setujui").on("click", function () {
        $.ajax({
            method:"POST",
            url: "Api/EMemo/persetujuan?Id=" + parseInt($("#pengadaanId").val()) ,
            success: function (data) {
				data = DOMPurify.sanitize(data);
                if (data.status == 200) {                    
                  //  $(location).attr('href', HOME_PAGE + "/pengadaan-list.html");
                    window.location.replace(HOME_PAGE + "/pengadaan-list.html");
                }
            },
            error: function (errormessage) {
                alert("gagal");
            }
        });
    });

    $(".checkbox-kualifikasi").on("click", function () {
        var ell = $(this);
        if ($(this).is(':checked')) {
            var data = {};
            data.kualifikasi = $(this).val();
            data.PengadaanId = $("#pengadaanId").val();           
            $.ajax({
                method: "POST",
                url: "Api/EMemo/saveKualifikasiKandidat",
                dataType: "json",
                data: JSON.stringify(data),
                contentType: 'application/json; charset=utf-8',
                success: function (xdata) {
                    $(ell).attr("attrId", xdata.Id);
                },
                error: function (errormessage) {
                    alert("gagal");
                }

            })

        }
        else {            
            $.ajax({
                method: "POST",
                url: "Api/EMemo/deleteKualifikasiKandidat?Id="+$(this).attr("attrId"),
                success: function (data) {
					data = DOMPurify.sanitize(data);
                    $(ell).attr("attrId", "");
                },
                error: function (errormessage) {
                    alert("gagal");
                }

            });
        }
    });

    $("[name=AturanPenawaran]").on('change', function () {
        aturanPenawaranView();
    });

    $("#downloadFile").on("click", function () {
        var tipe = $(this).parent().parent().parent().parent().attr("attr1");
        var FileId = $(this).parent().parent().parent().parent().attr("FileId");
        downloadFileUsingForm("/api/EMemo/OpenFile?Id=" + FileId);
    });
    
    $("#AturanPengadaan").on("change", function () {
        aturanPengadaanView();
    });
});

function aturanPengadaanView() {
    if ($("#AturanPengadaan").val() == "Pengadaan Terbuka") {
        //$("#BingkaiHps").hide();
        //$("#BingkaiKandidat").hide();
        $("#jadwal_pendaftaran").show();
        $("#aanwijzing").attr("disabled", "disabled");

    }
    else {
        // $("#BingkaiHps").show();
        $("#BingkaiKandidat").show();
        $("#jadwal_pendaftaran").hide();
        $("#aanwijzing").removeAttr("disabled");
    }
}

function renderDokumenDropzone(myDropzone,tipe) {
    $.ajax({
        url: "Api/EMemo/getDokumens?Id=" + $("#ememoId").val() + "&tipe=" + tipe,
        success: function (data) {
			data = DOMPurify.sanitize(data);
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

function hitungHPS(rksId) {

        $.ajax({
            url: "Api/EMemo/getRks?Id=" + rksId
        }).done(function (data) {
			data = DOMPurify.sanitize(data);
            var rksdetail = data.data;
            var total = 0;
            for (var key in rksdetail) {
                if (rksdetail[key].hps != null && rksdetail[key].hps != "") {
                    var jumlah = rksdetail[key].jumlah;
                    var hps = rksdetail[key].hps;
                    var totalPerItem = jumlah * hps;
                    total = total + totalPerItem;
                }
            }

            $("[name=HpsId]").val(total);
        });
    //}
}

function SetListCurrency() {
    $.ajax({
        url: "/api/ReferenceData/GetAllCurrency",
        success: function (data) {
			data = DOMPurify.sanitize(data);
            for (var i in data) {
                $("[name=HPSCurrency]").append("<option value='" + data[i].Name + "'>" + data[i].Name + "</option>");
            }
        }
    });
}




