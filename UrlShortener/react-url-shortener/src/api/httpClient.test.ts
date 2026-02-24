import axios from "axios"
import httpClient from "./httpClient"

jest.mock("axios")
const mockedAxios = axios as jest.Mocked<typeof axios>

describe("httpClient wrapper", () => {
  const testUrl = "/test-endpoint"
  const testData = { id: 1, name: "Test Item" }

  afterEach(() => {
    jest.clearAllMocks()
  })

  describe("get()", () => {
    it("should call axios.get with the correct parameters", async () => {
      mockedAxios.get.mockResolvedValue({ data: testData })

      const response = await httpClient.get(testUrl)

      expect(mockedAxios.get).toHaveBeenCalledWith(testUrl, undefined)
      expect(response.data).toEqual(testData)
    })
  })

  describe("delete()", () => {
    it("should call axios.delete (aliased as deleteCall) correctly", async () => {
      mockedAxios.delete.mockResolvedValue({ data: { success: true } })

      const response = await httpClient.delete(testUrl)

      expect(mockedAxios.delete).toHaveBeenCalledWith(testUrl, undefined)
      expect(response.data).toEqual({ success: true })
    })
  })

  describe("Other methods (post, put, patch)", () => {
    it("should export standard axios methods directly", async () => {
      mockedAxios.post.mockResolvedValue({ status: 201 })
      
      const response = await httpClient.post(testUrl, { body: "data" })

      expect(mockedAxios.post).toHaveBeenCalledWith(testUrl, { body: "data" })
      expect(response.status).toBe(201)
    })
  })
})
