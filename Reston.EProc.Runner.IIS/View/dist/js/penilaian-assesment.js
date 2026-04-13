var SpkId = gup("id");

$(function () {

    function gup(name, url) {
        if (!url) url = location.href;
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(url);
        return results == null ? null : results[1];
    }

    //var pengadaanid = window.location.hash.replace("#", "");
    var pengadaanid = "";
    var vendorId = "";
    var sudahcreate = "";

    $.ajax({
        method: "POST",
        url: "Api/NilaiVendor/detailSPKNilaiVendor?Id=" + gup("Id"),
        dataType: "json"
    }).done(function (data) {
        pengadaanid = data.pengadaanId;
        vendorId = data.VendorId;
        tampiljudul();
        tampilpenilai();
        tampildropdownpenilai();
        sudahcreate = data.CekCreate;
        $(".list-question").html('');
        $(".label-kosong").remove();
        if (data.CekCreate == "sudah") {
            tampilassessment();
        } else
        {
            cekcreatepertanyaan();
            //tampilquestion();
        }
        //document.getElementById("simpan-assessment").style.visibility = "hidden";
        //document.getElementById("calculate").style.visibility = "hidden";
    });
    


    //cekcreatepertanyaan()

    userid();
    $(".create-pertanyaan").on("click", function () {
        cekcreatepertanyaan();
    });

    $("#hapus-question").on("click", function () {
        console.log("masuk fungsi hapus question");
        //alert("id nya " + pengadaanid);
        waitingDialog.showloading("Proses Harap Tunggu");
        $.ajax({
            method: "post",
            url: "api/NilaiVendor/deletePenilaian?Id=" + pengadaanid + "&VendorId=" + vendorId,
        }).done(function (data) {
            waitingDialog.hideloading();
            if (data == 0) {
                BootstrapDialog.show({
                    title: 'Error',
                    message: 'Save Gagal!',
                    buttons: [{
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
            }
            else {
                window.location.reload();
            }
        });

    });

    function cekcreatepertanyaan() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/CekCreatePertanyaan?Id=" + pengadaanid + "&VendorId=" + vendorId,
        }).done(function (data) {
            console.log(pengadaanid);
            rendercekcreate(data);
        });
    }

    function rendercekcreate(data) {

        if (data != null && sudahcreate == "sudah") {
            //console.log("yuhuuuu " + sudahcreate);
            $("#btn-add-pertanyaan").remove();
            $("#btn-add-pemenang").remove();
            //tampiljudul();
            tampilpointpenilaian();
            tampilvendorpenilai();
            tampilpersonilpenilai();
            $("#simpan").remove();
            $("#batal").remove();
            $("#hapus-question").show();
            $(".add-user-penilaian").remove();
        }
        else {
            $(".info").remove();
            $("#hapus-question").remove();
            //tampiljudul();
            tampilpertanyaan();
            tampilpemenang();
        }
        //$("#modal-create-pertanyaan").modal("show");
    }

    function tampilpointpenilaian() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/PointPenilaian?Id=" + pengadaanid + "&VendorId=" + vendorId,
        }).done(function (data) {
            renderpointpenilaian(data);
        });
    }

    function tampilvendorpenilai() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/TampilAssessment?Id=" + pengadaanid + "&VendorId=" + vendorId,
        }).done(function (data) {
            rendervendorassessment(data);
        });
    }

    function tampilpersonilpenilai() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/GetPersonPenilai?Id=" + pengadaanid + "&VendorId=" + vendorId,
            //url: "Api/NilaiVendor/tampilpersonilpenilai?Id=" + gup("Id"),
        }).done(function (data) {
            //console.log(data);
            //alert("mashook");
            renderpersonilpenilai(data);
        });
    }

    function renderpersonilpenilai(data) {
        //$(".controls-pemenang").remove();
        //$(".cekven").remove();
        var html = '';
        for (var i in data) {
            html = html + '<input type="text" class="form-control" style="margin-bottom:7px;" value="' + data[i].NamaPenilai + '" disabled>';
        }
        $(".entry-person").append(html);

        //var html = '';
        //for (var i in data) {
        //    html = html + '<input type="text" class="form-control cekven" style="margin-bottom:7px;" value="' + data[i].Nama + '" disabled/>';
        //}
        //$(".vp").append(html);
    }

    function renderpointpenilaian(data) {
        $(".controls").remove();
        $(".cekpoint").remove();
        $(".cekpoint-bobot").remove();
        var html = '';
        for (var i in data) {
            html = html + '<table style="width:100%;"><tr><td style="min-width:90%"><input type="text" class="form-control cekpoint" style="margin-bottom:7px;" value="' + data[i].LocalizedName + '" disabled/>';
            html = html + '</td><td style="min-width:10%"><input type="text" class="form-control cekpoint-bobot" style="margin-bottom:7px;" value="' + data[i].Bobot + '" disabled/></td>';
        
           // html = html + '<input type="text" class="form-control cekpoint" style="margin-bottom:7px;" value="' + data[i].LocalizedName + '" disabled/>';
           // html = html + '<input type="text" class="form-control cekpoint-bobot" style="margin-bottom:7px;" value="' + data[i].Bobot + '" disabled/>';
        }
        $(".pp").append(html);
    }

    function rendervendorassessment(data) {
        $(".controls-pemenang").remove();
        $(".cekven").remove();
        var html = '';
        for (var i in data) {
            html = html + '<input type="text" class="form-control cekven" style="margin-bottom:7px;" value="' + data[i].Nama + '" disabled/>';
        }
        $(".vp").append(html);
    }


    $("#calculate").on("click", function () {
        $(".total").remove();
        $(".avg").remove();
        var quest = {};
        var dd = {};
        var bbt = {};
        // Mencari Nilai Total
        dd.nilai = document.getElementsByClassName('quest');
        bbt.bobot = document.getElementsByClassName('question-bobot');

        var xyz = {};
        xyz.bobot = [];
        xyz.bobotutuh = [];
        quest.score = [];
        quest.total = 0;
        for (var i = 0; i < dd.nilai.length; i++) {
            quest.score[i] = dd.nilai[i].value;
            var a = parseFloat(quest.score[i]);
            var b = parseFloat(quest.total);
            //bobot di bagi 100
            xyz.bobot[i] = bbt.bobot[i].value;
            var x = parseFloat(xyz.bobot[i]);
            var y = x / 100;
            var z = a * y;

            var c = z + b;
            quest.total = c;
        }

        // Mencari Nilai Average
        quest.average = quest.total / dd.nilai.length;

        var html = '';
        html = html + '<p class="total">' + quest.total + '</p>';
        $(".total-score").append(html);

        var avhtml = '';
        avhtml = avhtml + '<p class="avg">' + quest.average + '</p>';
        $(".average-score").append(avhtml);
    });

    $(".create-asessment").on("click", function () {
        $(".list-question").html('');
        tampilassessment();
        tampiljudul();
        tampilpenilai();
        tampildropdownpenilai();
        $(".label-kosong").remove();
        $("#modal-create-assessment").modal("show");
    });

    function tampilassessment() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/TampilAssessment?Id=" + pengadaanid + "&VendorId=" + vendorId,
        }).done(function (data) {
            rendervendor(data);
        });
    }

    function tampildropdownpenilai() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/TampilDropDownPenilai?Id=" + pengadaanid + "&VendorId=" + vendorId,
        }).done(function (data) {
            renderlistpenilai(data);
        });
    }

    function tampilquestion() {
        var VendorId = document.getElementById('vendor').value;
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/TampilQuestion?Id=" + pengadaanid + "&VendorId=" + vendorId
        }).done(function (data) {
            renderquestion(data);
        });
    }

    function tampilpenilai() {
        $.ajax({
            method: "POST",
            url: "Api/PengadaanE/TampilPenilai?Id=" + pengadaanid
        }).done(function (data) {
            renderpenilai(data);
        });
    }

    function gantipenilai(userid) {
        $.ajax({
            method: "POST",
            url: "Api/PengadaanE/TampilGantiPenilai?Id=" + pengadaanid + "&Userid=" + userid
        }).done(function (data) {
            rendergantipenilai(data);
        });
    }

    function renderlistpenilai(data) {
        $('.drlist').remove();
        var userlogin = document.getElementById('user-id').value;
        var html = '';
        html = html + '<select class="form-control drlist" id="dlpenilai" disabled>';
        for (var i in data) {
            if (data[i].AppriserUserId == userlogin) {
                html = html + '<option value="' + data[i].AppriserUserId + '" selected>' + data[i].NamaPenilai + '</option>';
            }
            else {
                html = html + '<option value="' + data[i].AppriserUserId + '">' + data[i].NamaPenilai + '</option>';
            }
        }
        html = html + '</select>';
        $('.dropdown-penilai').append(html);
        gantivalue();
    }

    //$("#dlpenilai").change(function () {
    //    alert("Handler for .change() called.");
    //    gantivalue();
    //})

    $(".dropdown-penilai").on("change", ".drlist", function () {
        gantivalue();
    });

    $("#modal-create-assessment").on("change", "#vendor", function () {
        gantivalue();
    });

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

    function gantivalue() {
        //alert("Boom");
        var userid = document.getElementById("dlpenilai").value;
        //var VendorId = document.getElementById('vendor').value;
        var VendorId = vendorId;
        var userlogin = document.getElementById('user-id').value;
        if (userid != userlogin) {
            gantipenilai(userid);
            document.getElementById("simpan-assessment").style.visibility = "hidden";
            document.getElementById("calculate").style.visibility = "hidden";
        }
        else {
            tampilpenilai();
        }
        $.ajax({
            method: "GET",
            url: "Api/NilaiVendor/GetDataValue?Id=" + pengadaanid + "&UserIdAssessment=" + userid + "&VendorId=" + VendorId
        }).done(function (data) {
            rendervalue(data);
        });
    }

    function rendervalue(data) {
        //alert("masuk rendervalue");
        $(this).find('.comquestion').remove();
        $(this).find('.tampilnilai').remove();
        $(this).find('.label-kosong').remove();
        $(".comquestion").remove();
        $(".tampilnilai").remove();
        $(".label-kosong").remove();
        $('.comval').remove();
        $(".list-question").html('');
        var userid = document.getElementById("dlpenilai").value;
        var userlogin = document.getElementById('user-id').value;
        var html = '';
        if (userid == userlogin) {
            rendercekdata(data);
        }
        else if (data[0] == null) {
            html = html + '<p class="label-kosong" style="color:red;">Belum Melakukan Penilaian</p>';
            $(".warning").append(html);
            $(".comquestion").remove();
            $(".tampilnilai").remove();
            $(".total").remove();
            $(".avg").remove();
        }
        else {
            for (var i in data) {
                html = html + '<div class="col-md-7 comquestion" style="margin-bottom:5px;">';
                html = html + '<input type="text" class="form-control question" id="' + data[i].Code + '" value="' + data[i].LocalizedName + '" disabled>';
                html = html + '</div>';

                html = html + '<div class="col-md-2 comval" style="margin-bottom:5px;">';
                html = html + '<input type="number" class="form-control question-bobot" id="bobot-' + data[i].Code + '" value="' + data[i].Bobot + '" disabled>';
                html = html + '</div>';

                html = html + '<div class="col-md-1 comquestion" style="margin-bottom:5px;">';
                html = html + '<input type="text" class="form-control" value="%" disabled>';
                html = html + '</div>';

                html = html + '<div class="col-md-2 tampilnilai" style="margin-bottom:5px;">';
                html = html + '<input type="text" class="form-control quest" value="' + data[i].Score + '" disabled>';
                html = html + '</div>';
            }
            $(".list-question").append(html);
            var htmlv = '';
            htmlv = htmlv + '<p class="label-kosong" style="color:blue;">User Telah Melakukan Penilaian</p>';
            $(".warning").append(htmlv);
            calculate();
        }
    }


    function renderpenilai(data) {
        $(".nama-penilai").val(data.DisplayName);
        $(".nama-penilai").attr("disabled", "disabled");
        $(".divisi").val(data.Position);
        $(".divisi").attr("disabled", "disabled");
    }

    function rendergantipenilai(data) {
        $(".nama-penilai").val(data.DisplayName);
        $(".nama-penilai").attr("disabled", "disabled");
        $(".divisi").val(data.Position);
        $(".divisi").attr("disabled", "disabled");
    }

    function renderquestion(data) {
        //alert("masuk renderquestion");
        $('.comquestion').remove();
        $('.comval').remove();
        $('.label-kosong').remove();
        $('.quest').remove();
        var html = '';
        for (var i in data) {
            html = html + '<div class="col-md-7 comquestion" style="margin-bottom:5px;">';
            html = html + '<input type="text" class="form-control question" id="' + data[i].Code + '" value="' + data[i].LocalizedName + '" disabled>';
            html = html + '</div>';

            html = html + '<div class="col-md-2 comval" style="margin-bottom:5px;">';
            html = html + '<input type="number" class="form-control question-bobot" id="bobot-' + data[i].Code + '" value="' + data[i].Bobot + '" disabled>';
            html = html + '</div>';

            html = html + '<div class="col-md-1 comquestion" style="margin-bottom:5px;">';
            html = html + '<input type="text" class="form-control" value="%" disabled>';
            html = html + '</div>';

            html = html + '<div class="col-md-2 comval" style="margin-bottom:5px;">';
            html = html + '<select class="form-control quest">';
            html = html + '<option value="0">--Pilih--';
            for (var a = 1; a <= 5; a++) {
                html = html + '<option value="' + a + ',' + data[i].TenderScoringDetailId + '">' + a + '';
            }
            html = html + '</select>';
            html = html + '</div>';
        }
        $(".list-question").append(html);
        $(".total").remove();
        $(".avg").remove();
        document.getElementById("simpan-assessment").style.visibility = "visible";
        document.getElementById("calculate").style.visibility = "visible";
    }

    function rendervendor(data) {
        $('.vendor').remove();
        var html = '';
        html = html + '<select class="form-control vendor" id="vendor" disabled>'
        for (var i in data) {
            html = html + '<option value="' + data[i].Id + '">' + data[i].Nama + '';
        }
        html = html + '</select>';
        $(".dropdown-vendor").append(html);

        var VendorId = document.getElementById("vendor").value;
        //$.ajax({
        //    method: "POST",
        //    url: "Api/PengadaanE/CekDataAssessment?Id=" + pengadaanid + "&VendorId=" + VendorId
        //}).done(function (data) {
        //    //rendercekdata(data);
        //});

        //var userid = document.getElementById("dlpenilai").value;
        //$.ajax({
        //    method: "GET",
        //    url: "Api/NilaiVendor/GetDataValue?Id=" + pengadaanid + "&UserIdAssessment=" + userid + "&VendorId=" + VendorId
        //}).done(function (data) {
        //    rendercekdata(data);
        //});
    }

    function calculate() {
        $(".total").remove();
        $(".avg").remove();
        var quest = {};
        var dd = {};
        var bbt = {};
        // Mencari Nilai Total
        dd.nilai = document.getElementsByClassName('quest');
        bbt.bobot = document.getElementsByClassName('question-bobot');

        var xyz = {};
        xyz.bobot = [];
        xyz.bobotutuh = [];
        quest.score = [];
        quest.total = 0;
        for (var i = 0; i < dd.nilai.length; i++) {
            quest.score[i] = dd.nilai[i].value;
            var a = parseFloat(quest.score[i]);
            var b = parseFloat(quest.total);
            //bobot di bagi 100
            xyz.bobot[i] = bbt.bobot[i].value;
            var x = parseFloat(xyz.bobot[i]);
            var y = x / 100;
            var z = a * y;

            var c = z + b;
            quest.total = c;
        }

        // Mencari Nilai Average
        quest.average = quest.total / dd.nilai.length;

        var html = '';
        html = html + '<p class="total">' + quest.total + '</p>';
        $(".total-score").append(html);
        var avhtml = '';

        avhtml = avhtml + '<p class="avg">' + quest.average + '</p>';
        $(".average-score").append(avhtml);
    }

    function rendercekdata(data) {
        //alert("masuk rendercekdata");
        $('.comquestion').remove();
        $('.tampilnilai').remove();
        console.log(data[0]);
        if (data[0] != null) {
            var html = '';
            for (var i in data) {
                html = html + '<div class="col-md-7 comquestion" style="margin-bottom:5px;">';
                html = html + '<input type="text" class="form-control question" id="' + data[i].Code + '" value="' + data[i].LocalizedName + '" disabled>';
                html = html + '</div>';
                html = html + '<div class="col-md-2 tampilnilai" style="margin-bottom:5px;">';
                html = html + '<input type="number" class="form-control question-bobot" id="' + data[i].Code + '" value="' + data[i].Bobot + '" disabled>';
                html = html + '</div>';
                html = html + '<div class="col-md-1 comquestion" style="margin-bottom:5px;">';
                html = html + '<input type="text" class="form-control" value="%" disabled>';
                html = html + '</div>';
                html = html + '<div class="col-md-2 tampilnilai" style="margin-bottom:5px;">';
                html = html + '<input type="text" class="form-control quest" value="' + data[i].Score + '" disabled>';
                html = html + '</div>';
            }
            $(".list-question").append(html);
            var htmlv = '';
            htmlv = htmlv + '<p class="label-kosong" style="color:blue;">Anda Telah Melakukan Penilaian</p>';
            $(".warning").append(htmlv);
            document.getElementById("simpan-assessment").style.visibility = "hidden";
            document.getElementById("calculate").style.visibility = "hidden";
            calculate();
        };
        if (data[0] == null) {
            tampilquestion();
        };
        //else {
        //    tampilquestion();
        //}
    }

    function tampiljudul() {
        $.ajax({
            method: "POST",
            url: "Api/PengadaanE/TampilJudul?Id=" + pengadaanid
        }).done(function (data) {
            renderjudul(data);
        });
    }

    function tampilpertanyaan() {
        $.ajax({
            method: "POST",
            url: "Api/NilaiVendor/TampilPertanyaan"
        }).done(function (data) {
            renderdata(data);
        });
    }

    function tampilpemenang() {
        $.ajax({
            method: "POST",
            //url: "Api/PengadaanE/GetKandidats?Pid=" + pengadaanid
            url: "Api/NilaiVendor/detailSPKNilaiVendor?Id=" + gup("Id"),
        }).done(function (data) {
            renderdatapemenang(data);
        });
    }

    function renderjudul(data) {
        $(".nama-pengadaan").val(data.Judul);
        $(".nama-pengadaan").attr("disabled", "disabled");
    }

    function renderdata(data) {
        $('.hasil-pertanyaan').remove();
        $('.bobot-nilai').remove();
        var html = '';
        html = html + '<select class="form-control hasil-pertanyaan" id="hasil-pertanyaan" style="max-width: 85%;">'
        for (var i in data) {
            html = html + '<option value="' + data[i].Code + '">' + data[i].LocalizedName + '';
        }
        html = html + '</select><span><input type="number" class="form-control bobot-nilai" value="0" max="100" min="0" style="max-width: 15%;"></span>';
        $(".entry-pertanyaan").append(html);

        
    }

    function renderdatapemenang(data) {
        $('.pemenang').remove();
        var html = '';
        html = html + '<select class="form-control pemenang" id="pemenang" disabled>'
        html = html + '<option value="' + data.VendorId + '">' + data.PemenangPengadaan + '';
        html = html + '</select>';
        $(".entry-pemenang").append(html);
    }

    $(document).on('click', '.btn-add-pertanyaan', function (e) {
        e.preventDefault();
        var controlForm = $('.controls-pertanyaan form:first'),
            currentEntry = $(this).parents('.entry-pertanyaan:first'),
            newEntry = $(currentEntry.clone()).appendTo(controlForm);

        newEntry.find('input').val('');
        controlForm.find('.entry-pertanyaan:not(:last) .btn-add-pertanyaan')
            .removeClass('btn-add-pertanyaan').addClass('btn btn-remove-pertanyaan')
            .removeClass('btn-success').addClass('btn-danger')
            .html('<span class="fa fa-trash-o"></span>');
    }).on('click', '.btn-remove-pertanyaan', function (e) {
        $(this).parents('.entry-pertanyaan:first').remove();

        e.preventDefault();
        return false;
    });

    $(document).on('click', '.btn-remove-person-penilai', function (e) {
        $(this).parents('.person-penilai:first').remove();
        e.preventDefault();
        return false;
    });

    //$(document).on('click', '.btn-add-pemenang', function (e) {
    //    e.preventDefault();
    //    var controlForm = $('.controls-pemenang form:first'),
    //        currentEntry = $(this).parents('.entry-pemenang:first'),
    //        newEntry = $(currentEntry.clone()).appendTo(controlForm);
    //
    //    newEntry.find('input').val('');
    //    controlForm.find('.entry-pemenang:not(:last) .btn-add-pemenang')
    //        .removeClass('btn-add-pemenang').addClass('btn btn-remove-pemenang')
    //        .removeClass('btn-success').addClass('btn-danger')
    //        .html('<span class="fa fa-trash-o"></span>');
    //}).on('click', '.btn-remove-pemenang', function (e) {
    //    $(this).parents('.entry-pemenang:first').remove();
    //
    //    e.preventDefault();
    //    return false;
    //});

    $(".dateJadwal").datetimepicker({
        format: "DD MMMM YYYY HH:mm",
        locale: 'id',
        useCurrent: false,
        minDate: Date.now()

    });

    $("#simpan").on("click", function () {
        var data = {};
        var cc = {};
        
        
        cc.TenderScoringDetails = document.getElementsByClassName('hasil-pertanyaan');
        data.TenderScoringDetails = [];
        for (var i = 0; i < cc.TenderScoringDetails.length; i++) {
            data.TenderScoringDetails[i] = {};
            data.TenderScoringDetails[i].Code = cc.TenderScoringDetails[i].value;
        }
        cc.VendorId = document.getElementsByClassName('pemenang');
        data.VendorId = [];
        for (var i = 0; i < cc.VendorId.length; i++) {
            data.VendorId[i] = {};
            data.VendorId[i].Id = cc.VendorId[i].value;
        }

        //data.PengadaanId = window.location.hash.replace("#", "");
        data.PengadaanId = pengadaanid;
        cc.TenderScoringBobot = document.getElementsByClassName('bobot-nilai');
        data.TenderScoringBobot = [];
        for (var i = 0; i < cc.TenderScoringBobot.length; i++) {
            data.TenderScoringBobot[i] = {};
            data.TenderScoringBobot[i].Code = cc.TenderScoringDetails[i].value;
            data.TenderScoringBobot[i].Bobot = cc.TenderScoringBobot[i].value;
        }

        cc.TenderScoringPenilais = document.getElementsByClassName('hasil-person');
        data.TenderScoringPenilais = [];
        for (var i = 0; i < cc.TenderScoringPenilais.length; i++) {
            data.TenderScoringPenilais[i] = {};
            data.TenderScoringPenilais[i].PengadaanId = pengadaanid;
            data.TenderScoringPenilais[i].VendorId = cc.VendorId.value;
            data.TenderScoringPenilais[i].UserId = cc.TenderScoringPenilais[i].value;
            //data.TenderScoringPenilais[i].nama = cc.TenderScoringPenilais.find('option:selected').text();
            //data.TenderScoringPenilais[i].userId = cc.TenderScoringPenilais.id.value;
        }

        //pengecekan total bobot
        var n = 0;
        for (var i = 0; i < cc.TenderScoringBobot.length; i++) {
            var o = parseInt(data.TenderScoringBobot[i].Bobot);
            n = n + o;
        }

        //pengecekan point penilaian
        var j = 0;//tanda adanya kembar
        for (var l = 0; l < cc.TenderScoringDetails.length; l++) {
            var k = data.TenderScoringDetails[l].Code;
            var m = l;
            for (var t = 0; t < cc.TenderScoringDetails.length; t++) {
                if (k == data.TenderScoringDetails[t].Code) {
                    if (j == 0) {
                        //pengecekan jika terjadi kembar pada sequence yg sama
                        if (m == t) {
                            j = 0;
                        } else {
                            j = 1;
                        }
                    } else {
                        j = 1;
                    }
                }
            }
        }

        //pengecekan vendor
        var u = 0;//tanda kembar di sequence yg sama
        for (var i = 0; i < cc.VendorId.length; i++) {
            var v = data.VendorId[i].Id;
            var w = i;
            for (var l = 0; l < cc.VendorId.length; l++) {
                if (v == data.VendorId[l].Id) {
                    if (u == 0) {
                        //pengecekan jika terjadi kembar pada sequence yg sama
                        if (w == l) {
                            u = 0;
                        } else {
                            u = 1;
                        }
                    } else {
                        u = 1;
                    }
                }
            }
        }


        if (u == 1) {
            //alert("Vendor kembar, u : " + u);
            BootstrapDialog.alert("Vendor terpilih harus berbeda.")
        } else {
            //alert("Vendor tidak kembar, u : " + u);
            if (j == 1) {
                //alert("pertanyaan kembar, j : " + j);
                alert("Point penilaian harus berbeda");
            } else {
                //alert("pertanyaan sesuai, j : " + j);
                if (n != 100) {
                    alert("Tidak bisa disimpan! Total bobot baru = " + n + ", Total bobot harus sama dengan 100.");
                } else {
                    //alert("n : " + n);
                    //console.log(data);
                    waitingDialog.showloading("Proses Harap Tunggu");
                    //alert("masuk loading");
                    //$.ajax({
                    //    url: "Api/PengadaanE/SaveCreatePertanyaan",
                    //    method: "POST",
                    //    dataType: "json",
                    //    data: JSON.stringify(data),
                    //    contentType: 'application/json; charset=utf-8',
                    //    success: function (d) {
                    //        waitingDialog.hideloading();
                    //        alert("Data Berhasil Disimpan");
                    //        window.location.reload();
                    //    },
                    //    error: function (errormessage) {
                    //
                    //    }
                    //});

                    $.ajax({
                        url: "Api/NilaiVendor/SaveCreatePertanyaan",
                        method: "POST",
                        dataType: "json",
                        data: JSON.stringify(data),
                        contentType: 'application/json; charset=utf-8',
                        success: function (d) {
                            waitingDialog.hideloading();
                            alert("Data Berhasil Disimpan");
                            window.location.reload();
                        },
                        error: function (errormessage) {

                        }
                    });
                }
            }
        }




    });

    $("#simpan-assessment").on("click", function () {
        console.log("uhuy");
        var data = {};
        var cc = {};
        var bbt = {};
        var ddd = {};
        cc.Ques = document.getElementsByClassName('quest');
        bbt.bobot = document.getElementsByClassName('question-bobot');
        ddd.Code = document.getElementsByClassName('question');
        data.TenderScoringUser = [];

        for (var i = 0; i < cc.Ques.length; i++) {
            data.TenderScoringUser[i] = {};
            var a = cc.Ques[i].value;
            var b = a.split(",");
            data.TenderScoringUser[i].Score = b[0];
            data.TenderScoringUser[i].VWTenderScoringDetailId = b[1];
            data.TenderScoringUser[i].Bobot = bbt.bobot[i].value;
            data.TenderScoringUser[i].Code = ddd.Code[i].getAttribute('id');
        }
        console.log(data);
        //var Id = window.location.hash.replace("#", "");
        var Id = pengadaanid;
        var VendorId = document.getElementById('vendor').value;

        var quest = {};
        var dd = {};
        // Mencari Nilai Total
        dd.nilai = document.getElementsByClassName('quest');

        var xyz = {};
        xyz.bobot = [];
        xyz.bobotutuh = [];
        quest.score = [];
        quest.total = 0;
        for (var i = 0; i < dd.nilai.length; i++) {
            quest.score[i] = dd.nilai[i].value;
            var a = parseFloat(quest.score[i]);
            var b = parseFloat(quest.total);

            //bobot di bagi 100
            xyz.bobot[i] = bbt.bobot[i].value;

            var x = parseFloat(xyz.bobot[i]);
            var y = x / 100;
            var z = a * y;

            var c = z + b;
            quest.total = c;
        }

        // Mencari Nilai Average
        quest.average = quest.total / dd.nilai.length;

        console.log("data : " + JSON.stringify(data));
        console.log("VendorId : " + VendorId);
        console.log("Total : " + quest.total);
        console.log("Average : " + quest.average);

        $.ajax({
            url: "Api/NilaiVendor/SaveCreateAssessment?Id=" + Id + "&VendorId=" + VendorId + "&Total=" + quest.total,
            method: "POST",
            dataType: "json",
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            success: function (d) {
                alert("Data Berhasil Disimpan");
                window.location.reload();
            },
            error: function (errormessage) {

            }
        });
    });


});