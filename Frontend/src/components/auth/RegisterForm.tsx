import { useRef, useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { register } from "../../services/authService";
import { useNavigate } from "react-router-dom";
import type { User } from "../../types/user";
import type { RegisterPayload } from "../../types/auth";
import LoadingSpinner from "../ui/Spinner";
import Button from "../ui/Button";
import axios from "axios";

// Reusable checkbox component
function CheckboxField({
  name,
  label,
  description,
  defaultChecked = true,
}: {
  name: string;
  label: string;
  description?: string;
  defaultChecked?: boolean;
}) {
  return (
    <div className="flex flex-col space-y-1">
      <div className="flex items-center space-x-2">
        <input
          type="checkbox"
          name={name}
          id={name}
          defaultChecked={defaultChecked}
          className="h-4 w-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
        />
        <label htmlFor={name} className="text-sm font-medium">
          {label}
        </label>
      </div>
      {description && (
        <p className="text-xs text-gray-500 ml-6">{description}</p>
      )}
    </div>
  );
}

export default function RegisterForm() {
  const formRef = useRef<HTMLFormElement>(null);
  const { setUser } = useAuth();
  const navigate = useNavigate();
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsLoading(true);
    setErrors({});
    const formData = new FormData(e.currentTarget);

    try {
      const rawPassword = formData.get("password")?.toString() ?? "";
      if (rawPassword !== rawPassword.trim()) {
        setErrors({
          Password: ["Password cannot contain leading or trailing spaces."],
        });
        return;
      }

      const payload: RegisterPayload = {
        username: formData.get("username")?.toString().trim() ?? "",
        email: formData.get("email")?.toString().trim() ?? "",
        password: rawPassword,
        confirmPassword: formData.get("confirmPassword")?.toString() ?? "",
        fullName: formData.get("fullName")?.toString().trim() || undefined,
        phoneNumber:
          formData.get("phoneNumber")?.toString().trim() || undefined,
        pushNotifications: formData.get("pushNotifications") === "on",
        emailNotifications: formData.get("emailNotifications") === "on",
      };

      const user: User = await register(payload);
      setUser(user);
      navigate("/");
    } catch (err: any) {
      if (axios.isAxiosError(err)) {
        const data = err.response?.data;
        if (data?.errors) setErrors(data.errors);
        else if (data?.error) setErrors({ general: [data.error] });
      } else {
        console.error("Unknown error", err);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <form className="space-y-5 mt-6" ref={formRef} onSubmit={handleSubmit}>
      {errors.general && (
        <div className="text-red-600 text-sm">
          {errors.general.map((msg, idx) => (
            <p key={idx}>{msg}</p>
          ))}
        </div>
      )}

      <div>
        {errors.UserName && (
          <p className="text-red-600 text-sm">{errors.UserName[0]}</p>
        )}
        <label htmlFor="username" className="block mb-1 font-medium">
          Username *
        </label>
        <input
          type="text"
          name="username"
          id="username"
          placeholder="Username"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
          required
        />
      </div>

      <div>
        <label htmlFor="fullName" className="block mb-1 font-medium">
          Full Name
        </label>
        <input
          type="text"
          name="fullName"
          id="fullName"
          placeholder="Full Name"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
      </div>

      <div>
        <label htmlFor="phoneNumber" className="block mb-1 font-medium">
          Phone Number
        </label>
        <input
          type="tel"
          name="phoneNumber"
          id="phoneNumber"
          placeholder="Phone Number"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
        />
      </div>

      <div>
        {errors.Email && (
          <p className="text-red-600 text-sm">{errors.Email[0]}</p>
        )}
        <label htmlFor="email" className="block mb-1 font-medium">
          Email *
        </label>
        <input
          type="text"
          name="email"
          id="email"
          placeholder="Email"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
          required
        />
      </div>

      <div>
        {errors.Password && (
          <p className="text-red-600 text-sm">{errors.Password[0]}</p>
        )}
        <label htmlFor="password" className="block mb-1 font-medium">
          Password *
        </label>
        <input
          type="password"
          name="password"
          id="password"
          placeholder="********"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
          required
        />
      </div>

      <div>
        {errors.ConfirmPassword && (
          <p className="text-red-600 text-sm">{errors.ConfirmPassword[0]}</p>
        )}
        <label htmlFor="confirmPassword" className="block mb-1 font-medium">
          Confirm Password *
        </label>
        <input
          type="password"
          name="confirmPassword"
          id="confirmPassword"
          placeholder="********"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
          required
        />
      </div>

      {/* Notification checkboxes */}
      <CheckboxField
        name="pushNotifications"
        label="Enable Push Notifications"
        description="Get instant alerts on your device when there are issues with your subscribed lines."
        defaultChecked
      />
      <CheckboxField
        name="emailNotifications"
        label="Enable Email Notifications"
        description="Receive daily updates and important disruption notices via email."
        defaultChecked
      />

      <Button type="submit" disabled={isLoading}>
        {isLoading ? <LoadingSpinner /> : "Sign Up"}
      </Button>
    </form>
  );
}
