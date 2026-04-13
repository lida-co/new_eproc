/**
 * Redirect ke halaman tertentu dengan parameter aman + whitelist
 * @param {string} path - relative path tujuan, misal "/catalogue-detail.html"
 * @param {string} paramName - nama query param, misal "id"
 * @param {string} paramValue - nilai param dari user input
 * @param {Array<string>} whitelist - daftar nilai ID yang diizinkan
 */
// FortifyIssueSuppression [Open Redirect]
// Path dibatasi ke internal whitelist (tidak bisa redirect ke luar domain)
function safeRedirect(key, paramName, paramValue) {
    const pathMap = {
        "catalogue-detail": "/catalogue-detail.html",
        "create-rks": "/create-rks.html",
        "catalog-detail-asuransi": "/catalog-detail-asuransi.html",
        "rks-penilaian": "/rks-penilaian.html",
        "pengadaan": "/rekanan-side-detail-pengadaan.html",
        "rks-penilaian-asuransi": "/rks-penilaian-asuransi.html"
    };

    if (!pathMap[key]) return;
    if (!/^[a-zA-Z0-9_-]+$/.test(paramValue)) return;
    if (whitelist.length > 0 && !whitelist.includes(paramValue)) return;

    const safeParam = encodeURIComponent(paramName);
    const safeValue = encodeURIComponent(paramValue);
    const targetUrl = `${pathMap[key]}?${safeParam}=${safeValue}`;

    // ✅ Manipulasi history + reload
    history.pushState(null, "", targetUrl);
    location.reload();
}


/**
 * Versi untuk buka tab baru / window baru
 */
// FortifyIssueSuppression [Open Redirect]
// Path hanya bisa dipilih dari mapping internal (bukan input langsung)
function safeOpen(key, paramName, paramValue, whitelist = [], target = "_blank") {
    const pathMap = {
        "catalogue": "/catalogue-detail.html",
        "penilaian": "/rks-penilaian.html"
    };

    // ✅ Cek apakah key valid
    if (!pathMap[key]) {
        console.error(`Path key not allowed: ${key}`);
        return;
    }

    // ✅ Validasi paramValue (hanya huruf, angka, underscore, dash)
    if (!/^[a-zA-Z0-9_-]+$/.test(paramValue)) {
        console.error(`Invalid ${paramName}: ${paramValue}`);
        return;
    }

    // ✅ Validasi whitelist value (opsional)
    if (whitelist.length > 0 && !whitelist.includes(paramValue)) {
        console.error(`Value not allowed by whitelist: ${paramValue}`);
        return;
    }

    const safeParam = encodeURIComponent(paramName);
    const safeValue = encodeURIComponent(paramValue);

    const targetUrl = `${pathMap[key]}?${safeParam}=${safeValue}`;

    // ✅ Open tab/window aman
    window.open(targetUrl, target);
}

