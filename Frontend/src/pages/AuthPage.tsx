import { useState } from "react";
import Section from "../components/Section";
import LoginForm from "../components/auth/LoginForm";
import RegisterForm from "../components/auth/RegisterForm";

export default function AuthPage() {
  const [isLogin, setIsLogin] = useState(true);

  const toggleMode = () => setIsLogin((prev) => !prev);

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
        {isLogin ? <LoginForm /> : <RegisterForm />}

        <p className="text-center text-sm mt-6">
          {isLogin ? "Don't have an account?" : "Already have an account?"}
          <button
            type="button"
            onClick={toggleMode}
            className="text-blue-600 font-semibold hover:underline ml-1 cursor-pointer"
            aria-pressed={isLogin}
          >
            {isLogin ? "Sign up here" : "Login here"}
          </button>
        </p>
      </Section>
    </div>
  );
}
