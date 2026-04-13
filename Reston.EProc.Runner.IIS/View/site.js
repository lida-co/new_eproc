var IDSRV = '';
var PROC = 'http://localhost:44392/';


var UNAUTHORIZED_CODE = '403';
var SUCCESS_CODE = '200';
var ERROR_CODE = '0';
var LOGIN_PAGE;// = 'http://localhost:7348/';
var HOME_PAGE;
var ADMIN;// = 'http://localhost:44392/master.html';
var MANAGER;// = 'http://localhost:44392/pengadaan-list.html';
var STAFF;// = 'http://localhost:44392/dashboard.html';//pengadaan list
var REKANAN;// = 'http://localhost:44392/rekanan-side-terdaftar.html';
var ENDUSER;// = 'http://localhost:44392/dashboard.html';//pengadaan list
var COMPLIANCE;// = 'http://localhost:44392/pengadaan-list.html';
var HEAD;// = 'http://localhost:44392/pengadaan-list.html';
var DIREKSI;// = 'http://localhost:44392/dashboard.html';
var DIRUT;// = 'http://localhost:44392/dashboard.html';
var LEGAL;// = 'http://localhost:44392/dashboard.html';

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

function getParameterByName(name, url) {
    if (!url) url = DOMPurify.sanitize(window.location.href, { ALLOWED_URI_REGEXP: /^(?:(?:https?|ftp):|[^a-z]|[a-z+.\-]+(?:[^a-z+.\-:]|$))/i });

    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function ajaxCompleteProcess(xhr) {
    if (xhr.status == UNAUTHORIZED_CODE) {//Authenticated, but no access
        window.location = HOME_PAGE;
    }
    else if (xhr.status == ERROR_CODE) {//not authenticated
        window.location = LOGIN_PAGE;
    }
    else if (xhr.status == SUCCESS_CODE) {
        //$("#loading").hide();
        //$("html").show();
    }
}

function cekLandingPage() {
    $.ajax({
        url: "Api/Header/cekRole",
        method: "POST",
        complete: function (e, xhr, settings) {
            var data = e.responseJSON;
            if (data.message.indexOf("eproc_superadmin") >= 0) {
                window.location.replace(ADMIN);
            }
            else if (data.message.indexOf("procurement_staff") >= 0) {
                window.location.replace(STAFF);
            }
            else if (data.message.indexOf("procurement_manager") >= 0) {
                window.location.replace(MANAGER);
            }
            else if (data.message.indexOf("rekanan_terdaftar") >= 0) {
                window.location.replace(REKANAN);
            }
            else if (data.message.indexOf("end_user") >= 0) {
                window.location.replace(ENDUSER);
            }
            else if (data.message.indexOf("compliance") >= 0) {
                window.location.replace(COMPLIANCE);
            }
            else if (data.message.indexOf("procurement_head") >= 0) {
                window.location.replace(HEAD);
            }
            else if (data.message.indexOf("direksi") >= 0) {
                window.location.replace(DIREKSI);
            }
            else if (data.message.indexOf("dirut") >= 0) {
                window.location.replace(DIREKSI);
            }
            else if (data.message.indexOf("legal_admin") >= 0) {
                window.location.replace(LEGAL);
            }
        }

    });
}
function cekLogin(cek) {
    $.ajax({
        url: "Api/Header/cekLogin",
        method: "GET", // ✅ WAJIB GET
        complete: function (e) {

            $.ajax({
                url: "api/header/geturl",
                method: "GET", // ✅ sekalian GET juga
                success: function (da) {

                    if (da == null) {
                        ajaxCompleteProcess(ERROR_CODE);
                        return;
                    }

                    IDSRV = da.idsrv;
                    PROC = da.proc;

                    LOGIN_PAGE = IDSRV;
                    HOME_PAGE = PROC;
                    ADMIN = PROC + 'master.html';
                    MANAGER = PROC + 'dashboard.html';
                    STAFF = PROC + 'dashboard.html';
                    REKANAN = PROC + 'rekanan-side-terdaftar.html';
                    ENDUSER = PROC + 'dashboard.html';
                    COMPLIANCE = PROC + 'pengadaan-list.html';
                    HEAD = PROC + 'dashboard.html';
                    DIREKSI = PROC + 'dashboard.html';
                    DIRUT = PROC + 'dashboard.html';
                    LEGAL = PROC + 'dashboard.html';

                    ajaxCompleteProcess(e);

                    if (e.status == 200 && cek == 1) {
                        cekLandingPage();
                    }
                },
                error: function (e) {
                    ajaxCompleteProcess(e);
                }
            });
        }
    });
}

function LogOut() {
    $.ajax({
        url: "Api/Header/Signout",
        method: "POST",
        complete: function (e, xhr, settings) {
            window.location(HOME_PAGE + "/index.html");

        }

    });
}
//$(function () {
//    //await initCsrf();  // Pastikan CSRF token siap
//    cekLogin(0);  // Setup data sesi login tanpa redirect
//});