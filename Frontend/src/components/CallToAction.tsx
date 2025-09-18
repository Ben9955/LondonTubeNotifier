function CallToAction() {
  return (
    <div className="py-16 px-5 ">
      <div className="flex flex-col md:flex-row items-center md:justify-between gap-2 ">
        <div className="text-center px-5 md:text-left md:w-1/2">
          <h1 className="font-semibold text-2xl md:text-4xl lg:text-5xl mb-5  ">
            Get Started with London Tube Notifier
          </h1>
          <p className="text-gray-700 text-base md:text-lg">
            Sign up now to receive notifications for your favorite lines.
          </p>

          <div className="mt-6 mb-10 flex justify-center md:justify-start gap-6">
            <button className="bg-blue-600 text-amber-200">
              Subscribe Now
            </button>
          </div>
        </div>
        <div className="md:w-1/2">
          <img
            src="https://cdn.pixabay.com/photo/2024/03/19/15/37/call-center-8643475_1280.jpg"
            className="rounded-2xl w-full"
          />
        </div>
      </div>
    </div>
  );
}

export default CallToAction;
