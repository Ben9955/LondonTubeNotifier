type FeatureCardProps = {
  title: string;
  tag: string;
  description: string;
  imgUrl: string;
};

const FeatureCard = ({ tag, title, description, imgUrl }: FeatureCardProps) => {
  return (
    <div className="flex flex-col md:flex-row  bg-white rounded-xl shadow-md overflow-hidden hover:shadow-lg transition h-full">
      <div className="md:w-1/3">
        <img
          src={imgUrl}
          alt={title}
          className="w-full h-48 md:h-full object-cover"
        />
      </div>

      {/* <div className="p-5 md:w-2/3 flex flex-col justify-center"> */}
      <div className="p-5 flex flex-col flex-1">
        <p className="text-blue-600 font-semibold ">{tag}</p>
        <h5 className="font-semibold text-2xl my-2 ">{title}</h5>
        <p>{description}</p>
      </div>
    </div>
  );
};

export default FeatureCard;
