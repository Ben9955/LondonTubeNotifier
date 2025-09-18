import CallToAction from "../components/CallToAction";
import Features from "../components/Features";
import Footer from "../components/Footer";
import Header from "../components/Header";
import LineList, { dummyLines } from "../components/LineList";
import Section from "../components/Section";

const HomePage = () => {
  const dummyList = dummyLines;
  return (
    <div>
      <Header />
      <Section
        tag="Features"
        title="Powerful Tube Notification Tools"
        description="Manage your London Underground travel with ease"
      >
        <Features />
      </Section>
      <Section
        tag="Lines"
        title="London Underground Line Status"
        description="Real-time updates for all London Tube lines"
      >
        <LineList lines={dummyLines} />
      </Section>

      <CallToAction />

      <Footer />
    </div>
  );
};

export default HomePage;
