import * as signalR from "@microsoft/signalr";
import { getAccessToken } from "./apiClient";

let connection: signalR.HubConnection | null = null;

export const startConnection = async (): Promise<signalR.HubConnection> => {
  if (connection) return connection;

  connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7284/TflLiveHub", {
      accessTokenFactory: () => getAccessToken() || "",
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Trace)
    .build();

  connection.onclose(() => {
    console.log("SignalR connection closed");
  });

  await waitForConnectionStart(connection);
  return connection;
};

const waitForConnectionStart = async (conn: signalR.HubConnection) => {
  if (conn.state === signalR.HubConnectionState.Connected) return;
  return new Promise<void>((resolve) => {
    const start = async () => {
      try {
        await conn.start();
        console.log("✅ SignalR Connected.");
        resolve();
      } catch {
        console.log("❌ SignalR start failed, retrying...");
        setTimeout(start, 1000);
      }
    };
    start();
  });
};

// Updates for authenticated users (toasters)
export const onLineUpdate = (callback: (data: any) => void) => {
  if (!connection) throw new Error("SignalR not started yet");
  connection.on("ReceiveLineUpdate", callback);
};

export const offLineUpdate = () => {
  if (!connection) return;
  connection.off("ReceiveLineUpdate");
};

// Page-specific updates
export const onFullStatus = (callback: (data: any) => void) => {
  if (!connection) throw new Error("SignalR not started yet");
  connection.on("ReceiveFullStatusUpdate", callback);
};

export const stopConnection = async () => {
  if (connection && connection.state === signalR.HubConnectionState.Connected) {
    await connection.stop();
    console.log("SignalR connection stopped");
    connection = null;
  }
};
