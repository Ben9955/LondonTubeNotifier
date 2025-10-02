import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { AuthProvider } from "./context/AuthProvider.tsx";
import { SubscriptionProvider } from "./context/SubscriptionProvider.tsx";

createRoot(document.getElementById("root")!).render(
  // <StrictMode>
  <AuthProvider>
    <SubscriptionProvider>
      <App />
    </SubscriptionProvider>
  </AuthProvider>
  //* </StrictMode>
);
