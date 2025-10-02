import type { Line } from "../types/line";
import LineList from "./lines/LineList";

type ProfileSubscriptionsProps = {
  subscriptions: Line[];
};

const ProfileSubscriptions = ({ subscriptions }: ProfileSubscriptionsProps) => {
  if (!subscriptions.length) {
    return (
      <p className="text-gray-600">You are not subscribed to any lines yet.</p>
    );
  }

  return <LineList lines={subscriptions} />;
};

export default ProfileSubscriptions;
