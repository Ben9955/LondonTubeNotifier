import { team } from "../../data/team";
import TeamMemberCard from "./TeamMemberCard";

function TeamMembers() {
  return (
    <div className="grid sm:grid-cols-2 md:grid-cols-4 gap-6 mt-6">
      {team.map((member) => (
        <TeamMemberCard key={member.name} {...member} />
      ))}
    </div>
  );
}

export default TeamMembers;
