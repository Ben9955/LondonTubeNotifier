import { Link } from "react-router-dom";
import { buttonBase } from "./ui/buttonVariants";
import Button from "./ui/Button";

type HeaderProps = {
  isAuthenticated: boolean;
  hasSubscriptions: boolean;
};

const Header = ({ isAuthenticated, hasSubscriptions }: HeaderProps) => {
  const handleScrollToLines = () => {
    const section = document.getElementById("lines-section");
    if (section) {
      section.scrollIntoView({ behavior: "smooth" });
    }
  };

  const noSubscriptions = isAuthenticated && !hasSubscriptions;
  const hasLines = isAuthenticated && hasSubscriptions;

  const heading = !isAuthenticated
    ? "Stay Informed About London Tube Lines"
    : noSubscriptions
    ? "Pick your favorite lines"
    : "Manage Your Subscribed Lines";

  const description = !isAuthenticated
    ? "Get real-time notifications for the lines you care about — never miss a delay again."
    : noSubscriptions
    ? "Choose the lines you use most to start receiving live updates."
    : "Scroll to your lines below to review updates or adjust subscriptions.";

  return (
    <div className="py-16 px-5">
      <div className="flex flex-col md:flex-row items-center md:justify-between gap-6">
        <div className="text-center px-5 md:text-left md:w-1/2">
          <h1 className="font-semibold text-2xl md:text-4xl lg:text-5xl mb-5">
            {heading}
          </h1>

          <p className="text-gray-700 text-base md:text-lg">{description}</p>

          <div className="mt-6 mb-10 flex justify-center md:justify-start gap-6">
            {!isAuthenticated && (
              <Link to="/auth" className={buttonBase}>
                Subscribe Now
              </Link>
            )}

            {noSubscriptions && (
              <Button onClick={handleScrollToLines}>Subscribe to Lines</Button>
            )}

            {hasLines && (
              <Button onClick={handleScrollToLines}>Go to My Lines</Button>
            )}
          </div>
        </div>

        <div className="md:w-1/2">
          <img
            src="https://cdn.pixabay.com/photo/2020/09/22/18/15/passenger-5593947_1280.jpg"
            className="rounded-2xl w-full"
            alt="Passenger waiting for tube"
          />
        </div>
      </div>
    </div>
  );
};

export default Header;
