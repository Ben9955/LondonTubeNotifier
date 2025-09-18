function Footer() {
  return (
    <footer className="py-16 px-5">
      <hr className="border-t border-gray-300 my-5" />
      <div className=" flex flex-col md:flex-row md:justify-between gap-5">
        <div className=" flex flex-col md:flex-row gap-2">
          <a>Privacy Policy</a>
          <a>Terms of Service</a>
          <a>Cookie Settings</a>
        </div>
        <p className="text-gray-400">
          © 2024 London Tube Notifier. All rights reserved.
        </p>
      </div>
    </footer>
  );
}

export default Footer;
