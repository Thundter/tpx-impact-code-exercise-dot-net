import React from 'react'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { UrlList } from './UrlList'
import { Urls, Delete } from '../services/urlService'
import { isErrorResponse } from '../services/errorService'
import '@testing-library/jest-dom'

jest.mock('../services/urlService')
jest.mock('../services/errorService')

const mockedUrls = Urls as jest.MockedFunction<typeof Urls>
const mockedDelete = Delete as jest.MockedFunction<typeof Delete>
const mockedIsErrorResponse = isErrorResponse as jest.MockedFunction<typeof isErrorResponse>

describe('UrlList Component', () => {
    const mockData = [
        { alias: 'banana', fullUrl: 'https://banana.com', shortUrl: 'http://localhost:8080/url/banana' }
    ]

    beforeEach(() => {
        jest.clearAllMocks()
    })

    it('shows loading state initially and then renders list', async () => {
        mockedUrls.mockResolvedValue(mockData)
        mockedIsErrorResponse.mockReturnValue(false)

        render(<UrlList refreshCount={0} />)

        // Check loading
        expect(screen.getByText(/isLoading Urls.../i)).toBeInTheDocument()

        // Wait for data to appear
        await waitFor(() => {
            expect(screen.getByText('banana')).toBeInTheDocument()
            expect(screen.getByText('https://banana.com')).toBeInTheDocument()
            expect(screen.getByText('http://localhost:8080/url/banana')).toBeInTheDocument()
        })
    })

    it('displays error message when API fails', async () => {
        const errorMsg = "Failed to fetch URLs"
        mockedUrls.mockResolvedValue({ message: errorMsg })
        mockedIsErrorResponse.mockReturnValue(true)

        render(<UrlList refreshCount={0} />)

        await waitFor(() => {
            expect(screen.getByText(errorMsg)).toBeInTheDocument()
        })
    })

    it('handles the delete confirmation flow', async () => {
        mockedUrls.mockResolvedValue(mockData)
        mockedIsErrorResponse.mockReturnValue(false)
        mockedDelete.mockResolvedValue(undefined)

        render(<UrlList refreshCount={0} />)

        // Wait for list to load and click Delete
        const deleteBtn = await screen.findByText('Delete')
        fireEvent.click(deleteBtn)

        // Check if Confirmation buttons appear
        expect(screen.getByText('Yes, Confirm Delete!')).toBeInTheDocument()
        const cancelBtn = screen.getByText('Cancel')

        // Click Confirm
        const confirmBtn = screen.getByText('Yes, Confirm Delete!')
        fireEvent.click(confirmBtn)

        // Verify service call
        await waitFor(() => {
            expect(mockedDelete).toHaveBeenCalledWith('banana')
        })
    })

    it('handles the cancel confirmation flow', async () => {
        mockedUrls.mockResolvedValue(mockData);
        mockedIsErrorResponse.mockReturnValue(false);

        render(<UrlList refreshCount={0} />);

        // Wait for list to load and click Delete
        const deleteBtn = await screen.findByText('Delete');
        fireEvent.click(deleteBtn);

        // Check if Confirmation buttons appear
        expect(screen.getByText('Yes, Confirm Delete!')).toBeInTheDocument();
        const cancelBtn = screen.getByText('Cancel');

        // Click Cancel
        fireEvent.click(cancelBtn);

        // Verify the UI reverted to the original state
        expect(screen.queryByText('Yes, Confirm Delete!')).not.toBeInTheDocument();
        expect(screen.getByText('Delete')).toBeInTheDocument();

        // Ensure the service was not called
        expect(mockedDelete).not.toHaveBeenCalled();
    });

    it('re-fetches data when refreshCount changes', async () => {
        mockedUrls.mockResolvedValue(mockData)
        mockedIsErrorResponse.mockReturnValue(false)

        // note 1st instance to load form
        const { rerender } = render(<UrlList refreshCount={1} />)

        // 2nd instance to reload the form
        await waitFor(() => expect(mockedUrls).toHaveBeenCalledTimes(2))

        // Change prop to trigger useEffect
        rerender(<UrlList refreshCount={2} />)

        // 3rdd instance to reload the form again
        await waitFor(() => expect(mockedUrls).toHaveBeenCalledTimes(3))
    })
})
