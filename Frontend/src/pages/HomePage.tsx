import { useEffect, useState } from "react";
import CallToAction from "../components/CallToAction";
import Features from "../components/features/Features";
import Footer from "../components/Footer";
import Header from "../components/Header";
import LineList from "../components/lines/LineList";
import Section from "../components/Section";
import api from "../services/apiClient";
import { startConnection, onFullStatus } from "../services/signalrService";
import type { Line } from "../types/line";
import { useSubscriptions } from "../hooks/useSubscriptions";
import { useAuth } from "../hooks/useAuth";
import Spinner from "../components/ui/Spinner";

const HomePage = () => {
  const [lines, setLines] = useState<Line[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const { subscribedLines } = useSubscriptions();
  const { isAuthenticated } = useAuth();

  const hasSubscriptions = subscribedLines.length > 0;

  // Initial REST API call to fetch static line data
  useEffect(() => {
    const fetchLines = async () => {
      try {
        const res = await api.get("/lines");
        setLines(res.data);
      } catch (err) {
        setError("Failed to load lines");
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchLines();
  }, []);

  // SignalR Setup for Real-time Line Status (Public Homepage Updates)
  useEffect(() => {
    const setupHomePageSignalR = async () => {
      try {
        const connection = await startConnection();

        await connection.invoke("JoinGroup", "homepage-group");

        onFullStatus((data) => {
          console.log(data);
          setLines((prev) =>
            prev.map((line) => {
              const updated = data.updatedLines.find(
                (l) => l.lineId === line.id
              );

              return updated
                ? { ...line, statusDescriptions: updated.statusDescriptions }
                : line;
            })
          );
          console.log("Full status updated for homepage display.");
        });
      } catch (err) {
        console.error("SignalR setup failed in HomePage:", err);
      }
    };

    setupHomePageSignalR();
  }, []);

  const subscribedLineData = lines.filter((l) =>
    subscribedLines.some((s) => l.id === s.id)
  );

  const ctaTitle = !isAuthenticated
    ? "Stay Informed About London Tube Lines"
    : hasSubscriptions
    ? "You're all set!"
    : "Subscribe to the lines you use most";

  const ctaDescription = !isAuthenticated
    ? "Get real-time notifications for the lines you care about — never miss a delay again."
    : hasSubscriptions
    ? "We'll notify you whenever something changes on your lines."
    : "Pick your favorite lines to start receiving live updates.";

  return (
    <div>
      <Header
        isAuthenticated={isAuthenticated}
        hasSubscriptions={hasSubscriptions}
      />

      <Section
        tag="Features"
        title="Powerful Tube Notification Tools"
        description="Manage your London Underground travel with ease"
      >
        <Features />
      </Section>

      <Section
        tag="My Lines"
        title="Your Subscribed Lines"
        description="Quick access to the lines you care about"
      >
        {subscribedLineData.length === 0 ? (
          <p className="text-center text-gray-400">
            You haven't subscribed to any lines yet.
          </p>
        ) : (
          <LineList lines={subscribedLineData} />
        )}
      </Section>

      <Section
        tag="Lines"
        title="London Underground Line Status"
        description="Real-time updates for all London Tube lines"
        sectionId="lines-section"
      >
        {loading ? (
          <Spinner />
        ) : error ? (
          <p className="text-red-500 text-center">{error}</p>
        ) : (
          <LineList lines={lines} />
        )}
      </Section>

      <CallToAction
        title={ctaTitle}
        description={ctaDescription}
        isAuthenticated={isAuthenticated}
        hasSubscriptions={hasSubscriptions}
      />

      <Footer />
    </div>
  );
};

export default HomePage;
