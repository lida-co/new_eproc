// Ensure global csrfToken getter/setter and promise are set up once within an IIFE
(function () {
    if (!Object.getOwnPropertyDescriptor(window, 'csrfToken')) {
        let _csrfToken = "";
        Object.defineProperty(window, 'csrfToken', {
            get: function () { return _csrfToken; },
            set: function (val) { _csrfToken = val; },
            configurable: true,
            enumerable: true
        });
    }

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
})();

// Keep initCsrf alias for compatibility
var initCsrf = window.initCsrf;

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

function UnformatFloat(value) {
    if (value != null && value!="")
        return value.toString().replace(/\./g, "");
    else value;
}


function isGuid(value) {
    var regex = /[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i;// /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
    var match = regex.exec(value);
    return match != null;
}

/**
 * Baca GUID dari URL — mendukung hash fragment (#guid) dan
 * fallback ke query string (?%23=guid) yang terjadi akibat
 * history.pushState meng-encode # menjadi %23.
 * Jika ditemukan dari query string, URL otomatis diperbaiki ke format hash.
 */
function getIdFromUrl() {
    // Coba baca dari hash fragment dulu
    var fromHash = DOMPurify.sanitize(window.location.hash).replace(/^#/, '');
    if (isGuid(fromHash)) return fromHash;

    // Fallback: baca dari query string (kasus ?%23=guid atau ?id=guid)
    try {
        var params = new URLSearchParams(window.location.search);
        var candidates = ['#', '%23', 'id', 'Id', 'pengadaanId'];
        for (var i = 0; i < candidates.length; i++) {
            var val = params.get(candidates[i]);
            if (val && isGuid(val)) {
                // Perbaiki URL ke format hash yang benar
                history.replaceState(null, '', window.location.pathname + '#' + val);
                return val;
            }
        }
    } catch (e) { }

    return '';
}

function gup(name, url) {
    if (!url) url = HOME_PAGE;
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(url);
    return results == null ? null : results[1];
}


function SetListRegion(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllRegion",
        success: function (data) {
            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Name) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
                }
            }
        }
    });
}



function downloadFileUsingForm(url) {
    var form = document.createElement("form");
    console.log(url);
    console.log(DOMPurify.sanitize(url));
    form.method = "post";
    form.action = DOMPurify.sanitize(url);
    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}

/**
 * Helper: tambahkan CSRF token ke Dropzone instance
 * Panggil ini setelah membuat Dropzone baru
 */
function addCsrfToDropzone(dropzoneInstance) {
    dropzoneInstance.on("sending", function(file, xhr, formData) {
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
            } catch(e) { console.warn('Gagal ambil CSRF token untuk Dropzone:', e); }
        }
        if (token) {
            xhr.setRequestHeader("X-CSRF-TOKEN", token);
            xhr.setRequestHeader("X-XSRF-TOKEN", token);
        }
    });
}


function SetListProvinsi(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllProvinsi",
        success: function (data) {

            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Name) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
                }
            }

        }
    });
}

function SetListBranch(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllBranch",
        success: function (data) {

            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Branch_Name) + "'>" + DOMPurify.sanitize(data[i].Branch_Name) + "</option>");
                }
            }
			
        }
    });
}

function SetListDepartment(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllDepartment",
        success: function (data) {

            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Department_Name) + "'>" + DOMPurify.sanitize(data[i].Department_Name) + "</option>");
                }
            }

        }
    });
}

function SetListDepartmentWithBranch(el, branch) {
    $.ajax({
        url: "/api/ReferenceData/GetAllDepartmentPengadaanAdd?branch=" + branch,
        success: function (data) {

            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Department_Name) + "'>" + DOMPurify.sanitize(data[i].Department_Name) + "</option>");
                }
            }
        }
    });
}

function SetListPeriode(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllPeriodeAnggaran",
        success: function (data) {

            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Code) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
                }
            }

        }
    });
}


function SetListUnitKerja(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllUnitKerja",
        success: function (data) {

            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Name) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
                }
            }

        }
    });
}


function SetListJenisPekerjaan(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllJenisPekerjaan",
        success: function (data) {


            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Name) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
                }
            }

        }
    });
}


function SetListJenisPembelanjaan(el) {
    $.ajax({
        url: "/api/ReferenceData/GetAllJenisPembelanjaan",
        success: function (data) {


            if (data.length > 0 && typeof (data) == 'object') {
                for (var i in data) {
                    $(el).append("<option value='" + DOMPurify.sanitize(data[i].Code) + "'>" + DOMPurify.sanitize(data[i].Name) + "</option>");
                }
            }

        }
    });
}

/**
 * Module for displaying "Waiting for..." dialog using Bootstrap
 *
 * @author Eugene Maslovich <ehpc@em42.ru>
 */


var waitingDialog = waitingDialog || (function ($) {
    'use strict';

    // Creating modal dialog's DOM
    var $loading = $(
		'<div class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
		'<div class="modal-dialog modal-m">' +
		'<div class="modal-content">' +
			'<div class="modal-header"><h3 style="margin:0;"></h3></div>' +
			'<div class="modal-body">' +
				'<div class="progress progress-striped active" style="margin-bottom:0;"><div class="progress-bar" style="width: 100%"></div></div>' +
			'</div>' +
		'</div></div></div>');


    var $dialogresult = $(
		'<div class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
		'<div class="modal-dialog modal-m">' +
		'<div class="modal-content">' +
			'<div class="modal-header"><h3 id="modalheader" style="margin:0;"></h3></div>' +
			'<div class="modal-body">' +
				'<h3 id="modalmessages" style="margin-bottom:0;"></h3>' +
			'</div>' +
		'</div></div></div>');



    return {
        /**
		 * Opens our dialog
		 * @param message Custom message
		 * @param options Custom options:
		 * 				  options.dialogSize - bootstrap postfix for dialog size, e.g. "sm", "m";
		 * 				  options.progressType - bootstrap postfix for progress bar type, e.g. "success", "warning".
		 */
        showloading: function (message, options) {
            // Assigning defaults
            if (typeof options === 'undefined') {
                options = {};
            }
            if (typeof message === 'undefined') {
                message = 'Loading';
            }
            var settings = $.extend({
                dialogSize: 'm',
                progressType: '',
                onHide: null // This callback runs after the dialog was hidden
            }, options);

            // Configuring dialog
            $loading.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);
            $loading.find('.progress-bar').attr('class', 'progress-bar');
            if (settings.progressType) {
                $loading.find('.progress-bar').addClass('progress-bar-' + settings.progressType);
            }
            $loading.find('h3').text(message);
            // Adding callbacks
            if (typeof settings.onHide === 'function') {
                $loading.off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
                    settings.onHide.call($loading);
                });
            }
            // Opening dialog
            $loading.modal();
        },



        showresult: function (messageheader, messagebody) {

            if (typeof messageheader === 'undefined') {
                messageheader = 'Message';
            }

            if (typeof messagebody === 'undefined') {
                messagebody = '..........';
            }


            $dialogresult.find('#modalheader').text(messageheader);
            $dialogresult.find('#modalmessages').text(messagebody);
            // Adding callbacks

            // Opening dialog
            $dialogresult.modal();
        },
        /**
		 * Closes dialog
		 */

        hideresult: function () {
            $dialogresult.modal('hide');
        },


        hideloading: function () {
            $loading.modal('hide');
        }
    };

})(jQuery);
