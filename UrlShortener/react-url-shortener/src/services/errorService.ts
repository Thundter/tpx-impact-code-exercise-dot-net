import type { ErrorResponse } from "../types/ErrorResponse";

export const isErrorResponse =(response: any): response is ErrorResponse => {
    return response && typeof response.message === 'string';
}

export const handleRequestError = () => {
    // only needed for auth stuff 
}

export const handleResponseError = () => {
    // only needed for auth stuff
}
