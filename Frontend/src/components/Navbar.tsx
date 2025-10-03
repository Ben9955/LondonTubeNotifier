import { useState } from "react";
import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

const links = [
  { to: "/", label: "Home" },
  { to: "/about", label: "About" },
];

function Navbar() {
  const [isOpen, setIsOpen] = useState(false);
  const { isAuthenticated } = useAuth();

  const AuthButton = ({ isMobile = false }: { isMobile?: boolean }) => {
    if (isAuthenticated) {
      return (
        <Link
          to="/profile"
          className={`${
            isMobile ? "mt-4" : "hidden md:block"
          } hover:text-blue-600`}
        >
          Profile
        </Link>
      );
    }
    return (
      <Link
        to="/auth"
        className={`${
          isMobile ? "mt-4" : "hidden md:block"
        } bg-blue-600 text-amber-200 px-4 py-2 rounded font-semibold hover:bg-blue-700 transition`}
      >
        Login
      </Link>
    );
  };

  return (
    <nav className="fixed z-50 w-full bg-white opacity-96 backdrop-blur-md shadow-md px-6 py-4 flex justify-between items-center">
      {/* Logo */}
      <Link to="/" className="text-2xl font-bold hover:text-blue-600">
        TubeNotifier
      </Link>

      {/* Desktop links */}
      <ul className="hidden md:flex space-x-6">
        {links.map((link) => (
          <li key={link.to}>
            <NavLink
              to={link.to}
              className={({ isActive }) =>
                `hover:text-blue-600 ${
                  isActive ? "text-blue-600 font-semibold" : ""
                }`
              }
            >
              {link.label}
            </NavLink>
          </li>
        ))}
      </ul>

      {/* Auth button desktop */}
      <AuthButton />

      {/* Hamburger */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        aria-label="Toggle menu"
        aria-expanded={isOpen}
        className="focus:outline-none md:hidden cursor-pointer"
      >
        <div className="space-y-1">
          <span className="block w-6 h-0.5 bg-black"></span>
          <span className="block w-6 h-0.5 bg-black"></span>
          <span className="block w-6 h-0.5 bg-black"></span>
        </div>
      </button>

      {/* Mobile menu */}
      {isOpen && (
        <div className="absolute top-full left-0 w-full bg-white shadow-md flex flex-col items-center gap-4 md:hidden z-40 py-6">
          {links.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              onClick={() => setIsOpen(false)}
              className={({ isActive }) =>
                `py-2 hover:text-blue-600 ${
                  isActive ? "text-blue-600 font-semibold" : ""
                }`
              }
            >
              {link.label}
            </NavLink>
          ))}

          <AuthButton isMobile />
        </div>
      )}
    </nav>
  );
}

export default Navbar;
