var IDSRV = '';
var PROC = window.location.origin + '/';


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

// Ensure global csrfToken getter/setter is set up once
if (!Object.getOwnPropertyDescriptor(window, 'csrfToken')) {
    let _csrfToken = "";
    Object.defineProperty(window, 'csrfToken', {
        get: function () { return _csrfToken; },
        set: function (val) { _csrfToken = val; },
        configurable: true,
        enumerable: true
    });
}

// Ensure global csrfPromise and initCsrf are set up once
window.csrfRetryCount = window.csrfRetryCount || 0;
const MAX_RETRIES = 3;

if (!window.csrfPromise) {
    let csrfResolver = null;
    let csrfRejecter = null;
    window.csrfPromise = new Promise((resolve, reject) => {
        csrfResolver = resolve;
        csrfRejecter = reject;
    });

    window.initCsrf = function () {
        fetch('/api/security/GetCsrfToken')
            .then(res => {
                if (!res.ok) throw new Error(`HTTP ${res.status}: ${res.statusText}`);
                return res.json();
            })
            .then(data => {
                if (!data.csrfToken) throw new Error('CSRF token tidak ditemukan dalam response');
                csrfToken = data.csrfToken; // updates the getter/setter
                window.csrfRetryCount = 0;
                csrfResolver(csrfToken);
            })
            .catch(e => {
                console.error("Gagal mengambil CSRF token:", e.message);
                window.csrfRetryCount++;
                if (window.csrfRetryCount <= MAX_RETRIES) {
                    setTimeout(window.initCsrf, 5000);
                } else {
                    console.error('Gagal mengambil CSRF token setelah 3 kali percobaan');
                    csrfRejecter(e);
                }
            });
        return window.csrfPromise;
    };

    // Mulai load CSRF token segera secara asinkron
    window.initCsrf();
}

// Keep initCsrf alias for compatibility
var initCsrf = window.initCsrf;

// ✅ Auto-init CSRF token saat DOM ready - berlaku untuk SEMUA halaman
$(document).ready(async function () {
    if (window.csrfPromise) {
        try {
            await window.csrfPromise;
        } catch (e) {
            console.warn("Melanjutkan cekLogin meskipun CSRF token gagal diambil.");
        }
    }
    cekLogin(0);
});

// Setup jQuery AJAX interception & setup
if (typeof $ !== 'undefined') {
    $.ajaxSetup({
        beforeSend: function (xhr, settings) {
            // Skip untuk GET, HEAD, OPTIONS
            var method = (settings.type || settings.method || '').toUpperCase();
            if (method === 'GET' || method === 'HEAD' || method === 'OPTIONS') return;

            if (!csrfToken) {
                try {
                    var req = new XMLHttpRequest();
                    req.open('GET', '/api/security/GetCsrfToken', false); // synchronous request
                    req.send(null);
                    if (req.status === 200) {
                        var data = JSON.parse(req.responseText);
                        csrfToken = data.csrfToken;
                    }
                } catch (e) {
                    console.warn('Gagal sinkron CSRF token:', e);
                }
            }

            if (csrfToken) {
                xhr.setRequestHeader("X-CSRF-TOKEN", csrfToken);
                xhr.setRequestHeader("X-XSRF-TOKEN", csrfToken);
                xhr.setRequestHeader("RequestVerificationToken", csrfToken);
            } else {
                console.warn('CSRF token tetap belum tersedia');
            }
        }
    });

    // Override $.ajax untuk menunda request non-GET sampai csrfPromise selesai
    if (!$.ajax._isIntercepted) {
        const originalAjax = $.ajax;
        $.ajax = function (options) {
            var settings = $.extend(true, {}, $.ajaxSettings, options);
            var method = (settings.type || settings.method || 'GET').toUpperCase();

            // Skip intersept untuk GET, HEAD, OPTIONS
            if (method === 'GET' || method === 'HEAD' || method === 'OPTIONS') {
                return originalAjax.apply(this, arguments);
            }

            // Jika token sudah siap, langsung jalankan
            if (csrfToken) {
                return originalAjax.apply(this, arguments);
            }

            // Jika belum siap, tunda hingga csrfPromise selesai
            var self = this;
            var args = arguments;
            var deferred = $.Deferred();

            window.csrfPromise.then(function () {
                originalAjax.apply(self, args).done(deferred.resolve).fail(deferred.reject).progress(deferred.notify);
            }).catch(function () {
                // Fallback: jalankan saja request-nya jika gagal setelah semua retry
                originalAjax.apply(self, args).done(deferred.resolve).fail(deferred.reject).progress(deferred.notify);
            });

            return deferred.promise();
        };
        $.ajax._isIntercepted = true;
    }
}

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