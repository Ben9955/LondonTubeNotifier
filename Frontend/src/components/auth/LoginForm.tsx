import { useRef, useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import { login } from "../../services/authService";
import { Link, useNavigate } from "react-router-dom";
import type { User } from "../../types/user";
import type { LoginPayload } from "../../types/auth";
import LoadingSpinner from "../ui/Spinner";
import Button from "../ui/Button";
import axios from "axios";

export default function LoginForm() {
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
      const payload: LoginPayload = {
        emailOrUsername:
          formData.get("username")?.toString().trim() ??
          formData.get("email")?.toString().trim() ??
          "",
        password: formData.get("password")?.toString() ?? "",
      };

      const user: User = await login(payload);
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
        <label htmlFor="email" className="block mb-1 font-medium">
          Email or Username *
        </label>
        <input
          type="text"
          name="email"
          id="email"
          placeholder="Email or Username"
          className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
          required
        />
      </div>

      <div>
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

      <Button type="submit" disabled={isLoading}>
        {isLoading ? <LoadingSpinner /> : "Login"}
      </Button>
    </form>
  );
}
