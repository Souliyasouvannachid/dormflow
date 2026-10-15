import { defineConfig } from "@playwright/test";
export default defineConfig({
    testDir: "./e2e",
    workers: 1,
    timeout: 60000,
    use: {
        baseURL: process.env.QA_WEB_URL || "http://127.0.0.1:5179",
        headless: true,
        launchOptions: {
            executablePath:
                process.env.QA_BROWSER_PATH ||
                "C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe",
        },
        screenshot: "only-on-failure",
    },
    reporter: [
        ["list"],
        ["html", { outputFolder: "playwright-report", open: "never" }],
    ],
});
