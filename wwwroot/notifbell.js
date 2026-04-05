import { getNotifications, clearNotifications } from "./notifications.js?v=2";
import { t } from "./i18n.js?v=4";

let unreadCount = 0;

function formatTime(iso) {
    if (!iso) return "";
    const d = new Date(iso);
    return d.toLocaleTimeString(undefined, { hour: "2-digit", minute: "2-digit" });
}

function renderList(listEl) {
    const items = getNotifications();
    if (items.length === 0) {
        listEl.innerHTML = `<p class="muted" style="padding:12px;margin:0;font-size:13px;">${t("notify.empty")}</p>`;
        return;
    }
    listEl.innerHTML = items.map((n, i) => {
        const href = n.ticketId ? `/ticket.html?id=${encodeURIComponent(n.ticketId)}` : "#";
        const label = n.kind === "newTicketStaff" || n.kind === "newTicketClient"
            ? t("notify.newTicketLabel")
            : t("notify.replyLabel");
        const isNew = i < unreadCount ? "is-new" : "";
        return `<a class="notif-item ${isNew}" href="${href}">
            <div class="notif-item-title">${n.title || "—"}</div>
            <div class="notif-item-meta">${label} · ${formatTime(n.receivedAt)}</div>
        </a>`;
    }).join("");
}

/**
 * Инициализирует колокол уведомлений.
 * Ожидает в DOM: #notif-bell-btn, #notif-badge, #notif-dropdown, #notif-list, #notif-clear-btn
 * @param {{ onNewTicket?: () => void }} options
 */
export function initNotifBell(options = {}) {
    const bellBtn = document.getElementById("notif-bell-btn");
    const badge = document.getElementById("notif-badge");
    const dropdown = document.getElementById("notif-dropdown");
    const listEl = document.getElementById("notif-list");
    const clearBtn = document.getElementById("notif-clear-btn");

    if (!bellBtn || !dropdown || !listEl) return;

    let open = false;

    function setOpen(val) {
        open = val;
        dropdown.style.display = open ? "block" : "none";
        if (open) {
            unreadCount = 0;
            updateBadge();
            renderList(listEl);
        }
    }

    function updateBadge() {
        if (!badge) return;
        if (unreadCount > 0) {
            badge.style.display = "flex";
            badge.textContent = unreadCount > 99 ? "99+" : String(unreadCount);
        } else {
            badge.style.display = "none";
        }
    }

    bellBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        setOpen(!open);
    });

    document.addEventListener("click", (e) => {
        if (open && !dropdown.contains(e.target) && e.target !== bellBtn) {
            setOpen(false);
        }
    });

    if (clearBtn) {
        clearBtn.addEventListener("click", (e) => {
            e.stopPropagation();
            clearNotifications();
            unreadCount = 0;
            updateBadge();
            renderList(listEl);
        });
    }

    window.addEventListener("ticketnotify", (e) => {
        unreadCount++;
        updateBadge();
        if (open) {
            renderList(listEl);
        }
        if (options.onNewTicket) {
            options.onNewTicket(e.detail);
        }
    });
}
