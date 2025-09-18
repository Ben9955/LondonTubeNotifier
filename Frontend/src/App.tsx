import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import SubscriptionsPage from "./pages/SubscriptionsPage";
import Navbar from "./components/NavBar";

export default function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <main className="pt-20">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/subscriptions" element={<SubscriptionsPage />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}
