type LineCardProps = {
  name: string;
  status: string;
  color: string;
};

const LineCard = ({ name, status, color }: LineCardProps) => (
  <div
    className="p-4 rounded-md text-white flex justify-between items-center shadow-md hover:shadow-lg transition"
    style={{ backgroundColor: color }}
  >
    <span className="font-semibold">{name}</span>
    <span className="text-sm">{status}</span>
  </div>
);

export default LineCard;
