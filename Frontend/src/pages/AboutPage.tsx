import Section from "../components/Section";
import TeamMemberCard from "../components/TeamMemberCard";
import FAQItem from "../components/FAQItem";
import Footer from "../components/Footer";

function AboutPage() {
  const team = [
    {
      name: "Emma Thompson",
      role: "Lead developer",
      description:
        "Experienced software engineer with a passion for creating user-friendly transportation solutions.",
    },
    {
      name: "Michael Chen",
      role: "UX designer",
      description:
        "Creative professional focused on designing intuitive and engaging digital experiences.",
    },
    {
      name: "Sarah Rodriguez",
      role: "Product manager",
      description:
        "Strategic thinker dedicated to delivering innovative technology solutions for urban mobility.",
    },
    {
      name: "David Kim",
      role: "Data analyst",
      description:
        "Expert in transforming complex transportation data into actionable insights.",
    },
  ];

  const faqs = [
    {
      question: "How does the app work?",
      answer:
        "Our app connects directly to Transport for London's live data feed, providing real-time updates on tube line statuses. Users can subscribe to specific lines and receive instant notifications.",
    },
    {
      question: "Is the app free?",
      answer:
        "Yes, the basic version of our app is completely free. We offer a premium version with additional features for users who want more detailed notifications.",
    },
    {
      question: "How accurate are the notifications?",
      answer:
        "We use official TfL data sources to ensure the highest level of accuracy and reliability. Notifications are updated in real-time as soon as status changes occur.",
    },
    {
      question: "Can I use the app offline?",
      answer:
        "While real-time updates require an internet connection, we provide email notifications as a backup for users who are currently offline.",
    },
    {
      question: "Which tube lines are covered?",
      answer:
        "Our app covers all London Underground lines, including Bakerloo, Central, Circle, District, Hammersmith & City, Jubilee, Metropolitan, Northern, Piccadilly, Victoria, and Waterloo & City lines.",
    },
  ];

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
        <div className="grid sm:grid-cols-2 md:grid-cols-4 gap-6 mt-6">
          {team.map((member) => (
            <TeamMemberCard key={member.name} {...member} />
          ))}
        </div>
      </Section>

      {/* FAQ Section */}
      <Section tag="FAQs" title="Find answers to common questions">
        <div className="mt-6 space-y-4">
          {faqs.map((faq, i) => (
            <FAQItem key={i} {...faq} />
          ))}
        </div>
      </Section>

      {/* Contact Section */}
      <Section tag="Connect" title="Contact">
        <div className="mt-6 space-y-2">
          <p>Email: support@LondonTubeNotifier.com</p>
          <p>Phone: +44 58 9123 4567</p>
          <p>Office: 23 LondonTubeNotifier, London, UK DC7V 5NY</p>
        </div>
      </Section>

      {/* Footer / Awards */}
      <Footer />
    </div>
  );
}

export default AboutPage;
