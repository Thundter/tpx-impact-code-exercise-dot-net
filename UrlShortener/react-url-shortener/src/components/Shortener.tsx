import { useState } from "react"
import { Shorten } from "../services/urlService"
import { isErrorResponse } from "../services/errorService"

interface ShortenerProps {
    onRefresh: () => void;
}

export const Shortener = ({ onRefresh }: ShortenerProps) => {

    const [inputFullUrl, setInputFullUrl] = useState("")
    const [inputAlias, setInputAlias] = useState("")
    const [errorText, setErrorText] = useState<string | null>(null)
    const [isLoading, setIsLoading] = useState(false)

    const handleFullUrlChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInputFullUrl(e.target.value)
    }
    const handleAliasChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setInputAlias(e.target.value)
    }

    const handleShorten = async () => {
        setIsLoading(true)
        if (!inputFullUrl) {
            setErrorText("full url input has no value")
            setIsLoading(false)
            return
        }

        // special char validation for alias
        const aliasRegex = /^[a-zA-Z0-9-]*$/
        if (!aliasRegex.test(inputAlias)) {
            setErrorText("alias can only contain letters, numbers, and hyphens")
            setIsLoading(false)
            return
        }

        setErrorText(null)

        var response = await Shorten(inputFullUrl, inputAlias)

        if (isErrorResponse(response)) {
            console.log("Shortener:handleShorten:error=>", response.message)
            setErrorText(response.message)
            return
        }

        console.log("Shortener:handleShorten:response=>", response)

        setInputFullUrl("")
        setInputAlias("")
        setIsLoading(false)
        onRefresh()
    }

    return (
        <div className="url-shortener-card">
            <div>
                <label htmlFor="fullUrl">Full Url</label>
                <input
                    id="fullUrl"
                    type="text"
                    value={inputFullUrl}
                    onChange={handleFullUrlChange}
                    className="full-url-input"
                />
            </div>
            <div className="alias-div">
                <label htmlFor="alias">Alias</label>
                <input
                    id="alias"
                    type="text"
                    value={inputAlias}
                    onChange={handleAliasChange}
                    className="alias-input"
                />
            </div>
            <div>
                <button className="btn-shorten"
                    onClick={handleShorten}
                    disabled={isLoading}>Shorten</button>
            </div>
            <p className="input-error">{errorText}</p>
        </div>
    )
}