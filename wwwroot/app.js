const state = {
    token: localStorage.getItem("token") || "",
    currentUser: null
};

const apiBase = "";

function setOutput(id, value) {
    const el = document.getElementById(id);
    if (!el) return;
    el.textContent = typeof value === "string" ? value : JSON.stringify(value, null, 2);
}

async function api(path, options = {}) {
    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {})
    };

    if (state.token) {
        headers.Authorization = `Bearer ${state.token}`;
    }

    const response = await fetch(`${apiBase}${path}`, { ...options, headers });
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
        const message = typeof data === "string" ? data : JSON.stringify(data);
        throw new Error(`${response.status}: ${message}`);
    }

    return data;
}

async function loadCurrentUser() {
    const me = await api("/api/Auth/Me");
    state.currentUser = me;
    setOutput("profile-output", me);
    return me;
}

document.getElementById("register-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const f = e.target;
    try {
        const payload = {
            name: f.name.value,
            surname: f.surname.value,
            email: f.email.value,
            userName: f.userName.value,
            phoneNumber: f.phoneNumber.value,
            password: f.password.value
        };

        const result = await api("/api/Auth/Register", {
            method: "POST",
            body: JSON.stringify(payload),
            skipAuth: true
        });
        setOutput("profile-output", { registeredUserId: result, message: "Registration successful, now login." });
        f.reset();
    } catch (err) {
        setOutput("profile-output", err.message);
    }
});

document.getElementById("login-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const f = e.target;
    try {
        const payload = {
            email: f.email.value,
            password: f.password.value
        };
        const result = await api("/api/Auth/Login", {
            method: "POST",
            body: JSON.stringify(payload),
            skipAuth: true
        });
        state.token = result.token;
        localStorage.setItem("token", state.token);
        await loadCurrentUser();
        f.reset();
    } catch (err) {
        setOutput("profile-output", err.message);
    }
});

document.getElementById("me-btn").addEventListener("click", async () => {
    try {
        await loadCurrentUser();
    } catch (err) {
        setOutput("profile-output", err.message);
    }
});

document.getElementById("logout-btn").addEventListener("click", () => {
    state.token = "";
    state.currentUser = null;
    localStorage.removeItem("token");
    setOutput("profile-output", "Logged out");
});

document.getElementById("create-ticket-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const f = e.target;
    try {
        if (!state.currentUser) {
            await loadCurrentUser();
        }

        const payload = {
            title: f.title.value,
            description: f.description.value,
            priority: Number(f.priority.value),
            userId: state.currentUser.id,
            assignedToId: null
        };

        const ticketId = await api("/api/Tickets/CreateTicket", {
            method: "POST",
            body: JSON.stringify(payload)
        });
        setOutput("create-ticket-output", { ticketId, message: "Ticket created" });
        f.reset();
    } catch (err) {
        setOutput("create-ticket-output", err.message);
    }
});

document.getElementById("ticket-details-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const f = e.target;
    try {
        const result = await api(`/api/Tickets/TicketDetails/${encodeURIComponent(f.ticketId.value)}`);
        setOutput("ticket-details-output", result);
    } catch (err) {
        setOutput("ticket-details-output", err.message);
    }
});

document.getElementById("create-comment-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const f = e.target;
    try {
        if (!state.currentUser) {
            await loadCurrentUser();
        }

        const payload = {
            text: f.text.value,
            userId: state.currentUser.id,
            ticketId: f.ticketId.value
        };
        const commentId = await api("/api/Tickets/Comments/AddCommentToTicket", {
            method: "POST",
            body: JSON.stringify(payload)
        });
        setOutput("create-comment-output", { commentId, message: "Comment created" });
        f.reset();
    } catch (err) {
        setOutput("create-comment-output", err.message);
    }
});

if (state.token) {
    loadCurrentUser().catch((err) => {
        setOutput("profile-output", err.message);
    });
}
