import { getUrls, shortenUrl, deleteUrl } from "../api/urlClient"
import type { UrlItem } from "../types/UrlItem"
import type { ErrorResponse } from "../types/ErrorResponse"
import { isErrorResponse } from "./errorService"
import { AxiosError } from "axios"

export const Urls = async (): Promise<UrlItem[] | ErrorResponse> => {
  try {
    var response = await getUrls()

    if (isErrorResponse(response)) {
      console.log("UrlService:Urls:isErrorResponse", response)
      return response as ErrorResponse
    }

    return response
  } catch (error) {
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

    if (isErrorResponse(response)) {
      console.log("UrlService:Shorten:isErrorResponse", response)
      return response as ErrorResponse
    }

    console.log("UrlService:Shorten", response)

    return response
  } catch (error) {
    if (error instanceof AxiosError ) {
      var axiosError = error as AxiosError
      console.log("UrlService:Shorten:AxiosError", axiosError, axiosError.cause, axiosError.code, axiosError.status)
      if (axiosError.status === 400) {
        return { message: "Invalid input or alias already taken" } as ErrorResponse
      }
      return { message: axiosError.message } as ErrorResponse
    } else if (error instanceof Error ) {
      console.log("UrlService:Shorten:Error", error)
      return { message: error.message } as ErrorResponse
    } else {
      console.log("UrlService:Shorten:UnknownError", error)
      return { message: "Unknown error" } as ErrorResponse
    }
  }
}

export const Delete = async (alias: string): Promise<void | ErrorResponse> => {
  try {
    var response = await deleteUrl(alias)

    if (isErrorResponse(response)) {
      console.log("UrlService:Delete:isErrorResponse", response)
      return response as ErrorResponse
    }

    console.log("UrlService:Delete", response)

    return response
  } catch (error) {
    console.log("UrlService:Delete:Error", error)
    if (error instanceof AxiosError ) {
      var axiosError = error as AxiosError
      console.log("UrlService:Shorten:AxiosError", axiosError, axiosError.cause, axiosError.code, axiosError.status)
      if (axiosError.status === 404) {
        return { message: "Alias not found" } as ErrorResponse
      }
      return { message: axiosError.message } as ErrorResponse
    } else if (error instanceof Error) {
      console.log("UrlService:Shorten:Error", error)
      return { message: error.message } as ErrorResponse
    } else {
      console.log("UrlService:Shorten:UnknownError", error)
      return { message: "Unknown error" } as ErrorResponse
    }
  }
}
