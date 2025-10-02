import React, { useEffect, useMemo, useState } from "react";
import Button from "./ui/Button";
import Spinner from "./ui/Spinner";
import type { User } from "../types/user";

type ProfileFormProps = {
  user: User;
  onSave: (updatedUser: User) => void | Promise<void>;
  loading: boolean;
};

const NotificationCheckbox = ({
  name,
  label,
  checked,
  onChange,
}: {
  name: string;
  label: string;
  checked: boolean;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}) => (
  <label className="flex items-center gap-2">
    <input type="checkbox" name={name} checked={checked} onChange={onChange} />
    {label}
  </label>
);

const ProfileForm = ({ user, onSave, loading }: ProfileFormProps) => {
  const [formData, setFormData] = useState<User>({
    ...user,
    fullName: user.fullName || "",
    phoneNumber: user.phoneNumber || "",
    pushNotifications: user.pushNotifications ?? false,
    emailNotifications: user.emailNotifications ?? false,
  });

  useEffect(() => {
    setFormData({
      ...user,
      fullName: user.fullName || "",
      phoneNumber: user.phoneNumber || "",
      pushNotifications: user.pushNotifications ?? false,
      emailNotifications: user.emailNotifications ?? false,
    });
  }, [user]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, type, value, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const isChanged = useMemo(() => {
    return (
      (user.fullName || "") !== (formData.fullName || "") ||
      (user.phoneNumber || "") !== (formData.phoneNumber || "") ||
      !!user.pushNotifications !== !!formData.pushNotifications ||
      !!user.emailNotifications !== !!formData.emailNotifications
    );
  }, [user, formData]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!isChanged || loading) return;

    onSave({
      ...user,
      fullName: formData.fullName || "",
      phoneNumber: formData.phoneNumber || "",
      pushNotifications: !!formData.pushNotifications,
      emailNotifications: !!formData.emailNotifications,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6 max-w-xl mx-auto">
      {/* Username */}
      <div>
        <label className="block text-sm font-medium">Username</label>
        <input
          type="text"
          name="username"
          value={formData.username}
          disabled
          className="mt-1 block w-full border rounded-md p-2 bg-gray-100 text-gray-400"
        />
      </div>

      {/* Email */}
      <div>
        <label className="block text-sm font-medium">Email</label>
        <input
          type="text"
          name="email"
          value={formData.email}
          disabled
          className="mt-1 block w-full border rounded-md p-2 bg-gray-100 text-gray-400"
        />
      </div>

      {/* Full Name */}
      <div>
        <label className="block text-sm font-medium">Full Name</label>
        <input
          type="text"
          name="fullName"
          value={formData.fullName}
          onChange={handleChange}
          className="mt-1 block w-full border rounded-md p-2"
        />
      </div>

      {/* Phone Number */}
      <div>
        <label className="block text-sm font-medium">Phone Number</label>
        <input
          type="tel"
          name="phoneNumber"
          value={formData.phoneNumber}
          onChange={handleChange}
          className="mt-1 block w-full border rounded-md p-2"
        />
      </div>

      {/* Notifications */}
      <div className="flex flex-col gap-3">
        <NotificationCheckbox
          name="emailNotifications"
          checked={formData.emailNotifications}
          onChange={handleChange}
          label="Email Notifications (Get updates about line disruptions via email)"
        />
        <NotificationCheckbox
          name="pushNotifications"
          checked={formData.pushNotifications}
          onChange={handleChange}
          label="Push Notifications (Receive alerts directly on your device)"
        />
      </div>

      {/* Submit Button */}
      <Button type="submit" disabled={loading || !isChanged}>
        {loading ? <Spinner /> : "Save Changes"}
      </Button>
    </form>
  );
};

export default ProfileForm;
