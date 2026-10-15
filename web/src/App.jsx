import { useEffect, useState, useCallback, useRef, lazy, Suspense } from "react";
import { Alert, Button, Form, Spinner } from "react-bootstrap";
import {
    Building2,
    LayoutDashboard,
    DoorOpen,
    Users,
    CalendarDays,
    FileText,
    Gauge,
    Receipt,
    Wallet,
    Wrench,
    Package,
    Megaphone,
    Bell,
    Settings,
    ChevronLeft,
    ChevronRight,
    Plus,
    Search,
    Menu,
    LogOut,
    ArrowUpRight,
    RefreshCw,
    Download,
    X,
} from "lucide-react";
const ApexChart = lazy(() => import("react-apexcharts"));
function Chart(props) {
    return (
        <Suspense fallback={<Loading />}>
            <ApexChart
                {...props}
                options={{
                    ...props.options,
                    chart: {
                        ...props.options?.chart,
                        animations: { enabled: false },
                    },
                }}
            />
        </Suspense>
    );
}
import resources from "./schema.json";
import { api, download, session, saveSession } from "./api";
import { labels, states, money, today, show, recordName } from "./labels";
import { Editor, field, Lookup } from "./Forms";

const icons = {
    rooms: DoorOpen,
    tenants: Users,
    reservations: CalendarDays,
    contracts: FileText,
    "meter-readings": Gauge,
    "billing-periods": CalendarDays,
    invoices: Receipt,
    payments: Wallet,
    receipts: Receipt,
    expenses: Wallet,
    "maintenance-requests": Wrench,
    parcels: Package,
    announcements: Megaphone,
    notifications: Bell,
};
const viewPermission = {
    "billing-periods": "invoices.view",
    "maintenance-requests": "maintenance.view",
    users: "users.manage",
    roles: "roles.manage",
    settings: "settings.manage",
};
const createPermission = {
    parcels: "parcels.log",
    "maintenance-requests": "maintenance.update-status",
    users: "users.manage",
    roles: "roles.manage",
};
const reports = [
    ["revenue", "รายรับ"],
    ["expenses", "รายจ่าย"],
    ["cash-flow", "กระแสเงินสด"],
    ["outstanding", "ยอดค้างชำระ"],
    ["occupancy", "อัตราการเข้าพัก"],
    ["contracts", "สัญญาเช่า"],
    ["reservations", "การจอง"],
    ["maintenance", "งานซ่อม"],
    ["parcels", "พัสดุ"],
    ["move-in-out", "เข้าอยู่ / ย้ายออก"],
    ["room-performance", "ประสิทธิภาพห้องพัก"],
];
const useSession = () => {
    const [value, setValue] = useState(session());
    useEffect(() => {
        const update = () => setValue(session());
        window.addEventListener("sessionchange", update);
        return () => window.removeEventListener("sessionchange", update);
    }, []);
    return value;
};
function useData(path, revision = 0) {
    const [state, setState] = useState({
        loading: true,
        data: null,
        error: "",
    });
    useEffect(() => {
        const controller = new AbortController();
        setState((s) => ({ ...s, loading: true, error: "" }));
        api(path, { signal: controller.signal })
            .then((data) => setState({ loading: false, data, error: "" }))
            .catch((e) => {
                if (e.name !== "AbortError")
                    setState({ loading: false, data: null, error: e.message });
            });
        return () => controller.abort();
    }, [path, revision]);
    return state;
}
function Loading() {
    return (
        <div className="loading" role="status">
            <Spinner size="sm" /> กำลังโหลดข้อมูล…
        </div>
    );
}
function Problem({ error, retry }) {
    return (
        <Alert variant="danger">
            <p>{error}</p>
            {retry && (
                <Button variant="outline-danger" onClick={retry}>
                    ลองใหม่
                </Button>
            )}
        </Alert>
    );
}
function Status({ value }) {
    return (
        <span className={`status state-${String(value).toLowerCase()}`}>
            {states[value] || value}
        </span>
    );
}

function Login() {
    const [error, setError] = useState(""),
        [busy, setBusy] = useState(false);
    async function login(e) {
        e.preventDefault();
        setBusy(true);
        setError("");
        const data = new FormData(e.currentTarget);
        try {
            saveSession(
                await api("auth/login", {
                    method: "POST",
                    body: {
                        email: data.get("email"),
                        password: data.get("password"),
                    },
                }),
            );
        } catch (e) {
            setError(e.message);
        } finally {
            setBusy(false);
        }
    }
    return (
        <main className="login-page">
            <section className="login-story">
                <div className="brand">
                    <Building2 size={30} />
                    <span>DormFlow</span>
                </div>
                <div>
                    <h1>
                        ทุกเรื่องของหอพัก
                        <br />
                        อยู่ในที่เดียว
                    </h1>
                    <p>
                        ดูแลห้องพัก ผู้เช่า และการเงิน
                        <br />
                        ให้ทุกวันทำงานได้คล่องขึ้น
                    </p>
                    <div className="login-line">
                        <DoorOpen />
                        <span>จากห้องว่าง สู่การเข้าอยู่</span>
                        <ArrowUpRight />
                    </div>
                    <div className="login-line">
                        <Receipt />
                        <span>จากบิล สู่ยอดรับชำระ</span>
                        <ArrowUpRight />
                    </div>
                </div>
                <small>จัดการหอพักอย่างเป็นระบบ</small>
            </section>
            <section className="login-form">
                <div className="login-box">
                    <h2>เข้าสู่ระบบ</h2>
                    <p className="muted">ใช้บัญชีที่ผู้ดูแลระบบสร้างให้คุณ</p>
                    <Form onSubmit={login}>
                        <Form.Group className="mb-4">
                            <Form.Label htmlFor="login-email">อีเมล</Form.Label>
                            <Form.Control
                                id="login-email"
                                name="email"
                                type="email"
                                autoComplete="username"
                                required
                                autoFocus
                                placeholder="name@example.com"
                            />
                        </Form.Group>
                        <Form.Group className="mb-4">
                            <Form.Label htmlFor="login-password">
                                รหัสผ่าน
                            </Form.Label>
                            <Form.Control
                                id="login-password"
                                name="password"
                                type="password"
                                autoComplete="current-password"
                                required
                            />
                        </Form.Group>
                        {error && <Problem error={error} />}
                        <Button type="submit" className="w-100" disabled={busy}>
                            {busy ? "กำลังเข้าสู่ระบบ…" : "เข้าสู่ระบบ"}
                        </Button>
                    </Form>
                    <p className="login-help">
                        หากยังไม่มีบัญชีหรือลืมรหัสผ่าน กรุณาติดต่อผู้ดูแลหอพัก
                    </p>
                </div>
            </section>
        </main>
    );
}

export default function App() {
    const auth = useSession();
    const menuTrigger = useRef(null);
    const [route, setRoute] = useState(location.hash.slice(1) || "dashboard"),
        [mobile, setMobile] = useState(false),
        [revision, setRevision] = useState(0);
    useEffect(()=>{
        if(!mobile) return;
        const previousOverflow=document.body.style.overflow;
        document.body.style.overflow='hidden';
        const focusable=()=>Array.from(document.querySelectorAll('.sidebar a, .sidebar button')).filter(el=>el.getClientRects().length);
        focusable()[0]?.focus();
        const onKey=e=>{
            if(e.key==='Escape'){setMobile(false);return;}
            if(e.key==='Tab'){
                const elements=focusable(), first=elements[0], last=elements.at(-1);
                if(e.shiftKey&&document.activeElement===first){e.preventDefault();last?.focus();}
                else if(!e.shiftKey&&document.activeElement===last){e.preventDefault();first?.focus();}
            }
        };
        document.addEventListener('keydown',onKey);
        return()=>{document.removeEventListener('keydown',onKey);document.body.style.overflow=previousOverflow;menuTrigger.current?.focus();};
    },[mobile]);
    useEffect(() => {
        const update = () => setRoute(location.hash.slice(1) || "dashboard");
        window.addEventListener("hashchange", update);
        return () => window.removeEventListener("hashchange", update);
    }, []);
    const can = useCallback(
        (permission) =>
            auth?.user.roles.includes("SuperAdmin") ||
            auth?.user.permissions.includes(permission),
        [auth],
    );
    const navigate = (key) => {
        location.hash = key;
        setRoute(key);
        setMobile(false);
    };
    if (!auth) return <Login />;
    const available = resources.filter(
        (r) =>
            r.key === "notifications" ||
            can(viewPermission[r.key] || `${r.key}.view`),
    );
    const selected = resources.find((r) => r.key === route);
    async function logout() {
        try {
            await api("auth/logout", {
                method: "POST",
                body: { refreshToken: auth.refreshToken },
            });
        } finally {
            saveSession(null);
        }
    }
    return (
        <div className="app-shell">
            <a className="skip-link" href="#main-content">
                ข้ามไปเนื้อหา
            </a>
            <aside id="main-sidebar" className={mobile ? "sidebar open" : "sidebar"} role={mobile?'dialog':undefined} aria-modal={mobile?true:undefined} aria-label="เมนูจัดการหอพัก">
                <a href="#dashboard" className="brand">
                    <Building2 size={27} />
                    <span>DormFlow</span>
                </a>
                <div className="sidebar-caption">จัดการหอพัก</div>
                <nav aria-label="เมนูหลัก">
                    <button
                        className={
                            route === "dashboard"
                                ? "nav-item active"
                                : "nav-item"
                        }
                        onClick={() => navigate("dashboard")}
                    >
                        <LayoutDashboard size={18} />
                        ภาพรวม
                    </button>
                    {[...new Set(available.map((r) => r.group))].map(
                        (group) => (
                            <div key={group}>
                                <div className="nav-group">{group}</div>
                                {available
                                    .filter((r) => r.group === group)
                                    .map((r) => {
                                        const Icon = icons[r.key] || Settings;
                                        return (
                                            <button
                                                key={r.key}
                                                className={
                                                    route === r.key
                                                        ? "nav-item active"
                                                        : "nav-item"
                                                }
                                                onClick={() => navigate(r.key)}
                                            >
                                                <Icon size={18} />
                                                {r.title}
                                            </button>
                                        );
                                    })}
                            </div>
                        ),
                    )}
                    {can("reports.view") && (
                        <button
                            className={
                                route === "reports"
                                    ? "nav-item active"
                                    : "nav-item"
                            }
                            onClick={() => navigate("reports")}
                        >
                            <LayoutDashboard size={18} />
                            รายงานและวิเคราะห์
                        </button>
                    )}
                </nav>
                <div className="sidebar-footer">
                    <div className="avatar">
                        {auth.user.firstName?.[0] || "D"}
                    </div>
                    <div>
                        <strong>{auth.user.fullName || auth.user.email}</strong>
                        <small>
                            {auth.user.roles
                                .map((r) => states[r] || r)
                                .join(", ")}
                        </small>
                    </div>
                    <button
                        onClick={logout}
                        title="ออกจากระบบ"
                        aria-label="ออกจากระบบ"
                    >
                        <LogOut size={19} />
                    </button>
                </div>
            </aside>
            {mobile && (
                <button
                    className="sidebar-scrim"
                    aria-label="ปิดเมนู"
                    onClick={() => setMobile(false)}
                />
            )}
            <div className="workspace">
                <header className="topbar">
                    <button
                        className="mobile-toggle"
                        ref={menuTrigger}
                        aria-expanded={mobile}
                        aria-controls="main-sidebar"
                        aria-label="เปิดเมนู"
                        onClick={() => setMobile(!mobile)}
                    >
                        <Menu />
                    </button>
                    <span>
                        พื้นที่จัดการ /{" "}
                        <strong>
                            {selected?.title ||
                                (route === "reports"
                                    ? "รายงานและวิเคราะห์"
                                    : "ภาพรวม")}
                        </strong>
                    </span>
                    <div className="topbar-right">
                        <span className="today">
                            {new Intl.DateTimeFormat("th-TH", {
                                dateStyle: "long",
                                timeZone: "Asia/Bangkok",
                            }).format(new Date())}
                        </span>
                        <NotificationBell
                            revision={revision}
                            onClick={() => navigate("notifications")}
                        />
                    </div>
                </header>
                <main id="main-content" tabIndex="-1">
                    {route === "dashboard" ? (
                        <Dashboard can={can} navigate={navigate} />
                    ) : route === "reports" && can("reports.view") ? (
                        <Reports />
                    ) : selected && available.some((r) => r.key === route) ? (
                        <Resource
                            key={route}
                            resource={selected}
                            can={can}
                            changed={() => setRevision((x) => x + 1)}
                        />
                    ) : (
                        <Problem error="คุณไม่มีสิทธิ์เข้าถึงหน้านี้" />
                    )}
                </main>
                <footer className="page-footer">
                    DormFlow <span>ดูแลทุกวัน ให้เป็นระบบ</span>
                </footer>
            </div>
        </div>
    );
}

function NotificationBell({ revision, onClick }) {
    const { data } = useData("notifications/unread-count", revision);
    return (
        <button
            className="notification-button"
            onClick={onClick}
            aria-label={`การแจ้งเตือน ${data?.unreadCount || 0} รายการยังไม่อ่าน`}
        >
            <Bell size={21} />
            {data?.unreadCount > 0 && (
                <span>{data.unreadCount > 99 ? "99+" : data.unreadCount}</span>
            )}
        </button>
    );
}

function Dashboard({ can, navigate }) {
    const [revision, setRevision] = useState(0);
    const { data, loading, error } = useData("dashboard/summary", revision);
    const from = today().slice(0, 7) + "-01";
    const analytics = useData(`analytics?from=${from}&to=${today()}`);
    if (loading) return <Loading />;
    if (error)
        return (
            <Problem error={error} retry={() => setRevision((x) => x + 1)} />
        );
    const financial = can("reports.view");
    const metrics = financial
        ? [
              ["รายรับเดือนนี้", data.revenueThisMonth, "payments"],
              ["รายจ่ายเดือนนี้", data.expensesThisMonth, "expenses"],
              [
                  "เงินสุทธิจากการดำเนินงาน",
                  data.netOperatingCashThisMonth,
                  "reports",
              ],
              ["ยอดค้างชำระทั้งหมด", data.outstandingInvoicesTotal, "invoices"],
          ]
        : [];
    return (
        <>
            <div className="page-heading">
                <div>
                    <h1>ภาพรวมหอพัก</h1>
                    <p>สิ่งที่เกิดขึ้นวันนี้ และรายการที่ต้องดูแลต่อ</p>
                </div>
                <Button
                    variant="outline-secondary"
                    onClick={() => setRevision((x) => x + 1)}
                >
                    <RefreshCw size={16} /> อัปเดตข้อมูล
                </Button>
            </div>
            {financial && (
                <div className="metrics">
                    {metrics.map(([label, value, target]) => (
                        <button key={label} onClick={() => navigate(target)}>
                            <span>
                                {label}
                                <ArrowUpRight size={16} />
                            </span>
                            <strong>{money(value)}</strong>
                        </button>
                    ))}
                </div>
            )}
            <div className="dashboard-grid">
                <section className="panel occupancy">
                    <div className="section-title">
                        <h2>ห้องพักในขณะนี้</h2>
                        <Button
                            variant="link"
                            onClick={() => navigate("rooms")}
                        >
                            ดูห้องทั้งหมด <ArrowUpRight size={16} />
                        </Button>
                    </div>
                    <div className="occupancy-layout">
                        <div>
                            <strong className="occupancy-number">
                                {data.occupancyRate ?? 0}
                                <small>%</small>
                            </strong>
                            <p className="muted">
                                อัตราการเข้าพัก · {data.totalRooms ?? 0}{" "}
                                ห้องทั้งหมด
                            </p>
                            <div className="status-legend">
                                {Object.entries(
                                    data.roomStatusCounts || {},
                                ).map(([key, count]) => (
                                    <div key={key}>
                                        <Status value={key} />
                                        <b>{count}</b>
                                    </div>
                                ))}
                            </div>
                        </div>
                        {data.totalRooms > 0 ? (
                            <Chart
                                type="donut"
                                height={230}
                                options={{
                                    labels: Object.keys(
                                        data.roomStatusCounts || {},
                                    ).map((x) => states[x]),
                                    colors: [
                                        "#61a988",
                                        "#dcad59",
                                        "#216650",
                                        "#91b9b0",
                                        "#b97a62",
                                        "#a4ada6",
                                    ],
                                    legend: { show: false },
                                    dataLabels: { enabled: false },
                                    stroke: { width: 4, colors: ["#fff"] },
                                    chart: { toolbar: { show: false } },
                                    plotOptions: {
                                        pie: { donut: { size: "72%" } },
                                    },
                                }}
                                series={Object.values(
                                    data.roomStatusCounts || {},
                                )}
                            />
                        ) : (
                            <div className="empty">
                                <DoorOpen size={36} />
                                <p>ยังไม่มีห้องพัก</p>
                                <Button
                                    size="sm"
                                    onClick={() => navigate("rooms")}
                                >
                                    เริ่มเพิ่มห้องพัก
                                </Button>
                            </div>
                        )}
                    </div>
                </section>
                <section className="panel attention">
                    <h2>รายการที่ต้องดูแล</h2>
                    {[
                        [
                            "invoices",
                            "ยอดค้างชำระ",
                            data.outstandingInvoicesCount,
                        ],
                        [
                            "contracts",
                            "สัญญาจะหมดใน 30 วัน",
                            data.expiringContractsCount,
                        ],
                        [
                            "maintenance-requests",
                            "งานซ่อมที่ยังไม่เสร็จ",
                            data.openMaintenanceCount,
                        ],
                        ["parcels", "พัสดุรอรับ", data.parcelsAwaitingPickup],
                        [
                            "reservations",
                            "การจองใกล้หมดเวลา",
                            data.expiringReservationsCount,
                        ],
                    ]
                        .filter(([key]) =>
                            can(viewPermission[key] || `${key}.view`),
                        )
                        .map(([key, label, count]) => (
                            <button
                                className="attention-row"
                                key={key}
                                onClick={() => navigate(key)}
                            >
                                <span>{label}</span>
                                <b>{count ?? 0}</b>
                                <ChevronRight size={16} />
                            </button>
                        ))}
                </section>
                {financial && (
                    <section className="panel cash-chart">
                        <div className="section-title">
                            <h2>รายรับเทียบรายจ่าย</h2>
                            <span className="muted">
                                เดือนนี้ · ตามวันที่รับ–จ่ายจริง
                            </span>
                        </div>
                        {analytics.loading ? (
                            <Loading />
                        ) : analytics.error ? (
                            <Problem error={analytics.error} />
                        ) : (
                            <Chart
                                type="bar"
                                height={260}
                                options={{
                                    chart: {
                                        toolbar: { show: false },
                                        fontFamily: "Noto Sans Thai",
                                    },
                                    colors: ["#216650", "#d6a459"],
                                    plotOptions: {
                                        bar: {
                                            borderRadius: 3,
                                            columnWidth: "35%",
                                        },
                                    },
                                    xaxis: {
                                        categories:
                                            analytics.data?.monthlyRevenue.map(
                                                (x) => x.label,
                                            ) || [],
                                    },
                                    dataLabels: { enabled: false },
                                    yaxis: {
                                        labels: {
                                            formatter: (v) =>
                                                v.toLocaleString("th-TH"),
                                        },
                                    },
                                }}
                                series={[
                                    {
                                        name: "รายรับ",
                                        data:
                                            analytics.data?.monthlyRevenue.map(
                                                (x) => x.amount,
                                            ) || [],
                                    },
                                    {
                                        name: "รายจ่ายและคืนเงิน",
                                        data:
                                            analytics.data?.monthlyExpenses.map(
                                                (x) => x.amount,
                                            ) || [],
                                    },
                                ]}
                            />
                        )}
                    </section>
                )}
                <section className="panel">
                    <div className="section-title">
                        <h2>กิจกรรมล่าสุด</h2>
                    </div>
                    {!data.recentActivity?.length ? (
                        <p className="empty">ยังไม่มีกิจกรรม</p>
                    ) : (
                        <ol className="activity">
                            {data.recentActivity.map((item) => (
                                <li key={item.id}>
                                    <span>{item.message}</span>
                                    <time>
                                        {show(item.occurredAt, "createdAt")}
                                    </time>
                                </li>
                            ))}
                        </ol>
                    )}
                </section>
            </div>
        </>
    );
}

function getActions(resource, row, can) {
    const key = resource.key;
    const action = (
        label,
        path,
        permission,
        fields = [],
        method = "POST",
        body,
    ) => ({ label, path, permission, fields, method, body });
    let actions = [];
    if (key === "reservations" && row.status === "Pending")
        actions = [
            ...(!row.confirmedAt
                ? [action("ยืนยันการจอง", "confirm", "reservations.create")]
                : []),
            action("ยกเลิกการจอง", "cancel", "reservations.cancel"),
        ];
    if (key === "contracts") {
        if (row.status === "Draft")
            actions.push(
                action(
                    "เข้าอยู่ / เปิดใช้สัญญา",
                    "activate",
                    "contracts.activate",
                ),
            );
        if (row.status === "Active")
            actions.push(
                action("ต่ออายุสัญญา", "renew", "contracts.create", [
                    field("endDate", "DateOnly"),
                    field("monthlyRent", "decimal", true),
                    field("waterRatePerUnit", "decimal", true),
                    field("electricityRatePerUnit", "decimal", true),
                ]),
                action(
                    "ย้ายออก / สิ้นสุดสัญญา",
                    "terminate",
                    "contracts.terminate",
                    [field("reason")],
                ),
            );
    }
    if (key === "invoices" && row.status === "Draft")
        actions = [
            action("ออกใบแจ้งหนี้", "issue", "invoices.issue"),
            action("เพิ่มค่าใช้จ่าย", "charges", "invoices.create", [
                field("description"),
                field("amount", "decimal"),
            ]),
            action("ยกเลิกใบแจ้งหนี้", "cancel", "invoices.cancel"),
        ];
    if (key === "payments" && row.status === "Pending")
        actions = [action("ยืนยันรับชำระ", "confirm", "payments.approve")];
    if (key === "payments" && row.status === "Confirmed")
        actions = [
            action("คืนเงิน", "refund", "payments.refund", [field("reason")]),
        ];
    if (key === "parcels" && row.status === "Received")
        actions = [
            action("รับพัสดุ", "pickup", "parcels.pickup", [
                field("pickedUpByName"),
            ]),
            action("คืนพัสดุ", "return", "parcels.pickup"),
        ];
    if (key === "announcements") {
        if (row.status === "Draft")
            actions.push(action("เผยแพร่", "publish", "announcements.publish"));
        if (row.status === "Published")
            actions.push(
                action("เก็บถาวร", "archive", "announcements.publish"),
            );
        if (["Draft", "Published"].includes(row.status))
            actions.push(
                action("ยกเลิกประกาศ", "cancel", "announcements.publish"),
            );
    }
    if (key === "maintenance-requests") {
        if (!["Completed", "Cancelled"].includes(row.status))
            actions = [
                action("มอบหมายช่าง", "assign", "maintenance.assign", [
                    field("technicianId", "Guid"),
                ]),
                action(
                    "เปลี่ยนสถานะ",
                    "status",
                    "maintenance.update-status",
                    [
                        field("status", "string", false, {
                            options: ["InProgress", "Completed", "Cancelled"],
                        }),
                    ],
                    "PATCH",
                ),
                action(
                    "เพิ่มบันทึกการทำงาน",
                    "timeline",
                    "maintenance.update-status",
                    [field("note")],
                ),
            ];
        actions.push(
            action("บันทึกค่าใช้จ่ายจริง", "costs", "expenses.create", [
                field("amount", "decimal"),
                field("description"),
            ]),
        );
    }
    if (key === "rooms")
        actions = [
            action(
                "เปลี่ยนสถานะห้อง",
                "status",
                "rooms.change-status",
                [
                    field("status", "string", false, {
                        options: [
                            "Available",
                            "Cleaning",
                            "Maintenance",
                            "Inactive",
                        ],
                    }),
                ],
                "PATCH",
            ),
        ];
    if (key === "billing-periods" && !row.isClosed)
        actions = [
            action("สร้างบิลทั้งรอบ", "generate", "invoices.create"),
            action("ปิดรอบบิล", "close", "invoices.issue"),
        ];
    if (key === "notifications" && !row.isRead)
        actions = [action("ทำเครื่องหมายอ่านแล้ว", "read", null)];
    if (key === "roles")
        actions = [
            action(
                "กำหนดสิทธิ์",
                "permissions",
                "roles.manage",
                [field("permissions", "string[]", true, { array: true })],
                "POST",
            ),
        ];
    if (key === "users")
        actions = [
            action(
                "กำหนดบทบาท",
                "roles",
                "users.manage",
                [
                    field("roles", "string[]", false, {
                        array: true,
                        options: [
                            "Owner",
                            "DormManager",
                            "Staff",
                            "Technician",
                            "Accountant",
                        ],
                    }),
                ],
                "POST",
            ),
            action(
                "กำหนดหอพัก",
                "dormitories",
                "users.manage",
                [
                    field("dormitoryId", "Guid"),
                    field("isPrimary", "bool", true),
                ],
                "POST",
            ),
        ];
    return actions.filter((a) => !a.permission || can(a.permission));
}

function Resource({ resource: r, can, changed }) {
    const [query, setQuery] = useState(""),
        [search, setSearch] = useState(""),
        [status, setStatus] = useState(""),
        [currentOnly, setCurrentOnly] = useState(false),
        [page, setPage] = useState(1),
        [revision, setRevision] = useState(0),
        [editor, setEditor] = useState(null),
        [detail, setDetail] = useState(null),
        [toast, setToast] = useState(""),
        [error, setError] = useState("");
    useEffect(() => {
        const timer = setTimeout(() => {
            setSearch(query);
            setPage(1);
        }, 250);
        return () => clearTimeout(timer);
    }, [query]);
    const url = `${r.key}?page=${page}&pageSize=20&search=${encodeURIComponent(search)}${status ? `&status=${status}` : ""}${currentOnly ? "&currentOnly=true" : ""}`;
    const { data, loading, error: loadError } = useData(url, revision);
    const rows = Array.isArray(data) ? data.filter(row=>!search || Object.values(row).some(value=>String(value).toLowerCase().includes(search.toLowerCase()))) : data?.items || [];
    const refresh = () => {
        setRevision((x) => x + 1);
        changed();
    };
    async function perform(path, method, body) {
        await api(path, { method, body });
        setEditor(null);
        setDetail(null);
        setToast("บันทึกเรียบร้อยแล้ว");
        refresh();
    }
    const canCreate =
        (r.canCreate && can(createPermission[r.key] || `${r.key}.create`)) ||
        (r.key === "billing-periods" && can("invoices.create")) ||
        (r.key === "settings" && can("settings.manage"));
    const openAction = (row, a) =>
        setEditor({
            title: a.label,
            fields: a.fields,
            initial: {},
            onSubmit: (body) =>
                perform(
                    `${r.key}/${row.id}/${a.path}`,
                    a.method,
                    a.fields.length ? body : a.body,
                ),
        });
    if (editor?.custom === "allocation")
        return (
            <Allocation
                onCancel={() => setEditor(null)}
                onDone={() => {
                    setEditor(null);
                    refresh();
                    setToast("รับชำระและออกใบเสร็จเรียบร้อยแล้ว");
                }}
            />
        );
    if (editor)
        return (
            <Editor
                key={editor.title}
                {...editor}
                resource={r.key}
                onCancel={() => setEditor(null)}
            />
        );
    if (detail)
        return (
            <Detail
                resource={r}
                row={detail}
                can={can}
                back={() => setDetail(null)}
                edit={() =>
                    setEditor({
                        title: `แก้ไข${r.title}`,
                        fields: r.key === "settings" ? [field("value")] : r.updateFields,
                        initial: detail,
                        onSubmit: (body) =>
                            perform(`${r.key}/${r.key === "settings" ? encodeURIComponent(detail.key) : detail.id}`, "PUT", body),
                    })
                }
                action={(a) => openAction(detail, a)}
                revision={revision}
            />
        );
    return (
        <>
            <div className="page-heading">
                <div>
                    <h1>{r.title}</h1>
                    <p>
                        {r.key === "rooms"
                            ? "ค้นหาห้อง ดูสถานะ และจัดการการเข้าพัก"
                            : `จัดการ${r.title}และดูรายละเอียดในที่เดียว`}
                    </p>
                </div>
                <div className="heading-actions">
                    {r.key === "notifications" && (
                        <Button
                            variant="outline-secondary"
                            onClick={() =>
                                perform("notifications/read-all", "POST").catch(
                                    (e) => setError(e.message),
                                )
                            }
                        >
                            อ่านทั้งหมด
                        </Button>
                    )}
                    {r.key === "payments" && can("payments.approve") && (
                        <Button
                            variant="outline-secondary"
                            onClick={() =>
                                setEditor({
                                    title: "จัดสรรยอดชำระหลายบิล",
                                    fields: [],
                                    custom: "allocation",
                                })
                            }
                        >
                            ชำระหลายบิล
                        </Button>
                    )}
                    {canCreate && (
                        <Button
                            onClick={() =>
                                setEditor({
                                    title: `เพิ่ม${r.title}`,
                                    fields:
                                        r.key === "billing-periods"
                                            ? [
                                                  field("dormitoryId", "Guid"),
                                                  field("month", "DateOnly"),
                                              ]
                            : r.key === "settings" ? [field("organizationId","Guid",true),field("key"),field("value")] : r.fields,
                                    initial: {},
                                    onSubmit: (body) =>
                                    r.key === "settings" ? perform(`settings/${encodeURIComponent(body.key)}`,"PUT",body) : perform(r.key, "POST", body),
                                })
                            }
                        >
                            <Plus size={17} /> เพิ่ม{r.title}
                        </Button>
                    )}
                </div>
            </div>
            {toast && (
                <Alert
                    variant="success"
                    dismissible
                    onClose={() => setToast("")}
                >
                    {toast}
                </Alert>
            )}
            {error && <Problem error={error} />}
            <section className="panel list-panel">
                <div className="list-toolbar">
                    <div className="search-input">
                        <Search size={18} />
                        <Form.Control
                            aria-label={`ค้นหา${r.title}`}
                            placeholder={`ค้นหา${r.title}…`}
                            value={query}
                            onChange={(e) => setQuery(e.target.value)}
                        />
                    </div>
                    {r.key === "rooms" && (
                        <Form.Select
                            aria-label="กรองสถานะห้อง"
                            value={status}
                            onChange={(e) => {
                                setStatus(e.target.value);
                                setPage(1);
                            }}
                        >
                            <option value="">ทุกสถานะห้อง</option>
                            {[
                                "Available",
                                "Reserved",
                                "Occupied",
                                "Cleaning",
                                "Maintenance",
                                "Inactive",
                            ].map((s) => (
                                <option key={s} value={s}>
                                    {states[s]}
                                </option>
                            ))}
                        </Form.Select>
                    )}
                    {r.key === "tenants" && <Form.Check id="current-tenants" label="ผู้เช่าปัจจุบัน" checked={currentOnly} onChange={e=>{setCurrentOnly(e.target.checked);setPage(1);}}/>}
                    <Button
                        variant="outline-secondary"
                        aria-label="โหลดรายการใหม่"
                        onClick={refresh}
                    >
                        <RefreshCw size={16} />
                    </Button>
                </div>
                {loading ? (
                    <Loading />
                ) : loadError ? (
                    <Problem error={loadError} retry={refresh} />
                ) : !rows.length ? (
                    <div className="empty">
                        <FileText size={34} />
                        <h2>
                            ยังไม่มีรายการ{search ? "ที่ตรงกับการค้นหา" : ""}
                        </h2>
                        <p>
                            {search
                                ? "ลองเปลี่ยนคำค้นหรือสถานะ"
                                : "เพิ่มรายการแรกเพื่อเริ่มใช้งาน"}
                        </p>
                        {search && (
                            <Button
                                variant="outline-secondary"
                                onClick={() => {
                                    setQuery("");
                                    setStatus("");
                                }}
                            >
                                ล้างการค้นหา
                            </Button>
                        )}
                    </div>
                ) : (
                    <div className="table-wrap">
                        <table>
                            <thead>
                                <tr>
                                    {r.columns.map((c) => (
                                        <th key={c}>{labels[c] || c}</th>
                                    ))}
                                    <th>
                                        <span className="visually-hidden">
                                            รายละเอียด
                                        </span>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                {rows.map((row) => (
                                    <tr key={row.id || row.key}>
                                        {r.columns.map((c) => (
                                            <td key={c}>
                                                {c === "status" ? (
                                                    <Status value={row[c]} />
                                                ) : c.endsWith("Id") ? (
                                                    <RelatedName
                                                        id={row[c]}
                                                        fieldKey={c}
                                                    />
                                                ) : (
                                                    show(row[c], c)
                                                )}
                                            </td>
                                        ))}
                                        <td>
                                            <button
                                                className="detail-button"
                                                aria-label={`รายละเอียด ${recordName(row)}`}
                                                onClick={async () => {
                                                    try {
                                                        const full =
                                                            r.key ===
                                                                "billing-periods" ||
                                                            r.key ===
                                                                "notifications" ||
                                                            r.key === "settings"
                                                                ? row
                                                                : await api(
                                                                      `${r.key}/${row.id}`,
                                                                  );
                                                        setDetail(full);
                                                    } catch (e) {
                                                        setError(e.message);
                                                    }
                                                }}
                                            >
                                                รายละเอียด{" "}
                                                <ChevronRight size={15} />
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
                <div className="pagination-row">
                    <span>{data?.totalItems ?? rows.length} รายการ</span>
                    <div>
                        <Button
                            variant="outline-secondary"
                            size="sm"
                            disabled={page <= 1 || loading}
                            aria-label="หน้าก่อน"
                            onClick={() => setPage((x) => x - 1)}
                        >
                            <ChevronLeft size={16} />
                        </Button>
                        <span>
                            หน้า {page} / {Math.max(1, data?.totalPages || 1)}
                        </span>
                        <Button
                            variant="outline-secondary"
                            size="sm"
                            disabled={!data?.hasNextPage || loading}
                            aria-label="หน้าถัดไป"
                            onClick={() => setPage((x) => x + 1)}
                        >
                            <ChevronRight size={16} />
                        </Button>
                    </div>
                </div>
            </section>
        </>
    );
}

const relationResources = {
    roomId: "rooms",
    tenantId: "tenants",
    buildingId: "buildings",
    floorId: "floors",
    dormitoryId: "dormitories",
    invoiceId: "invoices",
    paymentId: "payments",
};
const nameCache = new Map();
function RelatedName({ id, fieldKey }) {
    const [name, setName] = useState(nameCache.get(id));
    useEffect(() => {
        let active = true;
        if (!id || name || !relationResources[fieldKey]) return;
        api(`${relationResources[fieldKey]}/${id}`)
            .then((row) => {
                const value = recordName(row);
                nameCache.set(id, value);
                if (active) setName(value);
            })
            .catch(() => {});
        return () => {
            active = false;
        };
    }, [id, fieldKey, name]);
    return (
        name ||
        (!id
            ? "—"
            : `${labels[fieldKey] || "รายการ"} ${String(id).slice(0, 6)}`)
    );
}
function Detail({ resource: r, row, can, back, edit, action, revision }) {
    const actions = getActions(r, row, can);
    return (
        <>
            <div className="page-heading">
                <div>
                    <Button
                        variant="link"
                        className="back-button"
                        onClick={back}
                    >
                        <ChevronLeft size={16} /> กลับไปที่{r.title}
                    </Button>
                    <h1>{recordName(row)}</h1>
                    {row.status && <Status value={row.status} />}
                </div>
            {(r.canUpdate || r.key === "settings") &&
                    can(
                    r.key === "settings" ? "settings.manage" : r.key === "users"
                            ? "users.manage"
                            : r.key === "roles"
                              ? "roles.manage"
                              : `${r.key}.update`,
                    ) && (
                        <Button variant="outline-secondary" onClick={edit}>
                            แก้ไขข้อมูล
                        </Button>
                    )}
            </div>
            {actions.length > 0 && (
                <div className="record-actions">
                    {actions.map((a) => (
                        <Button
                            variant="outline-primary"
                            key={a.path}
                            onClick={() => action(a)}
                        >
                            {a.label}
                        </Button>
                    ))}
                </div>
            )}
            <section className="panel">
                <h2>รายละเอียด{r.title}</h2>
                <dl className="detail-grid">
                    {Object.entries(row)
                        .filter(
                            ([key]) =>
                                !["id", "organizationId", "version"].includes(
                                    key,
                                ) && !key.startsWith("floorPlan"),
                        )
                        .map(([key, value]) => (
                            <div key={key}>
                                <dt>{labels[key] || key}</dt>
                                <dd>
                                    {relationResources[key] ? (
                                        <RelatedName
                                            id={value}
                                            fieldKey={key}
                                        />
                                    ) : (
                                        show(value, key)
                                    )}
                                </dd>
                            </div>
                        ))}
                </dl>
                {r.key === "receipts" && (
                    <Button onClick={() => window.print()}>พิมพ์ใบเสร็จ</Button>
                )}
            </section>
            {r.key === "maintenance-requests" && (
                <RelatedList
                    title="Timeline การทำงาน"
                    path={`${r.key}/${row.id}/timeline`}
                    revision={revision}
                />
            )}{" "}
            {r.key === "contracts" && (
                <RelatedList
                    title="ประวัติต่อสัญญา"
                    path={`contracts/${row.id}/renewals`}
                    revision={revision}
                />
            )}{" "}
            {r.key === "tenants" && can("contracts.view") && (
                <RelatedList
                    title="ประวัติการเข้าพัก"
                    path={`tenants/${row.id}/history`}
                    revision={revision}
                />
            )}{" "}
            {r.key === "invoices" && (
                <RelatedList
                    title="รายการค่าใช้จ่ายเพิ่มเติม"
                    path={`invoices/${row.id}/charges`}
                    revision={revision}
                />
            )}
        </>
    );
}
function RelatedList({ title, path, revision }) {
    const { data, loading, error } = useData(path, revision);
    return (
        <section className="panel related-panel">
            <h2>{title}</h2>
            {loading ? (
                <Loading />
            ) : error ? (
                <Problem error={error} />
            ) : !data?.length ? (
                <p className="muted">ยังไม่มีรายการ</p>
            ) : (
                <div className="table-wrap">
                    <table>
                        <thead>
                            <tr>
                                {Object.keys(data[0])
                                    .filter((k) => k !== "id")
                                    .map((k) => (
                                        <th key={k}>{labels[k] || k}</th>
                                    ))}
                            </tr>
                        </thead>
                        <tbody>
                            {data.map((row, i) => (
                                <tr key={row.id || i}>
                                    {Object.entries(row)
                                        .filter(([k]) => k !== "id")
                                        .map(([k, v]) => (
                                            <td key={k}>{show(v, k)}</td>
                                        ))}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </section>
    );
}

function Reports() {
    const [report, setReport] = useState("cash-flow"),
        [from, setFrom] = useState(today().slice(0, 4) + "-01-01"),
        [to, setTo] = useState(today()),
        [revision, setRevision] = useState(0),
        [error, setError] = useState("");
    const query = `from=${from}&to=${to}`;
    const result = useData(`reports/detail/${report}?${query}`, revision),
        analytics = useData(`analytics?${query}`, revision);
    return (
        <>
            <div className="page-heading">
                <div>
                    <h1>รายงานและวิเคราะห์</h1>
                    <p>ติดตามผลการดำเนินงานจากข้อมูลที่บันทึกจริง</p>
                </div>
                <Button
                    variant="outline-primary"
                    onClick={() =>
                        download(
                            `reports/detail/${report}/export?${query}`,
                            `${report}.csv`,
                        ).catch((e) => setError(e.message))
                    }
                >
                    <Download size={17} /> ส่งออก CSV
                </Button>
            </div>
            <section className="panel report-filters">
                <Form.Group>
                    <Form.Label htmlFor="report-type">รายงาน</Form.Label>
                    <Form.Select
                        id="report-type"
                        value={report}
                        onChange={(e) => setReport(e.target.value)}
                    >
                        {reports.map(([value, label]) => (
                            <option key={value} value={value}>
                                {label}
                            </option>
                        ))}
                    </Form.Select>
                </Form.Group>
                <Form.Group>
                    <Form.Label htmlFor="report-from">ตั้งแต่วันที่</Form.Label>
                    <Form.Control
                        id="report-from"
                        type="date"
                        value={from}
                        max={to}
                        onChange={(e) => setFrom(e.target.value)}
                    />
                </Form.Group>
                <Form.Group>
                    <Form.Label htmlFor="report-to">ถึงวันที่</Form.Label>
                    <Form.Control
                        id="report-to"
                        type="date"
                        value={to}
                        min={from}
                        onChange={(e) => setTo(e.target.value)}
                    />
                </Form.Group>
                <Button
                    variant="outline-secondary"
                    onClick={() => setRevision((x) => x + 1)}
                >
                    อัปเดต
                </Button>
            </section>
            {error && <Problem error={error} />}{" "}
            {analytics.data && (
                <section className="panel">
                    <div className="section-title">
                        <h2>แนวโน้มรายเดือน</h2>
                        <span className="muted">
                            ตามวันที่รับ–จ่ายจริง รวมเงินคืนในรายจ่าย
                        </span>
                    </div>
                    <Chart
                        type="line"
                        height={280}
                        options={{
                            chart: {
                                toolbar: { show: false },
                                fontFamily: "Noto Sans Thai",
                            },
                            colors: ["#216650", "#b27c35"],
                            stroke: { width: 3, curve: "straight" },
                            xaxis: {
                                categories: analytics.data.monthlyRevenue.map(
                                    (r) => r.label,
                                ),
                            },
                            dataLabels: { enabled: false },
                            yaxis: {
                                labels: {
                                    formatter: (v) => v.toLocaleString("th-TH"),
                                },
                            },
                        }}
                        series={[
                            {
                                name: "รายรับ",
                                data: analytics.data.monthlyRevenue.map(
                                    (r) => r.amount,
                                ),
                            },
                            {
                                name: "รายจ่ายและคืนเงิน",
                                data: analytics.data.monthlyExpenses.map(
                                    (r) => r.amount,
                                ),
                            },
                        ]}
                    />
                    <div className="report-summary">
                        <span>
                            รายรับ <b>{money(analytics.data.revenue)}</b>
                        </span>
                        <span>
                            รายจ่ายและคืนเงิน{" "}
                            <b>{money(analytics.data.expenses)}</b>
                        </span>
                        <span>
                            เงินสุทธิ{" "}
                            <b>{money(analytics.data.netOperatingCash)}</b>
                        </span>
                    </div>
                </section>
            )}
            <section className="panel related-panel">
                <h2>{reports.find(([key]) => key === report)?.[1]}</h2>
                {result.loading ? (
                    <Loading />
                ) : result.error ? (
                    <Problem error={result.error} />
                ) : !result.data?.rows.length ? (
                    <p className="empty">ไม่มีข้อมูลในช่วงวันที่เลือก</p>
                ) : (
                    <div className="table-wrap">
                        <table>
                            <thead>
                                <tr>
                                    {result.data.columns.map((c) => (
                                        <th key={c}>{reportColumn(c)}</th>
                                    ))}
                                </tr>
                            </thead>
                            <tbody>
                                {result.data.rows.map((row, i) => (
                                    <tr key={i}>
                                        {row.map((cell, j) => (
                                            <td key={j}>
                                                {states[cell] || cell}
                                            </td>
                                        ))}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
                <small className="muted">
                    แสดงรายละเอียดสูงสุด 5,000 รายการต่อช่วงวันที่ ·
                    รายงานสถานะห้องและยอดค้างแสดงสถานะปัจจุบัน
                </small>
            </section>
            {analytics.data?.paymentMethods.length > 0 && (
                <section className="panel related-panel">
                    <h2>ช่องทางการรับชำระ</h2>
                    <Chart
                        type="donut"
                        height={280}
                        options={{
                            labels: analytics.data.paymentMethods.map(
                                (r) => states[r.label] || r.label,
                            ),
                            colors: [
                                "#216650",
                                "#88ad97",
                                "#d6a459",
                                "#a2b3ac",
                            ],
                            chart: { fontFamily: "Noto Sans Thai" },
                            legend: { position: "bottom" },
                        }}
                        series={analytics.data.paymentMethods.map(
                            (r) => r.amount,
                        )}
                    />
                </section>
            )}
        </>
    );
}
function reportColumn(value) {
    return (
        {
            Month: "เดือน",
            Revenue: "รายรับ",
            Outflows: "รายจ่ายและคืนเงิน",
            Net: "เงินสุทธิ",
            Payment: "รายการชำระ",
            Invoice: "ใบแจ้งหนี้",
            Date: "วันที่",
            Method: "ช่องทาง",
            Amount: "จำนวนเงิน",
            Status: "สถานะ",
            Category: "หมวดหมู่",
            Description: "รายละเอียด",
            Maintenance: "งานซ่อม",
            "Due date": "ครบกำหนด",
            Total: "ยอดรวม",
            Paid: "ชำระแล้ว",
            Outstanding: "ยอดค้าง",
            Room: "ห้อง",
            "Monthly rent": "ค่าเช่า",
            "Collected in period": "รับชำระในช่วงวันที่",
            Contract: "สัญญา",
            Tenant: "ผู้เช่า",
            Start: "เริ่ม",
            End: "สิ้นสุด",
            "Move in": "เข้าอยู่",
            "Move out": "ย้ายออก",
            Reservation: "รายการจอง",
            Created: "สร้างเมื่อ",
            Job: "งานซ่อม",
            Priority: "ความสำคัญ",
            "Actual cost": "ค่าใช้จ่ายจริง",
            Tracking: "เลขพัสดุ",
            Received: "วันที่รับ",
        }[value] || value
    );
}

function Allocation({ onCancel, onDone }) {
    const [items, setItems] = useState([{ invoiceId: "", amount: "" }]),
        [method, setMethod] = useState("Cash"),
        [reference, setReference] = useState(""),
        [error, setError] = useState(""),
        [busy, setBusy] = useState(false),
        [key] = useState(() => crypto.randomUUID());
    const total = items.reduce((sum, row) => sum + Number(row.amount || 0), 0);
    async function submit(e) {
        e.preventDefault();
        setBusy(true);
        setError("");
        try {
            await api("payments/allocate", {
                method: "POST",
                body: {
                    idempotencyKey: key,
                    amount: Math.round(total * 100) / 100,
                    method,
                    paymentDate: new Date().toISOString(),
                    referenceNumber: reference || null,
                    allocations: items.map((row) => ({
                        invoiceId: row.invoiceId,
                        amount: Number(row.amount),
                    })),
                },
            });
            onDone();
        } catch (e) {
            setError(e.message);
        } finally {
            setBusy(false);
        }
    }
    return (
        <section className="editor">
            <div className="section-title">
                <h2>จัดสรรยอดชำระหลายบิล</h2>
                <Button variant="link" onClick={onCancel}>
                    กลับไปที่รายการ
                </Button>
            </div>
            <p className="muted">
                เลือกบิลของผู้เช่าคนเดียวกัน ระบุยอดที่ต้องการชำระแต่ละบิล
                ระบบจะออกใบเสร็จเมื่อบันทึกสำเร็จ
            </p>
            <Form onSubmit={submit}>
                {items.map((row, i) => (
                    <div key={i} className="allocation-row">
                        <Form.Group>
                            <Form.Label htmlFor={`allocation-invoice-${i}`}>
                                ใบแจ้งหนี้ {i + 1}
                            </Form.Label>
                            <Lookup
                                id={`allocation-invoice-${i}`}
                                label="ใบแจ้งหนี้"
                                resource="invoices"
                                value={row.invoiceId}
                                required
                                onChange={(value) =>
                                    setItems((all) =>
                                        all.map((x, j) =>
                                            j === i
                                                ? { ...x, invoiceId: value }
                                                : x,
                                        ),
                                    )
                                }
                            />
                        </Form.Group>
                        <Form.Group>
                            <Form.Label htmlFor={`allocation-amount-${i}`}>
                                ยอดชำระ
                            </Form.Label>
                            <Form.Control
                                id={`allocation-amount-${i}`}
                                type="number"
                                required
                                min="0.01"
                                step="0.01"
                                value={row.amount}
                                onChange={(e) =>
                                    setItems((all) =>
                                        all.map((x, j) =>
                                            j === i
                                                ? {
                                                      ...x,
                                                      amount: e.target.value,
                                                  }
                                                : x,
                                        ),
                                    )
                                }
                            />
                        </Form.Group>
                        <Button
                            variant="link"
                            aria-label={`ลบบิล ${i + 1}`}
                            disabled={items.length === 1}
                            onClick={() =>
                                setItems((all) => all.filter((_, j) => j !== i))
                            }
                        >
                            <X size={18} />
                        </Button>
                    </div>
                ))}
                <Button
                    variant="outline-secondary"
                    disabled={items.length >= 100}
                    onClick={() =>
                        setItems((all) => [
                            ...all,
                            { invoiceId: "", amount: "" },
                        ])
                    }
                >
                    <Plus size={16} /> เพิ่มบิล
                </Button>
                <div className="form-grid">
                    <Form.Group>
                        <Form.Label htmlFor="allocation-method">
                            ช่องทางชำระ
                        </Form.Label>
                        <Form.Select
                            id="allocation-method"
                            value={method}
                            onChange={(e) => setMethod(e.target.value)}
                        >
                            {["Cash", "BankTransfer", "QrCode", "Other"].map(
                                (v) => (
                                    <option key={v} value={v}>
                                        {states[v]}
                                    </option>
                                ),
                            )}
                        </Form.Select>
                    </Form.Group>
                    <Form.Group>
                        <Form.Label htmlFor="allocation-reference">
                            เลขอ้างอิง
                        </Form.Label>
                        <Form.Control
                            id="allocation-reference"
                            value={reference}
                            onChange={(e) => setReference(e.target.value)}
                        />
                    </Form.Group>
                </div>
                <p className="allocation-total">
                    ยอดรับชำระทั้งหมด {money(total)}
                </p>
                {error && <Problem error={error} />}
                <div className="form-actions">
                    <Button type="submit" disabled={busy || total <= 0}>
                        {busy ? "กำลังบันทึก…" : "ยืนยันรับชำระและออกใบเสร็จ"}
                    </Button>
                    <Button
                        variant="outline-secondary"
                        disabled={busy}
                        onClick={onCancel}
                    >
                        ยกเลิก
                    </Button>
                </div>
            </Form>
        </section>
    );
}
