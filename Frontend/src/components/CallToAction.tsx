import { Link } from "react-router-dom";
import Button from "./ui/Button";
import { buttonBase } from "./ui/buttonVariants";

type CallToActionProps = {
  title: string;
  description: string;
  isAuthenticated: boolean;
  hasSubscriptions: boolean;
};

function CallToAction({
  title,
  description,
  isAuthenticated,
  hasSubscriptions,
}: CallToActionProps) {
  const handleScrollToLines = () => {
    const section = document.getElementById("lines-section");
    if (section) {
      section.scrollIntoView({ behavior: "smooth" });
    }
  };

  const showButton = !isAuthenticated || !hasSubscriptions;

  return (
    <div className="py-16 px-5">
      <div className="flex flex-col md:flex-row items-center md:justify-between gap-8">
        <div className="text-center px-5 md:text-left md:w-1/2">
          <h1 className="font-semibold text-2xl md:text-4xl lg:text-5xl mb-5">
            {title}
          </h1>

          <p className="text-gray-700 text-base md:text-lg">{description}</p>

          {showButton && (
            <div className="mt-6 mb-10 flex justify-center md:justify-start gap-6">
              {!isAuthenticated ? (
                <Link to="/auth" className={buttonBase}>
                  Subscribe Now
                </Link>
              ) : (
                <Button onClick={handleScrollToLines}>
                  Subscribe to Lines
                </Button>
              )}
            </div>
          )}
        </div>

        <div className="md:w-1/2">
          <img
            src="https://cdn.pixabay.com/photo/2024/03/19/15/37/call-center-8643475_1280.jpg"
            className="rounded-2xl w-full"
            alt="Stay informed illustration"
          />
        </div>
      </div>
    </div>
  );
}

export default CallToAction;
