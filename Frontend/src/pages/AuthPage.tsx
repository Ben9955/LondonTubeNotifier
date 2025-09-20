import { useState } from "react";
import Section from "../components/Section";

export default function AuthPage() {
  const [isLogin, setIsLogin] = useState(true);

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
        <form className="space-y-5 mt-6">
          {!isLogin && (
            <>
              <div>
                <label htmlFor="username" className="block mb-1 font-medium">
                  Username *
                </label>
                <input
                  type="text"
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
                  id="phoneNumber"
                  placeholder="PhoneNumber"
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
                />
              </div>
            </>
          )}

          <div>
            <label htmlFor="email" className="block mb-1 font-medium">
              Email {isLogin && "or Username"} *
            </label>
            <input
              type="text"
              id="email"
              placeholder={`Email ${isLogin ? "or Username" : ""}`}
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
              id="password"
              placeholder="********"
              className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
              required
            />
          </div>

          {!isLogin && (
            <div>
              <label
                htmlFor="confirmPassword"
                className="block mb-1 font-medium"
              >
                Confirm Password *
              </label>
              <input
                type="Confirm Password"
                id="confirmPassword"
                placeholder="********"
                className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-600"
                required
              />
            </div>
          )}

          <button
            type="submit"
            className="w-full bg-blue-600 text-amber-200 font-semibold rounded-lg py-2 hover:bg-blue-700 transition cursor-pointer"
          >
            {isLogin ? "Login" : "Sign Up"}
          </button>
        </form>

        {/* Toggle between login/signup */}
        <p className="text-center text-sm mt-6">
          {isLogin ? "Don't have an account?" : "Already have an account?"}
          <button
            type="button"
            onClick={() => setIsLogin(!isLogin)}
            className="text-blue-600 font-semibold hover:underline cursor-pointer"
          >
            {isLogin ? "Sign up here" : "Login here"}
          </button>
        </p>
      </Section>
    </div>
  );
}
