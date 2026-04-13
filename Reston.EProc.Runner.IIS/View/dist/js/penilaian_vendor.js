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
        method: "post",
        url: "api/PenilaianVendor/CekSudahDiNilai?Id=" + gup("Id"),
        success: function (d) {
            //if (d.IsSudahDiNilai ==1 ) {
            //    $("#simpan-nilai-konfirmasi").remove();
            //    $("#simpan-nilai-konfirmasi").attr("disabled","disabled");
            //}
            if (d.isSudahDinilaiDariNoSPK == 1) {
                $("#simpan-nilai-konfirmasi").hide();
                $("#simpan-nilai-konfirmasi").remove();
                $("#simpan-nilai-konfirmasi").attr("disabled", "disabled");
                //alert("masuk if");
                url = 'api/PenilaianVendor/TampilPenilaian?Id=' + gup("Id");
                url2 = "api/PenilaianVendor/TampilNilai?Id=" + gup("Id");
                tampil_table_nilai(url);
                Kriteria(url2);
            }

            else if (d.isSudahDinilaiDariProyID == 1) {
                $("#simpan-nilai-konfirmasi").hide();
                $("#simpan-nilai-konfirmasi").remove();
                $("#simpan-nilai-konfirmasi").attr("disabled", "disabled");


                url = 'api/Proyek/TampilPenilaian?Id=' + d.proyID;
                url2 = 'api/Proyek/TampilNilai?Id=' + d.proyID;
                tampil_table_nilai(url);
                Kriteria(url2);
            } else {
                url = 'api/PenilaianVendor/TampilPenilaian?Id=' + gup("Id");
                url2 = "api/PenilaianVendor/TampilNilai?Id=" + gup("Id");
                tampil_table_nilai(url);
                Kriteria(url2);
            }
        },
    })

    $.ajax({
        method: "post",
        url: "api/PenilaianVendor/NgeheTampilJudulDetail?Id=" + gup("Id"),
        success: function (d) {
            $("#NamaProyek").text(d.Judul);
            $("#vendorID").val(d.vendor_Id);
            $("#SpkID").val(d.Spk_Id);
            Kriteria(url2);
        },
    });

    function tampil_table_nilai(url) {
        var table_nilai;
        var ID = gup("Id");
        table_nilai = $('#nilai').DataTable({
            "paging": true,
            "lengthChange": false,
            "searching": false,
            "ordering": false,
            "info": false,
            "autoWidth": true,
            "ajax": {
                //"url": 'api/PenilaianVendor/TampilPenilaian?Id=' + ID,
                "url": url,
                "type": 'POST',
            },
            "columns": [
                    { "data": "NamaPenilaian" },
                    { "data": "Nilai" },
                    { "data": "Catatan_item" },
            ],
            "columnDefs": [
                    {
                        "render": function (data, type, row) {
                            var html = '<textarea class="form-group col-md-12 catatan_item" rows="3"> ' + row.Catatan_item + '</textarea>';
                            return html;
                        },
                        "targets": 2,
                        "orderable": false
                    },
                    {
                        "render": function (data, type, row) {
                            if (row.Nilai == 0) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00" selected> Tidak Ada Pemilihan Kriteria Ini</option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 1) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pemilihan Kriteria Ini</option>';
                                html = html + '<option value="01" selected> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 2) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02" selected> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 3) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03" selected> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 4) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04" selected> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 5) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05" selected> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 6) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06" selected> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 7) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07" selected> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 8) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08" selected> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 9) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pilihan </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09" selected> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else if (row.Nilai == 10) {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value="00"> Tidak Ada Pemilihan Kriteria Ini </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10" selected> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            else {
                                var html = '<div class="form-group col-md-8">';
                                html = html + '<select class="form-control hasil_penilaian" style="width:100%">';
                                html = html + '<option value=""> Pilih </option>';
                                html = html + '<option value="00"> Tidak Ada Pemilihan Kriteria Ini </option>';
                                html = html + '<option value="01"> 01 </option>';
                                html = html + '<option value="02"> 02 </option>';
                                html = html + '<option value="03"> 03 </option>';
                                html = html + '<option value="04"> 04 </option>';
                                html = html + '<option value="05"> 05 </option>';
                                html = html + '<option value="06"> 06 </option>';
                                html = html + '<option value="07"> 07 </option>';
                                html = html + '<option value="08"> 08 </option>';
                                html = html + '<option value="09"> 09 </option>';
                                html = html + '<option value="10"> 10 </option>';
                                html = html + '</select>';
                                html = html + '</div>';
                                html = html + '<input type="hidden" class="referencedataid" value="' + row.Id + '">';
                                html = html + '<input type="hidden" class="vendorid" value="' + row.VendorId + '">';
                            }
                            return html;
                        },
                        "targets": 1,
                        "orderable": false
                    },
            ],
        });
    }

    // end datatable




    $("#simpan-nilai-konfirmasi").on("click", function () {
        var Catatan = $(".catatan").val();
        if (Catatan == "") {
            $("#konfirmasi-data").show();
        }
        else {
            $("#modal-nilai").show();
        }
    });

    $("#batal").on("click", function () {
        $("#modal-nilai").hide();
    });

    $("#OK").on("click", function () {
        $("#konfirmasi-data").hide();
    });

    // save DataTable
    $("#simpan-nilai").on("click", function () {
        $("#modal-nilai").hide();
        var Catatan = "";
        var objPenilaianHeader = {};
        var arrobjPenilaian = [];
        var VendorId;
        var Jumlah_penilaian = $('#nilai tbody tr').length;
        var Total_nilai = 0;
        $("#nilai tbody tr").each(function () {
            var objPenilaian = {};
            objPenilaian.Spk_Id = $("#SpkID").val();
            objPenilaian.VendorId = $("#vendor_id").val();
            objPenilaian.referencedataid = $(this).find(".referencedataid").val();
            objPenilaian.Nilai = $(this).find(".hasil_penilaian").val();
            objPenilaian.Catatan_item = $(this).find(".catatan_item").val();
            VendorId = $(this).find("#vendor_id").val();
            arrobjPenilaian.push(objPenilaian);
            Total_nilai += parseInt($(this).find('.hasil_penilaian').val());
        });
        objPenilaianHeader.Spk_Id = $("#SpkID").val();
        objPenilaianHeader.VendorId = VendorId;
        objPenilaianHeader.Catatan = $(".catatan").val();
        objPenilaianHeader.Jumlah_penilaian = Jumlah_penilaian;
        objPenilaianHeader.Total_nilai = Total_nilai;
        objPenilaianHeader.PenilaianVendorDetails = arrobjPenilaian;
        $.ajax({
            method: "POST",
            url: "Api/PenilaianVendor/SimpanNilai",
            data: objPenilaianHeader,
            success: function (data) {
                BootstrapDialog.show({
                    title: 'Konfirmasi',
                    message: 'Data Berhasil di Simpan',
                    buttons: [{
                        label: 'Close',
                        action: function (dialog) {
                            dialog.close();
                        }
                    }]
                });
                table_nilai.ajax.reload();
                Kriteria();
            }
        });
    });

    function datatableToJson(table) {
        var data = [];
        var beforeJudul = "";
        table.rows().every(function () {
            data.push(this.data());
        });
        //console.log(JSON.stringify(data));
        return data;
    }

    function Kriteria(url2) {
        var ProyekId = gup("Id");
        var hitung;
        var to_nilai;
        var jum_penilaian;

        $.ajax({
            method: "post",
            //url: "api/PenilaianVendor/TampilNilai?Id=" + ProyekId,
            url: url2,
            success: function (d) {
                to_nilai = d.Total_nilai;
                jum_penilaian = d.Jumlah_penilaian;
                hitung = to_nilai / jum_penilaian;


                $(".catatan").val(d.Catatan);
                $("#total_nilai").text(d.Total_nilai);
                $("#jumlah_penilaian").text(parseFloat(hitung).toFixed(2));
                if (hitung > 8.5 && hitung <= 10) {
                    $("#kpi").text("EXCELLENT");
                }
                else if (hitung <= 8.5 && hitung > 7) {
                    $("#kpi").text("GOOD");
                }
                else if (hitung <= 7 && hitung > 6) {
                    $("#kpi").text("FAIR");
                }
                else if (hitung <= 6 && hitung > 4) {
                    $("#kpi").text("BAD");
                }
                else if (hitung <= 4) {
                    $("#kpi").text("VERY BAD");
                }
            }
        })
    }


});