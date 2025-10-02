import { useState } from "react";
import Section from "../components/Section";
import ProfileForm from "../components/ProfileForm";
import { useAuth } from "../hooks/useAuth";
import LineList from "../components/lines/LineList";
import { useSubscriptions } from "../hooks/useSubscriptions";
import { logout, updateProfile } from "../services/authService";
import Spinner from "../components/ui/Spinner";
import type { User } from "../types/user";

function ProfilePage() {
  const { user, setUser, isAuthenticated } = useAuth();
  const { subscribedLines } = useSubscriptions();

  const [saving, setSaving] = useState(false);
  const [loggingOut, setLoggingOut] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!isAuthenticated || !user) {
    return (
      <div className="px-5 py-10 max-w-4xl mx-auto">
        <p className="text-center text-gray-600">
          You must be logged in to view this page.
        </p>
      </div>
    );
  }

  // Handle profile updates
  const handleSave = async (updatedUser: User) => {
    setError(null);
    try {
      setSaving(true);
      const newUser = await updateProfile({
        fullName: updatedUser.fullName,
        phoneNumber: updatedUser.phoneNumber,
        pushNotifications: updatedUser.pushNotifications,
        emailNotifications: updatedUser.emailNotifications,
      });
      setUser(newUser);
    } catch (err) {
      console.error("Failed to save profile", err);
      setError("Failed to save profile. Please try again.");
    } finally {
      setSaving(false);
    }
  };

  // Handle logout
  const handleLogout = async () => {
    try {
      setLoggingOut(true);
      await logout();
      setUser(null);
    } catch (err) {
      console.error("Failed to logout", err);
      setError("Failed to log out. Please try again.");
    } finally {
      setLoggingOut(false);
    }
  };

  return (
    <div className="px-5 py-10 max-w-6xl mx-auto space-y-16">
      {/* Hero */}
      <Section
        tag="My Profile"
        title="Manage your account and preferences"
        description="Update your profile information and choose how you receive London Tube updates."
      />

      {/* Profile Form */}
      <Section
        title="Account Information"
        description="Edit your personal details."
      >
        {error && <p className="text-red-600 mb-4">{error}</p>}
        <ProfileForm user={user} onSave={handleSave} loading={saving} />
      </Section>

      {/* Subscriptions */}
      <Section
        title="Subscribed Lines"
        description="Review and manage your subscribed lines."
      >
        {subscribedLines.length === 0 ? (
          <p className="text-gray-500">You have no subscribed lines yet.</p>
        ) : (
          <LineList lines={subscribedLines} />
        )}
      </Section>

      {/* Logout */}
      <div className="flex justify-end">
        <button
          onClick={handleLogout}
          className=" px-5 py-2 rounded-md bg-red-600 hover:bg-red-500 text-white cursor-pointer "
        >
          {loggingOut ? <Spinner /> : "Logout"}
        </button>
      </div>
    </div>
  );
}

export default ProfilePage;
