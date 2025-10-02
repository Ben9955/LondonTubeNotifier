import type { Line } from "../../types/line";

import LineCard from "./LineCard";

function groupByMode(lines: Line[]): Record<string, Line[]> {
  return lines.reduce((groups, line) => {
    if (!groups[line.modeName]) {
      groups[line.modeName] = [];
    }
    groups[line.modeName].push(line);
    return groups;
  }, {} as Record<string, Line[]>);
}

function LineList({ lines }: { lines: Line[] }) {
  const grouped = groupByMode(lines);

  const modeLabels: Record<string, string> = {
    tube: "🚇 Tube",
    dlr: "🚊 DLR",
    overground: "🚆 Overground",
    "elizabeth-line": "🚄 Elizabeth Line",
    tram: "🚋 Tram",
  };

  return (
    <div className="mt-5 space-y-8 max-w-2xl mx-auto">
      {Object.entries(grouped).map(([mode, lines]) => (
        <div key={mode} className="space-y-3">
          <h2 className="text-2xl font-semibold border-b pb-2">
            {modeLabels[mode] || mode}
          </h2>
          <div className="space-y-2">
            {lines.map((line) => (
              <LineCard key={line.id} line={line} />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

export default LineList;
