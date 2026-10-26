import { useEffect, useState } from "react";
import { Form, Button, Alert, Spinner } from "react-bootstrap";
import { api, session } from "./api";
import {
    labels,
    choices,
    relations,
    states,
    recordName,
    today,
} from "./labels";

export const field = (key, type = "string", optional = false, extra = {}) => ({
    key,
    type,
    optional,
    ...extra,
});
export function Lookup({
    resource,
    multiple,
    value,
    onChange,
    required,
    label,
    id,
    describedBy,
    invalid,
}) {
    const [query, setQuery] = useState(""),
        [options, setOptions] = useState([]),
        [error, setError] = useState(""),
        [loading, setLoading] = useState(false);
    useEffect(() => {
        const controller = new AbortController();
        const timer = setTimeout(() => {
            setLoading(true);
            api(
                `${resource}?pageSize=100&search=${encodeURIComponent(query)}`,
                { signal: controller.signal },
            )
                .then((data) => {
                    setOptions(data?.items || data || []);
                    setError("");
                })
                .catch((e) => {
                    if (e.name !== "AbortError") setError(e.message);
                })
                .finally(() => {
                    if (!controller.signal.aborted) setLoading(false);
                });
        }, 200);
        return () => {
            clearTimeout(timer);
            controller.abort();
        };
    }, [resource, query]);
    return (
        <div className="lookup">
            <Form.Control
                aria-label={`ค้นหา${label}`}
                placeholder={`ค้นหา${label}…`}
                value={query}
                onChange={(e) => setQuery(e.target.value)}
            />
            <Form.Select
                id={id}
                aria-describedby={describedBy || (error ? `${id}-lookup-error` : undefined)}
                aria-invalid={!!invalid || !!error}
                isInvalid={!!invalid || !!error}
                multiple={multiple}
                required={required}
                value={value || (multiple ? [] : "")}
                onChange={(e) =>
                    onChange(
                        multiple
                            ? Array.from(e.target.selectedOptions).map(
                                  (o) => o.value,
                              )
                            : e.target.value,
                    )
                }
            >
                <option value="" disabled={multiple}>
                    เลือก{label}
                </option>
                {options.map((row) => (
                    <option key={row.id} value={row.id}>
                        {recordName(row)}
                    </option>
                ))}
                {!multiple && value && !options.some((o) => o.id === value) && (
                    <option value={value}>รายการที่เลือกไว้</option>
                )}
            </Form.Select>
            {loading && <small>กำลังค้นหา…</small>}
            {error && <small id={`${id}-lookup-error`} className="text-danger">{error}</small>}
            {options.length === 100 && (
                <small>พิมพ์ชื่อเพื่อค้นหารายการเพิ่มเติม</small>
            )}
        </div>
    );
}
export function Editor({
    title,
    fields: providedFields,
    initial = {},
    onSubmit,
    onCancel,
    resource = "",
    submitLabel = "บันทึก",
}) {
    const isSuperAdmin = session()?.user?.roles?.includes("SuperAdmin") === true;
    const fields = providedFields
        .filter(
            (f) => f.key !== "organizationId" || isSuperAdmin,
        )
        .map((f) =>
            f.key === "organizationId" && isSuperAdmin
                ? { ...f, optional: false }
                : f,
        );
    const [values, setValues] = useState(() =>
        Object.fromEntries(
            fields.map((f) => [
                f.key,
                initial[f.key] ??
                    (f.array
                        ? []
                        : f.optional
                          ? ""
                          : /DateOnly/.test(f.type)
                            ? today()
                            : f.type === "DateTimeOffset"
                              ? new Date().toISOString().slice(0, 16)
                              : f.key === "holdDays"
                                ? 3
                                : f.key === "maximumOccupants"
                                  ? 1
                                  : choices[f.key]?.[0] || ""),
            ]),
        ),
    );
    const [busy, setBusy] = useState(false),
        [error, setError] = useState(""),
        [errors, setErrors] = useState({});
    async function submit(e) {
        e.preventDefault();
        setError("");
        setErrors({});
        setBusy(true);
        try {
            const body = {};
            for (const f of fields) {
                let v = values[f.key];
                if (v === "" || v == null) {
                    if (f.optional) body[f.key] = null;
                    continue;
                }
                if (/decimal|int|double/.test(f.type)) v = Number(v);
                if (f.type.replace("?", "") === "DateTimeOffset" && v)
                    v = new Date(v).toISOString();
                if (f.array && typeof v === "string")
                    v = v
                        .split(",")
                        .map((x) => x.trim())
                        .filter(Boolean);
                body[f.key] = v;
            }
            await onSubmit(body);
        } catch (e) {
            setError(e.message);
            setErrors(e.fields || {});
        } finally {
            setBusy(false);
        }
    }
    return (
        <section className="editor">
            <div className="section-title">
                <h2>{title}</h2>
                <Button variant="link" onClick={onCancel}>
                    กลับไปที่รายการ
                </Button>
            </div>
            <Form onSubmit={submit}>
                {fields.length === 0 && <p className="muted">กดยืนยันเพื่อทำรายการนี้ ระบบจะตรวจสอบสถานะล่าสุดก่อนบันทึก</p>}
                <div className="form-grid">
                    {fields.map((f) => {
                        const label = labels[f.key] || f.key,
                            id = `field-${f.key}`;
                        let options = f.options || choices[f.key];
                        if (
                            f.key === "category" &&
                            resource === "maintenance-requests"
                        )
                            options = [
                                "General",
                                "Water",
                                "Electricity",
                                "Other",
                            ];
                        if (
                            f.key === "priority" &&
                            resource === "maintenance-requests"
                        )
                            options = ["Low", "Medium", "High", "Urgent"];
                        return (
                            <Form.Group
                                key={f.key}
                                className={
                                    /description|content|notes|note/.test(f.key)
                                        ? "wide"
                                        : ""
                                }
                            >
                                <Form.Label htmlFor={id}>
                                    {label}
                                    {!f.optional && (
                                        <span className="required"> *</span>
                                    )}
                                </Form.Label>
                                {relations[f.key] ? (
                                    <Lookup
                                        id={id}
                                        describedBy={errors[f.key] ? `${id}-error` : undefined}
                                        invalid={!!errors[f.key]}
                                        label={label}
                                        resource={relations[f.key]}
                                        multiple={f.array}
                                        value={values[f.key]}
                                        required={!f.optional && !f.array}
                                        onChange={(v) =>
                                            setValues((x) => ({
                                                ...x,
                                                [f.key]: v,
                                            }))
                                        }
                                    />
                                ) : options ? (
                                    <Form.Select
                                        id={id}
                                        aria-invalid={!!errors[f.key]}
                                        aria-describedby={errors[f.key] ? `${id}-error` : undefined}
                                        multiple={f.array}
                                        value={values[f.key]}
                                        required={!f.optional}
                                        onChange={(e) =>
                                            setValues((x) => ({
                                                ...x,
                                                [f.key]: f.array
                                                    ? Array.from(
                                                          e.target
                                                              .selectedOptions,
                                                      ).map((o) => o.value)
                                                    : e.target.value,
                                            }))
                                        }
                                    >
                                        {!f.array && (
                                            <option value="">
                                                เลือก{label}
                                            </option>
                                        )}
                                        {options.map((v) => (
                                            <option value={v} key={v}>
                                                {states[v] || v}
                                            </option>
                                        ))}
                                    </Form.Select>
                                ) : f.type === "bool" ? (
                                    <Form.Check
                                        id={id}
                                        checked={!!values[f.key]}
                                        onChange={(e) =>
                                            setValues((x) => ({
                                                ...x,
                                                [f.key]: e.target.checked,
                                            }))
                                        }
                                    />
                                ) : (
                                    <Form.Control
                                        id={id}
                                        aria-invalid={!!errors[f.key]}
                                        aria-describedby={errors[f.key] ? `${id}-error` : undefined}
                                        as={
                                            /description|content|notes|note/.test(
                                                f.key,
                                            )
                                                ? "textarea"
                                                : "input"
                                        }
                                        rows={3}
                                        required={!f.optional}
                                        type={
                                            f.key === "password"
                                                ? "password"
                                                : f.key === "email"
                                                  ? "email"
                                                  : /decimal|int|double/.test(
                                                          f.type,
                                                      )
                                                    ? "number"
                                                    : f.type.replace("?", "") === "DateOnly"
                                                      ? "date"
                                                      : f.type.replace("?", "") ===
                                                          "DateTimeOffset"
                                                        ? "datetime-local"
                                                        : "text"
                                        }
                                        step={
                                            /decimal|double/.test(f.type)
                                                ? "0.01"
                                                : undefined
                                        }
                                        min={
                                            /decimal|int/.test(f.type)
                                                ? 0
                                                : undefined
                                        }
                                        value={values[f.key]}
                                        onChange={(e) =>
                                            setValues((x) => ({
                                                ...x,
                                                [f.key]: e.target.value,
                                            }))
                                        }
                                        isInvalid={!!errors[f.key]}
                                        autoComplete={
                                            f.key === "password"
                                                ? "new-password"
                                                : undefined
                                        }
                                    />
                                )}{" "}
                                {errors[f.key] && (
                                    <div className="field-error" id={`${id}-error`}>
                                        {errors[f.key].join(" ")}
                                    </div>
                                )}
                            </Form.Group>
                        );
                    })}
                </div>
                {error && (
                    <Alert variant="danger" role="alert">
                        {error}
                    </Alert>
                )}
                <div className="form-actions">
                    <Button type="submit" disabled={busy}>
                        {busy ? (
                            <>
                                <Spinner size="sm" /> กำลังบันทึก…
                            </>
                        ) : (
                            submitLabel
                        )}
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
