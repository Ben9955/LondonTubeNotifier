import Section from "../components/Section";
import LineList from "../components/LineList";
import Button from "../components/Button";
import { dummyLines } from "../components/LineList";

function SettingsPage() {
  const subscribedLines = dummyLines;

  return (
    <div className="px-5 py-10 max-w-6xl mx-auto space-y-16">
      {/* Hero Section */}
      <Section
        tag="My Tube Subscriptions"
        title="Manage your London Tube line notifications"
        description="Stay informed about the latest London Underground service updates across your selected lines."
      />

      {/* Subscribed Lines */}
      <Section
        title="Subscribed Lines"
        description="Review and modify your subscribed lines to ensure you receive the most relevant updates."
      >
        <LineList lines={subscribedLines} />
      </Section>

      {/* Preferences */}
      <Section
        title="Notification Preferences"
        description="Customize how you receive updates about your subscribed tube lines."
      >
        <div className="mt-6 flex flex-col md:flex-row gap-4 md:justify-center">
          <Button>Real-time Push Notifications</Button>
          <Button>Email Notifications</Button>
          <Button>Configure Alerts</Button>
        </div>
      </Section>

      {/* Account Management */}
      <Section
        title="Account Settings"
        description="Manage your personal information and subscription preferences."
      >
        <div className="mt-6 flex flex-col md:flex-row gap-4 md:justify-center">
          <Button>Edit Profile</Button>
          <Button>Change Password</Button>
          <Button className="bg-red-600 text-white">Cancel Account</Button>
        </div>
      </Section>
    </div>
  );
}

export default SettingsPage;
