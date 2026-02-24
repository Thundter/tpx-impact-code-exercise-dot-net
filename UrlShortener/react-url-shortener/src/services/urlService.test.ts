import * as urlService from "./urlService"
import * as urlClient from "../api/urlClient"
import { isErrorResponse } from "./errorService"

// Mock the dependencies
jest.mock("../api/urlClient")
jest.mock("./errorService")

const mockedUrlClient = urlClient as jest.Mocked<typeof urlClient>
const mockedIsErrorResponse = isErrorResponse as jest.MockedFunction<
  typeof isErrorResponse
>

describe("UrlService", () => {
  afterEach(() => {
    jest.clearAllMocks()
  })

  const fullUrl = "https://banana.com"
  const alias = "banana"

  describe("Urls", () => {
    it("should return UrlItem[] when client succeeds and it is not an error response", async () => {
      const mockData = [
        {
          fullUrl: "http://banana.com/apple/peach",
          alias: "test",
          shortUrl: "http://localhost:8080/url/test",
        },
      ]
      mockedUrlClient.getUrls.mockResolvedValue(mockData)
      mockedIsErrorResponse.mockReturnValue(false)

      const result = await urlService.Urls()

      expect(result).toEqual(mockData)
    })

    it("should return ErrorResponse if isErrorResponse returns true", async () => {
      const mockError = { message: "API Error" }
      mockedUrlClient.getUrls.mockResolvedValue(mockError)
      mockedIsErrorResponse.mockReturnValue(true)

      const result = await urlService.Urls()

      expect(result).toEqual(mockError)
    })

    it("should handle generic catch block errors", async () => {
      mockedUrlClient.getUrls.mockRejectedValue(new Error("Crash"))
      const result = await urlService.Urls()
      expect(result).toEqual({ message: "Crash" })
    })
  })

  describe("Shorten", () => {
    it("should return the shortened URL item on success", async () => {
      const mockItem = { fullUrl: "a", alias: "b", shortUrl: "c" }
      mockedUrlClient.shortenUrl.mockResolvedValue(mockItem)
      mockedIsErrorResponse.mockReturnValue(false)

      const result = await urlService.Shorten("a", "b")

      expect(result).toEqual(mockItem)
    })

    it("should return ErrorResponse if isErrorResponse returns true", async () => {
      const mockError = { message: "API Error" }
      mockedUrlClient.getUrls.mockResolvedValue(mockError)
      mockedIsErrorResponse.mockReturnValue(true)

      const result = await urlService.Urls()

      expect(result).toEqual(mockError)
    })

    // API returns an error object (e.g., 200 OK but with error payload)
    it("should return ErrorResponse when isErrorResponse identifies a logical error", async () => {
      const mockErrorResponse = { message: "Alias already exists" }

      mockedUrlClient.shortenUrl.mockResolvedValue(mockErrorResponse as any)
      mockedIsErrorResponse.mockReturnValue(true)

      const result = await urlService.Shorten(fullUrl, alias)

      expect(result).toEqual(mockErrorResponse)
      expect(isErrorResponse).toHaveBeenCalledWith(mockErrorResponse)
    })

    // API throws a hard exception (e.g., 500 Internal Server Error)
    it("should return ErrorResponse when the client throws an exception", async () => {
      const exceptionMessage = "Network Failure"
      mockedUrlClient.shortenUrl.mockRejectedValue(new Error(exceptionMessage))

      const result = await urlService.Shorten(fullUrl, alias)

      expect(result).toEqual({ message: exceptionMessage })
    })

    // Non-Error object thrown (Edge Case)
    it("should return empty ErrorResponse when a non-Error is thrown", async () => {
      mockedUrlClient.shortenUrl.mockRejectedValue("Unexpected String Throw")

      const result = await urlService.Shorten(fullUrl, alias)

      expect(result).toEqual({})
    })
  })

  describe("Delete", () => {
    it("should call deleteUrl and return undefined on success", async () => {
      mockedUrlClient.deleteUrl.mockResolvedValue(undefined)
      mockedIsErrorResponse.mockReturnValue(false)

      const result = await urlService.Delete("test-alias")

      expect(mockedUrlClient.deleteUrl).toHaveBeenCalledWith("test-alias")
      expect(result).toBeUndefined()
    })

    it("should return an ErrorResponse if deletion fails", async () => {
      const mockError = { message: "failed deletion" }
      mockedUrlClient.deleteUrl.mockRejectedValue(new Error(mockError.message))

      const result = await urlService.Delete("test-alias")

      expect(result).toEqual(mockError)
    })

    it("should return undefined (void) on a successful delete", async () => {
      mockedUrlClient.deleteUrl.mockResolvedValue(undefined)
      mockedIsErrorResponse.mockReturnValue(false)

      const result = await urlService.Delete(alias)

      expect(mockedUrlClient.deleteUrl).toHaveBeenCalledWith(alias)
      expect(result).toBeUndefined()
    })

    it("should return ErrorResponse when the API returns a logical error object", async () => {
      const mockError = { message: "Cannot delete protected alias" }

      mockedUrlClient.deleteUrl.mockResolvedValue(mockError as any)
      mockedIsErrorResponse.mockReturnValue(true)

      const result = await urlService.Delete(alias)

      expect(result).toEqual(mockError)
      expect(mockedIsErrorResponse).toHaveBeenCalledWith(mockError)
    })

    it("should return ErrorResponse when the client throws an exception", async () => {
      const errorMessage = "Internal Server Error"
      mockedUrlClient.deleteUrl.mockRejectedValue(new Error(errorMessage))

      const result = await urlService.Delete(alias)

      expect(result).toEqual({ message: errorMessage })
    })

    it("should return an empty ErrorResponse when a non-Error object is thrown", async () => {
      mockedUrlClient.deleteUrl.mockRejectedValue(null)

      const result = await urlService.Delete(alias)

      expect(result).toEqual({})
    })
  })
})
