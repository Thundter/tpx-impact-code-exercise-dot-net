import httpClient from "./httpClient"
import type { UrlItem } from "../types/UrlItem"
import type { ErrorResponse } from "../types/ErrorResponse"

export const getUrls = async (): Promise<UrlItem[] | ErrorResponse> => {
  try {
    var response = await httpClient.get<UrlItem[]>("/url")
    console.log("ulrClient:getUrls:response=>", response)
    return response.data
  } catch (error) {
    console.log("ulrClient:getUrls:error=>", error)
    if (error instanceof Error) {
      return {message: error.message} as ErrorResponse
    }
    return {} as ErrorResponse
  }
}
  
export const getUrl = async (alias: string): Promise<UrlItem[] > =>
  (await httpClient.get<UrlItem[]>("/url")).data

export const shortenUrl = async (fullUrl: string, alias: string): Promise<UrlItem> =>
  await httpClient.post("/url", { fullUrl, CustomAlias:alias})

export const deleteUrl = async (alias: string): Promise<void> =>
  await httpClient.delete(`/url/${alias}`)
