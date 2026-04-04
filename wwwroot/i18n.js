const LOCALE_KEY = "locale";
const FALLBACK = "ru";

const MESSAGES = {
    ru: {
        "meta.indexTitle": "Ticket Support System",
        "meta.loginTitle": "Вход",
        "meta.registerTitle": "Регистрация",
        "meta.adminTitle": "Панель администратора",
        "meta.userTitle": "Кабинет пользователя",
        "meta.supportTitle": "Служба поддержки",
        "meta.ticketTitle": "Тикет",
        "meta.profileTitle": "Профиль",
        "home.title": "Ticket Support System",
        "home.subtitle": "Выберите действие:",
        "home.login": "Войти",
        "home.register": "Регистрация",
        "home.redirectHint": "Если вы уже вошли, система автоматически перенаправит в нужный кабинет.",
        "home.notAuth": "Вы не авторизованы.",
        "lang.ru": "Русский",
        "lang.ky": "Кыргызча",
        "nav.console": "Консоль",
        "nav.ratings": "Рейтинг",
        "nav.userView": "Пользовательский вид",
        "nav.login": "Вход",
        "nav.user": "Пользователь",
        "nav.home": "Главная",
        "nav.myTickets": "Мои тикеты",
        "nav.back": "Назад",
        "nav.mainNav": "Навигация",
        "top.refresh": "Обновить",
        "top.createTicket": "Создать тикет",
        "top.profileTitle": "Профиль",
        "top.logout": "Выйти",
        "top.menu": "Меню",
        "top.profileLink": "Профиль",
        "stats.totalTickets": "Всего тикетов",
        "stats.open": "Открытые",
        "stats.inProgress": "В работе",
        "stats.closed": "Закрытые",
        "stats.myTotal": "Всего моих",
        "table.recentTickets": "Недавние тикеты",
        "table.myTickets": "Мои тикеты",
        "table.searchTitle": "Поиск по заголовку...",
        "table.search": "Поиск...",
        "table.colTicket": "Тикет",
        "table.colStatus": "Статус",
        "table.colPriority": "Приоритет",
        "table.colAction": "Действие",
        "table.colEmployee": "Сотрудник",
        "table.colAssigned": "Назначено",
        "table.colClosed": "Закрыто",
        "table.colFirstResponse": "Первый ответ",
        "table.colResolution": "Решение",
        "table.open": "Открыть",
        "table.noTickets": "Тикеты не найдены.",
        "table.noMyTickets": "Тикеты пока не созданы.",
        "table.noData": "Нет данных.",
        "roles.title": "Управление ролями",
        "roles.emailPh": "Email пользователя",
        "roles.add": "Добавить роль",
        "roles.remove": "Удалить роль",
        "roles.added": "Роль добавлена.",
        "roles.removed": "Роль удалена.",
        "ratings.sectionTitle": "Рейтинг службы поддержки",
        "ratings.refresh": "Обновить рейтинг",
        "support.myRating": "Мой рейтинг",
        "support.metricsHint": "Метрики считаются автоматически по назначенным тикетам и ответам.",
        "login.emailPh": "Email",
        "login.passwordPh": "Пароль",
        "login.submit": "Войти",
        "login.hasAccount": "Регистрация",
        "login.toHome": "На главную",
        "login.profileLoadError": "Не удалось загрузить профиль.",
        "register.title": "Регистрация",
        "register.hint": "Пароль — не короче 8 символов. Если этот email уже зарегистрирован, откройте",
        "register.hintLogin": "Вход",
        "register.namePh": "Имя",
        "register.surnamePh": "Фамилия",
        "register.emailPh": "Email",
        "register.userNamePh": "Username (уникальное)",
        "register.phonePh": "Телефон",
        "register.passwordPh": "Пароль (мин. 8 символов)",
        "register.submit": "Зарегистрироваться",
        "register.hasAccount": "Уже есть аккаунт",
        "register.toHome": "На главную",
        "register.success": "Успешно зарегистрированы. Теперь войдите.",
        "user.createTitle": "Создать тикет",
        "user.titlePh": "Заголовок",
        "user.descPh": "Описание",
        "user.priorityLabel": "Приоритет",
        "user.createBtn": "Создать",
        "user.ticketCreated": "Тикет создан:",
        "ticket.status0": "Открыт",
        "ticket.status1": "В работе",
        "ticket.status2": "Закрыт",
        "ticket.priority0": "Низкий",
        "ticket.priority1": "Средний",
        "ticket.priority2": "Высокий",
        "ticket.editTitle": "Обновить тикет",
        "ticket.assignedPh": "AssignedToId (GUID, необязательно)",
        "ticket.saveChanges": "Сохранить изменения",
        "ticket.comments": "Комментарии",
        "ticket.reply": "Ответить",
        "ticket.replyPh": "Введите ваш ответ...",
        "ticket.sendComment": "Отправить комментарий",
        "ticket.author": "Автор:",
        "ticket.created": "Создан:",
        "ticket.attachments": "Вложения:",
        "ticket.noAttachments": "нет",
        "ticket.noComments": "Комментариев пока нет.",
        "ticket.missingId": "Не передан id тикета.",
        "ticket.commentSent": "Комментарий отправлен.",
        "ticket.updated": "Тикет обновлен.",
        "ticket.editText": "Изменить текст",
        "ticket.save": "Сохранить",
        "ticket.cancel": "Отмена",
        "ticket.saved": "Сохранено.",
        "profile.dataTitle": "Данные профиля",
        "profile.passwordTitle": "Изменить пароль",
        "profile.emailTitle": "Изменить email",
        "profile.emailHint": "После смены email потребуется войти заново (JWT-токен привязан к email).",
        "profile.namePh": "Имя",
        "profile.surnamePh": "Фамилия",
        "profile.userNamePh": "Username",
        "profile.phonePh": "Телефон",
        "profile.save": "Сохранить",
        "profile.currentPasswordPh": "Текущий пароль",
        "profile.newPasswordPh": "Новый пароль",
        "profile.changePassword": "Сменить пароль",
        "profile.newEmailPh": "Новый email",
        "profile.changeEmail": "Сменить email",
        "profile.loaded": "Загружено.",
        "profile.saved": "Сохранено.",
        "profile.passwordChanged": "Пароль изменён.",
        "profile.emailChanged": "Email изменён. Нужно войти заново.",
        "time.minShort": "мин",
        "time.hourShort": "ч",
        "api.requestError": "Ошибка запроса."
    },
    ky: {
        "meta.indexTitle": "Билдирүүлөрдү колдоо системасы",
        "meta.loginTitle": "Кирүү",
        "meta.registerTitle": "Катталуу",
        "meta.adminTitle": "Администратор панели",
        "meta.userTitle": "Колдонуучу кабинети",
        "meta.supportTitle": "Колдоо кызматы",
        "meta.ticketTitle": "Тикет",
        "meta.profileTitle": "Профиль",
        "home.title": "Билдирүүлөрдү колдоо системасы",
        "home.subtitle": "Аракетти тандаңыз:",
        "home.login": "Кирүү",
        "home.register": "Катталуу",
        "home.redirectHint": "Эгер сиз кирип калган болсоңуз, система сизди автоматтык түрдө туура кабинетке багыттайт.",
        "home.notAuth": "Сиз авторизацияланган эмессиз.",
        "lang.ru": "Орусча",
        "lang.ky": "Кыргызча",
        "nav.console": "Консоль",
        "nav.ratings": "Рейтинг",
        "nav.userView": "Колдонуучу көрүнүшү",
        "nav.login": "Кирүү",
        "nav.user": "Колдонуучу",
        "nav.home": "Башкы бет",
        "nav.myTickets": "Менин тикеттерим",
        "nav.back": "Артка",
        "nav.mainNav": "Навигация",
        "top.refresh": "Жаңылоо",
        "top.createTicket": "Тикет түзүү",
        "top.profileTitle": "Профиль",
        "top.logout": "Чыгуу",
        "top.menu": "Меню",
        "top.profileLink": "Профиль",
        "stats.totalTickets": "Бардык тикеттер",
        "stats.open": "Ачык",
        "stats.inProgress": "Иштелүүдө",
        "stats.closed": "Жабылган",
        "stats.myTotal": "Меники бардыгы",
        "table.recentTickets": "Акыркы тикеттер",
        "table.myTickets": "Менин тикеттерим",
        "table.searchTitle": "Аталышы боюнча издөө...",
        "table.search": "Издөө...",
        "table.colTicket": "Тикет",
        "table.colStatus": "Статус",
        "table.colPriority": "Приоритет",
        "table.colAction": "Аракет",
        "table.colEmployee": "Кызматкер",
        "table.colAssigned": "Дайындалган",
        "table.colClosed": "Жабылган",
        "table.colFirstResponse": "Биринчи жооп",
        "table.colResolution": "Чечим",
        "table.open": "Ачуу",
        "table.noTickets": "Тикеттер табылган жок.",
        "table.noMyTickets": "Тикеттер азырынча жок.",
        "table.noData": "Маалымат жок.",
        "roles.title": "Ролдорду башкаруу",
        "roles.emailPh": "Колдонуучунун emailи",
        "roles.add": "Рол кошуу",
        "roles.remove": "Ролду алып салуу",
        "roles.added": "Рол кошулду.",
        "roles.removed": "Рол алынып салынды.",
        "ratings.sectionTitle": "Колдоо кызматынын рейтинги",
        "ratings.refresh": "Рейтингти жаңылоо",
        "support.myRating": "Менин рейтингим",
        "support.metricsHint": "Метрикалар дайындалган тикеттер жана жооптор боюнча автоматтык эсептелет.",
        "login.emailPh": "Email",
        "login.passwordPh": "Сырсөз",
        "login.submit": "Кирүү",
        "login.hasAccount": "Катталуу",
        "login.toHome": "Башкы бетке",
        "login.profileLoadError": "Профильди жүктөө мүмкүн болгон жок.",
        "register.title": "Катталуу",
        "register.hint": "Сырсөз — эң кыскасы 8 символ. Бул email мурун катталган болсо,",
        "register.hintLogin": "Кирүү",
        "register.namePh": "Аты",
        "register.surnamePh": "Фамилиясы",
        "register.emailPh": "Email",
        "register.userNamePh": "Username (уникалдуу)",
        "register.phonePh": "Телефон",
        "register.passwordPh": "Сырсөз (мин. 8 символ)",
        "register.submit": "Катталуу",
        "register.hasAccount": "Аккаунт бар",
        "register.toHome": "Башкы бетке",
        "register.success": "Ийгиликтүү катталдыңыз. Эми кириңиз.",
        "user.createTitle": "Тикет түзүү",
        "user.titlePh": "Аталышы",
        "user.descPh": "Баяндоо",
        "user.priorityLabel": "Приоритет",
        "user.createBtn": "Түзүү",
        "user.ticketCreated": "Тикет түзүлдү:",
        "ticket.status0": "Ачык",
        "ticket.status1": "Иштелүүдө",
        "ticket.status2": "Жабылган",
        "ticket.priority0": "Төмөн",
        "ticket.priority1": "Орто",
        "ticket.priority2": "Жогору",
        "ticket.editTitle": "Тикетти жаңылоо",
        "ticket.assignedPh": "AssignedToId (GUID, милдеттүү эмес)",
        "ticket.saveChanges": "Өзгөртүүлөрдү сактоо",
        "ticket.comments": "Комментарийлер",
        "ticket.reply": "Жооп берүү",
        "ticket.replyPh": "Жообуңузду жазыңыз...",
        "ticket.sendComment": "Комментарий жөнөтүү",
        "ticket.author": "Автор:",
        "ticket.created": "Түзүлгөн:",
        "ticket.attachments": "Тиркемелер:",
        "ticket.noAttachments": "жок",
        "ticket.noComments": "Комментарийлер азырынча жок.",
        "ticket.missingId": "Тикет id берилген эмес.",
        "ticket.commentSent": "Комментарий жөнөтүлдү.",
        "ticket.updated": "Тикет жаңыланды.",
        "ticket.editText": "Текстти өзгөртүү",
        "ticket.save": "Сактоо",
        "ticket.cancel": "Жокко чыгаруу",
        "ticket.saved": "Сакталды.",
        "profile.dataTitle": "Профиль маалыматы",
        "profile.passwordTitle": "Сырсөздү өзгөртүү",
        "profile.emailTitle": "Emailди өзгөртүү",
        "profile.emailHint": "Email өзгөргөндөн кийин кайра кирүү керек (JWT токен emailге байланган).",
        "profile.namePh": "Аты",
        "profile.surnamePh": "Фамилиясы",
        "profile.userNamePh": "Username",
        "profile.phonePh": "Телефон",
        "profile.save": "Сактоо",
        "profile.currentPasswordPh": "Учурдагы сырсөз",
        "profile.newPasswordPh": "Жаңы сырсөз",
        "profile.changePassword": "Сырсөздү өзгөртүү",
        "profile.newEmailPh": "Жаңы email",
        "profile.changeEmail": "Emailди өзгөртүү",
        "profile.loaded": "Жүктөлдү.",
        "profile.saved": "Сакталды.",
        "profile.passwordChanged": "Сырсөз өзгөртүлдү.",
        "profile.emailChanged": "Email өзгөртүлдү. Кайра кириңиз.",
        "time.minShort": "мүн",
        "time.hourShort": "саат",
        "api.requestError": "Суроо катасы."
    }
};

export function getLocale() {
    const stored = localStorage.getItem(LOCALE_KEY);
    if (stored === "ru" || stored === "ky") {
        return stored;
    }
    return FALLBACK;
}

export function setLocale(lang) {
    if (lang === "ru" || lang === "ky") {
        localStorage.setItem(LOCALE_KEY, lang);
    }
}

export function t(key) {
    const locale = getLocale();
    const pack = MESSAGES[locale] || MESSAGES[FALLBACK];
    const fallbackPack = MESSAGES[FALLBACK];
    return pack[key] ?? fallbackPack[key] ?? key;
}

export function localeTag() {
    return getLocale() === "ky" ? "ky-KG" : "ru-RU";
}

export function applyDomI18n(root = document) {
    const lang = getLocale();
    const doc = root.ownerDocument || root;
    const htmlEl = doc.documentElement;
    if (htmlEl) {
        htmlEl.lang = lang === "ky" ? "ky" : "ru";
    }

    root.querySelectorAll("[data-i18n]").forEach((el) => {
        const key = el.getAttribute("data-i18n");
        if (key) {
            el.textContent = t(key);
        }
    });

    root.querySelectorAll("[data-i18n-placeholder]").forEach((el) => {
        const key = el.getAttribute("data-i18n-placeholder");
        if (key && "placeholder" in el) {
            el.placeholder = t(key);
        }
    });

    root.querySelectorAll("[data-i18n-title]").forEach((el) => {
        const key = el.getAttribute("data-i18n-title");
        if (key) {
            el.title = t(key);
        }
    });

    root.querySelectorAll("[data-i18n-aria-label]").forEach((el) => {
        const key = el.getAttribute("data-i18n-aria-label");
        if (key) {
            el.setAttribute("aria-label", t(key));
        }
    });

    const titleEl = root.querySelector("title[data-i18n]");
    if (titleEl) {
        const key = titleEl.getAttribute("data-i18n");
        if (key) {
            titleEl.textContent = t(key);
        }
    }
}

export function bindLangSwitch(root = document) {
    root.querySelectorAll("[data-set-lang]").forEach((btn) => {
        btn.addEventListener("click", () => {
            const lang = btn.getAttribute("data-set-lang");
            if (lang === "ru" || lang === "ky") {
                setLocale(lang);
                window.location.reload();
            }
        });
    });
}

export function statusLabel(status) {
    return t(`ticket.status${status}`);
}

export function priorityLabel(priority) {
    return t(`ticket.priority${priority}`);
}
