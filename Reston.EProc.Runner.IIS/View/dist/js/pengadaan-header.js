var id_rks = DOMPurify.sanitize(window.location.hash);

$(function () {
    if (isGuid(id_rks)) {
        $("#pengadaanId").val(id_rks);
        loadData(id_rks);
    }
    else {
        if (isGuid($("#pengadaanId").val())) {
            window.location.hash = $("#pengadaanId").val();
           
        } else {
            $(".SimpanAjukan").show();
            $(".Simpan").show();
            $(".Hapus").show();
            silensave(getHeaderPengadaan());//save dulu
        }
    }
});


