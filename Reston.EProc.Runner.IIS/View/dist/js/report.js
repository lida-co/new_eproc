
$(function () {

    $(".date").datetimepicker({
        format: "DD MMMM YYYY",
        locale: 'id'
    }).on('changeDate', function (ev) {
    });

    $("#report").on("click", function () {
        var dari = moment($("#report_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var divisi = $("#DivisiPengadaan").val();
        var status = $("#Tahapan").val();

        //alert ('tahapan')

        if (divisi == '0' && status == '0') {
            downloadFileUsingForm("/api/report/ReportPengadaan2?dari=" + dari + "&sampai=" + sampai);
        }
        else if (divisi == '0' && status != '0'){
            downloadFileUsingForm("/api/report/ReportPengadaan3?dari=" + dari + "&sampai=" + sampai + "&status=" + status);
        }
        else if (divisi != '0' && status == '0') {
            downloadFileUsingForm("/api/report/ReportPengadaan?dari=" + dari + "&sampai=" + sampai + "&divisi=" + divisi);
        }
        else {
            downloadFileUsingForm("/api/report/ReportPengadaan4?dari=" + dari + "&sampai=" + sampai + "&divisi=" + divisi + "&status=" + status);
        }
        
    });

    $("#report_pks").on("click", function () {
        var dari = moment($("#report_pks_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report_pks_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");

        downloadFileUsingForm("/api/PKS/ReportPKS?dari=" + dari + "&sampai=" + sampai);
    });

    $("#report_spk").on("click", function () {
        var dari = moment($("#report_spk_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report_spk_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var divisi = $("#DivisiSPK").val();

        if ($("#DivisiSPK").val() == '') {
            downloadFileUsingForm("/api/SPK/ReportSPK2?dari=" + dari + "&sampai=" + sampai);
        }
        else {
            downloadFileUsingForm("/api/SPK/ReportSPK?dari=" + dari + "&sampai=" + sampai + "&divisi=" + divisi);
        }
    });

    $("#report_po").on("click", function () {
        var dari = moment($("#report_po_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report_po_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var searchby = $("#SearchBy").val();

        if ($("#SearchBy").val() == 'Tanggal PO') {
            downloadFileUsingForm("/api/PO/ReportPO?dari=" + dari + "&sampai=" + sampai);
        }
        else if ($("#SearchBy").val() == 'Tanggal DO') {
            downloadFileUsingForm("/api/PO/ReportDO?dari=" + dari + "&sampai=" + sampai);
        }
        else if ($("#SearchBy").val() == 'Tanggal Invoice') {
            downloadFileUsingForm("/api/PO/ReportInvoice?dari=" + dari + "&sampai=" + sampai);
        }
        else if ($("#SearchBy").val() == 'Tanggal Finance') {
            downloadFileUsingForm("/api/PO/ReportFinance?dari=" + dari + "&sampai=" + sampai);
        }
    });

    $("#report_monitoring").on("click", function () {
        var dari = moment($("#report_monitoring_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report_monitoring_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");

        downloadFileUsingForm("/api/MonitoringSelection/ReportMonitoring?dari=" + dari + "&sampai=" + sampai);
    });
    $("#report-vendor").on("click", function () {
        downloadFileUsingForm("/api/Report/CetakVendor");
    });
    $("#report-penilaian-vendor").on("click", function () {
        
        var dari = moment($("#report-penilaian-vendor_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report-penilaian-vendor_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");

        downloadFileUsingForm("/api/Report/CetakPenilaianVendor?dari=" + dari + "&sampai=" + sampai);
    });

    $("#report_spk_non_pks").on("click", function () {
        var dari = moment($("#report_spk_non_pks_dari").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var sampai = moment($("#report_spk_non_pks_sampai").val(), ["D MMMM YYYY"], "id").format("DD/MM/YYYY");
        var divisi = $("#DivisiSPKNONPKS").val();

        if ($("#DivisiSPKNONPKS").val() == '') {
            downloadFileUsingForm("/api/SPK/ReportSPKNONPKS2?dari=" + dari + "&sampai=" + sampai);
        }
        else {
            downloadFileUsingForm("/api/SPK/ReportSPKNONPKS?dari=" + dari + "&sampai=" + sampai + "&divisi=" + divisi);
        }
    });
    
});

function downloadFileUsingForm(url) {
	url = DOMPurify.sanitize(url);
    var form = document.createElement("form");
    form.method = "post";
    form.action = url;
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}

function SetListDivisiPengadaan() {
    $.ajax({
        url: "/api/ReferenceData/GetAllUnitKerja",
        success: function (data) {
		//data = DOMPurify.sanitize(data);
		if(data){		
            for (var i in data) {
                $("#DivisiPengadaan").append("<option value='" + DOMPurify.sanitize(data[i].Name) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
            }
		}
        },
        complete: function (xhr, textStatus) {
            ajaxCompleteProcess(xhr);
        }
    });
}

function SetListDivisiSPK() {
    $.ajax({
        url: "/api/ReferenceData/GetAllUnitKerja",
        success: function (data) {
			data = DOMPurify.sanitize(data);
			if(data){				
				for (var i in data) {
                    $("#DivisiSPK").append("<option value='" + DOMPurify.sanitize(data[i].Name) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
				}
			}
        },
        complete: function (xhr, textStatus) {
            ajaxCompleteProcess(xhr);
        }
    });
}


$(function () {
    SetListDivisiPengadaan();
    SetListDivisiSPK();
});

