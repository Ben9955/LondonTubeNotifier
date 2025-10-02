import type { StatusDescription } from "./StatusDescription";

export type Line = {
  id: string;
  name: string;
  color: string;
  modeName: string;
  statusDescriptions: StatusDescription[];
};
