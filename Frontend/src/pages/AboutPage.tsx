import Section from "../components/Section";
import Footer from "../components/Footer";
import FAQs from "../components/faqs/FAQs";
import TeamMembers from "../components/team/TeamMembers";
import Contacts from "../components/Contacts";

function AboutPage() {
  return (
    <div className="space-y-16 px-5 py-10 max-w-6xl mx-auto">
      <Section
        tag="London Tube Notifier"
        title="Stay informed about London Underground line statuses"
        description="Receive real-time notifications and updates directly to your device."
      />

      {/* Mission Section */}
      <Section
        title="Your smart companion for navigating London's public transport"
        description="Helping commuters move with ease and confidence through reliable, real-time information."
      />

      {/* Team Section */}
      <Section tag="Team" title="Our dedicated team">
        <TeamMembers />
      </Section>

      {/* FAQ Section */}
      <Section tag="FAQs" title="Find answers to common questions">
        <FAQs />
      </Section>

      {/* Contact Section */}
      <Section tag="Connect" title="Contact">
        <Contacts />
      </Section>

      {/* Footer / Awards */}
      <Footer />
    </div>
  );
}

export default AboutPage;
