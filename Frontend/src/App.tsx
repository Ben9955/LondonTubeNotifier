import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "./pages/HomePage";
import Navbar from "./components/Navbar";
import AboutPage from "./pages/AboutPage";
import ProfilePage from "./pages/ProfilePage";
import AuthPage from "./pages/AuthPage";
import PrivateRoute from "./components/PrivateRoute";
import { useAuth } from "./hooks/useAuth";
import {
  offLineUpdate,
  onLineUpdate,
  startConnection,
  stopConnection,
} from "./services/signalrService";
import { useEffect } from "react";
import { Toaster } from "react-hot-toast";
import { showLineUpdateToast } from "./components/toasts/showLineUpdateToast";

export default function App() {
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    const initSignalR = async () => {
      if (isAuthenticated) {
        await stopConnection();

        await startConnection();

        offLineUpdate();
        onLineUpdate((data) => {
          console.log("🔥 Global line update:", data);
          showLineUpdateToast(data);
        });
      }
    };

    initSignalR();
  }, [isAuthenticated]);

  return (
    <BrowserRouter>
      <Navbar />
      <Toaster position="top-center" reverseOrder={false} />
      <main className="pt-20">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/about" element={<AboutPage />} />
          <Route path="/auth" element={<AuthPage />} />
          <Route
            path="/profile"
            element={
              <PrivateRoute>
                <ProfilePage />
              </PrivateRoute>
            }
          />
        </Routes>
      </main>
    </BrowserRouter>
  );
}
