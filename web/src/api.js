const base = import.meta.env.VITE_API_BASE || "/api/v1";
let tokens;
try {
    tokens = JSON.parse(sessionStorage.getItem("dormflow.session") || "null");
} catch {
    tokens = null;
}
let refreshPending;
export function session() {
    return tokens;
}
export function saveSession(value) {
    tokens = value;
    if (value)
        sessionStorage.setItem("dormflow.session", JSON.stringify(value));
    else sessionStorage.removeItem("dormflow.session");
    window.dispatchEvent(new Event("sessionchange"));
}
async function refresh() {
    if (!tokens?.refreshToken) throw new Error("กรุณาเข้าสู่ระบบอีกครั้ง");
    if (!refreshPending)
        refreshPending = (async () => {
            const response = await fetch(`${base}/auth/refresh`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ refreshToken: tokens.refreshToken }),
            });
            const body = await response.json();
            if (!response.ok) {
                saveSession(null);
                throw new Error("หมดเวลาใช้งาน กรุณาเข้าสู่ระบบอีกครั้ง");
            }
            saveSession(body.data);
        })().finally(() => {
            refreshPending = null;
        });
    return refreshPending;
}
export async function api(path, options = {}, retry = true) {
    const headers = {
        ...(options.body ? { "Content-Type": "application/json" } : {}),
        ...(tokens?.accessToken
            ? { Authorization: `Bearer ${tokens.accessToken}` }
            : {}),
        ...options.headers,
    };
    let response;
    try {
        response = await fetch(`${base}/${path}`, {
            ...options,
            headers,
            body: options.body ? JSON.stringify(options.body) : undefined,
        });
    } catch (error) {
        if (error.name === "AbortError") throw error;
        throw new Error("เชื่อมต่อระบบไม่ได้ ตรวจสอบการเชื่อมต่อแล้วลองใหม่");
    }
    if (response.status === 401 && retry && !path.startsWith("auth/")) {
        await refresh();
        return api(path, options, false);
    }
    if (options.download && response.ok) return response.blob();
    const body = await response.json().catch(() => ({}));
    if (!response.ok) {
        const error = new Error(
            body.message ||
                (response.status === 403
                    ? "คุณไม่มีสิทธิ์ทำรายการนี้"
                    : "ทำรายการไม่สำเร็จ กรุณาลองใหม่"),
        );
        error.fields = body.errors;
        error.status = response.status;
        throw error;
    }
    return body.data;
}
export async function download(path, filename) {
    const blob = await api(path, { download: true });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");
    link.href = url;
    link.download = filename;
    link.click();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
}
