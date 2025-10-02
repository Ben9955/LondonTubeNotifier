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
    .configureLogging(signalR.LogLevel.Information)
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

// Global updates for authenticated users (toasters)
export const onLineUpdate = (callback: (data: any) => void) => {
  if (!connection) throw new Error("SignalR not started yet");
  connection.on("ReceiveLineUpdate", callback);
};

// Page-specific updates
export const onFullStatus = (callback: (data: any) => void) => {
  if (!connection) throw new Error("SignalR not started yet");
  connection.on("ReceiveFullStatusUpdate", callback);
};

// import * as signalR from "@microsoft/signalr";

// let connection: signalR.HubConnection | null = null;

// import { getAccessToken } from "./authService";

// export const startConnection = async (): Promise<signalR.HubConnection> => {
//   const token = getAccessToken();

//   if (connection) return connection;

//   connection = new signalR.HubConnectionBuilder()
//     .withUrl("https://localhost:7284/TflLiveHub", {
//       accessTokenFactory: () => token || "",
//       // withCredentials: !!token,
//     })
//     .withAutomaticReconnect()
//     .configureLogging(signalR.LogLevel.Information)
//     .build();

//   connection.onclose(async () => {
//     const stillAuthenticated = !!getAccessToken();
//     if (!stillAuthenticated) {
//       console.log("❌ Not reconnecting SignalR — user is logged out.");
//       return;
//     }

//     console.log("🔁 Reconnecting SignalR...");
//     await startConnection();
//   });

//   await waitForConnectionStart(connection);
//   return connection;
// };

// // Helper to wait until connection is fully connected
// const waitForConnectionStart = async (conn: signalR.HubConnection) => {
//   if (conn.state === signalR.HubConnectionState.Connected) return;

//   return new Promise<void>((resolve, reject) => {
//     const start = async () => {
//       try {
//         await conn.start();
//         console.log("✅ SignalR Connected.");
//         resolve();
//       } catch (err) {
//         console.error("❌ SignalR start failed, retrying...", err);
//         setTimeout(start, 1000); // retry
//       }
//     };
//     start();
//   });
// };

// export const onFullStatus = (callback: (data: any) => void) => {
//   if (!connection) throw new Error("SignalR not started yet");
//   connection.on("ReceiveFullStatusUpdate", callback);
// };

// export const onLineUpdate = (callback: (data: any) => void) => {
//   if (!connection) throw new Error("SignalR not started yet");
//   connection.on("ReceiveLineUpdate", callback);
// };
