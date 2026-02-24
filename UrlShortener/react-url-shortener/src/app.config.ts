import { type AppConfig } from "./types/config"

const isProd = process.env.NODE_ENV === "production"

export const appConfig: AppConfig = {
  axiosBaseUrl: isProd ? "https://unknown.com" : "http://localhost:5284",
  // add correct prod variable if needed 
}
