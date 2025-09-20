import { Link } from "react-router-dom";

const Header = () => {
  return (
    <div className="py-16 px-5 ">
      <div className="flex flex-col md:flex-row items-center md:justify-between gap-3 ">
        <div className="text-center px-5 md:text-left md:w-1/2">
          <h1 className="font-semibold text-2xl md:text-4xl lg:text-5xl mb-5  ">
            Stay Informed About London Tube Lines
          </h1>
          <p className="text-gray-700 text-base md:text-lg">
            Get real-time notifications for the lines you care about — never
            miss a delay again.
          </p>

          <div className="mt-6 mb-10 flex justify-center md:justify-start gap-6">
            <Link
              to="/auth"
              className="bg-blue-600  text-amber-200 px-4 py-2 rounded font-semibold hover:bg-blue-700 transition"
            >
              Subscribe Now
            </Link>
          </div>
        </div>
        <div className="md:w-1/2">
          <img
            src="https://cdn.pixabay.com/photo/2020/09/22/18/15/passenger-5593947_1280.jpg"
            className="rounded-2xl w-full"
          />
        </div>
      </div>
    </div>
  );
};

export default Header;
