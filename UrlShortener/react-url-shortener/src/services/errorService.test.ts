import { isErrorResponse } from './errorService'

describe('isErrorResponse', () => {
  it('should return true if the object has a message string', () => {
    const validError = { message: 'Something went wrong' }
    expect(isErrorResponse(validError)).toBe(true)
  })

  it.each([
    [null],
    [undefined],
    [0],
    [''],
    [false]
  ])('should return falsy value: %p', (val) => {
    expect(isErrorResponse(val)).toBeFalsy()
  })

  it('should return false if message is missing', () => {
    const wrongData = { code: 500 }
    expect(isErrorResponse(wrongData)).toBe(false)
  })

  it('should return false if message is not a string', () => {
    const wrongData = { message: 12345 }
    expect(isErrorResponse(wrongData)).toBe(false)
  })

  it('should return false for an empty object', () => {
    expect(isErrorResponse({})).toBe(false)
  })

  // 4. Edge case: Array
  it('should return false if an array is passed', () => {
    expect(isErrorResponse([])).toBe(false)
  })
})
