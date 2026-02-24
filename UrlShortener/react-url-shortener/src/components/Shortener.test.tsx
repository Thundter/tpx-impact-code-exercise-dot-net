import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { Shortener } from './Shortener';
import { Shorten } from '../services/urlService';
import { isErrorResponse } from '../services/errorService';
import '@testing-library/jest-dom';

// 1. Mock the services
jest.mock('../services/urlService');
jest.mock('../services/errorService');

const mockedShorten = Shorten as jest.MockedFunction<typeof Shorten>;
const mockedIsErrorResponse = isErrorResponse as jest.MockedFunction<typeof isErrorResponse>;

describe('Shortener Component', () => {
  const mockOnRefresh = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('updates input values on change', () => {
    render(<Shortener onRefresh={mockOnRefresh} />);
    
    const urlInput = screen.getByLabelText(/Full Url/i) as HTMLInputElement;
    const aliasInput = screen.getByLabelText(/Alias/i) as HTMLInputElement;

    fireEvent.change(urlInput, { target: { value: 'https://banana.com' } });
    fireEvent.change(aliasInput, { target: { value: 'banana' } });

    expect(urlInput.value).toBe('https://banana.com');
    expect(aliasInput.value).toBe('banana');
  });

  it('shows validation error if Full Url is empty', async () => {
    render(<Shortener onRefresh={mockOnRefresh} />);
    
    const button = screen.getByRole('button', { name: /Shorten/i });
    fireEvent.click(button);

    expect(await screen.findByText(/full url input has no value/i)).toBeInTheDocument();
    expect(mockedShorten).not.toHaveBeenCalled();
  });

  it('calls onRefresh and clears inputs on success', async () => {
    mockedShorten.mockResolvedValue({ alias: 'abc', fullUrl: '...', shortUrl: '...' });
    mockedIsErrorResponse.mockReturnValue(false);

    render(<Shortener onRefresh={mockOnRefresh} />);
    
    const urlInput = screen.getByLabelText(/Full Url/i);
    const aliasInput = screen.getByLabelText(/Alias/i);
    const button = screen.getByRole('button', { name: /Shorten/i });

    fireEvent.change(urlInput, { target: { value: 'https://banana.com' } });
    fireEvent.change(aliasInput, { target: { value: 'banana' } });
    fireEvent.click(button);

    // Assertions
    await waitFor(() => {
      expect(mockedShorten).toHaveBeenCalledWith('https://banana.com', 'banana');
      expect(mockOnRefresh).toHaveBeenCalled();
    });

    // Check if inputs are cleared
    expect((urlInput as HTMLInputElement).value).toBe("");
    expect((aliasInput as HTMLInputElement).value).toBe("");
  });

  it('displays API error message when request fails', async () => {
    const apiError = "Alias already taken";
    mockedShorten.mockResolvedValue({ message: apiError });
    mockedIsErrorResponse.mockReturnValue(true);

    render(<Shortener onRefresh={mockOnRefresh} />);
    
    fireEvent.change(screen.getByLabelText(/Full Url/i), { target: { value: 'https://banana.com' } });
    fireEvent.click(screen.getByRole('button', { name: /Shorten/i }));

    expect(await screen.findByText(apiError)).toBeInTheDocument();
  });

  it('disables the shorten button while loading', async () => {
    mockedShorten.mockReturnValue(new Promise(() => {})); 
  
    render(<Shortener onRefresh={jest.fn()} />);
    
    const urlInput = screen.getByLabelText(/Full Url/i);
    const button = screen.getByRole('button', { name: /Shorten/i });
  
    fireEvent.change(urlInput, { target: { value: 'https://banana.com' } });
    fireEvent.click(button);
  
    expect(button).toBeDisabled();
  });

  it('clears existing error messages when a new valid request starts', async () => {
    render(<Shortener onRefresh={jest.fn()} />);
    const button = screen.getByRole('button', { name: /Shorten/i });
  
    // 1. Trigger validation error
    fireEvent.click(button);
    expect(screen.getByText(/full url input has no value/i)).toBeInTheDocument();
  
    // 2. Fill it in and try again
    fireEvent.change(screen.getByLabelText(/Full Url/i), { target: { value: 'https://banana.com' } });
    fireEvent.click(button);
  
    // 3. The error should be gone (queryByText returns null if not found)
    expect(screen.queryByText(/full url input has no value/i)).not.toBeInTheDocument();
  });
  
  it('allows shortening without a custom alias', async () => {
    mockedShorten.mockResolvedValue({ alias: 'auto-gen', fullUrl: '...', shortUrl: '...' });
    mockedIsErrorResponse.mockReturnValue(false);
  
    render(<Shortener onRefresh={jest.fn()} />);
    
    fireEvent.change(screen.getByLabelText(/Full Url/i), { target: { value: 'https://banana.com' } });
    fireEvent.click(screen.getByRole('button', { name: /Shorten/i }));
  
    // Ensure it was called with the URL and an EMPTY string for alias
    expect(mockedShorten).toHaveBeenCalledWith('https://banana.com', '');
  });
  
  it('fails validation if alias contains special characters', async () => {
    render(<Shortener onRefresh={jest.fn()} />);
    
    const urlInput = screen.getByLabelText(/Full Url/i);
    const aliasInput = screen.getByLabelText(/Alias/i);
    const button = screen.getByRole('button', { name: /Shorten/i });
  
    // Input an invalid alias
    fireEvent.change(urlInput, { target: { value: 'https://banana.com' } });
    fireEvent.change(aliasInput, { target: { value: 'invalid@alias!' } });
    fireEvent.click(button);
  
    // Assertions
    expect(await screen.findByText(/alias can only contain letters, numbers, and hyphens/i)).toBeInTheDocument();
    expect(mockedShorten).not.toHaveBeenCalled();
  });
  
});
