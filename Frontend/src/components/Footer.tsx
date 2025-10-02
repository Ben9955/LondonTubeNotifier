function Footer() {
  return (
    <footer className="py-16 px-5">
      <hr className="border-t border-gray-300 my-5" />

      <div className="flex flex-col md:flex-row md:justify-between gap-5">
        <div className="flex flex-col md:flex-row gap-4">
          <a
            href="#"
            className="text-gray-600 hover:text-gray-800 hover:underline transition"
          >
            Privacy Policy
          </a>
          <a
            href="#"
            className="text-gray-600 hover:text-gray-800 hover:underline transition"
          >
            Terms of Service
          </a>
          <a
            href="#"
            className="text-gray-600 hover:text-gray-800 hover:underline transition"
          >
            Cookie Settings
          </a>
        </div>

        <p className="text-gray-400 mt-4 md:mt-0">
          © 2025 London Tube Notifier. All rights reserved.
        </p>
      </div>
    </footer>
  );
}

export default Footer;
