import { test, expect } from "@playwright/test";
import { mkdirSync, writeFileSync } from "node:fs";
import { resolve } from "node:path";

const reviewDirectory = resolve(process.cwd(), "../.impeccable/review");
mkdirSync(reviewDirectory, { recursive: true });

test("real API: tenant → reservation → contract → meter → invoice → payment → receipt; desktop and mobile", async ({
    page,
    request,
}) => {
    const email = process.env.QA_EMAIL,
        password = process.env.QA_PASSWORD;
    test.skip(
        !email || !password,
        "Set QA_EMAIL and QA_PASSWORD for an isolated test database.",
    );
    const errors = [];
    page.on("pageerror", (e) => errors.push(e.message));
    const login = await request.post("/api/v1/auth/login", {
        data: { email, password },
    });
    expect(login.ok()).toBeTruthy();
    const token = (await login.json()).data.accessToken;
    const headers = { Authorization: `Bearer ${token}` };
    const post = async (path, data) => {
        const res = await request.post(`/api/v1/${path}`, { headers, data });
        expect(res.ok(), await res.text()).toBeTruthy();
        return (await res.json()).data;
    };
    const suffix = Date.now().toString().slice(-7);
    const org = await post("organizations", {
        code: `QA${suffix}`,
        name: "องค์กรทดสอบ DormFlow",
    });
    const dorm = await post("dormitories", {
        organizationId: org.id,
        code: `D${suffix}`,
        name: "หอพักทดสอบ",
        billingDay: 1,
        paymentDueDay: 5,
        lateFeeType: "Fixed",
        lateFeeAmount: 0,
        waterRatePerUnit: 18,
        electricityRatePerUnit: 8,
    });
    const building = await post("buildings", {
        dormitoryId: dorm.id,
        code: "A",
        name: "อาคาร A",
        totalFloors: 3,
    });
    const floor = await post("floors", {
        buildingId: building.id,
        floorNumber: 1,
        name: "ชั้น 1",
    });
    const type = await post("room-types", {
        dormitoryId: dorm.id,
        code: "STD",
        name: "ห้องมาตรฐาน",
        monthlyRent: 3500,
        depositAmount: 3500,
        maximumOccupants: 2,
    });
    const room = await post("rooms", {
        buildingId: building.id,
        floorId: floor.id,
        roomTypeId: type.id,
        roomNumber: `QA-${suffix}`,
        monthlyRent: 3500,
        depositAmount: 3500,
        maximumOccupants: 2,
    });
    await page.goto("/");
    await page.getByLabel("อีเมล", { exact: true }).fill(email);
    await page.getByLabel("รหัสผ่าน", { exact: true }).fill(password);
    await page
        .getByRole("button", { name: "เข้าสู่ระบบ", exact: true })
        .click();
    await expect(
        page.getByRole("heading", { name: "ภาพรวมหอพัก" }),
    ).toBeVisible();
    await page.getByRole("button", { name: "ผู้เช่า", exact: true }).click();
    await page
        .getByRole("button", { name: "เพิ่มผู้เช่า", exact: true })
        .click();
    await page.getByLabel("องค์กร", { exact: true }).selectOption(org.id);
    await page.getByLabel("ชื่อ *", { exact: true }).fill("ทดสอบ");
    await page.getByLabel("นามสกุล *", { exact: true }).fill(suffix);
    await page.getByLabel("เบอร์โทร", { exact: true }).fill("0812345678");
    await page.getByRole("button", { name: "บันทึก", exact: true }).click();
    await expect(page.getByText("บันทึกเรียบร้อยแล้ว")).toBeVisible();
    const tenants = await request.get(`/api/v1/tenants?search=${suffix}`, {
        headers,
    });
    const tenant = (await tenants.json()).data.items[0];
    expect(tenant).toBeTruthy();
    const date = new Date().toISOString().slice(0, 10),
        end = `${Number(date.slice(0, 4)) + 1}${date.slice(4)}`;
    const reservation = await post("reservations", {
        roomId: room.id,
        tenantId: tenant.id,
        moveInDate: date,
        holdDays: 3,
        reservationFee: 0,
    });
    const contract = await post("contracts", {
        roomId: room.id,
        tenantId: tenant.id,
        reservationId: reservation.id,
        startDate: date,
        endDate: end,
        waterRatePerUnit: 12,
        electricityRatePerUnit: 6,
    });
    await page.getByRole("button", { name: "สัญญาเช่า", exact: true }).click();
    await page
        .getByRole("row")
        .filter({ hasText: contract.contractNumber })
        .getByRole("button", { name: /รายละเอียด/ })
        .click();
    await page
        .getByRole("button", { name: "เข้าอยู่ / เปิดใช้สัญญา", exact: true })
        .click();
    await page.getByRole("button", { name: "บันทึก", exact: true }).click();
    await expect(page.getByText("บันทึกเรียบร้อยแล้ว")).toBeVisible();
    await post("meter-readings", {
        roomId: room.id,
        meterType: "Water",
        periodMonth: date,
        readingDate: date,
        currentReading: 10,
    });
    const invoice = await post("invoices", {
        contractId: contract.id,
        periodMonth: date,
    });
    expect(invoice.waterAmount).toBe(120);
    await post(`invoices/${invoice.id}/issue`);
    await page
        .getByRole("button", { name: "รับชำระเงิน", exact: true })
        .click();
    await page
        .getByRole("button", { name: "เพิ่มรับชำระเงิน", exact: true })
        .click();
    await page
        .getByLabel("ใบแจ้งหนี้ *", { exact: true })
        .selectOption(invoice.id);
    await page.getByLabel("จำนวนเงิน *", { exact: true }).fill("3620");
    await page.getByRole("button", { name: "บันทึก", exact: true }).click();
    await expect(page.getByText("บันทึกเรียบร้อยแล้ว")).toBeVisible();
    const paymentRow = page
        .getByRole("row")
        .filter({ hasText: "3,620.00" })
        .first();
    await paymentRow.getByRole("button", { name: /รายละเอียด/ }).click();
    await page
        .getByRole("button", { name: "ยืนยันรับชำระ", exact: true })
        .click();
    await page.getByRole("button", { name: "บันทึก", exact: true }).click();
    await expect(page.getByText("บันทึกเรียบร้อยแล้ว")).toBeVisible();
    await page.getByRole("button", { name: "ใบเสร็จ", exact: true }).click();
    await expect(
        page.getByRole("cell", { name: "3,620.00" }).first(),
    ).toBeVisible();
    await page.setViewportSize({ width: 1440, height: 1000 });
    await page.getByRole("button", { name: "ภาพรวม", exact: true }).click();
    await expect(
        page.getByRole("heading", { name: "ห้องพักในขณะนี้" }),
    ).toBeVisible();
    await expect(page.locator(".cash-chart .apexcharts-canvas")).toBeVisible();
    await page.evaluate(() => document.fonts.ready);
    const desktopScreenshot = await page.screenshot({
        fullPage: true,
        animations: "disabled",
    });
    writeFileSync(resolve(process.cwd(), "../TestResults/dashboard-desktop.png"), desktopScreenshot);
    writeFileSync(resolve(reviewDirectory, "desktop.png"), desktopScreenshot);
    await page
        .getByRole("button", { name: "Room Center", exact: true })
        .click();
    await expect(
        page.getByText(room.roomNumber, { exact: true }),
    ).toBeVisible();
    await page.screenshot({
        path: "../TestResults/rooms-desktop.png",
        fullPage: true,
        animations: "disabled",
    });
    await page
        .getByRole("button", { name: "รายงานและวิเคราะห์", exact: true })
        .click();
    await expect(
        page.getByRole("heading", { name: "แนวโน้มรายเดือน" }),
    ).toBeVisible();
    await page.screenshot({
        path: "../TestResults/reports-desktop.png",
        fullPage: true,
        animations: "disabled",
    });
    await page.setViewportSize({ width: 390, height: 844 });
    await page.getByRole("button", { name: "เปิดเมนู", exact: true }).click();
    await page.getByRole("button", { name: "ภาพรวม", exact: true }).click();
    await expect(
        page.getByRole("heading", { name: "ภาพรวมหอพัก" }),
    ).toBeVisible();
    await page.locator(".cash-chart .apexcharts-canvas").waitFor({ state: "visible" });
    await page.evaluate(() => document.fonts.ready);
    const mobileScreenshot = await page.screenshot({
        fullPage: true,
        animations: "disabled",
    });
    writeFileSync(resolve(process.cwd(), "../TestResults/dashboard-mobile.png"), mobileScreenshot);
    writeFileSync(resolve(reviewDirectory, "mobile.png"), mobileScreenshot);
    expect(
        await page.evaluate(
            () => document.documentElement.scrollWidth <= innerWidth,
        ),
    ).toBeTruthy();
    expect(errors).toEqual([]);
});
