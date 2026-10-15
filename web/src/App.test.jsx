import { beforeEach, describe, expect, it, vi } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import App from "./App";
import { api, saveSession } from "./api";

vi.mock("react-apexcharts", () => ({
    default: () => <div aria-label="กราฟข้อมูล" />,
}));
const ok = (data) =>
    Promise.resolve({ ok: true, status: 200, json: async () => ({ data }) });
const auth = {
    accessToken: "test-access",
    refreshToken: "test-refresh",
    user: {
        firstName: "ทดสอบ",
        fullName: "ผู้ใช้ทดสอบ",
        roles: ["Owner"],
        permissions: ["rooms.view", "rooms.create"],
        email: "test@example.com",
    },
};
describe("DormFlow", () => {
    beforeEach(() => {
        saveSession(null);
        location.hash = "";
        vi.restoreAllMocks();
    });
    it("logs in through the API with labeled credentials", async () => {
        const fetch = vi.fn((url) =>
            String(url).endsWith("auth/login")
                ? ok(auth)
                : ok({ unreadCount: 0, items: [] }),
        );
        vi.stubGlobal("fetch", fetch);
        render(<App />);
        await userEvent.type(
            screen.getByLabelText("อีเมล"),
            "test@example.com",
        );
        await userEvent.type(
            screen.getByLabelText("รหัสผ่าน"),
            "example-password",
        );
        await userEvent.click(
            screen.getByRole("button", { name: "เข้าสู่ระบบ" }),
        );
        await waitFor(() =>
            expect(fetch).toHaveBeenCalledWith(
                "/api/v1/auth/login",
                expect.objectContaining({
                    method: "POST",
                    body: JSON.stringify({
                        email: "test@example.com",
                        password: "example-password",
                    }),
                }),
            ),
        );
    });
    it("shows an actionable login error", async () => {
        vi.stubGlobal(
            "fetch",
            vi.fn(() =>
                Promise.resolve({
                    ok: false,
                    status: 401,
                    json: async () => ({
                        message: "อีเมลหรือรหัสผ่านไม่ถูกต้อง",
                    }),
                }),
            ),
        );
        render(<App />);
        await userEvent.type(
            screen.getByLabelText("อีเมล"),
            "test@example.com",
        );
        await userEvent.type(screen.getByLabelText("รหัสผ่าน"), "wrong");
        await userEvent.click(
            screen.getByRole("button", { name: "เข้าสู่ระบบ" }),
        );
        expect(
            await screen.findByText("อีเมลหรือรหัสผ่านไม่ถูกต้อง"),
        ).toBeVisible();
    });
    it("only offers menus allowed by permissions", async () => {
        saveSession(auth);
        location.hash = "rooms";
        vi.stubGlobal(
            "fetch",
            vi.fn(() => ok({ items: [], totalItems: 0, totalPages: 0 })),
        );
        render(<App />);
        expect(
            screen.getByRole("button", { name: "Room Center" }),
        ).toBeVisible();
        expect(
            screen.queryByRole("button", { name: "รับชำระเงิน" }),
        ).not.toBeInTheDocument();
        expect(await screen.findByText("ยังไม่มีรายการ")).toBeVisible();
    });
    it("requests the next page from the server", async () => {
        saveSession(auth);
        location.hash = "rooms";
        const fetch = vi.fn((url) =>
            String(url).includes("rooms?")
                ? ok({
                      items: [
                          {
                              id: "room-1",
                              roomNumber: "101",
                              status: "Available",
                          },
                      ],
                      totalItems: 21,
                      totalPages: 2,
                      hasNextPage: !String(url).includes("page=2"),
                  })
                : ok({ unreadCount: 0 }),
        );
        vi.stubGlobal("fetch", fetch);
        render(<App />);
        await screen.findByText("101");
        await userEvent.click(
            screen.getByRole("button", { name: "หน้าถัดไป" }),
        );
        await waitFor(() =>
            expect(fetch).toHaveBeenCalledWith(
                expect.stringContaining("rooms?page=2"),
                expect.anything(),
            ),
        );
    });
    it("refreshes an expired token once before retrying a request", async () => {
        saveSession(auth);
        let first = true;
        const fetch = vi.fn((url) => {
            if (String(url).endsWith("auth/refresh"))
                return ok({ ...auth, accessToken: "new-access" });
            if (first) {
                first = false;
                return Promise.resolve({
                    status: 401,
                    ok: false,
                    json: async () => ({}),
                });
            }
            return ok({ count: 2 });
        });
        vi.stubGlobal("fetch", fetch);
        await expect(api("rooms")).resolves.toEqual({ count: 2 });
        expect(fetch).toHaveBeenLastCalledWith(
            "/api/v1/rooms",
            expect.objectContaining({
                headers: expect.objectContaining({
                    Authorization: "Bearer new-access",
                }),
            }),
        );
    });
    it("keeps API validation errors attached to fields", async () => {
        vi.stubGlobal(
            "fetch",
            vi.fn(() =>
                Promise.resolve({
                    ok: false,
                    status: 400,
                    json: async () => ({
                        message: "ข้อมูลไม่ถูกต้อง",
                        errors: { amount: ["ยอดชำระต้องมากกว่า 0"] },
                    }),
                }),
            ),
        );
        await expect(
            api("payments", { method: "POST", body: { amount: 0 } }),
        ).rejects.toMatchObject({
            fields: { amount: ["ยอดชำระต้องมากกว่า 0"] },
        });
    });
});
