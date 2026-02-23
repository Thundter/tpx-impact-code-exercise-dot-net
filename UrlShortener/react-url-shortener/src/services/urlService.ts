import { getUrl, getUrls, shortenUrl, deleteUrl } from "../api/urlClient"
import type { UrlItem } from "../types/UrlItem"
import type { ErrorResponse } from "../types/ErrorResponse"
import { isErrorResponse } from "./errorService"

export const Url = async (alias: string): Promise<UrlItem | ErrorResponse> => {
  try {
    var response = await getUrl(alias)

    console.log("UrlService:Url", response)

    return {} as UrlItem
  } catch (error: any) {
    console.log("UrlService:Url:Error", error)
    return { message: error.message } as ErrorResponse
  }
}

export const Urls = async (): Promise<UrlItem[] | ErrorResponse> => {
  try {
    var response = await getUrls()

    if (isErrorResponse(response)) {
      console.log("UrlService:Urls:isErrorResponse", response)
      return response as ErrorResponse
    }

    return response
  } catch (error: any) {
    console.log("UrlService:Urls:Error", error)
    if (error instanceof Error) {
        return { message: error.message } as ErrorResponse
    } else {
        return {} as ErrorResponse
    }
  }
}

export const Shorten = async (
  fullUrl: string,
  alias: string
): Promise<UrlItem | ErrorResponse> => {
  try {
    var response = await shortenUrl(fullUrl, alias)

    console.log("UrlService:Shorten", response)

    return response
  } catch (error: any) {
    console.log("UrlService:Shorten:Error", error)
    return { message: error.message } as ErrorResponse
  }
}

export const Delete = async (alias: string): Promise<void | ErrorResponse> => {
  try {
    var response = await deleteUrl(alias)

    console.log("UrlService:Delete", response)

    return response
  } catch (error: any) {
    console.log("UrlService:Delete:Error", error)
    return { message: error.message } as ErrorResponse
  }
}
