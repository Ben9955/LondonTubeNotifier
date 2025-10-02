import { features } from "../../data/features";
import FeatureCard from "./FeatureCard";

function Features() {
  return (
    <div className="mt-5 grid md:grid-cols-3 gap-5">
      {features.map((f) => (
        <FeatureCard key={f.title} {...f} />
      ))}
    </div>
  );
}

export default Features;
