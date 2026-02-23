import React, { useEffect, useState } from 'react'
import type { UrlItem } from '../types/UrlItem'
import { Urls, Delete } from '../services/urlService'
import type { ErrorResponse } from '../types/ErrorResponse'
import { isErrorResponse } from '../services/errorService'

interface UrlListProps {
  refreshCount: number
}

export const UrlList = ({ refreshCount }: UrlListProps) => {
  const [urls, setUrls] = useState<UrlItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorText, setErrorText] = useState<string | null>(null)
  const [deleteConfirmAlias, setDeleteConfirmAlias] = useState<string | null>(null)

  useEffect(() => {
    console.log("UrlList refresh", refreshCount)
    fetchData()
  }, [])

  useEffect(() => {
    console.log("UrlList refresh", refreshCount)
    fetchData()
  }, [refreshCount])

  const fetchData = async () => {
    console.log("UrlList:fetchData start")

    setIsLoading(true)

    var response = await Urls()

    if (isErrorResponse(response)) {
      handleError(response as ErrorResponse)
      return
    }

    setUrls(response as UrlItem[])

    setIsLoading(false)
  }

  var handleError = (response: ErrorResponse) => {
    setErrorText(response.message)
  }

  var handleConfirmDelete = async (alias: string) => {
    setIsLoading(true)

    console.log("UrlList confirmDelete")

    var response = await Delete(alias)

    if (isErrorResponse(response)) {
      setErrorText(response.message)
      setIsLoading(false)
      return
    }

    setIsLoading(false)

    await fetchData()
  }

  if (isLoading) return <p>isLoading Urls...</p>

  return (
    <div className="url-list-container">
      <div className='error-text'>
        {errorText}
      </div>
      {urls.map((item) => (
        <div className="url-card" key={item.alias}>
          <div className="url-details">
            <div className="margin-div">
              <span className="alias-badge">{item.alias}</span>
              {deleteConfirmAlias === item.alias ?
                <>
                  <button className="btn-confirm-delete"
                    onClick={() => handleConfirmDelete(item.alias)}
                    disabled={isLoading}>Yes, Confirm Delete!</button>
                  &nbsp;&nbsp;&nbsp;  
                  <button className="btn-cancel-delete"
                    onClick={() => setDeleteConfirmAlias(null)}
                    disabled={isLoading}>Cancel</button>
                </>
                : 
                <button className="btn-delete"
                  onClick={() => setDeleteConfirmAlias(item.alias)}
                  disabled={isLoading}>Delete</button>
              }
            </div>
            <div className="margin-div">
              <a href={item.fullUrl} target="_blank" rel="noopener noreferrer" className="full-url">
                {item.fullUrl}
              </a>
            </div>
            <div className="short-url-div">
              <p className="short-url">
                <strong>Short Link:</strong> {item.shortUrl}
              </p>
            </div>
          </div>
        </div>
      ))
      }
      <p className='refresh-count'>{refreshCount}</p>
    </div >
  )
}
