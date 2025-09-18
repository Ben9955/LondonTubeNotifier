import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "./pages/HomePage";
import SubscriptionsPage from "./pages/SettingsPage";
import Navbar from "./components/Navbar";
import AboutPage from "./pages/AboutPage";
import SettingsPage from "./pages/SettingsPage";

export default function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <main className="pt-20">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/about" element={<AboutPage />} />
          <Route path="/subscriptions" element={<SettingsPage />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}
