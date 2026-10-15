import React from "react";
import { createRoot } from "react-dom/client";
import "bootstrap/dist/css/bootstrap.min.css";
import "@fontsource/noto-sans-thai/400.css";
import "@fontsource/noto-sans-thai/500.css";
import "@fontsource/noto-sans-thai/600.css";
import "@fontsource/manrope/600.css";
import App from "./App.jsx";
import "./styles.css";
class ErrorBoundary extends React.Component {
    state = { error: false };
    static getDerivedStateFromError() {
        return { error: true };
    }
    render() {
        return this.state.error ? (
            <main className="fatal">
                <h1>เปิดหน้านี้ไม่สำเร็จ</h1>
                <p>ลองโหลดใหม่ หากยังพบปัญหา กรุณาติดต่อผู้ดูแล</p>
                <button
                    className="btn btn-primary"
                    onClick={() => location.reload()}
                >
                    โหลดหน้าใหม่
                </button>
            </main>
        ) : (
            this.props.children
        );
    }
}
createRoot(document.getElementById("root")).render(
    <ErrorBoundary>
        <App />
    </ErrorBoundary>,
);
