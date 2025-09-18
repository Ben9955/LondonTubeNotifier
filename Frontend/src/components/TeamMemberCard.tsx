type TeamMemberProps = {
  name: string;
  role: string;
  description: string;
};

const TeamMemberCard = ({ name, role, description }: TeamMemberProps) => {
  return (
    <div className="bg-white rounded-lg shadow-md p-5 text-center">
      <h4 className="font-semibold">{name}</h4>
      <p className="text-gray-500">{role}</p>
      <p className="mt-2 text-sm text-gray-600">{description}</p>
    </div>
  );
};

export default TeamMemberCard;
