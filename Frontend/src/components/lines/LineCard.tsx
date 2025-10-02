import { useState } from "react";
import { useSubscriptions } from "../../hooks/useSubscriptions";
import type { Line } from "../../types/line";
import { useAuth } from "../../hooks/useAuth";

function formatLastUpdate(dateString: string): string {
  const date = new Date(dateString);
  const now = new Date();

  const isToday =
    date.getDate() === now.getDate() &&
    date.getMonth() === now.getMonth() &&
    date.getFullYear() === now.getFullYear();

  const yesterday = new Date(now);
  yesterday.setDate(now.getDate() - 1);

  const isYesterday =
    date.getDate() === yesterday.getDate() &&
    date.getMonth() === yesterday.getMonth() &&
    date.getFullYear() === yesterday.getFullYear();

  if (isToday) {
    return `Today ${date.toLocaleTimeString("en-GB")}`;
  }

  if (isYesterday) {
    return `Yesterday ${date.toLocaleTimeString("en-GB")}`;
  }

  return date.toLocaleDateString("en-GB");
}

function getStatusClass(severity: number) {
  if (severity >= 10) return "bg-green-100 text-green-700";
  if (severity >= 6) return "bg-yellow-100 text-yellow-700";
  return "bg-red-100 text-red-700";
}

type LineCardProps = {
  line: Line;
};

const LineCard = ({ line }: LineCardProps) => {
  const [showReason, setShowReason] = useState(false);
  const { subscribedLines, toggleSubscription } = useSubscriptions();
  const { name, statusDescriptions, color } = line;
  const first = statusDescriptions?.[0];
  const isSubscribed = subscribedLines.some((s) => line.id === s.id);

  const { isAuthenticated } = useAuth();

  return (
    <div className="p-4 rounded-xl bg-white border shadow-sm hover:shadow-md transition relative">
      <div
        className="absolute top-0 left-0 h-full w-2 rounded-l-xl"
        style={{ backgroundColor: color }}
      />

      <div className="ml-4">
        <div className="flex justify-between items-center">
          <span className="font-semibold text-gray-900">{name}</span>

          <div className="flex items-center gap-2">
            {first && (
              <span
                className={`text-sm px-2 py-1 rounded-md ${getStatusClass(
                  first.statusSeverity
                )}`}
              >
                {first.statusDescription}
              </span>
            )}

            {isAuthenticated && (
              <button
                className={`text-xs px-2 py-1 rounded border cursor-pointer ${
                  isSubscribed
                    ? "bg-blue-600 text-white"
                    : "bg-white text-blue-600 border-blue-600"
                }`}
                onClick={() => toggleSubscription(line, isSubscribed)}
              >
                {isSubscribed ? "Subscribed" : "Subscribe"}
              </button>
            )}
          </div>
        </div>

        {first?.lastUpdate && (
          <p className="text-xs text-gray-500 mt-1">
            Last status change: {formatLastUpdate(first.lastUpdate)}
          </p>
        )}

        {first?.reason && (
          <div className="mt-2">
            <button
              className="text-blue-600 text-sm hover:underline cursor-pointer"
              onClick={() => setShowReason(!showReason)}
            >
              {showReason ? "Hide details" : "More info"}
            </button>

            {showReason && (
              <p className="text-sm text-gray-700 mt-1">{first.reason}</p>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default LineCard;
