import { useState } from "react";
import { NavLink } from "react-router-dom";

function Navbar() {
  const [isOpen, setIsOpen] = useState(false);

  console.log(isOpen);
  return (
    <nav className="fixed w-full bg-white opacity-96 backdrop-blur-md shadow-md px-6 py-4 flex justify-between items-center">
      {/* Desktop links */}
      <ul className="hidden md:flex space-x-6">
        <li>
          <NavLink className="hover:text-blue-600" to="/">
            Home
          </NavLink>
        </li>
        <li>
          <NavLink className="hover:text-blue-600" to="/about">
            About
          </NavLink>
        </li>
      </ul>

      {/* Hamburger button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="focus:outline-none md:hidden "
      >
        <div className="space-y-1">
          <span className="block w-6 h-0.5 bg-black"></span>
          <span className="block w-6 h-0.5 bg-black"></span>
          <span className="block w-6 h-0.5 bg-black"></span>
        </div>
      </button>

      <div className="text-2xl font-bold">TubeNotifier</div>
      <button className="bg-blue-600 text-amber-200">Explore</button>

      {/* Mobile menu */}
      {isOpen && (
        <ul className="absolute top-full left-0 w-full bg-white shadow-md flex flex-col items-center md:hidden">
          <li className="py-2">
            <a href="#" className="hover:text-blue-600">
              Home
            </a>
          </li>
          <li className="py-2">
            <a href="#" className="hover:text-blue-600">
              Features
            </a>
          </li>
          <li className="py-2">
            <a href="#" className="hover:text-blue-600">
              About
            </a>
          </li>
        </ul>
      )}
    </nav>
  );
}

export default Navbar;
