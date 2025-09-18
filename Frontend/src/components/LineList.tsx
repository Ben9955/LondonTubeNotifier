import type { Line } from "../types/Line";

export const dummyLines: Line[] = [
  {
    id: "central",
    code: "CEN",
    name: "Central",
    color: "#DC241F",
    modeName: "tube",
    status: "Good Service",
  },
  {
    id: "district",
    code: "DIS",
    name: "District",
    color: "#00782A",
    modeName: "tube",
    status: "Severe Delays",
  },
  {
    id: "victoria",
    code: "VIC",
    name: "Victoria",
    color: "#00A0E2",
    modeName: "tube",
    status: "Minor Delays",
  },
  {
    id: "dlr",
    code: "DLR",
    name: "Docklands Light Railway",
    color: "#00AFAD",
    modeName: "dlr",
    status: "Good Service",
  },
  {
    id: "overground",
    code: "OVG",
    name: "London Overground",
    color: "#EE7C0E",
    modeName: "overground",
    status: "Part Closure",
  },
  {
    id: "elizabeth-line",
    code: "ELI",
    name: "Elizabeth Line",
    color: "#9364CC",
    modeName: "elizabeth-line",
    status: "Good Service",
  },
  {
    id: "tram",
    code: "TRM",
    name: "London Trams",
    color: "#84B817",
    modeName: "tram",
    status: "Minor Delays",
  },
];

function groupByMode(lines: Line[]): Record<string, Line[]> {
  return lines.reduce((groups, line) => {
    if (!groups[line.modeName]) {
      groups[line.modeName] = [];
    }
    groups[line.modeName].push(line);
    return groups;
  }, {} as Record<string, Line[]>);
}

function LineList() {
  const grouped = groupByMode(dummyLines);

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
              <div
                key={line.id}
                className="p-3 rounded-md text-white"
                style={{
                  backgroundColor: line.color,
                }}
              >
                <strong>{line.name}</strong> — {line.status}
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}

export default LineList;
