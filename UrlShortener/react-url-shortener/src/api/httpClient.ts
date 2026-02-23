import { handleRequestError, handleResponseError } from "../services/errorService"
import axios, { type AxiosResponse, type AxiosRequestConfig } from "axios"
import { appConfig } from '../app.config'

export interface CustomAxiosRequestConfig<D = any> extends AxiosRequestConfig<D> {  }

// request interceptor
axios.interceptors.request.use(
	(config) => {

        // here you would get authentication values

		config.baseURL = appConfig.axiosBaseUrl

		return config
	}, handleRequestError)

// response interceptor
axios.interceptors.response.use(response => {

	// here you would potentially update access tokens
    
	return response
}, handleResponseError)

const get = <T = any, R = AxiosResponse<T>, D = any>(url: string, config?: CustomAxiosRequestConfig<D>): Promise<R> => axios.get<T,R,D>(url, config)
const deleteCall = <T = any, R = AxiosResponse<T>, D = any>(url: string, config?: CustomAxiosRequestConfig<D>): Promise<R> => axios.delete<T,R,D>(url, config)

export default {
	get,
	delete: deleteCall,
	post: axios.post,
	put: axios.put,
	patch: axios.patch
}