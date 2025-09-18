import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "./pages/HomePage";
import SubscriptionsPage from "./pages/SubscriptionsPage";
import Navbar from "./components/Navbar";
import AboutPage from "./pages/AboutPage";

export default function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <main className="pt-20">
        <Routes>
          <Route path="/" element={<AboutPage />} />
          <Route path="/login" element={<AboutPage />} />
          <Route path="/subscriptions" element={<SubscriptionsPage />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}
