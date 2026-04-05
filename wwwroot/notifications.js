import { getToken } from "./common.js?v=4";
import { t } from "./i18n.js?v=4";

let connectionStarted = false;
let signalRLoadPromise = null;

const MAX_NOTIFS = 20;
const _notifList = [];

function addNotification(payload) {
    _notifList.unshift({ ...payload, receivedAt: new Date().toISOString() });
    if (_notifList.length > MAX_NOTIFS) _notifList.length = MAX_NOTIFS;
    window.dispatchEvent(new CustomEvent("ticketnotify", { detail: payload }));
}

export function getNotifications() {
    return [..._notifList];
}

export function clearNotifications() {
    _notifList.length = 0;
}

/** Локальный скрипт — не зависит от CDN (иначе при сбое CDN ломается вся страница). */
function ensureSignalR() {
    if (typeof window !== "undefined" && window.signalR?.HubConnectionBuilder) {
        return Promise.resolve(window.signalR);
    }
    if (signalRLoadPromise) {
        return signalRLoadPromise;
    }
    signalRLoadPromise = new Promise((resolve, reject) => {
        const s = document.createElement("script");
        s.src = "/lib/signalr.min.js";
        s.async = true;
        s.onload = () => {
            if (window.signalR?.HubConnectionBuilder) {
                resolve(window.signalR);
            } else {
                reject(new Error("signalR не найден после загрузки"));
            }
        };
        s.onerror = () => reject(new Error("Не удалось загрузить /lib/signalr.min.js"));
        document.head.appendChild(s);
    });
    return signalRLoadPromise;
}

function formatMessage(payload) {
    const title = payload.title || "";
    switch (payload.kind) {
        case "newTicketClient":
            return `${t("notify.newTicketClient")}: ${title}`;
        case "newTicketStaff":
            return `${t("notify.newTicketStaff")}: ${title}`;
        case "ticketReply":
            return `${t("notify.ticketReply")}: ${title}`;
        default:
            return title || t("notify.ticketReply");
    }
}

function showToast(text, ticketId) {
    let container = document.getElementById("toast-container");
    if (!container) {
        container = document.createElement("div");
        container.id = "toast-container";
        container.className = "toast-container";
        container.setAttribute("aria-live", "polite");
        document.body.appendChild(container);
    }
    const el = document.createElement("div");
    el.className = "toast";
    el.textContent = text;
    if (ticketId) {
        el.style.cursor = "pointer";
        el.title = t("table.open");
        el.addEventListener("click", () => {
            window.location.href = `/ticket.html?id=${encodeURIComponent(ticketId)}`;
        });
    }
    container.appendChild(el);
    const remove = () => {
        el.classList.add("toast-out");
        setTimeout(() => el.remove(), 320);
    };
    setTimeout(remove, 6500);
}

/**
 * Подключает SignalR и показывает push (вкладка + системное уведомление при разрешении).
 */
export async function initNotifications() {
    if (connectionStarted) {
        return;
    }
    if (!getToken()) {
        return;
    }

    let signalR;
    try {
        signalR = await ensureSignalR();
    } catch (e) {
        console.warn("SignalR notifications:", e);
        return;
    }

    try {
        if (
            typeof Notification !== "undefined" &&
            Notification.permission === "default" &&
            !localStorage.getItem("notifyPromptAsked")
        ) {
            localStorage.setItem("notifyPromptAsked", "1");
            void Notification.requestPermission();
        }
    } catch {
        /* ignore */
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${window.location.origin}/hubs/notifications`, {
            accessTokenFactory: () => getToken() || ""
        })
        .withAutomaticReconnect()
        .build();

    connection.on("notify", (payload) => {
        addNotification(payload);
        const text = formatMessage(payload);
        showToast(text, payload.ticketId);
        try {
            if (typeof Notification !== "undefined" && Notification.permission === "granted") {
                new Notification(document.title, { body: text });
            }
        } catch {
            /* ignore */
        }
    });

    try {
        await connection.start();
        connectionStarted = true;
    } catch (e) {
        console.warn("SignalR notifications:", e);
    }
}
