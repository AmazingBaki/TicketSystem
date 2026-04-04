import { t, localeTag, statusLabel, priorityLabel } from "./i18n.js?v=4";

const TOKEN_KEY = "token";

export function getToken() {
    return localStorage.getItem(TOKEN_KEY) || "";
}

export function setToken(token) {
    localStorage.setItem(TOKEN_KEY, token);
}

export function clearToken() {
    localStorage.removeItem(TOKEN_KEY);
}

export async function api(path, options = {}) {
    const headers = {
        ...(options.headers || {})
    };

    if (!headers["Content-Type"] && !(options.body instanceof FormData)) {
        headers["Content-Type"] = "application/json";
    }

    const token = getToken();
    if (token && !options.skipAuth) {
        headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch(path, { ...options, headers });
    const raw = await response.text();
    let data = null;

    if (raw) {
        try {
            data = JSON.parse(raw);
        } catch {
            data = raw;
        }
    }

    if (!response.ok) {
        const message = formatApiError(data);
        throw new Error(message ? `${response.status}: ${message}` : `${response.status}: ${t("api.requestError")}`);
    }

    return data;
}

export async function getCurrentUser() {
    const token = getToken();
    if (!token) {
        return null;
    }

    try {
        return await api("/api/Auth/Me");
    } catch {
        clearToken();
        return null;
    }
}

export async function requireAuth(allowedRoles = []) {
    const me = await getCurrentUser();
    if (!me) {
        window.location.href = "/login.html";
        return null;
    }

    if (allowedRoles.length > 0) {
        const hasRole = me.roles.some((r) => allowedRoles.includes(r));
        if (!hasRole) {
            if (me.roles.includes("Admin")) {
                window.location.href = "/admin.html";
            } else if (me.roles.includes("SupportAgent")) {
                window.location.href = "/support.html";
            } else {
                window.location.href = "/user.html";
            }
            return null;
        }
    }

    return me;
}

export function setOutput(id, value) {
    const el = document.getElementById(id);
    if (!el) return;
    el.textContent = typeof value === "string" ? value : JSON.stringify(value, null, 2);
}

export function formatApiError(data) {
    if (!data) return "";
    if (typeof data === "string") return data;

    const parts = [];

    if (typeof data.message === "string" && data.message) {
        parts.push(data.message);
    }

    if (data.errors) {
        if (Array.isArray(data.errors)) {
            const lines = data.errors.map((e) => {
                if (typeof e === "string") return e;
                return e.errorMessage || e.message || e.description || String(e);
            });
            parts.push(...lines);
        } else if (typeof data.errors === "object") {
            for (const [key, val] of Object.entries(data.errors)) {
                const msgs = Array.isArray(val) ? val : [val];
                for (const m of msgs) {
                    parts.push(`${key}: ${typeof m === "string" ? m : JSON.stringify(m)}`);
                }
            }
        }
    }

    if (parts.length > 0) {
        return parts.join("\n");
    }

    if (typeof data.title === "string" && data.title) {
        return data.detail || data.title;
    }

    return JSON.stringify(data);
}

export function formatDate(value) {
    if (!value) return "-";
    const date = new Date(value);
    return date.toLocaleString(localeTag());
}

export function statusText(status) {
    return statusLabel(status);
}

export function priorityText(priority) {
    return priorityLabel(priority);
}
