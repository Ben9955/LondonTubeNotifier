import { useRef, useState } from "react";
import Section from "../components/Section";
import { login, register } from "../services/authService";
import axios from "axios";
import LoadingSpinner from "../components/Spinner";
import type { LoginPayload, RegisterPayload } from "../types/auth";

export default function AuthPage() {
  const [isLogin, setIsLogin] = useState(true);
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const [isLoading, setIsLoading] = useState(false);
  const formRef = useRef<HTMLFormElement>(null);

  function toggleMode() {
    setIsLogin(!isLogin);
    setErrors({});
    formRef.current?.reset();
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setIsLoading(true);

    const formData = new FormData(e.currentTarget);

    try {
      if (isLogin) {
        const payload: LoginPayload = {
          emailOrUsername:
            formData.get("username")?.toString().trim() ??
            formData.get("email")?.toString().trim() ??
            "",
          password: formData.get("password")?.toString() ?? "",
        };
        const user = await login(payload);

        console.log("Logged in:", user);
        // TODO: save token in localStorage and redirect to homepage
      } else {
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
        };
        const user = await register(payload);
        console.log("Registered:", user);
        // TODO: auto-login or redirect to login page
      }
    } catch (err: any) {
      console.log(err);
      if (axios.isAxiosError(err)) {
        const data = err.response?.data;
        // If it's structured validation errors
        if (data?.errors) {
          setErrors(data.errors);
        } else if (data?.error) {
          // If it's a single error message (non-field-specific)
          setErrors({ general: [data.error] });
        }
      } else {
        console.error("Unknown error", err);
      }
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="px-5 py-10 max-w-md mx-auto">
      <Section
        tag={isLogin ? "Welcome Back" : "Join Us"}
        title={isLogin ? "Login to TubeNotifier" : "Create Your Account"}
        description={
          isLogin
            ? "Access your personalized tube line subscriptions and notifications."
            : "Sign up to manage your tube line alerts and stay informed in real-time."
        }
      >
        {/* Auth Form */}
        <form className="space-y-5 mt-6" ref={formRef} onSubmit={handleSubmit}>
          {errors.general && (
            <div className="text-red-600 text-sm">
              {errors.general.map((msg, idx) => (
                <p key={idx}>{msg}</p>
              ))}
            </div>
          )}
          {!isLogin && (
            <>
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
                  FullName
                </label>
                <input
                  type="text"
                  name="fullName"
                  id="fullName"
                  placeholder="FullName"
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
                />
              </div>
              <div>
                <label htmlFor="phoneNumber" className="block mb-1 font-medium">
                  PhoneNumber
                </label>
                <input
                  type="tel"
                  name="phoneNumber"
                  id="phoneNumber"
                  placeholder="PhoneNumber"
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
                />
              </div>
            </>
          )}

          <div>
            {errors.Email && (
              <p className="text-red-600 text-sm">{errors.Email[0]}</p>
            )}

            <label htmlFor="email" className="block mb-1 font-medium">
              Email {isLogin && "or Username"} *
            </label>
            <input
              type="text"
              name="email"
              id="email"
              placeholder={`Email ${isLogin ? "or Username" : ""}`}
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

          {!isLogin && (
            <div>
              {errors.ConfirmPassword && (
                <p className="text-red-600 text-sm">
                  {errors.ConfirmPassword[0]}
                </p>
              )}

              <label
                htmlFor="confirmPassword"
                className="block mb-1 font-medium"
              >
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
          )}

          <button
            type="submit"
            className="flex items-center justify-center w-full bg-blue-600 text-amber-200 font-semibold rounded-lg py-2 hover:bg-blue-700 transition cursor-pointer "
            disabled={isLoading}
          >
            {isLoading ? <LoadingSpinner /> : isLogin ? "Login" : "Sign Up"}
          </button>
        </form>

        {/* Toggle between login/signup */}
        <p className="text-center text-sm mt-6">
          {isLogin ? "Don't have an account?" : "Already have an account?"}
          <button
            type="button"
            onClick={toggleMode}
            className="text-blue-600 font-semibold hover:underline cursor-pointer"
          >
            {isLogin ? "Sign up here" : "Login here"}
          </button>
        </p>
      </Section>
    </div>
  );
}
