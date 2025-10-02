import toast from "react-hot-toast";

export const showLineUpdateToast = (lineUpdate: any) => {
  const status = lineUpdate.statusDescriptions[0];
  const severityColor =
    status.statusSeverity >= 10
      ? "#52c41a"
      : status.statusSeverity >= 5
      ? "#faad14"
      : "#ff4d4f";

  const emoji =
    status.statusSeverity >= 10
      ? "✅"
      : status.statusSeverity >= 5
      ? "⚠️"
      : "🚨";

  toast.custom(
    (t) => (
      <div
        style={{
          background: severityColor,
          color: "#fff",
          padding: "16px 24px",
          borderRadius: "8px",
          boxShadow: "0 4px 8px rgba(0,0,0,0.2)",
          fontWeight: "bold",
          display: "flex",
          flexDirection: "column",
          minWidth: "300px",
          top: "10vh",
          zIndex: 9999,
        }}
      >
        {lineUpdate.statusDescriptions.map((status: any, idx: number) => (
          <div key={idx}>
            {emoji} {lineUpdate.lineName}: {status.statusDescription}
          </div>
        ))}
        <div style={{ fontSize: "0.75rem", marginTop: "4px", opacity: 0.8 }}>
          More info in your subscriptions
        </div>
      </div>
    ),
    { position: "top-center", duration: 5000 }
  );
};
