import { getUrls, shortenUrl, deleteUrl } from "./urlClient"
import httpClient from "./httpClient"

jest.mock("./httpClient")
const mockedHttpClient = httpClient as jest.Mocked<typeof httpClient>

describe("urlClient", () => {
  const mockUrls = [{ fullUrl: "https://test.com", alias: "test", shortenUrl: "http://localhost:8080/url/test" }]

  afterEach(() => {
    jest.clearAllMocks() // Resets call counts between tests
  })

  describe("getUrls", () => {
    it("should return data on success", async () => {
      mockedHttpClient.get.mockResolvedValue({ data: mockUrls })

      const result = await getUrls()

      expect(mockedHttpClient.get).toHaveBeenCalledWith("/url")
      expect(result).toEqual(mockUrls)
    })

    it("should return ErrorResponse on catch", async () => {
      const errorMessage = "Network Error"
      mockedHttpClient.get.mockRejectedValue(new Error(errorMessage))

      const result = await getUrls()

      expect(result).toEqual({ message: errorMessage })
    })
  })

  describe("shortenUrl", () => {
    it("should post data and return the new UrlItem", async () => {
      const newUrl = { id: "2", fullUrl: "https://new.com", alias: "http://localhost:8080/url/new" }
      mockedHttpClient.post.mockResolvedValue(newUrl)

      const result = await shortenUrl("https://new.com", "new")

      expect(mockedHttpClient.post).toHaveBeenCalledWith("/url", {
        fullUrl: "https://new.com",
        CustomAlias: "new",
      })
      expect(result).toEqual(newUrl)
    })
  })

  describe("deleteUrl", () => {
    it("should call delete with the correct alias", async () => {
      mockedHttpClient.delete.mockResolvedValue({})

      await deleteUrl("test-alias")

      expect(mockedHttpClient.delete).toHaveBeenCalledWith("/url/test-alias")
    })
  })
})
